using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Models.Database;

namespace ZagerOrderFixWorker.Repositorio.Interfaces
{
    public interface IShippersConfirmationQueries
    {
        Task<IEnumerable<ShippersConfirmation>> getShippersConfirmationByOrderNumber(string orderNumber);
        Task<IEnumerable<ShippersConfirmationEntry>> getShippersConfirmationEntriesByID(int id, string trackingNo);
        Task<int> updateShippersConfirmationEntries(int shipConfirmationID, int ID);
    }
}
