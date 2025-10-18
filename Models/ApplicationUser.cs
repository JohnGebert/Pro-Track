using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProTrack.Models
{
    /// <summary>
    /// Extended IdentityUser for custom user data
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [MaxLength(200)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
