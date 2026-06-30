using Microsoft.EntityFrameworkCore;
using N1.Models;

namespace N1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<ClassSession> ClassSessions { get; set; } = null!;
        public DbSet<ImportBatch> ImportBatches { get; set; } = null!;
        public DbSet<ImportError> ImportErrors { get; set; } = null!;
        public DbSet<StudentEnrollment> StudentEnrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>().Property(c => c.DefaultFee).HasPrecision(18, 2);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Course)
                .WithMany(co => co.Classes)
                .HasForeignKey(c => c.CourseId);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<ClassSession>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.ClassId);

            modelBuilder.Entity<ImportError>()
                .HasOne<ImportBatch>()
                .WithMany(b => b.Errors)
                .HasForeignKey(e => e.ImportBatchId);

            modelBuilder.Entity<StudentEnrollment>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId);
        }
    }
}
