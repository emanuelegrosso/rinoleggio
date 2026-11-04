using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rinoleggio.Web.Pages;

public class IndexModel(IConfiguration configuration) : PageModel
{
    public string DatabasePath { get; private set; } = configuration.GetConnectionString("Default") ?? "(default)";

    public void OnGet()
    {
    }
}
