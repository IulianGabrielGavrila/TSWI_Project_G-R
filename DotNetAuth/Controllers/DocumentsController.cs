using DotNetAuth.Data;
using DotNetAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetAuth.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _um;

        public DocumentsController(ApplicationDbContext context, UserManager<IdentityUser> um)
        {
            _context = context;
            _um = um;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var userId = _um.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var docs = isAdmin
                ? await _context.Documents.ToListAsync()
                : await _context.Documents.Where(d => d.OwnerId == userId).ToListAsync();

            return View(docs);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Document document)
        {
            if (ModelState.IsValid)
            {
                // 1) Get current user’s ID
                var userId = _um.GetUserId(User);

                // 2) Assign it as the owner
                document.OwnerId = userId;

                // 3) Save
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // No OwnerId dropdown needed anymore
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            

            var document = await _context.Documents.FindAsync(id);

            var userId = _um.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && document.OwnerId != userId)
                return Forbid();   // or RedirectToAction("AccessDenied", "Account");

            if (document == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", document.OwnerId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,OwnerId")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", document.OwnerId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userId = _um.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && document.OwnerId != userId)
                return Forbid();   // or RedirectToAction("AccessDenied", "Account");
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            var userId = _um.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && document.OwnerId != userId)
                return Forbid();   // or RedirectToAction("AccessDenied", "Account");
            if (document != null)
            {
                _context.Documents.Remove(document);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
