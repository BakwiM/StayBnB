using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.ViewModels;

namespace Staybnb.Controllers;
[Authorize]
public class CommunicationController : Controller
{
    readonly ApplicationDbContext _db; readonly UserManager<ApplicationUser> _users;
    public CommunicationController(ApplicationDbContext db,UserManager<ApplicationUser> users){_db=db;_users=users;}
    public async Task<IActionResult> Inbox(){var id=_users.GetUserId(User)!;var messages=await _db.Messages.Include(x=>x.Sender).Where(x=>x.RecipientId==id).OrderByDescending(x=>x.SentAt).ToListAsync();foreach(var m in messages)m.IsRead=true;await _db.SaveChangesAsync();return View(messages);}
    [HttpGet] public async Task<IActionResult> Compose(){var id=_users.GetUserId(User)!;ViewBag.Users=await _db.Users.Where(x=>x.Id!=id).OrderBy(x=>x.Email).ToListAsync();return View(new MessageVm());}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Compose(MessageVm vm){if(!ModelState.IsValid){ViewBag.Users=await _db.Users.Where(x=>x.Id!=_users.GetUserId(User)).ToListAsync();return View(vm);}var id=_users.GetUserId(User)!;_db.Messages.Add(new Message{SenderId=id,RecipientId=vm.RecipientId,Subject=vm.Subject,Body=vm.Body});_db.Notifications.Add(new Notification{UserId=vm.RecipientId,Title="New Message",Message=$"You received a new message: {vm.Subject}"});await _db.SaveChangesAsync();return RedirectToAction(nameof(Inbox));}
    public async Task<IActionResult> Notifications(){var id=_users.GetUserId(User)!;var n=await _db.Notifications.Where(x=>x.UserId==id).OrderByDescending(x=>x.CreatedAt).ToListAsync();foreach(var x in n)x.IsRead=true;await _db.SaveChangesAsync();return View(n);}
}
