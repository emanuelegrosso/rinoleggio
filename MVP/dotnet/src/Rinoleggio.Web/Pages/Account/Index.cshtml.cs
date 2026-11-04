using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Infrastructure.Data;

namespace Rinoleggio.Web.Pages.Account;

[Authorize]
public class IndexModel(
    UserManager<AppUser> userManager,
    RinoleggioDbContext db) : PageModel
{
    public string DisplayName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public int ListingCount { get; private set; }

    public async Task OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Email = user.Email ?? string.Empty;

        var profile = await db.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == user.Id);
        DisplayName = profile?.FullName ?? Email;

        ListingCount = await db.Listings.CountAsync(l => l.OwnerId == user.Id);
    }
}
