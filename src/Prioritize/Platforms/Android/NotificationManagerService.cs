using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Prioritize.Core.Models;
using Prioritize.Core.Services;

namespace Prioritize.Platforms.Android
{
    public class NotificationManagerService : INotificationManagerService
    {
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        public const string TitleKey = "title";
        public const string MessageKey = "message";

        int messageId = 0;
        int pendingIntentId = 0;
        bool channelInitialized = false;

        NotificationManagerCompat compatManager;

        public event EventHandler NotificationReceived;
        public static NotificationManagerService Instance { get; private set; }

        public NotificationManagerService()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                compatManager = NotificationManagerCompat.From(Platform.AppContext);
                Instance = this;
            }
        }

        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            if (notifyTime != null)
            {
                Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
                intent.PutExtra(TitleKey, title);
                intent.PutExtra(MessageKey, message);
                intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

                var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                    ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                    : PendingIntentFlags.CancelCurrent;

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
                long triggerTime = GetNotifyTime(notifyTime.Value);
                AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
                alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
            else
            {
                Show(title, message);
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            NotificationReceived?.Invoke(null, new NotificationEventArgs
            {
                Title = title,
                Message = message
            });
        }

        public void Show(string title, string message)
        {
            Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent;

            PendingIntent pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Platform.AppContext.Resources, Resource.Drawable.ic_notification))
                .SetSmallIcon(Resource.Drawable.ic_notification);

            Notification notification = builder.Build();
            compatManager.Notify(messageId++, notification);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    channelId,
                    new Java.Lang.String(channelName),
                    NotificationImportance.High) // High = shows as heads-up
                {
                    Description = channelDescription
                };
                var manager = (NotificationManager)Platform.AppContext
                    .GetSystemService(Context.NotificationService);
                manager.CreateNotificationChannel(channel);
            }
            channelInitialized = true;
        }

        long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            return utcTime.AddSeconds(-epochDiff).Ticks / 10000;
        }

        public async Task RequestPermissionsAsync()
        {

            // Notification permission (Android 13+ = API 33)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                System.Diagnostics.Debug.WriteLine("=== Requesting POST_NOTIFICATIONS ===");
                var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                System.Diagnostics.Debug.WriteLine($"=== Permission result: {status} ===");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("=== Android < 13, no notification permission needed ===");
            }

            // Exact alarm (Android 12+ = API 31)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                var alarmManager = Platform.AppContext
                    .GetSystemService(Context.AlarmService) as AlarmManager;

                if (alarmManager != null)
                {
                    bool canSchedule = alarmManager.CanScheduleExactAlarms();

                    if (!canSchedule)
                    {
                        var intent = new Intent(
                            global::Android.Provider.Settings.ActionRequestScheduleExactAlarm);
                        intent.SetFlags(ActivityFlags.NewTask);
                        Platform.AppContext.StartActivity(intent);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("=== Android < 12, no exact alarm permission needed ===");
            }
        }
    }
}