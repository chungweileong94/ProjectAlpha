using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Tasks.Models;
using Tasks.Services;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;
using Windows.Web.Http;

namespace Tasks
{
    public sealed class LiveTileBackgroundTask : IBackgroundTask
    {
        private string APIKEY = "cdee049e58c5baca6ba156795889cb78";
        private CancellationTokenSource cts;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var _deferral = taskInstance.GetDeferral();

            try
            {
                taskInstance.Canceled += TaskInstance_Canceled;

                if (cts == null)
                {
                    cts = new CancellationTokenSource();
                }

                bool canRun = true;
                double lat = 0, lon = 0;

                #region Get Coord
                if ((int)AppSettingsService.GetSetting("default_location_mode") == 0)
                {
                    Geolocator geo = new Geolocator();
                    var pos = await geo.GetGeopositionAsync().AsTask(cts.Token);
                    lat = pos.Coordinate.Point.Position.Latitude;
                    lon = pos.Coordinate.Point.Position.Longitude;
                }
                else
                {
                    string[] temp = AppSettingsService.GetSetting("default_location").ToString().Split('|');
                    lat = double.Parse(temp[0]);
                    lon = double.Parse(temp[1]);
                }
                #endregion

                if (canRun)
                {
                    var http = new HttpClient();
                    string xmlString = await http.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&mode=xml&appid={APIKEY}")).AsTask(cts.Token);
                    XmlSerializer serializer = new XmlSerializer(typeof(OpenWeatherObj));
                    OpenWeatherObj obj;
                    using (TextReader reader = new StringReader(xmlString))
                    {
                        obj = (OpenWeatherObj)serializer.Deserialize(reader);
                    }

                    var liveTile = TileUpdateManager.CreateTileUpdaterForApplication();

                    string temperature = obj.temperature.TemperatureUnit ? $"{obj.temperature.temp_c}C" : $"{obj.temperature.temp_f}F";
                    string time = DateTime.Now.ToString("ddd h:mm tt");

                    //tile template
                    string tileXMLString = $@"<tile>
                                                <visual baseUri=""Assets/Weather/"">
                                              
                                                  <binding template=""TileSmall"" hint-textStacking=""center"">
                                                    <text hint-style=""body"" hint-align=""center"">{temperature}</text>
                                                  </binding>
                                              
                                                  <binding displayName=""{obj.city.name}"" template=""TileMedium"" hint-textStacking=""center"">
                                                    <text hint-style=""body"" hint-align=""center"">{obj.weather.value}</text>
                                                    <text hint-style=""titleSubtle"" hint-align=""center"">{temperature}</text>
                                                    <text hint-style=""captionSubtle"" hint-align=""center"">{time}</text>
                                                  </binding>
                                              
                                                  <binding displayName=""{obj.city.name}"" template=""TileWide"" branding=""nameAndLogo"" hint-textStacking=""center"">
                                                    <group>
                                                      <subgroup hint-weight=""44"">
                                                        <image src=""{obj.weather.iconUrl}"" hint-removeMargin=""false""/>
                                                      </subgroup>
                                                      <subgroup>
                                                        <text hint-style=""body"">{obj.weather.value}</text>
                                                        <text hint-style=""titleSubtle"">{temperature}</text>
                                                        <text hint-style=""captionSubtle"">{time}</text>
                                                      </subgroup>
                                                    </group>
                                                  </binding>
                                              
                                                  <binding displayName=""{obj.city.name}"" template=""TileLarge"" branding=""nameAndLogo"" hint-textStacking=""center"">
                                                    <group>
                                                      <subgroup hint-weight=""1""/>
                                                      <subgroup hint-weight=""2"">
                                                        <image src=""{obj.weather.iconUrl}""/>
                                                      </subgroup>
                                                      <subgroup hint-weight=""1""/>
                                                    </group>
                                                    <text hint-style=""body"" hint-align=""center"">{obj.weather.value}</text>
                                                    <text hint-style=""titleSubtle"" hint-align=""center"">{temperature}</text>
                                                    <text hint-style=""captionSubtle"" hint-align=""center"">{time}</text>
                                                  </binding>
                                              
                                                </visual>
                                              </tile>";

                    XmlDocument tileXML = new XmlDocument();
                    tileXML.LoadXml(tileXMLString);
                    liveTile.Update(new TileNotification(tileXML));
                }
            }
            catch (Exception) { }
            finally
            {
                _deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}
