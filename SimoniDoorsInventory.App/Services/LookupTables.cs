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
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class LookupTables : ILookupTables
    {
        public LookupTables(ILogService logService, IDataServiceFactory dataServiceFactory)
        {
            LogService = logService;
            DataServiceFactory = dataServiceFactory;
        }

        public ILogService LogService { get; }
        public IDataServiceFactory DataServiceFactory { get; }

        public IList<AccessoryModel> Accessories { get; private set; }
        public IList<CategoryModel> Categories { get; private set; }
        public IList<CrewModel> Crews { get; private set; }
        public IList<InteriorDoorDesignModel> InteriorDoorDesigns { get; private set; }
        public IList<InteriorDoorSkinModel> InteriorDoorSkins { get; private set; }
        public IList<OpeningSideModel> OpeningSides { get; private set; }
        public IList<OpeningTypeModel> OpeningTypes { get; private set; }
        public IList<OrderStatusModel> OrderStatus { get; private set; }
        public IList<PaymentTypeModel> PaymentTypes { get; private set; }

        public async Task InitializeAsync()
        {
            Accessories = await GetAccessoriesAsync();
            Categories = await GetCategoriesAsync();
            Crews = await GetCrewsAsync();
            InteriorDoorDesigns = await GetInteriorDoorDesignsAsync();
            InteriorDoorSkins = await GetInteriorDoorSkinsAsync();
            OpeningSides = await GetOpeningSidesAsync();
            OpeningTypes = await GetOpeningTypesAsync();
            OrderStatus = await GetOrderStatusAsync();
            PaymentTypes = await GetPaymentTypesAsync();
        }

        public string GetAccessory(int id)
        {
            return Accessories.Where(r => r.AccessoryID == id).Select(r => r.Name).FirstOrDefault();
        }
        public string GetCategory(int id)
        {
            return Categories.Where(r => r.CategoryID == id).Select(r => r.Name).FirstOrDefault();
        }
        public string GetCrew(int id)
        {
            string name = Crews.Where(r => r.CrewID == id).Select(r => r.Name).FirstOrDefault();
            string phone = Crews.Where(r => r.CrewID == id).Select(r => r.Phone).FirstOrDefault();
            return $"{name} ({phone})";
        }
        public string GetInteriorDoorDesign(string id)
        {
            return InteriorDoorDesigns.Where(r => r.InteriorDoorDesignID == id)
                                      .Select(r => r.InteriorDoorDesignID).FirstOrDefault();
        }
        public string GetInteriorDoorSkin(string id)
        {
            return InteriorDoorSkins.Where(r => r.InteriorDoorSkinID == id)
                                           .Select(r => r.InteriorDoorSkinID).FirstOrDefault();
        }
        public int GetInteriorDoorSkinStockUnits(string id)
        {
            return InteriorDoorSkins.Where(r => r.InteriorDoorSkinID == id)
                                    .Select(r => r.StockUnits).FirstOrDefault();
        }
        public int GetInteriorDoorSkinSafetyStockLevel(string id)
        {
            return InteriorDoorSkins.Where(r => r.InteriorDoorSkinID == id)
                                    .Select(r => r.SafetyStockLevel).FirstOrDefault();
        }
        public string GetOpeningSide(int id)
        {
            return OpeningSides.Where(r => r.OpeningSideID == id).Select(r => r.Name).FirstOrDefault();
        }
        public string GetOpeningType(int id)
        {
            return OpeningTypes.Where(r => r.OpeningTypeID == id).Select(r => r.Name).FirstOrDefault();
        }
        public string GetOrderStatus(int id)
        {
            return OrderStatus.Where(r => r.Status == id).Select(r => r.Name).FirstOrDefault();
        }
        public string GetPaymentType(int? id)
        {
            return id == null ? "" : PaymentTypes.Where(r => r.PaymentTypeID == id).Select(r => r.Name).FirstOrDefault();
        }

        private async Task<IList<AccessoryModel>> GetAccessoriesAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetAccessoriesAsync();
                    return items.Select(r => new AccessoryModel
                    {
                        AccessoryID = r.AccessoryID,
                        Name = r.Name
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load Accessories", ex);
            }
            return new List<AccessoryModel>();
        }

        private async Task<IList<CategoryModel>> GetCategoriesAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetCategoriesAsync();
                    return items.Select(r => new CategoryModel
                    {
                        CategoryID = r.CategoryID,
                        Name = r.Name
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load Categories", ex);
            }
            return new List<CategoryModel>();
        }

        private async Task<IList<CrewModel>> GetCrewsAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetCrewsAsync();
                    return items.OrderBy(r => r.Name)
                                .Select(r => new CrewModel
                                {
                                    CrewID = r.CrewID,
                                    Name = r.Name,
                                    Phone = r.Phone
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load Crews", ex);
            }
            return new List<CrewModel>();
        }

        private async Task<IList<InteriorDoorDesignModel>> GetInteriorDoorDesignsAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetInteriorDoorDesignsAsync();
                    return items.OrderBy(r => r.InteriorDoorDesignID)
                                .Select(r => new InteriorDoorDesignModel
                                {
                                    InteriorDoorDesignID = r.InteriorDoorDesignID
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load InteriorDoorDesigns", ex);
            }
            return new List<InteriorDoorDesignModel>();
        }

        private async Task<IList<InteriorDoorSkinModel>> GetInteriorDoorSkinsAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetInteriorDoorSkinsAsync();
                    return items.OrderBy(r => r.StockUnits)
                                .Select(r => new InteriorDoorSkinModel
                                {
                                    InteriorDoorSkinID = r.InteriorDoorSkinID,
                                    StockUnits = r.StockUnits,
                                    SafetyStockLevel = r.SafetyStockLevel
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load InteriorDoorSkins", ex);
            }
            return new List<InteriorDoorSkinModel>();
        }

        private async Task<IList<OpeningSideModel>> GetOpeningSidesAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetOpeningSidesAsync();
                    return items.OrderBy(r => r.Name).Select(r => new OpeningSideModel
                    {
                        OpeningSideID = r.OpeningSideID,
                        Name = r.Name
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load OpeningSides", ex);
            }
            return new List<OpeningSideModel>();
        }

        private async Task<IList<OpeningTypeModel>> GetOpeningTypesAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetOpeningTypesAsync();
                    return items.OrderBy(r => r.Name).Select(r => new OpeningTypeModel
                    {
                        OpeningTypeID = r.OpeningTypeID,
                        Name = r.Name
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load OpeningTypes", ex);
            }
            return new List<OpeningTypeModel>();
        }

        private async Task<IList<OrderStatusModel>> GetOrderStatusAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetOrderStatusAsync();
                    return items.Select(r => new OrderStatusModel
                    {
                        Status = r.Status,
                        Name = r.Name
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load OrderStatus", ex);
            }
            return new List<OrderStatusModel>();
        }

        private async Task<IList<PaymentTypeModel>> GetPaymentTypesAsync()
        {
            try
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var items = await dataService.GetPaymentTypesAsync();
                    return items.Select(r => new PaymentTypeModel
                    {
                        PaymentTypeID = r.PaymentTypeID,
                        Name = r.Name
                    })
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("LookupTables", "Load PaymentTypes", ex);
            }
            return new List<PaymentTypeModel>();
        }

        private async void LogException(string source, string action, Exception exception)
        {
            await LogService.WriteAsync(LogType.Error, source, action, exception.Message, exception.ToString());
        }
    }
}
