using DotNetAuth.Data;
using DotNetAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DotNetAuth.Areas.Identity.Pages.Documents
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;
        public IList<Document> Documents { get; set; } = new List<Document>();

        public IndexModel(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task OnGetAsync()
        {
            var userId = _um.GetUserId(User)!;
            Documents = await _db.Documents
                                 .Where(d => d.OwnerId == userId)
                                 .ToListAsync();
        }
    }
}