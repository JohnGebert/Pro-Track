using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProTrack.Data;
using ProTrack.Models;
using ProTrack.Services;
using System.Security.Claims;
using System.Linq;
using System.Text;

namespace ProTrack.Controllers
{
    /// Invoices controller handling invoice management functionality
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoicesController> _logger;
        private readonly IAiDescriptionService _aiDescriptionService;

        public InvoicesController(
            ApplicationDbContext context,
            ILogger<InvoicesController> logger,
            IAiDescriptionService aiDescriptionService)
        {
            _context = context;
            _logger = logger;
            _aiDescriptionService = aiDescriptionService;
        }

        /// Display list of invoices for the current user with optional search functionality
        /// Supports wildcard search using * for any characters
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
                    // Sanitize search term: convert wildcard * to SQL LIKE pattern %, escape SQL wildcards
                    string sanitizedTerm = searchTerm
                        .Replace("%", "[%]")
                        .Replace("_", "[_]")
                        .Replace("[", "[[]")
                        .Replace("*", "%");
                    
                    // Search across multiple fields: Invoice Number, Client Name, Notes
                    invoicesQuery = invoicesQuery.Where(i =>
                        EF.Functions.Like(i.InvoiceNumber, $"%{sanitizedTerm}%") ||
                        EF.Functions.Like(i.Client.Name, $"%{sanitizedTerm}%") ||
                        EF.Functions.Like(i.Notes ?? "", $"%{sanitizedTerm}%")
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
        /// Display invoice details
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
        /// Display create invoice form
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
        /// Create new invoice
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
        /// Display edit invoice form
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
        /// Update invoice
        /// <param name="id">Invoice ID</param>
        /// <param name="invoice">Updated invoice data</param>
        /// <returns>Redirect to invoices index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,InvoiceDate,DueDate,TotalAmount,Notes,InvoiceNumber,UserId,CreatedDate,IsPaid,PaymentDate")] Invoice invoice)
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

            // Read IsPaid directly from form to ensure we get the checkbox value
            // When checkbox is checked, it submits "true", when unchecked, it doesn't submit (or submits false)
            bool isPaid = false;
            if (Request.Form.ContainsKey("IsPaid"))
            {
                var isPaidValues = Request.Form["IsPaid"];
                // Check if any of the values is "true"
                if (isPaidValues.Count > 0)
                {
                    isPaid = isPaidValues.Any(v => !string.IsNullOrEmpty(v) && v.Equals("true", StringComparison.OrdinalIgnoreCase));
                }
            }
            
            // Debug: Log the received values
            _logger.LogInformation("Edit POST - Invoice ID: {Id}, IsPaid from form: {IsPaid}, Invoice.IsPaid model: {InvoiceIsPaid}, PaymentDate: {PaymentDate}", 
                invoice.Id, isPaid, invoice.IsPaid, invoice.PaymentDate);
            
            // Override invoice.IsPaid with the value from the form
            invoice.IsPaid = isPaid;

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
                        // Fetch the existing invoice from database to avoid tracking issues
                        var existingInvoice = await _context.Invoices
                            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
                        
                        if (existingInvoice == null)
                        {
                            return NotFound();
                        }
                        
                        // Update properties
                        existingInvoice.ClientId = invoice.ClientId;
                        existingInvoice.InvoiceDate = invoice.InvoiceDate;
                        existingInvoice.DueDate = invoice.DueDate;
                        existingInvoice.TotalAmount = invoice.TotalAmount;
                        existingInvoice.Notes = invoice.Notes;
                        existingInvoice.InvoiceNumber = invoice.InvoiceNumber;
                        existingInvoice.LastModified = DateTime.UtcNow;
                        
                        // Update payment status - use the value we calculated from form
                        _logger.LogInformation("Updating invoice {InvoiceId}: IsPaid from form = {IsPaid}, Invoice.IsPaid = {InvoiceIsPaid}, Current DB = {Current}", 
                            id, isPaid, invoice.IsPaid, existingInvoice.IsPaid);
                        existingInvoice.IsPaid = isPaid; // Use the form value we calculated earlier
                        
                        // If marked as paid and no payment date, set it to today
                        if (isPaid && !invoice.PaymentDate.HasValue)
                        {
                            existingInvoice.PaymentDate = DateTime.UtcNow;
                            _logger.LogInformation("Setting payment date to today for invoice {InvoiceId}", id);
                        }
                        // If payment date was manually set, use it
                        else if (invoice.PaymentDate.HasValue)
                        {
                            existingInvoice.PaymentDate = invoice.PaymentDate;
                            _logger.LogInformation("Using manually set payment date {PaymentDate} for invoice {InvoiceId}", invoice.PaymentDate, id);
                        }
                        // If marked as unpaid, clear payment date
                        else if (!isPaid)
                        {
                            existingInvoice.PaymentDate = null;
                            _logger.LogInformation("Clearing payment date for unpaid invoice {InvoiceId}", id);
                        }
                        
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
        /// Display delete confirmation
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
        /// Delete invoice
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
        /// Toggle paid status of invoice
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
        /// Generates AI-assisted invoice notes based on a short user prompt.
        /// </summary>
        /// <param name="request">Prompt and invoice metadata.</param>
        /// <returns>JSON result with generated notes.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateNotes([FromBody] GenerateInvoiceNotesRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return Json(new { success = false, message = "Please provide a quick prompt so we can generate invoice notes." });
            }

            try
            {
                var userId = GetCurrentUserId();

                string? clientLabel = null;

                if (request.ClientId.HasValue)
                {
                    var client = await _context.Clients
                        .FirstOrDefaultAsync(c => c.Id == request.ClientId.Value && c.UserId == userId);

                    if (client != null)
                    {
                        clientLabel = !string.IsNullOrWhiteSpace(client.Address)
                            ? $"{client.Name} — {client.Address}"
                            : client.Name;
                    }
                }

                var contextBuilder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(clientLabel))
                {
                    contextBuilder.AppendLine($"Client: {clientLabel}");
                }

                if (request.TotalAmount.HasValue)
                {
                    contextBuilder.AppendLine($"Invoice Total: ${request.TotalAmount.Value:F2}");
                }

                if (request.InvoiceDate.HasValue)
                {
                    contextBuilder.AppendLine($"Invoice Date: {request.InvoiceDate.Value:MMM dd, yyyy}");
                }

                if (request.DueDate.HasValue)
                {
                    var referenceDate = request.InvoiceDate ?? DateTime.UtcNow;
                    var dueInDays = (int)Math.Round((request.DueDate.Value.Date - referenceDate.Date).TotalDays);
                    contextBuilder.AppendLine($"Due Date: {request.DueDate.Value:MMM dd, yyyy} ({dueInDays} day terms)");
                }

                var aiRequest = new AiDescriptionRequest
                {
                    Prompt = request.Prompt,
                    ProjectName = clientLabel,
                    AdditionalContext = contextBuilder.Length > 0 ? contextBuilder.ToString().Trim() : null
                };

                var result = await _aiDescriptionService.GenerateDescriptionAsync(aiRequest);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Error ?? "The assistant could not generate notes right now." });
                }

                return Json(new { success = true, notes = result.Description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI invoice notes for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "An unexpected error occurred while generating notes." });
            }
        }
        /// Generate invoice from time entries
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
        /// Generate next invoice number
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
        /// Check if invoice exists for current user
        /// <param name="id">Invoice ID</param>
        /// <returns>True if invoice exists</returns>
        private bool InvoiceExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Invoices.Any(e => e.Id == id && e.UserId == userId);
        }
        /// Get current user ID from claims
        /// <returns>Current user ID</returns>
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        /// <summary>
        /// Request payload for AI-generated invoice notes.
        /// </summary>
        public class GenerateInvoiceNotesRequest
        {
            public string Prompt { get; set; } = string.Empty;
            public int? ClientId { get; set; }
            public decimal? TotalAmount { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public DateTime? DueDate { get; set; }
        }
    }
}