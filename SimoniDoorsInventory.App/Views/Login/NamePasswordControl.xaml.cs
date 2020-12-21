using System;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimoniDoorsInventory.Views
{
    public sealed partial class NamePasswordControl : UserControl
    {
        public NamePasswordControl()
        {
            InitializeComponent();
        }

        #region UserName
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        #region Password
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        #region LoginWithPasswordCommand
        public ICommand LoginWithPasswordCommand
        {
            get { return (ICommand)GetValue(LoginWithPasswordCommandProperty); }
            set { SetValue(LoginWithPasswordCommandProperty, value); }
        }

        public static readonly DependencyProperty LoginWithPasswordCommandProperty = DependencyProperty.Register(nameof(LoginWithPasswordCommand), typeof(ICommand), typeof(NamePasswordControl), new PropertyMetadata(null));
        #endregion

        public void Focus()
        {
            userName.Focus(FocusState.Programmatic);
        }
    }
}
