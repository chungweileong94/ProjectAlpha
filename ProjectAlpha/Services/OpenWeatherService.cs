using ProjectAlpha.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;

namespace ProjectAlpha.Services
{
    public abstract class OpenWeatherService
    {
        private const string APIKEY = "...";

        public static async Task<OpenWeatherObj> GetWeatherByCoordinateAsync(double lat, double lon)
        {
            HttpClient http = new HttpClient();
            string xmlString = await http.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&mode=xml&appid={APIKEY}"));
            XmlSerializer serializer = new XmlSerializer(typeof(OpenWeatherObj));
            OpenWeatherObj obj;
            using (TextReader reader = new StringReader(xmlString))
            {
                obj = (OpenWeatherObj)serializer.Deserialize(reader);
            }

            obj.city.country = await CountryTranslateService.TranslateAsync(obj.city.country);

            return obj;
        }

        public static async Task<OpenWeatherObj> GetWeatherByCityNameAsync(string city, string countryCode = "")
        {
            HttpClient http = new HttpClient();
            string xmlString = await http.GetStringAsync(new Uri($"http://api.openweathermap.org/data/2.5/weather?q={city},{countryCode}&units=metric&mode=xml&appid={APIKEY}"));
            XmlSerializer serializer = new XmlSerializer(typeof(OpenWeatherObj));
            OpenWeatherObj obj;
            using (TextReader reader = new StringReader(xmlString))
            {
                obj = (OpenWeatherObj)serializer.Deserialize(reader);
            }

            obj.city.country = await CountryTranslateService.TranslateAsync(obj.city.country);

            return obj;
        }
    }
}
