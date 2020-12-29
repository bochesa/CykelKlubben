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
    public class BicycleController : Controller
    {
        private readonly DataContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public BicycleController(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet("Bicycle")]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            var thisUsersBicycles = context.Bicycles.Where(b=>b.UserId == user.Id).ToList();

            return View(thisUsersBicycles);
        }
        [HttpGet("[controller]/{id}")]
        public IActionResult Bicycle(int id)
        {
            var bicycle = context.Bicycles.FirstOrDefault(b => b.Id == id);
           
            if (bicycle==null)
            {
                return NotFound($"Unable to find bicycle with Id: {id}.");
            }
            return View(bicycle);
        }
        [HttpPost("[controller]/{id}")]
        public IActionResult UpdateBicycle(Bicycle bicycle)
        {
            context.Bicycles.Update(bicycle);
            context.SaveChanges();
            return RedirectToAction("Bicycle", new { id = bicycle.Id});
        }
        [HttpGet("Bicycle/Create")]
        public IActionResult CreateBicycle()
        {
            var newBicycle = new Bicycle();
            return View(newBicycle);
        }
        [HttpPost("Bicycle/Create")]
        public async Task<IActionResult> CreateBicycle(Bicycle bicycle)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        bicycle.Picture = dataStream.ToArray();
                    }
                }
                bicycle.UserId = user.Id;
                context.Bicycles.Add(bicycle);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
