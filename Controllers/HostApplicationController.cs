using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Services;
using Staybnb.ViewModels;

namespace Staybnb.Controllers;

[Authorize(Roles="Guest")]
public class HostApplicationController : Controller
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users; private readonly IActivityLogger _log; private readonly IWebHostEnvironment _env;
    public HostApplicationController(ApplicationDbContext db, UserManager<ApplicationUser> users,IActivityLogger log,IWebHostEnvironment env){_db=db;_users=users;_log=log;_env=env;}
    [HttpGet] public IActionResult Apply()=>View(new HostApplicationVm());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(HostApplicationVm vm)
    {
        if(vm.Image==null || vm.Image.Length==0) ModelState.AddModelError(nameof(vm.Image),"At least one property image is required.");
        if(!ModelState.IsValid) return View(vm);
        var userId=_users.GetUserId(User)!; var dir=Path.Combine(_env.WebRootPath,"uploads");
        Directory.CreateDirectory(dir); var name=Guid.NewGuid()+Path.GetExtension(vm.Image!.FileName); var path=Path.Combine(dir,name);
        using(var stream=System.IO.File.Create(path)) await vm.Image.CopyToAsync(stream);
        _db.HostApplications.Add(new HostApplication{ApplicantId=userId,Title=vm.Title,Description=vm.Description,City=vm.City,Address=vm.Address,PropertyType=vm.PropertyType,PricePerNight=vm.PricePerNight,CleaningFee=vm.CleaningFee,ServiceFee=vm.ServiceFee,ImagePath="/uploads/"+name});
        await _db.SaveChangesAsync(); await _log.LogAsync(userId,"Host application submitted",vm.Title);
        return RedirectToAction("Index","Home");
    }
}
