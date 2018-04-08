using AForge.Video.DirectShow;
using ImageMagick;
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
        private bool IsRecording = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string saveFolder = Path.Combine(Application.StartupPath, "Snapshots");

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            GetVideoDevices();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label2.Text = "Camera Running At: " + videoSource.FramesReceived.ToString() + " FPS";
            PreviewFps_Label.Text = string.Format("Preview Running At: {0} FPS", (1000 / (int)captureInterval));
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

                    videoSource.VideoResolution = capabilities[16];
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, -5, CameraControlFlags.Manual);
                    videoSource.SetCameraProperty(CameraControlProperty.Focus, -5, CameraControlFlags.Auto);

                    videoSource.ProvideSnapshots = false;

                    CloseVideoSource();

                    videoSource.Start();
                    label2.Text = "Camera Running At: ";
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

        void EncodeImage(Bitmap map)
        {
            var stream = new MemoryStream();
            map.Save(stream, encoder, encParams);

            Image jpeg = Bitmap.FromStream(stream);
            pictureBox1.Image = jpeg;

            if (IsRecording)
            {
                Task.Run(async () =>
                {
                    await SaveGifFrame(jpeg.Clone() as Image);
                });
            }
        }

        Task<bool> SaveGifFrame(Image image)
        {
            string savePath = Path.Combine(Application.StartupPath, "Snapshots", DateTime.Now.ToFileTimeUtc() + ".jpg");
            image.Save(savePath);

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

        private void RecordAsGif_Button_Click(object sender, EventArgs e)
        {
            if (this.RecordAsGif_Button.Text.StartsWith("Rec"))
            {
                this.RecordAsGif_Button.Text = "Stop Recording";
                IsRecording = true;


            }
            else
            {
                this.RecordAsGif_Button.Text = "Record As Gif";
                IsRecording = false;
            }
        }

        private void Output_Button_Click(object sender, EventArgs e)
        {
            string readPath = Path.Combine(Application.StartupPath, "Snapshots");

            var imageFrames = Directory.EnumerateFiles(readPath, "*.jpg", SearchOption.TopDirectoryOnly);


            using (MagickImageCollection collection = new MagickImageCollection())
            {
                //collection.CacheDirectory = @"C:\MyProgram\MyTempDir";
                // Add first image and set the animation delay to 100ms
                //MagickNET.Initialize(@"C:\Users\johsam\Downloads\Magick\MagickScript.xsd");

                var outputFps = Convert.ToInt32(this.OutputFps_Num.Value);
                var animationDelay = 1000 / outputFps;

                int curFrame = 0;

                imageFrames.All(delegate (string fileName)
                {
                    collection.Add(fileName);
                    collection[curFrame].AnimationDelay = 15;

                    return true;
                });

                curFrame = 0;
                collection.All(delegate (IMagickImage image)
                {
                    //image.Implode(0.5, PixelInterpolateMethod.Average);
                    image.OilPaint();
                    curFrame++;

                    return true;
                });

                // Optionally reduce colors
                QuantizeSettings settings = new QuantizeSettings();
                settings.Colors = 16;
                collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                collection.Optimize();

                var savePath = Path.Combine(Application.StartupPath, "Output.gif");
                collection.Write(savePath);
            }
        }
    }
}
