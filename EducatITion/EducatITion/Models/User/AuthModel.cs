using System.ComponentModel.DataAnnotations;

namespace EducatITion.Models.User
{
    public class AuthModel
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
