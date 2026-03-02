using System.ComponentModel.DataAnnotations;

namespace EducatITion.Models.User
{
    public class RegModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string RepeatPassword { get; set; } = null!;
    }
}
