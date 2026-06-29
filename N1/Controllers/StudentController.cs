using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N1.Data;

namespace N1.Controllers
{
    [Authorize(Roles = "Student")]
    [ApiController]
    [Route("api/course-schedule/student")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");

        [HttpGet("courses")]
        public async Task<IActionResult> GetActiveCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();
            return Ok(courses);
        }

        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourseDetail(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null || !course.IsActive) return NotFound();
            return Ok(course);
        }

        [HttpGet("classes/open")]
        public async Task<IActionResult> GetOpenClasses()
        {
            var now = DateTime.UtcNow;
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Where(c => c.Status == "Open" &&
                            c.RegistrationStartDate <= now &&
                            c.RegistrationEndDate >= now)
                .ToListAsync();
            return Ok(classes);
        }

        [HttpGet("classes/{classId}")]
        public async Task<IActionResult> GetClassDetail(int classId)
        {
            var @class = await _context.Classes
                .Include(c => c.Course)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (@class == null) return NotFound();
            return Ok(@class);
        }

        [HttpGet("classes/{classId}/registration-info")]
        public async Task<IActionResult> GetRegistrationInfo(int classId)
        {
            var @class = await _context.Classes.FindAsync(classId);
            if (@class == null) return NotFound();

            return Ok(new
            {
                @class.Id,
                @class.MaxStudents,
                @class.CurrentStudents,
                RemainingSpots = @class.MaxStudents - @class.CurrentStudents,
                @class.RegistrationEndDate,
                @class.StartDate
            });
        }

        [HttpGet("classes/{classId}/sessions")]
        public async Task<IActionResult> GetClassSessions(int classId)
        {
            var sessions = await _context.ClassSessions
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SessionNo)
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpGet("schedule")]
        public async Task<IActionResult> GetMySchedule([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var enrolledClassIds = await _context.StudentEnrollments
                .Where(e => e.StudentUserId == userId && e.Status == "Active")
                .Select(e => e.ClassId)
                .ToListAsync();

            var query = _context.ClassSessions
                .Include(s => s.Class)
                .ThenInclude(c => c!.Course)
                .Where(s => enrolledClassIds.Contains(s.ClassId));

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
    }
}
