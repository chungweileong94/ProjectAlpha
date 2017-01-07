using System.Xml.Serialization;
using Tasks.Services;

namespace Tasks.Models
{
    [XmlRoot("current")]
    public sealed class OpenWeatherObj
    {
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
    }

    public sealed class City
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlElement]
        public Coord coord { get; set; }
        [XmlElement]
        public string country { get; set; }
        public string location => $"{name}, {country}";
    }

    public sealed class Coord
    {
        [XmlAttribute]
        public float lon { get; set; }
        [XmlAttribute]
        public float lat { get; set; }
    }

    public sealed class Temperature
    {
        public Temperature()
        {
            TemperatureUnit = (bool)AppSettingsService.GetSetting("temperature_unit");
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
        public bool TemperatureUnit { get; set; }
    }

    public sealed class Humidity
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string unit { get; set; }
    }

    public sealed class Pressure
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string unit { get; set; }
    }

    public sealed class Wind
    {
        [XmlElement]
        public Speed speed { get; set; }
        [XmlElement]
        public Direction direction { get; set; }
    }

    public sealed class Speed
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
    }

    public sealed class Direction
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string code { get; set; }
    }

    public sealed class Clouds
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public float value { get; set; }
    }

    public sealed class Visibility
    {
        [XmlAttribute]
        public float value { get; set; }
    }

    public sealed class Precipitation
    {
        [XmlAttribute]
        public float value { get; set; }
        [XmlAttribute]
        public string mode { get; set; }
    }

    public sealed class Weather
    {
        [XmlAttribute]
        public string value { get; set; }
        [XmlAttribute]
        public float number { get; set; }
        [XmlAttribute]
        public string icon { get; set; }
        public string iconUrl => $"{icon}.png";
    }
}
