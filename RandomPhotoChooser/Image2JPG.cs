using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPhotoChooser
{
    internal class Image2JPG
    {
        internal static void Convert(string file, string outputFile)
        {
            Bitmap bitmap = new Bitmap(file);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 10L);
            bitmap.Save(outputFile, jgpEncoder, encoderParameters);
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
