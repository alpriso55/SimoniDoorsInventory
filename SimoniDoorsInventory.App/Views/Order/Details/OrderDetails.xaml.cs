using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Controls;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrderDetails : UserControl
    {
        public OrderDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public OrderDetailsWithItemsViewModel ViewModel
        {
            get { return (OrderDetailsWithItemsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(OrderDetailsWithItemsViewModel), typeof(OrderDetails), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }

        public int GetRowSpan(bool isItemNew)
        {
            return isItemNew ? 2 : 1;
        }

        public AppBarButton GetPrintButton()
        {
            return details.GetPrintButton();
        }

        public DetailToolbar GetToolBar()
        {
            return details.GetDetailToolbar();
        }

    }
}
