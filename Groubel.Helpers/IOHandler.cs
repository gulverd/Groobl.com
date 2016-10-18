using System;
using System.Web;
using System.IO;

namespace Groubel.Helpers
{
    public class IOHandler
    {
        private string _destination;

        private int? _width;

        private int? _height;

        public IOHandler(string destination)
        {
            _destination = destination;
        }

        private string GetName(string fileName)
        {
            var time = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

            fileName = fileName.Replace(" ", "_");

            fileName = time + "_" + fileName;

            return fileName;

        }

        public string Save(HttpPostedFileBase file)
        {
            if (file != null)
            {

                string extension = Path.GetExtension(file.FileName);
                string newName = GetName(Path.GetFileNameWithoutExtension(file.FileName)) +"."+ extension;
                string renamedImage = HttpContext.Current.Server.MapPath(_destination + newName);

                file.SaveAs(renamedImage);

                return _destination + newName;
            }

            return "";
        }

        public void Delete(string name)
        {
            var filePath = _destination + name;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }

        //private Image ResizeImage(Image original, int targetWidth)
        //{
        //    double percent = (double)original.Width / targetWidth;
        //    int destWidth = (int)(original.Width / percent);
        //    int destHeight = (int)(original.Height / percent);

        //    Bitmap b = new Bitmap(destWidth, destHeight);
        //    Graphics g = Graphics.FromImage((Image)b);
        //    try
        //    {
        //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        g.SmoothingMode = SmoothingMode.HighQuality;
        //        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        g.CompositingQuality = CompositingQuality.HighQuality;

        //        g.DrawImage(original, 0, 0, destWidth, destHeight);
        //    }
        //    finally
        //    {
        //        g.Dispose();
        //    }

        //    return (Image)b;
        //}
    }
}
