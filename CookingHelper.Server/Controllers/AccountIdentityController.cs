using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CookingHelper.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CookingHelper.Controllers;

/*
    ! 怎麼上到 azure
    ! check azure cost
    ! 正式機 也要 dotnet ef migrations add someThing
    todo rememberMe 正式機

    todo http cookie
*/
//! Aspnet Core Identity ? search?

//!  Azure Table Storage.

//! Microsoft identity platform
//!  重新產生 AppLog 的資料
//! line 帳號重新登入
/*
    todo
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
*/
[ApiController]
[Route("api/[controller]")]
public class AccountIdentityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountIdentityController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        if (User.Identity?.IsAuthenticated == true) // 判斷有沒有登入
        {
            // 獲取當前用戶的 UserId
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                // 通過 UserManager 獲取完整的 ApplicationUser 資料
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    return Ok(
                        new
                        {
                            IsAuthenticated = true,
                            Username = User.Identity.Name,
                            Permission = user.Permission
                        }
                    );
                }
            }
        }

        return Ok(new { IsAuthenticated = false, Message = "User is not logged in." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Permission = model.Permission
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return Conflict(new { message = "This email address is already registered." }); // 做其他

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid login attempt." });
        }
        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            false
        );

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid login attempt." });

        return Ok("User logged in successfully.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("User logged out successfully.");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Ok(new { Message = "If the email is valid, a reset link has been sent." });

        // 更新安全戳記
        await _userManager.UpdateSecurityStampAsync(user);
        // 生成密碼重置令牌
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // 建立重置連結
        var resetLink =
            $"https://localhost:5173/reset-password?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";

        // 發送重置連結至電子郵件
        await _emailSender.SendEmailAsync(
            user.Email,
            "Reset Password",
            $"Click here to reset your password: {resetLink} This link will expire in 3 minutes"
        );

        return Ok(new { Message = "If the email is valid, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest(new { Message = "Invalid request." });

        // 驗證令牌並重置密碼
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Password has been reset successfully." });
    }
}

public class RegisterModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Permission { get; set; }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class ForgotPasswordModel
{
    [EmailAddress]
    public string Email { get; set; }
}

public class ResetPasswordModel
{
    [EmailAddress]
    public string Email { get; set; }

    public string Token { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string NewPassword { get; set; }
}
