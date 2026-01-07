using MyGymSystem.Models;

namespace MyGymSystem.Join
{
    public class PlansVM
    {
        public Membershipplanspage? Page { get; set; }
        public List<Membershipplan> Plans { get; set; } = new();
    }
}
