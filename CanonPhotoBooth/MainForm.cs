using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebEye.Controls.WinForms.WebCameraControl;

namespace CanonPhotoBooth
{
    public partial class MainForm : Form
    {
        public WebCameraControl Camera = new WebCameraControl();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var cameras = Camera.GetVideoCaptureDevices();
        }
    }
}
