using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Eksik tabloları oluşturur; arkadaşların elle komut yazmasına gerek kalmaz
        db.Database.Migrate();

        if (!db.Admins.Any())
        {
            db.Admins.Add(new Admin
            {
                KullaniciAdi = "admin",
                Email = "admin@staj.local",
                SifreHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            });
            db.SaveChanges();
        }
    }
}