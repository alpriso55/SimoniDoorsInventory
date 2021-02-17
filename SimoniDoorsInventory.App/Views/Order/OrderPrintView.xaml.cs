using System;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Services;
using SimoniDoorsInventory.Views;
using SimoniDoorsInventory.Controls;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrderPrintView : Page
    {
        public OrderPrintView()
        {
            ViewModel = ServiceLocator.Current.GetService<OrderPrintDetailsWithItemsViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            
            this.InitializeComponent();
        }

        public OrderPrintDetailsWithItemsViewModel ViewModel { get; }
        public INavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as OrderDetailsArgs);

            if (ViewModel.OrderDetails.IsEditMode)
            {
                await Task.Delay(100);
                details.SetFocus();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        // private async void OnLoaded(object sender, RoutedEventArgs e)
        // {
        //     ViewModel.Subscribe();
        //     await ViewModel.LoadAsync(ViewModel.OrderDetails.CreateArgs());
        // }
        // 
        // private void OnUnloaded(object sender, RoutedEventArgs e)
        // {
        //     ViewModel.Unload();
        //     ViewModel.Unsubscribe();
        // }

        // #region ViewModel
        // public OrderPrintDetailsWithItemsViewModel ViewModel
        // {
        //     get { return (OrderPrintDetailsWithItemsViewModel)GetValue(ViewModelProperty); }
        //     set { SetValue(ViewModelProperty, value); }
        // }
        // 
        // public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(OrderPrintDetailsWithItemsViewModel), typeof(OrderPrintView), new PropertyMetadata(null));
        // #endregion

    }
}
