using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    partial class DataServiceBase
    {
        private IQueryable<InteriorDoor> GetInteriorDoors(DataRequest<InteriorDoor> request)
        {
            IQueryable<InteriorDoor> items = _dataSource.InteriorDoors;

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

        public async Task<InteriorDoor> GetInteriorDoorAsync(long orderID, int interiorDoorID)
        {
            return await _dataSource.InteriorDoors.Where(r => r.OrderID == orderID && r.InteriorDoorID == interiorDoorID)
                                                  .Include(r => r.InteriorDoorSkin)
                                                  .Include(r => r.InteriorDoorDesign)
                                                  .FirstOrDefaultAsync();
        }

        public async Task<IList<InteriorDoor>> GetInteriorDoorsAsync(int skip, int take, DataRequest<InteriorDoor> request)
        {
            IQueryable<InteriorDoor> items = GetInteriorDoors(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Include(r => r.InteriorDoorSkin)
                                     .Include(r => r.InteriorDoorDesign)
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<IList<InteriorDoor>> GetInteriorDoorKeysAsync(int skip, int take, DataRequest<InteriorDoor> request)
        {
            IQueryable<InteriorDoor> items = GetInteriorDoors(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new InteriorDoor
                                     {
                                         OrderID = r.OrderID,
                                         InteriorDoorID = r.InteriorDoorID
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<int> GetInteriorDoorsCountAsync(DataRequest<InteriorDoor> request)
        {
            IQueryable<InteriorDoor> items = _dataSource.InteriorDoors;

            // Query
            // TODO: Not supported
            // if (!string.IsNullOrEmpty(request.Query))
            // {
            //     items = items.Where(r => r.SearchTerms.ToLowerInvariant().Contains(request.Query.ToLower()));
            // }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateInteriorDoorAsync(InteriorDoor interiorDoor)
        {
            if (interiorDoor.InteriorDoorID > 0)
            {
                _dataSource.Entry(interiorDoor).State = EntityState.Modified;
            }
            else
            {
                interiorDoor.InteriorDoorID = _dataSource.InteriorDoors.Where(r => r.OrderID == interiorDoor.OrderID)
                                                                       .Select(r => r.InteriorDoorID)
                                                                       .DefaultIfEmpty(0).Max() + 1;
                interiorDoor.Price = 0.0m;
                // TODO:
                // interiorDoor.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(interiorDoor).State = EntityState.Added;
            }
            // TODO:
            // interiorDoor.LastModifiedOn = DateTime.UtcNow;
            // interiorDoor.SearchTerms = interiorDoor.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteInteriorDoorsAsync(params InteriorDoor[] interiorDoors)
        {
            _dataSource.InteriorDoors.RemoveRange(interiorDoors);
            return await _dataSource.SaveChangesAsync();
        }
    }
}
