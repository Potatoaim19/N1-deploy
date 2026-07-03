using N1.Models;
using Microsoft.EntityFrameworkCore;

namespace N1.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Courses.Any()) return; // DB has been seeded

            // 1. Seed Courses (Seed ít nhất 20 để có ID 10)
            var courses = new List<Course>();
            for (int i = 1; i <= 20; i++)
            {
                courses.Add(new Course
                {
                    Code = $"CRS{i:D3}",
                    Name = $"Course Name {i}",
                    Description = $"Description for course {i}",
                    Level = i % 3 == 0 ? "Beginner" : (i % 3 == 1 ? "Intermediate" : "Advanced"),
                    DurationWeeks = 4 + (i % 8),
                    TotalSessions = 8 + (i % 16),
                    DefaultFee = 1000000 + (i * 50000),
                    IsActive = true
                });
            }
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // 2. Seed Teachers (Seed ít nhất 210 để có ID 201)
            var teachers = new List<Teacher>();
            for (int i = 1; i <= 210; i++)
            {
                teachers.Add(new Teacher
                {
                    TeacherCode = $"TCH{i:D3}",
                    FullName = $"Teacher Full Name {i}",
                    Email = $"teacher{i}@example.com",
                    Phone = $"0900000{i:D3}",
                    Specialty = i % 2 == 0 ? "Programming" : "Design",
                    Status = "Active"
                });
            }
            context.Teachers.AddRange(teachers);
            context.SaveChanges();

            // 3. Seed Classes (Seed ít nhất 110 để có ID 100)
            var classes = new List<Class>();
            var courseList = context.Courses.ToList();
            var teacherList = context.Teachers.ToList();
            for (int i = 1; i <= 110; i++)
            {
                var startDate = DateTime.UtcNow.AddDays(i);
                var cIdx = (i - 1) % courseList.Count;
                var tIdx = (i - 1) % teacherList.Count;

                classes.Add(new Class
                {
                    CourseId = courseList[cIdx].Id,
                    TeacherId = teacherList[tIdx].Id,
                    ClassCode = $"CLS{i:D3}",
                    Name = $"Class {i} for {courseList[cIdx].Name}",
                    StartDate = startDate,
                    EndDate = startDate.AddMonths(2),
                    RegistrationStartDate = startDate.AddDays(-14),
                    RegistrationEndDate = startDate.AddDays(-2),
                    MaxStudents = 20 + (i % 10),
                    CurrentStudents = i % 20,
                    LearningMode = i % 2 == 0 ? "Offline" : "Online",
                    Location = i % 2 == 0 ? $"Room {100 + i}" : $"https://meet.google.com/abc-{i}",
                    Status = "Open"
                });
            }
            context.Classes.AddRange(classes);
            context.SaveChanges();

            // 4. Seed ClassSessions
            var sessions = new List<ClassSession>();
            var classList = context.Classes.ToList();
            for (int i = 1; i <= classList.Count; i++)
            {
                sessions.Add(new ClassSession
                {
                    ClassId = classList[i - 1].Id,
                    SessionNo = 1,
                    StudyDate = classList[i - 1].StartDate.AddDays(1),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Room = classList[i - 1].Location,
                    Topic = $"Introduction to {classList[i - 1].Name}",
                    Status = "Scheduled"
                });
            }
            context.ClassSessions.AddRange(sessions);
            context.SaveChanges();

            // 5. Seed ImportBatches
            var batches = new List<ImportBatch>();
            for (int i = 1; i <= 50; i++)
            {
                batches.Add(new ImportBatch
                {
                    ImportType = i % 2 == 0 ? "Course" : "Teacher",
                    FileName = $"data_import_{i}.xlsx",
                    TotalRows = 100,
                    SuccessRows = 90,
                    ErrorRows = 10,
                    Status = "Processed",
                    CreatedBy = "Admin",
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }
            context.ImportBatches.AddRange(batches);
            context.SaveChanges();

            // 6. Seed ImportErrors
            var errors = new List<ImportError>();
            var batchList = context.ImportBatches.ToList();
            for (int i = 1; i <= 50; i++)
            {
                errors.Add(new ImportError
                {
                    ImportBatchId = batchList[i - 1].Id,
                    RowNumber = i,
                    ErrorMessage = $"Missing required field at row {i}",
                    RawData = "{ \"code\": \"\", \"name\": \"Test\" }"
                });
            }
            context.ImportErrors.AddRange(errors);
            context.SaveChanges();

            // 7. Seed StudentEnrollments
            var enrollments = new List<StudentEnrollment>();
            for (int i = 1; i <= 50; i++)
            {
                enrollments.Add(new StudentEnrollment
                {
                    StudentUserId = $"student-uuid-{i}",
                    ClassId = classList[i - 1].Id,
                    EnrolledAt = DateTime.UtcNow.AddDays(-i),
                    Status = "Active"
                });
            }
            context.StudentEnrollments.AddRange(enrollments);
            context.SaveChanges();
        }
    }
}
