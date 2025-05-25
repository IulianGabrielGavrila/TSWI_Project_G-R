using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetAuth.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        // ← only one constructor, taking exactly the one service you need
        [ActivatorUtilitiesConstructor]
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // a simple DTO to hold what we need per user
        public class UserDto
        {
            public string Id { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Roles { get; set; } = "";
        }

        // the list your page will bind to
        public List<UserDto> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            // 1) fetch all users
            var allUsers = _userManager.Users.ToList();

            // 2) for each, grab their roles
            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                Users.Add(new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                    Roles = string.Join(", ", roles)
                });
            }
        }
    }
}
