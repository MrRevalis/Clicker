using Clicker.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Clicker.ViewModel
{
    public class MainVM : ViewModelBase
    {
        #region Public Properties
        public string Time { get; set; }
        public double[] Position1 { get; set; }
        public double[] Position2 { get; set; }
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
        public ICommand OnKeyClicked { get; set; }
        #endregion

        public MainVM()
        {
            OnKeyClicked = new RelayParameterizedCommand((parameter) => KeyClickedMethod((KeyEventArgs)parameter));
            Start = new RelayCommand(() => StartMethod());
        }

        public void KeyClickedMethod(KeyEventArgs e)
        {
            MessageBox.Show(e.Key.ToString());
        }

        public void StartMethod()
        {

        }
    }
}