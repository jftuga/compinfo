using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compinfo
{
    class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        extern static uint GetTickCount64();

        public static string Uptime() {
            TimeSpan ts = TimeSpan.FromMilliseconds(GetTickCount64());
            return String.Format("{0}.{1:D2}:{2:D2}", ts.Days, ts.Hours, ts.Minutes);
            
        }
    }
}
