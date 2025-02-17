using CurrencyConverter.Controllers;
using CurrencyConverter.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly.Registry;
using System.Diagnostics.Eventing.Reader;
using System.Net;

namespace CurrencyConverter.Services
{
    public class FrankfurterService : ControllerBase, ICurrencyProvider
    {
        public FrankfurterService() { }
        public async Task<IActionResult> GetLatestExchangeRates(string baseCurrency, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            var pipeline = resiliencePipelineProvider.GetPipeline("default");
            ExchangeRates? exchangeRates = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string hostName = Dns.GetHostName();
                    string IpAddr = Dns.GetHostEntry(hostName).AddressList[1].ToString();

                    logger.LogInformation("Get Latest ExchangeRates Request: " + "https://api.frankfurter.dev/v1/latest?base=" + baseCurrency);
                    using (var response = await pipeline.ExecuteAsync(async ct => await httpClient.GetAsync("https://api.frankfurter.dev/v1/latest?base=" + baseCurrency, ct)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            logger.LogInformation("Get Latest ExchangeRates Response: " + JsonConvert.SerializeObject(apiResponse));
                            exchangeRates = JsonConvert.DeserializeObject<ExchangeRates>(apiResponse);
                            return Ok(exchangeRates);
                        }
                        else
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                logger.LogError("GetLatestExchangeRates - Error: Exchange rates not found.");
                                throw new Exception("Exchange rates not found.");
                            }
                            else
                            {
                                logger.LogError("GetLatestExchangeRates - Error: Failed to fetch data from the server. Status code: " + response.StatusCode);
                                throw new Exception("Failed to fetch data from the server. Status code: " + response.StatusCode);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("GetLatestExchangeRates - Error: HTTP request failed: " + ex.Message);
                throw new Exception("HTTP request failed: " + ex.Message);
            }
            catch (JsonException ex)
            {
                logger.LogError("GetLatestExchangeRates - Error: HTTP request failed: " + ex.Message);
                throw new Exception("JSON deserialization failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("GetLatestExchangeRates - Error: HTTP request failed: " + ex.Message);
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }
        public async Task<IActionResult> ConvertCurrency(string currencyFrom, string currencyTo, int amount, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            var pipeline = resiliencePipelineProvider.GetPipeline("default");
            string apiResponse = string.Empty;
            ExchangeRates? exchangeRates = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    logger.LogInformation("ConvertCurrency Request: " + "https://api.frankfurter.dev/v1/latest?base=" + currencyFrom + "&symbols=" + currencyTo);
                    using (var response = await pipeline.ExecuteAsync(async ct => await httpClient.GetAsync("https://api.frankfurter.dev/v1/latest?base=" + currencyFrom + "&symbols=" + currencyTo, ct)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                            logger.LogInformation("ConvertCurrency response: " + JsonConvert.SerializeObject(apiResponse));
                            exchangeRates = JsonConvert.DeserializeObject<ExchangeRates>(apiResponse);
                            JObject jsonObj = JObject.Parse(JsonConvert.SerializeObject(exchangeRates));
                            string value = jsonObj["rates"]?[currencyTo.ToUpper()]?.ToString()!;
                            double convertedAmt = amount * Convert.ToDouble(value);
                            return Ok(Math.Round(convertedAmt, 2));
                        }
                        else
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                logger.LogError("ConvertCurrency Error: Exchange rates history not found.");
                                throw new Exception("Exchange rates history not found.");
                            }
                            else
                            {
                                logger.LogError("ConvertCurrency Error: Failed to fetch data from the server. Status code.");
                                throw new Exception("Failed to fetch data from the server. Status code: " + response.StatusCode);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("ConvertCurrency Error: " + ex.Message);
                throw new Exception("HTTP request failed: " + ex.Message);
            }
            catch (JsonException ex)
            {
                logger.LogError("ConvertCurrency Error: " + ex.Message);
                throw new Exception("JSON deserialization failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("ConvertCurrency Error: " + ex.Message);
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }
        public async Task<IActionResult> GetHistoricExchangeRates(string baseCurrency, string startDate, string endDate, ILogger<object> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            var pipeline = resiliencePipelineProvider.GetPipeline("default");
            string apiResponse = string.Empty;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    logger.LogInformation("GetHistoricExchangeRates Request: " + "https://api.frankfurter.dev/v1/" + startDate + ".." + endDate);
                    using (var response = await pipeline.ExecuteAsync(async ct => await httpClient.GetAsync("https://api.frankfurter.dev/v1/" + startDate + ".." + endDate, ct)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                            logger.LogInformation("GetHistoricExchangeRates Response: " + JsonConvert.SerializeObject(apiResponse));
                        }
                        else
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                logger.LogError("GetHistoricExchangeRates Error: Exchange rates history not found.");
                                throw new Exception("Exchange rates history not found.");
                            }
                            else
                            {
                                logger.LogError("GetHistoricExchangeRates Error: Failed to fetch data from the server. Status code: " + response.StatusCode);
                                throw new Exception("Failed to fetch data from the server. Status code: " + response.StatusCode);
                            }
                        }
                        return Ok(JsonConvert.SerializeObject(apiResponse, Formatting.Indented));
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("GetHistoricExchangeRates Error: " + ex.Message);
                throw new Exception("HTTP request failed: " + ex.Message);
            }
            catch (JsonException ex)
            {
                logger.LogError("GetHistoricExchangeRates Error: " + ex.Message);
                throw new Exception("JSON deserialization failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("GetHistoricExchangeRates Error: " + ex.Message);
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
