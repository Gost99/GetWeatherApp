using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

#region WeatherModels.WeatherResponse
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class CurrentWeatherResponse
{
    [JsonProperty("name")]
    public string Name { get; set; }

    public WeatherInfo Weather { get; set; }

    [JsonProperty("main")]
    public TemperatureInfo Main { get; set; }

    [JsonProperty("sys")]
    public CountryInfo Sys { get; set; }
}

public class NullWeatherResponse : CurrentWeatherResponse
{
    public NullWeatherResponse() : base()
    {
        Name = "Not found";
        Weather = new WeatherInfo() { Main = "Not found" };
        Main = new TemperatureInfo();
        Sys = new CountryInfo() { Country = "Not found" };
    }
}
#endregion

#region WeatherModels.WeatherForecastResponse
public class WeatherForecastResponse
{
    private DateTime _date;
    [JsonProperty("dt_txt")]
    public DateTime Date {
        get
        {
            return _date;
        }
        set {
            _date = value.ToLocalTime();
        }
    }

    [JsonProperty("main")]
    public TemperatureInfo Main { get; set; }

    [JsonProperty("weather")]
    public WeatherInfo[] Weather { get; set; }
}

public class NullWeatherForecastResponse : WeatherForecastResponse
{
    public NullWeatherForecastResponse() : base()
    {
        Main = new TemperatureInfo();
        Weather = new WeatherInfo[] { new WeatherInfo() { Main = "Not found" } };
    }
}
#endregion

public class WeatherResponse
{
    public CurrentWeatherResponse CurrentWeather { get; set; }

    public IEnumerable<WeatherForecastResponse> WeatherForecast { get; set; }
}
public class WeatherData
{
    private string _apiKey;

    public WeatherData(string apiKey)
    {
        _apiKey = apiKey;
    }
    private CurrentWeatherResponse GetCurrentWeather(string city)
    {
        string url = "http://api.openweathermap.org/data/2.5/weather?q="
            + city.Replace(' ', '+') + "&units=metric&appid=" + _apiKey;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseText = String.Empty;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }

            JObject parseWeather = JObject.Parse(responseText);
            IList<JToken> weather = parseWeather["weather"].Children().ToList();
            var responseObject = JsonConvert.DeserializeObject<CurrentWeatherResponse>(responseText);
            responseObject.Weather = weather[0].ToObject<WeatherInfo>();


            return responseObject;
        }
        catch (WebException)
        {
            return new NullWeatherResponse();
        }
    }

    private IEnumerable<WeatherForecastResponse> GetForecast(string city)
    {
        string url = "http://api.openweathermap.org/data/2.5/forecast?q="
            + city.Replace(' ', '+') + "&units=metric&appid=" + _apiKey;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseText = String.Empty;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }

            JObject parsedObject = JObject.Parse(responseText);

            JArray parsedArray = (JArray)parsedObject["list"];

            IList<WeatherForecastResponse> responseArray = parsedArray.ToObject<IList<WeatherForecastResponse>>();

            return responseArray;
        }
        catch (WebException e)
        {
            return new List<WeatherForecastResponse>() { new NullWeatherForecastResponse() };
        }
    }

    public WeatherResponse GetWeather(string city)
    {
        WeatherResponse result = new WeatherResponse
        {
            CurrentWeather = GetCurrentWeather(city),
            WeatherForecast = GetForecast(city)
        };
        return result;
    }
}

