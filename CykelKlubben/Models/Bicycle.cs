using System.ComponentModel.DataAnnotations;

namespace CykelKlubben.Models
{
    public class Bicycle
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Moel")]
        public string Model { get; set; }
        [Required]
        [Display(Name = "Antal Gear")]
        public int NumberOfGears { get; set; }
        [Required]
        [Display(Name = "Billede")]
        public byte[] Picture { get; set; }
        public string UserId { get; set; }


    }
}
