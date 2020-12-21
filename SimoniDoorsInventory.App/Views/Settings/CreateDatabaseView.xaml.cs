using System;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class CreateDatabaseView : ContentDialog
    {
        private string _connectionString = null;

        public CreateDatabaseView(string connectionString)
        {
            _connectionString = connectionString;
            ViewModel = ServiceLocator.Current.GetService<CreateDatabaseViewModel>();
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public CreateDatabaseViewModel ViewModel { get; }

        public Result Result { get; private set; }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.ExecuteAsync(_connectionString);
            Result = ViewModel.Result;
        }

        private void OnOkClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnCancelClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = Result.Ok("Operation cancelled by user");
        }
    }
}
