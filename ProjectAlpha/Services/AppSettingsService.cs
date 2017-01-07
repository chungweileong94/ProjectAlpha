using Windows.Storage;

namespace ProjectAlpha.Services
{
    public abstract class AppSettingsService
    {
        public const string DEFAULT_LOCATION_MODE = "default_location_mode"; // 0 or 1
        public const string DEFAULT_LOCATION = "default_location"; // lat|lon
        public const string DEFAULT_LOCATION_FULL_NAME = "default_location_full_name"; // city, country
        public const string TEMPERATURE_UNIT = "temperature_unit"; // 

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
