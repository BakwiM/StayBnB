using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Staybnb.Models;
using Staybnb.Services;

namespace Staybnb.Areas.Identity.Pages.Account;
public class LoginModel : PageModel
{
    readonly SignInManager<ApplicationUser> _signIn; readonly UserManager<ApplicationUser> _users; readonly IActivityLogger _log;
    public LoginModel(SignInManager<ApplicationUser> s,UserManager<ApplicationUser> u,IActivityLogger l){_signIn=s;_users=u;_log=l;}
    [BindProperty] public InputModel Input{get;set;}=new();
    public class InputModel{[Required,EmailAddress]public string Email{get;set;}="";[Required,DataType(DataType.Password)]public string Password{get;set;}="";}
    public void OnGet(){}
    public async Task<IActionResult> OnPostAsync()
    {
        if(!ModelState.IsValid)return Page();
        var user=await _users.FindByEmailAsync(Input.Email);if(user==null){ModelState.AddModelError("","Invalid login.");return Page();}
        var result=await _signIn.PasswordSignInAsync(user,Input.Password,false,false);if(!result.Succeeded){ModelState.AddModelError("","Invalid login.");return Page();}
        await _log.LogAsync(user.Id,"Login","Successful login");return LocalRedirect("/");
    }
}
