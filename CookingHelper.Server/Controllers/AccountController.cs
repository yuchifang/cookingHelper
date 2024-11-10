using CookingHelper.Data;
using CookingHelper.Model;
using Microsoft.AspNetCore.Mvc;

namespace CookingHelper.Controllers;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserListDbContext _userListDbContext;

    public AccountController(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Account>> Register(Account request)
    {
        var accountExist = await _userListDbContext.Account.FirstOrDefaultAsync(account =>
            account.Email == request.Email
        );
        if (accountExist != null)
        {
            return BadRequest(new { message = "This email address is already registered." });
        }

        string passwordHash = BCrypt.HashPassword(request.Password, workFactor: 11);
        var account = new Account
        {
            Email = request.Email,
            Password = passwordHash,
            Name = request.Name,
            Permission = request.Permission
        };
        await _userListDbContext.AddAsync(account);
        await _userListDbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Register), new { id = account.AccountId }, account);
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDto LoginRequestDto)
    {
        var loginAccount = await _userListDbContext.Account.SingleOrDefaultAsync(account =>
            account.Email == LoginRequestDto.Email
        );

        if (loginAccount == null || !BCrypt.Verify(LoginRequestDto.Password, loginAccount.Password))
        {
            return NotFound(new { message = "Wrong Username Or Password" });
        }
        return Ok(loginAccount);
    }
}
