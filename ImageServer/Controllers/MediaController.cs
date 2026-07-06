using ImageServer.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly MediaStorageService _mediaService;
        private readonly IWebHostEnvironment _env;

        public MediaController(MediaStorageService mediaService, IWebHostEnvironment env)
        {
            _mediaService = mediaService;
            _env = env;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedia(IFormFile file, [FromQuery] string? folderName = null)
        {

            var clientId = User.FindFirst("clientId")?.Value;
            folderName = folderName?.ToLower();

            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("clientId missing in token");

            if (file == null || file.Length == 0)
            {
                return BadRequest("File not selected");
            }

            var fileName = await _mediaService.SaveFileAsync(file, clientId, folderName);

            var mediaUrlWithoutScheme = $"/media/{clientId}/{folderName ?? string.Empty}/{fileName}".Replace("//", "/");

            Console.WriteLine(mediaUrlWithoutScheme);
            return Ok(new
            {
                mediaUrl = $"{mediaUrlWithoutScheme}"
            });
        }

        [HttpGet("media/{clientId}/{fileName}")]
        [HttpGet("media/{clientId}/{folderName}/{fileName}")]
        public IActionResult GetMedia(string clientId, string fileName, string? folderName = null)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var path = string.IsNullOrEmpty(folderName)
                ? Path.Combine(webRoot, "uploads", clientId, fileName)
                : Path.Combine(webRoot, "uploads", clientId, folderName, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = "application/octet-stream";
            return PhysicalFile(path, contentType);
        }
    }
}
