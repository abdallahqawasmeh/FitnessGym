using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Membershipplan
    {
        [Key]
        public long Planid { get; set; }
        public string Planname { get; set; } = null!;
        public string? Description { get; set; }
		public string? Description1 { get; set; }
		public string? Description2 { get; set; }
		public string? Description3 { get; set; }
		public string? Description4 { get; set; }

		public int Durationmonths { get; set; }
        public decimal Price { get; set; }
        public long? Createdbyadminid { get; set; }
        public string? Plantype { get; set; }
        public bool? Status { get; set; }

        public virtual Admin? Createdbyadmin { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
