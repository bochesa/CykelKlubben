using CykelKlubben.Models;
using CykelKlubben.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CykelKlubben.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly DataContext context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DataContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        
        [HttpGet("MySite")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MySite()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            return View(user);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("MySite")]
        public async Task<IActionResult> MySite(AppUser appuser)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if(ModelState.IsValid)
            {
                user.FirstName = appuser.FirstName;
                user.LastName = appuser.LastName;
                user.UserName = appuser.UserName;
                user.PhoneNumber = appuser.PhoneNumber;
            }

            if(appuser.ProfilePicture == null)
            {
                user.ProfilePicture = appuser.ProfilePicture;

            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
            }

            await userManager.UpdateAsync(user);
            await signInManager.RefreshSignInAsync(user);
            return RedirectToAction("MySite");
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await signInManager.PasswordSignInAsync(
                login.EmailAddress, login.Password,
                login.RememberMe, false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login error!");
                return View();
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            //returnUrl = returnUrl ?? Url.Content("~/");
            await signInManager.SignOutAsync();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registration)
        {

            if (!ModelState.IsValid)
                return View(registration);

            var newUser = new AppUser()
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Email = registration.EmailAddress,
                UserName = registration.EmailAddress
            };

            var result = await userManager.CreateAsync(newUser, registration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors.Select(x => x.Description))
                {
                    ModelState.AddModelError("", error);

                }
                return View();
            }
            return RedirectToAction("Login");

        }

    }
}
