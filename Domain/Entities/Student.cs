using Domain.exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; private set; }

        public Student(string name, int age, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("student.name","Name is required");

            if (email.Contains("@throw.com"))
                throw new DomainException("student.email","email is not acceptable");

            Name = name;
            Age = age;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
