using Windows.Storage;

namespace Tasks.Services
{
    public sealed class AppSettingsService
    {
        //public static string DEFAULT_LOCATION_MODE = "default_location_mode"; // 0 or 1
        //public static string DEFAULT_LOCATION = "default_location"; // lat|lon
        //public static string DEFAULT_LOCATION_FULL_NAME = "default_location_full_name"; // city, country
        //public static string TEMPERATURE_UNIT = "temperature_unit"; // 

        public static object GetSetting(string key) => ApplicationData.Current.LocalSettings.Values[key];

        public static void SaveSetting(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static void RemoveSetting(string key)
        {
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }

        public static bool ContainsSetting(string key) => ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
    }
}
