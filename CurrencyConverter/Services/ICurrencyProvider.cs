using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Models;
using CurrencyConverter.Controllers;
using Polly.Registry;

namespace CurrencyConverter.Services
{
    public interface ICurrencyProvider
    {
        Task<IActionResult> GetLatestExchangeRates(string baseCurrency, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider);
        Task<IActionResult> ConvertCurrency(string currencyFrom, string currencyTo, int amount, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider);
        Task<IActionResult> GetHistoricExchangeRates(string baseCurrency, string startDate, string endDate, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider);
    }
}
