using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProTrack.Data;
using ProTrack.Models;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    /// <summary>
    /// Invoices controller handling invoice management functionality
    /// </summary>
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(ApplicationDbContext context, ILogger<InvoicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display list of invoices for the current user with optional search functionality
        /// Supports wildcard search using * for any characters
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter invoices (supports * wildcard)</param>
        /// <returns>Invoices index view</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get invoices with related client information
                var invoicesQuery = _context.Invoices
                    .Include(i => i.Client)
                    .Include(i => i.TimeEntries)
                    .Where(i => i.UserId == userId);

                // Apply search filter if search term is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Convert wildcard * to SQL LIKE pattern %
                    string searchPattern = searchTerm.Replace("*", "%");
                    
                    // Search across multiple fields: Invoice Number, Client Name, Notes
                    invoicesQuery = invoicesQuery.Where(i =>
                        EF.Functions.Like(i.InvoiceNumber, $"%{searchPattern}%") ||
                        EF.Functions.Like(i.Client.Name, $"%{searchPattern}%") ||
                        EF.Functions.Like(i.Notes ?? "", $"%{searchPattern}%")
                    );
                    
                    ViewBag.SearchTerm = searchTerm;
                }

                var invoices = await invoicesQuery
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToListAsync();

                return View(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading invoices for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading invoices.";
                return View(new List<Invoice>());
            }
        }

        /// <summary>
        /// Display invoice details
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice details view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var invoice = await _context.Invoices
                    .Include(i => i.Client)
                    .Include(i => i.TimeEntries)
                    .ThenInclude(te => te.Project)
                    .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

                if (invoice == null)
                {
                    return NotFound();
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading invoice details for invoice {InvoiceId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading invoice details.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Display create invoice form
        /// </summary>
        /// <returns>Create invoice view</returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get user's active clients for dropdown
                ViewBag.ClientId = new SelectList(
                    await _context.Clients
                        .Where(c => c.UserId == userId && c.IsActive)
                        .OrderBy(c => c.Name)
                        .ToListAsync(),
                    "Id", "Name");

                // Generate next invoice number
                var nextInvoiceNumber = await GenerateNextInvoiceNumber(userId);

                var invoice = new Invoice
                {
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceNumber = nextInvoiceNumber,
                    DueDate = DateTime.UtcNow.AddDays(30)
                };

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create invoice form for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the create form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Create new invoice
        /// </summary>
        /// <param name="invoice">Invoice data</param>
        /// <returns>Redirect to invoices index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,InvoiceDate,DueDate,TotalAmount,Notes,InvoiceNumber")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    
                    // Verify client belongs to user
                    var clientExists = await _context.Clients
                        .AnyAsync(c => c.Id == invoice.ClientId && c.UserId == userId && c.IsActive);
                    
                    if (!clientExists)
                    {
                        ModelState.AddModelError("ClientId", "Selected client is not valid.");
                    }
                    else
                    {
                        invoice.UserId = userId;
                        invoice.CreatedDate = DateTime.UtcNow;
                        invoice.LastModified = DateTime.UtcNow;

                        _context.Add(invoice);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Invoice created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating invoice for user {UserId}", GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while creating the invoice.";
                }
            }

            // Reload clients for dropdown if validation fails
            try
            {
                var userId = GetCurrentUserId();
                ViewBag.ClientId = new SelectList(
                    await _context.Clients
                        .Where(c => c.UserId == userId && c.IsActive)
                        .OrderBy(c => c.Name)
                        .ToListAsync(),
                    "Id", "Name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading clients for create invoice form");
            }

            return View(invoice);
        }

        /// <summary>
        /// Display edit invoice form
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Edit invoice view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

                if (invoice == null)
                {
                    return NotFound();
                }

                // Get user's active clients for dropdown
                ViewBag.ClientId = new SelectList(
                    await _context.Clients
                        .Where(c => c.UserId == userId && c.IsActive)
                        .OrderBy(c => c.Name)
                        .ToListAsync(),
                    "Id", "Name", invoice.ClientId);

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit invoice form for invoice {InvoiceId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Update invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="invoice">Updated invoice data</param>
        /// <returns>Redirect to invoices index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,InvoiceDate,DueDate,TotalAmount,Notes,InvoiceNumber,UserId,CreatedDate")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (invoice.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verify client belongs to user
                    var clientExists = await _context.Clients
                        .AnyAsync(c => c.Id == invoice.ClientId && c.UserId == userId && c.IsActive);
                    
                    if (!clientExists)
                    {
                        ModelState.AddModelError("ClientId", "Selected client is not valid.");
                    }
                    else
                    {
                        invoice.LastModified = DateTime.UtcNow;
                        _context.Update(invoice);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Invoice updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating invoice {InvoiceId} for user {UserId}", id, GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while updating the invoice.";
                }
            }

            // Reload clients for dropdown if validation fails
            try
            {
                ViewBag.ClientId = new SelectList(
                    await _context.Clients
                        .Where(c => c.UserId == userId && c.IsActive)
                        .OrderBy(c => c.Name)
                        .ToListAsync(),
                    "Id", "Name", invoice.ClientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading clients for edit invoice form");
            }

            return View(invoice);
        }

        /// <summary>
        /// Display delete confirmation
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Delete confirmation view</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var invoice = await _context.Invoices
                    .Include(i => i.Client)
                    .Include(i => i.TimeEntries)
                    .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

                if (invoice == null)
                {
                    return NotFound();
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete invoice form for invoice {InvoiceId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the delete confirmation.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Delete invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Redirect to invoices index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

                if (invoice != null)
                {
                    _context.Invoices.Remove(invoice);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Invoice deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice {InvoiceId} for user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while deleting the invoice.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Toggle paid status of invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>JSON result</returns>
        [HttpPost]
        public async Task<IActionResult> TogglePaid(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

                if (invoice == null)
                {
                    return Json(new { success = false, message = "Invoice not found." });
                }

                invoice.IsPaid = !invoice.IsPaid;
                invoice.PaymentDate = invoice.IsPaid ? DateTime.UtcNow : null;
                invoice.LastModified = DateTime.UtcNow;

                _context.Update(invoice);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    isPaid = invoice.IsPaid,
                    paymentDate = invoice.PaymentDate?.ToString("MMM dd, yyyy"),
                    message = invoice.IsPaid ? "Invoice marked as paid." : "Invoice marked as unpaid."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling paid status for invoice {InvoiceId} and user {UserId}", id, GetCurrentUserId());
                return Json(new { success = false, message = "An error occurred while updating the invoice." });
            }
        }

        /// <summary>
        /// Generate invoice from time entries
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="projectId">Project ID (optional)</param>
        /// <param name="startDate">Start date for time entries</param>
        /// <param name="endDate">End date for time entries</param>
        /// <returns>JSON result with invoice data</returns>
        [HttpPost]
        public async Task<IActionResult> GenerateFromTimeEntries(int clientId, int? projectId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Verify client belongs to user
                var clientExists = await _context.Clients
                    .AnyAsync(c => c.Id == clientId && c.UserId == userId && c.IsActive);
                
                if (!clientExists)
                {
                    return Json(new { success = false, message = "Selected client is not valid." });
                }

                // Get unbilled time entries
                var query = _context.TimeEntries
                    .Include(te => te.Project)
                    .Where(te => te.UserId == userId && !te.IsBilled && te.Project.ClientId == clientId);

                if (projectId.HasValue)
                {
                    query = query.Where(te => te.ProjectId == projectId.Value);
                }

                var timeEntries = await query
                    .Where(te => te.StartTime.Date >= startDate.Date && te.StartTime.Date <= endDate.Date)
                    .ToListAsync();

                if (!timeEntries.Any())
                {
                    return Json(new { success = false, message = "No unbilled time entries found for the selected criteria." });
                }

                // Calculate total amount
                var totalAmount = timeEntries.Sum(te => te.Amount);

                return Json(new { 
                    success = true, 
                    timeEntries = timeEntries.Select(te => new {
                        id = te.Id,
                        description = te.Description,
                        projectTitle = te.Project.Title,
                        startTime = te.StartTime.ToString("MMM dd, yyyy HH:mm"),
                        duration = te.DurationInHours,
                        amount = te.Amount
                    }),
                    totalAmount = totalAmount,
                    entryCount = timeEntries.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice from time entries for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "An error occurred while generating the invoice data." });
            }
        }

        /// <summary>
        /// Generate next invoice number
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Next invoice number</returns>
        private async Task<string> GenerateNextInvoiceNumber(string userId)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"INV-{year}-";
            
            var lastInvoice = await _context.Invoices
                .Where(i => i.UserId == userId && i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            if (lastInvoice == null)
            {
                return $"{prefix}001";
            }

            var lastNumber = lastInvoice.InvoiceNumber.Replace(prefix, "");
            if (int.TryParse(lastNumber, out int number))
            {
                return $"{prefix}{(number + 1):D3}";
            }

            return $"{prefix}001";
        }

        /// <summary>
        /// Check if invoice exists for current user
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>True if invoice exists</returns>
        private bool InvoiceExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Invoices.Any(e => e.Id == id && e.UserId == userId);
        }

        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        /// <returns>Current user ID</returns>
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}