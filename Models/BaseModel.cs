using System.ComponentModel.DataAnnotations;

namespace ProTrack.Models
{
    /// <summary>
    /// Base model class containing common properties for all entities
    /// </summary>
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
    }
}
