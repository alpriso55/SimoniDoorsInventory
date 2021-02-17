using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    partial class DataServiceBase
    {
        private IQueryable<Customer> GetCustomers(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

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

        public async Task<Customer> GetCustomerAsync(long id)
        {
            return await _dataSource.Customers.Where(r => r.CustomerID == id)
                                              .FirstOrDefaultAsync();
        }

        public async Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            IQueryable<Customer> items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new Customer
                                     {
                                         CustomerID = r.CustomerID,
                                         Balance = r.Balance,
                                         FirstName = r.FirstName,
                                         LastName = r.LastName,
                                         Phone1 = r.Phone1,
                                         Phone2 = r.Phone2,
                                         Email = r.Email,
                                         AddressLine = r.AddressLine,
                                         City = r.City,
                                         PostalCode = r.PostalCode,
                                         Floor = r.Floor,
                                         CreatedOn = r.CreatedOn,
                                         LastModifiedOn = r.LastModifiedOn,
                                         Thumbnail = r.Thumbnail,
                                         Observations = r.Observations
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request)
        {
            IQueryable<Customer> items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new Customer
                                     {
                                         CustomerID = r.CustomerID,
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

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

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            if (customer.CustomerID > 0)
            {
                _dataSource.Entry(customer).State = EntityState.Modified;
            }
            else
            {
                customer.CustomerID = UIDGenerator.Next();
                customer.Balance = 0.0m;
                customer.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(customer).State = EntityState.Added;
            }
            customer.LastModifiedOn = DateTime.UtcNow;
            customer.SearchTerms = customer.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();

            return res;
        }

        public async Task<int> DeleteCustomersAsync(params Customer[] customers)
        {
            _dataSource.Customers.RemoveRange(customers);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
