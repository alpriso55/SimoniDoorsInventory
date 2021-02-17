using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;
using SimoniDoorsInventory.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class OrderPrint : UserControl
    {
        public OrderPrint()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public OrderPrintDetailsWithItemsViewModel ViewModel
        {
            get { return (OrderPrintDetailsWithItemsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(OrderPrintDetailsWithItemsViewModel), typeof(OrderPrint), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }

    }
}
