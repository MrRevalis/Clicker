using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Clicker.AttachedProperties
{
    public static class TimeProperty
    {
        public static readonly DependencyProperty TimeBoxProperty =
            DependencyProperty.RegisterAttached(
                "TimeBox",
                typeof(string),
                typeof(TimeProperty),
                new PropertyMetadata(string.Empty, TimeBoxChanged));

        public static void SetTimeBox(DependencyObject textBox, string value)
        {
            textBox.SetValue(TimeBoxProperty, value);
        }

        public static string GetTimeBox(DependencyObject textBox)
        {
            return (string)textBox.GetValue(TimeBoxProperty);
        }

        private static void TimeBoxChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.MaxLength = 8;
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            SetTimeBox(textBox, textBox.Text.ToString());
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            foreach(char x in e.Text)
            {
                if (!char.IsDigit(x))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
    }
}
