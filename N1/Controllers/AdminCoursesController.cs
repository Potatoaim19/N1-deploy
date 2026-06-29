using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using N1.Data;
using N1.Models;

namespace N1.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/course-schedule/admin/courses")]
    public class AdminCoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminCoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for File Upload
        public class FileUploadRequest
        {
            public IFormFile File { get; set; } = null!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return course;
        }

        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, Course course)
        {
            if (id != course.Id) return BadRequest();

            course.UpdatedAt = DateTime.UtcNow;
            _context.Entry(course).State = EntityState.Modified;
            _context.Entry(course).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(int id, [FromBody] bool isActive)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.IsActive = isActive;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Import endpoints placeholders
        [HttpGet("import/template")]
        public IActionResult GetImportTemplate() => Ok(new { templateLink = "/templates/courses.xlsx" });

        [HttpPost("import/preview")]
        [Consumes("multipart/form-data")]
        public IActionResult PreviewImport([FromForm] FileUploadRequest request) => Ok(new { batchId = 2, fileName = request.File.FileName });

        [HttpPost("import/confirm")]
        public IActionResult ConfirmImport([FromBody] int batchId) => Ok(new { message = "Import started", batchId });

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
