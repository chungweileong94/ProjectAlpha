using ProjectAlpha.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ProjectAlpha.Models
{
    [XmlRoot("current")]
    public class OpenWeatherObj
    {
        public OpenWeatherObj()
        {
            IsAnimated = false;
        }

        [XmlElement]
        public City city { get; set; }
        [XmlElement]
        public Temperature temperature { get; set; }
        [XmlElement]
        public Humidity humidity { get; set; }
        [XmlElement]
        public Pressure pressure { get; set; }
        [XmlElement]
        public Wind wind { get; set; }
        [XmlElement]
        public Clouds clouds { get; set; }
        [XmlElement]
        public Visibility visibility { get; set; }
        [XmlElement]
        public Precipitation precipitation { get; set; }
        [XmlElement]
        public Weather weather { get; set; }
        [XmlElement]
        public Lastupdate lastupdate { get; set; }

        public bool IsAnimated { get; set; }
    }

    public class City
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlElement]
        public Coord coord { get; set; }
        [XmlElement]
        public string country { get; set; }
        [XmlElement]
        public Sun sun { get; set; }
        public string location => $"{name}, {country}";
    }

    public class Coord
    {
        [XmlAttribute]
        public float lon { get; set; }
        [XmlAttribute]
        public float lat { get; set; }
    }

    public class Sun
    {
        [XmlAttribute]
        public DateTime rise { get; set; }
        [XmlAttribute]
        public DateTime set { get; set; }
    }

    public class Temperature : INotifyPropertyChanged
    {
        public Temperature()
        {
            try
            {
                TemperatureUnit = (bool)AppSettingsService.GetSetting(AppSettingsService.TEMPERATURE_UNIT);
            }
            catch
            {
                TemperatureUnit = true;
            }
        }

        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public float max { get; set; }
        [XmlAttribute]
        public string unit { get; set; }
        [XmlAttribute]
        public float min { get; set; }
        public string temp_c => $"{value:0}°";
        public string temp_f => $"{value * (9 / 5) + 32:0}°";
        public string temp_max_c => $"{max:0}°";
        public string temp_max_f => $"{max * (9 / 5) + 32:0}°";
        public string temp_min_c => $"{min:0}°";
        public string temp_min_f => $"{min * (9 / 5) + 32:0}°";

        private bool _TemperatureUnit;
        public bool TemperatureUnit
        {
            get { return _TemperatureUnit; }
            set
            {
                Set(ref _TemperatureUnit, value);
                AppSettingsService.SaveSetting(AppSettingsService.TEMPERATURE_UNIT, value);
            }
        }

        #region INotifyProperty Helpers
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        #endregion
    }

    public class Humidity
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string unit { get; set; }
    }

    public class Pressure
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string unit { get; set; }
    }

    public class Wind
    {
        [XmlElement]
        public Speed speed { get; set; }
        [XmlElement]
        public Direction direction { get; set; }
    }

    public class Speed
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
    }

    public class Direction
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string code { get; set; }
    }

    public class Clouds
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
    }

    public class Visibility
    {
        [XmlAttribute]
        public float value { get; set; }
    }

    public class Precipitation
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string mode { get; set; }
    }

    public class Weather
    {
        [XmlAttribute]
        public string value { get; set; }
        [XmlAttribute]
        public float number { get; set; }
        [XmlAttribute]
        public string icon { get; set; }
        public string iconUrl => $"ms-appx:///Assets/Weather/{icon}.png";
        public string backgroundUrl => $"ms-appx:///Assets/Background/{icon}-back.jpg";
    }

    public class Lastupdate
    {
        [XmlAttribute]
        public DateTime value { get; set; }
        public string DateTimeString => value.ToString("d MMM yyyy h:mm tt");
    }
}
