using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface IInteriorDoorService
    {
        Task<InteriorDoorModel> GetInteriorDoorAsync(long orderID, int interiorDoorID);
        Task<IList<InteriorDoorModel>> GetInteriorDoorsAsync(DataRequest<InteriorDoor> request);
        Task<IList<InteriorDoorModel>> GetInteriorDoorsAsync(int skip, int take, DataRequest<InteriorDoor> request);
        Task<int> GetInteriorDoorsCountAsync(DataRequest<InteriorDoor> request);

        Task<int> UpdateInteriorDoorAsync(InteriorDoorModel model);

        Task<int> DeleteInteriorDoorAsync(InteriorDoorModel model);
        Task<int> DeleteInteriorDoorRangeAsync(int index, int length, DataRequest<InteriorDoor> request);

        Task SaveInteriorDoorListToExcelFileAsync(IList<InteriorDoorModel> interiorDoorList);
    }
}
