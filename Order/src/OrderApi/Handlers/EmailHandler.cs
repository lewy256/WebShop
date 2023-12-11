using MediatR;
using OrderApi.Notifications;
using System.Net;
using System.Net.Mail;

namespace OrderApi.Handlers;

public sealed class EmailHandler : INotificationHandler<OrderCreatedNotification> {

    public async Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken) {
        var smtpClient = new SmtpClient() {
            Port = 587,
            Credentials = new NetworkCredential("", ""),
            EnableSsl = true,
            Host = "smtp.gmail.com"
        };
        //smtpClient.Send("", email, "Order", message);

        await Task.CompletedTask;
    }
}
