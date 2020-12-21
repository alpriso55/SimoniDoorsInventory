using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrdersInteriorDoors : UserControl
    {
        public OrdersInteriorDoors()
        {
            InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorListViewModel ViewModel
        {
            get { return (InteriorDoorListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(InteriorDoorListViewModel), typeof(OrdersInteriorDoors), new PropertyMetadata(null));
        #endregion
    }
}
