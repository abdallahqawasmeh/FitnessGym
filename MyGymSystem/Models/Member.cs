using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long Memberid { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phonenumber { get; set; }
        public DateTime? Dateofbirth { get; set; }
        public string? Fitnessgoal { get; set; }
        public string? Imagepath { get; set; }
        [NotMapped] public  IFormFile? ImageFile { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

        public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();

        // CHANGED: direct relation to plans (drop Workoutplanmember)
        public virtual ICollection<Workoutplan> Workoutplans { get; set; } = new List<Workoutplan>();

        public virtual ICollection<MemberTrainer> MemberTrainers { get; set; } = new List<MemberTrainer>();
	}
}
