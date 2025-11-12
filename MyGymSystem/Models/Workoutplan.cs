using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Workoutplan
    {
        [Key]
        public long Workoutplanid { get; set; }

        public long? Trainerid { get; set; }
        public virtual Trainer? Trainer { get; set; }

        // NEW: one plan targets one member (customized)
        public long Memberid { get; set; }
        public virtual Member Member { get; set; } = null!;

        public string Planname { get; set; } = null!;
        public string? Plandescription { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string? Routinedetails { get; set; }
        public string? Schedule { get; set; }
        public string? Goal { get; set; }
        public bool? Status { get; set; }
    }
}
