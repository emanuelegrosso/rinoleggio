using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Listings;

[Authorize]
public class EditModel(
    UserManager<AppUser> userManager,
    RinoleggioDbContext db,
    ListingService listingService) : ListingEditorPageModel(userManager, db, listingService)
{
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var listing = await listingService.GetOwnedListingAsync(id, user.Id);
        if (listing is null)
        {
            return NotFound();
        }

        Input = ListingService.FromListing(listing);
        await LoadLookupsAsync();
        return Page();
    }

    public Task<IActionResult> OnPostAsync() => SaveAsync();

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, string deleteReason)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(deleteReason))
        {
            ModelState.AddModelError(string.Empty, "Indica un motivo per l'eliminazione.");
            var listing = await listingService.GetOwnedListingAsync(id, user.Id);
            if (listing is null)
            {
                return NotFound();
            }

            Input = ListingService.FromListing(listing);
            await LoadLookupsAsync();
            return Page();
        }

        var ok = await listingService.SoftDeleteAsync(id, user.Id, deleteReason);
        if (!ok)
        {
            return NotFound();
        }

        return RedirectToPage("Index");
    }
}
