using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Testimonial
    {
        [Key]
        public long Testimonialid { get; set; }
        public long? Memberid { get; set; }
        public string Feedbacktext { get; set; } = null!;


        // Only: "Pending", "Approved", "Rejected"
        public string? Approvalstatus { get; set; } = "Pending";

        public DateTime Submitteddate { get; set; }
        public long? Approvedbyadminid { get; set; }

        public virtual Admin? Approvedbyadmin { get; set; }
        public virtual Member? Member { get; set; }
    }
}
