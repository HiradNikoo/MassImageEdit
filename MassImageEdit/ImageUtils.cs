using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MassImageEdit
{
    public class ImageUtils
    {

        public static void SaveImage(string sourcePath, string targetPath, int width, int quality)
        {
            using (var sourceImage = System.Drawing.Image.FromFile(sourcePath))
            {
                var height = (sourceImage.Height * width) / sourceImage.Width;
                using (var image = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(image))
                    {
                        g.DrawImage(sourceImage, new Rectangle(0, 0, width, height));
                    }

                    ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    encoderParams.Param[0] = qualityParam;

                    image.Save(targetPath, jpegEncoder, encoderParams);
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

    }
}
