using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<OkunanAlan> OkunanAlanlar => Set<OkunanAlan>();
    public DbSet<ValidationIssue> ValidationIssues => Set<ValidationIssue>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Öğrenci ve admin tek "Users" tablosunda; Rol sütunu hangisi olduğunu belirtir (TPH)
        modelBuilder.Entity<User>()
            .HasDiscriminator(u => u.Rol)
            .HasValue<Student>(UserRole.Student)
            .HasValue<Admin>(UserRole.Admin);

        // Enum'lar veritabanında sayı yerine yazı olarak tutulsun (DB Browser'da okunabilir olur)
        modelBuilder.Entity<User>().Property(u => u.Rol).HasConversion<string>();
        modelBuilder.Entity<Document>().Property(d => d.Durum).HasConversion<string>();
        modelBuilder.Entity<Document>().Property(d => d.DosyaTuru).HasConversion<string>();
        modelBuilder.Entity<ValidationIssue>().Property(v => v.Tur).HasConversion<string>();

        // Aynı kullanıcı adıyla iki kişi olamaz
        modelBuilder.Entity<User>().HasIndex(u => u.KullaniciAdi).IsUnique();
    }
}