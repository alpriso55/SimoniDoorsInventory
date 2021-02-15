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
    public class InteriorDoorSkinService : IInteriorDoorSkinService
    {
        public InteriorDoorSkinService(IDataServiceFactory dataServiceFactory, 
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

        public async Task SaveInteriorDoorSkinListToExcelFileAsync(IList<InteriorDoorSkinModel> interiorDoorSkinList)
        {
            Stream newFileStream = await FilePickerService.GetExcelFileStreamAsync($"{DateTime.Now}_ΕΠΕΝΔΫΣΕΙΣ_ΕΣΩΤΕΡΙΚΩΝ_ΠΟΡΤΩΝ_ΛΙΣΤΑ.xlsx");

            FileInfo templateFileInfo = new FileInfo("Assets/ExcelTemplates/ΙnteriorDoorSkinListTemplate.xlsx");
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
                        int interiorDoorSkinCount = interiorDoorSkinList.Count;
                        worksheet.InsertRow(cellIndex, interiorDoorSkinCount);

                        for (int i = 0; i < interiorDoorSkinCount; i++)
                        {
                            worksheet.Cells[$"A{cellIndex + i}"].Value = interiorDoorSkinList[i].InteriorDoorSkinID;
                            worksheet.Cells[$"B{cellIndex + i}"].Value = interiorDoorSkinList[i].StockUnits;
                            worksheet.Cells[$"C{cellIndex + i}"].Value = interiorDoorSkinList[i].SafetyStockLevel;
                            worksheet.Cells[$"D{cellIndex + i}"].Value = interiorDoorSkinList[i].Description;
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
                    await LogService.WriteAsync(LogType.Error, "InteriorDoorSkinService", "SaveInteriorDoorSkinListToExcelFileAsync", ex);
                }

                newFileStream.Close();
                templateFileStream.Close();
            }
        }

    }
}
