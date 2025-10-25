using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProTrack.Models
{
    /// <summary>
    /// Represents a project in the Pro-Track system
    /// Projects are specific work engagements with clients
    /// Each project tracks time entries, hourly rates, and overall status
    /// </summary>
    public class Project : BaseModel
    {
        /// <summary>
        /// The title or name of the project (required)
        /// Maximum length: 200 characters
        /// Example: "Website Redesign", "Mobile App Development"
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the project scope and requirements
        /// Maximum length: 2000 characters
        /// </summary>
        [MaxLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        /// <summary>
        /// The billing rate per hour for this project
        /// Must be a positive number with up to 2 decimal places
        /// Used to calculate total billable amount from time entries
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Hourly Rate")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be a positive number")]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Current status of the project
        /// Options: Active, Completed, or Invoiced
        /// Defaults to Active when created
        /// </summary>
        [Display(Name = "Project Status")]
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        /// <summary>
        /// The date when the project began or is scheduled to begin
        /// Optional field
        /// </summary>
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date when the project ended or is scheduled to end
        /// Optional field
        /// </summary>
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The date and time when this project record was created
        /// Defaults to current UTC time
        /// </summary>
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date and time when this project record was last modified
        /// Updated automatically on each save
        /// </summary>
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        
        /// <summary>
        /// The ID of the client this project belongs to (required)
        /// </summary>
        [Required]
        public int ClientId { get; set; }

        // Navigation properties
        
        /// <summary>
        /// The user (freelancer) who owns this project
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// The client this project is associated with
        /// </summary>
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        /// <summary>
        /// Collection of all time entries logged for this project
        /// </summary>
        public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

        // Computed properties (not stored in database)
        
        /// <summary>
        /// Calculates the total billed hours for this project
        /// Sums up all time entries marked as billed
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Hours")]
        public decimal TotalHours => TimeEntries?.Where(te => te.IsBilled).Sum(te => te.DurationInHours) ?? 0;

        /// <summary>
        /// Calculates the total billable amount for this project
        /// Total Hours multiplied by Hourly Rate
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount => TotalHours * HourlyRate;
    }
}
