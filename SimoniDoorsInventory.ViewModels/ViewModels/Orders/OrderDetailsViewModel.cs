using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    #region OrderDetailsArgs
    public class OrderDetailsArgs
    {
        static public OrderDetailsArgs CreateDefault() => new OrderDetailsArgs { CustomerID = 0 };

        public long CustomerID { get; set; }
        public long OrderID { get; set; }

        public bool IsNew => OrderID <= 0;
    }
    #endregion

    public class OrderDetailsViewModel : GenericDetailsViewModel<OrderModel>
    {
        public OrderDetailsViewModel(IOrderService orderService, 
                                     ICommonServices commonServices) : base(commonServices)
        {
            OrderService = orderService;
        }

        public IOrderService OrderService { get; }

        public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "Νέα Παραγγελία" : $"Νέα Παραγγελία, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Παραγγελία" : $"Παραγγελία #{Item?.OrderID}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerID <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerModel>(CustomerSelected);
        private void CustomerSelected(CustomerModel customer)
        {
            EditableItem.CustomerID = customer.CustomerID;
            EditableItem.Customer = customer;

            /* 
             * These lines were included in the original code, but later we made some changes 
             * because Customer's address was not always the same as Order's address. 
             */
            EditableItem.AddressLine = customer.AddressLine;
            EditableItem.City = customer.City;
            EditableItem.PostalCode = customer.PostalCode;
            EditableItem.Floor = customer.Floor;

            EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = await OrderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await OrderService.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new OrderModel { OrderID = ViewModelArgs.OrderID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Order", "Load", ex);
                }
            }
            NotifyPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnDetailsMessage);
            MessageService.Subscribe<OrderListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0,
                OrderID = Item?.OrderID ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Αποθήκευση παραγγελίας...");
                await Task.Delay(100);
                await OrderService.UpdateOrderAsync(model);
                EndStatusMessage("Παραγγελία αποθηκεύτηκε");
                LogInformation("Order", "Save", "Order saved successfully", $"Order #{model.OrderID} was saved successfully.");
                NotifyPropertyChanged(nameof(CanEditCustomer));
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Σφάλμα αποθήκευσης παραγγελίας: {ex.Message}");
                LogException("Order", "Save", ex);
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Διαγραφή παραγγελίας...");
                await Task.Delay(100);
                await OrderService.DeleteOrderAsync(model);
                EndStatusMessage("Παραγγελία διεγράφη");
                LogWarning("Order", "Delete", "Order deleted", $"Order #{model.OrderID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Σφάλμα διαγραφής Παραγγελίας: {ex.Message}");
                LogException("Order", "Delete", ex);
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Επιβεβαίωση Διαγραφής", "Σίγουρα θέλετε να διαγράψετε αυτή τη παραγγελία;", "Ναι", "Ακύρωση");
        }

        override protected IEnumerable<IValidationConstraint<OrderModel>> GetValidationConstraints(OrderModel model)
        {
            yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Customer", m => m.CustomerID);
            yield return new RequiredConstraint<OrderModel>("Status", m => m.Status);
            yield return new PositiveConstraint<OrderModel>("TotalCost", m => m.TotalCost);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await OrderService.GetOrderAsync(current.OrderID);
                                    item = item ?? new OrderModel { OrderID = current.OrderID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This order has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Order", "Handle Changes", ex);
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

        private async void OnListMessage(OrderListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<OrderModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await OrderService.GetOrderAsync(current.OrderID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Order", "Handle Ranges Deleted", ex);
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
                StatusMessage("WARNING: This order has been deleted externally");
            });
        }

    }
}
