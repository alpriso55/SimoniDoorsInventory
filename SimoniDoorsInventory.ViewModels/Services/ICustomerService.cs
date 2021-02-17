﻿using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface ICustomerService
    {
        Task<CustomerModel> GetCustomerAsync(long id);
        Task<IList<CustomerModel>> GetCustomersAsync(DataRequest<Customer> request);
        Task<IList<CustomerModel>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request);
        Task<int> GetCustomersCountAsync(DataRequest<Customer> request);

        Task<int> UpdateCustomerAsync(CustomerModel model);

        Task<int> DeleteCustomerAsync(CustomerModel model);
        Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request);

        Task SaveCustomerDetailsToExcelFileAsync(CustomerModel customerModel, IList<OrderModel> customerOrders, IList<PaymentModel> customerPayments);
        Task SaveCustomerListToExcelFileAsync(IList<CustomerModel> customerList);
    }
}
