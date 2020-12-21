using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorsDetails : UserControl
    {
        public InteriorDoorsDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorDetailsViewModel ViewModel
        {
            get { return (InteriorDoorDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(InteriorDoorDetailsViewModel), typeof(InteriorDoorsDetails), new PropertyMetadata(null));
        #endregion
    }
}
