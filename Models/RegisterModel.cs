using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReactProjectsAuthApi.Models
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
