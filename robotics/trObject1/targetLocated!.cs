using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math.Geometry;
using System.Drawing.Imaging;
using System.IO;


namespace trObject1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics g;
        Bitmap video;

        int mode;

        // For trackBar
        int Red, Green, Blue;



        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;


        private void Form1_Load(object sender, EventArgs e)
        {

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
        }
        private void btn_Begin_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);

            FinalFrame.Start();
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

            video = (Bitmap)eventArgs.Frame.Clone();
            Bitmap video2 = (Bitmap)eventArgs.Frame.Clone();



            // this is mode 1
            if (mode == 1)
            {
                //
                ColorFiltering colorfilter = new ColorFiltering();
                colorfilter.Red = new IntRange(Red, (int)numericUpDownRed.Value);
                colorfilter.Green = new IntRange(Green, (int)numericUpDownGreen.Value);
                colorfilter.Blue = new IntRange(Blue, (int)numericUpDownBlue.Value);
                colorfilter.ApplyInPlace(video2);


                //BlobCounter
                BlobCounter blobcounter = new BlobCounter();
                blobcounter.MinHeight = 20;
                blobcounter.MinWidth = 20;
                blobcounter.ObjectsOrder = ObjectsOrder.Size;
                blobcounter.ProcessImage(video2);
                Rectangle[] rect = blobcounter.GetObjectsRectangles();
                if (rect.Length > 0)
                {
                    Rectangle target = rect[0];
                    Graphics graphic = Graphics.FromImage(video2); // To apply Grapghics on video2, that is, pictureBox2.

                    g = Graphics.FromImage(video2);
                 
                    using (Pen pen = new Pen(Color.White, 3))
                    {

                        graphic.DrawRectangle(pen, target);

                        g.DrawString("Target Located !", new Font("Arial", 20), new SolidBrush(Color.White), new PointF(2, 2)); // to demonstrate 'recognition' of target.
                        g.Dispose();
                    }
                    graphic.Dispose();
                }
                pictureBox2.Image = video2;
            }
            pictureBox1.Image = video;
            
        }
      
        
        private void btnTrackingObject_Click(object sender, EventArgs e)
        {
            mode = 1;

        }

        private void trackBarRed_Scroll(object sender, EventArgs e)
        {
            Red = (int)trackBarRed.Value;
        }


        private void trackBarGreen_Scroll(object sender, EventArgs e)
        {
            Green = (int)trackBarGreen.Value;
        }

        private void trackBarBlue_Scroll(object sender, EventArgs e)
        {
            Blue = (int)trackBarBlue.Value;
        }

        
        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        
    }
}
