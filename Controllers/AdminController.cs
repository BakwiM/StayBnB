using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Services;

namespace Staybnb.Controllers;
[Authorize(Roles="Admin,SuperAdmin")]
public class AdminController : Controller
{
    readonly ApplicationDbContext _db; readonly UserManager<ApplicationUser> _users; readonly IActivityLogger _log;
    public AdminController(ApplicationDbContext db,UserManager<ApplicationUser> users,IActivityLogger log){_db=db;_users=users;_log=log;}
    public async Task<IActionResult> Dashboard()
    {
        var apps = await _db.HostApplications.Include(x => x.Applicant).OrderByDescending(x => x.Id).ToListAsync();
        ViewBag.TotalUsers = await _users.Users.CountAsync();
        ViewBag.PendingHostApps = apps.Count(a => a.Status == ApplicationStatus.Pending);
        ViewBag.ActiveProperties = await _db.Properties.CountAsync(p => p.IsActive);
        ViewBag.PlatformRevenue = await _db.Bookings.Where(b => b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn).SumAsync(b => b.TotalPrice);
        ViewBag.TotalBookings = await _db.Bookings.CountAsync();
        ViewBag.ApprovedHosts = (await _users.GetUsersInRoleAsync("Host")).Count;
        return View(apps);
    }
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Decide(int id,bool approve)
    {
        var a=await _db.HostApplications.FirstOrDefaultAsync(x=>x.Id==id);if(a==null)return NotFound();
        if(a.Status!=ApplicationStatus.Pending)return BadRequest();
        a.Status=approve?ApplicationStatus.Approved:ApplicationStatus.Rejected;
        if(approve){
            var user=await _users.FindByIdAsync(a.ApplicantId); if(user==null)return NotFound();
            await _users.AddToRoleAsync(user,"Host"); await _users.RemoveFromRoleAsync(user,"Guest");
            var p=new Property{HostId=user.Id,Title=a.Title,Description=a.Description,City=a.City,Address=a.Address,PropertyType=a.PropertyType,PricePerNight=a.PricePerNight,CleaningFee=a.CleaningFee,ServiceFee=a.ServiceFee,IsActive=true};
            _db.Properties.Add(p); await _db.SaveChangesAsync();
            _db.PropertyImages.Add(new PropertyImage{PropertyId=p.Id,ImagePath=a.ImagePath});
            _db.Notifications.Add(new Notification{UserId=user.Id,Title="Host Application Approved",Message="Your property application has been approved. You are now a Host."});
        } else _db.Notifications.Add(new Notification{UserId=a.ApplicantId,Title="Host Application Rejected",Message="Your host application was rejected."});
        await _db.SaveChangesAsync(); await _log.LogAsync(_users.GetUserId(User),"Host application decision",$"Application {id}: {a.Status}");
        return RedirectToAction(nameof(Dashboard));
    }
    [Authorize(Roles="SuperAdmin")]
    public async Task<IActionResult> Users()=>View(await _users.GetUsersInRoleAsync("Guest"));
    [Authorize(Roles="SuperAdmin"),HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote(string id)
    {
        var user=await _users.FindByIdAsync(id);if(user==null)return NotFound();
        await _users.AddToRoleAsync(user,"Admin"); await _users.RemoveFromRoleAsync(user,"Guest");
        await _log.LogAsync(_users.GetUserId(User),"Role change",$"User {id} promoted to Admin");
        return RedirectToAction(nameof(Users));
    }
    public async Task<IActionResult> Logs()=>View(await _db.ActivityLogs.OrderByDescending(x=>x.CreatedAt).Take(500).ToListAsync());
}
