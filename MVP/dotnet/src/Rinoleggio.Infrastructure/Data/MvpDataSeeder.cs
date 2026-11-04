using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Infrastructure.Data;

public static class MvpDataSeeder
{
    public static readonly string[] Roles = ["Administrator", "BackOffice", "Authenticated"];

    public static async Task SeedAsync(
        RinoleggioDbContext db,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        await EnsureAnonymousOwnerAsync(userManager);

        if (await db.Provinces.AnyAsync())
        {
            return;
        }

        db.Provinces.AddRange(
            new Province { Id = Guid.NewGuid(), Name = "Milano", Code = "MI", SortOrder = 1 },
            new Province { Id = Guid.NewGuid(), Name = "Roma", Code = "RM", SortOrder = 2 },
            new Province { Id = Guid.NewGuid(), Name = "Torino", Code = "TO", SortOrder = 3 });

        db.Brands.AddRange(
            new Brand { Id = Guid.NewGuid(), Name = "Fiat", SortOrder = 1 },
            new Brand { Id = Guid.NewGuid(), Name = "Volkswagen", SortOrder = 2 },
            new Brand { Id = Guid.NewGuid(), Name = "Toyota", SortOrder = 3 });

        db.RentalCompanies.Add(new RentalCompany
        {
            Id = Guid.NewGuid(),
            Name = "Leasys",
            WebsiteUrl = "https://www.leasys.com",
            IsActive = true
        });

        db.SitePages.Add(new SitePage
        {
            Id = Guid.NewGuid(),
            Title = "Rinoleggio — subentro contratti NLT",
            SlugPath = "",
            BodyHtml = "<p>Marketplace MVP (custom .NET + SQLite).</p>",
            IsHomePage = true,
            IsPublished = true
        });

        await db.SaveChangesAsync();
    }

    private static async Task EnsureAnonymousOwnerAsync(UserManager<AppUser> userManager)
    {
        var existing = await userManager.FindByIdAsync(AnonymousListingOwner.UserId.ToString());
        if (existing is not null)
        {
            return;
        }

        var user = new AppUser
        {
            Id = AnonymousListingOwner.UserId,
            UserName = AnonymousListingOwner.Email,
            Email = AnonymousListingOwner.Email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, $"Anon-{AnonymousListingOwner.UserId:N}-NotForLogin!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Impossibile creare l'utente anonimo: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
