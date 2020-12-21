﻿using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorsList : UserControl
    {
        public InteriorDoorsList()
        {
            InitializeComponent();
        }

        #region ViewModel
        public InteriorDoorListViewModel ViewModel
        {
            get { return (InteriorDoorListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(InteriorDoorListViewModel), typeof(InteriorDoorsList), new PropertyMetadata(null));
        #endregion
    }
}
