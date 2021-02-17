using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class CustomersViewModel : ViewModelBase
    {
        public CustomersViewModel(ICustomerService customerService,
                                  IOrderService orderService,
                                  IPaymentService paymentService,  // NEW LINE
                                  IFilePickerService filePickerService,
                                  ICommonServices commonServices)
            : base(commonServices)
        {
            CustomerService = customerService;

            CustomerList = new CustomerListViewModel(CustomerService, commonServices);
            CustomerDetails = new CustomerDetailsViewModel(CustomerService, filePickerService, commonServices);
            CustomerOrders = new OrderListViewModel(orderService, commonServices);
            CustomerPayments = new PaymentListViewModel(paymentService, commonServices);
        }

        public ICustomerService CustomerService { get; }

        public CustomerListViewModel CustomerList { get; set; }
        public CustomerDetailsViewModel CustomerDetails { get; set; }
        public OrderListViewModel CustomerOrders { get; set; }
        public PaymentListViewModel CustomerPayments { get; set; }

        public async Task LoadAsync(CustomerListArgs args)
        {
            await CustomerList.LoadAsync(args);
        }

        public void Unload()
        {
            CustomerDetails.CancelEdit();
            CustomerList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<CustomerListViewModel>(this, OnCustomerListMessage);
            MessageService.Subscribe<CustomerDetailsViewModel>(this, OnCustomerDetailsMessage);
            // MessageService.Subscribe<OrderListViewModel>(this, OnOrdersMessage);
            // MessageService.Subscribe<PaymentListViewModel>(this, OnPaymentsMessage);
            CustomerList.Subscribe();
            CustomerDetails.Subscribe();
            CustomerOrders.Subscribe();
            CustomerPayments.Subscribe();  // NEW LINE
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            CustomerList.Unsubscribe();
            CustomerDetails.Unsubscribe();
            CustomerOrders.Unsubscribe();
            CustomerPayments.Unsubscribe();
        }

        private async void OnCustomerListMessage(CustomerListViewModel viewModel, string message, object args)
        {
            if (viewModel == CustomerList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnCustomerDetailsMessage(CustomerDetailsViewModel viewModel, string message, object args)
        {
            if (viewModel == CustomerDetails && message == "PrintButtonPressed")
            {
                await CustomerService.SaveCustomerDetailsToExcelFileAsync(CustomerDetails.Item, CustomerOrders.Items, CustomerPayments.Items);
            }
        }

        public async Task SelectedPivotItemChanged(int selectedIndex)
        {
            // Memorize selected item so as you can use it again
            var selectedItem = new CustomerModel();
            selectedItem.Merge(CustomerList.SelectedItem);

            if (selectedIndex == 0)
            {
                await CustomerList.LoadAsync(CustomerList.CreateArgs());
            }
            
            CustomerList.SelectedItem.Merge(selectedItem);
        }

        public async void OnItemSelected()
        {
            if (CustomerDetails.IsEditMode)
            {
                StatusReady();
                CustomerDetails.CancelEdit();
            }
            CustomerOrders.IsMultipleSelection = false;
            CustomerPayments.IsMultipleSelection = false;  // NEW LINE
            var selected = CustomerList.SelectedItem;
            if (!CustomerList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateOrders(selected);
                    await PopulatePayments(selected);  // NEW LINE
                }
            }
            var totalPayments = await CustomerPayments.GetTotalPayments();
            var totalCosts = await CustomerOrders.GetTotalCosts();
            selected.Balance = totalPayments - totalCosts;
            CustomerDetails.Item = selected;
        }

        private async Task PopulateDetails(CustomerModel selected)
        {
            try
            {
                var model = await CustomerService.GetCustomerAsync(selected.CustomerID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Customers", "Load Details", ex);
            }
        }

        private async Task PopulateOrders(CustomerModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await CustomerOrders.LoadAsync(new OrderListArgs { CustomerID = selectedItem.CustomerID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Customers", "Load Orders", ex);
            }
        }

        private async Task PopulatePayments(CustomerModel selectedItem)  // New LINE
        {
            try
            {
                if (selectedItem != null)
                {
                    await CustomerPayments.LoadAsync(new PaymentListArgs { CustomerID = selectedItem.CustomerID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                LogException("Customers", "Load Payments", ex);
            }
        }

    }
}
