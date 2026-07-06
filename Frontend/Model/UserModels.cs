using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Model
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Minimum length is 3")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
