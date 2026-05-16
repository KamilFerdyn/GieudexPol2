using System.ComponentModel.DataAnnotations;

namespace GieudexPol.Application.Auth.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}