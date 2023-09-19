namespace WindowsFormsApp1
{
    partial class Soapgrabber
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.filepathInput = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.consoleOutput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(307, 64);
            this.button1.TabIndex = 0;
            this.button1.Text = "Grab Notice IDs";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.GrabIDList);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 190);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(307, 64);
            this.button2.TabIndex = 1;
            this.button2.Text = "Grab Notice Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.GrabNoticesFromIDList);
            // 
            // filepathInput
            // 
            this.filepathInput.Location = new System.Drawing.Point(12, 25);
            this.filepathInput.Name = "filepathInput";
            this.filepathInput.Size = new System.Drawing.Size(307, 20);
            this.filepathInput.TabIndex = 2;
            this.filepathInput.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.maskedTextBox1_MaskInputRejected);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Specify Output Filepath: (Default: C:/Soapgrabber)";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 51);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(307, 64);
            this.button3.TabIndex = 4;
            this.button3.Text = "Submit Path";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.SubmitNewFilePath);
            // 
            // consoleOutput
            // 
            this.consoleOutput.Location = new System.Drawing.Point(335, 25);
            this.consoleOutput.Name = "consoleOutput";
            this.consoleOutput.Size = new System.Drawing.Size(412, 326);
            this.consoleOutput.TabIndex = 5;
            this.consoleOutput.Text = "";
            this.consoleOutput.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // Soapgrabber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 366);
            this.Controls.Add(this.consoleOutput);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filepathInput);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Soapgrabber";
            this.Text = "Simap-Soapgrabber";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.MaskedTextBox filepathInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox consoleOutput;
    }
}

