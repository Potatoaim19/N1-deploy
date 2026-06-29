using System.ComponentModel.DataAnnotations;

namespace N1.Models
{
    public class ImportBatch
    {
        [Key]
        public int Id { get; set; }
        public string ImportType { get; set; } = string.Empty; // Course, Teacher, Session
        public string FileName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int ErrorRows { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processed, Failed
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ImportError> Errors { get; set; } = new List<ImportError>();
    }

    public class ImportError
    {
        [Key]
        public int Id { get; set; }
        public int ImportBatchId { get; set; }
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
    }
}
