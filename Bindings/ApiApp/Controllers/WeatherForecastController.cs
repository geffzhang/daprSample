using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BingingApi.Controllers
{
    [ApiController]
    [Route("bindingeventdemo2")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void ProcessEvent(byte[] data)
        {
            _logger.LogInformation("Binding event (demo1) received from Rabbitmq Queue !!!");
             _logger.LogInformation($"-> data：{ Encoding.UTF8.GetString(data)}");

            // _logger.LogInformation($"-> Today ({forecast.Date.DayOfWeek}) will have {forecast.TemperatureC}C. data：{ Encoding.UTF8.GetString(forecast.Data)}");
        }
    }
}
