using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGymSystem.Models
{
    public class Trainer
    {
        [Key]
        public long Trainerid { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phonenumber { get; set; }
        public string? Specialization { get; set; }
        public string? Imagepath { get; set; }
        [NotMapped] public virtual IFormFile? ImageFilet { get; set; }

        public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();
        public virtual ICollection<Workoutplan> Workoutplans { get; set; } = new List<Workoutplan>();
		public virtual ICollection<MemberTrainer> MemberTrainers { get; set; } = new List<MemberTrainer>();

	}
}
