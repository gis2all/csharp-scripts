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
            var cmdWindow = Win32Utils.GetConsoleWindow();
            Win32Utils.ShowWindow(cmdWindow, Win32Constants.SW_HIDE);
            int i = 1;
            // 死循环保持屏幕常量
            while (true)
            {
                if (i % 2 == 0)
                {
                    Win32Utils.MouseEvent(Win32Constants.MOUSEEVENTF_MOVE, 2, 0, 0, 0);


                }
                else
                {
                    Win32Utils.MouseEvent(Win32Constants.MOUSEEVENTF_MOVE, -2, 0, 0, 0);
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
            var arrowPoint = new Point(0, 0);
            Win32Utils.GetCursorPos(ref arrowPoint);
            // 超出边界则回到屏幕中心
            if (arrowPoint.X > 10 && arrowPoint.X <= screenSize.Width - 10)
            {
                return;
            }
            else
            {
                arrowPoint.X = screenSize.Width / 2;
            }
            Win32Utils.MouseEvent(Win32Constants.MOUSEEVENTF_MOVE, screenSize.Width / 2, 0, 0, 0);
        }
    }
}
