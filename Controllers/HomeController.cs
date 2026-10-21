using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
namespace Staybnb.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    public async Task<IActionResult> Index(string? city, string? propertyType)
    {
        var q = _db.Properties.Include(p => p.Images).Include(p => p.Reviews).Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(city)) q = q.Where(p => p.City.Contains(city));
        if (!string.IsNullOrWhiteSpace(propertyType)) q = q.Where(p => p.PropertyType == propertyType);
        ViewBag.City = city; ViewBag.PropertyType = propertyType;
        if (User.Identity?.IsAuthenticated ?? false)
        {
            var uid = _users.GetUserId(User)!;
            var currentUser = await _users.GetUserAsync(User);
            ViewBag.DisplayName = string.IsNullOrWhiteSpace(currentUser?.FullName) ? User.Identity!.Name : currentUser!.FullName;
            ViewBag.WishlistedIds = (await _db.WishlistItems.Where(w => w.UserId == uid).Select(w => w.PropertyId).ToListAsync()).ToHashSet();

        }
        else
        {
            ViewBag.WishlistedIds = new HashSet<int>();
        }
        return View(await q.OrderByDescending(p => p.Id).ToListAsync());
    }
}