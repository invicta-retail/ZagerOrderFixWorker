using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Models.Database;
using ZagerOrderFixWorker.Models.Zager;
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
            //await FixManually();
            await FixZagerOrder();
            //await FixZagerMatch();
        }

        private async Task FixManually()
        {
            Console.WriteLine("Select option");
            Console.WriteLine("1 Process, 2 Exit");
            var result = Console.ReadLine();
            if(result == "1")
            {
                Console.WriteLine("Insert the orderNumber");
                _logger.LogInformation("Insert the orderNumber");
                var order = (string)Console.ReadLine();
                Console.WriteLine("Insert the tracking number");
                _logger.LogInformation("Insert the tracking number");
                var tracking = (string)Console.ReadLine();
                Console.WriteLine($"Order {order}, Tracking {tracking}");
                _logger.LogInformation($"Order {order}, Tracking {tracking}");
                var shippersList = await _ship.getShippersConfirmationByOrderNumber(order);
                if (shippersList == null || shippersList.Count() == 0)
                {
                    var dataLog = await _ship.getZagerData(order);
                    if (dataLog != null)
                    {
                        var json = JsonConvert.DeserializeObject<ZagerJson>(dataLog.JSON);
                        var resp = JsonConvert.DeserializeObject<Response>(dataLog.responseString);
                        var sc = new ShippersConfirmation()
                        {
                            OrderDate = dataLog.AddedDate.AddDays(2),
                            OrderNumber = order,
                            IsNewEntry = 1,
                            SupplierID = 3,
                            CompanyID = 0
                        };
                        var rsc = await _ship.insertShippersConfirmation(sc);
                        if (rsc > 0)
                        {
                            var nsc = await _ship.getShippersConfirmationByOrderNumberZager(order);
                            if (nsc != null)
                            {
                                foreach (var i in json.order_lines)
                                {
                                    var eo = await _ship.getEntry(order, i.product_number);
                                    if (eo != null)
                                    {
                                        var sce = new ShippersConfirmationEntry()
                                        {
                                            ActualShippedDate = dataLog.AddedDate.AddDays(2),
                                            Carrier = json.shipping_method,
                                            CancelledQty = 0,
                                            CompanyID = 0,
                                            Invoice = "-200",
                                            ItemCode = i.product_number,
                                            OrderedQty = eo.QtyOrdered,
                                            ItemID = eo.ItemID,
                                            LineNumber = eo.SimpleProdLineNo,
                                            ShipConfirmationID = nsc.ID,
                                            ShippedQty = eo.QtyOrdered,
                                            TrackingNumber = tracking,
                                            ShipmentID = Convert.ToInt32(resp.success)
                                        };
                                        var rsce = await _ship.insertShippersConfirmationEntries(sce);
                                        if (rsce > 0)
                                        {
                                            _logger.LogInformation($"Order {order} , Item {i.product_number} Processed succesfully");
                                            Console.WriteLine($"Order {order} , Item {i.product_number} Processed succesfully");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var sh = shippersList.FirstOrDefault(x => x.SupplierID == 3);
                    if (sh == null)
                    {
                        var dataLog = await _ship.getZagerData(order);
                        if (dataLog != null)
                        {
                            var json = JsonConvert.DeserializeObject<ZagerJson>(dataLog.JSON);
                            var resp = JsonConvert.DeserializeObject<Response>(dataLog.responseString);
                            var sc = new ShippersConfirmation()
                            {
                                OrderDate = dataLog.AddedDate.AddDays(2),
                                OrderNumber = order,
                                IsNewEntry = 1,
                                SupplierID = 3,
                                CompanyID = 0
                            };
                            var rsc = await _ship.insertShippersConfirmation(sc);
                            if (rsc > 0)
                            {
                                var nsc = await _ship.getShippersConfirmationByOrderNumberZager(order);
                                if (nsc != null)
                                {
                                    foreach (var i in json.order_lines)
                                    {
                                        var eo = await _ship.getEntry(order, i.product_number);
                                        if (eo != null)
                                        {
                                            var sce = new ShippersConfirmationEntry()
                                            {
                                                ActualShippedDate = dataLog.AddedDate.AddDays(2),
                                                Carrier = json.shipping_method,
                                                CancelledQty = 0,
                                                CompanyID = 0,
                                                Invoice = "-200",
                                                ItemCode = i.product_number,
                                                OrderedQty = eo.QtyOrdered,
                                                ItemID = eo.ItemID,
                                                LineNumber = eo.SimpleProdLineNo,
                                                ShipConfirmationID = nsc.ID,
                                                ShippedQty = eo.QtyOrdered,
                                                TrackingNumber = tracking,
                                                ShipmentID = Convert.ToInt32(resp.success)
                                            };
                                            var rsce = await _ship.insertShippersConfirmationEntries(sce);
                                            if (rsce > 0)
                                            {
                                                _logger.LogInformation($"Order {order} , Item {i.product_number} Processed succesfully");
                                                Console.WriteLine($"Order {order} , Item {i.product_number} Processed succesfully");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                await FixManually();
            }
            else
            {
                Console.WriteLine("Closing worker");
                Environment.Exit(0);
            }


        }

        private async Task FixZagerMatch()
        {
            var date = new DateTime(2022, 02, 01);
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
                        if(item.po == "1006393936")
                        {
                            Console.WriteLine("OrdenDetectada");
                        }
                        var shippersList = await _ship.getShippersConfirmationByOrderNumber(item.po);
                        if (shippersList == null || shippersList.Count() == 0)
                        {
                            var dataLog = await _ship.getZagerData(item.po);
                            if(dataLog != null)
                            {
                                var json = JsonConvert.DeserializeObject<ZagerJson>(dataLog.JSON);
                                var resp = JsonConvert.DeserializeObject<Response>(dataLog.responseString);
                                var sc = new ShippersConfirmation()
                                {
                                    OrderDate = dataLog.AddedDate.AddDays(2),
                                    OrderNumber = item.po,
                                    IsNewEntry = 1,
                                    SupplierID = 3,
                                    CompanyID = 0
                                };
                                var rsc = await _ship.insertShippersConfirmation(sc);
                                if (rsc > 0)
                                {
                                    var nsc = await _ship.getShippersConfirmationByOrderNumberZager(item.po);
                                    if(nsc != null)
                                    {
                                        foreach(var i in json.order_lines)
                                        {
                                            var eo = await _ship.getEntry(item.po,i.product_number);
                                            if(eo != null)
                                            {
                                                var sce = new ShippersConfirmationEntry()
                                                {
                                                    ActualShippedDate = dataLog.AddedDate.AddDays(2),
                                                    Carrier = json.shipping_method,
                                                    CancelledQty = 0,
                                                    CompanyID = 0,
                                                    Invoice = item.invoice_number,
                                                    ItemCode = i.product_number,
                                                    OrderedQty = eo.QtyOrdered,
                                                    ItemID = eo.ItemID,
                                                    LineNumber = eo.SimpleProdLineNo,
                                                    ShipConfirmationID = nsc.ID,
                                                    ShippedQty = eo.QtyOrdered,
                                                    TrackingNumber = item.tracking_number,
                                                    ShipmentID = Convert.ToInt32(resp.success)
                                                };
                                                var rsce = await _ship.insertShippersConfirmationEntries(sce);
                                                if (rsce > 0)
                                                {
                                                    _logger.LogInformation($"Order {item.po} , Item {i.product_number} Processed succesfully");
                                                    Console.WriteLine($"Order {item.po} , Item {i.product_number} Processed succesfully");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else 
                        {
                            var sh = shippersList.FirstOrDefault(x => x.SupplierID == 3);
                            if(sh == null)
                            {
                                var dataLog = await _ship.getZagerData(item.po);
                                if (dataLog != null)
                                {
                                    var json = JsonConvert.DeserializeObject<ZagerJson>(dataLog.JSON);
                                    var resp = JsonConvert.DeserializeObject<Response>(dataLog.responseString);
                                    var sc = new ShippersConfirmation()
                                    {
                                        OrderDate = dataLog.AddedDate.AddDays(2),
                                        OrderNumber = item.po,
                                        IsNewEntry = 1,
                                        SupplierID = 3,
                                        CompanyID = 0
                                    };
                                    var rsc = await _ship.insertShippersConfirmation(sc);
                                    if (rsc > 0)
                                    {
                                        var nsc = await _ship.getShippersConfirmationByOrderNumberZager(item.po);
                                        if (nsc != null)
                                        {
                                            foreach (var i in json.order_lines)
                                            {
                                                var eo = await _ship.getEntry(item.po, i.product_number);
                                                if (eo != null)
                                                {
                                                    var sce = new ShippersConfirmationEntry()
                                                    {
                                                        ActualShippedDate = dataLog.AddedDate.AddDays(2),
                                                        Carrier = json.shipping_method,
                                                        CancelledQty = 0,
                                                        CompanyID = 0,
                                                        Invoice = item.invoice_number,
                                                        ItemCode = i.product_number,
                                                        OrderedQty = eo.QtyOrdered,
                                                        ItemID = eo.ItemID,
                                                        LineNumber = eo.SimpleProdLineNo,
                                                        ShipConfirmationID = nsc.ID,
                                                        ShippedQty = eo.QtyOrdered,
                                                        TrackingNumber = item.tracking_number,
                                                        ShipmentID = Convert.ToInt32(resp.success)
                                                    };
                                                    var rsce = await _ship.insertShippersConfirmationEntries(sce);
                                                    if (rsce > 0)
                                                    {
                                                        _logger.LogInformation($"Order {item.po} , Item {i.product_number} Processed succesfully");
                                                        Console.WriteLine($"Order {item.po} , Item {i.product_number} Processed succesfully");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                date = date.AddDays(1);
            } while (date < new DateTime(2022, 03, 01));
        }

        private async Task FixZagerOrder()
        {
            var date = new DateTime(2022, 05, 01);
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
                        if (shipper == null)
                        {
                            continue;
                        }
                        if (shipperZager == null)
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
            } while (date < new DateTime(2022, 06, 01));
        }
    }
}
