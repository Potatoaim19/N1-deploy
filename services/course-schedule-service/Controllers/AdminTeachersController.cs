using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using N1.Data;
using N1.Models;
using System.Net.Http.Json;

namespace N1.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/course-schedule/admin/teachers")]
    public class AdminTeachersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminTeachersController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // DTO for File Upload
        public class FileUploadRequest
        {
            public IFormFile File { get; set; } = null!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        {
            return await _context.Teachers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return teacher;
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(Teacher teacher, [FromQuery] bool createAccount = false)
        {
            teacher.CreatedAt = DateTime.UtcNow;
            teacher.UpdatedAt = DateTime.UtcNow;
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            if (createAccount)
            {
                await CreateExternalAccount(teacher);
            }

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, Teacher teacher)
        {
            if (id != teacher.Id) return BadRequest();

            teacher.UpdatedAt = DateTime.UtcNow;
            _context.Entry(teacher).State = EntityState.Modified;
            _context.Entry(teacher).Property(x => x.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            teacher.Status = status;
            teacher.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/create-account")]
        public async Task<IActionResult> CreateAccount(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            var success = await CreateExternalAccount(teacher);
            if (!success) return StatusCode(500, "Failed to create account in Auth service");

            return Ok(new { message = "Account creation request sent" });
        }

        private async Task<bool> CreateExternalAccount(Teacher teacher)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthService");
                var response = await client.PostAsJsonAsync("/api/payment-report/internal/users/create-teacher-account", new
                {
                    teacherId = teacher.Id,
                    fullName = teacher.FullName,
                    email = teacher.Email,
                    phone = teacher.Phone
                });

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet("import/template")]
        public IActionResult GetImportTemplate() => Ok(new { templateLink = "/templates/teachers.xlsx" });

        [HttpPost("import/preview")]
        [Consumes("multipart/form-data")]
        public IActionResult PreviewImport([FromForm] FileUploadRequest request) => Ok(new { batchId = 1, fileName = request.File.FileName });

        [HttpPost("import/confirm")]
        public IActionResult ConfirmImport([FromBody] int batchId) => Ok(new { message = "Import started", batchId });

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
