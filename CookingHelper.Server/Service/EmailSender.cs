using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CookingHelper.Service;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // 使用 SMTP、SendGrid 或其他服務發送電子郵件
        // SendGrid
        // UseDefaultCredentials 功用 建立 page
        using (var client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(
                "cookinghelper01@gmail.com",
                "cwhr ivee lmdr rbkc"
            );
            using (
                var message = new MailMessage(
                    from: new MailAddress("cookinghelper01@gmail.com", "CookingHelper"),
                    to: new MailAddress(email, "User")
                )
            )
            {
                message.Subject = subject;
                message.Body = htmlMessage;

                client.Send(message);
            }
        }
        // todo SMTP
        // todo 完成 這個
        return Task.CompletedTask;
    }
}
