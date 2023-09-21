using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentDocVault.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? DocumentTitle { get; set; }
        public string? DocumentDescription { get; set;}
        public DateTime UploadedDate { get; set; }
        [DisplayName("Uploaded Docs")]
        public string? Url { get; set; }

    }
}
