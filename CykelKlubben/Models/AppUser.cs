using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CykelKlubben.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternavn")]
        public string LastName { get; set; }

        [Display(Name = "Profilbilede")]
        public byte[] ProfilePicture { get; set; }
    }
}
