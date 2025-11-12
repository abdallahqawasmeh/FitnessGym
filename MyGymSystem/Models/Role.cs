namespace MyGymSystem.Models
{
    public class Role
    {
        public long Roleid { get; set; }
        public string Rolename { get; set; } = null!;
        public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();
    }
}
