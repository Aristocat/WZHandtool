using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WZHandtool
{
    public static class BinaryTool
    {
        public static void WriteString(this BinaryWriter Writer, string str)
        {
            if (str == null)
                str = string.Empty;
            Writer.Write((short)Encoding.UTF32.GetBytes(str).Length);
            Writer.Write(Encoding.UTF32.GetBytes(str));
        }

        public static string ReadString(this BinaryReader Reader, bool custom)
        {
            string str = string.Empty;
            short length = Reader.ReadInt16();
            for (int i = 0; i < length; i++)
            {
                int k = Reader.ReadByte();
                char c = (char)k;
                if (k == 0)
                    c = Convert.ToChar(Reader.ReadUInt16());
                str += c.ToString();
            }
            return str;
        }

        public static void WriteBitmap(this BinaryWriter Writer, Bitmap bmp)
        {
            if (bmp == null)
                bmp = new Bitmap(1, 1);
            Writer.Write(bmp.ToByteArray(ImageFormat.Png).Length);
            Writer.Write(bmp.ToByteArray(ImageFormat.Png));
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            return ms.ToArray();
        }

        public static Image ToImage(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);
            return image;
        }
    }
}
