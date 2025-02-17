using CurrencyConverter.Services;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Polly.Registry;

namespace CurrencyConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FrankfurterController : ControllerBase
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly IAuthProvider _authProvider;
        private readonly ILogger<FrankfurterController> _logger;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;
        public FrankfurterController(ILogger<FrankfurterController> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            _currencyProvider = CurrencyProviderFactory.GetCurrencyProvider("Frankfurter");
            _authProvider = AuthProviderFactory.GetAuthProvider("JWT");
            _logger = logger;
            _resiliencePipelineProvider = resiliencePipelineProvider;
        }

        [HttpGet]
        [OutputCache]
        [Route("/GetLatestExchangeRates")]
        public async Task<IActionResult> GetLatestExchangeRates(string baseCurrency)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
                else if(_authProvider.ValidateTkn(token))
                {
                    _logger.LogInformation("Get Latest ExchangeRates called sucessfully");
                    return await _currencyProvider.GetLatestExchangeRates(baseCurrency, _logger, _resiliencePipelineProvider);
                }
                else
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetLatestExchangeRates Error: " + ex.Message);
                return BadRequest();
            }
            
        }

        [HttpGet]
        [OutputCache]
        [Route("/GetCurrencyConversion")]
        public async Task<IActionResult> GetCurrencyConversion(string currencyFrom, string currencyTo, int amount)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
                else if (_authProvider.ValidateTkn(token))
                {
                    _logger.LogInformation("Get Currency Conversion called sucessfully");
                    return await _currencyProvider.ConvertCurrency(currencyFrom, currencyTo, amount, _logger, _resiliencePipelineProvider);
                }
                else
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCurrencyConversion Error: " + ex.Message);
                return BadRequest();
            }  
        }

        [HttpGet]
        [OutputCache]
        [Route("/GetHistoricExchangeRates")]
        public async Task<IActionResult> GetHistoricExchangeRates(string baseCurrency, string startDate, string endDate)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
                else if (_authProvider.ValidateTkn(token))
                {
                    _logger.LogInformation("Get Historic Exchange Rates called sucessfully");
                    return await _currencyProvider.GetHistoricExchangeRates(baseCurrency, startDate, endDate, _logger, _resiliencePipelineProvider);
                }
                else
                {
                    _logger.LogError("Invalid Token");
                    return Unauthorized("Invalid Token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetHistoricExchangeRates Error: " + ex.Message);
                return BadRequest();
            }
           
        }
    }
}
