using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Web.Pages.Listings;

[Authorize]
public class IndexModel(UserManager<AppUser> userManager, ListingService listingService) : PageModel
{
    public IReadOnlyList<Listing> Items { get; private set; } = Array.Empty<Listing>();

    public async Task OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Items = await listingService.GetByOwnerAsync(user.Id);
    }
}
