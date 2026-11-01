using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Data;

/// <summary>İlk açılışta varsayılan admin hesabını oluşturur.</summary>
public class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public DbSeeder(AppDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public void AdminOlustur()
    {
        if (_db.Admins.Any())
        {
            return;
        }

        var admin = new Admin("admin", "admin@stajevrak.local", _hasher.Hashle("Admin123!"));
        _db.Admins.Add(admin);
        _db.SaveChanges();
    }
}
