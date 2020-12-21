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
    #region ProductListArgs
    public class PaymentListArgs
    {
        static public PaymentListArgs CreateEmpty() => new PaymentListArgs { IsEmpty = true };

        public PaymentListArgs()
        {
            OrderByDesc = r => r.PaymentDate;
        }

        public bool IsEmpty { get; set; }

        public long AccountID { get; set; }

        public string Query { get; set; }

        public Expression<Func<Payment, object>> OrderBy { get; set; }
        public Expression<Func<Payment, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class PaymentListViewModel : GenericListViewModel<PaymentModel>
    {
        public PaymentListViewModel(IPaymentService paymentService, ICommonServices commonServices) : base(commonServices)
        {
            PaymentService = paymentService;
        }

        public IPaymentService PaymentService { get; }

        public PaymentListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<PaymentModel>(ItemInvoked);
        private async void ItemInvoked(PaymentModel model)
        {
            await NavigationService.CreateNewViewAsync<PaymentDetailsViewModel>(new PaymentDetailsArgs { PaymentID = model.PaymentID });
        }

        public async Task LoadAsync(PaymentListArgs args)
        {
            ViewModelArgs = args ?? PaymentListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading Payments...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Payments loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PaymentListViewModel>(this, OnMessage);
            MessageService.Subscribe<PaymentDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public PaymentListArgs CreateArgs()
        {
            return new PaymentListArgs
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
                Items = new List<PaymentModel>();
                StatusError($"Error loading Payments: {ex.Message}");
                LogException("Payments", "Refresh", ex);
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

        private async Task<IList<PaymentModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Payment> request = BuildDataRequest();
                return await PaymentService.GetPaymentsAsync(request);
            }
            return new List<PaymentModel>();
        }

        protected override async void OnNew()
        {

            if (IsMainView)
            {
                await NavigationService.CreateNewViewAsync<PaymentDetailsViewModel>(new PaymentDetailsArgs());
            }
            else
            {
                NavigationService.Navigate<PaymentDetailsViewModel>(new PaymentDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading Payments...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Payments loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε την επιλεγμένη πληρωμή;", "Ναι", "Ακύρωση"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} Payments...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} Payments...");
                        await DeleteItemsAsync(SelectedItems);
                        MessageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Payments: {ex.Message}");
                    LogException("Payments", "Delete", ex);
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} Payments deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<PaymentModel> models)
        {
            foreach (var model in models)
            {
                await PaymentService.DeletePaymentAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Payment> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await PaymentService.DeletePaymentRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Payment> BuildDataRequest()
        {
            return new DataRequest<Payment>()
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
