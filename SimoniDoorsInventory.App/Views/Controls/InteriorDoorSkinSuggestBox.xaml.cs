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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Controls
{
    public sealed partial class InteriorDoorSkinSuggestBox : UserControl
    {
        public InteriorDoorSkinSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                InteriorDoorSkinService = ServiceLocator.Current.GetService<IInteriorDoorSkinService>();
            }

            this.InitializeComponent();
        }

        private IInteriorDoorSkinService InteriorDoorSkinService { get; }

        #region Items
        public IList<InteriorDoorSkinModel> Items
        {
            get { return (IList<InteriorDoorSkinModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<InteriorDoorSkinModel>), typeof(InteriorDoorSkinSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(InteriorDoorSkinSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InteriorDoorSkinSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(InteriorDoorSkinSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region InteriorDoorSkinSelectedCommand
        public ICommand InteriorDoorSkinSelectedCommand
        {
            get { return (ICommand)GetValue(InteriorDoorSkinSelectedCommandProperty); }
            set { SetValue(InteriorDoorSkinSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty InteriorDoorSkinSelectedCommandProperty = DependencyProperty.Register(nameof(InteriorDoorSkinSelectedCommand), typeof(ICommand), typeof(InteriorDoorSkinSuggestBox), new PropertyMetadata(null));
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

        private async Task<IList<InteriorDoorSkinModel>> GetItems(string query)
        {
            var request = new DataRequest<InteriorDoorSkin>()
            {
                Query = query,
                OrderByDesc = r => r.InteriorDoorSkinID
            };
            return await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(0, 30, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            InteriorDoorSkinSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
