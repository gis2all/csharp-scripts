using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Utils.Core
{
    // 参考 https://www.developerfusion.com/code/4630/capture-a-screen-shot/                
    public class Screenshot
    {
        // 保存特定窗口截图      
        public void CaptureWindowToFile(IntPtr handle, string fileName, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            FileUtils.CreateDir(fileName);
            img.Save(fileName, format);
        }

        // 保存整个桌面截图
        public void CaptureScreenToFile(string fileName, ImageFormat format)
        {
            Image img = CaptureScreen();
            FileUtils.CreateDir(fileName);
            img.Save(fileName, format);
        }

        public void CaptureAreaToFile(string fileName, int sourceX, int sourceY, int destinationX, int destinationY, Size imageSize, ImageFormat format)
        {
            var screenSize = GetScreenPhysicalSzie();
            Bitmap baseImage = new Bitmap(imageSize.Width, imageSize.Height);
            Graphics imgGraphics = Graphics.FromImage(baseImage);

            // 左上角
            imgGraphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, imageSize);
            imgGraphics.Dispose();

            // 保存截图
            FileUtils.CreateDir(fileName);
            baseImage.Save(fileName, format);
        }

        // 桌面窗口的截图对象
        private Image CaptureScreen()
        {
            return CaptureWindow(Win32Utils.GetDesktopWindow());
        }

        // 特定窗口的截图对象
        private Image CaptureWindow(IntPtr handle)
        {
            // 获得目标窗口的hDC          
            IntPtr hdcSrc = Win32Utils.GetWindowDC(handle);

            var screenSize = GetScreenPhysicalSzie();

            // create a device context we can copy to
            IntPtr hdcDest = GDI32Utils.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32Utils.CreateCompatibleBitmap(hdcSrc, screenSize.Width, screenSize.Height);
            // select the bitmap object
            IntPtr hOld = GDI32Utils.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32Utils.BitBlt(hdcDest, 0, 0, screenSize.Width, screenSize.Height, hdcSrc, 0, 0, GDI32Utils.SRCCOPY);
            // restore selection
            GDI32Utils.SelectObject(hdcDest, hOld);
            // clean up
            GDI32Utils.DeleteDC(hdcDest);
            Win32Utils.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32Utils.DeleteObject(hBitmap);
            return img;
        }

        private static float GetScreenDisplayScale()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = Win32Utils.GetDeviceCaps(desktop, (int)Win32Constants.DeviceCap.VERTRES);
            int PhysicalScreenHeight = Win32Utils.GetDeviceCaps(desktop, (int)Win32Constants.DeviceCap.DESKTOPVERTRES);
            float ScreenScalingFactor = float.Parse(((float)PhysicalScreenHeight / (float)LogicalScreenHeight).ToString("0.00"));
            return ScreenScalingFactor; // 1.25 = 125%
        }

        public static Size GetScreenPhysicalSzie()
        {
            var desktopHwd = Win32Utils.GetDesktopWindow();

            // 获取分辨率，这里是缩放后的尺寸，比如显示器物理尺寸是1920x1080，缩放200%就是960x540
            // 这里得到的是960x540的尺寸
            Win32Utils.RECT windowRect = new Win32Utils.RECT();
            Win32Utils.GetWindowRect(desktopHwd, ref windowRect);

            var factor = GetScreenDisplayScale();

            int width = (int)((windowRect.right - windowRect.left) * factor);
            int height = (int)((windowRect.bottom - windowRect.top) * factor);
            return new Size(width, height);
        }
    }
}
