using Newtonsoft.Json;
using ProjectAlpha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ProjectAlpha.Services
{
    public abstract class CountryTranslateService
    {
        public static async Task<string> TranslateAsync(string countryCode)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Country.json"));
            string jsonString = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<List<Country>>(jsonString).Where(c => c.Code == countryCode).ToList()[0].Name;
        }
    }
}
