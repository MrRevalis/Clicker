using Clicker.Methods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Clicker.AttachedProperties
{
    public static class ListViewValue
    {
        public static readonly DependencyProperty ListViewProperty =
                    DependencyProperty.RegisterAttached(
                        "ListView",
                        typeof(ObservableCollection<Position>),
                        typeof(ListViewValue),
                        new PropertyMetadata(null, ListViewChanged));

        public static void SetListView(DependencyObject listView, ObservableCollection<Position> value)
        {
            listView.SetValue(ListViewProperty, value);
        }

        public static ObservableCollection<Position> GetListView(DependencyObject listView)
        {
            return (ObservableCollection<Position>)listView.GetValue(ListViewProperty);
        }

        private static void ListViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = sender as ListView;

           
            listView.MouseDoubleClick += ListView_MouseDoubleClick;

            var source = (INotifyCollectionChanged)listView.Items.SourceCollection;
            if (e.NewValue != null)
            {
                NotifyCollectionChangedEventHandler scrollToEndHandler = delegate
                {
                    if (listView.Items.Count <= 0)
                        return;
                    listView.Items.MoveCurrentToLast();
                    listView.ScrollIntoView(listView.Items.CurrentItem);
                };

                source.CollectionChanged += scrollToEndHandler;
            }
        }

        private static void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;

            ObservableCollection<Position> mousePosition = GetListView(listView);
            int index = listView.SelectedIndex;
            if (index >= 0)
                mousePosition.RemoveAt(index);
        }
    }
}