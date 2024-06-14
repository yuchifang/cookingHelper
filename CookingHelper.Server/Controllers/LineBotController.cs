using Microsoft.AspNetCore.Mvc;
using CookingHelper.LineDto;

using System.ComponentModel.DataAnnotations;
using CookingHelper.LineDtoService;
using CookingHelper.ProviderGroup;


namespace CookingHelper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LineBotController : ControllerBase
{

    // 宣告 service
    private readonly LineDtoService.LineBotService _lineBotService;
    // ------ 宣告 services ------
    private readonly RichMenuService _richMenuService;

    private readonly JsonProvider _jsonProvider;
    // constructor

    public LineBotController(LineDtoService.LineBotService LineBotService)
    {
        _lineBotService = LineBotService;
        _richMenuService = new RichMenuService();
        _jsonProvider = new JsonProvider();
    }

    // 使用 Post 方法的原因是因為這支 API 會接收 Line 傳送的 webhook event，
    // 這部分在下一篇會介紹～
    // [HttpPost("Webhook")]
    // public IActionResult Webhook(WebhookRequestBodyDto body)
    // {
    //     _lineBotService.ReceiveWebhook(body); // 呼叫 Service
    //     return Ok();
    // }


    // // 對全部的使用者傳送訊息
    // [HttpPost("SendMessage/Broadcast")]
    // public IActionResult Broadcast([Required] string messageType, object body)
    // {
    //     _lineBotService.BroadcastMessageHandler(messageType, body);
    //     return Ok();
    // }
    // ------ 新增 api ------
    [HttpPost("Webhook")]
    public IActionResult Webhook(WebhookRequestBodyDto body)
    {
        _lineBotService.ReceiveWebhook(body); // 呼叫 Service
        return Ok();
    }

    //rich menu api
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