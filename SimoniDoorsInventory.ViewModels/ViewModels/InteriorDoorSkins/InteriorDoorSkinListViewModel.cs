using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region InteriorDoorSkinListArgs
    public class InteriorDoorSkinListArgs
    {
        static public InteriorDoorSkinListArgs CreateEmpty() => new InteriorDoorSkinListArgs { IsEmpty = true };

        public InteriorDoorSkinListArgs()
        {
            OrderBy = r => r.InteriorDoorSkinID;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<InteriorDoorSkin, object>> OrderBy { get; set; }
        public Expression<Func<InteriorDoorSkin, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class InteriorDoorSkinListViewModel : GenericListViewModel<InteriorDoorSkinModel>
    {
        public InteriorDoorSkinListViewModel(IInteriorDoorSkinService interiorDoorSkinService, ICommonServices commonServices)
            : base(commonServices)
        {
            InteriorDoorSkinService = interiorDoorSkinService;
        }

        public IInteriorDoorSkinService InteriorDoorSkinService { get; set; }

        public InteriorDoorSkinListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(InteriorDoorSkinListArgs args)
        {
            ViewModelArgs = args ?? InteriorDoorSkinListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Φόρτωση επενδύσεων...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Επενδύσεις φορτώθηκαν");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<InteriorDoorSkinListViewModel>(this, OnMessage);
            MessageService.Subscribe<InteriorDoorSkinDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public InteriorDoorSkinListArgs CreateArgs()
        {
            return new InteriorDoorSkinListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
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
                Items = new List<InteriorDoorSkinModel>();
                StatusError($"Σφάλμα φόρτωσης επενδύσεων: {ex.Message}");
                LogException("InteriorDoorSkins", "Refresh", ex);
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

        private async Task<IList<InteriorDoorSkinModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<InteriorDoorSkin> request = BuildDataRequest();
                return await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(request);
            }
            return new List<InteriorDoorSkinModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await NavigationService.CreateNewViewAsync<InteriorDoorSkinDetailsViewModel>(new InteriorDoorSkinDetailsArgs { InteriorDoorSkinID = SelectedItem.InteriorDoorSkinID });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<InteriorDoorSkinDetailsViewModel>(new InteriorDoorSkinDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<InteriorDoorSkinDetailsViewModel>(new InteriorDoorSkinDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Φόρτωση επενδύσεων...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Επενδύσεις φορτώθηκαν");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε τις επιλεγμένες επενδύσεις;", "Ναι", "Ακύρωση"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Διαγραφή {count} επενδύσεων...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Διαγραφή {count} επενδύσεων...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Σφάλμα διαγραφής {count} επενδύσεων: {ex.Message}");
                    LogException("InteriorDoorSkins", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} επενδύσεις διαγράφτηκαν");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<InteriorDoorSkinModel> models)
        {
            foreach (var model in models)
            {
                await InteriorDoorSkinService.DeleteInteriorDoorSkinAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<InteriorDoorSkin> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await InteriorDoorSkinService.DeleteInteriorDoorSkinRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<InteriorDoorSkin> BuildDataRequest()
        {
            return new DataRequest<InteriorDoorSkin>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        private async void OnMessage(ViewModelBase sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
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

    }
}
