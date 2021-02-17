using System;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SimoniDoorsInventory.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimoniDoorsInventory.Views
{
    public sealed partial class InteriorDoorSkinsPane : UserControl
    {
        public InteriorDoorSkinsPane()
        {
            this.InitializeComponent();

            // ThisPersonViewModel = new PersonViewModel();
        }

        public PersonViewModel ThisPersonViewModel { get; set; }

        #region ItemsSource
        public IList<InteriorDoorSkinModel> ItemsSource
        {
            get { return (IList<InteriorDoorSkinModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<InteriorDoorSkinModel>), typeof(InteriorDoorSkinsPane), new PropertyMetadata(null));
        #endregion
    }

    public class Person
    {
        public string Name { get; set; }

        public double Height { get; set; }
    }

    public class PersonViewModel
    {
        public List<Person> Data { get; set; }

        public PersonViewModel()
        {
            Data = new List<Person>()
            {
                new Person { Name = "David", Height = 180 },
                new Person { Name = "Michael", Height = 170 },
                new Person { Name = "Steve", Height = 160 },
                new Person { Name = "Joel", Height = 182 }
            };
        }
    }

}
