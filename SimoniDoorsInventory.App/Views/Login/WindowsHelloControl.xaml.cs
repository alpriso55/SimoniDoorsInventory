using System;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class WindowsHelloControl : UserControl
    {
        public WindowsHelloControl()
        {
            InitializeComponent();
        }

        #region UserName
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(WindowsHelloControl), new PropertyMetadata(null));
        #endregion

        #region LoginWithWindowHelloCommand
        public ICommand LoginWithWindowHelloCommand
        {
            get { return (ICommand)GetValue(LoginWithWindowHelloCommandProperty); }
            set { SetValue(LoginWithWindowHelloCommandProperty, value); }
        }

        public static readonly DependencyProperty LoginWithWindowHelloCommandProperty = DependencyProperty.Register(nameof(LoginWithWindowHelloCommand), typeof(ICommand), typeof(WindowsHelloControl), new PropertyMetadata(null));
        #endregion
    }
}
