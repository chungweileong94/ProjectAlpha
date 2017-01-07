using ProjectAlpha.Models;
using ProjectAlpha.Services;
using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;

namespace ProjectAlpha.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        #region Properties
        private OpenWeatherObj _CurrentWeather;
        public OpenWeatherObj CurrentWeather
        {
            get { return _CurrentWeather; }
            set { Set(ref _CurrentWeather, value); }
        }

        private int _Status;
        public int Status
        {
            get { return _Status; }
            set { Set(ref _Status, value); }
        } //0: none, 1: no permission, 2: no internet

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { Set(ref _IsLoading, value); }
        }
        #endregion

        public WeatherViewModel()
        {
            Status = 0;
            IsLoading = false;

            if (!AppSettingsService.ContainsSetting(AppSettingsService.TEMPERATURE_UNIT))
            {
                AppSettingsService.SaveSetting(AppSettingsService.TEMPERATURE_UNIT, true);
            }
        }

        public async void LoadCurrentWeather()
        {
            await LoadCurrentWeatherAsync();
        }

        public async Task LoadCurrentWeatherAsync()
        {
            IsLoading = true;
            Status = 0;
            CurrentWeather = null;

            //get coordinate
            double lat = 0, lon = 0;

            if ((int)AppSettingsService.GetSetting(AppSettingsService.DEFAULT_LOCATION_MODE) == 0)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        Geolocator geolocator = new Geolocator();
                        Geoposition pos = await geolocator.GetGeopositionAsync();
                        lat = pos.Coordinate.Point.Position.Latitude;
                        lon = pos.Coordinate.Point.Position.Longitude;
                        break;
                    case GeolocationAccessStatus.Unspecified:
                    case GeolocationAccessStatus.Denied:
                        Status = 1;
                        break;
                }
            }
            else
            {
                string[] temp = AppSettingsService.GetSetting(AppSettingsService.DEFAULT_LOCATION).ToString().Split('|');
                lat = double.Parse(temp[0]);
                lon = double.Parse(temp[1]);
            }

            //http call
            if (Status == 0)
            {
                if (IsInternetAvailable())
                {
                    try
                    {
                        CurrentWeather = await OpenWeatherService.GetWeatherByCoordinateAsync(lat, lon);

                        #region Update live tile
                        var liveTile = TileUpdateManager.CreateTileUpdaterForApplication();

                        string temperature = CurrentWeather.temperature.TemperatureUnit ? $"{CurrentWeather.temperature.temp_c}C" : $"{CurrentWeather.temperature.temp_f}F";
                        string time = DateTime.Now.ToString("ddd h:mm tt");

                        //tile template
                        string tileXMLString = $@"<tile>
                                                    <visual baseUri=""Assets/Weather/"">
                                                  
                                                      <binding template=""TileSmall"" hint-textStacking=""center"">
                                                        <text hint-style=""body"" hint-align=""center"">{temperature}</text>
                                                      </binding>
                                                  
                                                      <binding displayName=""{CurrentWeather.city.name}"" template=""TileMedium"" hint-textStacking=""center"">
                                                        <text hint-style=""body"" hint-align=""center"">{CurrentWeather.weather.value}</text>
                                                        <text hint-style=""titleSubtle"" hint-align=""center"">{temperature}</text>
                                                        <text hint-style=""captionSubtle"" hint-align=""center"">{time}</text>
                                                      </binding>
                                                  
                                                      <binding displayName=""{CurrentWeather.city.name}"" template=""TileWide"" branding=""nameAndLogo"" hint-textStacking=""center"">
                                                        <group>
                                                          <subgroup hint-weight=""44"">
                                                            <image src=""{CurrentWeather.weather.iconUrl}"" hint-removeMargin=""false""/>
                                                          </subgroup>
                                                          <subgroup>
                                                            <text hint-style=""body"">{CurrentWeather.weather.value}</text>
                                                            <text hint-style=""titleSubtle"">{temperature}</text>
                                                            <text hint-style=""captionSubtle"">{time}</text>
                                                          </subgroup>
                                                        </group>
                                                      </binding>
                                                  
                                                      <binding displayName=""{CurrentWeather.city.name}"" template=""TileLarge"" branding=""nameAndLogo"" hint-textStacking=""center"">
                                                        <group>
                                                          <subgroup hint-weight=""1""/>
                                                          <subgroup hint-weight=""2"">
                                                            <image src=""{CurrentWeather.weather.iconUrl}""/>
                                                          </subgroup>
                                                          <subgroup hint-weight=""1""/>
                                                        </group>
                                                        <text hint-style=""body"" hint-align=""center"">{CurrentWeather.weather.value}</text>
                                                        <text hint-style=""titleSubtle"" hint-align=""center"">{temperature}</text>
                                                        <text hint-style=""captionSubtle"" hint-align=""center"">{time}</text>
                                                      </binding>
                                                  
                                                    </visual>
                                                  </tile>";

                        XmlDocument tileXML = new XmlDocument();
                        tileXML.LoadXml(tileXMLString);
                        liveTile.Update(new TileNotification(tileXML));
                        #endregion
                    }
                    catch
                    {
                        Status = 2;
                    }
                }
                else
                {
                    Status = 2;
                }
            }

            IsLoading = false;
        }

        private bool IsInternetAvailable()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            return connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}
