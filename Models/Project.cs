using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProTrack.Models
{
    /// <summary>
    /// Represents a project in the Pro-Track system
    /// </summary>
    public class Project : BaseModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Hourly Rate")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be a positive number")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Project Status")]
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int ClientId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        // Navigation properties
        public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

        // Computed properties
        [NotMapped]
        [Display(Name = "Total Hours")]
        public decimal TotalHours => TimeEntries?.Where(te => te.IsBilled).Sum(te => te.DurationInHours) ?? 0;

        [NotMapped]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount => TotalHours * HourlyRate;
    }
}
