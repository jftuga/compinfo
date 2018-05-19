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

        public static TimeSpan Uptime() {
            return TimeSpan.FromMilliseconds(GetTickCount64());
            
        }
    }
}
