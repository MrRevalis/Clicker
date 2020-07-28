using Clicker.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Clicker.Pages
{
    public class BasePage<VM> : Page where VM : ViewModelBase, new()
    {
        #region Private Member
        private VM mViewModel;
        #endregion

        #region Public Properties
        public VM ViewModel
        {
            get { return mViewModel; }
            set
            {
                if (mViewModel == value)
                    return;
                mViewModel = value;
                this.DataContext = mViewModel;
            }
        }
        #endregion

        #region Constructor
        public BasePage()
        {
            this.ViewModel = new VM();
        }
        #endregion
    }
}
