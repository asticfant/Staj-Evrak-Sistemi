using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<StajBilgisi> StajBilgileri => Set<StajBilgisi>();
    public DbSet<OkunanAlan> OkunanAlanlar => Set<OkunanAlan>();
    public DbSet<ValidationIssue> ValidationIssues => Set<ValidationIssue>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<VeriSetiKaydi> VeriSetiKayitlari => Set<VeriSetiKaydi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- User / Student / Admin: tek tablo (TPH), ayırıcı sütun = Rol ---
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.Rol).HasConversion<string>();
            e.HasDiscriminator(u => u.Rol)
                .HasValue<Student>(UserRole.Student)
                .HasValue<Admin>(UserRole.Admin);
            e.HasIndex(u => u.KullaniciAdi).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        // --- Document ---
        modelBuilder.Entity<Document>(e =>
        {
            e.Property(d => d.Durum).HasConversion<string>();
            e.Property(d => d.DosyaTuru).HasConversion<string>();

            e.HasOne(d => d.Student)
                .WithMany(s => s.Belgeler)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.StajBilgisi)
                .WithOne(s => s.Document)
                .HasForeignKey<StajBilgisi>(s => s.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(d => d.OkunanAlanlar)
                .WithOne()
                .HasForeignKey(o => o.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(d => d.Hatalar)
                .WithOne()
                .HasForeignKey(v => v.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- StajBilgisi: çalışılan günler tek sütunda "1,2,3" gibi saklanır ---
        modelBuilder.Entity<StajBilgisi>(e =>
        {
            e.Property(s => s.CalisilanGunler)
                .HasConversion(
                    v => string.Join(',', v.Select(g => (int)g)),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(x => (DayOfWeek)int.Parse(x))
                          .ToList(),
                    new ValueComparer<List<DayOfWeek>>(
                        (a, b) => a!.SequenceEqual(b!),
                        v => v.Aggregate(0, (h, g) => HashCode.Combine(h, g)),
                        v => v.ToList()));
        });

        modelBuilder.Entity<ValidationIssue>()
            .Property(v => v.Tur).HasConversion<string>();

        modelBuilder.Entity<VeriSetiKaydi>()
            .HasIndex(k => k.OgrenciNo).IsUnique();
    }
}
