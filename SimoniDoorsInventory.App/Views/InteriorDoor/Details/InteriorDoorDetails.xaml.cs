using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorDetails : UserControl
    {
        public InteriorDoorDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorDetailsViewModel ViewModel
        {
            get { return (InteriorDoorDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(InteriorDoorDetailsViewModel), typeof(InteriorDoorDetails), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }

    }
}
