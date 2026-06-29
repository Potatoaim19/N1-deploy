using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace N1.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [JsonIgnore]
        public virtual Course? Course { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        [JsonIgnore]
        public virtual Teacher? Teacher { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationStartDate { get; set; }
        public DateTime RegistrationEndDate { get; set; }

        public int MaxStudents { get; set; }
        public int CurrentStudents { get; set; }

        public string? LearningMode { get; set; } // Offline, Online, Hybrid
        public string? Location { get; set; } // Room / Online Link

        public string Status { get; set; } = "Draft"; // Draft, Open, InProgress, Completed, Cancelled

        public virtual ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    }
}
