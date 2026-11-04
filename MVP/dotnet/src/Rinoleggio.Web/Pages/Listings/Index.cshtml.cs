using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rinoleggio.Web.Pages.Listings;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Contratti/Index");
}
