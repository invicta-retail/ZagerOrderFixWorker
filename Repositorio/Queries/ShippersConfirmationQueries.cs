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
