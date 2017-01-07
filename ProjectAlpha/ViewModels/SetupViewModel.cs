using ProjectAlpha.Models;
using ProjectAlpha.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectAlpha.ViewModels
{
    public class SetupViewModel : BaseViewModel
    {
        #region Properties
        private int _DefaultLocationMode;
        public int DefaultLocationMode
        {
            get { return _DefaultLocationMode; }
            set
            {
                if (value != -1)
                {
                    Set(ref _DefaultLocationMode, value);
                    IsSaveAvailable = DefaultLocationMode == 0 ? true : string.IsNullOrEmpty(DefaultLocationCoord) ? false : true;
                }
            }
        }

        private string _DefaultLocationCoord;
        public string DefaultLocationCoord
        {
            get { return _DefaultLocationCoord; }
            set
            {
                Set(ref _DefaultLocationCoord, value);
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

        private string _SearchKeyword;
        public string SearchKeyword
        {
            get { return _SearchKeyword; }
            set { Set(ref _SearchKeyword, value); }
        }

        private List<OpenWeatherObj> _SearchResult;
        public List<OpenWeatherObj> SearchResult
        {
            get { return _SearchResult; }
            set { Set(ref _SearchResult, value); }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { Set(ref _IsLoading, value); }
        }
        #endregion

        public SetupViewModel()
        {
            IsLoading = false;
            DefaultLocationMode = 0;
            DefaultLocationFullName = "Specific location";
        }

        public async Task SetupSettingsAsync()
        {
            IsSaveAvailable = false;
            AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_MODE, DefaultLocationMode);
            if (DefaultLocationMode == 1)
            {
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION, DefaultLocationCoord);
                AppSettingsService.SaveSetting(AppSettingsService.DEFAULT_LOCATION_FULL_NAME, DefaultLocationFullName);
            }

            await LiveTileService.RegisterTaskAsync();
        }

        public async void SearchLocation()
        {
            try
            {
                IsLoading = true;
                var obj = await OpenWeatherService.GetWeatherByCityNameAsync(SearchKeyword, "");
                SearchResult = new List<OpenWeatherObj>() { obj };
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }

        public void SelectLocation()
        {
            SearchKeyword = string.Empty;
            DefaultLocationCoord = $"{SearchResult[0].city.coord.lat}|{SearchResult[0].city.coord.lon}";
            DefaultLocationFullName = SearchResult[0].city.location;
            SearchResult = null;
        }
    }
}
