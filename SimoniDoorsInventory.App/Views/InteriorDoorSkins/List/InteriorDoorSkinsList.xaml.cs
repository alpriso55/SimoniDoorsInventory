using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorSkinsList : UserControl
    {
        public InteriorDoorSkinsList()
        {
            this.InitializeComponent();
        }
        
        #region ViewModel
        public InteriorDoorSkinListViewModel ViewModel
        {
            get { return (InteriorDoorSkinListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(InteriorDoorSkinListViewModel), typeof(InteriorDoorSkinsList), new PropertyMetadata(null));
        #endregion

    }
}
