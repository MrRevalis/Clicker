using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Input;
namespace Clicker.ViewModel
{
    using ViewModel.Base;
    public class MainVM : ViewModelBase
    {
        #region Imports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        #endregion

        #region Private Methods
        private Window mWindow;

        private int mOuterMarginSize = 10;
        private int mWindowRadius = 10;
        private ApplicationPage currentPage = ApplicationPage.Clicker;
        #endregion

        #region Commands
        public ICommand MinimalizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Public Properties
        public string TextBlockText { get; } = "MrRevalis";
        public int ResizeBorder { get; set; } = 6;
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + mOuterMarginSize); } }
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(mWindowRadius); } }
        public Thickness WindowPadding { get { return new Thickness(mOuterMarginSize); } }
        public double TitleBarHeight { get; set; } = 30;
        public GridLength TitleBarGridHeight { get { return new GridLength(TitleBarHeight + ResizeBorder); } }
        #endregion

        #region Constructor
        public MainVM(Window window)
        {
            mWindow = window;

            MinimalizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, MousePosition()));
        }
        #endregion

        #region Methods
        private Point MousePosition()
        {
            POINT point;
            GetCursorPos(out point);
            return new Point(point.X, point.Y);
        }
        #endregion
    }
}