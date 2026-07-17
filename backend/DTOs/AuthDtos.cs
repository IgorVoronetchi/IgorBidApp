using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class RegisterDto
    {
        [Required, StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; } = "";

        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = "";

        [Required, StringLength(100, MinimumLength = 8,
            ErrorMessage = "Parola trebuie sa aiba minim 8 caractere.")]
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = "";

        [Required, StringLength(100)]
        public string Password { get; set; } = "";
    }
}
