using System.ComponentModel.DataAnnotations;

namespace Frontend.Model
{
    public class UserModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Range(1, 120)]
        public int? Age { get; set; }
    }
}
