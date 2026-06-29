using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace N1.Models
{
    public class StudentEnrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentUserId { get; set; } = string.Empty; // From Auth/Student Service

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        [JsonIgnore] // Tránh vòng lặp Enrollment -> Class -> Enrollment
        public virtual Class? Class { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active"; // Active, Withdrawn
    }
}
