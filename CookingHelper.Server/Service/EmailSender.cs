using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CookingHelper.Service;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(
                "cookinghelper01@gmail.com",
                _configuration["Email:EmailToken"]
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

        return Task.CompletedTask;
    }
}
