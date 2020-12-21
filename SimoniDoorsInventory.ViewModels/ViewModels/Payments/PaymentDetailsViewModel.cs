﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region ProductDetailsArgs
    public class PaymentDetailsArgs
    {
        static public PaymentDetailsArgs CreateDefault() => new PaymentDetailsArgs();

        public long AccountID { get; set; }
        public long PaymentID { get; set; }

        public bool IsNew => PaymentID <= 0;
    }
    #endregion

    public class PaymentDetailsViewModel : GenericDetailsViewModel<PaymentModel>
    {
        public PaymentDetailsViewModel(IPaymentService paymentService, 
                                       ICommonServices commonServices) : base(commonServices)
        {
            PaymentService = paymentService;
        }

        public IPaymentService PaymentService { get; }

        override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Account == null ? "New Payment" : $"New Payment, {Item?.Account?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Payment" : $"Payment {Item?.PaymentID}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditAccount => Item?.AccountID <= 0;

        public ICommand AccountSelectedCommand => new RelayCommand<AccountModel>(AccountSelected);
        private void AccountSelected(AccountModel account)
        {
            EditableItem.AccountID = account.CustomerID;
            EditableItem.Account = account;

            EditableItem.NotifyChanges();
        }

        public PaymentDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(PaymentDetailsArgs args)
        {
            ViewModelArgs = args ?? PaymentDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new PaymentModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await PaymentService.GetPaymentAsync(ViewModelArgs.PaymentID);
                    Item = item ?? new PaymentModel { PaymentID = ViewModelArgs.PaymentID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Payment", "Load", ex);
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.PaymentID = Item?.PaymentID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PaymentDetailsViewModel, PaymentModel>(this, OnDetailsMessage);
            MessageService.Subscribe<PaymentListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public PaymentDetailsArgs CreateArgs()
        {
            return new PaymentDetailsArgs
            {
                PaymentID = Item?.PaymentID ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(PaymentModel model)
        {
            try
            {
                StartStatusMessage("Saving Payment...");
                await Task.Delay(100);
                await PaymentService.UpdatePaymentAsync(model);
                EndStatusMessage("Payment saved");
                LogInformation("Payment", "Save", "Payment saved successfully", $"Payment {model.PaymentID} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Payment: {ex.Message}");
                LogException("Payment", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(PaymentModel model)
        {
            try
            {
                StartStatusMessage("Deleting Payment...");
                await Task.Delay(100);
                await PaymentService.DeletePaymentAsync(model);
                EndStatusMessage("Payment deleted");
                LogWarning("Payment", "Delete", "Payment deleted", $"Payment {model.PaymentID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Payment: {ex.Message}");
                LogException("Payment", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε αυτή τη πληρωμή;", "Ναι", "Ακύρωση");
        }

        override protected IEnumerable<IValidationConstraint<PaymentModel>> GetValidationConstraints(PaymentModel model)
        {
            yield return new RequiredConstraint<PaymentModel>("Account", m => m.AccountID);
            yield return new RequiredConstraint<PaymentModel>("Amount", m => m.Amount);
            yield return new RequiredGreaterThanZeroConstraint<PaymentModel>("Amount", m => m.Amount);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(PaymentDetailsViewModel sender, string message, PaymentModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.PaymentID == current?.PaymentID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await PaymentService.GetPaymentAsync(current.PaymentID);
                                    item = item ?? new PaymentModel { PaymentID = current.PaymentID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This payment has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Payment", "Handle Changes", ex);
                                }
                            });
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(PaymentListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<PaymentModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.PaymentID == current.PaymentID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await PaymentService.GetPaymentAsync(current.PaymentID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Payment", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This payment has been deleted externally");
            });
        }
    }
}
