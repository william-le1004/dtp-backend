using Application.Contracts.Cloudinary;
using Application.Dtos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using ResourceType = Application.Dtos.ResourceType;

namespace Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<List<string>> UploadMediaAsync(List<FileUploadDto> fileDtos)
    {
        var uploadedUrls = new List<string>();

        foreach (var file in fileDtos)
        {
            if (file.File == null || file.File.Length == 0)
                continue;

            await using var stream = file.File.OpenReadStream();
            await ProcessUpload(file, stream, uploadedUrls);
        }

        return uploadedUrls;
    }

    private async Task ProcessUpload(FileUploadDto file, Stream stream, List<string> uploadedUrls)
    {
        if (file.ResourceType == ResourceType.Image)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.File.FileName, stream),
                PublicId = Path.GetFileNameWithoutExtension(file.File.FileName),
                Folder = file.Type.ToString(),
            };
            var result = await UploadAsync(uploadParams, ResourceType.Image.ToString());
            uploadedUrls.Add(result);
        }
        else
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.File.FileName, stream),
                PublicId = Path.GetFileNameWithoutExtension(file.File.FileName),
                Folder = file.Type.ToString(),
            };
            var result = await UploadAsync(uploadParams, ResourceType.Video.ToString());
            uploadedUrls.Add(result);
        }
    }

    private async Task<string> UploadAsync(RawUploadParams uploadParams, string resourceType)
    {
        var uploadResult = await _cloudinary.UploadAsync(uploadParams, resourceType);
        return uploadResult.SecureUrl?.ToString();
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        var deleteParams = new DeletionParams(imagePath);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        return result.Result == "ok";
    }
}