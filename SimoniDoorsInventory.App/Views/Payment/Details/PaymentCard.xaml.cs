using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class PaymentCard : UserControl
    {
        public PaymentCard()
        {
            InitializeComponent();
        }

        #region ViewModel
        public PaymentDetailsViewModel ViewModel
        {
            get { return (PaymentDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(PaymentDetailsViewModel), typeof(PaymentCard), new PropertyMetadata(null));
        #endregion

        #region Item
        public PaymentModel Item
        {
            get { return (PaymentModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(PaymentModel), typeof(PaymentCard), new PropertyMetadata(null));
        #endregion
    }
}
