using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace N1.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; } // Link to Auth Service
        [Required]
        [MaxLength(50)]
        public string TeacherCode { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public string Status { get; set; } = "Active"; // Active, Inactive
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
