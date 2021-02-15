using System;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Services;
using SimoniDoorsInventory.Views;
using SimoniDoorsInventory.Controls;
using Windows.Storage;
using System.IO;
using OfficeOpenXml;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using Windows.Storage.AccessCache;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrderView : Page
    {
        public OrderView()
        {
            ViewModel = ServiceLocator.Current.GetService<OrderDetailsWithItemsViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            InitializeComponent();

            // Toolbar = details.GetToolBar();
            // Toolbar.ButtonClick += OnPrintButtonClicked;
        }

        public DetailToolbar Toolbar { get; set; }

        public OrderDetailsWithItemsViewModel ViewModel { get; }
        public INavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as OrderDetailsArgs);

            if (ViewModel.OrderDetails.IsEditMode)
            {
                await Task.Delay(100);
                details.SetFocus();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        /*
        private async void OnPrintButtonClicked(object sender, ToolbarButtonClickEventArgs e)
        {
            if (e.ClickedButton == ToolbarButton.Print)
            {
                // await NavigationService.CreateNewViewAsync<OrderPrintDetailsWithItemsViewModel>(ViewModel.OrderDetails.CreateArgs());
                
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Excel File", new List<string>() { ".xlsx" });
                
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = $"{ViewModel.OrderDetails.Item.OrderID}_Order_Details.xlsx";
                StorageFile newFile = await savePicker.PickSaveFileAsync();

                /// =======================================================================================
                /// 
                // FolderPicker folderPicker = new FolderPicker();
                // folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
                // folderPicker.FileTypeFilter.Add(".docx");
                // folderPicker.FileTypeFilter.Add(".xlsx");
                // folderPicker.FileTypeFilter.Add(".pptx");
                // StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                if (newFile != null)
                {
                    var folder = await newFile.GetParentAsync();

                    // FileInfo newFile = new FileInfo(folder.Path + $"\\{ViewModel.OrderDetails.Item.OrderID}_Order_Details.xlsx");
                    Stream newFileStream = await newFile.OpenStreamForWriteAsync();
                    // FileInfo templateFile = FileInputUtil.GetFileInfo("ms-appx:///Assets/ExcelTemplates/", "OrderDetailsTemplate.xlsx");
                    // Uri uri = new Uri("ms-appx:///Assets/ExcelTemplates/OrderDetailsTemplate.xlsx");
                    // FileInfo templateFile = new FileInfo("Assets/ExcelTemplates/OrderDetailsTemplate.xlsx");
                    FileInfo templateFileInfo = new FileInfo("Assets/ExcelTemplates/OrderDetailsTemplate.xlsx");
                    StorageFile templateFile = await StorageFile.GetFileFromPathAsync(templateFileInfo.FullName);
                    Stream templateFileStream = await templateFile.OpenStreamForReadAsync();

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage(newFileStream, templateFileStream))
                    {
                        //Open the first worksheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        worksheet.Cells["C1"].Value = ViewModel.OrderDetails.Item.OrderID.ToString();
                        worksheet.Cells["A6"].Value = ViewModel.OrderDetails.Item.FullName;
                        worksheet.Cells["A8"].Value = ViewModel.OrderDetails.Item.FullAddress;
                        worksheet.Cells["A10"].Value = $"{ViewModel.OrderDetails.Item.Customer?.Phone1} - {ViewModel.OrderDetails.Item.Customer?.Phone2}";

                        worksheet.InsertRow(14, ViewModel.InteriorDoorList.ItemsCount);

                        int cellIndex;
                        for (int i = 0; i < ViewModel.InteriorDoorList.ItemsCount; i++)
                        {
                            cellIndex = 14 + i;
                            worksheet.Cells[$"A{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].InteriorDoorID;
                            worksheet.Cells[$"B{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].OpeningTypeDesc;
                            worksheet.Cells[$"C{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].InteriorDoorSkinID;
                            worksheet.Cells[$"D{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].InteriorDoorDesignID;
                            worksheet.Cells[$"E{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].AccessoryDesc;
                            worksheet.Cells[$"F{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].OpeningSideDesc;
                            worksheet.Cells[$"G{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].ManufacturingWidth;
                            worksheet.Cells[$"H{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].ManufacturingHeight;
                            worksheet.Cells[$"I{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].Lamb;
                            worksheet.Cells[$"J{cellIndex}"].Value = ViewModel.InteriorDoorList.Items[i].Observations;
                        }

                        // worksheet.Cells["E2:E6"].FormulaR1C1 = "RC[-2]*RC[-1]";

                        // var name = worksheet.Names.Add("SubTotalName", worksheet.Cells["C7:E7"]);
                        // name.Style.Font.Italic = true;
                        // name.Formula = "SUBTOTAL(9,C2:C6)";
                        // 
                        // //Format the new rows
                        // worksheet.Cells["C5:C6"].Style.Numberformat.Format = "#,##0";
                        // worksheet.Cells["D5:E6"].Style.Numberformat.Format = "#,##0.00";

                        // var chart = worksheet.Drawings.AddPieChart("PieChart", ePieChartType.Pie3D);

                        // chart.Title.Text = "Total";
                        //From row 1 colum 5 with five pixels offset
                        // chart.SetPosition(0, 0, 5, 5);
                        // chart.SetSize(600, 300);

                        // ExcelAddress valueAddress = new ExcelAddress(2, 5, 6, 5);
                        // var ser = (chart.Series.Add(valueAddress.Address, "B2:B6") as ExcelPieChartSerie);
                        // chart.DataLabel.ShowCategory = true;
                        // chart.DataLabel.ShowPercent = true;
                        // 
                        // chart.Legend.Border.LineStyle = eLineStyle.Solid;
                        // chart.Legend.Border.Fill.Style = eFillStyle.SolidFill;
                        // chart.Legend.Border.Fill.Color = Color.DarkBlue;
                        // 
                        // //Set the chart style to match the preset style for 3D pie charts.
                        // chart.StyleManager.SetChartStyle(ePresetChartStyle.Pie3dChartStyle3);

                        //Switch the PageLayoutView back to normal
                        worksheet.View.PageLayoutView = false;
                        // save our new workbook and we are done!
                        await package.SaveAsync();
                    }

                    newFileStream.Close();
                    templateFileStream.Close();
                }

            }
        }
        */

    }
}
