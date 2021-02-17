using System;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Controls
{
    public sealed partial class DataListPrint : UserControl, INotifyExpressionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DataListPrint()
        {
            this.InitializeComponent();
            DependencyExpressions.Initialize(this);
        }

        static private readonly DependencyExpressions DependencyExpressions = new DependencyExpressions();

        #region ItemsSource*
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataListPrint;
            control.UpdateItemsSource(e.NewValue, e.OldValue);
            DependencyExpressions.UpdateDependencies(control, nameof(ItemsSource));
        }

        private void UpdateItemsSource(object newValue, object oldValue)
        {
            if (oldValue is INotifyCollectionChanged oldSource)
            {
                oldSource.CollectionChanged -= OnCollectionChanged;
            }
            if (newValue is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += OnCollectionChanged;
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataListPrint), new PropertyMetadata(null, ItemsSourceChanged));
        #endregion

        #region SelectedItem
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataList), new PropertyMetadata(null));
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(DataListPrint), new PropertyMetadata(null));
        #endregion

        #region ItemTemplate
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DataListPrint), new PropertyMetadata(null));
        #endregion

        #region ItemsCount
        public int ItemsCount
        {
            get { return (int)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }

        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register(nameof(ItemsCount), typeof(int), typeof(DataListPrint), new PropertyMetadata(0));
        #endregion

        public bool IsDataAvailable => (ItemsSource?.Cast<object>().Any() ?? false);
        static DependencyExpression IsDataAvailableExpression = DependencyExpressions.Register(nameof(IsDataAvailable), nameof(ItemsSource));

        public bool IsDataUnavailable => !IsDataAvailable;
        static DependencyExpression IsDataUnavailableExpression = DependencyExpressions.Register(nameof(IsDataUnavailable), nameof(IsDataAvailable));

        public string DataUnavailableMessage => ItemsSource == null ? "Loading..." : "No items found.";
        static DependencyExpression DataUnavailableMessageExpression = DependencyExpressions.Register(nameof(DataUnavailableMessage), nameof(ItemsSource));

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource is IList list)
            {
                if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    if (ItemsSource is ISelectionInfo selectionInfo)
                    {
                        if (selectionInfo.IsSelected(e.NewStartingIndex))
                        {
                            SelectedItem = list[e.NewStartingIndex];
                            System.Diagnostics.Debug.WriteLine("SelectedItem {0}", SelectedItem);
                        }
                    }
                }
            }
        }

        #region NotifyPropertyChanged
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
