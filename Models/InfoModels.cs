using Newtonsoft.Json;

public class CountryInfo
{
    public string Country { get; set; }

}

public class WeatherInfo
{
    public string Main { get; set; }

}

public class TemperatureInfo
{
    [JsonProperty("temp_min")]
    public float Temp_min { get; set; }

    [JsonProperty("temp_max")]
    public float Temp_max { get; set; }
}
