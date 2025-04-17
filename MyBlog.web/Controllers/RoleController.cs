using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.domain.Models;
using MyBlog.domain.ViewModels;
using Newtonsoft.Json;

namespace MyBlog.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager,UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // add new role
        public async Task<IActionResult> AddNewRole() => View();

        [HttpPost]
        public async Task<IActionResult> AddNewRole(IdentityRole identityRole)
        {
            string roleName = identityRole.Name;
            // check if name is null or empty
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("Name", "Role name is required");
                return View(identityRole);
            }
            // check if role already exists
            var isRoleExist = await _roleManager.RoleExistsAsync(roleName);
            if(isRoleExist)
            {
                ModelState.AddModelError("Name", "Role already exists");
                return View(identityRole);
            }
            var role = new IdentityRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
            // create role
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Name", error.Description);
            }
            return View(identityRole);


        }
        // get all roles
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        // delete role

        [HttpPost]
        public async Task<IActionResult> Delete (string roleId)
        {
            var  role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Roles");
        }


        // all users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Asign Role To User 
        public async Task<IActionResult> AsignRoleToUser(string userId)
        {
            // get user by id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Posts","Post");
            // get all user roles
            var userRoles =  await _userManager.GetRolesAsync(user);
            // get all roles in my system
            var roles = await _roleManager.Roles.ToListAsync(); // []
            if (!roles.Any())
            {
                // add defualt roles
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
                await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
                // reload roles
                // ["Admin", "User"]
                roles = await _roleManager.Roles.ToListAsync();
            }
            // create a list of role view model
            var roleList = roles.Select(r => new RoleViewModel
            {
                RoleId = r.Id,
                RoleName = r.Name,
                UserRole = userRoles.Contains(r.Name) // check if user has this role
                //UserRole = userRoles.Any(x => x == r.Name)
            }).ToList();

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.FullName = user.FullName;
            return View(roleList);







        }

        // post method
        [HttpPost]
        public async Task<IActionResult> AsignRoleToUser(string userId,string jsonRoles)
        {
            // get user by id
            var user = await _userManager.FindByIdAsync(userId);
            
            if (string.IsNullOrWhiteSpace(jsonRoles))
            {
                ModelState.AddModelError("Role", "Please select at least one role");
                return RedirectToAction("AsignRoleToUser", new { userId = userId });
            }
            List<RoleViewModel> MyRoles = JsonConvert.DeserializeObject<List<RoleViewModel>>(jsonRoles);
            if(user is not null )
            {
                // get all user roles
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach(var role in MyRoles)
                {
                    // check if the role is exist in the user roles but not selected in the view
                    // remove the role from the user
                    // check if user has this role             ! false 
                    if (userRoles.Contains(role.RoleName.Trim()) && !role.UserRole)
                    {
                        // delete old role
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName.Trim());
                    }
                    // check if the role is not exist in the user roles but selected in the view
                    // add the role to the user
                    if(!userRoles.Contains(role.RoleName.Trim()) && role.UserRole)
                    {
                        // add new role
                        await _userManager.AddToRoleAsync(user, role.RoleName.Trim());
                    }
                }
                return RedirectToAction("Users");
            }
            ModelState.AddModelError("Role", "User not found");
            return RedirectToAction("AsignRoleToUser", new { userId = userId });


        }


    }
}
