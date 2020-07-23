using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Clicker.AttachedProperties
{
    public static class RadioButtonClicked
    {
        public static readonly DependencyProperty RadioButtonProperty =
            DependencyProperty.RegisterAttached(
                "RadioButton",
                typeof(string),
                typeof(RadioButtonClicked),
                new PropertyMetadata(string.Empty, OnRadioButtonChanged));

        public static void SetRadioButton(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(RadioButtonProperty, value);
        }

        public static string GetRadioButton(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(RadioButtonProperty);
        }

        private static void OnRadioButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButton radioButton = d as RadioButton;

            radioButton.Checked += RadioButton_Checked;
        }

        private static void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            SetRadioButton(radioButton, radioButton.Tag.ToString());
        }
    }
}
