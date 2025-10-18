using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProTrack.Models
{
    /// <summary>
    /// Represents a time entry in the Pro-Track system
    /// </summary>
    public class TimeEntry : BaseModel
    {
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [NotMapped]
        [Display(Name = "Duration (Hours)")]
        public decimal DurationInHours
        {
            get
            {
                if (EndTime <= StartTime)
                    return 0;

                var duration = EndTime - StartTime;
                return (decimal)duration.TotalHours;
            }
        }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Is Billed")]
        public bool IsBilled { get; set; } = false;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        // Computed properties
        [NotMapped]
        [Display(Name = "Amount")]
        public decimal Amount => DurationInHours * Project?.HourlyRate ?? 0;
    }
}
