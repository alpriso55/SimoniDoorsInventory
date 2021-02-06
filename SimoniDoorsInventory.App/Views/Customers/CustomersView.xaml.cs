using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Services;
using System.Threading.Tasks;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class CustomersView : Page
    {
        public CustomersView()
        {
            ViewModel = ServiceLocator.Current.GetService<CustomersViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            InitializeComponent();
        }

        public CustomersViewModel ViewModel { get; }
        public INavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as CustomerListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            await NavigationService.CreateNewViewAsync<CustomersViewModel>(ViewModel.CustomerList.CreateArgs());
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.CustomerDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                await NavigationService.CreateNewViewAsync<CustomerDetailsViewModel>(ViewModel.CustomerDetails.CreateArgs());
            }
            else if (pivot.SelectedIndex == 1)
            {
                await NavigationService.CreateNewViewAsync<OrdersViewModel>(ViewModel.CustomerOrders.CreateArgs());
            }
            else
            {
                await NavigationService.CreateNewViewAsync<PaymentsViewModel>(ViewModel.CustomerPayments.CreateArgs());
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            await ViewModel.SelectedPivotItemChanged(pivot.SelectedIndex);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}
