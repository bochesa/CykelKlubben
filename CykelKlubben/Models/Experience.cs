using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CykelKlubben.Models
{
    public class Experience
    {
        public int Id { get; set; }
        //public bool IsShareable { get; set; }
        [Required]
        [Display(Name = "Navn")]
        public string Name { get; set; }
        public List<Picture> ExperiencePictures { get; set; }
        public string UserId { get; set; }

    }
}
