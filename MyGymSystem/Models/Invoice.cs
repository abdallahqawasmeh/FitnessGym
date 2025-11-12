using System.ComponentModel.DataAnnotations;

namespace MyGymSystem.Models
{
    public class Invoice
    {
        [Key]
        public long Invoiceid { get; set; }
        public long? Paymentid { get; set; }
        public DateTime Invoicedate { get; set; }
        public long? Memberid { get; set; }
        public string? Filepath { get; set; }
        public bool? Status { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Discount { get; set; }
        public decimal Totalamount { get; set; }

        public virtual Member? Member { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}
