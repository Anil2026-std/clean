using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepo
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student);
    }
}
