using ImageServer.dto;
using Microsoft.Extensions.Options;

namespace ImageServer.service
{
    public class MediaStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MediaSettings _mediaSettings;

        public MediaStorageService(IWebHostEnvironment env,
            IHttpContextAccessor accessor,
            IOptions<MediaSettings> mediaSettings)
        {
            _env = env;
            _httpContextAccessor = accessor;
            _mediaSettings = mediaSettings.Value;

        }

        public async Task<string> SaveFileAsync(IFormFile file, string clientId, string? folderName = null)
        {
            var maxFileSizeBytes = _mediaSettings.MaxFileSizeMB * 1024 * 1024;
            if (file.Length > maxFileSizeBytes)
                throw new InvalidOperationException($"File size exceeds {_mediaSettings.MaxFileSizeMB} MB limit.");

            // 2. Validate file extension from config (case-insensitive)
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !_mediaSettings.AllowedFileExtensions.Contains(ext))
                throw new InvalidOperationException("Invalid file type.");

            // 3. Build folder path
            var baseFolder = Path.Combine(_env.WebRootPath, "uploads", clientId);
            var targetFolder = string.IsNullOrEmpty(folderName) ? baseFolder : Path.Combine(baseFolder, folderName);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // 4. Save file
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(targetFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
        public string GetPublicUrl(string clientId, string fileName)
        {
            var req = _httpContextAccessor?.HttpContext?.Request;
            return $"{req?.Scheme}://{req?.Host}/uploads/{clientId}/{fileName}";
        }
    }
}
