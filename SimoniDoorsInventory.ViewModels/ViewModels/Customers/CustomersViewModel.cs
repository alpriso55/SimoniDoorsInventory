using System;
using System.Threading.Tasks;

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
        public PaymentListViewModel CustomerPayments { get; set; }  // NEW LINE

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
            MessageService.Subscribe<CustomerListViewModel>(this, OnMessage);
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

        private async void OnMessage(CustomerListViewModel viewModel, string message, object args)
        {
            if (viewModel == CustomerList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
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
