using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Dtos;

public class FileUploadDto
{
    [Required]
    public IFormFile File { get; set; }
    public ImageType Type { get; set; }
    public ResourceType ResourceType { get; set; }
}

public enum ImageType
{
    Review = 0,
    Destination = 1,
    Tour = 2
}

public enum ResourceType
{
    Image = 0,
    Video = 1
}