using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class CustomersPayments : UserControl
    {
        public CustomersPayments()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public PaymentListViewModel ViewModel
        {
            get { return (PaymentListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PaymentListViewModel), typeof(CustomersPayments), new PropertyMetadata(null));
        #endregion
    }
}
