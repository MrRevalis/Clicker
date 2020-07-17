using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Clicker.Methods
{
    public class Program
    {
        public ImageSource AppIcon { get; set; }
        public string AppName { get; set; }
        public UInt32 ProcessID { get; set; }
        public string ExecutablePath { get; set; }
        public Program(Icon icon, string name, UInt32 id, string pathEx)
        {
            AppIcon = IconConverter(icon);
            AppName = name;
            ProcessID = id;
            ExecutablePath = pathEx;
        }

        private ImageSource IconConverter(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }
    }
}
