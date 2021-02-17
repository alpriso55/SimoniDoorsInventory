using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Controls
{
    public sealed partial class DetailsPrint : UserControl, INotifyExpressionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DetailsPrint()
        {
            this.InitializeComponent();

            Loaded += OnLoaded;
            DependencyExpressions.Initialize(this);
        }

        static private readonly DependencyExpressions DependencyExpressions = new DependencyExpressions();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var ctrl in GetFormControls())
            {
                ctrl.VisualStateChanged += OnVisualStateChanged;
            }
            UpdateEditMode();
        }

        #region IsEditMode*
        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        private static void IsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DetailsPrint;
            DependencyExpressions.UpdateDependencies(control, nameof(IsEditMode));
            control.UpdateEditMode();
        }

        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(DetailsPrint), new PropertyMetadata(false, IsEditModeChanged));
        #endregion

        #region DetailsContent
        public object DetailsContent
        {
            get { return (object)GetValue(DetailsContentProperty); }
            set { SetValue(DetailsContentProperty, value); }
        }

        public static readonly DependencyProperty DetailsContentProperty = DependencyProperty.Register(nameof(DetailsContent), typeof(object), typeof(DetailsPrint), new PropertyMetadata(null));
        #endregion

        #region DetailsTemplate
        public DataTemplate DetailsTemplate
        {
            get { return (DataTemplate)GetValue(DetailsTemplateProperty); }
            set { SetValue(DetailsTemplateProperty, value); }
        }

        public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(nameof(DetailsTemplate), typeof(DataTemplate), typeof(DetailsPrint), new PropertyMetadata(null));
        #endregion

        #region DefaultCommands
        public string DefaultCommands
        {
            get { return (string)GetValue(DefaultCommandsProperty); }
            set { SetValue(DefaultCommandsProperty, value); }
        }

        public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(DetailsPrint), new PropertyMetadata("edit"));
        #endregion

        #region EditCommand
        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(DetailsPrint), new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            GetFormControls().FirstOrDefault()?.Focus(FocusState.Programmatic);
        }

        private void OnVisualStateChanged(object sender, FormVisualState e)
        {
            if (e == FormVisualState.Focused)
            {
                if (!IsEditMode)
                {
                    EditCommand?.TryExecute();
                }
            }
        }

        private void UpdateEditMode()
        {
            if (IsEditMode)
            {
                foreach (var ctrl in GetFormControls().Where(r => r.VisualState != FormVisualState.Focused))
                {
                    ctrl.SetVisualState(FormVisualState.Ready);
                }
            }
            else
            {
                Focus(FocusState.Programmatic);
                foreach (var ctrl in GetFormControls())
                {
                    ctrl.SetVisualState(FormVisualState.Idle);
                }
            }
        }

        private IEnumerable<IFormControl> GetFormControls()
        {
            return ElementSet.Children<Control>(container)
                .Where(r =>
                {
                    if (r is IFormControl ctrl)
                    {
                        return true;
                    }
                    return false;
                })
                .Cast<IFormControl>();
        }

        #region NotifyPropertyChanged
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
