using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Homepage
    {
        [Key]
        public long Homepageid { get; set; }
        public string? Welcomemessage { get; set; }
        public string? Featuredcontent { get; set; }
        public long? Updatedbyadminid { get; set; }
        public DateTime? Lastupdated { get; set; }
        public string? Sliderimagepath { get; set; }
        [NotMapped] public virtual IFormFile? Sliderimage { get; set; }
        public virtual Admin? Updatedbyadmin { get; set; }
    }
}
