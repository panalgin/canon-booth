namespace CanonPhotoBooth
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RecordAsGif_Button = new System.Windows.Forms.Button();
            this.Cameras_Combo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Start_Button = new System.Windows.Forms.Button();
            this.Preview_Group = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Interval_Num = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.PreviewFps_Label = new System.Windows.Forms.Label();
            this.Output_Button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.OutputFps_Num = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.Preview_Group.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Interval_Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputFps_Num)).BeginInit();
            this.SuspendLayout();
            // 
            // RecordAsGif_Button
            // 
            this.RecordAsGif_Button.Location = new System.Drawing.Point(506, 77);
            this.RecordAsGif_Button.Name = "RecordAsGif_Button";
            this.RecordAsGif_Button.Size = new System.Drawing.Size(112, 23);
            this.RecordAsGif_Button.TabIndex = 1;
            this.RecordAsGif_Button.Text = "Record As Gif";
            this.RecordAsGif_Button.UseVisualStyleBackColor = true;
            this.RecordAsGif_Button.Click += new System.EventHandler(this.RecordAsGif_Button_Click);
            // 
            // Cameras_Combo
            // 
            this.Cameras_Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cameras_Combo.FormattingEnabled = true;
            this.Cameras_Combo.Location = new System.Drawing.Point(167, 16);
            this.Cameras_Combo.Name = "Cameras_Combo";
            this.Cameras_Combo.Size = new System.Drawing.Size(231, 21);
            this.Cameras_Combo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Detected Recording Devices:";
            // 
            // Start_Button
            // 
            this.Start_Button.Location = new System.Drawing.Point(404, 14);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(75, 23);
            this.Start_Button.TabIndex = 4;
            this.Start_Button.Text = "&Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // Preview_Group
            // 
            this.Preview_Group.Controls.Add(this.pictureBox1);
            this.Preview_Group.Location = new System.Drawing.Point(12, 136);
            this.Preview_Group.Name = "Preview_Group";
            this.Preview_Group.Size = new System.Drawing.Size(328, 259);
            this.Preview_Group.TabIndex = 5;
            this.Preview_Group.TabStop = false;
            this.Preview_Group.Text = "Live Preview";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 240);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Camera Running At:";
            // 
            // Interval_Num
            // 
            this.Interval_Num.Location = new System.Drawing.Point(167, 78);
            this.Interval_Num.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.Interval_Num.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.Interval_Num.Name = "Interval_Num";
            this.Interval_Num.Size = new System.Drawing.Size(120, 20);
            this.Interval_Num.TabIndex = 7;
            this.Interval_Num.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.Interval_Num.ValueChanged += new System.EventHandler(this.Interval_Num_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(245, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Capturing Interval:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(293, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Set";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // PreviewFps_Label
            // 
            this.PreviewFps_Label.AutoSize = true;
            this.PreviewFps_Label.Location = new System.Drawing.Point(57, 110);
            this.PreviewFps_Label.Name = "PreviewFps_Label";
            this.PreviewFps_Label.Size = new System.Drawing.Size(104, 13);
            this.PreviewFps_Label.TabIndex = 11;
            this.PreviewFps_Label.Text = "Preview Running At:";
            // 
            // Output_Button
            // 
            this.Output_Button.Location = new System.Drawing.Point(506, 183);
            this.Output_Button.Name = "Output_Button";
            this.Output_Button.Size = new System.Drawing.Size(75, 23);
            this.Output_Button.TabIndex = 12;
            this.Output_Button.Text = "Save Gif";
            this.Output_Button.UseVisualStyleBackColor = true;
            this.Output_Button.Click += new System.EventHandler(this.Output_Button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(435, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Output FPS:";
            // 
            // OutputFps_Num
            // 
            this.OutputFps_Num.Location = new System.Drawing.Point(506, 152);
            this.OutputFps_Num.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.OutputFps_Num.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.OutputFps_Num.Name = "OutputFps_Num";
            this.OutputFps_Num.Size = new System.Drawing.Size(112, 20);
            this.OutputFps_Num.TabIndex = 14;
            this.OutputFps_Num.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.Window;
            this.label6.Location = new System.Drawing.Point(576, 157);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "ms";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 445);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.OutputFps_Num);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Output_Button);
            this.Controls.Add(this.PreviewFps_Label);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Interval_Num);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Preview_Group);
            this.Controls.Add(this.Start_Button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cameras_Combo);
            this.Controls.Add(this.RecordAsGif_Button);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Canon Photo Booth - Test";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Preview_Group.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Interval_Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputFps_Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button RecordAsGif_Button;
        private System.Windows.Forms.ComboBox Cameras_Combo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.GroupBox Preview_Group;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.NumericUpDown Interval_Num;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label PreviewFps_Label;
        private System.Windows.Forms.Button Output_Button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown OutputFps_Num;
        private System.Windows.Forms.Label label6;
    }
}

