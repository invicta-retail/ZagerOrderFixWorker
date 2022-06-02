using System;
using System.Threading.Tasks;
using ZagerOrderFixWorker.Models;

namespace ZagerOrderFixWorker.Services.Interfaces
{
    public interface IZagerService
    {
        Task<FixEntity> getZagerInformationByDate(DateTime date);
    }
}
