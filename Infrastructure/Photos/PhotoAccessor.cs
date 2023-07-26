using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    //This is going to implement IPhotoAccessor interface in application
    public class PhotoAccessor : IPhotoAccessor
    {
        //We need access to the coudinary configuration. Cloudinary coming from ClouldinaryDotNet
        private readonly Cloudinary _cloudinary;

        //we need a new instance so that we can use the methods from cloudinary
        public PhotoAccessor(IOptions<CloudinarySettingsConfig> config)
        {
            //Acount coming from ClouldinaryDotNet
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            //Cloudinary coming from ClouldinaryDotNet
            _cloudinary = new Cloudinary(account);
        }

        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if (file.Length > 0)
            {
                //Using using because consume memory and we want to realease it as soon as finished with this method
                await using var stream = file.OpenReadStream();
                //ImageUploadParams coming from ClouldinaryDotNet
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };
            }

            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            //DeletionParams class coming from ClouldinaryDotNet
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}