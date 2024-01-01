using Mediator;
using System.Net;
using System.Net.Mail;

namespace OrderApi.Notifications;

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

    ValueTask INotificationHandler<OrderCreatedNotification>.Handle(OrderCreatedNotification notification, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
