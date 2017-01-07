using ProjectAlpha.Models;
using ProjectAlpha.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace ProjectAlpha.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        #region Properties
        private OpenWeatherObj _CurrentWeather;
        public OpenWeatherObj CurrentWeather
        {
            get { return _CurrentWeather; }
            set { Set(ref _CurrentWeather, value); }
        }

        private List<OpenWeatherObj> _QueryWeather;
        public List<OpenWeatherObj> QueryWeather
        {
            get { return _QueryWeather; }
            set { Set(ref _QueryWeather, value); }
        }

        private string _QueryString;
        public string QueryString
        {
            get { return _QueryString; }
            set { Set(ref _QueryString, value); }
        }

        private string _Coord;
        public string Coord
        {
            get { return _Coord; }
            set { Set(ref _Coord, value); }
        }

        private int _Status;
        public int Status
        {
            get { return _Status; }
            set { Set(ref _Status, value); }
        } //0: none, 1: no internet, 2: not found, 3: no internet(search), 10: idle

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { Set(ref _IsLoading, value); }
        }
        #endregion

        public SearchViewModel()
        {
            Status = 10;
            IsLoading = false;
        }

        public async void SearchLocation()
        {
            if (IsInternetAvailable())
            {
                try
                {
                    IsLoading = true;
                    if (!string.IsNullOrEmpty(QueryString))
                    {
                        QueryWeather = new List<OpenWeatherObj>() { await OpenWeatherService.GetWeatherByCityNameAsync(QueryString) };
                    }
                }
                catch (COMException) { Status = 2; }
                catch (Exception) { Status = 3; }
                finally
                {
                    IsLoading = false;
                }
            }
            else
            {
                Status = 3;
            }
        }

        public async void SelectLocation()
        {
            QueryString = string.Empty;
            Coord = $"{QueryWeather[0].city.coord.lat}|{QueryWeather[0].city.coord.lon}";
            QueryWeather = null;
            await LoadCurrentWeatherAsync();
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

            string[] temp = Coord.Split('|');
            lat = double.Parse(temp[0]);
            lon = double.Parse(temp[1]);

            //http call
            if (Status == 0)
            {
                if (IsInternetAvailable())
                {
                    try
                    {
                        CurrentWeather = await OpenWeatherService.GetWeatherByCoordinateAsync(lat, lon);
                    }
                    catch
                    {
                        Status = 1;
                    }
                }
                else
                {
                    Status = 1;
                }
            }

            IsLoading = false;
        }

        public async Task<bool> SetAsDefaultLocation()
        {
            bool result = false;

            MessageDialog msg = new MessageDialog($"Are you sure to set \"{CurrentWeather.city.location}\" as your default location?", "Set as default location");
            msg.Commands.Add(new UICommand("Yes", delegate
            {
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_MODE, 1);
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION, $"{CurrentWeather.city.coord.lat}|{CurrentWeather.city.coord.lon}");
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_FULL_NAME, $"{CurrentWeather.city.location}");

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

                result = true;
            }));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();

            return result;
        }

        private bool IsInternetAvailable()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            return connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}
