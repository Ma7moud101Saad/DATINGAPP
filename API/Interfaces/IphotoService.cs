using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IphotoService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile formFile);
        Task<DeletionResult> DeletePhotoAsync(string PublicId);
    }
}
