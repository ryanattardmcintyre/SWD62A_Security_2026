using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize(Roles ="admin,DBADMIN")]
    public class AdminController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;
        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager
            ) 
        {
            _userManager = userManager;
            _roleManager = roleManager;

            
        }

        
        public async Task<IActionResult> PopulateDefaultRolesAsync()
        {
            string[] roles = { "admin", "reader", "user", "writer" };

            foreach (string role in roles)
            {
                bool exists = await _roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole(role));

                    if (!roleResult.Succeeded)
                    {
                        return Content($"Failed to create role: {role}");
                    }
                }
            }
            var users = await _userManager.Users.ToListAsync() ; //this closes the connection with the database
                                                                    //making it a fixed list
            foreach (var user in users)
            {
                bool exists = await _userManager.IsInRoleAsync(user, "user");
                if (!exists)
                {
                    var result = await _userManager.AddToRoleAsync(user, "user");
                    if (!result.Succeeded)
                        return Content($"Failed assigning role to {user.UserName}");
                }
            }

            return Content("done");
        }

        public async Task<IActionResult> AddRole(string roleName) 
        {
            //differeniate between addrole vs addtoRole
            //note:
            //     addrole => adds a new role to the AspNetRoles
            //    addToRole=> adds a record to the AspNetUserRoles

            await _roleManager.CreateAsync(new IdentityRole() { Name = roleName });
            return Content("Done");
        }

        public async Task<IActionResult> AllocateRole(string username, string roleName) { 
                var user = await _userManager.FindByEmailAsync(username);

            bool exists = await _userManager.IsInRoleAsync(user, roleName);
            if (!exists)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                    return Content($"Failed assigning role to {user.UserName}");
            }

            return Content("done");
        }

        public async Task<IActionResult> DeallocateRole(string username, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(username);

            bool exists = await _userManager.IsInRoleAsync(user, roleName);
            if (exists == true)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                    return Content($"Failed assigning role to {user.UserName}");
            }

            return Content("done");
        }
    }

    /*  public IActionResult AddRole() { }
      public IActionResult DeleteRole() { }
      public IActionResult AllocateRole() { }

      public IActionResult DeallocateRole()
      { }*/



}
