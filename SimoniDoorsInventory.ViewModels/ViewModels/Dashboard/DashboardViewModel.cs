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
                                  IInteriorDoorSkinService interiorDoorSkinService,
                                  ICommonServices commonServices) : base(commonServices)
        {
            CustomerService = customerService;
            OrderService = orderService;
            PaymentService = paymentService;
            InteriorDoorSkinService = interiorDoorSkinService;
        }

        public ICustomerService CustomerService { get; }
        public IOrderService OrderService { get; }
        public IPaymentService PaymentService { get; }
        public IInteriorDoorSkinService InteriorDoorSkinService { get; }

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

        private IList<InteriorDoorSkinModel> _interiorDoorSkins = null;
        public IList<InteriorDoorSkinModel> InteriorDoorSkins
        {
            get => _interiorDoorSkins;
            set => Set(ref _interiorDoorSkins, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Φόρτωση αρχικής οθόνης...");
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadPaymentsAsync();
            await LoadInteriorDoorSkinsAsync();
            EndStatusMessage("Αρχική οθόνη φορτώθηκε");
        }
        public void Unload()
        {
            Customers = null;
            Payments = null;
            Orders = null;
            InteriorDoorSkins = null;
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

        private async Task LoadInteriorDoorSkinsAsync()
        {
            try
            {
                var request = new DataRequest<InteriorDoorSkin>
                {
                    OrderBy = r => r.StockUnits
                };
                var totalSkins = await InteriorDoorSkinService.GetInteriorDoorSkinsCountAsync(request);
                InteriorDoorSkins = await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(0, totalSkins, request);
            }
            catch (Exception ex)
            {
                LogException("Dashoard", "Load Interior Door Skins", ex);
            }
        }

        public void ItemSelected(string item)
        {
            switch (item)
            {
                case "Πελάτες":
                    NavigationService.Navigate<CustomersViewModel>(new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                    break;
                case "Παραγγελίες":
                    NavigationService.Navigate<OrdersViewModel>(new OrderListArgs { OrderByDesc = r => r.OrderDate });
                    break;
                case "Πληρωμές":
                    NavigationService.Navigate<PaymentsViewModel>(new PaymentListArgs { OrderByDesc = r => r.PaymentDate });
                    break;
                case "Επενδύσεις":
                    NavigationService.Navigate<InteriorDoorSkinsViewModel>(new InteriorDoorSkinListArgs { OrderBy = r => r.InteriorDoorSkinID });
                    break;
                default:
                    break;
            }
        }

    }
}
