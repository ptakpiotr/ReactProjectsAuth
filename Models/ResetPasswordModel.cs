using System.ComponentModel.DataAnnotations;

namespace ReactProjectsAuthApi.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}
