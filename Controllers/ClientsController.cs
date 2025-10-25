using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProTrack.Data;
using ProTrack.Models;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Display list of clients for the current user with optional search functionality
        /// Supports wildcard search using * for any characters
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter clients (supports * wildcard)</param>
        /// <returns>Clients index view</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            var userId = GetCurrentUserId();
            
            // Start with base query for active clients
            var clientsQuery = _context.Clients
                .Where(c => c.UserId == userId && c.IsActive);

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Convert wildcard * to SQL LIKE pattern %
                string searchPattern = searchTerm.Replace("*", "%");
                
                // Search across multiple fields: Name, Email, Phone, Address, Notes
                clientsQuery = clientsQuery.Where(c =>
                    EF.Functions.Like(c.Name, $"%{searchPattern}%") ||
                    EF.Functions.Like(c.ContactEmail ?? "", $"%{searchPattern}%") ||
                    EF.Functions.Like(c.PhoneNumber ?? "", $"%{searchPattern}%") ||
                    EF.Functions.Like(c.Address ?? "", $"%{searchPattern}%") ||
                    EF.Functions.Like(c.Notes ?? "", $"%{searchPattern}%")
                );
                
                ViewBag.SearchTerm = searchTerm;
            }

            var clients = await clientsQuery
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .Include(c => c.Projects)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ContactEmail,PhoneNumber,Address,Notes,IsActive")] Client client)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                client.UserId = userId;
                client.CreatedDate = DateTime.UtcNow;

                _context.Add(client);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactEmail,PhoneNumber,Address,Notes,IsActive,UserId,CreatedDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (client.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .Include(c => c.Projects)
                .Include(c => c.Invoices)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client != null)
            {
                // Check if client has associated projects or invoices
                var hasProjects = await _context.Projects.AnyAsync(p => p.ClientId == id);
                var hasInvoices = await _context.Invoices.AnyAsync(i => i.ClientId == id);

                if (hasProjects || hasInvoices)
                {
                    // Soft delete - mark as inactive instead of hard delete
                    client.IsActive = false;
                    _context.Update(client);
                    TempData["InfoMessage"] = "Client has been deactivated due to existing projects or invoices.";
                }
                else
                {
                    // Hard delete - no dependencies
                    _context.Clients.Remove(client);
                    TempData["SuccessMessage"] = "Client deleted successfully.";
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
            {
                return NotFound();
            }

            client.IsActive = false;
            _context.Update(client);
            await _context.SaveChangesAsync();

            TempData["InfoMessage"] = "Client has been deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Reactivate/5
        public async Task<IActionResult> Reactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
            {
                return NotFound();
            }

            client.IsActive = true;
            _context.Update(client);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Client has been reactivated.";
            return RedirectToAction(nameof(Index));
        }

        // AJAX endpoint to check if client name is unique
        [HttpGet]
        public async Task<IActionResult> IsClientNameUnique(string name, int? id)
        {
            var userId = GetCurrentUserId();
            var exists = await _context.Clients
                .AnyAsync(c => c.UserId == userId && c.Name.ToLower() == name.ToLower() && c.Id != id);

            return Json(!exists);
        }

        private bool ClientExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Clients.Any(e => e.Id == id && e.UserId == userId);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
