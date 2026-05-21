using Microsoft.EntityFrameworkCore;
using Education.API.Models;

namespace Education.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // الجداول الحقيقية في قاعدة البيانات
        public DbSet<Student> Students { get; set; }
        public DbSet<ScratchCard> ScratchCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- إعدادات جدول الطلاب ---
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentID);
                entity.Property(s => s.FullName).IsRequired().HasMaxLength(150);
                entity.HasIndex(s => s.Username).IsUnique();
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.WalletBalance).HasColumnType("decimal(18,2)");
            });

            // --- إعدادات جدول كروت الشحن ---
            modelBuilder.Entity<ScratchCard>(entity =>
            {
                entity.HasKey(c => c.CardID);
                entity.HasIndex(c => c.SecureCode).IsUnique();
                entity.Property(c => c.SecureCode).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Value).HasColumnType("decimal(18,2)");

                // العلاقة: الطالب يشحن عدة كروت، والكارت لطالب واحد
                entity.HasOne(c => c.UsedByStudent)
                      .WithMany(s => s.UsedCards)
                      .HasForeignKey(c => c.UsedByStudentID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}