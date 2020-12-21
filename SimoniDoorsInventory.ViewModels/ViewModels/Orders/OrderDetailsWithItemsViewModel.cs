using System;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class OrderDetailsWithItemsViewModel : ViewModelBase
    {
        public OrderDetailsWithItemsViewModel(IOrderService orderService, 
                                              IInteriorDoorService interiorDoorService, 
                                              ICommonServices commonServices) : base(commonServices)
        {
            OrderDetails = new OrderDetailsViewModel(orderService, commonServices);
            InteriorDoorList = new InteriorDoorListViewModel(interiorDoorService, commonServices);
        }

        public OrderDetailsViewModel OrderDetails { get; set; }
        public InteriorDoorListViewModel InteriorDoorList { get; set; }

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
        }

    }
}
