using CykelKlubben.Models;
using CykelKlubben.Models.Account;
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
        public async Task<IActionResult> MySite()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            return View(user);
        }
        
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
       
        [HttpGet("Experience")]
        public async Task<IActionResult> Experience()
        {
            var user = await userManager.GetUserAsync(User);
            var experiences = context.Experiences.Where(x=>x.UserId == user.Id).ToList();
            return View(experiences);
        }
        
        [HttpPost("Experience")]
        public IActionResult Experience(Experience experience)
        {
            if(ModelState.IsValid)
            {
                context.Update(experience);
                context.SaveChanges();
                return RedirectToAction("Experience");
            }
            return View();
        }

        [HttpGet("Experience/Create")]
        public IActionResult CreateExperience()
        {
            return View();
        }
        
        [HttpPost("Experience/Create")]
        public async Task<IActionResult> CreateExperience(Experience experience)
        {
            var user = await userManager.GetUserAsync(User);
            experience.UserId = user.Id;
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);

                        var picture = new Picture() { Image = dataStream.ToArray()};
                        experience.ExperiencePictures = new List<Picture>() { picture };
                    }
                }
                await context.Experiences.AddAsync(experience);
                await context.SaveChangesAsync();
                return RedirectToAction("Experience", new { id = experience.Id });
            }
            return View();
        }
        
        [HttpGet("Experience/{id}")]
        public IActionResult EditExperience(int id)
        {

            var experience = context.Experiences.FirstOrDefault(exp => exp.Id == id);
            experience.ExperiencePictures = context.ExperiencePictures.Where(pic => pic.ExperienceId == id).ToList();
            var expView = new ExperienceViewModel() { Experience = experience };
            return View(expView);
        }
        
        [HttpPost("Experience/{id}")]
        public async Task<IActionResult> EditExperience(int id, ExperienceViewModel exp, [FromRoute]int picId)
        {
            var experience = context.Experiences.First(exp => exp.Id == id);
            experience.Name = exp.Experience.Name;
            if (ModelState.IsValid)
            {
                
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);

                        var picture = new Picture() { Image = dataStream.ToArray() };
                        experience.ExperiencePictures = context.ExperiencePictures.ToList();
                        experience.ExperiencePictures.Add(picture);
                    }
                }
            }
            context.Update(experience);
            await context.SaveChangesAsync();
            return RedirectToAction("EditExperience", new { id = experience.Id});
        }
        
        [HttpPost]
        public IActionResult DeletePicture(int id, int picid)
        {
            var pic = context.ExperiencePictures.First(x => x.Id == picid);
            var returnid = pic.ExperienceId;
            context.ExperiencePictures.Remove(pic);
            context.SaveChanges();

            return RedirectToAction("EditExperience", new { Id = returnid });
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
