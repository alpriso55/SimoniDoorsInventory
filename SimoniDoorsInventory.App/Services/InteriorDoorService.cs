#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Data.Services;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class InteriorDoorService : IInteriorDoorService
    {
        public InteriorDoorService(IDataServiceFactory dataServiceFactory)
        {
            DataServiceFactory = dataServiceFactory;
        }

        public IDataServiceFactory DataServiceFactory { get; }

        public async Task<InteriorDoorModel> GetInteriorDoorAsync(long orderID, int interiorDoorID)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetInteriorDoorAsync(dataService, orderID, interiorDoorID);
            }
        }
        static private async Task<InteriorDoorModel> GetInteriorDoorAsync(IDataService dataService, long orderID, int interiorDoorID)
        {
            var item = await dataService.GetInteriorDoorAsync(orderID, interiorDoorID);
            if (item != null)
            {
                return await CreateInteriorDoorModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public Task<IList<InteriorDoorModel>> GetInteriorDoorsAsync(DataRequest<InteriorDoor> request)
        {
            // InteriorDoors are not virtualized
            return GetInteriorDoorsAsync(0, 100, request);
        }

        public async Task<IList<InteriorDoorModel>> GetInteriorDoorsAsync(int skip, int take, DataRequest<InteriorDoor> request)
        {
            var models = new List<InteriorDoorModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetInteriorDoorsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateInteriorDoorModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetInteriorDoorsCountAsync(DataRequest<InteriorDoor> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetInteriorDoorsCountAsync(request);
            }
        }

        public async Task<int> UpdateInteriorDoorAsync(InteriorDoorModel model)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var interiorDoor = model.InteriorDoorID > 0 ? await dataService.GetInteriorDoorAsync(model.OrderID, model.InteriorDoorID) : new InteriorDoor();
                if (interiorDoor != null)
                {
                    UpdateInteriorDoorFromModel(interiorDoor, model);
                    await dataService.UpdateInteriorDoorAsync(interiorDoor);
                    model.Merge(await GetInteriorDoorAsync(dataService, interiorDoor.OrderID, interiorDoor.InteriorDoorID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteInteriorDoorAsync(InteriorDoorModel model)
        {
            var orderItem = new InteriorDoor { OrderID = model.OrderID, InteriorDoorID = model.InteriorDoorID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteInteriorDoorsAsync(orderItem);
            }
        }

        public async Task<int> DeleteInteriorDoorRangeAsync(int index, int length, DataRequest<InteriorDoor> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetInteriorDoorKeysAsync(index, length, request);
                return await dataService.DeleteInteriorDoorsAsync(items.ToArray());
            }
        }

        static public async Task<InteriorDoorModel> CreateInteriorDoorModelAsync(InteriorDoor source, bool includeAllFields)
        {
            await Task.Delay(50);
            var model = new InteriorDoorModel()
            {
                OrderID = source.OrderID,
                InteriorDoorID = source.InteriorDoorID,
                InteriorDoorSkinID = source.InteriorDoorSkinID,
                InteriorDoorDesignID = source.InteriorDoorDesignID,
                OpeningSideID = source.OpeningSideID,
                OpeningTypeID = source.OpeningTypeID,
                Width = source.Width,
                Height = source.Height,
                Lamb = source.Lamb,
                AccessoryID = source.AccessoryID,
                Observations = source.Observations,
                Price = source.Price
            };
            return model;
        }

        private void UpdateInteriorDoorFromModel(InteriorDoor target, InteriorDoorModel source)
        {
            target.OrderID = source.OrderID;
            target.InteriorDoorID = source.InteriorDoorID;
            target.InteriorDoorSkinID = source.InteriorDoorSkinID;
            target.InteriorDoorDesignID = source.InteriorDoorDesignID;
            target.OpeningSideID = source.OpeningSideID;
            target.OpeningTypeID = source.OpeningTypeID;
            target.Width = source.Width;
            target.Height = source.Height;
            target.Lamb = source.Lamb;
            target.AccessoryID = source.AccessoryID;
            target.Observations = source.Observations;
            target.Price = source.Price;
        }
    }
}
