using Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Cloudinary;

public interface ICloudinaryService
{
    Task<List<string>> UploadMediaAsync(List<FileUploadDto> fileDtos);
    Task<bool> DeleteImageAsync(string imagePath);
}