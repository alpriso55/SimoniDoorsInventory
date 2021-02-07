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
    public sealed partial class OrderView : Page
    {
        public OrderView()
        {
            ViewModel = ServiceLocator.Current.GetService<OrderDetailsWithItemsViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            InitializeComponent();

            // Toolbar = details.GetToolBar();
            // Toolbar.ButtonClick += OnPrintButtonClicked;
        }

        public DetailToolbar Toolbar { get; set; }

        public OrderDetailsWithItemsViewModel ViewModel { get; }
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

        // private async void OnPrintButtonClicked(object sender, ToolbarButtonClickEventArgs e)
        // {
        //     if (e.ClickedButton == ToolbarButton.Print)
        //     {
        //         await NavigationService.CreateNewViewAsync<OrderPrintDetailsWithItemsViewModel>(ViewModel.OrderDetails.CreateArgs());
        //     }
        // }

    }
}
