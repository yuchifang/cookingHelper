using Microsoft.AspNetCore.Mvc;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.ProviderGroup;


namespace CookingHelper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineBotController : ControllerBase
{
    private readonly LineBotService _lineBotService;
    private readonly RichMenuService _richMenuService;
    private readonly JsonProvider _jsonProvider;

    public LineBotController(LineBotService LineBotService, RichMenuService RichMenuService)
    {
        _lineBotService = LineBotService;
        _richMenuService = RichMenuService;
        _jsonProvider = new JsonProvider();
    }

    // 使用者傳訊息會由此API接收
    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook(WebhookRequestBodyDto body)
    {
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