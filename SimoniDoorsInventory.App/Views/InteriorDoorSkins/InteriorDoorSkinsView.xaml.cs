using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Services;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimoniDoorsInventory.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InteriorDoorSkinsView : Page
    {
        public InteriorDoorSkinsView()
        {
            ViewModel = ServiceLocator.Current.GetService<InteriorDoorSkinsViewModel>();
            NavigationService = ServiceLocator.Current.GetService<INavigationService>();
            this.InitializeComponent();
        }

        public InteriorDoorSkinsViewModel ViewModel { get; }
        public INavigationService NavigationService { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as InteriorDoorSkinListArgs);

            // var childDetails = details.GetDetails();
            // var detailToolBar = childDetails.GetDetailToolbar();
            // // detailToolBar.DefaultCommands = "edit,delete";
            // var printButton = detailToolBar.GetPrintButton();
            // printButton.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            await NavigationService.CreateNewViewAsync<InteriorDoorSkinsViewModel>(ViewModel.InteriorDoorSkinList.CreateArgs());
        }

        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            ViewModel.InteriorDoorSkinDetails.CancelEdit();
            await NavigationService.CreateNewViewAsync<InteriorDoorSkinDetailsViewModel>(ViewModel.InteriorDoorSkinDetails.CreateArgs());
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

    }
}
