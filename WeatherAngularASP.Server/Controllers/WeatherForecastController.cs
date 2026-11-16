using Microsoft.AspNetCore.Mvc;
using WeatherAngularASP.Server.Services;

namespace WeatherAngularASP.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IForecastService _forecastService;

        public WeatherForecastController(IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return _forecastService.GetForecasts(5);
        }
    }
}
