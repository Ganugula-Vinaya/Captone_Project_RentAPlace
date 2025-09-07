using Microsoft.AspNetCore.Hosting;

namespace RentAPlace.Api.Services
{
    public interface IImageService
    {
        Task<string> SavePropertyImageAsync(IFormFile file);
    }

    // Saves uploaded images into wwwroot/uploads/properties and returns relative URL
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        public ImageService(IWebHostEnvironment env) { _env = env; }

        public async Task<string> SavePropertyImageAsync(IFormFile file)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "properties");
            Directory.CreateDirectory(uploadsRoot);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsRoot, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            var relative = $"/uploads/properties/{fileName}";
            return relative;
        }
    }
}
