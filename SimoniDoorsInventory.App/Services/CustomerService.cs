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
using Windows.Storage;
using OfficeOpenXml;

namespace SimoniDoorsInventory.Services
{
    public class CustomerService : ICustomerService
    {
        public CustomerService(IDataServiceFactory dataServiceFactory, 
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

        public async Task<CustomerModel> GetCustomerAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetCustomerAsync(dataService, id);
            }
        }
        static private async Task<CustomerModel> GetCustomerAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetCustomerAsync(id);
            if (item != null)
            {
                return await CreateCustomerModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(DataRequest<Customer> request)
        {
            var collection = new CustomerCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var models = new List<CustomerModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCustomersAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreateCustomerModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetCustomersCountAsync(request);
            }
        }

        public async Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            long id = model.CustomerID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var customer = id > 0 ? await dataService.GetCustomerAsync(model.CustomerID) : new Customer();
                if (customer != null)
                {
                    UpdateCustomerFromModel(customer, model);
                    await dataService.UpdateCustomerAsync(customer);
                    model.Merge(await GetCustomerAsync(dataService, customer.CustomerID));
                }
                return 0;
            }
        }

        public async Task<int> DeleteCustomerAsync(CustomerModel model)
        {
            var customer = new Customer { CustomerID = model.CustomerID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeleteCustomersAsync(customer);
            }
        }

        public async Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetCustomerKeysAsync(index, length, request);
                return await dataService.DeleteCustomersAsync(items.ToArray());
            }
        }

        static public async Task<CustomerModel> CreateCustomerModelAsync(Customer source, bool includeAllFields)
        {
            var model = new CustomerModel()
            {
                CustomerID = source.CustomerID,
                Balance = source.Balance,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Phone1 = source.Phone1,
                Phone2 = source.Phone2,
                Email = source.Email,
                AddressLine = source.AddressLine,
                City = source.City,
                Floor = source.Floor,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };
            if (includeAllFields)
            {
                model.PostalCode = source.PostalCode;
                model.Observations = source.Observations;
                model.Picture = source.Picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        private void UpdateCustomerFromModel(Customer target, CustomerModel source)
        {
            target.Balance = source.Balance;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Phone1 = source.Phone1;
            target.Phone2 = source.Phone2;
            target.Email = source.Email;
            target.AddressLine = source.AddressLine;
            target.City = source.City;
            target.PostalCode = source.PostalCode;
            target.Floor = source.Floor;

            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Observations = source.Observations;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }

        public async Task SaveCustomerDetailsToExcelFileAsync(CustomerModel customerModel, IList<OrderModel> customerOrders, IList<PaymentModel> customerPayments)
        {
            Stream newFileStream = await FilePickerService.GetExcelFileStreamAsync($"{customerModel.CustomerID}_ΠΕΛΑΤΗΣ_ΛΕΠΤΟΜΕΡΕΙΕΣ.xlsx");

            FileInfo templateFileInfo = new FileInfo("Assets/ExcelTemplates/CutomerDetailsTemplate.xlsx");
            StorageFile templateStorageFile = await StorageFile.GetFileFromPathAsync(templateFileInfo.FullName);
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

                        worksheet.Cells["C1"].Value = customerModel.CustomerID.ToString();
                        worksheet.Cells["A3"].Value = customerModel.FullName;
                        worksheet.Cells["A5"].Value = customerModel.FullAddress;
                        worksheet.Cells["D5"].Value = customerModel.Email;
                        worksheet.Cells["A7"].Value = customerModel.Phone1;
                        worksheet.Cells["D7"].Value = customerModel?.Phone2 ?? "";
                        worksheet.Cells["A9"].Value = customerModel.Observations;
                        worksheet.Cells["D9"].Value = customerModel.Balance;

                        int cellIndex = 12;
                        worksheet.InsertRow(cellIndex, customerOrders.Count);

                        for (int i = 0; i < customerOrders.Count; i++)
                        {
                            worksheet.Cells[$"A{cellIndex + i}"].Value = customerOrders[i].OrderID;
                            worksheet.Cells[$"B{cellIndex + i}"].Value = customerOrders[i].OrderName;
                            worksheet.Cells[$"C{cellIndex + i}"].Value = customerOrders[i].FullAddress;
                            worksheet.Cells[$"D{cellIndex + i}"].Value = customerOrders[i].OrderDate;
                            worksheet.Cells[$"E{cellIndex + i}"].Value = customerOrders[i].StatusDesc;
                            worksheet.Cells[$"F{cellIndex + i}"].Value = customerOrders[i].Crew.CrewDesc;
                            worksheet.Cells[$"G{cellIndex + i}"].Value = customerOrders[i].TotalCost;
                            worksheet.Cells[$"H{cellIndex + i}"].Value = customerOrders[i].DeliveryDateTime;
                            worksheet.Cells[$"I{cellIndex + i}"].Value = customerOrders[i].Observations;
                        }

                        cellIndex += customerOrders.Count;
                        cellIndex += 6;
                        worksheet.InsertRow(cellIndex, customerOrders.Count);

                        for (int i = 0; i < customerPayments.Count; i++)
                        {
                            worksheet.Cells[$"A{cellIndex + i}"].Value = customerPayments[i].PaymentID;
                            worksheet.Cells[$"B{cellIndex + i}"].Value = customerPayments[i].PaymentDate;
                            worksheet.Cells[$"C{cellIndex + i}"].Value = customerPayments[i].PaymentType.Name;
                            worksheet.Cells[$"D{cellIndex + i}"].Value = customerPayments[i].Amount;
                            worksheet.Cells[$"I{cellIndex + i}"].Value = customerPayments[i].Observations;
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
                    await LogService.WriteAsync(LogType.Error, "CustomerService", "SaveCustomerDetailsToExcelFileAsync", ex);
                }

                newFileStream.Close();
                templateFileStream.Close();
            }
        }

        public async Task SaveCustomerListToExcelFileAsync(IList<CustomerModel> customerList)
        {
            Stream newFileStream = await FilePickerService.GetExcelFileStreamAsync($"{DateTime.Now}_ΠΕΛΑΤΕΣ_ΛΙΣΤΑ.xlsx");

            FileInfo templateFileInfo = new FileInfo("Assets/ExcelTemplates/CutomerListTemplate.xlsx");
            StorageFile templateStorageFile = await StorageFile.GetFileFromPathAsync(templateFileInfo.FullName);
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
                        int customerCount = customerList.Count;
                        worksheet.InsertRow(cellIndex, customerCount);

                        for (int i = 0; i < customerCount; i++)
                        {
                            worksheet.Cells[$"A{cellIndex + i}"].Value = customerList[i].CustomerID;
                            worksheet.Cells[$"B{cellIndex + i}"].Value = customerList[i].Balance;
                            worksheet.Cells[$"C{cellIndex + i}"].Value = customerList[i].FirstName;
                            worksheet.Cells[$"D{cellIndex + i}"].Value = customerList[i].LastName;
                            worksheet.Cells[$"E{cellIndex + i}"].Value = customerList[i].Phone1;
                            worksheet.Cells[$"F{cellIndex + i}"].Value = customerList[i].Phone2;
                            worksheet.Cells[$"G{cellIndex + i}"].Value = customerList[i].Email;
                            worksheet.Cells[$"H{cellIndex + i}"].Value = customerList[i].AddressLine;
                            worksheet.Cells[$"I{cellIndex + i}"].Value = customerList[i].City;
                            worksheet.Cells[$"J{cellIndex + i}"].Value = customerList[i].PostalCode;
                            worksheet.Cells[$"K{cellIndex + i}"].Value = customerList[i].Floor;
                            worksheet.Cells[$"L{cellIndex + i}"].Value = customerList[i].CreatedOn;
                            worksheet.Cells[$"M{cellIndex + i}"].Value = customerList[i].Observations;
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
                    await LogService.WriteAsync(LogType.Error, "CustomerService", "SaveCustomerListToExcelFileAsync", ex);
                }

                newFileStream.Close();
                templateFileStream.Close();
            }
        }

    }
}
