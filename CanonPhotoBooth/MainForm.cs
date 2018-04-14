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

namespace CanonPhotoBooth
{
    public partial class MainForm : Form
    {
        #region Public/Private Declarations
        CanonAPI APIHandler;
        Camera MainCamera;
        CameraValue[] AvList;
        CameraValue[] TvList;
        CameraValue[] ISOList;
        List<Camera> CamList;
        bool IsInit = false;
        Bitmap Evf_Bmp;

        int ErrCount;
        object ErrLock = new object();
        object LvLock = new object();

        private bool DeviceExist = false;
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

            CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            string saveFolder = Path.Combine(Application.StartupPath, "Snapshots");

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            timer2.Interval = 1000;
            timer2.Tick += Timer2_Tick;
            GetVideoDevices();

            try
            {
                APIHandler = new CanonAPI();
                APIHandler.CameraAdded += APIHandler_CameraAdded;
                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;
                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
                RefreshCamera();
                IsInit = true;
            }

            catch (DllNotFoundException) { ReportError("Canon DLLs not found!", true); }
            catch (Exception ex) { ReportError(ex.Message, true); }


            if (MainCamera?.SessionOpen == true)
                CloseSession();

            OpenSession();

            dslrTimer = new Timer();
            dslrTimer.Interval = 3000;
            dslrTimer.Tick += DslrTimer_Tick;
            dslrTimer.Start();
        }

        private void DslrTimer_Tick(object sender, EventArgs e)
        {
            if (MainCamera != null && MainCamera.SessionOpen)
            {
                try
                {
                    MainCamera.TakePhotoShutterAsync();
                }
                catch (Exception ex)
                {
                    ReportError(ex.Message, false);
                }
            }
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

            try
            {
                IsInit = false;
                MainCamera?.Dispose();
                APIHandler?.Dispose();
            }
            catch (Exception ex) { ReportError(ex.Message, false); }

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
                if (DeviceExist)
                {
                    CloseVideoSource();

                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

                    var selectedCapabilityIndex = this.Camera1_Caps_Combo.SelectedIndex;

                    videoSource.VideoResolution = videoSource.VideoCapabilities[selectedCapabilityIndex];
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.SetCameraProperty(CameraControlProperty.Zoom, 3, CameraControlFlags.Manual);
                    videoSource.SetCameraProperty(CameraControlProperty.Exposure, -5, CameraControlFlags.Auto);
                    videoSource.SetCameraProperty(CameraControlProperty.Focus, -5, CameraControlFlags.Auto);

                    videoSource.ProvideSnapshots = false;

                    videoSource.Start();
                    label2.Text = "Camera Running At: ";
                    this.Camera1_Start_Button.Text = "&Stop";
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
                    this.Camera1_Start_Button.Text = "&Start";
                }
            }
        }

        private void Camera2_Start_Button_Click(object sender, EventArgs e)
        {
            if (this.Camera2_Start_Button.Text == "&Start")
            {
                if (DeviceExist)
                {
                    if (videoSource2 != null)
                    {
                        if (videoSource2.IsRunning)
                        {
                            videoSource2.SignalToStop();
                            videoSource2 = null;
                        }
                    }

                    videoSource2 = new VideoCaptureDevice(videoDevices[1].MonikerString);

                    var selectedCapabilityIndex = this.Camera2_Caps_Combo.SelectedIndex;

                    videoSource2.VideoResolution = videoSource2.VideoCapabilities[selectedCapabilityIndex];
                    videoSource2.NewFrame += VideoSource2_NewFrame;
                    videoSource2.SetCameraProperty(CameraControlProperty.Zoom, 3, CameraControlFlags.Manual);
                    videoSource2.SetCameraProperty(CameraControlProperty.Exposure, -5, CameraControlFlags.Auto);
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
                    label34.Text = "Error: No Device selected.";
                }
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
                        Camera1_Name_Label.Text = device.Name;
                        Camera1_Path_Label.Text = device.MonikerString;

                        Camera1_Caps_Combo.Items.AddRange(capabilities.ToArray());
                        Camera1_Caps_Combo.SelectedIndex = 0;
                    }
                    else
                    {
                        Camera2_Name_Label.Text = device.Name;
                        Camera2_Path_Label.Text = device.MonikerString;

                        Camera2_Caps_Combo.Items.AddRange(capabilities.ToArray());
                        Camera2_Caps_Combo.SelectedIndex = 0;

                        DeviceExist = true;
                    }

                    index++;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);

                DeviceExist = false;

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
            captureInterval = Convert.ToDouble(this.Interval_Num.Value);
        }

        private void Camera2_Interval_Num_ValueChanged(object sender, EventArgs e)
        {
            captureInterval2 = Convert.ToDouble(this.Camera2_Interval_Num.Value);
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
            Task.Run(async () =>
            {
                await GenerateGifs();
            });
        }

        Task<bool> GenerateGifs()
        {
            for (int i = 0; i < 2; i++)
            {
                string readPath = Path.Combine(Application.StartupPath, "Snapshots", i.ToString());

                var imageFrames = Directory.EnumerateFiles(readPath, "*.jpg", SearchOption.TopDirectoryOnly);

                if (imageFrames.Count() > 0)
                {
                    using (MagickImageCollection collection = new MagickImageCollection())
                    {
                        var outputFps = Convert.ToInt32(this.OutputFps_Num.Value);
                        var animationDelay = /*1000 /*/ outputFps;

                        int curFrame = 0;

                        imageFrames.All(delegate (string fileName)
                        {
                            collection.Add(fileName);
                            collection[curFrame].AnimationDelay = animationDelay;

                            return true;
                        });

                        // Optionally reduce colors
                        QuantizeSettings settings = new QuantizeSettings();
                        settings.Colors = 32;
                        collection.Quantize(settings);

                        // Optionally optimize the images (images should have the same size).
                        //collection.Optimize();

                        var savePath = Path.Combine(Application.StartupPath, string.Format("Output-{0}-{1}.gif", i, DateTime.Now.ToFileTimeUtc()));
                        collection.Write(savePath);
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
        #region Dslr
        private void OpenSession()
        {
            RefreshCamera();

            if (CamList.Count > 0)
            {
                MainCamera = CamList[0];
                MainCamera.OpenSession();
                //MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
                MainCamera.ProgressChanged += MainCamera_ProgressChanged;
                MainCamera.StateChanged += MainCamera_StateChanged;
                MainCamera.DownloadReady += MainCamera_DownloadReady;

                //SessionButton.Text = "Close Session";
                //SessionLabel.Text = MainCamera.DeviceName;
                AvList = MainCamera.GetSettingsList(PropertyID.Av);
                TvList = MainCamera.GetSettingsList(PropertyID.Tv);
                ISOList = MainCamera.GetSettingsList(PropertyID.ISO);


                MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
                MainCamera.SetCapacity(4096, int.MaxValue);

                /*foreach (var Av in AvList) AvCoBox.Items.Add(Av.StringValue);
                foreach (var Tv in TvList) TvCoBox.Items.Add(Tv.StringValue);
                foreach (var ISO in ISOList) ISOCoBox.Items.Add(ISO.StringValue);
                AvCoBox.SelectedIndex = AvCoBox.Items.IndexOf(AvValues.GetValue(MainCamera.GetInt32Setting(PropertyID.Av)).StringValue);
                TvCoBox.SelectedIndex = TvCoBox.Items.IndexOf(TvValues.GetValue(MainCamera.GetInt32Setting(PropertyID.Tv)).StringValue);
                ISOCoBox.SelectedIndex = ISOCoBox.Items.IndexOf(ISOValues.GetValue(MainCamera.GetInt32Setting(PropertyID.ISO)).StringValue);
                SettingsGroupBox.Enabled = true;
                LiveViewGroupBox.Enabled = true;*/
            }
        }

        private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
            try
            {
                lock (LvLock)
                {
                    Evf_Bmp?.Dispose();
                    Evf_Bmp = new Bitmap(img);
                }
                //LiveViewPicBox.Invalidate();
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                string saveFolder = Path.Combine(Application.StartupPath, "Temp");
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                string dir = null;
                Invoke((Action)delegate { dir = saveFolder; });
                sender.DownloadFile(Info, dir);
                Invoke((Action)delegate { /*MainProgressBar.Value = 0;*/ });
            }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void ReportError(string message, bool lockdown)
        {
            int errc;
            lock (ErrLock) { errc = ++ErrCount; }

            if (lockdown) { /* EnableUI(false); */ }

            if (errc < 4) MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (errc == 4) MessageBox.Show("Many errors happened!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            lock (ErrLock) { ErrCount--; }
        }

        private void CloseSession()
        {
            MainCamera.CloseSession();
            //AvCoBox.Items.Clear();
            //TvCoBox.Items.Clear();
            //ISOCoBox.Items.Clear();
            //SettingsGroupBox.Enabled = false;
            //LiveViewGroupBox.Enabled = false;
            //SessionButton.Text = "Open Session";
            //SessionLabel.Text = "No open session";
            //LiveViewButton.Text = "Start LV";
        }

        private void RefreshCamera()
        {
            CamList = APIHandler.GetCameraList();
        }
        #endregion
        #region Dslr Api Events

        private void APIHandler_CameraAdded(CanonAPI sender)
        {
            try { Invoke((Action)delegate { RefreshCamera(); }); }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void MainCamera_StateChanged(Camera sender, StateEventID eventID, int parameter)
        {
            try { if (eventID == StateEventID.Shutdown && IsInit) { Invoke((Action)delegate { CloseSession(); }); } }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void MainCamera_ProgressChanged(object sender, int progress)
        {
            try { Invoke((Action)delegate { /*MainProgressBar.Value = progress;*/ }); }
            catch (Exception ex) { ReportError(ex.Message, false); }
        }

        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
            ReportError($"SDK Error code: {ex} ({((int)ex).ToString("X")})", false);
        }

        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
            ReportError(ex.Message, true);
        }

        #endregion
    }
}
