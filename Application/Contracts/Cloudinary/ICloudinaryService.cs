using Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Cloudinary;

public interface ICloudinaryService
{
    Task<List<string>> UploadMediaAsync(List<FileUploadDto> fileDtos);
    Task<string> UploadImageAsync(Stream image, string fileName);
    Task<bool> DeleteImageAsync(string imagePath);
}