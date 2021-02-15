using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region InteriorDoorListArgs
    public class InteriorDoorListArgs
    {
        static public InteriorDoorListArgs CreateEmpty() => new InteriorDoorListArgs { IsEmpty = true };

        public InteriorDoorListArgs()
        {
            OrderBy = r => r.InteriorDoorID;
        }

        public long OrderID { get; set; }
        public bool IsEmpty { get; set; }
        public string Query { get; set; }

        public Expression<Func<InteriorDoor, object>> OrderBy { get; set; }
        public Expression<Func<InteriorDoor, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class InteriorDoorListViewModel : GenericListViewModel<InteriorDoorModel>
    {
        public InteriorDoorListViewModel(IInteriorDoorService interiorDoorService,
                                         ICommonServices commonServices) : base(commonServices)
        {
            InteriorDoorService = interiorDoorService;
        }

        public IInteriorDoorService InteriorDoorService { get; }
        public InteriorDoorListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(InteriorDoorListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? InteriorDoorListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            if (silent)
            {
                await RefreshAsync();
            }
            else
            {
                StartStatusMessage("Loading Interior Doors...");
                if (await RefreshAsync())
                {
                    EndStatusMessage("InteriorDoors loaded");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorListViewModel>(this, OnMessage);
            MessageService.Subscribe<InteriorDoorDetailsViewModel>(this, OnMessage);
            // MessageService.Subscribe<GenericDetailsViewModel<OrderModel>, OrderModel>(this, OnOrderDetailsMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public InteriorDoorListArgs CreateArgs()
        {
            return new InteriorDoorListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                OrderID = ViewModelArgs.OrderID
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<InteriorDoorModel>();
                StatusError($"Error loading Interior Doors: {ex.Message}");
                LogException("InteriorDoors", "Refresh", ex);
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            NotifyPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<InteriorDoorModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<InteriorDoor> request = BuildDataRequest();
                return await InteriorDoorService.GetInteriorDoorsAsync(request);
            }
            return new List<InteriorDoorModel>();
        }

        public async Task<decimal> GetSumOfPrices()
        {
            if (Items == null)
            {
                return 0.0m;
            }

            return await Task.Run(() => Items.Select(r => r.Price).Sum());
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await NavigationService.CreateNewViewAsync<InteriorDoorDetailsViewModel>(new InteriorDoorDetailsArgs { OrderID = SelectedItem.OrderID, InteriorDoorID = SelectedItem.InteriorDoorID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<InteriorDoorDetailsViewModel>(new InteriorDoorDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }
            else
            {
                NavigationService.Navigate<InteriorDoorDetailsViewModel>(new InteriorDoorDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading Interior Doors...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Interior Doors loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε το επιλεγμένο προϊόν;", "Ναι", "Ακύρωση"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} Interior Doors...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} Interior Doors...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Interior Doors: {ex.Message}");
                    LogException("InteriorDoors", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} Interior Doors deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<InteriorDoorModel> models)
        {
            foreach (var model in models)
            {
                await InteriorDoorService.DeleteInteriorDoorAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<InteriorDoor> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await InteriorDoorService.DeleteInteriorDoorRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<InteriorDoor> BuildDataRequest()
        {
            var request = new DataRequest<InteriorDoor>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
            if (ViewModelArgs.OrderID > 0)
            {
                request.Where = (r) => r.OrderID == ViewModelArgs.OrderID;
            }
            return request;
        }

        private async void OnMessage(ViewModelBase sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemChanged":   // Maybe you should not use this line
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await ContextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }

        public ICommand PrintInNewViewCommand => new RelayCommand(OnPrintInNewView);
        private async void OnPrintInNewView()
        {
            await InteriorDoorService.SaveInteriorDoorListToExcelFileAsync(Items);
        }
    }
}

