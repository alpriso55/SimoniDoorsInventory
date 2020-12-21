using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrdersDetails : UserControl
    {
        public OrdersDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public OrderDetailsViewModel ViewModel
        {
            get { return (OrderDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(OrderDetailsViewModel), typeof(OrdersDetails), new PropertyMetadata(null));
        #endregion
    }
}
