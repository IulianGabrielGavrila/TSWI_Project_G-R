using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace DotNetAuth.Pages
{
    [AllowAnonymous]   // or [Authorize] if you want only logged-in users
    public class IndexModel : PageModel
    {
        public string GreetingUser { get; private set; } = "guest";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                GreetingUser = User.Identity.Name!;
        }
    }
}
