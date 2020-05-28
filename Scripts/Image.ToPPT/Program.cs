using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using Utils;

namespace Image.ToPPT
{
    class Program
    {
        static void Main(string[] args)
        {
            var imageFile = @"C:\Users\chao9441\Desktop\image.png";
            var outPPT = @"C:\Users\chao9441\Desktop\bb";

            // Create PPT Class
            Application pptApp = new Application();
            Presentations pptPres = pptApp.Presentations;
            Presentation pptPre = pptPres.Add(MsoTriState.msoFalse);


            var slide = pptPre.Slides.Add(1, PpSlideLayout.ppLayoutPictureWithCaption);
            var size = ImageHelper.GetImageSize(imageFile);

            
            var shape = slide.Shapes.AddPicture(imageFile, MsoTriState.msoTrue, MsoTriState.msoTrue, 0, 0, size.Width, size.Height);
            shape.Width = size.Width;
            shape.Height = size.Height;
            shape.Left = 0;
            shape.Top = 0;

            Thread.Sleep(4000);

            // Save it as a ppt file
            pptPre.SaveAs(outPPT);



        }
    }
}
