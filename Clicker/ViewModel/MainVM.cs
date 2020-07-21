using Clicker.Methods;
using Clicker.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
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
        
        #region Public Properties
        public string Time { get; set; }
        public string stringPosition1 { get; set; }
        public string stringPosition2 { get; set; }
        public bool IsActive { get; set; } = true;
        public CancellationToken Token { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public INPUT MouseInput { get; set; }
        public ObservableCollection<Position> MousePosition { get; set; }
        public ObservableCollection<Program> ProcessList { get; set; }
        public Key KeyPressed { get; set; }
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
        public ICommand OnKeyClicked { get; set; }
        #endregion
        public MainVM()
        {
            OnKeyClicked = new RelayCommand(() => KeyClickedMethod());
            Start = new RelayCommand(() => StartMethod());
            Stop = new RelayCommand(() => StopMethod());

            INPUT[] inputMouse = new INPUT[2];
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

            

            MousePosition = new ObservableCollection<Position>();
            ProcessList = GetListOfProcesses();

            TokenSource = new CancellationTokenSource();
            Token = TokenSource.Token;
            Task refreshProcesses = Task.Run(() => RefreshListOfProcesses(Token), Token);
        }

        private ObservableCollection<Program> GetListOfProcesses()
        {
            ObservableCollection<Program> listOfProcesses = new ObservableCollection<Program>();
            var query = "SELECT ProcessId, Name, ExecutablePath FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(query))
            using (var results = searcher.Get())
            {
                var processes = results.Cast<ManagementObject>().Select(x => new
                {
                    ProcessId = (UInt32)x["ProcessId"],
                    Name = (string)x["Name"],
                    ExecutablePath = (string)x["ExecutablePath"]
                });
                foreach (var p in processes)
                {
                    if (System.IO.File.Exists(p.ExecutablePath))
                    {
                        Icon icon = Icon.ExtractAssociatedIcon(p.ExecutablePath);
                        try
                        {
                            if (Process.GetProcessById((int)p.ProcessId).MainWindowHandle != IntPtr.Zero)
                            {
                                listOfProcesses.Add(new Program(icon, p.Name, p.ProcessId, p.ExecutablePath));
                            }
                        }
                        catch(Exception e) { }
                    }
                }
            }
            MessageBox.Show(listOfProcesses.Count().ToString());
            return listOfProcesses;
        }

        private Task RefreshListOfProcesses(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                Thread.Sleep(3000);
            }
        }

        public void KeyClickedMethod()
        {
            if (KeyPressed == Key.F1)
            {
                POINT position;
                if (GetCursorPos(out position))
                    MousePosition.Add(new Position(position.X, position.Y));
            }
            else if (KeyPressed == Key.F5)
            {
                StopMethod();
            }
        }

        public void StartMethod()
        {
            TokenSource.Cancel();
        }
        private void StartClicking(CancellationToken cancellationToken)
        {
            int timeOfWait = int.Parse(Time);
            while (!cancellationToken.IsCancellationRequested)
            {
                Position startPosition = MousePosition[0];
                SetCursorPos(startPosition.X, startPosition.Y);
                foreach (Position x in MousePosition)
                {
                    SmoothMouseMove(startPosition, x, 50, timeOfWait);

                }
            }
        }
        private void StartClicking()
        {
            /*if (Position1 != System.Drawing.Point.Empty && Position2 != System.Drawing.Point.Empty && Time != null)
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
            }*/
            MessageBox.Show("siema");
        }
        private void StopMethod()
        {
            IsActive = false;
            OnPropertyChanged(nameof(IsActive));
        }
        private void SmoothMouseMove(Position oldPosition, Position newPosition, int steps, int time)
        {
            Position pointDifference = new Position(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y);
            pointDifference.X /= steps;
            pointDifference.Y /= steps;
            Random random = new Random();
            for (int i = 0; i < steps; i++)
            {
                oldPosition = new Position(oldPosition.X + pointDifference.X, oldPosition.Y + pointDifference.Y);
                SetCursorPos(oldPosition.X, oldPosition.Y);
                Thread.Sleep(10);
            }
            SetCursorPos(oldPosition.X + random.Next(-5, 5), oldPosition.Y + random.Next(-5, 5));
            Thread.Sleep(time - (steps * 5));
        }
    }
}