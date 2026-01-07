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

        // =========================
        // ✅ NEW (ADD ONLY) - page header/intro
        // =========================
        public string? PageTitle { get; set; }
        public string? IntroText { get; set; }

        // =========================
        // ✅ NEW (ADD ONLY) - values section (3 blocks)
        // =========================
        public string? Value1Title { get; set; }
        public string? Value1Text { get; set; }
        public string? Value2Title { get; set; }
        public string? Value2Text { get; set; }
        public string? Value3Title { get; set; }
        public string? Value3Text { get; set; }
    }
}
