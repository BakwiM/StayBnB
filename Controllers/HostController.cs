using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Services;
using Staybnb.ViewModels;

namespace Staybnb.Controllers;

[Authorize(Roles="Host")]
public class HostController : Controller
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users; private readonly IActivityLogger _log;
    public HostController(ApplicationDbContext db, UserManager<ApplicationUser> users, IActivityLogger log) { _db=db; _users=users; _log=log; }
    public async Task<IActionResult> Dashboard()
    {
        var id=_users.GetUserId(User)!;
        ViewBag.PendingBookings=await _db.Bookings.Include(b=>b.Property).Where(b=>b.Property!.HostId==id && b.Status==BookingStatus.Pending).ToListAsync();
        ViewBag.PendingDocuments = await _db.GuestDocuments.Include(d => d.Booking).ThenInclude(b => b!.Property)
        .Where(d => d.Booking!.Property!.HostId == id && !d.Verified).ToListAsync();
        var hostReviews = await _db.Reviews.Where(r => r.Property!.HostId == id).ToListAsync();
        ViewBag.AverageRating = hostReviews.Any() ? Math.Round(hostReviews.Average(r => r.Rating), 1) : 0;
        return View(await _db.Properties.Include(p=>p.Images).Where(p=>p.HostId==id).ToListAsync());
    }
    
    public async Task<IActionResult> Reviews()
    {
        var id = _users.GetUserId(User)!;
        var reviews = await _db.Reviews.Include(r => r.Property).Include(r => r.Reviewer)
            .Where(r => r.Property!.HostId == id).OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(reviews);
    }
    public async Task<IActionResult> Earnings()
    {
        var id = _users.GetUserId(User)!;
        var bookings = await _db.Bookings.Include(b => b.Property)
            .Where(b => b.Property!.HostId == id && (b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn))
            .OrderByDescending(b => b.CreatedAt).ToListAsync();
        ViewBag.TotalEarnings = bookings.Sum(b => b.TotalPrice);
        return View(bookings);
    }
    public async Task<IActionResult> Toggle(int id)
    {
        var uid=_users.GetUserId(User)!; var p=await _db.Properties.FirstOrDefaultAsync(x=>x.Id==id && x.HostId==uid);
        if(p==null) return NotFound(); p.IsActive=!p.IsActive; await _db.SaveChangesAsync();
        await _log.LogAsync(uid,"Property status changed",$"Property {id} IsActive={p.IsActive}"); return RedirectToAction(nameof(Dashboard));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BookingDecision(int id, bool approve)
    {
        var uid=_users.GetUserId(User)!; var b=await _db.Bookings.Include(x=>x.Property).FirstOrDefaultAsync(x=>x.Id==id && x.Property!.HostId==uid);
        if(b==null) return NotFound(); b.Status=approve?BookingStatus.Approved:BookingStatus.Rejected;
        _db.Notifications.Add(new Notification{UserId=b.GuestId,Title=$"Booking {(approve?"Approved":"Rejected")}",Message=$"Your booking for {b.Property!.Title} has been {(approve?"approved":"rejected")}."});
        await _db.SaveChangesAsync(); await _log.LogAsync(uid,"Booking status update",$"Booking {id}: {b.Status}");
        return RedirectToAction(nameof(Dashboard));
    }
    [HttpGet] public async Task<IActionResult> CheckIn(int propertyId)
    {
        var uid=_users.GetUserId(User)!; var p=await _db.Properties.Include(x=>x.CheckInProcess).FirstOrDefaultAsync(x=>x.Id==propertyId&&x.HostId==uid);
        if(p==null) return NotFound(); return View(p);
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CheckIn(int propertyId,string rules,string requiredDocuments)
    {
        var uid=_users.GetUserId(User)!; var p=await _db.Properties.Include(x=>x.CheckInProcess).FirstOrDefaultAsync(x=>x.Id==propertyId&&x.HostId==uid);
        if(p==null) return NotFound();
        if(p.CheckInProcess==null) p.CheckInProcess=new CheckInProcess{PropertyId=propertyId};
        p.CheckInProcess.Rules=rules; p.CheckInProcess.RequiredDocuments=requiredDocuments; await _db.SaveChangesAsync();
        await _log.LogAsync(uid,"Check-in process updated",$"Property {propertyId}"); return RedirectToAction(nameof(Dashboard));
    }
    [HttpGet] public async Task<IActionResult> VerifyDocument(int id)
    {
        var uid=_users.GetUserId(User)!; var d=await _db.GuestDocuments.Include(x=>x.Booking).ThenInclude(b=>b!.Property).FirstOrDefaultAsync(x=>x.Id==id&&x.Booking!.Property!.HostId==uid);
        if(d==null) return NotFound(); return View(d);
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> VerifyDocument(int id,bool verified)
    {
        var uid=_users.GetUserId(User)!; var d=await _db.GuestDocuments.Include(x=>x.Booking).ThenInclude(b=>b!.Property).FirstOrDefaultAsync(x=>x.Id==id&&x.Booking!.Property!.HostId==uid);
        if(d==null) return NotFound(); d.Verified=verified; if(verified) d.Booking!.Status=BookingStatus.CheckedIn;
        await _db.SaveChangesAsync(); await _log.LogAsync(uid,"Guest document verification",$"Document {id}: {verified}");
        return RedirectToAction(nameof(Dashboard));
    }
}
