using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Admin
    {
        [Key]
        public long Adminid { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phonenumber { get; set; }
        public DateTime? Dateofbirth { get; set; }
        public string? Imagepath { get; set; }
        [NotMapped] public virtual IFormFile? ImageFilee { get; set; }

        public virtual ICollection<Aboutuspage> Aboutuspages { get; set; } = new List<Aboutuspage>();
        public virtual ICollection<Contactuspage> Contactuspages { get; set; } = new List<Contactuspage>();
        public virtual ICollection<Homepage> Homepages { get; set; } = new List<Homepage>();
        public virtual ICollection<Membershipplan> Membershipplans { get; set; } = new List<Membershipplan>();
        public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
        public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();

        public virtual ICollection<Trainerspage> Trainerspages { get; set; } = new List<Trainerspage>();
        public virtual ICollection<Membershipplanspage> Membershipplanspages { get; set; } = new List<Membershipplanspage>();
        public virtual ICollection<Testimonialspage> Testimonialspages { get; set; } = new List<Testimonialspage>();


    }




}
