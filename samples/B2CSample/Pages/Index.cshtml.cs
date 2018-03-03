using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.B2CIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace B2CSample.Pages
{
    public class IndexModel : PageModel
    {
        public async Task OnGetAsync([FromQuery] string scheme)
        {
            scheme = scheme ?? Request.Cookies[nameof(scheme)] ?? AzureAdB2CDefaults.AuthenticationScheme;
            Response.Cookies.Append(nameof(scheme), scheme,new CookieOptions {
                SameSite = SameSiteMode.None
            });

            var result = await HttpContext.AuthenticateAsync(scheme);
            if (result.Succeeded)
            {
                HttpContext.User = result.Principal;
            }
            ViewData["Scheme"] = scheme;
        }
    }
}
