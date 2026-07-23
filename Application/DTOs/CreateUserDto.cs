using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class CreateUserDto
    {
        [Required,MinLength(3,ErrorMessage ="Minimum length is 3")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string? ImageUrl { get; set; }
    }
}
