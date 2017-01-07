using ProjectAlpha.Models;
using ProjectAlpha.Services;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace ProjectAlpha.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties
        public string AppName => Package.Current.DisplayName;
        public string Version => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";

        private int _DefaultLocationMode;
        public int DefaultLocationMode
        {
            get { return _DefaultLocationMode; }
            set
            {
                if (value != -1)
                {
                    Set(ref _DefaultLocationMode, value);
                    IsSaveAvailable = value == 0 ? true : string.IsNullOrEmpty(DefaultLocation) ? false : true;
                }
            }
        } // true: current, false: custom

        private string _DefaultLocation;
        public string DefaultLocation
        {
            get { return _DefaultLocation; }
            set
            {
                Set(ref _DefaultLocation, value);
                IsSaveAvailable = DefaultLocationMode == 0 ? true : string.IsNullOrEmpty(value) ? false : true;
            }
        }

        private string _DefaultLocationFullName;
        public string DefaultLocationFullName
        {
            get { return _DefaultLocationFullName; }
            set { Set(ref _DefaultLocationFullName, value); }
        }

        private bool _IsSaveAvailable;
        public bool IsSaveAvailable
        {
            get { return _IsSaveAvailable; }
            set { Set(ref _IsSaveAvailable, value); }
        }

        private string _QueryString;
        public string QueryString
        {
            get { return _QueryString; }
            set { Set(ref _QueryString, value); }
        }

        private List<OpenWeatherObj> _QueryWeather;
        public List<OpenWeatherObj> QueryWeather
        {
            get { return _QueryWeather; }
            set { Set(ref _QueryWeather, value); }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { Set(ref _IsLoading, value); }
        }
        #endregion

        public SettingsViewModel()
        {
            IsLoading = false;
            DefaultLocationMode = (int)AppSettingsService.GetSetting(AppSettingsService.DEFAULT_LOCATION_MODE);

            var temp_dl = AppSettingsService.GetSetting(AppSettingsService.DEFAULT_LOCATION);
            DefaultLocation = temp_dl == null ? null : temp_dl.ToString();

            var temp_dlfn = AppSettingsService.GetSetting(AppSettingsService.DEFAULT_LOCATION_FULL_NAME);
            DefaultLocationFullName = temp_dlfn == null ? "Specific location" : temp_dlfn.ToString();
        }

        public async void SaveSettings()
        {
            AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_MODE, DefaultLocationMode);
            if (DefaultLocationMode == 1)
            {
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION, DefaultLocation);
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_FULL_NAME, DefaultLocationFullName);
            }
            await new MessageDialog("Settings Saved.").ShowAsync();
        }

        public async void SearchLocation()
        {
            try
            {
                IsLoading = true;
                var obj = await OpenWeatherService.GetWeatherByCityNameAsync(QueryString, "");
                QueryWeather = new List<OpenWeatherObj>() { obj };
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }

        public void SelectLocation()
        {
            QueryString = string.Empty;
            DefaultLocation = $"{QueryWeather[0].city.coord.lat}|{QueryWeather[0].city.coord.lon}";
            DefaultLocationFullName = QueryWeather[0].city.location;
            QueryWeather = null;
        }
    }
}
