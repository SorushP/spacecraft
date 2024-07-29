using Microsoft.AspNetCore.Mvc;
using Weather.Services;

namespace Weather.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<ActionResult> GetWeatherData()
        {
            var weatherData = await _weatherService.FetchWeatherDataAsync();
            return Content(weatherData, "application/json");
        }
    }
}
