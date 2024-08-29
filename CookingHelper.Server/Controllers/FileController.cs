using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CookingHelper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FileController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("UploadFile/RecipeImage/{fileName}")]
    public IActionResult GetRecipeImage(string fileName)
    {
        var path =
            $"{_configuration.GetValue<string>(WebHostDefaults.ContentRootKey)}/UploadFile/RecipeImage/{fileName}";
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            var fileBytes = System.IO.File.ReadAllBytes(path);
            new FileExtensionContentTypeProvider().TryGetContentType(
                Path.GetFileName(path),
                out var contentType
            );
            return new FileContentResult(fileBytes, contentType ?? "application/octet-stream");
        }
    }
}
