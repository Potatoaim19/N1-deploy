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
    [Route("api/course-schedule/admin/classes")]
    public class AdminClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for File Upload
        public class FileUploadRequest
        {
            public IFormFile File { get; set; } = null!;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
        {
            return await _context.Classes.Include(c => c.Course).Include(c => c.Teacher).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Class>> GetClass(int id)
        {
            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@class == null) return NotFound();
            return @class;
        }

        [HttpPost]
        public async Task<ActionResult<Class>> CreateClass(Class @class)
        {
            _context.Classes.Add(@class);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClass), new { id = @class.Id }, @class);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, Class @class)
        {
            if (id != @class.Id) return BadRequest();

            _context.Entry(@class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPatch("{id}/teacher")]
        public async Task<IActionResult> AssignTeacher(int id, [FromBody] int teacherId)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            @class.TeacherId = teacherId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            @class.Status = status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/capacity")]
        public async Task<IActionResult> UpdateCapacity(int id, [FromBody] int maxStudents)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            @class.MaxStudents = maxStudents;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class RegistrationWindowRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        [HttpPatch("{id}/registration-window")]
        public async Task<IActionResult> UpdateRegistrationWindow(int id, [FromBody] RegistrationWindowRequest window)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            @class.RegistrationStartDate = window.StartDate;
            @class.RegistrationEndDate = window.EndDate;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{classId}/sessions")]
        public async Task<ActionResult<IEnumerable<ClassSession>>> GetSessions(int classId)
        {
            return await _context.ClassSessions.Where(s => s.ClassId == classId).ToListAsync();
        }

        [HttpPost("{classId}/sessions")]
        public async Task<ActionResult<ClassSession>> CreateSession(int classId, ClassSession session)
        {
            session.ClassId = classId;
            _context.ClassSessions.Add(session);
            await _context.SaveChangesAsync();
            return Ok(session);
        }

        [HttpPost("{classId}/sessions/bulk")]
        public async Task<IActionResult> BulkCreateSessions(int classId, [FromBody] List<ClassSession> sessions)
        {
            foreach (var session in sessions)
            {
                session.ClassId = classId;
                session.Id = 0;
            }
            _context.ClassSessions.AddRange(sessions);
            await _context.SaveChangesAsync();
            return Ok(new { count = sessions.Count });
        }

        [HttpPut("{classId}/sessions/{sessionId}")]
        public async Task<IActionResult> UpdateSession(int classId, int sessionId, ClassSession session)
        {
            if (sessionId != session.Id || classId != session.ClassId) return BadRequest();
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{classId}/sessions/{sessionId}")]
        public async Task<IActionResult> DeleteSession(int classId, int sessionId)
        {
            var session = await _context.ClassSessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.ClassId == classId);
            if (session == null) return NotFound();

            _context.ClassSessions.Remove(session);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Import sessions endpoints
        [HttpGet("{classId}/sessions/import/template")]
        public IActionResult GetSessionImportTemplate(int classId) => Ok(new { templateLink = $"/templates/sessions.xlsx?classId={classId}" });

        [HttpPost("{classId}/sessions/import/preview")]
        [Consumes("multipart/form-data")]
        public IActionResult PreviewSessionImport(int classId, [FromForm] FileUploadRequest request) => Ok(new { batchId = 3, classId, fileName = request.File.FileName });

        [HttpPost("{classId}/sessions/import/confirm")]
        public IActionResult ConfirmSessionImport(int classId, [FromBody] int batchId) => Ok(new { message = "Import started", classId, batchId });

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
