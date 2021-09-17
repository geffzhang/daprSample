using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BingingApi.Controllers
{
    [ApiController]
    [Route("bindingeventdemo")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void ProcessEvent(WeatherForecast forecast)
        {
            _logger.LogInformation("Binding event (demo1) received from Rabbitmq Queue !!!");

            _logger.LogInformation($"-> Today ({forecast.Date.DayOfWeek}) will have {forecast.TemperatureC}C. data：{ Encoding.UTF8.GetString(forecast.Data)}");
        }
    }
}
