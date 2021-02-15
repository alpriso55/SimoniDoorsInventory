using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface IOrderService
    {
        Task<OrderModel> GetOrderAsync(long id);
        Task<IList<OrderModel>> GetOrdersAsync(DataRequest<Order> request);
        Task<IList<OrderModel>> GetOrdersAsync(int skip, int take, DataRequest<Order> request);
        Task<int> GetOrdersCountAsync(DataRequest<Order> request);

        Task<OrderModel> CreateNewOrderAsync(long customerID);
        Task<int> UpdateOrderAsync(OrderModel model);

        Task<int> DeleteOrderAsync(OrderModel model);
        Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request);

        Task SaveOrderDetailsWithItemsToExcelFileAsync(OrderModel orderModel, IList<InteriorDoorModel> orderItems);
        Task SaveOrderListToExcelFileAsync(IList<OrderModel> orderList);
    }
}
