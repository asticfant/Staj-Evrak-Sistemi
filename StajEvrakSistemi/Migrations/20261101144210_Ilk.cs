using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StajEvrakSistemi.Migrations
{
    /// <inheritdoc />
    public partial class Ilk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BasariliMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    SifreHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    HataliGirisSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    KilitBitisZamani = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: true),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: true),
                    Uyruk = table.Column<string>(type: "TEXT", nullable: true),
                    OgrenciNo = table.Column<string>(type: "TEXT", nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Adres = table.Column<string>(type: "TEXT", nullable: true),
                    Bolum = table.Column<string>(type: "TEXT", nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", nullable: true),
                    SaglikYardimiStatusu = table.Column<string>(type: "TEXT", nullable: true),
                    OncekiStajGunSayisi = table.Column<int>(type: "INTEGER", nullable: true),
                    GoogleFormUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VeriSetiKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OgrenciNo = table.Column<string>(type: "TEXT", nullable: false),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "TEXT", nullable: false),
                    Uyruk = table.Column<string>(type: "TEXT", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Adres = table.Column<string>(type: "TEXT", nullable: false),
                    Bolum = table.Column<string>(type: "TEXT", nullable: false),
                    Ek16TeslimYili = table.Column<string>(type: "TEXT", nullable: false),
                    OncekiStajGunSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", nullable: false),
                    SaglikYardimiStatusu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeriSetiKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DosyaYolu = table.Column<string>(type: "TEXT", nullable: false),
                    DosyaTuru = table.Column<string>(type: "TEXT", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: false),
                    YuklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OkunanAlanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlanAdi = table.Column<string>(type: "TEXT", nullable: false),
                    Deger = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OkunanAlanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OkunanAlanlar_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StajBilgileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ek16TeslimYili = table.Column<string>(type: "TEXT", nullable: false),
                    StajBaslangic = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StajBitis = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CalisilanGunler = table.Column<string>(type: "TEXT", nullable: false),
                    IsyeriAdi = table.Column<string>(type: "TEXT", nullable: false),
                    IsyeriAdresi = table.Column<string>(type: "TEXT", nullable: false),
                    IsyeriTelefon = table.Column<string>(type: "TEXT", nullable: false),
                    IsyeriEmail = table.Column<string>(type: "TEXT", nullable: false),
                    MuhendisSayisi = table.Column<int>(type: "INTEGER", nullable: false),
                    MuhendisBolumu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StajBilgileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StajBilgileri_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationIssues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlanAdi = table.Column<string>(type: "TEXT", nullable: false),
                    GirilenDeger = table.Column<string>(type: "TEXT", nullable: true),
                    OkunanDeger = table.Column<string>(type: "TEXT", nullable: true),
                    Mesaj = table.Column<string>(type: "TEXT", nullable: false),
                    Tur = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationIssues_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StudentId",
                table: "Documents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_OkunanAlanlar_DocumentId",
                table: "OkunanAlanlar",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_StajBilgileri_DocumentId",
                table: "StajBilgileri",
                column: "DocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_KullaniciAdi",
                table: "Users",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValidationIssues_DocumentId",
                table: "ValidationIssues",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_VeriSetiKayitlari_OgrenciNo",
                table: "VeriSetiKayitlari",
                column: "OgrenciNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginLogs");

            migrationBuilder.DropTable(
                name: "OkunanAlanlar");

            migrationBuilder.DropTable(
                name: "StajBilgileri");

            migrationBuilder.DropTable(
                name: "ValidationIssues");

            migrationBuilder.DropTable(
                name: "VeriSetiKayitlari");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
