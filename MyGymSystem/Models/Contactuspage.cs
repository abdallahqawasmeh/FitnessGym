using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Contactuspage
    {
        [Key]
        public long Contactuspageid { get; set; }
        public string? Address { get; set; }
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public string? Mapembedcode { get; set; }
        public string? Contenet { get; set; }
        public string? Picpath { get; set; }
        [NotMapped] public virtual IFormFile? picFile { get; set; }
        public long? Updatedbyadminid { get; set; }
        public DateTime? Lastupdated { get; set; }
        public virtual Admin? Updatedbyadmin { get; set; }
    }
}
