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
    /// Time entries controller handling time tracking functionality
    /// </summary>
    [Authorize]
    public class TimeEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimeEntriesController> _logger;

        public TimeEntriesController(ApplicationDbContext context, ILogger<TimeEntriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display list of time entries for the current user
        /// </summary>
        /// <returns>Time entries index view</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get time entries with related project and client information
                var timeEntries = await _context.TimeEntries
                    .Include(te => te.Project)
                    .ThenInclude(p => p.Client)
                    .Where(te => te.UserId == userId)
                    .OrderByDescending(te => te.StartTime)
                    .ToListAsync();

                return View(timeEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entries for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading time entries.";
                return View(new List<TimeEntry>());
            }
        }

        /// <summary>
        /// Display time entry details
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <returns>Time entry details view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Project)
                    .ThenInclude(p => p.Client)
                    .FirstOrDefaultAsync(te => te.Id == id && te.UserId == userId);

                if (timeEntry == null)
                {
                    return NotFound();
                }

                return View(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry details for entry {TimeEntryId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading time entry details.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Display create time entry form
        /// </summary>
        /// <returns>Create time entry view</returns>
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get user's active projects for dropdown
                ViewBag.ProjectId = new SelectList(
                    await _context.Projects
                        .Include(p => p.Client)
                        .Where(p => p.UserId == userId && p.Status == ProjectStatus.Active)
                        .OrderBy(p => p.Title)
                        .ToListAsync(),
                    "Id", "Title");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create time entry form for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the create form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Create new time entry
        /// </summary>
        /// <param name="timeEntry">Time entry data</param>
        /// <returns>Redirect to time entries index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,StartTime,EndTime,Description,IsBilled")] TimeEntry timeEntry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    
                    // Validate project belongs to user
                    var projectExists = await _context.Projects
                        .AnyAsync(p => p.Id == timeEntry.ProjectId && p.UserId == userId);
                    
                    if (!projectExists)
                    {
                        ModelState.AddModelError("ProjectId", "Selected project is not valid.");
                    }
                    else if (timeEntry.EndTime <= timeEntry.StartTime)
                    {
                        ModelState.AddModelError("EndTime", "End time must be after start time.");
                    }
                    else
                    {
                        timeEntry.UserId = userId;
                        timeEntry.CreatedDate = DateTime.UtcNow;
                        timeEntry.LastModified = DateTime.UtcNow;

                        _context.Add(timeEntry);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Time entry created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating time entry for user {UserId}", GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while creating the time entry.";
                }
            }

            // Reload projects for dropdown if validation fails
            try
            {
                var userId = GetCurrentUserId();
                ViewBag.ProjectId = new SelectList(
                    await _context.Projects
                        .Include(p => p.Client)
                        .Where(p => p.UserId == userId && p.Status == ProjectStatus.Active)
                        .OrderBy(p => p.Title)
                        .ToListAsync(),
                    "Id", "Title");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading projects for create time entry form");
            }

            return View(timeEntry);
        }

        /// <summary>
        /// Display edit time entry form
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <returns>Edit time entry view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == id && te.UserId == userId);

                if (timeEntry == null)
                {
                    return NotFound();
                }

                // Get user's active projects for dropdown
                ViewBag.ProjectId = new SelectList(
                    await _context.Projects
                        .Include(p => p.Client)
                        .Where(p => p.UserId == userId && p.Status == ProjectStatus.Active)
                        .OrderBy(p => p.Title)
                        .ToListAsync(),
                    "Id", "Title", timeEntry.ProjectId);

                return View(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit time entry form for entry {TimeEntryId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Update time entry
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <param name="timeEntry">Updated time entry data</param>
        /// <returns>Redirect to time entries index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,StartTime,EndTime,Description,IsBilled,UserId,CreatedDate")] TimeEntry timeEntry)
        {
            if (id != timeEntry.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (timeEntry.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate project belongs to user
                    var projectExists = await _context.Projects
                        .AnyAsync(p => p.Id == timeEntry.ProjectId && p.UserId == userId);
                    
                    if (!projectExists)
                    {
                        ModelState.AddModelError("ProjectId", "Selected project is not valid.");
                    }
                    else if (timeEntry.EndTime <= timeEntry.StartTime)
                    {
                        ModelState.AddModelError("EndTime", "End time must be after start time.");
                    }
                    else
                    {
                        timeEntry.LastModified = DateTime.UtcNow;
                        _context.Update(timeEntry);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Time entry updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeEntryExists(timeEntry.Id))
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
                    _logger.LogError(ex, "Error updating time entry {TimeEntryId} for user {UserId}", id, GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while updating the time entry.";
                }
            }

            // Reload projects for dropdown if validation fails
            try
            {
                ViewBag.ProjectId = new SelectList(
                    await _context.Projects
                        .Include(p => p.Client)
                        .Where(p => p.UserId == userId && p.Status == ProjectStatus.Active)
                        .OrderBy(p => p.Title)
                        .ToListAsync(),
                    "Id", "Title", timeEntry.ProjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading projects for edit time entry form");
            }

            return View(timeEntry);
        }

        /// <summary>
        /// Display delete confirmation
        /// </summary>
        /// <param name="id">Time entry ID</param>
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
                
                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Project)
                    .ThenInclude(p => p.Client)
                    .FirstOrDefaultAsync(te => te.Id == id && te.UserId == userId);

                if (timeEntry == null)
                {
                    return NotFound();
                }

                return View(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete time entry form for entry {TimeEntryId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the delete confirmation.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Delete time entry
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <returns>Redirect to time entries index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == id && te.UserId == userId);

                if (timeEntry != null)
                {
                    _context.TimeEntries.Remove(timeEntry);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Time entry deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time entry {TimeEntryId} for user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while deleting the time entry.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Toggle billed status of time entry
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <returns>JSON result</returns>
        [HttpPost]
        public async Task<IActionResult> ToggleBilled(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == id && te.UserId == userId);

                if (timeEntry == null)
                {
                    return Json(new { success = false, message = "Time entry not found." });
                }

                timeEntry.IsBilled = !timeEntry.IsBilled;
                timeEntry.LastModified = DateTime.UtcNow;

                _context.Update(timeEntry);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    isBilled = timeEntry.IsBilled,
                    message = timeEntry.IsBilled ? "Time entry marked as billed." : "Time entry marked as unbilled."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling billed status for time entry {TimeEntryId} and user {UserId}", id, GetCurrentUserId());
                return Json(new { success = false, message = "An error occurred while updating the time entry." });
            }
        }

        /// <summary>
        /// Check if time entry exists for current user
        /// </summary>
        /// <param name="id">Time entry ID</param>
        /// <returns>True if time entry exists</returns>
        private bool TimeEntryExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.TimeEntries.Any(e => e.Id == id && e.UserId == userId);
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