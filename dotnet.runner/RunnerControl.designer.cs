namespace dotnet.runner
{
    partial class RunnerControl
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
            if (Running)
            {
                Stop("Disposing");
            }

            if (_watcher != null)
            {
                _watcher.Changed -= Compiling;
                _watcher.Created -= Compiling;
                _watcher.Deleted -= Compiling;
                _watcher.Renamed -= Compiling;
                _watcher.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxWatch = new System.Windows.Forms.CheckBox();
            this.textBoxWorkingDirectory = new System.Windows.Forms.TextBox();
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxWatch);
            this.panel1.Controls.Add(this.textBoxWorkingDirectory);
            this.panel1.Controls.Add(this.buttonStartStop);
            this.panel1.Controls.Add(this.richTextBoxOutput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 317);
            this.panel1.TabIndex = 0;
            // 
            // checkBoxWatch
            // 
            this.checkBoxWatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxWatch.AutoSize = true;
            this.checkBoxWatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxWatch.Location = new System.Drawing.Point(611, 7);
            this.checkBoxWatch.Name = "checkBoxWatch";
            this.checkBoxWatch.Size = new System.Drawing.Size(70, 21);
            this.checkBoxWatch.TabIndex = 2;
            this.checkBoxWatch.Text = "Watch";
            this.checkBoxWatch.UseVisualStyleBackColor = true;
            // 
            // textBoxWorkingDirectory
            // 
            this.textBoxWorkingDirectory.Location = new System.Drawing.Point(3, 6);
            this.textBoxWorkingDirectory.Name = "textBoxWorkingDirectory";
            this.textBoxWorkingDirectory.ReadOnly = true;
            this.textBoxWorkingDirectory.Size = new System.Drawing.Size(589, 22);
            this.textBoxWorkingDirectory.TabIndex = 1;
            this.textBoxWorkingDirectory.TabStop = false;
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStartStop.AutoSize = true;
            this.buttonStartStop.Location = new System.Drawing.Point(687, 3);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(75, 27);
            this.buttonStartStop.TabIndex = 3;
            this.buttonStartStop.Text = "Start";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxOutput.BackColor = System.Drawing.Color.Black;
            this.richTextBoxOutput.ForeColor = System.Drawing.Color.White;
            this.richTextBoxOutput.Location = new System.Drawing.Point(0, 33);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.ShowSelectionMargin = true;
            this.richTextBoxOutput.Size = new System.Drawing.Size(765, 283);
            this.richTextBoxOutput.TabIndex = 4;
            this.richTextBoxOutput.Text = "";
            this.richTextBoxOutput.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxOutput_LinkClicked);
            // 
            // RunnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "RunnerControl";
            this.Size = new System.Drawing.Size(765, 317);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonStartStop;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.TextBox textBoxWorkingDirectory;
        private System.Windows.Forms.CheckBox checkBoxWatch;
    }
}
