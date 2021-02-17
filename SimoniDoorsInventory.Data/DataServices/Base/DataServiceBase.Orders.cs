using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    partial class DataServiceBase
    {
        private IQueryable<Order> GetOrders(DataRequest<Order> request)
        {
            IQueryable<Order> items = _dataSource.Orders;

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

        public async Task<Order> GetOrderAsync(long id)
        {
            return await _dataSource.Orders.Where(r => r.OrderID == id)
                                           .Include(r => r.Customer)
                                           .Include(r => r.Crew)
                                           .Include(r => r.OrderStatus)
                                           .FirstOrDefaultAsync();
        }

        public async Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            IQueryable<Order> items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Include(r => r.Customer)
                                     .Include(r => r.Crew)
                                     .Include(r => r.OrderStatus)
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request)
        {
            IQueryable<Order> items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new Order
                                     {
                                         OrderID = r.OrderID,
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            IQueryable<Order> items = _dataSource.Orders;

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

        public async Task<int> UpdateOrderAsync(Order order)
        {
            if (order.OrderID > 0)
            {
                _dataSource.Entry(order).State = EntityState.Modified;
            }
            else
            {
                order.OrderID = UIDGenerator.Next(4);
                order.OrderDate = DateTime.UtcNow;
                order.TotalCost = 0.0m;
                _dataSource.Entry(order).State = EntityState.Added;
            }

            order.LastModifiedOn = DateTime.UtcNow;
            order.SearchTerms = order.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteOrdersAsync(params Order[] orders)
        {
            _dataSource.Orders.RemoveRange(orders);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
