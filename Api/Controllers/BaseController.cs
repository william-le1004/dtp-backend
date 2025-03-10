using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[ApiController]
public abstract class BaseController : ODataController
{
    protected IActionResult HandleServiceResult<T>(ApiResponse<T> result)
    {
        if (result.Success)
        {
            return StatusCode(result.StatusCode, new
            {
                success = true,
                message = result.Message ?? "Request successful",
                data = result.Data
            });
        }

        return StatusCode(result.StatusCode, new
        {
            success = false,
            message = result.Message ?? "An error occurred",
            data = (T?)default,
            error = result.Errors
        });
    }
}