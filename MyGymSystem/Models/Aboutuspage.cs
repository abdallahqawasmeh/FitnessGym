using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Aboutuspage
    {
        [Key]
        public long Aboutuspageid { get; set; }
        public string? Missionstatement { get; set; }
        public string? Visionstatement { get; set; }
        public string? Teamdescription { get; set; }
        public string? Imagepath { get; set; }
        [NotMapped] public virtual IFormFile? ImageFiler { get; set; }
        public long? Updatedbyadminid { get; set; }
        public DateTime? Lastupdated { get; set; }
        public virtual Admin? Updatedbyadmin { get; set; }
    }
}
