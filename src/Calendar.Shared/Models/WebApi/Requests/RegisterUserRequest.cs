using System.ComponentModel.DataAnnotations;

namespace Calendar.Shared.Models.WebApi.Requests
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, ErrorMessage = "The username must be between 6 and 30 characters long.", MinimumLength = 6)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(30, ErrorMessage = "The password must be between 6 and 30 characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
