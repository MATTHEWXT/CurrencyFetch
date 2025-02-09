using CurrencyFetch.Core.Infrastructure;
using CurrencyFetch.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyFetch.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly CurrencyService _currencyService;
        public WeatherForecastController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] string symbol)
        {
            await _currencyService.InsertCurrencyDataAsync(symbol);
            return Ok();
        }
        [HttpGet]
        public async Task<ActionResult> GetStatus([FromQuery] Guid jobId)
        {
            var status = await MongoDbService.GetLoadStatus(jobId, "LoadStatuses");
            return Ok(status);
        }
    }
}
