using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProTrack.Models
{
    /// <summary>
    /// Represents a client in the Pro-Track system
    /// Clients are organizations or individuals that freelancers work for
    /// Each client can have multiple projects and invoices associated with them
    /// </summary>
    public class Client : BaseModel
    {
        /// <summary>
        /// The name of the client (required)
        /// Maximum length: 200 characters
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The client's primary contact email address
        /// Must be a valid email format
        /// Maximum length: 256 characters
        /// </summary>
        [EmailAddress]
        [MaxLength(256)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        /// <summary>
        /// The client's contact phone number
        /// Maximum length: 20 characters
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// The client's physical or mailing address
        /// Maximum length: 500 characters
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        /// <summary>
        /// Additional notes or information about the client
        /// Maximum length: 1000 characters
        /// </summary>
        [MaxLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// The date and time when this client record was created
        /// Defaults to current UTC time
        /// </summary>
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indicates whether this client is currently active
        /// Inactive clients are soft-deleted and won't appear in most lists
        /// Defaults to true
        /// </summary>
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        
        /// <summary>
        /// The user (freelancer) who owns this client record
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Collection of all projects associated with this client
        /// </summary>
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        
        /// <summary>
        /// Collection of all invoices issued to this client
        /// </summary>
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
