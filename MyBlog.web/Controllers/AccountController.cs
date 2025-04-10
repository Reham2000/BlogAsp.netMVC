using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.domain.Models;
using MyBlog.domain.ViewModels;

namespace MyBlog.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // register
        // get => show view

        public IActionResult Register()  => View();

        // post => register user

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(! ModelState.IsValid) 
                return View(model);
            // set user data
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };
            // create user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // sign in user automatically
                await _signInManager.SignInAsync(user, isPersistent:false);
                return RedirectToAction("Posts", "Post");


                // register only
                //return RedirectToAction("Login", "Account");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }




        // login
        // logout
    }
}
