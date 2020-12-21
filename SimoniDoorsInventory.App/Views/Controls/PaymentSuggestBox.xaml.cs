using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Controls
{
    public sealed partial class PaymentSuggestBox : UserControl
    {
        public PaymentSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                PaymentService = ServiceLocator.Current.GetService<IPaymentService>();
            }
            InitializeComponent();
        }

        private IPaymentService PaymentService { get; }

        #region Items
        public IList<PaymentModel> Items
        {
            get { return (IList<PaymentModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<PaymentModel>), typeof(PaymentSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(PaymentSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PaymentSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PaymentSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region PaymentSelectedCommand
        public ICommand PaymentSelectedCommand
        {
            get { return (ICommand)GetValue(PaymentSelectedCommandProperty); }
            set { SetValue(PaymentSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty PaymentSelectedCommandProperty = DependencyProperty.Register(nameof(PaymentSelectedCommand), typeof(ICommand), typeof(PaymentSuggestBox), new PropertyMetadata(null));
        #endregion

        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (args.CheckCurrent())
                {
                    Items = string.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
                }
            }
        }

        private async Task<IList<PaymentModel>> GetItems(string query)
        {
            var request = new DataRequest<Payment>()
            {
                Query = query,
                OrderByDesc = r => r.PaymentDate
            };
            return await PaymentService.GetPaymentsAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            PaymentSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
