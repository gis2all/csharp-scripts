using PInvoke;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using static PInvoke.Gdi32;
using static PInvoke.User32;

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
            return CaptureWindow(User32.GetDesktopWindow());

        }

        // 特定窗口的截图对象
        private Image CaptureWindow(IntPtr handle)
        {
            // 获得目标窗口的hDC          
            SafeDCHandle hdcSrc = User32.GetWindowDC(handle);
            var screenSize = GetScreenPhysicalSzie();
            // create a device context we can copy to
            var hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, screenSize.Width, screenSize.Height);
            // select the bitmap object
            IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            Gdi32.BitBlt(hdcDest.HWnd, 0, 0, screenSize.Width, screenSize.Height, hdcSrc.HWnd, 0, 0, WindowsAPIUtils.SRCCOPY);
            // restore selection
            Gdi32.SelectObject(hdcDest, hOld);
            // clean up
            Gdi32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc.HWnd);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            Gdi32.DeleteObject(hBitmap);
            return img;
        }

        private static float GetScreenDisplayScale()
        {

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            var screenSafeDC = new SafeDCHandle(IntPtr.Zero, desktop);
            int LogicalScreenHeight = Gdi32.GetDeviceCaps(screenSafeDC, DeviceCap.VERTRES);
            int PhysicalScreenHeight = Gdi32.GetDeviceCaps(screenSafeDC, DeviceCap.DESKTOPVERTRES);
            float ScreenScalingFactor = float.Parse(((float)PhysicalScreenHeight / (float)LogicalScreenHeight).ToString("0.00"));
            return ScreenScalingFactor; // 1.25 = 125%
        }

        public static Size GetScreenPhysicalSzie()
        {
            var desktopHwd = User32.GetDesktopWindow();

            // 获取分辨率，这里是缩放后的尺寸，比如显示器物理尺寸是1920x1080，缩放200%就是960x540
            // 这里得到的是960x540的尺寸
            // User32.layout            
            RECT windowRect = new RECT();
            User32.GetWindowRect(desktopHwd, out windowRect);

            var factor = GetScreenDisplayScale();
            int width = (int)((windowRect.right - windowRect.left) * factor);
            int height = (int)((windowRect.bottom - windowRect.top) * factor);
            return new Size(width, height);
        }
    }
}
