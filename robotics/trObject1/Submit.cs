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

        Graphics g;
        Bitmap video;

        int mode;

        // For trackBar
        int Red, Green, Blue;

        // this is for progress bar.
        public int count = 0;

        // this is for webcam.
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;

     
        private void Form1_Load(object sender, EventArgs e)
        {

            sp.Open();

            // this is for webcam
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
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
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);

            FinalFrame.Start();
        }

        /* FinalFrame_NewFrame is an event Handler which gets fired every time the webcam captures a fresh image. 
         This allows image data to be intercepted and different image processing techniques can be applied.*/
        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();

            video = (Bitmap)eventArgs.Frame.Clone();
            Bitmap video2 = (Bitmap)eventArgs.Frame.Clone();

            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //pictureBox1.Image = video;
            /*  Insert break-point here to view width and height of video in the ‘Locals’ Window. */


            // to draw rectangle in video

            Graphics ga = Graphics.FromImage(video); // To apply Grapghics on video2, that is, pictureBox2.

            ga = Graphics.FromImage(video);

            using (Pen pen = new Pen(Color.Red, 7))
            {


                ga.DrawRectangle(pen, 160, 120, 320, 240); // these values are selected after calculation from width = 640 and height = 480.

            }
            ga.Dispose();

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

                    Graphics gra = Graphics.FromImage(video);  // To show the white rectangle around red object in pictureBox1.
                    

                    using (Pen pen = new Pen(Color.White, 3))
                    {

                        graphic.DrawRectangle(pen, target);

                        gra.DrawRectangle(pen, target);

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
                //  textBox.Text += ("Robot rotate right 20 Deg" + Environment.NewLine);

                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                // textBox.Text += ("Robot rotate right 20 Deg" + Environment.NewLine);



                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //  textBox.Text += ("stop" + Environment.NewLine);

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
                //  textBox.Text += ("stop" + Environment.NewLine);

                sp.Write("6");
                i = 6;
                Thread.Sleep(800); // if you need to update the GUI to real time DO NOT USE SLEEP.
                bgw.ReportProgress(i);

                sp.Write("8");
                i = 8;
                Thread.Sleep(4000);
                bgw.ReportProgress(i);
                // textBox.Text += ("robot drift Right" + Environment.NewLine);


                // For progressBar at the end of each if loop:
                bgw.ReportProgress(101);
            }


            if (!abort)
            {
    
                sp.Write("6");
                i = 6;
                Thread.Sleep(300); // if you need to update the GUI to real time DO NOT USE SLEEP.
                bgw.ReportProgress(i);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("stop" + Environment.NewLine);

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
                //textBox.Text += ("Forward " + Environment.NewLine);

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
                //textBox.Text += ("Robot Stop " + Environment.NewLine);

                sp.Write("8");
                i = 8;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("Forward " + Environment.NewLine);

                sp.Write(".");
                i = 10;
                Thread.Sleep(100);
                bgw.ReportProgress(i);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("Robot Stop " + Environment.NewLine);

                sp.Write("9");
                i = 9;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("robot drift Right " + Environment.NewLine);

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
                //textBox.Text += ("Forward " + Environment.NewLine);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("Robot Stop " + Environment.NewLine);

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
                //textBox.Text += ("Robot Drive Right " + Environment.NewLine);

                sp.Write("5");
                i = 5;
                Thread.Sleep(100);
                bgw.ReportProgress(i);
                //textBox.Text += ("Robot Stop " + Environment.NewLine);

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
            //Thread.Sleep(100);
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
