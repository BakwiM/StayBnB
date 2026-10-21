using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.ViewModels;

namespace Staybnb.Controllers;
[Authorize(Roles="Guest")]
public class BookingController : Controller
{
    readonly ApplicationDbContext _db; readonly UserManager<ApplicationUser> _users;
    public BookingController(ApplicationDbContext db,UserManager<ApplicationUser> users){_db=db;_users=users;}
    [HttpGet] public async Task<IActionResult> Create(int id){var p=await _db.Properties.FindAsync(id); if(p==null||!p.IsActive)return NotFound(); return View(new BookingVm{PropertyId=id});}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingVm vm)
    {
        var p=await _db.Properties.FindAsync(vm.PropertyId); if(p==null||!p.IsActive)return NotFound();
        if(vm.CheckOut<=vm.CheckIn) ModelState.AddModelError("", "Check-out must be after check-in.");
        var overlap=await _db.Bookings.AnyAsync(b=>b.PropertyId==vm.PropertyId && b.Status!=BookingStatus.Rejected && vm.CheckIn<b.CheckOut && vm.CheckOut>b.CheckIn);
        if(overlap) ModelState.AddModelError("", "The property is not available for those dates.");
        if(!ModelState.IsValid) return View(vm);
        var nights=(vm.CheckOut.Date-vm.CheckIn.Date).Days;
        var total=(p.PricePerNight*nights)+p.CleaningFee+p.ServiceFee;
        var b=new Booking{PropertyId=p.Id,GuestId=_users.GetUserId(User)!,CheckIn=vm.CheckIn,CheckOut=vm.CheckOut,Travelers=vm.Travelers,TotalPrice=total};
        _db.Bookings.Add(b); _db.Notifications.Add(new Notification{UserId=p.HostId,Title="New Booking Request",Message=$"A new booking request was made for {p.Title}."});
        await _db.SaveChangesAsync(); return RedirectToAction(nameof(MyBookings));
    }
    public async Task<IActionResult> MyBookings()
    {
        var uid = _users.GetUserId(User)!;
        ViewBag.ReviewedPropertyIds = (await _db.Reviews.Where(r => r.ReviewerId == uid).Select(r => r.PropertyId).ToListAsync()).ToHashSet();
        return View(await _db.Bookings.Include(b => b.Property).Where(b => b.GuestId == uid).OrderByDescending(b => b.Id).ToListAsync());
    }
    [HttpGet] public async Task<IActionResult> CheckIn(int id){var b=await _db.Bookings.Include(x=>x.Property).FirstOrDefaultAsync(x=>x.Id==id&&x.GuestId==_users.GetUserId(User)&&x.Status==BookingStatus.Approved); if(b==null)return NotFound(); return View(b);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> CheckIn(int id,IFormFile? document)
    {
        var b=await _db.Bookings.FirstOrDefaultAsync(x=>x.Id==id&&x.GuestId==_users.GetUserId(User)&&x.Status==BookingStatus.Approved); if(b==null)return NotFound();
        if(document==null||document.Length==0){ModelState.AddModelError("","ID or Passport document is required."); return View(b);}
        var dir=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","uploads");Directory.CreateDirectory(dir);var name=Guid.NewGuid()+Path.GetExtension(document.FileName);using var s=System.IO.File.Create(Path.Combine(dir,name));await document.CopyToAsync(s);
        _db.GuestDocuments.Add(new GuestDocument{BookingId=id,DocumentPath="/uploads/"+name});await _db.SaveChangesAsync(); return RedirectToAction(nameof(MyBookings));
    }
    [HttpGet]
    public async Task<IActionResult> AddReview(int bookingId)
    {
        var uid = _users.GetUserId(User)!;
        var b = await _db.Bookings.Include(x => x.Property)
            .FirstOrDefaultAsync(x => x.Id == bookingId && x.GuestId == uid && x.Status == BookingStatus.CheckedIn);
        if (b == null) return NotFound();
        var already = await _db.Reviews.AnyAsync(r => r.PropertyId == b.PropertyId && r.ReviewerId == uid);
        if (already) return RedirectToAction(nameof(MyBookings));
        ViewBag.PropertyTitle = b.Property!.Title;
        return View(new Review { PropertyId = b.PropertyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(Review review)
    {
        var uid = _users.GetUserId(User)!;
        if (review.Rating < 1 || review.Rating > 5) ModelState.AddModelError("", "Rating must be between 1 and 5.");
        ModelState.Remove("ReviewerId");
        var ownsStay = await _db.Bookings.AnyAsync(x => x.PropertyId == review.PropertyId && x.GuestId == uid && x.Status == BookingStatus.CheckedIn);
        if (!ownsStay) return Forbid();
        if (!ModelState.IsValid) return View(review);
        var already = await _db.Reviews.AnyAsync(r => r.PropertyId == review.PropertyId && r.ReviewerId == uid);
        if (already) return RedirectToAction(nameof(MyBookings));
        review.ReviewerId = uid;
        review.Id = 0;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MyBookings));
    }
}
