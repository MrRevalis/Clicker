using Clicker.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static Clicker.Methods.Mouse;

namespace Clicker.ViewModel
{
    public class MainVM : ViewModelBase
    {
        #region Import
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs,int cbSize);
        #endregion

        #region Public Properties
        public string Time { get; set; }
        public System.Drawing.Point Position1 { get; set; }
        public System.Drawing.Point Position2 { get; set; }
        public string stringPosition1 { get; set; }
        public string stringPosition2 { get; set; }
        public bool IsActive { get; set; } = true;
        public INPUT[] inputMouse { get; set; }
        public Key KeyPressed { get; set; }
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
        public ICommand OnKeyClicked { get; set; }
        #endregion

        #region Private
        private Thread t;
        #endregion
        public MainVM()
        {
            OnKeyClicked = new RelayCommand(() => KeyClickedMethod());
            Start = new RelayCommand(() => StartMethod());
            Stop = new RelayCommand(() => StopMethod());

            inputMouse = new INPUT[2];
            for (int i = 0; i < 2; i++)
            {
                inputMouse[i].type = 0;
                inputMouse[i].mouseInput.dx = 0;
                inputMouse[i].mouseInput.dy = 0;
                inputMouse[i].mouseInput.dwFlags = MOUSEEVENTF_RIGHTDOWN;
                inputMouse[i].mouseInput.dwExtraInfo = IntPtr.Zero;
                inputMouse[i].mouseInput.mouseData = 0;
                inputMouse[i].mouseInput.time = 0;
            }
        }

        public void KeyClickedMethod()
        {
            if (KeyPressed == Key.F1)
            {
                Position1 = System.Windows.Forms.Control.MousePosition;

                stringPosition1 = $"x => {Position1.X}, y=> {Position1.Y}";
                OnPropertyChanged(nameof(stringPosition1));
            }
            else if (KeyPressed == Key.F2)
            {
                Position2 = System.Windows.Forms.Control.MousePosition;

                stringPosition2 = $"x => {Position2.X}, y=> {Position2.Y}";
                OnPropertyChanged(nameof(stringPosition2));
            }
            else if (KeyPressed == Key.F5)
            {
                StopMethod();
            }
        }

        public void StartMethod()
        {
            t = new Thread(new ThreadStart(StartClicking));
            t.IsBackground = true;
            t.Start();
        }

        private void StartClicking()
        {
            if (Position1 != System.Drawing.Point.Empty && Position2 != System.Drawing.Point.Empty && Time != null)
            {
                IsActive = true;
                int timeOfWait = int.Parse(Time);
                SetCursorPos(Position1.X, Position1.Y);
                while (IsActive)
                {
                    SmoothMouseMove(Position1, Position2, 50, timeOfWait);
                    Thread.Sleep(100);
                    inputMouse[0].mouseInput.dwFlags = MOUSEEVENTF_RIGHTDOWN;
                    SendInput(1, ref inputMouse[0], Marshal.SizeOf(inputMouse[0]));
                    inputMouse[0].mouseInput.dwFlags = MOUSEEVENTF_RIGHTUP;
                    SendInput(1, ref inputMouse[0], Marshal.SizeOf(inputMouse[0]));

                    SmoothMouseMove(Position2, Position1, 50, timeOfWait);
                    Thread.Sleep(100);
                    inputMouse[1].mouseInput.dwFlags = MOUSEEVENTF_RIGHTDOWN;
                    SendInput(1, ref inputMouse[1], Marshal.SizeOf(inputMouse[1]));
                    inputMouse[1].mouseInput.dwFlags = MOUSEEVENTF_RIGHTUP;
                    SendInput(1, ref inputMouse[1], Marshal.SizeOf(inputMouse[1]));
                }
            }
        }
        private void StopMethod()
        {
            IsActive = false;
            OnPropertyChanged(nameof(IsActive));
        }
        private void SmoothMouseMove(System.Drawing.Point oldPosition, System.Drawing.Point newPosition, int steps, int time)
        {
            System.Drawing.Point pointDifference = new System.Drawing.Point(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y);
            pointDifference.X /= steps;
            pointDifference.Y /= steps;
            Random random = new Random();
            for (int i = 0; i < steps; i++)
            {
                oldPosition = new System.Drawing.Point(oldPosition.X + pointDifference.X, oldPosition.Y + pointDifference.Y);
                SetCursorPos(oldPosition.X, oldPosition.Y);
                Thread.Sleep(10);
            }
            SetCursorPos(oldPosition.X + random.Next(-5, 5), oldPosition.Y + random.Next(-5, 5));
            Thread.Sleep(time - (steps * 5));
        }
    }
}