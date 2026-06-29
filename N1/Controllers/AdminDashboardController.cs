using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N1.Data;

namespace N1.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/course-schedule/admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var totalCourses = await _context.Courses.CountAsync();
            var totalClasses = await _context.Classes.CountAsync();
            var activeTeachers = await _context.Teachers.CountAsync(t => t.Status == "Active");

            var now = DateTime.UtcNow;
            var upcomingClasses = await _context.Classes
                .Where(c => c.StartDate > now && c.Status == "Open")
                .CountAsync();

            var nearFullClasses = await _context.Classes
                .Where(c => c.Status == "Open" && (c.MaxStudents - c.CurrentStudents) <= 5 && c.MaxStudents > 0)
                .Select(c => new { c.Id, c.Name, c.CurrentStudents, c.MaxStudents })
                .ToListAsync();

            return Ok(new
            {
                TotalCourses = totalCourses,
                TotalClasses = totalClasses,
                ActiveTeachers = activeTeachers,
                UpcomingClasses = upcomingClasses,
                NearFullClasses = nearFullClasses
            });
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetGlobalCalendar([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var sessions = await _context.ClassSessions
                .Include(s => s.Class)
                .ThenInclude(c => c!.Course)
                .Where(s => s.StudyDate >= start && s.StudyDate <= end)
                .Select(s => new
                {
                    s.Id,
                    s.ClassId,
                    ClassName = s.Class!.Name,
                    CourseName = s.Class.Course!.Name,
                    s.StudyDate,
                    s.StartTime,
                    s.EndTime,
                    s.Room,
                    s.Topic,
                    s.Status
                })
                .ToListAsync();

            return Ok(sessions);
        }
    }
}
