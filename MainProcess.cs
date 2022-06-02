using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Repositorio.Interfaces;
using ZagerOrderFixWorker.Services.Interfaces;

namespace ZagerOrderFixWorker
{
    public class MainProcess
    {
        private readonly ILogger<MainProcess> _logger;
        private readonly IZagerService _zager;
        private readonly IShippersConfirmationQueries _ship; 

        public MainProcess(ILogger<MainProcess> logger, IZagerService zager, IShippersConfirmationQueries ship)
        {
            _logger = logger;
            _zager = zager;
            _ship = ship;
        }
        public async Task StartService(string[] args)
        {
            var date = new DateTime(2022, 03, 01);
            do
            {
                _logger.LogInformation($"Processing for {date.ToString("yyyy-MM-dd")}");
                Console.WriteLine($"Processing for {date.ToString("yyyy-MM-dd")}");
                var data = await _zager.getZagerInformationByDate(date);
                if (data.fulfillment.Count == 0)
                {
                    _logger.LogInformation("There is not fullfilments for this day");
                    Console.WriteLine("There is not fullfilments for this day");
                }
                else
                {
                    foreach (var item in data.fulfillment)
                    {
                        var shippersList = await _ship.getShippersConfirmationByOrderNumber(item.po);
                        if (shippersList == null || shippersList.Count() == 0)
                        {
                            continue;
                        }
                        var shipper = shippersList.FirstOrDefault(x => x.SupplierID == 0);
                        var shipperZager = shippersList.FirstOrDefault(x => x.SupplierID == 3);
                        if (shipperZager == null)
                        {
                            continue;
                        }
                        if (shipper == null)
                        {
                            continue;
                        }
                        var entries = await _ship.getShippersConfirmationEntriesByID(shipper.ID, item.tracking_number);
                        if (entries == null || entries.Count() == 0)
                        {
                            continue;
                        }
                        foreach (var e in entries)
                        {
                            var up = await _ship.updateShippersConfirmationEntries(shipperZager.ID, e.ID);
                            if (up != 0)
                            {
                                _logger.LogInformation($"Order : {shipperZager.OrderNumber} was updated succesfully for zager");
                                Console.WriteLine($"Order : {shipperZager.OrderNumber} was updated succesfully for zager");
                            }
                        }
                    }
                }
                
                date = date.AddDays(1);
            } while (date < new DateTime(2022,04,01));
            

        }
    }
}
