using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class PaymentDetails : UserControl
    {
        public PaymentDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public PaymentDetailsViewModel ViewModel
        {
            get { return (PaymentDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PaymentDetailsViewModel), typeof(PaymentDetails), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }
    }
}
