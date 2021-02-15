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
using System.IO;
using OfficeOpenXml;
using Windows.Storage;

namespace SimoniDoorsInventory.Services
{
    public class OrderService : IOrderService
    {
        public OrderService(IDataServiceFactory dataServiceFactory, 
                            ILogService logService, 
                            IFilePickerService filePickerService, 
                            IDialogService dialogService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
            FilePickerService = filePickerService;
            DialogService = dialogService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }
        public IFilePickerService FilePickerService { get; }
        public IDialogService DialogService { get; }

        public async Task<OrderModel> GetOrderAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetOrderAsync(dataService, id);
            }
        }
        static private async Task<OrderModel> GetOrderAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetOrderAsync(id);
            if (item != null)
            {
                return await CreateOrderModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(DataRequest<Order> request)
        {
            var collection = new OrderCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var models = new List<OrderModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrdersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateOrderModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetOrdersCountAsync(request);
            }
        }

        public async Task<OrderModel> CreateNewOrderAsync(long customerID)
        {
            var model = new OrderModel
            {
                CustomerID = customerID,
                OrderDate = DateTime.UtcNow,
                Status = 0,
                TotalCost = 0.0m
            };
            if (customerID > 0)
            {
                using (var dataService = DataServiceFactory.CreateDataService())
                {
                    var parent = await dataService.GetCustomerAsync(customerID);
                    if (parent != null)
                    {
                        model.CustomerID = customerID;
                        model.AddressLine = parent.AddressLine;
                        model.City = parent.City;
                        model.PostalCode = parent.PostalCode;
                        model.Floor = parent.Floor;
                        model.Customer = await CustomerService.CreateCustomerModelAsync(parent, includeAllFields: true);
                    }
                }
            }
            return model;
        }

        public async Task<int> UpdateOrderAsync(OrderModel model)
        {
            long id = model.OrderID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var order = id > 0 ? await dataService.GetOrderAsync(model.OrderID) : new Order();
                if (order != null)
                {
                    UpdateOrderFromModel(order, model);
                    await dataService.UpdateOrderAsync(order);
                    model.Merge(await GetOrderAsync(dataService, order.OrderID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteOrderAsync(OrderModel model)
        {
            var order = new Order { OrderID = model.OrderID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetOrderKeysAsync(index, length, request);
                return await dataService.DeleteOrdersAsync(items.ToArray());
            }
        }

        static public async Task<OrderModel> CreateOrderModelAsync(Order source, bool includeAllFields)
        {
            var model = new OrderModel()
            {
                OrderID = source.OrderID,
                CustomerID = source.CustomerID,
                OrderName = source.OrderName,
                CrewId = source.CrewID,
                AddressLine = source.AddressLine,
                City = source.City,
                PostalCode = source.PostalCode,
                Floor = source.Floor,
                OrderDate = source.OrderDate,
                DeliveryDateTime = source.DeliveryDateTime,
                Status = source.Status,
                TotalCost = (decimal)source.TotalCost,
                Observations = source.Observations,
            };
            if (source.Customer != null)
            {
                model.Customer = await CustomerService.CreateCustomerModelAsync(source.Customer, includeAllFields);
            }
            if (source.Crew != null)
            {
                model.Crew = new CrewModel()
                {
                    CrewID = source.Crew.CrewID,
                    Name = source.Crew.Name,
                    Phone = source.Crew.Phone,
                    Observations = source.Crew.Observations
                };
            }
            return model;
        }

        private void UpdateOrderFromModel(Order target, OrderModel source)
        {
            target.CustomerID = source.CustomerID;
            target.OrderName = source.OrderName;
            target.CrewID = source.CrewId;
            target.AddressLine = source.AddressLine;
            target.City = source.City;
            target.PostalCode = source.PostalCode;
            target.Floor = source.Floor;
            target.OrderDate = source.OrderDate;
            target.DeliveryDateTime = source.DeliveryDateTime;
            target.Status = source.Status;
            target.TotalCost = source.TotalCost;
            target.Observations = source.Observations;
        }

        public async Task SaveOrderDetailsWithItemsToExcelFileAsync(OrderModel orderModel, IList<InteriorDoorModel> orderItems)
        {
            // await NavigationService.CreateNewViewAsync<OrderPrintDetailsWithItemsViewModel>(ViewModel.OrderDetails.CreateArgs());

            Stream newFileStream =  await FilePickerService.GetExcelFileStreamAsync($"{orderModel.OrderID}_ΠΑΡΑΓΓΕΛΙΑ_ΛΕΠΤΟΜΕΡΕΙΕΣ.xlsx");

            StorageFile templateStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/ExcelTemplates/OrderDetailsTemplate.xlsx"));
            Stream templateFileStream = await templateStorageFile.OpenStreamForReadAsync();

            if (newFileStream != Stream.Null && templateFileStream != Stream.Null)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(newFileStream, templateFileStream))
                {
                    //Open the first worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells["C1"].Value = orderModel.OrderID.ToString();
                    worksheet.Cells["A6"].Value = orderModel.FullName;
                    worksheet.Cells["A8"].Value = orderModel.FullAddress;
                    worksheet.Cells["A10"].Value = $"{orderModel.Customer?.Phone1} - {orderModel.Customer?.Phone2}";

                    int cellIndex = 14;
                    worksheet.InsertRow(cellIndex, orderItems.Count);

                    for (int i = 0; i < orderItems.Count; i++)
                    {
                        worksheet.Cells[$"A{cellIndex + i}"].Value = orderItems[i].InteriorDoorID;
                        worksheet.Cells[$"B{cellIndex + i}"].Value = orderItems[i].OpeningTypeDesc;
                        worksheet.Cells[$"C{cellIndex + i}"].Value = orderItems[i].InteriorDoorSkinID;
                        worksheet.Cells[$"D{cellIndex + i}"].Value = orderItems[i].InteriorDoorDesignID;
                        worksheet.Cells[$"E{cellIndex + i}"].Value = orderItems[i].AccessoryDesc;
                        worksheet.Cells[$"F{cellIndex + i}"].Value = orderItems[i].OpeningSideDesc;
                        worksheet.Cells[$"G{cellIndex + i}"].Value = orderItems[i].ManufacturingWidth;
                        worksheet.Cells[$"H{cellIndex + i}"].Value = orderItems[i].ManufacturingHeight;
                        worksheet.Cells[$"I{cellIndex + i}"].Value = orderItems[i].Lamb;
                        worksheet.Cells[$"J{cellIndex + i}"].Value = orderItems[i].Observations;
                    }

                    //Switch the PageLayoutView back to normal
                    worksheet.View.PageLayoutView = false;
                    // save our new workbook and we are done!
                    try
                    {
                        await package.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        await DialogService.ShowAsync("Σφάλμα", "Το έγγραφο πρέπει να είναι κενό ή να μην υπάρχει");
                        await LogService.WriteAsync(LogType.Error, "CustomerService", "SaveCustomerDetailsToExcelFileAsync", ex);
                    }
                }

                newFileStream.Close();
                templateFileStream.Close();
            }
        }

        public async Task SaveOrderListToExcelFileAsync(IList<OrderModel> orderList)
        {
            Stream newFileStream = await FilePickerService.GetExcelFileStreamAsync($"{DateTime.Now}_ΠΑΡΑΓΓΕΛΙΑ_ΛΙΣΤΑ.xlsx");

            StorageFile templateStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/ExcelTemplates/OrderListTemplate.xlsx"));
            Stream templateFileStream = await templateStorageFile.OpenStreamForReadAsync();

            if (newFileStream != Stream.Null && templateFileStream != Stream.Null)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                try
                {
                    using (ExcelPackage package = new ExcelPackage(newFileStream, templateFileStream))
                    {
                        //Open the first worksheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        int cellIndex = 3;
                        int orderCount = orderList.Count;
                        worksheet.InsertRow(cellIndex, orderCount);

                        for (int i = 0; i < orderCount; i++)
                        {
                            worksheet.Cells[$"A{cellIndex + i}"].Value = orderList[i].OrderID;
                            worksheet.Cells[$"B{cellIndex + i}"].Value = orderList[i].OrderName;
                            worksheet.Cells[$"C{cellIndex + i}"].Value = orderList[i].Customer.FullName;
                            worksheet.Cells[$"D{cellIndex + i}"].Value = orderList[i].Crew.CrewDesc;
                            worksheet.Cells[$"E{cellIndex + i}"].Value = orderList[i].AddressLine;
                            worksheet.Cells[$"F{cellIndex + i}"].Value = orderList[i].City;
                            worksheet.Cells[$"G{cellIndex + i}"].Value = orderList[i].PostalCode;
                            worksheet.Cells[$"H{cellIndex + i}"].Value = orderList[i].Floor;
                            worksheet.Cells[$"I{cellIndex + i}"].Value = orderList[i].OrderDate;
                            worksheet.Cells[$"J{cellIndex + i}"].Value = orderList[i].DeliveryDateTime;
                            worksheet.Cells[$"K{cellIndex + i}"].Value = orderList[i].StatusDesc;
                            worksheet.Cells[$"L{cellIndex + i}"].Value = orderList[i].TotalCost;
                            worksheet.Cells[$"M{cellIndex + i}"].Value = orderList[i].Observations;
                        }

                        //Switch the PageLayoutView back to normal
                        worksheet.View.PageLayoutView = false;
                        // save our new workbook and we are done!

                        await package.SaveAsync();
                    }
                }
                catch (Exception ex)
                {
                    await DialogService.ShowAsync("Σφάλμα", "Το έγγραφο πρέπει να είναι κενό ή να μην υπάρχει");
                    await LogService.WriteAsync(LogType.Error, "OrderService", "SaveOrderListToExcelFileAsync", ex);
                }

                newFileStream.Close();
                templateFileStream.Close();
            }
        }

    }
}
