using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Testimonialspage
    {
        [Key]
        public long Testimonialspageid { get; set; }

        public string? PageTitle { get; set; }
        public string? IntroText { get; set; }

        public string? Bannerimagepath { get; set; }
        [NotMapped] public virtual IFormFile? Bannerimage { get; set; }

        public long? Updatedbyadminid { get; set; }
        public DateTime? Lastupdated { get; set; }
        public virtual Admin? Updatedbyadmin { get; set; }
    }
}
