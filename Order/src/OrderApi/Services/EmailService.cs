using System.Net;
using System.Net.Mail;

namespace OrderApi.Services;

public class EmailService
{

    public void SendEmail(string email, string message)
    {
        var smtpClient = new SmtpClient()
        {
            Port = 587,
            Credentials = new NetworkCredential("", ""),
            EnableSsl = true,
            Host = "smtp.gmail.com"
        };
        smtpClient.Send("", email, "Order", message);

    }


}
