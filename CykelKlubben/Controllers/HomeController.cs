using CykelKlubben.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CykelKlubben.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IdentityDataContext idcontext;
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext context;

        public HomeController(ILogger<HomeController> logger, 
            IdentityDataContext idcontext, 
            UserManager<AppUser> usermanager, DataContext context)
        {
            _logger = logger;
            this.idcontext = idcontext;
            this.userManager = usermanager;
            this.context = context;
        }
        [Route("")]
        [Route("/[controller]")]
        [Route("/[controller]/index")]
        public IActionResult Index()
        {
            var pictures = context.ExperiencePictures.ToList();
            var random = new Random();
            var rndPictures = new List<Picture>();
            if(pictures.Count <= 3)
            {
                return View(pictures);
            }
            if(pictures.Count > 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Picture rndPic = null;
                    while(rndPic==null)
                    {
                        var index = random.Next(pictures.Count);
                        //rndPic = pictures.FirstOrDefault(x => x.Id == index);
                        rndPic = pictures.ElementAt(index);
                        if (!rndPictures.Contains(rndPic) && rndPic!=null)
                        {
                            rndPictures.Add(rndPic);
                        }
                        else
                            rndPic = null;
                    }
                }
            }
            return View(rndPictures);
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public IActionResult Members()
        {
            var users = idcontext.Users.ToList();
            return View(users);
        }
        

        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactMe contactMe)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.From = new MailAddress(contactMe.Email);
                    mailMsg.To.Add("bochesa@gmail.com");
                    mailMsg.Subject = contactMe.Subject;
                    mailMsg.Body = contactMe.Message;
                    mailMsg.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential
                    ("bochesa@gmail.com", "ixzcuftfrxwgjoxk");
                    smtp.Send(mailMsg);

                    ModelState.Clear();
                    TempData["Feedback"] = "Tak fordi du skrev til os!";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    TempData["Feedback"] = $"Beklager, der er opstået et problem {ex.Message}";
                }
            }

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
