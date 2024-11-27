using System.Security.Claims;
using CookingHelper.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CookingHelper.Controllers;

/*
    ! 怎麼上到 azure
    ! check azure cost
    ! 正式機 也要 dotnet ef migrations add someThing
    todo Unauthorized ??
*/
//!  重新產生 AppLog 的資料
//! line 帳號重新登入
/*
    todo 紀錄 page
    DBContext 要加一行
    base.OnModelCreating(modelBuilder);
    todo
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
   
    todo 忘記密碼

*/
[ApiController]
[Route("api/[controller]")]
public class AccountIdentityController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountIdentityController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        // todo logout 功用是甚麼


        // ClaimTypes.NameIdentifier 是甚麼 代表什麼意思?

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
                            Permission = user.Permission // 獲取自定義屬性
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
