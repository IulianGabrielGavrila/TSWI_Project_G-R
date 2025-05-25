using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuth.Controllers
{
    [Authorize]                // default: must be logged in
    public class PermissionController : Controller
    {
        // PUBLIC “home” page at GET /
        [AllowAnonymous]
        [HttpGet("/")]
        public IActionResult Index()
        {
            // Redirect to /Index (Razor Page)
            return RedirectToPage("/Index");
        }

        // ADMIN dashboard at GET /admin
        [Authorize(Roles = "Admin")]
        [HttpGet("/admin")]
        public IActionResult Admin()
            => RedirectToPage("/Admin/Index", new { area = "Identity" });
        // Razor-Page: /Areas/Identity/Pages/Admin/Index.cshtml

        // USER documents at GET /documents
        [HttpGet("/documents")]
        public IActionResult Documents()
            => RedirectToPage("/Documents/Index", new { area = "Identity" });
        // Razor-Page: /Areas/Identity/Pages/Documents/Index.cshtml

        // OPTIONAL “smart” dashboard at GET /dashboard
        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
            => User.IsInRole("Admin") ? Admin() : Documents();
    }
}
