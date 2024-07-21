using Microsoft.AspNetCore.Mvc;
using BuisnessLogicLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signinManager;
        public readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> SigninManger, ApplicationDbContext context)
        {
            _userManager= userManager;
            _signinManager= SigninManger;
            _context= context;
        }
        //public IActionResult Login()
        //{
        //    var response = new LogInViewModel(); //in case of reload the page it will save the inputs
        //    return View(response);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Login(LogInViewModel loginVM) 
        //{
        //    if (!ModelState.IsValid)
        //        return View(loginVM);

        //    var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);

        //    if (user != null)
        //    { 
        //        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
        //        if (passwordCheck == true)
        //        { 
        //            var result=await _signinManager.PasswordSignInAsync(user, loginVM.Password,false,false);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Index", "Dashboard");
        //            }
        //        }
        //        TempData["Error"] = "Wrong Credentials. Please Try Again";
        //        return View(loginVM);
        //    }
        //    TempData["Error"] = "Wrong credentials. Please try again";
        //    return View(loginVM);
        //}
        public IActionResult Register()
        {
            var response = new RegisterViewModel(); //in case of reload the page it will save the inputs
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }


            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress
            };

            var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (result.Succeeded) 
            { 
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                 
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    //ModelState.AddModelError(string.Empty, error.Description);
                    TempData["Error"] = error.Description;
                    return View(registerViewModel);
                }
            }
            return RedirectToAction("Login", "Home");

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }
    }
}
