using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        public DashboardViewModel(ICustomerService customerService, 
                                  IOrderService orderService,
                                  IPaymentService paymentService,
                                  ICommonServices commonServices) : base(commonServices)
        {
            CustomerService = customerService;
            OrderService = orderService;
            PaymentService = paymentService;
        }

        public ICustomerService CustomerService { get; }
        public IOrderService OrderService { get; }
        public IPaymentService PaymentService { get; }

        private IList<CustomerModel> _customers = null;
        public IList<CustomerModel> Customers
        {
            get => _customers;
            set => Set(ref _customers, value);
        }

        private IList<PaymentModel> _payments = null;
        public IList<PaymentModel> Payments
        {
            get => _payments;
            set => Set(ref _payments, value);
        }

        private IList<OrderModel> _orders = null;
        public IList<OrderModel> Orders
        {
            get => _orders;
            set => Set(ref _orders, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Φόρτωση αρχικής οθόνης...");
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadPaymentsAsync();
            EndStatusMessage("Αρχική οθόνη φορτώθηκε");
        }
        public void Unload()
        {
            Customers = null;
            Payments = null;
            Orders = null;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Customers = await CustomerService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Customers", ex);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var request = new DataRequest<Order>
                {
                    OrderByDesc = r => r.OrderDate
                };
                Orders = await OrderService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Orders", ex);
            }
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                var request = new DataRequest<Payment>
                {
                    OrderByDesc = r => r.PaymentDate
                };
                Payments = await PaymentService.GetPaymentsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                LogException("Dashboard", "Load Payments", ex);
            }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case "Customers":
                    NavigationService.Navigate<CustomersViewModel>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                    break;
                case "Orders":
                    NavigationService.Navigate<OrdersViewModel>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                    break;
                case "Payments":
                    NavigationService.Navigate<PaymentsViewModel>(new PaymentListArgs { OrderByDesc = r => r.PaymentDate });
                    break;
                default:
                    break;
            }
        }

    }
}
