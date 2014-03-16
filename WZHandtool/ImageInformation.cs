using reWZ;
using reWZ.WZProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    public partial class ImageInformation : Form
    {
        private List<Thread> Threads;
        private string ImagePath;
        private bool Animation;

        public delegate void InvokeDelegate(Bitmap pImage);
        public readonly InvokeDelegate InvokeControls;

        public ImageInformation(string pImagePath, bool pAnimation)
        {
            InitializeComponent();
            Threads = new List<Thread>();
            ImagePath = pImagePath;
            Animation = pAnimation;

            InvokeControls = new InvokeDelegate(DisplayImage);
        }

        private void ImageInformation_OnLoad(object sender, EventArgs e)
        {
            if (Program.OldVersion)
            {
                WZFile wz;
                wz = new WZFile(Program.DataLocation + @"\Data.wz", WZVariant.BMS, true);
                if (!Animation)
                    LoadImage(wz, GetImagePath(ImagePath));
                else
                {
                    Thread t = new Thread(new ThreadStart(
                        () =>
                        {
                            while (true)
                            {
                                foreach (WZObject obj in wz.MainDirectory.ResolvePath(GetImagePath(ImagePath)))
                                {
                                    WZObject img = obj;
                                    if (img.Name.Equals("0"))
                                        Thread.Sleep(1000);
                                    if (img is WZUOLProperty)
                                        img = wz.MainDirectory.ResolvePath(
                                            GetImagePath(ImagePath) + "/" + ((WZUOLProperty)img).Value);
                                    if (img is WZCanvasProperty)
                                    {
                                        DisplayImage(((WZCanvasProperty)img).Value);
                                        try
                                        {
                                            Thread.Sleep(img.ResolvePath("delay").ValueOrDie<int>());
                                        }
                                        catch
                                        {
                                            Thread.Sleep(int.MaxValue);
                                        }
                                    }
                                }
                            }
                        }));
                    Threads.Add(t);
                    t.Start();
                }
            }
        }

        private void ImageInformation_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Thread t in Threads)
                t.Abort();
        }

        private void LoadImage(WZFile pWZ, string pImagePath)
        {
            Bitmap wzImage = ((WZCanvasProperty)pWZ.MainDirectory.ResolvePath(pImagePath)).Value;
            Text = pImagePath;
            DisplayImage(wzImage);
        }

        private void DisplayImage(Bitmap pImage)
        {
            if (InvokeRequired)
                Invoke(InvokeControls, pImage);
            else
            {
                Size = new Size((int)(Text.Length * 9.3), pImage.Height + 38);
                if (pImage.Width > Size.Width)
                    Size = new Size(pImage.Width, Size.Height);
                pictureBox1.Image = pImage;
                pictureBox1.Size = new Size(Size.Width, pImage.Height);
                //pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private string GetImagePath(string pImagePath)
        {
            return pImagePath.TrimStart('/').TrimEnd('/');
        }
    }
}
