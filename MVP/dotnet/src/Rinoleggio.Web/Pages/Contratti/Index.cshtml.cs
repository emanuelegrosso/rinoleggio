using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Enums;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Contratti;

public class IndexModel(RinoleggioDbContext db, ListingService listingService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? ProvinceId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? BrandId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? RentalCompanyId { get; set; }

    [BindProperty(SupportsGet = true)]
    public FuelType? FuelType { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MonthlyFeeMin { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MonthlyFeeMax { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? RemainingKmMin { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public ContractSearchResult Results { get; private set; } = new() { Items = Array.Empty<ListingSummary>() };

    public int TotalPages =>
        Results.PageSize > 0
            ? (int)Math.Ceiling(Results.TotalCount / (double)Results.PageSize)
            : 0;

    public SelectList ProvinceOptions { get; private set; } = null!;

    public SelectList BrandOptions { get; private set; } = null!;

    public SelectList RentalCompanyOptions { get; private set; } = null!;

    public async Task OnGetAsync()
    {
        if (ProvinceId == Guid.Empty)
        {
            ProvinceId = null;
        }

        if (BrandId == Guid.Empty)
        {
            BrandId = null;
        }

        if (RentalCompanyId == Guid.Empty)
        {
            RentalCompanyId = null;
        }

        await LoadLookupsAsync();

        Results = await listingService.SearchPublicAsync(new ContractSearchCriteria
        {
            ProvinceId = ProvinceId,
            BrandId = BrandId,
            RentalCompanyId = RentalCompanyId,
            FuelType = FuelType,
            MonthlyFeeMin = MonthlyFeeMin,
            MonthlyFeeMax = MonthlyFeeMax,
            RemainingKmMin = RemainingKmMin,
            Query = Query,
            Page = PageNumber
        });
    }

    private async Task LoadLookupsAsync()
    {
        var provinces = await db.Provinces.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.SortOrder).ToListAsync();
        var brands = await db.Brands.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.SortOrder).ToListAsync();
        var companies = await db.RentalCompanies.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

        ProvinceOptions = new SelectList(provinces, "Id", "Name", ProvinceId);
        BrandOptions = new SelectList(brands, "Id", "Name", BrandId);
        RentalCompanyOptions = new SelectList(companies, "Id", "Name", RentalCompanyId);
    }
}
