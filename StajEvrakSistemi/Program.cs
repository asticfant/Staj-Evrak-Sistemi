using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Readers;
using StajEvrakSistemi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// --- Veritabanı (SQLite) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// --- Kimlik doğrulama: cookie (rol bilgisi claim olarak cookie'de taşınır) ---
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Giris";
        options.LogoutPath = "/Account/Cikis";
        options.AccessDeniedPath = "/Account/ErisimEngellendi";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });
builder.Services.AddAuthorization();

// --- Servisler ---
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProfilService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<BelgeSorguService>();
builder.Services.AddScoped<BelgeIslemeService>();
builder.Services.AddScoped<ComparisonService>();
builder.Services.AddScoped<DatasetImportService>();

// --- Belge okuyucular ---
builder.Services.AddScoped<DocxReader>();
builder.Services.AddScoped<PdfReader>();
builder.Services.AddScoped<OkuyucuSecici>();

var app = builder.Build();

// --- Veritabanını güncelle ve varsayılan admin'i oluştur ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    scope.ServiceProvider.GetRequiredService<DbSeeder>().AdminOlustur();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
