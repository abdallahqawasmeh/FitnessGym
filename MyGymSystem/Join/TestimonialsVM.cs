using MyGymSystem.Models;

namespace MyGymSystem.Join
{
    public class TestimonialsVM
    {
        public Testimonialspage? Page { get; set; }
        public List<Testimonial> Testimonials { get; set; } = new();

        // Keep your existing form strongly typed (you already use @model Testimonial in your view)
        public Testimonial Form { get; set; } = new Testimonial();
    }
}
