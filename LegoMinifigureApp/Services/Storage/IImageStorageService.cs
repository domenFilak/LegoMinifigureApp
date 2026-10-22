using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Storage
{
    public interface IImageStorageService
    {
        Task<Error?> ValidateAsync(ImageUpload upload, CancellationToken cancellationToken = default);
        Task<string> SaveAsync(ImageUpload upload, string folder, CancellationToken cancellationToken = default);
        void Delete(string? relativePath);
    }
}
