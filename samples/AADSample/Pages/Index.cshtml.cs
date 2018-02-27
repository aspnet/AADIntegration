using System.Threading.Tasks;
using Microsoft.AspNetCore.AADIntegration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AADSample.Pages
{
    public class IndexModel : PageModel
    {
        public async Task OnGetAsync([FromQuery] string scheme)
        {
            scheme = scheme ?? AzureAdDefaults.AuthenticationScheme;
            var result = await HttpContext.AuthenticateAsync(scheme);
            if (result.Succeeded)
            {
                HttpContext.User = result.Principal;
            }
            ViewData["Scheme"] = scheme;
        }
    }
}
