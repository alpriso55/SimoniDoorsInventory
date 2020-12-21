using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.Models;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorCard : UserControl
    {
        public InteriorDoorCard()
        {
            InitializeComponent();
        }

        #region Item
        public InteriorDoorModel Item
        {
            get { return (InteriorDoorModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(InteriorDoorModel), typeof(InteriorDoorCard), new PropertyMetadata(null));
        #endregion
    }
}
