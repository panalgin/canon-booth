using AForge.Video.DirectShow;
using CanonPhotoBooth.Properties;
using CefSharp;
using CefSharp.WinForms;

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
using EOSDigital.API;
using EOSDigital.SDK;
using System.IO.Ports;
using Mandrill;
using Mandrill.Model;

namespace CanonPhotoBooth
{
    public partial class MainForm : Form
    {
        #region Public/Private Declarations
        private FilterInfoCollection videoDevices;

        private VideoCaptureDevice videoSource = null;
        private VideoCaptureDevice videoSource2 = null;

        private Timer timer = new Timer();
        private Timer timer2 = new Timer();
        private Timer dslrTimer = new Timer();

        private double captureInterval = 200;
        private double captureInterval2 = 200;

        private bool IsRecording = false;
        #endregion
        #region MainForm Base

        KeyboardHook hook = new KeyboardHook();
        KeyboardHook resetHook = new KeyboardHook();
        KeyboardHook shutDownHook = new KeyboardHook();

        public MainForm()
        {
            InitializeComponent();


            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(ModifiedKeys.Control,
                Keys.D1);

            resetHook.KeyPressed += ResetHook_KeyPressed;
            resetHook.RegisterHotKey(ModifiedKeys.Control, Keys.D5);

            shutDownHook.KeyPressed += ShutDownHook_KeyPressed;
            shutDownHook.RegisterHotKey(ModifiedKeys.Control, Keys.D8);
        }

        private void ShutDownHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Application.Exit();
        }

        private void ResetHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Game.Reset();
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Game.Start();
        }

        void CleanUp()
        {
            var savePath = Path.Combine(Application.StartupPath, "Snapshots");
            var files = Directory.EnumerateFiles(savePath, "*.jpg", SearchOption.AllDirectories);

            files.All(delegate (string file)
            {
                FileInfo info = new FileInfo(file);

                info.Delete();

                return true;
            });
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CleanUp();

            World.Initialize();
            Game.Initialize();
            Saver.Initialize();

            LoadSettings();
            GetVideoDevices();
            GetComPorts();

            Cef.Initialize(new CefSettings() { });

            CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            string saveFolder = Path.Combine(Application.StartupPath, "Snapshots");

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            timer2.Interval = 1000;
            timer2.Tick += Timer2_Tick;

            EventSink.RecordRequested += EventSink_RecordRequested;
            EventSink.GameFinished += EventSink_GameFinished;

            captureInterval = Convert.ToDouble(this.Camera1_Interval_Num.Value);
            captureInterval2 = Convert.ToDouble(this.Camera2_Interval_Num.Value);
        }

        private void EventSink_GameFinished(Player winner)
        {

            IsRecording = false;

            Task.Run(async () =>
            {
                await GenerateGifs();
            });
        }

        private void EventSink_RecordRequested()
        {
            IsRecording = true;
        }

        void GetComPorts()
        {
            var comPorts = SerialPort.GetPortNames();

            this.comboBox2.Items.AddRange(comPorts);
            this.comboBox1.Items.AddRange(comPorts);

            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = 0;

            if (this.comboBox2.Items.Count > 0)
                this.comboBox2.SelectedIndex = 0;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.videoSource != null)
                CloseVideoSource();

            if (this.videoSource2 != null)
            {
                if (videoSource2.IsRunning)
                    videoSource2.SignalToStop();
            }

            Cef.Shutdown();
        }
        #endregion
        #region Camera Parts
        private void Timer2_Tick(object sender, EventArgs e)
        {
            label34.Text = "Camera Running At: " + videoSource2.FramesReceived.ToString() + " FPS";
            label37.Text = string.Format("Preview Running At: {0} FPS", (1000 / (int)captureInterval2));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label2.Text = "Camera Running At: " + videoSource.FramesReceived.ToString() + " FPS";
            PreviewFps_Label.Text = string.Format("Preview Running At: {0} FPS", (1000 / (int)captureInterval));
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
            if (this.Camera1_Start_Button.Text == "&Start")
            {
                if (videoDevices.Count > 0)
                {
                    CloseVideoSource();

                    videoSource = new VideoCaptureDevice(videoDevices[1].MonikerString);

                    var selectedCapabilityIndex = this.Camera1_Caps_Combo.SelectedIndex;

                    videoSource.VideoResolution = videoSource.VideoCapabilities[selectedCapabilityIndex];
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.SetCameraProperty(CameraControlProperty.Zoom, 0, CameraControlFlags.Auto);
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, -3, CameraControlFlags.Auto);
                    videoSource.SetCameraProperty(CameraControlProperty.Focus, -5, CameraControlFlags.Auto);

                    videoSource.ProvideSnapshots = false;

                    videoSource.Start();
                    label2.Text = "Camera Running At: ";
                    this.Camera1_Start_Button.Text = "&Stop";
                    timer.Enabled = true;
                    timer.Start();
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
                    this.Camera1_Start_Button.Text = "&Start";
                }
            }
        }

        private void Camera2_Start_Button_Click(object sender, EventArgs e)
        {
            if (this.Camera2_Start_Button.Text == "&Start")
            {
                if (videoSource2 != null)
                {
                    if (videoSource2.IsRunning)
                    {
                        videoSource2.SignalToStop();
                        videoSource2 = null;
                    }
                }

                videoSource2 = new VideoCaptureDevice(videoDevices[0].MonikerString);

                var selectedCapabilityIndex = this.Camera2_Caps_Combo.SelectedIndex;

                videoSource2.VideoResolution = videoSource2.VideoCapabilities[selectedCapabilityIndex];
                videoSource2.NewFrame += VideoSource2_NewFrame;
                videoSource2.SetCameraProperty(CameraControlProperty.Exposure, -3, CameraControlFlags.Auto);
                videoSource2.SetCameraProperty(CameraControlProperty.Focus, -5, CameraControlFlags.Auto);
                videoSource2.ProvideSnapshots = false;
                label34.Text = "Camera Running At: ";
                videoSource2.Start();

                label34.Text = "Camera Running At: ";
                this.Camera2_Start_Button.Text = "&Stop";
                timer2.Enabled = true;
                timer2.Start();
            }
            else
            {
                if (videoSource2.IsRunning)
                {
                    timer2.Enabled = false;
                    timer2.Stop();

                    if (videoSource2 != null)
                    {
                        if (videoSource2.IsRunning)
                        {
                            videoSource2.SignalToStop();
                            videoSource2 = null;
                        }
                    }

                    label34.Text = "Device stopped.";
                    this.Camera2_Start_Button.Text = "&Start";
                }
            }
        }

        DateTime lastSnapshotAt2 = DateTime.MinValue;

        private void VideoSource2_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (DateTime.Now - lastSnapshotAt2 > TimeSpan.FromMilliseconds(captureInterval2))
            {
                lastSnapshotAt2 = DateTime.Now;

                using (Bitmap img = (Bitmap)eventArgs.Frame.Clone())
                {
                    EncodeImage(img, 1);
                }
            }
        }

        ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        EncoderParameters encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L) } };

        void EncodeImage(Bitmap map, int cameraIndex)
        {
            var stream = new MemoryStream();
            map.Save(stream, encoder, encParams);

            try
            {
                Image jpeg = Bitmap.FromStream(stream);

                if (cameraIndex == 0)
                    pictureBox1.Image = jpeg;
                else
                    pictureBox2.Image = jpeg;

                if (IsRecording)
                {
                    Task.Run(async () =>
                    {
                        await SaveGifFrame(jpeg.Clone() as Image, cameraIndex);
                    });
                }
            }
            catch (OutOfMemoryException ex)
            {

            }
        }

        Task<bool> SaveGifFrame(Image image, int cameraIndex)
        {
            string saveFolder = Path.Combine(Application.StartupPath, "Snapshots", cameraIndex.ToString());
            string savePath = Path.Combine(saveFolder, DateTime.Now.ToFileTimeUtc() + ".jpg");

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

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
                    EncodeImage(img, 0);
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

                int index = 0;

                foreach (FilterInfo device in videoDevices)
                {
                    var capabilities = GetCapabilities(device.MonikerString).Select(q => string.Format("Fps: {0} | Resolution: {1}x{2}", q.AverageFrameRate, q.FrameSize.Width, q.FrameSize.Height));

                    if (index == 0)
                    {
                        Camera2_Name_Label.Text = device.Name;
                        Camera2_Path_Label.Text = device.MonikerString;

                        Camera2_Caps_Combo.Items.AddRange(capabilities.ToArray());
                        Camera2_Caps_Combo.SelectedIndex = 16;
                    }
                    else
                    {
                        Camera1_Name_Label.Text = device.Name;
                        Camera1_Path_Label.Text = device.MonikerString;

                        Camera1_Caps_Combo.Items.AddRange(capabilities.ToArray());
                        Camera1_Caps_Combo.SelectedIndex = 16;
                    }

                    index++;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error");

                Camera1_Name_Label.Text = "No capture device on your system";
                Camera2_Name_Label.Text = "No capture device on your system";
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
            captureInterval = Convert.ToDouble(this.Camera1_Interval_Num.Value);
        }

        private void Camera2_Interval_Num_ValueChanged(object sender, EventArgs e)
        {
            captureInterval2 = Convert.ToDouble(this.Camera2_Interval_Num.Value);
        }

        DateTime LastEnteredIn = DateTime.MinValue;

        Task<bool> GenerateGifs()
        {
            if (DateTime.Now - LastEnteredIn < TimeSpan.FromSeconds(15))
                return Task.FromResult(true);
            else
            {
                LastEnteredIn = DateTime.Now;

                for (int i = 0; i < 2; i++)
                {
                    string readPath = Path.Combine(Application.StartupPath, "Snapshots", i.ToString());

                    var imageFrames = Directory.EnumerateFiles(readPath, "*.jpg", SearchOption.TopDirectoryOnly);

                    if (imageFrames.Count() > 0)
                    {
                        using (MagickImageCollection collection = new MagickImageCollection())
                        {
                            var outputFps = 30;
                            var animationDelay = outputFps;

                            int curFrame = 0;

                            imageFrames.All(delegate (string fileName)
                            {
                                collection.Add(fileName);
                                collection[curFrame].AnimationDelay = animationDelay;

                                return true;
                            });

                            try
                            {
                                QuantizeSettings settings = new QuantizeSettings();
                                settings.Colors = 256;

                                collection.Quantize(settings);
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, "Gif Error");
                            }

                            var savePath = Path.Combine(Application.StartupPath, string.Format("Output-{0}-{1}.gif", i, DateTime.Now.ToFileTimeUtc()));
                            collection.Write(savePath);

                            EventSink.InvokeGifGenerated(i, savePath);
                        }

                        Directory.EnumerateFiles(readPath).All(delegate (string file)
                        {
                            var fileInfo = new FileInfo(file);
                            fileInfo.Delete();

                            return true;
                        });
                    }
                }

                return Task.FromResult(true);
            }
        }
        #endregion
        #region Screen Parts

        private void Left_Screen_Button_Click(object sender, EventArgs e)
        {
            LeftScreenForm form = new LeftScreenForm();
            form.Location = new System.Drawing.Point(0, 0);
            form.Show();

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

        private System.Drawing.Size GetSizeFor(string page)
        {
            int width = 0;
            int height = 0;

            switch (page)
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

            return new System.Drawing.Size(width, height);
        }

        private System.Drawing.Point GetLocationFor(string page)
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

            return new System.Drawing.Point(x, y);
        }

        private void Update_Screens_Button_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void Show_Left_Screen_Button_Click(object sender, EventArgs e)
        {
            if (this.Show_Left_Screen_Button.Text == "Show")
            {
                LeftScreenForm form = new LeftScreenForm();

                form.Location = GetLocationFor("Left");
                form.Size = GetSizeFor("Left");

                form.Show();

                this.Show_Left_Screen_Button.Text = "Hide";
            }
            else
            {
                var leftForm = Application.OpenForms.OfType<LeftScreenForm>().FirstOrDefault();

                if (leftForm != null)
                {
                    leftForm.Dispose();
                    this.Show_Left_Screen_Button.Text = "Show";
                }
            }
        }

        private void Show_Right_Screen_Button_Click(object sender, EventArgs e)
        {
            if (this.Show_Right_Screen_Button.Text == "Show")
            {
                RightScreenForm form = new RightScreenForm();

                form.Location = GetLocationFor("Right");
                form.Size = GetSizeFor("Right");

                form.Show();

                this.Show_Right_Screen_Button.Text = "Hide";
            }
            else
            {
                var rightForm = Application.OpenForms.OfType<RightScreenForm>().FirstOrDefault();

                if (rightForm != null)
                {
                    rightForm.Dispose();
                    this.Show_Right_Screen_Button.Text = "Show";
                }
            }
        }
        #endregion
        #region Settings
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
            var leftScreenSize = new System.Drawing.Size((int)this.Left_Width_Num.Value, (int)this.Left_Height_Num.Value);
            var rightScreenSize = new System.Drawing.Size((int)this.Right_Width_Num.Value, (int)this.Right_Height_Num.Value);
            var regScreenSize = new System.Drawing.Size((int)this.Promoter_Width_Num.Value, (int)this.Promoter_Height_Num.Value);

            var leftScreenLocation = new System.Drawing.Point((int)this.Left_X_Num.Value, (int)this.Left_Y_Num.Value);
            var rightScreenLocation = new System.Drawing.Point((int)this.Right_X_Num.Value, (int)this.Right_Y_Num.Value);
            var regScreenLocation = new System.Drawing.Point((int)this.Promoter_X_Num.Value, (int)this.Promoter_Y_Num.Value);

            Settings.Default.LeftScreenSize = leftScreenSize;
            Settings.Default.LeftScreenLocation = leftScreenLocation;

            Settings.Default.RightScreenSize = rightScreenSize;
            Settings.Default.RightScreenLocation = rightScreenLocation;

            Settings.Default.RegistrationScreenSize = regScreenSize;
            Settings.Default.RegistrationScreenLocation = regScreenLocation;

            Settings.Default.Save();
        }
        #endregion

        private void Connect_Left_Button_Click(object sender, EventArgs e)
        {
            if (World.Boards[0].Connect(this.comboBox1.SelectedItem.ToString(), 115200))
            {
                MessageBox.Show("Connection successfull", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.groupBox2.Enabled = false;
            }
            else
                MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void Connect_Right_Button_Click(object sender, EventArgs e)
        {
            if (World.Boards[1].Connect(this.comboBox2.SelectedItem.ToString(), 115200))
            {
                MessageBox.Show("Connection successfull", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.groupBox3.Enabled = false;
            }
            else
                MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Trigger_GameStart_Button_Click(object sender, EventArgs e)
        {
            Game.Start();
        }

        private void Source2_Properties_Button_Click(object sender, EventArgs e)
        {
            if (videoSource2 != null)
                videoSource2.DisplayPropertyPage(this.Handle);
        }

        private void Source1_Properties_Button_Click(object sender, EventArgs e)
        {
            if (videoSource != null)
                videoSource.DisplayPropertyPage(this.Handle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Gif Files|*.gif";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileName = dialog.FileName;
                Emailer.Send(new Player() { Email = "badal.dixit@gmail.com" }, fileName);
            }
        }
    }
}
