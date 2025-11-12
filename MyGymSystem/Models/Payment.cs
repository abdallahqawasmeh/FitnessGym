using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Payment
    {
        [Key]
        public long Paymentid { get; set; }
        public DateTime Paymentdate { get; set; } = DateTime.UtcNow;
        public decimal Amountpaid { get; set; }

        // e.g., "Cash", "Card", "Stripe", "PayPal"
        public string Paymentmethod { get; set; } = null!;

        // "Pending", "Succeeded", "Failed", "Refunded"
        public string Paymentstatus { get; set; } = "Pending";

        // NEW: safe provider reference
        public string? Externaltransactionid { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = [];

        // optional to keep; with unique FK on Subscription.Paymentid this is effectively 0..1
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
