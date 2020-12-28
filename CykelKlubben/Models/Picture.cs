namespace CykelKlubben.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public bool IsShareable { get; set; } = true;
        public int ExperienceId { get; set; }


    }
}
