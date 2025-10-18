using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProTrack.Data;
using System.Security.Claims;

namespace ProTrack.Controllers
{
    /// <summary>
    /// Home controller handling the main dashboard and landing page
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Main dashboard page displaying overview statistics and recent activity
        /// </summary>
        /// <returns>Dashboard view with statistics</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Get dashboard statistics
                var dashboardStats = await GetDashboardStatistics(userId);
                
                return View(dashboardStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return View(new DashboardViewModel());
            }
        }

        /// <summary>
        /// Privacy policy page
        /// </summary>
        /// <returns>Privacy view</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error page handler
        /// </summary>
        /// <returns>Error view</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Retrieves dashboard statistics for the current user
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <returns>Dashboard statistics</returns>
        private async Task<DashboardViewModel> GetDashboardStatistics(string userId)
        {
            var stats = new DashboardViewModel
            {
                UserId = userId
            };

            try
            {
                // Get basic counts
                stats.TotalClients = await _context.Clients
                    .CountAsync(c => c.UserId == userId && c.IsActive);

                stats.TotalProjects = await _context.Projects
                    .CountAsync(p => p.UserId == userId);

                stats.ActiveProjects = await _context.Projects
                    .CountAsync(p => p.UserId == userId && p.Status == Models.ProjectStatus.Active);

                stats.TotalInvoices = await _context.Invoices
                    .CountAsync(i => i.UserId == userId);

                stats.UnpaidInvoices = await _context.Invoices
                    .CountAsync(i => i.UserId == userId && !i.IsPaid);

                // Get unbilled hours
                var unbilledTimeEntries = await _context.TimeEntries
                    .Where(te => te.UserId == userId && !te.IsBilled)
                    .ToListAsync();

                stats.UnbilledHours = unbilledTimeEntries.Sum(te => te.DurationInHours);

                // Get total revenue (paid invoices)
                stats.TotalRevenue = await _context.Invoices
                    .Where(i => i.UserId == userId && i.IsPaid)
                    .SumAsync(i => i.TotalAmount);

                // Get pending revenue (unpaid invoices)
                stats.PendingRevenue = await _context.Invoices
                    .Where(i => i.UserId == userId && !i.IsPaid)
                    .SumAsync(i => i.TotalAmount);

                // Get recent activity
                stats.RecentClients = await _context.Clients
                    .Where(c => c.UserId == userId && c.IsActive)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .Select(c => new RecentClientViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CreatedDate = c.CreatedDate,
                        ProjectCount = c.Projects.Count()
                    })
                    .ToListAsync();

                stats.RecentProjects = await _context.Projects
                    .Include(p => p.Client)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(5)
                    .Select(p => new RecentProjectViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        ClientName = p.Client.Name,
                        Status = p.Status,
                        CreatedDate = p.CreatedDate,
                        HourlyRate = p.HourlyRate
                    })
                    .ToListAsync();

                stats.RecentTimeEntries = await _context.TimeEntries
                    .Include(te => te.Project)
                    .ThenInclude(p => p.Client)
                    .Where(te => te.UserId == userId)
                    .OrderByDescending(te => te.StartTime)
                    .Take(5)
                    .Select(te => new RecentTimeEntryViewModel
                    {
                        Id = te.Id,
                        Description = te.Description,
                        ProjectTitle = te.Project.Title,
                        ClientName = te.Project.Client.Name,
                        StartTime = te.StartTime,
                        Duration = te.DurationInHours,
                        IsBilled = te.IsBilled
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics for user {UserId}", userId);
                // Return empty stats if there's an error
            }

            return stats;
        }

        /// <summary>
        /// Gets the current user ID from claims
        /// </summary>
        /// <returns>Current user ID</returns>
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }

    /// <summary>
    /// View model for dashboard statistics
    /// </summary>
    public class DashboardViewModel
    {
        public string UserId { get; set; } = string.Empty;
        
        // Basic Statistics
        public int TotalClients { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalInvoices { get; set; }
        public int UnpaidInvoices { get; set; }
        
        // Financial Statistics
        public decimal UnbilledHours { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        
        // Recent Activity
        public List<RecentClientViewModel> RecentClients { get; set; } = new();
        public List<RecentProjectViewModel> RecentProjects { get; set; } = new();
        public List<RecentTimeEntryViewModel> RecentTimeEntries { get; set; } = new();
    }

    /// <summary>
    /// View model for recent client activity
    /// </summary>
    public class RecentClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int ProjectCount { get; set; }
    }

    /// <summary>
    /// View model for recent project activity
    /// </summary>
    public class RecentProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public ProTrack.Models.ProjectStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal HourlyRate { get; set; }
    }

    /// <summary>
    /// View model for recent time entry activity
    /// </summary>
    public class RecentTimeEntryViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public decimal Duration { get; set; }
        public bool IsBilled { get; set; }
    }
}
