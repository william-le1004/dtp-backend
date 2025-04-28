using Application.Contracts.Cloudinary;
using Application.Dtos;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = ApplicationConst.AuthenticatedUser)]
public class MediaController(ICloudinaryService cloudinaryService) : BaseController
{
    [HttpPost]
     public async Task<IActionResult> UploadMedia([FromForm] List<IFormFile> files, [FromForm] List<ImageType> types, [FromForm] List<ResourceType> resourceType)
     {
         try
         {
             if (files == null || files.Count == 0)
                 return UnprocessableEntity(new { error = "No files received." });

             if (types.Count != files.Count)
                 return UnprocessableEntity(new { error = "Number of files and types must match." });

             var fileDtos = files.Select((file, index) => new FileUploadDto
             {
                 File = file,
                 Type = types.ElementAtOrDefault(index),
                 ResourceType = resourceType.ElementAtOrDefault(index)
             }).ToList();

             var uploadedUrls = await cloudinaryService.UploadMediaAsync(fileDtos);
             return Ok(new { urls = uploadedUrls });
         }
         catch (Exception ex)
         {
             return BadRequest(new { error = ex.Message });
         }
     }
}

