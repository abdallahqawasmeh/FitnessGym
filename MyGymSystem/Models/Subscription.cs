using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{

    public class Subscription
    {
        [Key]
        public long Subscriptionid { get; set; }

        // Recommended: make them NOT nullable
        public long Memberid { get; set; }
        public long Planid { get; set; }

        // ✅ NEW: chosen trainer (only for Personal/Private plans)
        public long? Trainerid { get; set; }
        public virtual Trainer? Trainer { get; set; }

        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }

        public long? Paymentid { get; set; }
        public bool? Status { get; set; }
        public DateTime Createdat { get; set; } = DateTime.UtcNow;

        public virtual Member Member { get; set; } = null!;
        public virtual Membershipplan Plan { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }

    //public class Subscription
    //{
    //    [Key]
    //    public long Subscriptionid { get; set; }
    //    public long? Memberid { get; set; }
    //    public long? Planid { get; set; }
    //    public DateTime Startdate { get; set; }
    //    public DateTime Enddate { get; set; }

    //    // keep nullable until paid; enforce UNIQUE index in Fluent API
    //    public long? Paymentid { get; set; }

    //    public bool? Status { get; set; }

    //    // NEW: exact creation time for admin filters
    //    public DateTime Createdat { get; set; } = DateTime.UtcNow;

    //    public virtual Member? Member { get; set; }
    //    public virtual Payment? Payment { get; set; }
    //    public virtual Membershipplan? Plan { get; set; }
    //}
}
