using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProTrack.Models
{
    /// <summary>
    /// Represents a client in the Pro-Track system
    /// </summary>
    public class Client : BaseModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(256)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
