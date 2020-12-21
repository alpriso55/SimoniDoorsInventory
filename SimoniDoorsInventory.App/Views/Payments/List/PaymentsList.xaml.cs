using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class PaymentsList : UserControl
    {
        public PaymentsList()
        {
            InitializeComponent();
        }

        #region ViewModel
        public PaymentListViewModel ViewModel
        {
            get { return (PaymentListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PaymentListViewModel), typeof(PaymentsList), new PropertyMetadata(null));
        #endregion
    }
}
