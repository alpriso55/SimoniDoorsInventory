using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class OrderDetailsWithItemsViewModel : ViewModelBase
    {
        public OrderDetailsWithItemsViewModel(IOrderService orderService, 
                                              IInteriorDoorService interiorDoorService, 
                                              IFilePickerService filePickerService,
                                              ICommonServices commonServices) : base(commonServices)
        {
            OrderDetails = new OrderDetailsViewModel(orderService, commonServices);
            InteriorDoorList = new InteriorDoorListViewModel(interiorDoorService, commonServices);

            OrderService = orderService;
        }

        public OrderDetailsViewModel OrderDetails { get; set; }
        public InteriorDoorListViewModel InteriorDoorList { get; set; }

        IOrderService OrderService { get; set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            await OrderDetails.LoadAsync(args);

            long orderID = args?.OrderID ?? 0;
            if (orderID > 0)
            {
                await InteriorDoorList.LoadAsync(new InteriorDoorListArgs { OrderID = args.OrderID });
            }
            else
            {
                await InteriorDoorList.LoadAsync(new InteriorDoorListArgs { IsEmpty = true }, silent: true);
            }
        }
        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderDetails.Unload();
            InteriorDoorList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnMessage);
            MessageService.Subscribe<InteriorDoorListViewModel>(this, OnInteriorDoorMessage);
            MessageService.Subscribe<InteriorDoorDetailsViewModel>(this, OnInteriorDoorMessage);
            OrderDetails.Subscribe();
            InteriorDoorList.Subscribe();
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            OrderDetails.Unsubscribe();
            InteriorDoorList.Unsubscribe();
        }

        private async void OnMessage(OrderDetailsViewModel viewModel, string message, OrderModel order)
        {
            if (viewModel == OrderDetails && message == "ItemChanged")
            {
                await InteriorDoorList.LoadAsync(new InteriorDoorListArgs { OrderID = order.OrderID });
            }
            
            // if (viewModel == OrderDetails && message == "NewItemSaved")
            // {
            //     await this.LoadAsync(viewModel.ViewModelArgs);
            // }
        }

        private async void OnInteriorDoorMessage(ViewModelBase sender, string message, object args)
        {
            if (sender == InteriorDoorList)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                    case "ItemRangesDeleted":
                        await ContextService.RunAsync(() =>
                        {
                            OnInteriorDoorUpdated();
                        });
                        break;

                }
            }

            if (sender is InteriorDoorDetailsViewModel s && s != null && s.OrderID == OrderDetails.Item.OrderID)
            {
                switch (message)
                {
                    case "NewItemSaved":
                    case "ItemChanged":
                    case "ItemDeleted":
                        await ContextService.RunAsync(() =>
                        {
                            OnInteriorDoorUpdated();
                        });
                        break;
                }
            }
        }

        private async void OnInteriorDoorUpdated()
        {
            if (OrderDetails.IsEditMode)
            {
                StatusReady();
                OrderDetails.CancelEdit();
            }
            InteriorDoorList.IsMultipleSelection = false;

            var item = new OrderModel();
            item.Merge(OrderDetails.Item);
            if (item != null && !item.IsEmpty)
            {
                await PopulateInteriorDoors(item);
            }
            item.TotalCost = await InteriorDoorList.GetSumOfPrices();

            OrderDetails.Item = item;
            await OrderDetails.OrderService.UpdateOrderAsync(item);
        }

        private async Task PopulateInteriorDoors(OrderModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await InteriorDoorList.LoadAsync(new InteriorDoorListArgs { OrderID = selectedItem.OrderID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Orders", "Load Interior Doors", ex);
            }
        }

        public ICommand PrintInNewViewCommand => new RelayCommand(OnPrintInNewView);
        private async void OnPrintInNewView()
        {
            // await NavigationService.CreateNewViewAsync<OrderPrintDetailsWithItemsViewModel>(new OrderDetailsArgs { OrderID = OrderDetails.Item.OrderID });

            await OrderService.SaveOrderDetailsWithItemsToExcelFileAsync(OrderDetails.Item, InteriorDoorList.Items);
        }

    }
}
