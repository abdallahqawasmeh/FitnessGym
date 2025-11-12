using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MyGymSystem.Models
{
    public class Userlogin
    {
        [Key]
        public long Loginid { get; set; }
        public string Username { get; set; } = null!;
        public string Passwordhash { get; set; } = null!;
        public long? Roleid { get; set; }
        public long? Trainerid { get; set; }
        public long? Memberid { get; set; }
        public long? Adminid { get; set; }

        public virtual Admin? Admin { get; set; }
        public virtual Member? Member { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Trainer? Trainer { get; set; }
    }
}
