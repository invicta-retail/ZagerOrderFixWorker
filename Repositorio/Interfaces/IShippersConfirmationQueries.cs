using System.Collections.Generic;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Models.Database;

namespace ZagerOrderFixWorker.Repositorio.Interfaces
{
    public interface IShippersConfirmationQueries
    {
        Task<IEnumerable<ShippersConfirmation>> getShippersConfirmationByOrderNumber(string orderNumber);
        Task<IEnumerable<ShippersConfirmationEntry>> getShippersConfirmationEntriesByID(int id, string trackingNo);
        Task<int> updateShippersConfirmationEntries(int shipConfirmationID, int ID);
        Task<int> insertShippersConfirmation(ShippersConfirmation shippers);
        Task<int> insertShippersConfirmationEntries(ShippersConfirmationEntry shippers);
        Task<DataLogJSON> getZagerData(string order);
        Task<EcommerceOrderEntry> getEntry(string order, string sku);
        Task<IEnumerable<ShippersConfirmationEntry>> getShippersConfirmationEntryByOrderNumberSku(int id, string sku);
        Task<ShippersConfirmation> getShippersConfirmationByOrderNumberZager(string orderNumber);
    }
}
