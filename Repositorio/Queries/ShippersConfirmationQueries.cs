using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Models.Database;
using ZagerOrderFixWorker.Repositorio.Interfaces;

namespace ZagerOrderFixWorker.Repositorio.Queries
{
    public class ShippersConfirmationQueries : IShippersConfirmationQueries
    {
        const string CONNECTION = "server=127.0.0.1,20001;Database=InvictaAUX;User=sa;Password=Rte123456";

        private readonly ILogger<ShippersConfirmationQueries> _logger;

        public ShippersConfirmationQueries(ILogger<ShippersConfirmationQueries> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ShippersConfirmation>> getShippersConfirmationByOrderNumber(string orderNumber)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM ShippersConfirmation WHERE OrderNumber = @OrderNumber";
                    var data = await connection.QueryAsync<ShippersConfirmation>(script,
                        new
                        {
                            OrderNumber = orderNumber
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<ShippersConfirmation> getShippersConfirmationByOrderNumberZager(string orderNumber)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM ShippersConfirmation WHERE OrderNumber = @OrderNumber AND SupplierID = 3";
                    var data = await connection.QueryFirstAsync<ShippersConfirmation>(script,
                        new
                        {
                            OrderNumber = orderNumber
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<IEnumerable<ShippersConfirmationEntry>> getShippersConfirmationEntryByOrderNumberSku(int id,string sku)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM ShippersConfirmationEntry WHERE ShipConfirmationID = @ShipConfirmationID AND ItemCode = @ItemCode";
                    var data = await connection.QueryAsync<ShippersConfirmationEntry>(script,
                        new
                        {
                            ShipConfirmationID = id,
                            ItemCode = sku
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<IEnumerable<ShippersConfirmationEntry>> getShippersConfirmationEntriesByID(int id,string trackingNo)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM ShippersConfirmationEntry WHERE ShipConfirmationID = @ShipConfirmationID AND TrackingNumber = @TrackingNumber";
                    var data = await connection.QueryAsync<ShippersConfirmationEntry>(script,
                        new
                        {
                            ShipConfirmationID = id,
                            TrackingNumber = trackingNo
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<int> insertShippersConfirmation(ShippersConfirmation shippers)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "INSERT INTO ShippersConfirmation (OrderNumber,OrderDate,isNewEntry,SupplierID,CompanyId) VALUES (@OrderNumber,@OrderDate,@isNewEntry,@SupplierID,@CompanyId)";
                    var data = await connection.ExecuteAsync(script,
                        new
                        {
                            OrderNumber = shippers.OrderNumber,
                            OrderDate = shippers.OrderDate,
                            isNewEntry = shippers.IsNewEntry,
                            SupplierID = shippers.SupplierID,
                            CompanyId = shippers.CompanyID
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return 0;
            }
        }

        public async Task<int> insertShippersConfirmationEntries(ShippersConfirmationEntry shippers)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "INSERT INTO ShippersConfirmationEntry (LineNumber,ItemCode,OrderedQty,ShippedQty,CancelledQty,ActualShippedDate,Carrier,TrackingNumber,ShipConfirmationID, CompanyID,ItemID,ShipmentID,Invoice) " +
                        "VALUES (@LineNumber,@ItemCode,@OrderedQty,@ShippedQty,@CancelledQty,@ActualShippedDate,@Carrier,@TrackingNumber,@ShipConfirmationID,@CompanyID,@ItemID,@ShipmentID,@Invoice)";
                    var data = await connection.ExecuteAsync(script,
                        new
                        {
                            LineNumber = shippers.LineNumber,
                            ItemCode = shippers.ItemCode,
                            OrderedQty = shippers.OrderedQty,
                            ShippedQty = shippers.ShippedQty,
                            CancelledQty = shippers.CancelledQty,
                            ActualShippedDate = shippers.ActualShippedDate,
                            Carrier = shippers.Carrier,
                            TrackingNumber = shippers.TrackingNumber,
                            ShipConfirmationID = shippers.ShipConfirmationID,
                            CompanyID = shippers.CompanyID,
                            ItemID = shippers.ItemID,
                            ShipmentID = shippers.ShipmentID,
                            Invoice = shippers.Invoice
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return 0;
            }
        }

        public async Task<DataLogJSON> getZagerData(string order)
        {
            try
            {

                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM DataLogJSON WHERE PayloadType = 6 AND JSON LIKE @order";
                    var data = await connection.QueryFirstAsync<DataLogJSON>(script,new {order = $"%{order}%"});
                    return data;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<EcommerceOrderEntry> getEntry(string order, string sku)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "SELECT * FROM eCommerceOrderEntry WHERE OrderNumber = @OrderNumber AND ItemLookupCode LIKE @Sku";
                    var data = await connection.QueryFirstAsync<EcommerceOrderEntry>(script,
                        new
                        {
                            OrderNumber = order,
                            Sku = $"%{sku}%"
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return null;
            }
        }

        public async Task<int> updateShippersConfirmationEntries(int shipConfirmationID, int ID)
        {
            try
            {
                using (var connection = new SqlConnection(CONNECTION))
                {
                    var script = "UPDATE ShippersConfirmationEntry SET ShipConfirmationID = @ShipConfirmationID WHERE ID = @ID";
                    var data = await connection.ExecuteAsync(script,
                        new
                        {
                            ShipConfirmationID = shipConfirmationID,
                            ID = ID
                        });
                    return data;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Database connection failed");
                Console.WriteLine("Database connection failed");
                return 0;
            }
        }
    }
}
