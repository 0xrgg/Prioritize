using Foundation;
using Prioritize.Core.Models;
using Prioritize.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserNotifications;

namespace Prioritize.Platforms.iOS
{
    public class NotificationManagerService : INotificationManagerService
    {
        int messageId = 0;
        bool hasPermission;

        public event EventHandler NotificationReceived;

        public NotificationManagerService()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, err) => hasPermission = approved);
        }

        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = message,
                Sound = UNNotificationSound.Default
            };

            UNNotificationTrigger trigger;
            if (notifyTime != null)
            {
                // Calendar trigger — fires at exact time even when app is closed
                var dateComponents = new NSDateComponents();
                dateComponents.Year = notifyTime.Value.Year;
                dateComponents.Month = notifyTime.Value.Month;
                dateComponents.Day = notifyTime.Value.Day;
                dateComponents.Hour = notifyTime.Value.Hour;
                dateComponents.Minute = notifyTime.Value.Minute;
                dateComponents.Second = notifyTime.Value.Second;
                trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponents, false);
            }
            else
            {
                trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
            }

            var request = UNNotificationRequest.FromIdentifier(
                (messageId++).ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, err =>
            {
                if (err != null) Console.WriteLine($"Notification error: {err}");
            });
        }

        public void ReceiveNotification(string title, string message)
        {
            NotificationReceived?.Invoke(null, new NotificationEventArgs
            {
                Title = title,
                Message = message
            });
        }

        public Task RequestPermissionsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
