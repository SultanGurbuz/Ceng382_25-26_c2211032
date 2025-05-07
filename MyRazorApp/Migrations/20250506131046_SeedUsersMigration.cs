using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyRazorApp.Migrations
{
    public partial class SeedUsersMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. ROLLERİ EKLE
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
                BEGIN
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES 
                        ('1', 'Admin', 'ADMIN', NEWID()),
                        ('2', 'User', 'USER', NEWID())
                END
            ");

            // 2. KULLANICILARI EKLE
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = 'a1')
                BEGIN
                    INSERT INTO AspNetUsers 
                    (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                     PasswordHash, SecurityStamp, ConcurrencyStamp, FirstName, LastName, 
                     CreatedDate, IsActive, LockoutEnabled, AccessFailedCount, PhoneNumber, 
                     PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd)
                    VALUES 
                    (
                        'a1', 'admin@myapp.com', 'ADMIN@MYAPP.COM', 'admin@myapp.com', 'ADMIN@MYAPP.COM', 1,
                        'AQAAAAIAAYagAAAAEN9Okm5ZuerrGKQzrrEyzI3r2Qdpl9g/Bc1W4uEuc4C7B1Z7DC/RFd8MT3/2UMm/9Q==',
                        NEWID(), NEWID(), 'System', 'Admin', GETDATE(), 1, 0, 0,
                        '+905551112233', 1, 0, NULL
                    )
                END;

                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = 'u1')
                BEGIN
                    INSERT INTO AspNetUsers 
                    (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                     PasswordHash, SecurityStamp, ConcurrencyStamp, FirstName, LastName, 
                     CreatedDate, IsActive, LockoutEnabled, AccessFailedCount, PhoneNumber, 
                     PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd)
                    VALUES 
                    (
                        'u1', 'user@myapp.com', 'USER@MYAPP.COM', 'user@myapp.com', 'USER@MYAPP.COM', 1,
                        'AQAAAAIAAYagAAAAEKgaQarm08zgvMY8NM2D9WFvfvDOHzVTFCjUtRyS1gNj2Y8o0d7K99nPB6iVX4NiIw==',
                        NEWID(), NEWID(), 'Test', 'User', GETDATE(), 1, 0, 0,
                        '+905554445566', 1, 0, NULL
                    )
                END;
            ");

            // 3. ROL ATAMASI
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = 'a1' AND RoleId = '1')
                    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('a1', '1');

                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = 'u1' AND RoleId = '2')
                    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('u1', '2');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetUserRoles WHERE UserId IN ('a1', 'u1');");
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE Id IN ('a1', 'u1');");
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Id IN ('1', '2');");
        }
    }
}
