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
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Services
{
    public class PaymentCollection : VirtualCollection<PaymentModel>
    {
        private DataRequest<Payment> _dataRequest = null;

        public PaymentCollection(IPaymentService paymentService, ILogService logService) : base(logService)
        {
            PaymentService = paymentService;
        }

        public IPaymentService PaymentService { get; }

        private PaymentModel _defaultItem = PaymentModel.CreateEmpty();
        protected override PaymentModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Payment> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await PaymentService.GetPaymentsCountAsync(_dataRequest);
                Ranges[0] = await PaymentService.GetPaymentsAsync(0, RangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<PaymentModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await PaymentService.GetPaymentsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                LogException("PaymentCollection", "Fetch", ex);
            }
            return null;
        }
    }
}
