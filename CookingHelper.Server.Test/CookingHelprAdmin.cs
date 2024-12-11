using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync("https://localhost:5173/");
    }

    [Test]
    public async Task HasTitle()
    {
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Sign in" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task AdminTest()
    {
        await Page.GotoAsync("https://cookinghelper.azurewebsites.net/");
        await Page.GetByPlaceholder("your@email.com").ClickAsync();
        await Page.GetByPlaceholder("your@email.com").FillAsync("fangfelipe@gmail.com");
        await Page.GetByPlaceholder("••••••").ClickAsync();
        await Page.GetByPlaceholder("••••••").FillAsync("123456");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Img).GetByText("0", new() { Exact = true }))
            .ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Tab, new() { Name = "帳號管理" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "註冊帳號" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "登出" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Sign in" }))
            .ToBeVisibleAsync();
    }
}
