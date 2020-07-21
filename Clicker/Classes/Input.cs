using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.Methods
{
    //https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
    public class Input
    {
        private int _dx { get; } = 0;
        private int _dy { get; } = 0;
        private int _mouseData { get; } = 0;
        private int _time { get; } = 0;
        private IntPtr _dwExtraInfo { get; set; }
        private uint _type { get; } = 0;

        public int DwFlags { get; set; }

        public Input()
        {

        }

        public void 

    }
}
