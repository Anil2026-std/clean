using Application.DTOs;
using Application.Interfaces.IRepo;
using Application.Interfaces.IServices;
using Domain.comman;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Students
{
    public class StudentService
    {
        private readonly IStudentRepository _repository;
        private IEmailService _emailService;
        public StudentService(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<Result<int>> CreateStudent(CreateStudentDto createStudentDto)
        {

            return Result<int>.Failure(Error.Conflict("student.Create", "Valid", new Dictionary<string, object>
            {
                { "Age", "Age must be more than 10" }
            }));
            var student = new Student(
                name: createStudentDto.Name,
                age: createStudentDto.Age,
                email: createStudentDto.Email
            );
            await _repository.AddAsync(student);

                    await _emailService.SendAsync(
                    student.Email,
                    "Welcome to LMS",
                    $"Hello {student.Name}, your account is created."
                );

            return Result<int>.Failure(Error.Validation("student.age","Age is required"));

          // return Result<int>.Success(student.Id);
        }
    }
}
