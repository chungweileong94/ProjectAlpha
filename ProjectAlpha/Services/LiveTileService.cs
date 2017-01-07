using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Popups;

namespace ProjectAlpha.Services
{
    public abstract class LiveTileService
    {
        private const string TASK_NAME = "MainLiveTileBackgroundTask";

        public static bool IsTaskExisting()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == TASK_NAME)
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task RegisterTaskAsync()
        {
            bool isSystemAllow = false;

            if (ApiInformation.IsEnumNamedValuePresent(typeof(BackgroundAccessStatus).ToString(), "AllowedSubjectToSystemPolicy"))
            {
                switch (await BackgroundExecutionManager.RequestAccessAsync())
                {
                    case BackgroundAccessStatus.AlwaysAllowed:
                    case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                        isSystemAllow = true;
                        break;
                    case BackgroundAccessStatus.Unspecified:
                    case BackgroundAccessStatus.DeniedBySystemPolicy:
                    case BackgroundAccessStatus.DeniedByUser:
                        isSystemAllow = false;
                        break;
                }
            }
            else
            {
                switch (await BackgroundExecutionManager.RequestAccessAsync())
                {
                    case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                        isSystemAllow = true;
                        break;
                    case BackgroundAccessStatus.Unspecified:
                    case BackgroundAccessStatus.Denied:
                        isSystemAllow = false;
                        break;
                }
            }

            if (isSystemAllow)
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = TASK_NAME;
                builder.TaskEntryPoint = typeof(Tasks.LiveTileBackgroundTask).FullName;
                builder.SetTrigger(new TimeTrigger(15, false));
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                builder.Register();
            }
            else
            {
                MessageDialog message = new MessageDialog("You didn't allow this app to run in background.", "Permission Denied");
                message.Commands.Add(new UICommand("Go to Settings", async (command) =>
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps"));
                }));
                message.Commands.Add(new UICommand("Cancel"));
                await message.ShowAsync();
            }
        }

        public static void UnregisterTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == TASK_NAME)
                {
                    task.Value.Unregister(true);
                    return;
                }
            }
        }
    }
}
