using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProTrack.Data;
using ProTrack.Models;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    /// Projects controller handling project management functionality
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(ApplicationDbContext context, ILogger<ProjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// Display list of projects for the current user with optional search functionality
        /// Supports wildcard search using * for any characters
        /// <param name="searchTerm">Optional search term to filter projects (supports * wildcard)</param>
        /// <returns>Projects index view</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get projects with related client information
                var projectsQuery = _context.Projects
                    .Include(p => p.Client)
                    .Where(p => p.UserId == userId);

                // Apply search filter if search term is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Sanitize search term: convert wildcard * to SQL LIKE pattern %, escape SQL wildcards
                    string sanitizedTerm = searchTerm
                        .Replace("%", "[%]")
                        .Replace("_", "[_]")
                        .Replace("[", "[[]")
                        .Replace("*", "%");
                    
                    // Search across multiple fields: Title, Description, Client Name
                    projectsQuery = projectsQuery.Where(p =>
                        EF.Functions.Like(p.Title, $"%{sanitizedTerm}%") ||
                        EF.Functions.Like(p.Description ?? "", $"%{sanitizedTerm}%") ||
                        EF.Functions.Like(p.Client.Name, $"%{sanitizedTerm}%")
                    );
                    
                    ViewBag.SearchTerm = searchTerm;
                }

                var projects = await projectsQuery
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading projects for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading projects.";
                return View(new List<Project>());
            }
        }
        /// Display project details
        /// <param name="id">Project ID</param>
        /// <returns>Project details view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var project = await _context.Projects
                    .Include(p => p.Client)
                    .Include(p => p.TimeEntries)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project details for project {ProjectId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading project details.";
                return RedirectToAction(nameof(Index));
            }
        }
        /// Display create project form
        /// <returns>Create project view</returns>
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

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create project form for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the create form.";
                return RedirectToAction(nameof(Index));
            }
        }
        /// Create new project
        /// <param name="project">Project data</param>
        /// <returns>Redirect to projects index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,HourlyRate,Status,StartDate,EndDate,ClientId")] Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    
                    // Verify client belongs to user
                    var clientExists = await _context.Clients
                        .AnyAsync(c => c.Id == project.ClientId && c.UserId == userId && c.IsActive);
                    
                    if (!clientExists)
                    {
                        ModelState.AddModelError("ClientId", "Selected client is not valid.");
                    }
                    else
                    {
                        project.UserId = userId;
                        project.CreatedDate = DateTime.UtcNow;
                        project.LastModified = DateTime.UtcNow;

                        _context.Add(project);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Project created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating project for user {UserId}", GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while creating the project.";
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
                _logger.LogError(ex, "Error reloading clients for create project form");
            }

            return View(project);
        }
        /// Display edit project form
        /// <param name="id">Project ID</param>
        /// <returns>Edit project view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = GetCurrentUserId();
                
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (project == null)
                {
                    return NotFound();
                }

                // Get user's active clients for dropdown
                ViewBag.ClientId = new SelectList(
                    await _context.Clients
                        .Where(c => c.UserId == userId && c.IsActive)
                        .OrderBy(c => c.Name)
                        .ToListAsync(),
                    "Id", "Name", project.ClientId);

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit project form for project {ProjectId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }
        /// Update project
        /// <param name="id">Project ID</param>
        /// <param name="project">Updated project data</param>
        /// <returns>Redirect to projects index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,HourlyRate,Status,StartDate,EndDate,ClientId,UserId,CreatedDate")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (project.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verify client belongs to user
                    var clientExists = await _context.Clients
                        .AnyAsync(c => c.Id == project.ClientId && c.UserId == userId && c.IsActive);
                    
                    if (!clientExists)
                    {
                        ModelState.AddModelError("ClientId", "Selected client is not valid.");
                    }
                    else
                    {
                        project.LastModified = DateTime.UtcNow;
                        _context.Update(project);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Project updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
                    _logger.LogError(ex, "Error updating project {ProjectId} for user {UserId}", id, GetCurrentUserId());
                    TempData["ErrorMessage"] = "An error occurred while updating the project.";
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
                    "Id", "Name", project.ClientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading clients for edit project form");
            }

            return View(project);
        }
        /// Display delete confirmation
        /// <param name="id">Project ID</param>
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
                
                var project = await _context.Projects
                    .Include(p => p.Client)
                    .Include(p => p.TimeEntries)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete project form for project {ProjectId} and user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the delete confirmation.";
                return RedirectToAction(nameof(Index));
            }
        }
        /// Delete project
        /// <param name="id">Project ID</param>
        /// <returns>Redirect to projects index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (project != null)
                {
                    // Check if project has time entries
                    var hasTimeEntries = await _context.TimeEntries.AnyAsync(te => te.ProjectId == id);
                    
                    if (hasTimeEntries)
                    {
                        TempData["WarningMessage"] = "Cannot delete project with existing time entries. Please remove time entries first.";
                    }
                    else
                    {
                        _context.Projects.Remove(project);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Project deleted successfully.";
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId} for user {UserId}", id, GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while deleting the project.";
                return RedirectToAction(nameof(Index));
            }
        }
        /// Check if project exists for current user
        /// <param name="id">Project ID</param>
        /// <returns>True if project exists</returns>
        private bool ProjectExists(int id)
        {
            var userId = GetCurrentUserId();
            return _context.Projects.Any(e => e.Id == id && e.UserId == userId);
        }
        /// Get current user ID from claims
        /// <returns>Current user ID</returns>
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}