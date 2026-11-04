using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Domain.Enums;
using Rinoleggio.Infrastructure.Data;

namespace Rinoleggio.Web.Pages.Account;

public class RegisterModel(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RinoleggioDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Il nome visualizzato è obbligatorio.")]
        [StringLength(80, ErrorMessage = "Massimo 80 caratteri.")]
        [Display(Name = "Nome e cognome")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve avere almeno 8 caratteri.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conferma la password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono.")]
        [Display(Name = "Conferma password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona il tipo di account.")]
        [Display(Name = "Tipo account")]
        public AccountType AccountType { get; set; } = AccountType.Private;

        [Range(typeof(bool), "true", "true", ErrorMessage = "Devi accettare l'informativa privacy.")]
        [Display(Name = "Accetto l'informativa privacy")]
        public bool PrivacyConsent { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new AppUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, Input.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        await userManager.AddToRoleAsync(user, "Authenticated");

        db.UserProfiles.Add(new UserProfile
        {
            UserId = user.Id,
            FullName = Input.FullName.Trim(),
            AccountType = Input.AccountType,
            PrivacyConsent = true,
            PrivacyConsentAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Account/Index");
    }
}
