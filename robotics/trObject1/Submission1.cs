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
using System.Threading;


namespace trObject1
{
    public partial class Form1 : Form
    {

        static bool abort = false;
        BackgroundWorker bgw;

        public Form1()
        {
            InitializeComponent();
            // this is the  backgroundWorker for autonomous buttons to implement abort after 9.78 seconds delay.
            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;

        }

        

        // this is for progress bar.
        public int count = 0;

        // this is for webcam.
        private FilterInfoCollection CaptureDevices;

        private VideoCaptureDevice videoSource;


        private void Form1_Load(object sender, EventArgs e)
        {

            sp.Open();

            // this is for webcam
            CaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevices)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
        }


        private void sp_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // you get this block by going to serial port properties and flash -> double click the "data received".
            string myData = sp.ReadExisting();
            BeginInvoke((Action)delegate ()
            {
                tbData.Text += myData;
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sp.Close();
        }
        private void btn_Begin_Click(object sender, EventArgs e)
        {
            videoSource = new VideoCaptureDevice(CaptureDevices[comboBox1.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(VideoSource_NewFrame);

            /*
            videoSource.DesiredFrameRate = 20; // FPS
            videoSource.DesiredFrameSize = new Size(320, 240);
            */
            videoSource.Start();
        }

        /* FinalFrame_NewFrame is an event Handler which gets fired every time the webcam captures a fresh image. 
         This allows image data to be intercepted and different image processing techniques can be applied.*/
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;


            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            // pictureBox1.Image = image;
            /*  Insert break-point here to view width and height of video in the ‘Locals’ Window. */

            if (radiobtnRed.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new RGB(Color.FromArgb(215, 0, 0));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);

                ProcessIMG(image1);
            }



            if (radiobtnBlue.Checked)
            {
                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);

                ProcessIMG(image1);
            }

        }


        // define ProcessIMG function
        public void ProcessIMG(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            // Grayscale griFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            //Grayscale griFilter = new Grayscale(0.2, 0.2, 0.2);
            //Bitmap griImage = griFilter.Apply(image);


            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            //grayscaling
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));

            // unlock image
            image.UnlockBits(objectsData);

            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // coordinates: 
            foreach (Rectangle rc in rects)
            {


                if ((rects.Length > 0) && (rects[0].Height > 0))
                {
                    Rectangle target = rects[0];


                    //System.Diagnostics.Debug.WriteLine(
                    // string.Format("Position: ({0}, {1}), Size: {2} x {3}",
                    //  rc.Left, rc.Top, rc.Width, rc.Height));


                    PointF drawPoin = new PointF(target.X, target.Y);
                    int objectX = target.X + target.Width / 2 - image.Width / 2;
                    int objectY = image.Height / 2 - (target.Y + target.Height / 2);

                    // To decide in which direction the robot should steer.
                    int poZ = 320 - objectX;

                    Graphics graphic = Graphics.FromImage(image); // To apply Grapghics on video2, that is, pictureBox2.

                    Graphics g = Graphics.FromImage(image);
                    using (Pen pen = new Pen(Color.White, 3))
                    {

                        graphic.DrawRectangle(pen, target);

                        // with  PointF drawPoin
                        string Blobinformation = "X= " + objectX.ToString() + "\nY= " + objectY.ToString() + "\nSize=" + target.Size.ToString() + "\npoZ=" + poZ.ToString();
                        g.DrawString(Blobinformation, new Font("Arial", 19), new SolidBrush(Color.White), drawPoin);

                        g.DrawString("Target Located !", new Font("Arial", 20), new SolidBrush(Color.White), new PointF(2, 2)); // to demonstrate 'recognition' of target.

                        g.Dispose();
                    }
                    graphic.Dispose();
                }
            }

            pictureBox3.Image = image;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            videoSource.Stop();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            pictureBox2.Image = null;
            pictureBox2.Invalidate();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            videoSource.Stop();
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = (Bitmap)pictureBox1.Image.Clone();
        }

        private void btnTrackingObject_Click(object sender, EventArgs e)
        {
            

        }

      


       




        //******************************************************************************
        // this is for Manual Control

        //******************************************************************************
        private void btnDriftLeft_Click(object sender, EventArgs e)
        {
            sp.Write("7");
            Thread.Sleep(100);
            textBox.Text += ("robot drift left " + Environment.NewLine);
        }

        private void btnDriftRight_Click(object sender, EventArgs e)
        {
            sp.Write("9");
            Thread.Sleep(100);
            textBox.Text += ("robot drift Right " + Environment.NewLine);
        }

        private void btnDriveLeft_Click(object sender, EventArgs e)
        {
            sp.Write("4");
            Thread.Sleep(100);
            textBox.Text += ("robot drive Left " + Environment.NewLine);
        }

        private void btnStopRobot_Click(object sender, EventArgs e)
        {
            sp.Write("5");
            Thread.Sleep(100);
            textBox.Text += ("Robot Stop " + Environment.NewLine);
        }

        private void btnDriveRight_Click(object sender, EventArgs e)
        {
            sp.Write("6");
            Thread.Sleep(100);
            textBox.Text += ("Robot Drive Right " + Environment.NewLine);
        }

        private void btnbackLeft_Click(object sender, EventArgs e)
        {
            sp.Write("1");
            Thread.Sleep(100);
            textBox.Text += ("Robot Back Left " + Environment.NewLine);
        }

        private void btnDriveBack_Click(object sender, EventArgs e)
        {
            sp.Write("2");
            Thread.Sleep(100);
            textBox.Text += ("Robot Drive Back " + Environment.NewLine);
        }

        private void btnBackRight_Click(object sender, EventArgs e)
        {
            sp.Write("3");
            Thread.Sleep(100);
            textBox.Text += ("Robot Back Right " + Environment.NewLine);
        }

        private void btnRotateLeft20Deg_Click(object sender, EventArgs e)
        {
            sp.Write("0");
            Thread.Sleep(100);
            textBox.Text += ("Robot rotate left 20 Deg " + Environment.NewLine);
        }

        private void btnRotateRight20Deg_Click(object sender, EventArgs e)
        {
            sp.Write(".");
            Thread.Sleep(100);
            textBox.Text += ("Robot rotate right 20 Deg " + Environment.NewLine);
        }

        private void btnM_Click(object sender, EventArgs e)
        {
            byte[] command = { (byte)'M', 50, 206, 75 }; ////77 = ‘M’,   206 = -50
            sp.Write(command, 0, 4);
            Thread.Sleep(100);
            textBox.Text += ("Robot to rotate with a motor speed of +50 and -50 for 750ms " + Environment.NewLine);
        }

        private void btnDriveForward_Click(object sender, EventArgs e)
        {
            sp.Write("8");
            Thread.Sleep(100);
            textBox.Text += ("Forward " + Environment.NewLine);
        }

        //*****************************************************************************************************************
        // The code for Abort and Autonomous path.
        //*****************************************************************************************************************


        //Function DoCommands
        public void DoCommands()
        {

            int i;
            // if statement to check for abort.
            if (!abort)
            {
                sp.Write("8");
                i = 8;
                Thread.Sleep(5500);
                bgw.ReportProgress(i);

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }

            if (!abort)
            {

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);


                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);


                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
              



                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
              

                sp.Write("6");
                i = 6;
                Thread.Sleep(2000);
                bgw.ReportProgress(i);

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }


            // if statement to check for abort.

            if (!abort)
            {

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
              

                sp.Write("6");
                i = 6;
                Thread.Sleep(800); 
                bgw.ReportProgress(i);

                sp.Write("8");
                i = 8;
                Thread.Sleep(4000);
                bgw.ReportProgress(i);
               


                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }


            if (!abort)
            {
    
                sp.Write("6");
                i = 6;
                Thread.Sleep(300); 
                bgw.ReportProgress(i);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                sp.Write(".");
                i = 10;
                Thread.Sleep(1000);
                bgw.ReportProgress(i);

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);


            }

            // if statement to check for abort.

            if (!abort)
            {
    
                sp.Write("6");
                i = 6;
                Thread.Sleep(5000);
                bgw.ReportProgress(i);
                

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);

            }
       
            // if statement to check for abort.
            if (!abort)
            {


                sp.Write("8");
                i = 8;
                Thread.Sleep(3000);
                bgw.ReportProgress(i);
                //textBox.Text += ("Forward " + Environment.NewLine);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);

                sp.Write("8");
                i = 8;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                sp.Write("9");
                i = 9;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }


            // if statement to check for abort.
            if (!abort)
            {

                sp.Write("8");
                i = 8;
                Thread.Sleep(200);
                bgw.ReportProgress(i);
                

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }




            // if statement to check for abort.
            if (!abort)
            {
                sp.Write("6");
                i = 6;
                Thread.Sleep(4000);
                bgw.ReportProgress(i);
              

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
               

                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }

            //  if statement to check for abort.
            if (!abort)
            {

                MessageBox.Show("Mission Accomplished!", "Status");
                i = 12;
                bgw.ReportProgress(i);
                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);

            }

        }
        // end of DoCommands function.



        //******************************************************************************
        // backgroundWoker 

        //******************************************************************************
        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            abort = false;
            DoCommands();
        }

        //______________________________________________________________________________________
        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int i = e.ProgressPercentage;

            switch (i)
            {
                case 101:
                    count++;
                    BatteryStatus.Text = (100 - (count * 10)).ToString();

                    progressBar1.Value = progressBar1.Value - 10;
                    break;

                case 0:
                    tbData.Text = tbData.Text + "\r\nrobot rotate left 20deg\r\n";
                    break;

                case 1:
                    tbData.Text = tbData.Text + "\r\nrobot back left\r\n";
                    break;

                case 2:
                    tbData.Text = tbData.Text + "\r\nrobot drive back\r\n";
                    break;

                case 3:
                    tbData.Text = tbData.Text + "\r\nrobot back right\r\n";
                    break;


                case 4:
                    tbData.Text = tbData.Text + "\r\nrobot drive left\r\n";
                    break;


                case 5:
                    tbData.Text = tbData.Text + "\r\nrobot Stop\r\n";
                    break;


                case 6:
                    tbData.Text = tbData.Text + "\r\nrobot drive right\r\n";
                    break;

                case 7:
                    tbData.Text = tbData.Text + "\r\nrobot drift left\r\n";
                    break;

                case 8:
                    tbData.Text = tbData.Text + "\r\nForward\r\n";
                    break;

                case 9:
                    tbData.Text = tbData.Text + "\r\nrobot drift right\r\n";
                    break;

                case 10:
                    tbData.Text = tbData.Text + "\r\nrobot rotate right 20 deg\r\n";
                    break;

                case 11:
                    tbData.Text = tbData.Text + "\r\nExecute command M\r\n";
                    break;

                case 12:

                    break;

                default:
                    tbData.Text = tbData.Text + "\r\n";
                    break;


            }


            tbData.Refresh();



            tbData.Text += sp.ReadExisting();
        }

       


        //___________________________________________________________________________________________________________
        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string myData = sp.ReadExisting();
        }
        //___________________________________________________________________________________________________________


        //***********************************************************************************************
        // START and ABORT buttons for DoCommands Function.

        //***********************************************************************************************
        private void btnStart_Click(object sender, EventArgs e)
        {
            bgw.RunWorkerAsync(); //call backgroundworker to handle task on click of start
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            bgw.CancelAsync();
            abort = true;
           
            // The code below is for ensuring that the robot doed not continue with its last operation and that it comes to a halt.
            sp.Write("5");
            
        }


        //***************************************************************************************************
        //  Autoscroll for manual and autonomous textboxes to show current status of the commands executed.
        //***************************************************************************************************
        private void tbData_TextChanged(object sender, EventArgs e)
        {
            tbData.SelectionStart = tbData.Text.Length;
            tbData.ScrollToCaret();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        //***************************************************************************************************
        //  
        //***************************************************************************************************
        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {

        }

      
    }
}
