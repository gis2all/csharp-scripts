using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static Utils.Win32Constants;

namespace Utils
{
    public static class Win32Utils
    {
        #region Mouse

        [DllImport("user32", EntryPoint = "mouse_event")]
        public static extern int MouseEvent(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("User32", EntryPoint = "SetCursorPos")]
        public extern static void SetCursorPos(int x, int y);

        // 获取当前鼠标的屏幕坐标
        [DllImport("User32")]
        public extern static bool GetCursorPos(ref Point cursorPoint);
        #endregion Mouse

        #region Window

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32", EntryPoint = "ShowWindow")]
        public static extern void SetWindowStatus(IntPtr windowPr, int status);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        public static extern void SetWindowPos(IntPtr windowPr, int posStatus, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string cpation);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr child, string className, string cption);

        public static IntPtr FindNextWindowEx(IntPtr parent, IntPtr previous)
        {
            return FindWindowEx(parent, previous, null, null);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);       

        #endregion Window

        #region Control

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr target, int Msg, int aa, int lParam);

        public static void InputStr(IntPtr control, string Input)
        {
            byte[] ch = (ASCIIEncoding.ASCII.GetBytes(Input));
            for (int i = 0; i < ch.Length; i++)
            {
                SendMessage(control, Win32Constants.WM_CHAR, ch[i], 0);
                Thread.Sleep(200);
            }
        }

        #endregion Control

        #region keyboard

        //第一个参数：虚拟键码（键盘键码对照表见附录）；
        //第二个参数：硬件扫描码，一般设置为0即可；
        //第三个参数：函数操作的一个标志位，如果值为KEYEVENTF_EXTENDEDKEY则该键被按下，也可设置为0即可，如果值为KEYEVENTF_KEYUP则该按键被释放；
        //第四个参数：定义与击键相关的附加的32位值，一般设置为0即可。
        [DllImport("User32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void KeyBoard_Event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void OperateKey(byte keyValue, KeyBoardConstant.KeyStatus keyStatus)
        {
            switch (keyStatus)
            {
                case KeyBoardConstant.KeyStatus.Press:
                    KeyBoard_Event(keyValue, 0, KeyBoardConstant.vKey_Press, 0);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                case KeyBoardConstant.KeyStatus.Release:
                    KeyBoard_Event(keyValue, 0, KeyBoardConstant.vKey_Release, 0);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                case KeyBoardConstant.KeyStatus.Click:
                    KeyBoard_Event(keyValue, 0, KeyBoardConstant.vKey_Press, 0);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    KeyBoard_Event(keyValue, 0, KeyBoardConstant.vKey_Release, 0);
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    break;
                default:
                    break;
            }

            //KeyBoard_Event(KeyBoardConstant.vKeyLeftWin, 0, KeyBoardConstant.vKey_Press, 0);
            //Console.WriteLine(KeyBoardConstant.vKeyLeftWin);
            //Thread.Sleep(TimeSpan.FromSeconds(1));

            //for (int i = 0; i < 10; i++)
            //{
            //    KeyBoard_Event(KeyBoardConstant.vKeySpace, 0, KeyBoardConstant.vKeySpace, 0);
            //    Thread.Sleep(TimeSpan.FromMilliseconds(10));
            //    KeyBoard_Event(KeyBoardConstant.vKeySpace, 0, KeyBoardConstant.vKey_Release, 0);
            //    Thread.Sleep(TimeSpan.FromMilliseconds(2000));
            //}
            //KeyBoard_Event(KeyBoardConstant.vKeyLeftWin, 0, KeyBoardConstant.vKey_Release, 0);
        }
        #endregion
    }
}
