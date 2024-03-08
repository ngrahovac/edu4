using Peer.API.Utils;
using Peer.Domain.Notifications;

namespace Peer.API.Models.Display;

public class NotificationDisplayModel
{
    public string Type { get; set; }
    public string Message { get; set; }
    public string When { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();

    public NotificationDisplayModel(AbstractNotification notification)
    {
        Type = notification.GetType().Name;
        Message = notification.Message;

        notification.GetType()
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
            .ToList()
            .ForEach(prop => Parameters.Add(prop.Name.ToCamelCase(), prop.GetValue(notification)));


        var today = DateOnly.FromDateTime(notification.Timestamp).Equals(DateOnly.FromDateTime(DateTime.Today));

        if (today)
        {
            When = TimeOnly.FromDateTime(notification.Timestamp).ToShortTimeString();
            return;
        }

        var yesterday = DateOnly.FromDateTime(notification.Timestamp).Equals(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

        if (yesterday)
        {
            When = "Yesterday";
            return;
        }

        When = DateOnly.FromDateTime(notification.Timestamp).ToShortDateString();
    }
}
