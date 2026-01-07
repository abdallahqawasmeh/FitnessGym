using MyGymSystem.Models;

namespace MyGymSystem.Join
{
    public class MemberPrivatePlanVM
    {
        public Trainer? Trainer { get; set; }
        public List<Workoutplan> Plans { get; set; } = new();
    }

}
