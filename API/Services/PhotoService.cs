using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace API.Services
{
    public class PhotoService : IphotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                    config.Value.CloudName,
                    config.Value.APIKey,
                    config.Value.APISecret
                );
            _cloudinary = new Cloudinary( acc );
        }
        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult=new ImageUploadResult();
            if (file.Length > 0) { 
                using var stream=file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File=new FileDescription(file.FileName,stream),
                    Transformation=new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder="da-net7"
                };
                uploadResult=await _cloudinary.UploadAsync(uploadParams);   
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string PublicId)
        {
            var deleteParams = new DeletionParams(PublicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
