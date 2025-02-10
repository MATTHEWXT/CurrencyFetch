using CurrencyFetch.Core.Infrastructure;
using CurrencyFetch.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CurrencyFetch.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyFetchController : ControllerBase
    {
        private readonly CurrencyService _currencyService;
        private readonly ILogger<CurrencyFetchController> _logger;

        public CurrencyFetchController(CurrencyService currencyService, ILogger<CurrencyFetchController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetStatus([FromQuery] Guid jobId)
        {
            try
            {
                var status = await MongoDbService.GetLoadStatus(jobId, "LoadStatuses");
                return Ok(status);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error getting data from MongoDb.");
                return BadRequest();
            }
        }
    }
}
