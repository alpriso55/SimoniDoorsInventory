using System;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class PaymentsPane : UserControl
    {
        public PaymentsPane()
        {
            InitializeComponent();
        }

        #region ItemsSource
        public IList<PaymentModel> ItemsSource
        {
            get { return (IList<PaymentModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<PaymentModel>), typeof(PaymentsPane), new PropertyMetadata(null));
        #endregion
    }
}
