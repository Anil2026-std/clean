using Application.DTOs;
using Application.UseCases.Students;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ApiControllerBase
    {
        private readonly StudentService _service;

        public StudentsController(StudentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDto dto)
        {
            var result = await _service.CreateStudent(dto);
            return ProcessResult(result);
        }
    }
}
