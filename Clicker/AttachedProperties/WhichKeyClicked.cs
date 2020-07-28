using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Clicker.AttachedProperties
{
    public static class WhichKeyClicked
    {
        public static readonly DependencyProperty WhichKeyProperty =
            DependencyProperty.RegisterAttached(
                "WhichKey",
                typeof(Key),
                typeof(WhichKeyClicked),
                new PropertyMetadata(Key.LeftCtrl, OnWhichKeyChanged));

        public static void SetWhichKey(DependencyObject _object, Key value)
        {
            _object.SetValue(WhichKeyProperty, value);
        }

        public static Key GetWhichKey(DependencyObject _object)
        {
            return (Key)_object.GetValue(WhichKeyProperty);
        }

        public static void OnWhichKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Page window = sender as Page;

            window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private static void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Page window = sender as Page;
            SetWhichKey(window, e.Key);
        }
    }
}
