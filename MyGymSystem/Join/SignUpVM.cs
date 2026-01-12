using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyGymSystem.Models.ViewModels
{
    public class SignUpVM
    {
        // Member fields (Required unless you want them optional)
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string Firstname { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        public string Lastname { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        [StringLength(120)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        [StringLength(25)]
        public string Phonenumber { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime? Dateofbirth { get; set; }

        [Required(ErrorMessage = "Fitness goal is required")]
        [StringLength(120)]
        public string Fitnessgoal { get; set; } = null!;

        // Login fields
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 4)]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        // optional image
        public IFormFile? ImageFile { get; set; }
    }
}
