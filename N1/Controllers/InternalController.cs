using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using N1.Data;

namespace N1.Controllers
{
    [Authorize(Roles = "InternalService,Admin")]
    [ApiController]
    [Route("api/course-schedule/internal")]
    public class InternalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InternalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("courses/{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpGet("classes/{classId}")]
        public async Task<IActionResult> GetClass(int classId)
        {
            var @class = await _context.Classes.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == classId);
            if (@class == null) return NotFound();
            return Ok(@class);
        }

        [HttpGet("classes/{classId}/registration-check")]
        public async Task<IActionResult> CheckRegistration(int classId)
        {
            var @class = await _context.Classes.FindAsync(classId);
            if (@class == null) return NotFound(new { message = "Class not found" });

            var now = DateTime.UtcNow;
            var canRegister = @class.Status == "Open" &&
                              @class.CurrentStudents < @class.MaxStudents &&
                              now >= @class.RegistrationStartDate &&
                              now <= @class.RegistrationEndDate;

            return Ok(new
            {
                CanRegister = canRegister,
                @class.Status,
                @class.CurrentStudents,
                @class.MaxStudents,
                @class.RegistrationStartDate,
                @class.RegistrationEndDate
            });
        }

        [HttpGet("classes/{classId}/capacity")]
        public async Task<IActionResult> GetCapacity(int classId)
        {
            var @class = await _context.Classes.Select(c => new { c.Id, c.CurrentStudents, c.MaxStudents }).FirstOrDefaultAsync(c => c.Id == classId);
            if (@class == null) return NotFound();
            return Ok(@class);
        }

        [HttpPatch("classes/{classId}/increase-student-count")]
        public async Task<IActionResult> IncreaseStudentCount(int classId)
        {
            var @class = await _context.Classes.FindAsync(classId);
            if (@class == null) return NotFound();

            if (@class.CurrentStudents >= @class.MaxStudents)
                return BadRequest("Class is full");

            @class.CurrentStudents++;
            await _context.SaveChangesAsync();
            return Ok(new { @class.CurrentStudents });
        }

        [HttpPatch("classes/{classId}/decrease-student-count")]
        public async Task<IActionResult> DecreaseStudentCount(int classId)
        {
            var @class = await _context.Classes.FindAsync(classId);
            if (@class == null) return NotFound();

            if (@class.CurrentStudents > 0)
            {
                @class.CurrentStudents--;
                await _context.SaveChangesAsync();
            }
            return Ok(new { @class.CurrentStudents });
        }

        [HttpGet("classes/{classId}/sessions")]
        public async Task<IActionResult> GetSessions(int classId)
        {
            var sessions = await _context.ClassSessions
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SessionNo)
                .ToListAsync();
            return Ok(sessions);
        }

        [HttpGet("teachers/{teacherId}")]
        public async Task<IActionResult> GetTeacher(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }
    }
}
