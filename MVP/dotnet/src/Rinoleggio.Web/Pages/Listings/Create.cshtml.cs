using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;
using Microsoft.AspNetCore.Identity;

namespace Rinoleggio.Web.Pages.Listings;

[Authorize]
public class CreateModel(
    UserManager<AppUser> userManager,
    RinoleggioDbContext db,
    ListingService listingService) : ListingEditorPageModel(userManager, db, listingService)
{
    public async Task OnGetAsync()
    {
        Input.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
        Input.TotalKm = 30000;
        await LoadLookupsAsync();
        if (Input.ProvinceId == Guid.Empty && ProvinceOptions.Any())
        {
            Input.ProvinceId = Guid.Parse(ProvinceOptions.First().Value!);
        }

        if (Input.RentalCompanyId == Guid.Empty && RentalCompanyOptions.Any())
        {
            Input.RentalCompanyId = Guid.Parse(RentalCompanyOptions.First().Value!);
        }

        if (Input.BrandId == Guid.Empty && BrandOptions.Any())
        {
            Input.BrandId = Guid.Parse(BrandOptions.First().Value!);
        }
    }

    public Task<IActionResult> OnPostAsync() => SaveAsync();
}
