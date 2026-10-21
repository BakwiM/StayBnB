using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Staybnb.Models;
using Staybnb.Services;
namespace Staybnb.Areas.Identity.Pages.Account;
public class LogoutModel:PageModel
{
 readonly SignInManager<ApplicationUser> _signIn; readonly UserManager<ApplicationUser> _users; readonly IActivityLogger _log;
 public LogoutModel(SignInManager<ApplicationUser>s,UserManager<ApplicationUser>u,IActivityLogger l){_signIn=s;_users=u;_log=l;}
 public void OnGet(){}
 public async Task<IActionResult> OnPostAsync(){var id=_users.GetUserId(User);await _signIn.SignOutAsync();await _log.LogAsync(id,"Logout","User logged out");return RedirectToPage("/Account/Login");}
}
