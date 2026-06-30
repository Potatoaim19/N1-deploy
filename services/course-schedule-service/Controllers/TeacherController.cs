using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N1.Data;
using N1.Models;

namespace N1.Controllers
{
    [Authorize(Roles = "Teacher")]
    [ApiController]
    [Route("api/course-schedule/teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");

        private async Task<Teacher?> GetCurrentTeacher()
        {
            var userId = GetUserId();
            return await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        [HttpGet("classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            var teacher = await GetCurrentTeacher();
            if (teacher == null) return NotFound("Teacher profile not found");

            var classes = await _context.Classes
                .Where(c => c.TeacherId == teacher.Id)
                .Include(c => c.Course)
                .ToListAsync();
            return Ok(classes);
        }

        [HttpGet("classes/{classId}")]
        public async Task<IActionResult> GetClassDetail(int classId)
        {
            var teacher = await GetCurrentTeacher();
            if (teacher == null) return Forbid();

            var @class = await _context.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == teacher.Id);

            if (@class == null) return NotFound();
            return Ok(@class);
        }

        [HttpGet("classes/{classId}/sessions")]
        public async Task<IActionResult> GetClassSessions(int classId)
        {
            var teacher = await GetCurrentTeacher();
            if (teacher == null) return Forbid();

            var sessions = await _context.ClassSessions
                .Include(s => s.Class)
                .Where(s => s.ClassId == classId && s.Class!.TeacherId == teacher.Id)
                .OrderBy(s => s.SessionNo)
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("schedule")]
        public async Task<IActionResult> GetMySchedule([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var teacher = await GetCurrentTeacher();
            if (teacher == null) return NotFound("Teacher profile not found");

            var query = _context.ClassSessions
                .Include(s => s.Class)
                .ThenInclude(c => c!.Course)
                .Where(s => s.Class!.TeacherId == teacher.Id);

            if (start.HasValue) query = query.Where(s => s.StudyDate >= start.Value);
            if (end.HasValue) query = query.Where(s => s.StudyDate <= end.Value);

            var sessions = await query
                .OrderBy(s => s.StudyDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetMyCalendar([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            return await GetMySchedule(start, end);
        }

        [HttpPatch("classes/{classId}/sessions/{sessionId}/topic")]
        public async Task<IActionResult> UpdateTopic(int classId, int sessionId, [FromBody] string topic)
        {
            var teacher = await GetCurrentTeacher();
            if (teacher == null) return Forbid();

            var session = await _context.ClassSessions
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClassId == classId);

            if (session == null) return NotFound();
            if (session.Class?.TeacherId != teacher.Id) return Forbid();

            session.Topic = topic;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("classes/{classId}/sessions/{sessionId}/change-request")]
        public async Task<IActionResult> RequestChange(int classId, int sessionId, [FromBody] string reason)
        {
            // Placeholder for change request logic
            return Ok(new { message = "Change request submitted", classId, sessionId, reason });
        }
    }
}
