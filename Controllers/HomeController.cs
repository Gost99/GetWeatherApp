using System.Web.Mvc;


namespace GetWeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherData _weatherData 
            = new WeatherData("fb5f0ffcfe2bc13911633f4340f880ee");
        public ActionResult Weather()
        {
            return View();
        }
     
        [HttpPost]
        public ActionResult Weather(string city)
        {
            WeatherResponse responseObj = _weatherData.GetWeather(city);
            return View(responseObj);
        }

    }
}