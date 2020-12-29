using CykelKlubben.Models;
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
    [Authorize]
    public class ExperienceController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly DataContext context;

        public ExperienceController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            DataContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [Authorize]
        [HttpGet("Experience")]
        public async Task<IActionResult> Experience()
        {
            var user = await userManager.GetUserAsync(User);
            var experiences = context.Experiences.Where(x => x.UserId == user.Id).ToList();
            return View(experiences);
        }

        [Authorize]
        [HttpPost("Experience")]
        public IActionResult Experience(Experience experience)
        {
            if (ModelState.IsValid)
            {
                context.Update(experience);
                context.SaveChanges();
                return RedirectToAction("Experience");
            }
            return View();
        }

        [Authorize]
        [HttpGet("Experience/Create")]
        public IActionResult CreateExperience()
        {
            return View();
        }

        [Authorize]
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

                        var picture = new Picture() { Image = dataStream.ToArray() };
                        experience.ExperiencePictures = new List<Picture>() { picture };
                    }
                }
                await context.Experiences.AddAsync(experience);
                await context.SaveChangesAsync();
                return RedirectToAction("Experience", new { id = experience.Id });
            }
            return View();
        }

        [Authorize]
        [HttpGet("Experience/{id}")]
        public IActionResult EditExperience(int id)
        {
            var experience = context.Experiences.FirstOrDefault(exp => exp.Id == id);
            experience.ExperiencePictures = context.ExperiencePictures.Where(pic => pic.ExperienceId == id).ToList();
            var expView = new ExperienceViewModel() { Experience = experience };
            return View(expView);
        }

        [Authorize]
        [HttpPost("Experience/{id}")]
        public async Task<IActionResult> EditExperience(int id, ExperienceViewModel exp)
        {
            var experience = context.Experiences.First(exp => exp.Id == id);
            experience.ExperiencePictures = context.ExperiencePictures.Where(pic => pic.ExperienceId == id).ToList();
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
                        experience.ExperiencePictures.Add(picture);
                    }
                }
            }
            context.Update(experience);
            await context.SaveChangesAsync();
            return RedirectToAction("EditExperience", new { id = experience.Id });
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
    }
}
