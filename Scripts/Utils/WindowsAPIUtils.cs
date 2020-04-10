using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Utils
{
    public static class WindowsAPIUtils
    {
        public static int SRCCOPY { get { return 0x00CC0020; } } // BitBlt dwRop parameter

        public enum KeyStatus
        {
            Press,
            Click,
            Release
        }

        public static void OperateKey(User32.VirtualKey virtualKey, KeyStatus keyStatus)
        {
            // keybd_event
            //第一个参数：虚拟键码
            //第二个参数：硬件扫描码，一般设置为0即可；
            //第三个参数：函数操作的一个标志位，如果值为KEYEVENTF_EXTENDEDKEY则该键被按下，也可设置为0即可，如果值为KEYEVENTF_KEYUP则该按键被释放；
            //第四个参数：定义与击键相关的附加的32位值，一般设置为0即可。              
            switch (keyStatus)
            {
                case KeyStatus.Press:
                    User32.keybd_event(virtualKey.ToByte(), 0, User32.KEYEVENTF.KEYEVENTF_UNICODE, IntPtr.Zero);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                case KeyStatus.Release:
                    User32.keybd_event(virtualKey.ToByte(), 0, User32.KEYEVENTF.KEYEVENTF_KEYUP, IntPtr.Zero);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                case KeyStatus.Click:
                    User32.keybd_event(virtualKey.ToByte(), 0, User32.KEYEVENTF.KEYEVENTF_UNICODE, IntPtr.Zero);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    User32.keybd_event(virtualKey.ToByte(), 0, User32.KEYEVENTF.KEYEVENTF_KEYUP, IntPtr.Zero);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                default:
                    break;
            }
        }

        public static byte ToByte(this User32.VirtualKey virtualKey)
        {
            return byte.Parse(((int)virtualKey).ToString());
        }
    }
}
