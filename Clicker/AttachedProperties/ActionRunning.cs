using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Clicker.AttachedProperties
{
    public static class ActionRunning
    {
        public static readonly DependencyProperty ActionIsRunningProperty =
            DependencyProperty.RegisterAttached(
                "ActionIsRunning",
                typeof(bool),
                typeof(ActionRunning),
                new PropertyMetadata(default(bool), ActionIsRunningChanged)
                );


        public static void SetActionIsRunning(DependencyObject _object, bool value)
        {
            _object.SetValue(ActionIsRunningProperty, value);
        }
        public static bool GetActionIsRunning(DependencyObject _object, bool value)
        {
            return (bool)_object.GetValue(ActionIsRunningProperty);
        }

        private static void ActionIsRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
