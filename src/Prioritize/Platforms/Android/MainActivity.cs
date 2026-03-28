using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Prioritize.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                               ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                               ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Request notification permission on Android 13+
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            //{
            //    RequestPermissions(new[] { Manifest.Permission.PostNotifications }, 0);
            //}

            //// Request exact alarm permission on Android 12+
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            //{
            //    var alarmManager = GetSystemService(Context.AlarmService) as AlarmManager;
            //    if (alarmManager != null && !alarmManager.CanScheduleExactAlarms())
            //    {
            //        var intent = new Intent(
            //            global::Android.Provider.Settings.ActionRequestScheduleExactAlarm);
            //        StartActivity(intent);
            //    }
            //}

            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            CreateNotificationFromIntent(intent);
        }

        static void CreateNotificationFromIntent(Intent? intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(NotificationManagerService.TitleKey);
                string message = intent.GetStringExtra(NotificationManagerService.MessageKey);
                NotificationManagerService.Instance?.ReceiveNotification(title, message);
            }
        }
    }
}