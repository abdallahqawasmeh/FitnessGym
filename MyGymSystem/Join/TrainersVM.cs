using MyGymSystem.Models;

namespace MyGymSystem.Join
{
    public class TrainersVM
    {
        public Trainerspage? Page { get; set; }
        public List<Trainer> Trainers { get; set; } = new();
    }
}
