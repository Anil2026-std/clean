using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class CreateStudentDto
    {
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
