using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface IInteriorDoorSkinService
    {
        Task<InteriorDoorSkinModel> GetInteriorDoorSkinAsync(string id);
        Task<IList<InteriorDoorSkinModel>> GetInteriorDoorSkinsAsync(DataRequest<InteriorDoorSkin> request);
        Task<IList<InteriorDoorSkinModel>> GetInteriorDoorSkinsAsync(int skip, int take, DataRequest<InteriorDoorSkin> request);
        Task<int> GetInteriorDoorSkinsCountAsync(DataRequest<InteriorDoorSkin> request);

        Task<int> UpdateInteriorDoorSkinAsync(InteriorDoorSkinModel model);

        Task<int> DeleteInteriorDoorSkinAsync(InteriorDoorSkinModel model);
        Task<int> DeleteInteriorDoorSkinRangeAsync(int index, int length, DataRequest<InteriorDoorSkin> request);
    }
}
