using Microsoft.EntityFrameworkCore;
using MemoriesBack.Entities;

namespace MemoriesBack.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupMemberClass> GroupMemberClasses { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SensitiveData> SensitiveData { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(u => u.ReceivedGrades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany(u => u.GivenGrades)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
