using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class InteriorDoorSkinCollection : VirtualCollection<InteriorDoorSkinModel>
    {
        private DataRequest<InteriorDoorSkin> _dataRequest = null;

        public InteriorDoorSkinCollection(IInteriorDoorSkinService interiorDoorSkinService, ILogService logService) : base(logService)
        {
            InteriorDoorSkinService = interiorDoorSkinService;
        }

        public IInteriorDoorSkinService InteriorDoorSkinService { get; }

        private InteriorDoorSkinModel _defaultItem = InteriorDoorSkinModel.CreateEmpty();
        protected override InteriorDoorSkinModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<InteriorDoorSkin> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await InteriorDoorSkinService.GetInteriorDoorSkinsCountAsync(_dataRequest);
                Ranges[0] = await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<InteriorDoorSkinModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("InteriorDoorSkinCollection", "Fetch", ex);
            }
            return null;
        }
    }
}
