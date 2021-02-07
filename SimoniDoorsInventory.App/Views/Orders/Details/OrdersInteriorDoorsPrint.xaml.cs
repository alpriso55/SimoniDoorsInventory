using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrdersInteriorDoorsPrint : UserControl
    {
        public OrdersInteriorDoorsPrint()
        {
            InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorListViewModel ViewModel
        {
            get { return (InteriorDoorListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(InteriorDoorListViewModel), typeof(OrdersInteriorDoorsPrint), new PropertyMetadata(null));
        #endregion
    }
}
