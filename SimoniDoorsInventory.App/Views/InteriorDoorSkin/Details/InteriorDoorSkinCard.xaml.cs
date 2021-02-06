using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorSkinCard : UserControl
    {
        public InteriorDoorSkinCard()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorSkinDetailsViewModel ViewModel
        {
            get { return (InteriorDoorSkinDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(InteriorDoorSkinDetailsViewModel), typeof(InteriorDoorSkinCard), new PropertyMetadata(null));
        #endregion

        #region Item
        public InteriorDoorSkinModel Item
        {
            get { return (InteriorDoorSkinModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(InteriorDoorSkinModel), typeof(InteriorDoorSkinCard), new PropertyMetadata(null));
        #endregion

    }
}
