using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.ProviderGroup;
using Microsoft.AspNetCore.Mvc;

namespace CookingHelper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineBotController : ControllerBase
{
    private readonly LineBotService _lineBotService;
    private readonly RichMenuService _richMenuService;
    private readonly JsonProvider _jsonProvider;

    private readonly ILogger _logger;

    public LineBotController(
        LineBotService LineBotService,
        RichMenuService RichMenuService,
        ILogger<LineBotController> logger
    )
    {
        _lineBotService = LineBotService;
        _richMenuService = RichMenuService;
        _jsonProvider = new JsonProvider();
        _logger = logger;
    }

    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook(WebhookRequestBodyDto body)
    {
        _logger.LogInformation("asdas");
        await _lineBotService.ReceiveWebhook(body);
        return Ok();
    }

    [HttpPost("RichMenu/Validate")]
    public async Task<IActionResult> ValidateRichMenu(RichMenuModel richMenu)
    {
        return Ok(await _richMenuService.ValidateRichMenu(richMenu));
    }

    [HttpPost("RichMenu/Create")]
    public async Task<IActionResult> CreateRichMenu(RichMenuModel richMenu)
    {
        return Ok(await _richMenuService.CreateRichMenu(richMenu));
    }

    [HttpGet("RichMenu/GetList")]
    public async Task<IActionResult> GetRichMenuList()
    {
        return Ok(await _richMenuService.GetRichMenuList());
    }

    [HttpPost("RichMenu/UploadImage/{richMenuId}")]
    public async Task<IActionResult> UploadRichMenuImage(IFormFile imageFile, string richMenuId)
    {
        return Ok(await _richMenuService.UploadRichMenuImage(richMenuId, imageFile));
    }

    [HttpGet("RichMenu/SetDefault/{richMenuId}")]
    public async Task<IActionResult> SetDefaultRichMenu(string richMenuId)
    {
        return Ok(await _richMenuService.SetDefaultRichMenu(richMenuId));
    }

    [HttpDelete("RichMenu/Delete/{richMenuId}")]
    public async Task<IActionResult> DeleteRichMenu(string richMenuId)
    {
        return Ok(await _richMenuService.DeleteRichMenu(richMenuId));
    }
}
