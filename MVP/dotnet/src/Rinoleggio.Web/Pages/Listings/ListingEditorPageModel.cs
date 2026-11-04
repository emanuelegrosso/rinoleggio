using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Listings;

public abstract class ListingEditorPageModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RinoleggioDbContext _db;
    private readonly ListingService _listingService;

    protected ListingEditorPageModel(
        UserManager<AppUser> userManager,
        RinoleggioDbContext db,
        ListingService listingService)
    {
        _userManager = userManager;
        _db = db;
        _listingService = listingService;
    }

    [BindProperty]
    public ListingEditorModel Input { get; set; } = new();

    public SelectList ProvinceOptions { get; private set; } = null!;

    public SelectList BrandOptions { get; private set; } = null!;

    public SelectList RentalCompanyOptions { get; private set; } = null!;

    protected async Task LoadLookupsAsync()
    {
        var provinces = await _db.Provinces.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        var brands = await _db.Brands.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.SortOrder).ToListAsync();
        var companies = await _db.RentalCompanies.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

        ProvinceOptions = new SelectList(provinces, nameof(Province.Id), nameof(Province.Name), Input.ProvinceId);
        BrandOptions = new SelectList(brands, nameof(Brand.Id), nameof(Brand.Name), Input.BrandId);
        RentalCompanyOptions = new SelectList(companies, nameof(RentalCompany.Id), nameof(RentalCompany.Name), Input.RentalCompanyId);
    }

    protected async Task<IActionResult> SaveAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return Page();
        }

        var (success, error, listingId) = await _listingService.SaveDraftAsync(user.Id, Input);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Salvataggio non riuscito.");
            await LoadLookupsAsync();
            return Page();
        }

        TempData["StatusMessage"] = "Bozza salvata.";
        return RedirectToPage("Edit", new { id = listingId });
    }
}
