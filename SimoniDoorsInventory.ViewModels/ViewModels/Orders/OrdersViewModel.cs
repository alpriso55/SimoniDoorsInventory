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
            InteriorDoorList = new InteriorDoorListViewModel(interiorDoorService, commonServices);
        }

        public IOrderService OrderService { get; }

        public OrderListViewModel OrderList { get; set; }
        public OrderDetailsViewModel OrderDetails { get; set; }
        public InteriorDoorListViewModel InteriorDoorList { get; set; }

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
            MessageService.Subscribe<OrderListViewModel>(this, OnMessage);
            OrderList.Subscribe();
            OrderDetails.Subscribe();
            InteriorDoorList.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            OrderList.Unsubscribe();
            OrderDetails.Unsubscribe();
            InteriorDoorList.Unsubscribe();
        }

        private async void OnMessage(OrderListViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (OrderDetails.IsEditMode)
            {
                StatusReady();
                OrderDetails.CancelEdit();
            }
            InteriorDoorList.IsMultipleSelection = false;
            var selected = OrderList.SelectedItem;
            if (!OrderList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateInteriorDoors(selected);
                }
            }
            OrderDetails.Item = selected;
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
                    await InteriorDoorList.LoadAsync(new InteriorDoorListArgs { OrderID = selectedItem.OrderID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Orders", "Load OrderItems", ex);
            }
        }
    }
}
