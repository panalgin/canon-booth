﻿namespace CanonPhotoBooth
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
            this.QuickFrame_Button = new System.Windows.Forms.Button();
            this.Cameras_Combo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Start_Button = new System.Windows.Forms.Button();
            this.Preview_Group = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Interval_Num = new System.Windows.Forms.NumericUpDown();
            this.Preview_Group.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Interval_Num)).BeginInit();
            this.SuspendLayout();
            // 
            // QuickFrame_Button
            // 
            this.QuickFrame_Button.Location = new System.Drawing.Point(506, 63);
            this.QuickFrame_Button.Name = "QuickFrame_Button";
            this.QuickFrame_Button.Size = new System.Drawing.Size(112, 23);
            this.QuickFrame_Button.TabIndex = 1;
            this.QuickFrame_Button.Text = "Take A Snapshot";
            this.QuickFrame_Button.UseVisualStyleBackColor = true;
            this.QuickFrame_Button.Click += new System.EventHandler(this.QuickFrame_Button_Click);
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
            this.Preview_Group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Preview_Group.Controls.Add(this.pictureBox1);
            this.Preview_Group.Location = new System.Drawing.Point(16, 86);
            this.Preview_Group.Name = "Preview_Group";
            this.Preview_Group.Size = new System.Drawing.Size(358, 293);
            this.Preview_Group.TabIndex = 5;
            this.Preview_Group.TabStop = false;
            this.Preview_Group.Text = "Live Preview";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(352, 274);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 6;
            // 
            // Interval_Num
            // 
            this.Interval_Num.Location = new System.Drawing.Point(167, 43);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 445);
            this.Controls.Add(this.Interval_Num);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Preview_Group);
            this.Controls.Add(this.Start_Button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cameras_Combo);
            this.Controls.Add(this.QuickFrame_Button);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Canon";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Preview_Group.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Interval_Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button QuickFrame_Button;
        private System.Windows.Forms.ComboBox Cameras_Combo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.GroupBox Preview_Group;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.NumericUpDown Interval_Num;
    }
}
