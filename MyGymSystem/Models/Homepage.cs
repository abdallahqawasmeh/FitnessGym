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

        // =========================
        // ✅ NEW (ADD ONLY) - optional hero/cta
        // =========================
        public string? HeroTitle { get; set; }
        public string? HeroSubtitle { get; set; }
        public string? HeroButtonText { get; set; }
        public string? HeroButtonUrl { get; set; }

        // =========================
        // ✅ NEW (ADD ONLY) - optional 3 image slider paths (Home page only)
        // (Keeping your existing Sliderimagepath as-is)
        // =========================
        [NotMapped] public IFormFile? SliderImage1 { get; set; }
        public string? Sliderimagepath1 { get; set; }

        [NotMapped] public IFormFile? SliderImage2 { get; set; }
        public string? Sliderimagepath2 { get; set; }

        [NotMapped] public IFormFile? SliderImage3 { get; set; }
        public string? Sliderimagepath3 { get; set; }



        public string? SliderCaption1 { get; set; }
        public string? SliderCaption2 { get; set; }
        public string? SliderCaption3 { get; set; }

    }
}
