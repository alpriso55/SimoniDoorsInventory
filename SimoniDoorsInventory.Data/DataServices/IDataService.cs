using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimoniDoorsInventory.Data.Services
{
    public interface IDataService : IDisposable
    {
        Task<Customer> GetCustomerAsync(long id);
        Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request);
        Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request);
        Task<int> GetCustomersCountAsync(DataRequest<Customer> request);
        Task<int> UpdateCustomerAsync(Customer customer);
        Task<int> DeleteCustomersAsync(params Customer[] customers);

        Task<Order> GetOrderAsync(long id);
        Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request);
        Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request);
        Task<int> GetOrdersCountAsync(DataRequest<Order> request);
        Task<int> UpdateOrderAsync(Order order);
        Task<int> DeleteOrdersAsync(params Order[] orders);

        // Task<ArmoredDoor> GetArmoredDoorAsync(string id);
        // Task<IList<ArmoredDoor>> GetArmoredDoorsAsync(int skip, int take, DataRequest<ArmoredDoor> request);
        // Task<IList<ArmoredDoor>> GetArmoredDoorKeysAsync(int skip, int take, DataRequest<ArmoredDoor> request);
        // Task<int> GetArmoredDoorsCountAsync(DataRequest<ArmoredDoor> request);
        // Task<int> UpdateArmoredDoorAsync(ArmoredDoor armoredDoor);
        // Task<int> DeleteArmoredDoorsAsync(params ArmoredDoor[] armoredDoors);

        Task<InteriorDoor> GetInteriorDoorAsync(long orderID, int interiorDoorID);
        Task<IList<InteriorDoor>> GetInteriorDoorsAsync(int skip, int take, DataRequest<InteriorDoor> request);
        Task<IList<InteriorDoor>> GetInteriorDoorKeysAsync(int skip, int take, DataRequest<InteriorDoor> request);
        Task<int> GetInteriorDoorsCountAsync(DataRequest<InteriorDoor> request);
        Task<int> UpdateInteriorDoorAsync(InteriorDoor interiorDoor);
        Task<int> DeleteInteriorDoorsAsync(params InteriorDoor[] interiorDoors);

        Task<Payment> GetPaymentAsync(long id);
        Task<IList<Payment>> GetPaymentsAsync(int skip, int take, DataRequest<Payment> request);
        Task<IList<Payment>> GetPaymentKeysAsync(int skip, int take, DataRequest<Payment> request);
        Task<int> GetPaymentsCountAsync(DataRequest<Payment> request);
        Task<int> UpdatePaymentAsync(Payment payment);
        Task<int> DeletePaymentsAsync(params Payment[] payments);

        // -------------------------------------------------------------------------------------
        Task<IList<Accessory>> GetAccessoriesAsync(); 
        Task<IList<Account>> GetAccountsAsync();
        Task<IList<Category>> GetCategoriesAsync();
        Task<IList<Crew>> GetCrewsAsync();
        Task<IList<InteriorDoorDesign>> GetInteriorDoorDesignsAsync();
        Task<IList<InteriorDoorSkin>> GetInteriorDoorSkinsAsync();
        Task<IList<OpeningSide>> GetOpeningSidesAsync();
        Task<IList<OpeningType>> GetOpeningTypesAsync();
        Task<IList<OrderStatus>> GetOrderStatusAsync();
        Task<IList<PaymentType>> GetPaymentTypesAsync();
    }
}
