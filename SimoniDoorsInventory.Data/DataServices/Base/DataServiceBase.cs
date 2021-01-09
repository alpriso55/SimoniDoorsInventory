using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SimoniDoorsInventory.Data.Services
{
    abstract public partial class DataServiceBase : IDataService, IDisposable
    {
        private IDataSource _dataSource = null;

        public DataServiceBase(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IList<Accessory>> GetAccessoriesAsync()
        {
            return await _dataSource.Accessories.ToListAsync();
        }

        public async Task<IList<Category>> GetCategoriesAsync()
        {
            return await _dataSource.Categories.ToListAsync();
        }

        public async Task<IList<Crew>> GetCrewsAsync()
        {
            return await _dataSource.Crews.ToListAsync();
        }

        public async Task<IList<InteriorDoorDesign>> GetInteriorDoorDesignsAsync()
        {
            return await _dataSource.InteriorDoorDesigns.ToListAsync();
        }

        public async Task<IList<InteriorDoorSkin>> GetInteriorDoorSkinsAsync()
        {
            return await _dataSource.InteriorDoorSkins.ToListAsync();
        }

        public async Task<IList<OpeningSide>> GetOpeningSidesAsync()
        {
            return await _dataSource.OpeningSides.ToListAsync();
        }

        public async Task<IList<OpeningType>> GetOpeningTypesAsync()
        {
            return await _dataSource.OpeningTypes.ToListAsync();
        }

        public async Task<IList<OrderStatus>> GetOrderStatusAsync()
        {
            return await _dataSource.OrderStatus.ToListAsync();
        }

        public async Task<IList<PaymentType>> GetPaymentTypesAsync()
        {
            return await _dataSource.PaymentTypes.ToListAsync();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
            }
        }
        #endregion
    }
}
