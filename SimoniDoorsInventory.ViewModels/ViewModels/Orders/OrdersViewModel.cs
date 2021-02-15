using System;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        public OrdersViewModel(IOrderService orderService, 
                               IInteriorDoorService interiorDoorService, 
                               ICommonServices commonServices) : base(commonServices)
        {
            OrderService = orderService;

            OrderList = new OrderListViewModel(OrderService, commonServices);
            OrderDetails = new OrderDetailsViewModel(OrderService, commonServices);
            OrderInteriorDoors = new InteriorDoorListViewModel(interiorDoorService, commonServices);
        }

        public IOrderService OrderService { get; }

        public OrderListViewModel OrderList { get; set; }
        public OrderDetailsViewModel OrderDetails { get; set; }
        public InteriorDoorListViewModel OrderInteriorDoors { get; set; }

        public async Task LoadAsync(OrderListArgs args)
        {
            await OrderList.LoadAsync(args);
        }
        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderListViewModel>(this, OnOrderListMessage);
            MessageService.Subscribe<OrderDetailsViewModel>(this, OnOrderDetailsMessage);
            MessageService.Subscribe<InteriorDoorListViewModel>(this, OnInteriorDoorMessage);
            MessageService.Subscribe<InteriorDoorDetailsViewModel>(this, OnInteriorDoorMessage);

            OrderList.Subscribe();
            OrderDetails.Subscribe();
            OrderInteriorDoors.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            OrderList.Unsubscribe();
            OrderDetails.Unsubscribe();
            OrderInteriorDoors.Unsubscribe();
        }

        private async void OnOrderListMessage(OrderListViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnOrderDetailsMessage(OrderDetailsViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderDetails && message == "PrintButtonPressed")
            {
                await OrderService.SaveOrderDetailsWithItemsToExcelFileAsync(OrderDetails.Item, OrderInteriorDoors.Items);
            }
        }

        private async void OnInteriorDoorMessage(ViewModelBase sender, string message, object args)
        {
            if (sender == OrderInteriorDoors)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                    case "ItemRangesDeleted":
                        await ContextService.RunAsync( () =>
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
            var selected = new OrderModel();
            selected.Merge(OrderList.SelectedItem);
            if (!OrderList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateInteriorDoors(selected);
                }
            }
            selected.TotalCost = await OrderInteriorDoors.GetSumOfPrices();
            OrderList.SelectedItem = selected;
            OrderDetails.Item = selected;
            await OrderService.UpdateOrderAsync(selected);
        }

        private async void OnItemSelected()
        {
            if (OrderDetails.IsEditMode)
            {
                StatusReady();
                OrderDetails.CancelEdit();
            }
            OrderInteriorDoors.IsMultipleSelection = false;
            var selected = OrderList.SelectedItem;
            if (!OrderList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateInteriorDoors(selected);
                }
            }
            // selected.TotalCost = await OrderInteriorDoors.GetSumOfPrices();
            OrderDetails.Item = selected;
            // OrderDetails.SaveCommand.Execute(null);  // NEW LINE
        }

        private async Task PopulateDetails(OrderModel selected)
        {
            try
            {
                var model = await OrderService.GetOrderAsync(selected.OrderID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Orders", "Load Details", ex);
            }
        }

        private async Task PopulateInteriorDoors(OrderModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await OrderInteriorDoors.LoadAsync(new InteriorDoorListArgs { OrderID = selectedItem.OrderID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Orders", "Load Interior Doors", ex);
            }
        }
    }
}
