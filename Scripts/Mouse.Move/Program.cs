using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using Utils;

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
            Win32Utils.ShowWindow(cmdWindow, Win32Constants.SW_SHOW);
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
                // 每隔5分钟移动鼠标
                Thread.Sleep(TimeSpan.FromMinutes(5));
                i++;
            }
        }
    }
}
