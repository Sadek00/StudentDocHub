using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StudentDocVault.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [ProtectedPersonalData]
    public int StudentId { get; set; }
    [PersonalData]
    [Column(TypeName="nvarchar(250)")]
    public string? Name { get; set;}
}

