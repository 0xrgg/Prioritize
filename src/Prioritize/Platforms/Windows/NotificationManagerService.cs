using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Prioritize.Core.Services;

namespace Prioritize.Platforms.Windows
{

    public class NotificationManagerService : INotificationManagerService
    {
        public event EventHandler NotificationReceived;

        public void ReceiveNotification(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task RequestPermissionsAsync()
        {
            return Task.CompletedTask;
        }

        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (notifyTime != null && notifyTime.Value > DateTime.Now)
            {
                // Schedule toast
                var scheduledTime = notifyTime.Value;
                var toastXml = CreateToastXml(title, message);
                var scheduledToast = new ScheduledToastNotification(toastXml, scheduledTime);
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
            }
            else
            {
                // Show immediately
                var toastXml = CreateToastXml(title, message);
                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }

        private XmlDocument CreateToastXml(string title, string message)
        {
            var toastXmlString = $@"
                <toast>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>{title}</text>
                            <text>{message}</text>
                        </binding>
                    </visual>
                </toast>";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastXmlString);
            return xmlDoc;
        }
    }
#endif
}
