using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Staybnb.Models;
using Staybnb.Services;
namespace Staybnb.Areas.Identity.Pages.Account;
public class RegisterModel:PageModel
{
 readonly UserManager<ApplicationUser> _users; readonly SignInManager<ApplicationUser> _signIn; readonly IActivityLogger _log;
 public RegisterModel(UserManager<ApplicationUser> u,SignInManager<ApplicationUser>s,IActivityLogger l){_users=u;_signIn=s;_log=l;}
 [BindProperty]public InputModel Input{get;set;}=new();
 public class InputModel{[Required]public string FullName{get;set;}="";[Required,EmailAddress]public string Email{get;set;}="";[Required,DataType(DataType.Password),MinLength(6)]public string Password{get;set;}="";[Compare(nameof(Password))]public string ConfirmPassword{get;set;}="";}
 public void OnGet(){}
 public async Task<IActionResult> OnPostAsync(){if(!ModelState.IsValid)return Page();var u=new ApplicationUser{UserName=Input.Email,Email=Input.Email,FullName=Input.FullName};var r=await _users.CreateAsync(u,Input.Password);if(!r.Succeeded){foreach(var e in r.Errors)ModelState.AddModelError("",e.Description);return Page();}await _users.AddToRoleAsync(u,"Guest");await _signIn.SignInAsync(u,false);await _log.LogAsync(u.Id,"Registration","New user registered as Guest");return LocalRedirect("/");}
}
