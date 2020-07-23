using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Clicker.ViewModel
{
    using Clicker.Classes;
    using Clicker.Methods;
    using Clicker.ViewModel.Base;
    using static Clicker.Methods.MouseInput;
    public class MainVM : ViewModelBase
    {
        #region Import
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

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
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);
        #endregion
        #region Public Properties
        public string Time { get; set; }
        public string MouseKey { get; set; }
        public CancellationToken Token { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public INPUT[] MouseInput { get; set; }
        public Program SelectedProgram { get; set; }
        public ObservableCollection<Position> MousePosition { get; set; }
        public ObservableCollection<Program> ProcessList { get; set; }
        public Key KeyPressed { get; set; }
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
        public ICommand OnKeyClicked { get; set; }
        #endregion
        #region Private Methods
        private GlobalKeyboardHook _globalKeyboardHook;
        #endregion
        #region Constructor
        public MainVM()
        {
            OnKeyClicked = new RelayCommand(() => KeyClickedMethod());
            Start = new RelayCommand(() => StartMethod());
            Stop = new RelayCommand(() => StopMethod());

            MouseInput = new INPUT[1];
            MouseInput[0].type = 0;
            MouseInput[0].mouseInput.dx = 0;
            MouseInput[0].mouseInput.dy = 0;
            MouseInput[0].mouseInput.dwFlags = MouseEvent.MOUSEEVENTF_RIGHTDOWN;
            MouseInput[0].mouseInput.dwExtraInfo = IntPtr.Zero;
            MouseInput[0].mouseInput.mouseData = 0;
            MouseInput[0].mouseInput.time = 0;

            MousePosition = new ObservableCollection<Position>();
            ProcessList = GetListOfProcesses();

            _globalKeyboardHook = new GlobalKeyboardHook(new Keys[] { Keys.F5 });
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            Task.Run(RefreshListOfProcesses);
        }
        #endregion
        #region Methods
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
            return listOfProcesses;
        }

        private void RefreshListOfProcesses()
        {
            while (true)
            {
                ObservableCollection<Program> refreshList = GetListOfProcesses();
                if(refreshList.Count != ProcessList.Count)
                {
                    ProcessList = refreshList;
                }
                Thread.Sleep(5000);
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
        }

        public void StartMethod()
        {
            if(SelectedProgram != null && Time != string.Empty && MousePosition.Count >= 1 && MouseKey != string.Empty)
            {
                TokenSource = new CancellationTokenSource();
                Token = TokenSource.Token;
                Task.Run(() => StartClicking(Token), Token);
            }
        }
        private void StartClicking(CancellationToken cancellationToken)
        {
            int timeOfWait = int.Parse(Time);
            Position firstPosition = MousePosition[0];
            SetCursorPos(firstPosition.X, firstPosition.Y);
            int selectedProgramID = (int)SelectedProgram.ProcessID;
            int mouseClick;
            if (MouseKey == "left")
            {
                mouseClick = MouseEvent.MOUSEEVENTF_LEFTDOWN;
            }
            else
            {
                mouseClick = MouseEvent.MOUSEEVENTF_RIGHTDOWN;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Position startPosition = MousePosition[0];
                foreach (Position x in MousePosition)
                {
                    if (Process.GetProcessById(selectedProgramID).MainWindowHandle == IntPtr.Zero)
                    {
                        TokenSource.Cancel();
                    }
                    SmoothMouseMove(startPosition, x, 50, timeOfWait);
                    Thread.Sleep(100);
                    MouseInput[0].mouseInput.dwFlags = mouseClick;
                    SendInput(1, ref MouseInput[0], Marshal.SizeOf(MouseInput[0]));
                    MouseInput[0].mouseInput.dwFlags = mouseClick + 2;
                    SendInput(1, ref MouseInput[0], Marshal.SizeOf(MouseInput[0]));
                    startPosition = x;
                }
                SmoothMouseMove(startPosition, firstPosition, 50, timeOfWait);
            }
        }
        private void StopMethod()
        {
            TokenSource.Cancel();
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
            SetCursorPos(newPosition.X, newPosition.Y);
            Thread.Sleep(time - (steps * 10));
        }
        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                StopMethod();
            }
        }
        #endregion
    }
}