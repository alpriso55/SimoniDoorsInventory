#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Data.Services;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class PaymentService : IPaymentService
    {
        public PaymentService(IDataServiceFactory dataServiceFactory, ILogService logService)
        {
            DataServiceFactory = dataServiceFactory;
            LogService = logService;
        }

        public IDataServiceFactory DataServiceFactory { get; }
        public ILogService LogService { get; }

        public async Task<PaymentModel> GetPaymentAsync(long id)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await GetPaymentAsync(dataService, id);
            }
        }
        static private async Task<PaymentModel> GetPaymentAsync(IDataService dataService, long id)
        {
            var item = await dataService.GetPaymentAsync(id);
            if (item != null)
            {
                return await CreatePaymentModelAsync(item, includeAllFields: true);
            }
            return null;
        }

        public async Task<IList<PaymentModel>> GetPaymentsAsync(DataRequest<Payment> request)
        {
            var collection = new PaymentCollection(this, LogService);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<PaymentModel>> GetPaymentsAsync(int skip, int take, DataRequest<Payment> request)
        {
            var models = new List<PaymentModel>();
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetPaymentsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(await CreatePaymentModelAsync(item, includeAllFields: true));
                }
                return models;
            }
        }

        public async Task<int> GetPaymentsCountAsync(DataRequest<Payment> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.GetPaymentsCountAsync(request);
            }
        }

        public async Task<int> UpdatePaymentAsync(PaymentModel model)
        {
            long id = model.PaymentID;
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var payment = id > 0 ? await dataService.GetPaymentAsync(model.PaymentID) : new Payment();
                if (payment != null)
                {
                    UpdatePaymentFromModel(payment, model);
                    await dataService.UpdatePaymentAsync(payment);
                    model.Merge(await GetPaymentAsync(dataService, payment.PaymentID));
                }
                return 0;
            }
        }

        public async Task<int> DeletePaymentAsync(PaymentModel model)
        {
            var payment = new Payment { PaymentID = model.PaymentID };
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                return await dataService.DeletePaymentsAsync(payment);
            }
        }

        public async Task<int> DeletePaymentRangeAsync(int index, int length, DataRequest<Payment> request)
        {
            using (var dataService = DataServiceFactory.CreateDataService())
            {
                var items = await dataService.GetPaymentKeysAsync(index, length, request);
                return await dataService.DeletePaymentsAsync(items.ToArray());
            }
        }

        static public async Task<PaymentModel> CreatePaymentModelAsync(Payment source, bool includeAllFields)
        {
            var model = new PaymentModel()
            {
                PaymentID = source.PaymentID,
                CustomerID = source.CustomerID,
                Amount = source.Amount,
                PaymentTypeID = source.PaymentTypeID,
                PaymentDate = source.PaymentDate,
                Observations = source.Observations
            };

            if (includeAllFields)
            {
                model.Customer = await CustomerService.CreateCustomerModelAsync(source.Customer, includeAllFields);
                model.PaymentType = new PaymentTypeModel()
                {
                    PaymentTypeID = source.PaymentType.PaymentTypeID,
                    Name = source.PaymentType.Name
                };
                // TODO: implement the same thing for model.Account
            }
            return model;
        }

        private void UpdatePaymentFromModel(Payment target, PaymentModel source)
        {
            target.PaymentID = source.PaymentID;
            target.CustomerID = source.CustomerID;
            target.Amount = source.Amount;
            target.PaymentTypeID = source.PaymentTypeID;
            target.PaymentDate = source.PaymentDate;
            target.Observations = source.Observations;
        }
    }
}
