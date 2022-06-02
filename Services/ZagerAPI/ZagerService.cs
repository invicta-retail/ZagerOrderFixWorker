using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using ZagerOrderFixWorker.Services.Interfaces;
using System.Net.Http;
using Newtonsoft.Json;
using ZagerOrderFixWorker.Models;
using System.Collections.Generic;

namespace ZagerOrderFixWorker.Services.ZagerAPI
{
    public class ZagerService : IZagerService
    {
        readonly private ILogger<ZagerService> _logger;

        public ZagerService(ILogger<ZagerService> logger)
        {
            _logger = logger;
        }

        const string URL = "https://zagerwatch.com/api/webshop_orders/fulfilled/1?modified_at=";
        const string SECRET = @"3b/4KT3""6R=Q%dW";

        public async Task<FixEntity> getZagerInformationByDate(DateTime date)
        {
            try
            {
                _logger.LogInformation("Extracting information from ZagerAPI");
                Console.WriteLine("Extracting information from ZagerAPI");
                _logger.LogInformation("Preparing client for API");
                Console.WriteLine("Preparing client for API");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Apikey", SECRET);
                    _logger.LogInformation("Executing GET fulfillment by day");
                    Console.WriteLine("Executing GET fulfillment by day");
                    _logger.LogInformation($"URL : {URL+date.ToString("yyyy-MM-dd")}");
                    Console.WriteLine($"URL : {URL+date.ToString("yyyy-MM-dd")}");
                    var response = await client.GetAsync(URL+date.ToString("yyyy-MM-dd"));
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Extraction completed");
                        Console.WriteLine("Extraction completed");
                        var data = await response.Content.ReadAsStringAsync();
                        var zagerInfo = JsonConvert.DeserializeObject<FixEntity>(data);
                        return zagerInfo;
                    }
                    else
                    {
                        _logger.LogError("ZagerService connection failed");
                        Console.WriteLine("ZagerService connection failed");
                        throw new Exception("ZagerService connection failed");
                    }

                }

            }
            catch (Exception)
            {
                _logger.LogError("ZagerService connection failed");
                throw new Exception("ZagerService connection failed");
            }
        }
    }
}
