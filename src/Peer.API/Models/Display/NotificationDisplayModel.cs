using Peer.API.Utils;
using Peer.Domain.Notifications;

namespace Peer.API.Models.Display;

public class NotificationDisplayModel
{
    public string Type { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();

    public NotificationDisplayModel(AbstractNotification notification)
    {
        Type = notification.GetType().Name;
        Message = notification.Message;

        notification.GetType()
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
            .ToList()
            .ForEach(prop => Parameters.Add(prop.Name.ToCamelCase(), prop.GetValue(notification)));
    }
}
