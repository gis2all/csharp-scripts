using PInvoke;
using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using Utils;
using Utils.Core;

namespace Mouse.Move
{
    public class Program
    {
        /// <summary>
        /// 这个脚本是用来保持测试机器屏幕常量的
        /// </summary>
        static void Main(string[] args)
        {
            // 隐藏或显示cmd窗口            
            var cmdWindow = Kernel32.GetConsoleWindow();
            User32.ShowWindow(cmdWindow, User32.WindowShowStyle.SW_HIDE);
            int i = 1;
            // 死循环保持屏幕常量
            while (true)
            {
                if (i % 2 == 0)
                {
                    User32.mouse_event(User32.mouse_eventFlags.MOUSEEVENTF_MOVE, 2, 0, 0, IntPtr.Zero);
                }
                else
                {
                    User32.mouse_event(User32.mouse_eventFlags.MOUSEEVENTF_MOVE, -2, 0, 0, IntPtr.Zero);
                }
                KeepInScreen();
                // 每隔5分钟移动鼠标
                Thread.Sleep(TimeSpan.FromMinutes(5));
                i++;
            }
        }

        private static void KeepInScreen()
        {
            var screenSize = Screenshot.GetScreenPhysicalSzie();
            PInvoke.POINT arrowPoint;
            User32.GetCursorPos(out arrowPoint);
            // 超出边界则回到屏幕中心
            if (arrowPoint.x > 10 && arrowPoint.x <= screenSize.Width - 10)
            {
                return;
            }
            else
            {
                arrowPoint.x = screenSize.Width / 2;
            }
            User32.mouse_event(User32.mouse_eventFlags.MOUSEEVENTF_MOVE, screenSize.Width / 2, 0, 0, IntPtr.Zero);
        }
    }
}
