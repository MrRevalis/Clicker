using Clicker.Methods;
using Clicker.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
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

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs,int cbSize);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In] UInt32 dwFlags, [In] UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
     ProcessAccessFlags processAccess,
     bool bInheritHandle,
     int processId
);
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, proc.Id);
        }
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        #endregion

        #region Public Properties
        public string Time { get; set; }
        public System.Drawing.Point Position1 { get; set; }
        public System.Drawing.Point Position2 { get; set; }
        public string stringPosition1 { get; set; }
        public string stringPosition2 { get; set; }
        public bool IsActive { get; set; } = true;
        public INPUT[] inputMouse { get; set; }
        public ObservableCollection<Position> MousePosition { get; set; }
        public ObservableCollection<Program> ProcessList { get; set; }
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

            MousePosition = new ObservableCollection<Position>();
            ProcessList = GetListOfProcesses();
        }

        private ObservableCollection<Program> GetListOfProcesses()
        {
            ObservableCollection<Program> listOfProcesses = new ObservableCollection<Program>();

            //Process process = null;
            IntPtr handle = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 processEntry = new PROCESSENTRY32();
                processEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handle = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (Process32First(handle, ref processEntry))
                {
                    do
                    {
                        //listOfProcesses.Add(new Program(null, processEntry.szExeFile));
                        IntPtr hProcess = OpenProcess(ProcessAccessFlags.All, false, Convert.ToInt32(processEntry.th32ProcessID));
                        if(hProcess != null)
                        {
                            string path = string.Empty;
                            
                        }
                        CloseHandle(hProcess);
                    } while (Process32Next(handle, ref processEntry));
                }
            }
            catch(Exception e)
            {
                throw new ApplicationException($"Error process => {e}");
            }
            finally
            {
                CloseHandle(handle);
            }
            return listOfProcesses;
        }

        public void KeyClickedMethod()
        {
            if (KeyPressed == Key.F1)
            {
                System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
                MousePosition.Add(new Position(point.X, point.Y));
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
                    /*SmoothMouseMove(Position1, Position2, 50, timeOfWait);
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
                    SendInput(1, ref inputMouse[1], Marshal.SizeOf(inputMouse[1]));*/
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