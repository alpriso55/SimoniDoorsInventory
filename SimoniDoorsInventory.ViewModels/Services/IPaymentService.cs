using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public interface IPaymentService
    {
        Task<PaymentModel> GetPaymentAsync(long id);
        Task<IList<PaymentModel>> GetPaymentsAsync(DataRequest<Payment> request);
        Task<IList<PaymentModel>> GetPaymentsAsync(int skip, int take, DataRequest<Payment> request);
        Task<int> GetPaymentsCountAsync(DataRequest<Payment> request);

        Task<PaymentModel> CreateNewPaymentAsync(long customerID);
        Task<int> UpdatePaymentAsync(PaymentModel model);

        Task<int> DeletePaymentAsync(PaymentModel model);
        Task<int> DeletePaymentRangeAsync(int index, int length, DataRequest<Payment> request);
    }
}
