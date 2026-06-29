using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace N1.Models
{
    public class ClassSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        [JsonIgnore] // Tránh vòng lặp Class -> Sessions -> Class
        public virtual Class? Class { get; set; }

        public int SessionNo { get; set; }
        public DateTime StudyDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [MaxLength(100)]
        public string? Room { get; set; }

        [MaxLength(500)]
        public string? Topic { get; set; }

        public string Status { get; set; } = "Scheduled"; // Scheduled, Done, Cancelled
    }
}
