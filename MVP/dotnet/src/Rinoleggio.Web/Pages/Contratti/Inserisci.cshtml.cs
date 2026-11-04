using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Contratti;

public class InserisciModel(RinoleggioDbContext db, ListingService listingService) : PageModel
{
    [BindProperty]
    public ListingEditorModel Input { get; set; } = new();

    public SelectList ProvinceOptions { get; private set; } = null!;

    public SelectList BrandOptions { get; private set; } = null!;

    public SelectList RentalCompanyOptions { get; private set; } = null!;

    public async Task OnGetAsync()
    {
        Input.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
        Input.TotalKm = 30000;
        await LoadLookupsAsync();
        SetDefaultLookups();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return Page();
        }

        var (success, error, listingId) = await listingService.SaveContractAsync(
            AnonymousListingOwner.UserId,
            Input,
            publish: true);

        if (!success || listingId is null)
        {
            ModelState.AddModelError(string.Empty, error ?? "Salvataggio non riuscito.");
            await LoadLookupsAsync();
            return Page();
        }

        return RedirectToPage("Detail", new { id = listingId });
    }

    private async Task LoadLookupsAsync()
    {
        var provinces = await db.Provinces.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        var brands = await db.Brands.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.SortOrder).ToListAsync();
        var companies = await db.RentalCompanies.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

        ProvinceOptions = new SelectList(provinces, nameof(Province.Id), nameof(Province.Name), Input.ProvinceId);
        BrandOptions = new SelectList(brands, nameof(Brand.Id), nameof(Brand.Name), Input.BrandId);
        RentalCompanyOptions = new SelectList(companies, nameof(RentalCompany.Id), nameof(RentalCompany.Name), Input.RentalCompanyId);
    }

    private void SetDefaultLookups()
    {
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
}
