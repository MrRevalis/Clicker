using System;
using System.Windows;
using System.Windows.Controls;

namespace Clicker.AttachedProperties
{
    public static class ButtonIsRunningProperty
    {
        public static readonly DependencyProperty ButtonRunningProperty =
            DependencyProperty.RegisterAttached(
                "ButtonRunning",
                typeof(bool),
                typeof(ButtonIsRunningProperty),
                new PropertyMetadata(false, OnButtonRunning));

        public static void SetButtonRunning(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ButtonRunningProperty, value);
        }

        public static bool GetButtonRunning(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(ButtonRunningProperty);
        }


        private static void OnButtonRunning(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Button x = sender as Button;

            x.Click += X_Click;
        }

        private static void X_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
