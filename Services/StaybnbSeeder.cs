using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;

namespace Staybnb.Services;

public class StaybnbSeeder
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly ApplicationDbContext _db;

    public StaybnbSeeder(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles, ApplicationDbContext db)
    { _users = users; _roles = roles; _db = db; }

    public async Task SeedAsync()
    {
        string[] roleNames = ["Guest", "Host", "Admin", "SuperAdmin"];
        foreach (var role in roleNames)
            if (!await _roles.RoleExistsAsync(role)) await _roles.CreateAsync(new IdentityRole(role));

        const string email = "superadmin@staybnb.local";
        var admin = await _users.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = "Staybnb SuperAdmin" };
            var result = await _users.CreateAsync(admin, "Admin123!");
            if (!result.Succeeded) throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        if (!await _users.IsInRoleAsync(admin, "SuperAdmin"))
            await _users.AddToRoleAsync(admin, "SuperAdmin");

        if (!await _db.Properties.AnyAsync())
        {
            _db.Properties.Add(new Property
            {
                Title = "Sample Staybnb Apartment", Description = "A sample property for testing.",
                City = "Pretoria", Address = "Pretoria, Gauteng", PropertyType = "Apartment",
                PricePerNight = 850, CleaningFee = 150, ServiceFee = 100, IsActive = true, HostId = admin.Id
            });
            await _db.SaveChangesAsync();
        }
    }
}
