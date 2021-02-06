using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Data.Services;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class InteriorDoorSkinService : IInteriorDoorSkinService
    {
        public InteriorDoorSkinService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<InteriorDoorSkinModel> GetInteriorDoorSkinAsync(string id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetInteriorDoorSkinAsync(dataService, id);
            }
        }
        static private async Task<InteriorDoorSkinModel> GetInteriorDoorSkinAsync(IDataService dataService, string id)
        {
            var item = await dataService.GetInteriorDoorSkinAsync(id);
            if (item != null)
            {
                return await CreateInteriorDoorSkinModelAsync(item);
            }
            return null;
        }

        public async Task<IList<InteriorDoorSkinModel>> GetInteriorDoorSkinsAsync(DataRequest<InteriorDoorSkin> request)
        {
            var collection = new InteriorDoorSkinCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<InteriorDoorSkinModel>> GetInteriorDoorSkinsAsync(int skip, int take, DataRequest<InteriorDoorSkin> request)
        {
            var models = new List<InteriorDoorSkinModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetInteriorDoorSkinsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateInteriorDoorSkinModelAsync(item));
                }
                return models;
            }
        }

        public async Task<int> GetInteriorDoorSkinsCountAsync(DataRequest<InteriorDoorSkin> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetInteriorDoorSkinsCountAsync(request);
            }
        }

        public async Task<int> UpdateInteriorDoorSkinAsync(InteriorDoorSkinModel model)
        {
            string id = model.InteriorDoorSkinID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var interiorDoorSkin = !string.IsNullOrWhiteSpace(id) ? await dataService.GetInteriorDoorSkinAsync(model.InteriorDoorSkinID) : new InteriorDoorSkin();
                interiorDoorSkin = interiorDoorSkin ?? new InteriorDoorSkin();

                if (interiorDoorSkin != null)
                {
                    UpdateInteriorDoorSkinFromModel(interiorDoorSkin, model);
                    await dataService.UpdateInteriorDoorSkinAsync(interiorDoorSkin);
                    model.Merge(await GetInteriorDoorSkinAsync(dataService, interiorDoorSkin.InteriorDoorSkinID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteInteriorDoorSkinAsync(InteriorDoorSkinModel model)
        {
            var interiorDoorSkin = new InteriorDoorSkin { InteriorDoorSkinID = model.InteriorDoorSkinID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteInteriorDoorSkinsAsync(interiorDoorSkin);
            }
        }

        public async Task<int> DeleteInteriorDoorSkinRangeAsync(int index, int length, DataRequest<InteriorDoorSkin> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetInteriorDoorSkinKeysAsync(index, length, request);
                return await dataService.DeleteInteriorDoorSkinsAsync(items.ToArray());
            }
        }

        static public async Task<InteriorDoorSkinModel> CreateInteriorDoorSkinModelAsync(InteriorDoorSkin source)
        {
            await Task.Delay(50);
            var model = new InteriorDoorSkinModel()
            {
                InteriorDoorSkinID = source.InteriorDoorSkinID,
                StockUnits = source.StockUnits,
                SafetyStockLevel = source.SafetyStockLevel,
                Description = source.Description
            };
            
            return model;
        }

        private void UpdateInteriorDoorSkinFromModel(InteriorDoorSkin target, InteriorDoorSkinModel source)
        {
            target.InteriorDoorSkinID = source.InteriorDoorSkinID;
            target.StockUnits = source.StockUnits;
            target.SafetyStockLevel = source.SafetyStockLevel;
            target.Description = source.Description;
        }
    }
}
