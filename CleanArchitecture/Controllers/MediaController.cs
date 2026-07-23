using Application.DTOs;
using Infrastructure.Services.ApiHndler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ApiControllerBase
    {

        private readonly ApiHandler apiHandler;
        public MediaController(ApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        [Route("get-token")]
        public async Task<IActionResult> GetToken()
        {
            var body = new
            {
                ClientId = "admin-media",
                ClientSecret = "admin-media-secret-aa-portal-admin-one-data"
            };
            var response = await apiHandler.PostAsync<object, MediaResponse>("https://localhost:7180/api/auth/token", body);
            return Ok(response);
        }

    }
}
