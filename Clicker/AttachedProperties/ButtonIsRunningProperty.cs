using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Clicker.AttachedProperties
{
    public static class ButtonIsRunningProperty
    {
        public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.RegisterAttached(
                "Button",
                typeof(bool),
                typeof(ButtonIsRunningProperty),
                new PropertyMetadata(false, OnButtonPropertyChanged)
                );

        public static void SetButton(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ButtonProperty, value);
        }

        public static bool GetButton(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ButtonProperty);
        }


        private static void OnButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
