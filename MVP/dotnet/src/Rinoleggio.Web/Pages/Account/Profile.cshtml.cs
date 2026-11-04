using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Domain.Enums;
using Rinoleggio.Infrastructure.Data;

namespace Rinoleggio.Web.Pages.Account;

[Authorize]
public class ProfileModel(
    UserManager<AppUser> userManager,
    RinoleggioDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public DateTime? PrivacyConsentAt { get; private set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Il nome visualizzato è obbligatorio.")]
        [StringLength(80)]
        [Display(Name = "Nome e cognome")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Numero di telefono non valido.")]
        [StringLength(30)]
        [Display(Name = "Telefono (non pubblico)")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Tipo account")]
        public AccountType AccountType { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var profile = await LoadProfileAsync();
        if (profile is null)
        {
            return NotFound();
        }

        MapToInput(profile);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var existing = await LoadProfileAsync();
            PrivacyConsentAt = existing?.PrivacyConsentAt;
            return Page();
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var profile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (profile is null)
        {
            return NotFound();
        }

        profile.FullName = Input.FullName.Trim();
        profile.PhoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber.Trim();
        profile.AccountType = Input.AccountType;

        await db.SaveChangesAsync();

        TempData["StatusMessage"] = "Profilo aggiornato.";
        return RedirectToPage();
    }

    private async Task<UserProfile?> LoadProfileAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return null;
        }

        return await db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == user.Id);
    }

    private void MapToInput(UserProfile profile)
    {
        Input.FullName = profile.FullName;
        Input.PhoneNumber = profile.PhoneNumber;
        Input.AccountType = profile.AccountType;
        PrivacyConsentAt = profile.PrivacyConsentAt;
    }
}
