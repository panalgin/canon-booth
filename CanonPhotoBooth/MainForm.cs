using AForge.Video.DirectShow;
using CanonPhotoBooth.Properties;
using CefSharp;
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
            LoadSettings();

            Cef.Initialize(new CefSettings()
            {

            });

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

                    var selectedCapabilityIndex = this.Capabilities_Combo.SelectedIndex;

                    videoSource.VideoResolution = videoSource.VideoCapabilities[selectedCapabilityIndex];
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.SetCameraProperty(CameraControlProperty.Zoom, 3, CameraControlFlags.Manual);
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, -5, CameraControlFlags.Auto);
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
        EncoderParameters encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L) } };

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
                    var capabilities = GetCapabilities(device.MonikerString).Select(q => string.Format("Fps: {0} | Resolution: {1}x{2}", q.AverageFrameRate, q.FrameSize.Width, q.FrameSize.Height));

                    this.Capabilities_Combo.Items.AddRange(capabilities.ToArray());
                    this.Capabilities_Combo.SelectedIndex = 0;
                }

                Cameras_Combo.SelectedIndex = 0; //make dafault to first cam
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                Cameras_Combo.Items.Add("No capture device on your system");
            }
        }

        List<VideoCapabilities> GetCapabilities(string moniker)
        {
            videoSource = new VideoCaptureDevice(moniker);
            var capabilities = videoSource.VideoCapabilities.ToList();

            return capabilities;
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
                //Add first image and set the animation delay to 100ms
                //MagickNET.Initialize(@"C:\Users\johsam\Downloads\Magick\MagickScript.xsd");

                var outputFps = Convert.ToInt32(this.OutputFps_Num.Value);
                var animationDelay = /*1000 /*/ outputFps;

                int curFrame = 0;

                imageFrames.All(delegate (string fileName)
                {
                    collection.Add(fileName);
                    collection[curFrame].AnimationDelay = animationDelay;

                    return true;
                });

                curFrame = 0;
                collection.All(delegate (IMagickImage image)
                {
                    //image.Implode(0.5, PixelInterpolateMethod.Average);
                    //image.OilPaint();
                    curFrame++;

                    return true;
                });

                // Optionally reduce colors
                QuantizeSettings settings = new QuantizeSettings();
                settings.Colors = 32;
                collection.Quantize(settings);

                // Optionally optimize the images (images should have the same size).
                //collection.Optimize();

                var savePath = Path.Combine(Application.StartupPath, "Output.gif");
                collection.Write(savePath);
            }
        }

        private void Left_Screen_Button_Click(object sender, EventArgs e)
        {
            LeftScreenForm form = new LeftScreenForm();
            form.Location = new Point(0, 0);
            form.Show();
            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.videoSource != null)
                CloseVideoSource();

            Cef.Shutdown();
        }

        private void Show_Registration_Button_Click(object sender, EventArgs e)
        {
            if (this.Show_Registration_Button.Text == "Show")
            {
                RegistrationForm form = new RegistrationForm();

                form.Location = GetLocationFor("Registration");
                form.Size = GetSizeFor("Registration");

                form.Show();

                this.Show_Registration_Button.Text = "Hide";
            }
            else
            {
                var regForm = Application.OpenForms.OfType<RegistrationForm>().FirstOrDefault();

                if (regForm != null)
                {
                    regForm.Dispose();
                    this.Show_Registration_Button.Text = "Show";
                }
            }
        }

        private Size GetSizeFor(string page)
        {
            int width = 0;
            int height = 0;

            switch(page)
            {
                case "Registration":
                    {
                        width = Convert.ToInt32(this.Promoter_Width_Num.Value);
                        height = Convert.ToInt32(this.Promoter_Height_Num.Value);

                        break;
                    }

                case "Left":
                    {
                        width = Convert.ToInt32(this.Left_Width_Num.Value);
                        height = Convert.ToInt32(this.Left_Height_Num.Value);

                        break;
                    }

                case "Right":
                    {
                        width = Convert.ToInt32(this.Right_Width_Num.Value);
                        height = Convert.ToInt32(this.Right_Width_Num.Value);

                        break;
                    }
            }

            return new Size(width, height);
        }

        private Point GetLocationFor(string page)
        {
            int x = 0;
            int y = 0;

            switch (page)
            {
                case "Registration":
                    {
                        x = Convert.ToInt32(this.Promoter_X_Num.Value);
                        y = Convert.ToInt32(this.Promoter_Y_Num.Value);

                        break;
                    }

                case "Left":
                    {
                        x = Convert.ToInt32(this.Left_X_Num.Value);
                        y = Convert.ToInt32(this.Left_Y_Num.Value);

                        break;
                    }

                case "Right":
                    {
                        x = Convert.ToInt32(this.Right_X_Num.Value);
                        y = Convert.ToInt32(this.Right_Y_Num.Value);

                        break;
                    }
            }

            return new Point(x, y);
        }

        private void Update_Screens_Button_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadSettings()
        {
            Settings.Default.Reload();

            var settings = Settings.Default;

            this.Left_X_Num.Value = settings.LeftScreenLocation.X;
            this.Left_Y_Num.Value = settings.LeftScreenLocation.Y;

            this.Right_X_Num.Value = settings.RightScreenLocation.X;
            this.Right_Y_Num.Value = settings.RightScreenLocation.Y;

            this.Promoter_X_Num.Value = settings.RegistrationScreenLocation.X;
            this.Promoter_Y_Num.Value = settings.RegistrationScreenLocation.Y;

            this.Left_Width_Num.Value = settings.LeftScreenSize.Width;
            this.Left_Height_Num.Value = settings.LeftScreenSize.Height;

            this.Right_Width_Num.Value = settings.RightScreenSize.Width;
            this.Right_Height_Num.Value = settings.RightScreenSize.Height;

            this.Promoter_Width_Num.Value = settings.RegistrationScreenSize.Width;
            this.Promoter_Height_Num.Value = settings.RegistrationScreenSize.Height;
        }

        private void SaveSettings()
        {
            var leftScreenSize = new Size((int)this.Left_Width_Num.Value, (int)this.Left_Height_Num.Value);
            var rightScreenSize = new Size((int)this.Right_Width_Num.Value, (int)this.Right_Height_Num.Value);
            var regScreenSize = new Size((int)this.Promoter_Width_Num.Value, (int)this.Promoter_Height_Num.Value);

            var leftScreenLocation = new Point((int)this.Left_X_Num.Value, (int)this.Left_Y_Num.Value);
            var rightScreenLocation = new Point((int)this.Right_X_Num.Value, (int)this.Right_Y_Num.Value);
            var regScreenLocation = new Point((int)this.Promoter_X_Num.Value, (int)this.Promoter_Y_Num.Value);

            Settings.Default.LeftScreenSize = leftScreenSize;
            Settings.Default.LeftScreenLocation = leftScreenLocation;

            Settings.Default.RightScreenSize = rightScreenSize;
            Settings.Default.RightScreenLocation = rightScreenLocation;

            Settings.Default.RegistrationScreenSize = regScreenSize;
            Settings.Default.RegistrationScreenLocation = regScreenLocation;

            Settings.Default.Save();            
        }
    }
}
