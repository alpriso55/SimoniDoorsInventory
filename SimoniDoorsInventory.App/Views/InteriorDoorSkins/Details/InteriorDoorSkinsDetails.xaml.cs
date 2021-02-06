using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorSkinsDetails : UserControl
    {
        public InteriorDoorSkinsDetails()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorSkinDetailsViewModel ViewModel
        {
            get { return (InteriorDoorSkinDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(InteriorDoorSkinDetailsViewModel), typeof(InteriorDoorSkinsDetails), new PropertyMetadata(null));
        #endregion

    }
}
