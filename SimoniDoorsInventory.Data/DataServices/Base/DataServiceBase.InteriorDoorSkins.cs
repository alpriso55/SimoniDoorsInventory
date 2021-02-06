using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    partial class DataServiceBase
    {
        private IQueryable<InteriorDoorSkin> GetInteriorDoorSkins(DataRequest<InteriorDoorSkin> request)
        {
            IQueryable<InteriorDoorSkin> items = _dataSource.InteriorDoorSkins;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.ToLower().Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order by
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

        public async Task<InteriorDoorSkin> GetInteriorDoorSkinAsync(string id)
        {
            return await _dataSource.InteriorDoorSkins.Where(r => r.InteriorDoorSkinID == id)
                                              .FirstOrDefaultAsync();
        }

        public async Task<IList<InteriorDoorSkin>> GetInteriorDoorSkinsAsync(int skip, int take, DataRequest<InteriorDoorSkin> request)
        {
            IQueryable<InteriorDoorSkin> items = GetInteriorDoorSkins(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new InteriorDoorSkin
                                     {
                                         InteriorDoorSkinID = r.InteriorDoorSkinID,
                                         StockUnits = r.StockUnits,
                                         SafetyStockLevel = r.SafetyStockLevel,
                                         Description = r.Description
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<IList<InteriorDoorSkin>> GetInteriorDoorSkinKeysAsync(int skip, int take, DataRequest<InteriorDoorSkin> request)
        {
            IQueryable<InteriorDoorSkin> items = GetInteriorDoorSkins(request);

            // Execute
            var records = await items.Skip(skip)
                                     .Take(take)
                                     .Select(r => new InteriorDoorSkin
                                     {
                                         InteriorDoorSkinID = r.InteriorDoorSkinID,
                                     })
                                     .AsNoTracking()
                                     .ToListAsync();

            return records;
        }

        public async Task<int> GetInteriorDoorSkinsCountAsync(DataRequest<InteriorDoorSkin> request)
        {
            IQueryable<InteriorDoorSkin> items = _dataSource.InteriorDoorSkins;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.ToLower().Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateInteriorDoorSkinAsync(InteriorDoorSkin interiorDoorSkin)
        {
            if (await GetInteriorDoorSkinAsync(interiorDoorSkin.InteriorDoorSkinID) is null)
            {
                // interiorDoorSkin.InteriorDoorSkinID = UIDGenerator.Next();
                _dataSource.Entry(interiorDoorSkin).State = EntityState.Added;
            }
            else
            {
                _dataSource.Entry(interiorDoorSkin).State = EntityState.Modified;
            }

            // interiorDoorSkin.LastModifiedOn = DateTime.UtcNow;
            interiorDoorSkin.SearchTerms = interiorDoorSkin.BuildSearchTerms();
            int res = await _dataSource.SaveChangesAsync();

            return res;
        }

        public async Task<int> DeleteInteriorDoorSkinsAsync(params InteriorDoorSkin[] interiorDoorSkins)
        {
            _dataSource.InteriorDoorSkins.RemoveRange(interiorDoorSkins);
            return await _dataSource.SaveChangesAsync();
        }

    }
}
