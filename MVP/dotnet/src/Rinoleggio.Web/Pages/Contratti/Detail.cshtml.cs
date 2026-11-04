using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Contratti;

public class DetailModel(ListingService listingService) : PageModel
{
    public Listing? Contract { get; private set; }

    public string SellerDisplayName { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Contract = await listingService.GetPublicListingAsync(id);
        if (Contract is null)
        {
            return NotFound();
        }

        SellerDisplayName = Contract.OwnerId == AnonymousListingOwner.UserId
            ? "Inserimento pubblico"
            : Contract.Owner.Profile?.FullName ?? "Venditore";
        return Page();
    }
}
