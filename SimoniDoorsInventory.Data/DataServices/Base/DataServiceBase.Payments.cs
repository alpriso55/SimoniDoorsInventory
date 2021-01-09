using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    partial class DataServiceBase
    {
        private IQueryable<Payment> GetPayments(DataRequest<Payment> request)
        {
            IQueryable<Payment> items = _dataSource.Payments;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.ToLowerInvariant().Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }

        public async Task<Payment> GetPaymentAsync(long id)
        {
            return await _dataSource.Payments.Where(r => r.PaymentID == id)
                                             .Include(r => r.Account)
                                             .Include(r => r.Order)
                                             .Include(r => r.PaymentType)
                                             .FirstOrDefaultAsync();
        }

        public async Task<IList<Payment>> GetPaymentsAsync(int skip, int take, DataRequest<Payment> request)
        {
            IQueryable<Payment> items = GetPayments(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<IList<Payment>> GetPaymentKeysAsync(int skip, int take, DataRequest<Payment> request)
        {
            IQueryable<Payment> items = GetPayments(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new Payment
                                     {
                                         PaymentID = r.PaymentID,
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<int> GetPaymentsCountAsync(DataRequest<Payment> request)
        {
            IQueryable<Payment> items = _dataSource.Payments;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.ToLowerInvariant().Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdatePaymentAsync(Payment payment)
        {
            if (payment.PaymentID > 0)
            {
                _dataSource.Entry(payment).State = EntityState.Modified;
            }
            else
            {
                payment.PaymentID = UIDGenerator.Next(6);
                payment.PaymentDate = DateTime.UtcNow;
                _dataSource.Entry(payment).State = EntityState.Added;
            }

            payment.SearchTerms = payment.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeletePaymentsAsync(params Payment[] payments)
        {
            _dataSource.Payments.RemoveRange(payments);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
