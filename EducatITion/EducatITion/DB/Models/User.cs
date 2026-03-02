using System.ComponentModel.DataAnnotations;

namespace EducatITion.DB.Models
{
    public class User
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
        public bool WasInMenu { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
