using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StudentDocVault.Areas.Identity.Data;
using StudentDocVault.Data;
using StudentDocVault.Models;

namespace StudentDocVault.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContextContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentController(ApplicationDbContextContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Document
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return _context.Document != null ? 
                          View(await _context.Document.Where(d => d.StudentId == user.StudentId).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContextContext.Document'  is null.");
        }

        // GET: Document/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Document == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Document/Create
        public async Task<IActionResult> Create()
        { 
            var user = await _userManager.GetUserAsync(User);
            ViewData["StudentId"] = user.StudentId;
            return View();
        }

        // POST: Document/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,DocumentTitle,DocumentDescription,Url")] Document document,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string name = document.StudentId.ToString();
                    var studentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UploadedDocs", name);
                    if (!Directory.Exists(studentFolder))
                    {
                        Directory.CreateDirectory(studentFolder);
                    }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string docPath = Path.Combine(studentFolder, fileName);

                    using (var fileStream = new FileStream(docPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    string relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, docPath);
                    relativePath = relativePath.Replace("\\", "/");
                    document.Url = relativePath;
                }
                document.UploadedDate= DateTime.Now;
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (file==null)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["StudentId"] = user.StudentId;
                ModelState.AddModelError(string.Empty, "Please Select a File to Upload");
            }
            return View(document);
        }

        // GET: Document/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Document == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["StudentId"] = user.StudentId;
            var document = await _context.Document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Document/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,DocumentTitle,DocumentDescription,Url")] Document document,IFormFile file)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        string name = document.StudentId.ToString();
                        var studentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UploadedDocs", name);
                        if (!Directory.Exists(studentFolder))
                        {
                            Directory.CreateDirectory(studentFolder);
                        }
                        if (!string.IsNullOrEmpty(document.Url))
                        {
                            var check = _context.Document.Find(id);
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, document.Url);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string docPath = Path.Combine(studentFolder, fileName);

                        using (var fileStream = new FileStream(docPath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        string relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, docPath);
                        relativePath = relativePath.Replace("\\", "/");
                        document.Url = relativePath;
                        
                    }                    
                    var update = _context.Document.FirstOrDefault(x=>x.Id==id);
                    if (update != null)
                    {
                        update.DocumentTitle = document.DocumentTitle;
                        update.DocumentDescription = document.DocumentDescription;
                        update.UploadedDate = DateTime.Now;
                        update.Url = document.Url;
                        await _context.SaveChangesAsync();
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            if (file == null)
            {
                document.UploadedDate = DateTime.Now;
                _context.Document.Update(document);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["StudentId"] = user.StudentId;
            return View(document);
        }

        // GET: Document/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Document == null)
            {
                return NotFound();
            }

            var document = await _context.Document
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Document == null)
            {
                return Problem("Entity set 'ApplicationDbContextContext.Document'  is null.");
            }
            var document = await _context.Document.FindAsync(id);
            if (document != null)
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, document.Url);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                _context.Document.Remove(document);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
          return (_context.Document?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
