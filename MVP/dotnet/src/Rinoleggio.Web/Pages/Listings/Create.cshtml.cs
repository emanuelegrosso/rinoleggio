using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rinoleggio.Web.Pages.Listings;

public class CreateModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Contratti/Inserisci");
}
