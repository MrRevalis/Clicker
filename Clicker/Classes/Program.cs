using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker.Methods
{
    public class Program
    {
        public Icon AppIcon { get; set; }
        public string AppName { get; set; }
    
        public Program(Icon icon, string name)
        {
            AppIcon = icon;
            AppName = name;
        }

        public override string ToString()
        {
            return AppName;
        }
    }
}
