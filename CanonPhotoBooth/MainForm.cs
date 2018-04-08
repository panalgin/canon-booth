using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public partial class MainForm : Form
    {
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        private Timer timer = new Timer();
        private double captureInterval = 200;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            GetVideoDevices();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label2.Text = "Device running... " + videoSource.FramesReceived.ToString() + " FPS";
        }

        private void QuickFrame_Button_Click(object sender, EventArgs e)
        {

        }

        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            if (this.Start_Button.Text == "&Start")
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[Cameras_Combo.SelectedIndex].MonikerString);

                    var capabilities = videoSource.VideoCapabilities;

                    videoSource.VideoResolution = capabilities[0];
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, -5, CameraControlFlags.Manual);
                    videoSource.SetCameraProperty(CameraControlProperty.Focus, -5, CameraControlFlags.Auto);
                    
                    videoSource.ProvideSnapshots = true;

                    /*AForge.Video.DirectShow.
                    videoSource.SnapshotResolution.FrameSize = new Size 
                       w*/ 
                    CloseVideoSource();

                    videoSource.Start();
                    label2.Text = "Device running...";
                    this.Start_Button.Text = "&Stop";
                    timer.Enabled = true;
                    timer.Start();
                }
                else
                {
                    label2.Text = "Error: No Device selected.";
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                    timer.Enabled = false;
                    timer.Stop();
                    CloseVideoSource();
                    label2.Text = "Device stopped.";
                    this.Start_Button.Text = "&Start";
                }
            }
        }

        ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        EncoderParameters encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L) } };

        Task EncodeImage(Bitmap map)
        {
            var stream = new MemoryStream();
            map.Save(stream, encoder, encParams);

            Image jpeg = Bitmap.FromStream(stream);

            pictureBox1.BeginInvoke((MethodInvoker)delegate ()
            {
                pictureBox1.Image = jpeg;
            });

            return Task.FromResult(true);
        }

        DateTime lastSnapshotAt = DateTime.MinValue;

        private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (DateTime.Now - lastSnapshotAt > TimeSpan.FromMilliseconds(captureInterval))
            {
                lastSnapshotAt = DateTime.Now;

                using (Bitmap img = (Bitmap)eventArgs.Frame.Clone())
                {

                      EncodeImage(img);
                }
            }
        }

        private void GetVideoDevices()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;

                foreach (FilterInfo device in videoDevices)
                {
                    Cameras_Combo.Items.Add(device.Name);
                }

                Cameras_Combo.SelectedIndex = 0; //make dafault to first cam
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                Cameras_Combo.Items.Add("No capture device on your system");
            }
        }

        private void Interval_Num_ValueChanged(object sender, EventArgs e)
        {
            captureInterval = Convert.ToDouble(this.Interval_Num.Value);
        }
    }
}
