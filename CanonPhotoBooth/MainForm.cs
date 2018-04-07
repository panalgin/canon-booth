using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var cameras = this.webCameraControl1.GetVideoCaptureDevices();
            var cameraItems = new List<ComboBoxWebcamItem>();

            foreach(WebCameraId id in cameras)
            {
                ComboBoxWebcamItem item = new ComboBoxWebcamItem(id);
                cameraItems.Add(item);
            }

            this.Cameras_Combo.DataSource = cameraItems;
            this.Cameras_Combo.DisplayMember = "Value";
            this.Cameras_Combo.ValueMember = "ID";
        }

        private void QuickFrame_Button_Click(object sender, EventArgs e)
        {

        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            WebCameraId selected = (Cameras_Combo.SelectedItem as ComboBoxWebcamItem).ID;

            if (selected != null)
            {
                this.webCameraControl1.StartCapture(selected);
            }
        }
    }
}
