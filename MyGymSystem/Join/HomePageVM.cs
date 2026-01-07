using MyGymSystem.Models;

namespace MyGymSystem.Join
{

    public class HomePageVM
    {
        public Homepage? Home { get; set; }
        public Aboutuspage? About { get; set; }
        public Contactuspage? Contact { get; set; }

        public List<Membershipplan> Plans { get; set; } = new();
        public List<Trainer> Trainers { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
    }
}
