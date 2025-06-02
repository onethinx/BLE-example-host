namespace BLE_App
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btBLEscan = new System.Windows.Forms.Button();
            this.cbScannedDevices = new System.Windows.Forms.ComboBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.tbSend = new System.Windows.Forms.TextBox();
            this.btSend = new System.Windows.Forms.Button();
            this.pnlProgress = new BLE_App.DoubleBufferedPanel();
            this.btSendLoRa = new System.Windows.Forms.Button();
            this.btJoin = new System.Windows.Forms.Button();
            this.pnlBLE = new System.Windows.Forms.Panel();
            this.pnlBLE.SuspendLayout();
            this.SuspendLayout();
            // 
            // btBLEscan
            // 
            this.btBLEscan.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btBLEscan.Location = new System.Drawing.Point(12, 12);
            this.btBLEscan.Name = "btBLEscan";
            this.btBLEscan.Size = new System.Drawing.Size(192, 73);
            this.btBLEscan.TabIndex = 0;
            this.btBLEscan.Text = "Scan for BLE devices";
            this.btBLEscan.UseVisualStyleBackColor = true;
            this.btBLEscan.Click += new System.EventHandler(this.btBLEscan_Click);
            // 
            // cbScannedDevices
            // 
            this.cbScannedDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScannedDevices.FormattingEnabled = true;
            this.cbScannedDevices.Location = new System.Drawing.Point(210, 14);
            this.cbScannedDevices.Name = "cbScannedDevices";
            this.cbScannedDevices.Size = new System.Drawing.Size(192, 21);
            this.cbScannedDevices.TabIndex = 2;
            // 
            // btConnect
            // 
            this.btConnect.Enabled = false;
            this.btConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btConnect.Location = new System.Drawing.Point(210, 41);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(192, 44);
            this.btConnect.TabIndex = 3;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
            // 
            // rtbInfo
            // 
            this.rtbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbInfo.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbInfo.Location = new System.Drawing.Point(12, 107);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(839, 440);
            this.rtbInfo.TabIndex = 4;
            this.rtbInfo.Text = "";
            // 
            // tbSend
            // 
            this.tbSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSend.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSend.Location = new System.Drawing.Point(102, 6);
            this.tbSend.Name = "tbSend";
            this.tbSend.Size = new System.Drawing.Size(341, 16);
            this.tbSend.TabIndex = 6;
            this.tbSend.Text = "Hello World!";
            // 
            // btSend
            // 
            this.btSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btSend.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSend.Location = new System.Drawing.Point(100, 30);
            this.btSend.Name = "btSend";
            this.btSend.Size = new System.Drawing.Size(170, 44);
            this.btSend.TabIndex = 5;
            this.btSend.Text = "Send over UART";
            this.btSend.UseVisualStyleBackColor = true;
            this.btSend.Click += new System.EventHandler(this.btSend_Click);
            // 
            // pnlProgress
            // 
            this.pnlProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlProgress.BackColor = System.Drawing.SystemColors.Control;
            this.pnlProgress.Location = new System.Drawing.Point(12, 91);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(839, 10);
            this.pnlProgress.TabIndex = 119;
            this.pnlProgress.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlProgress_Paint);
            // 
            // btSendLoRa
            // 
            this.btSendLoRa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btSendLoRa.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btSendLoRa.Location = new System.Drawing.Point(273, 30);
            this.btSendLoRa.Name = "btSendLoRa";
            this.btSendLoRa.Size = new System.Drawing.Size(170, 44);
            this.btSendLoRa.TabIndex = 120;
            this.btSendLoRa.Text = "Send over LoRaWAN";
            this.btSendLoRa.UseVisualStyleBackColor = true;
            this.btSendLoRa.Click += new System.EventHandler(this.btSendLoRa_Click);
            // 
            // btJoin
            // 
            this.btJoin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btJoin.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btJoin.Location = new System.Drawing.Point(0, 1);
            this.btJoin.Name = "btJoin";
            this.btJoin.Size = new System.Drawing.Size(94, 73);
            this.btJoin.TabIndex = 121;
            this.btJoin.Text = "LoRaWAN Join";
            this.btJoin.UseVisualStyleBackColor = true;
            this.btJoin.Click += new System.EventHandler(this.btJoin_Click);
            // 
            // pnlBLE
            // 
            this.pnlBLE.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBLE.Controls.Add(this.btJoin);
            this.pnlBLE.Controls.Add(this.btSendLoRa);
            this.pnlBLE.Controls.Add(this.tbSend);
            this.pnlBLE.Controls.Add(this.btSend);
            this.pnlBLE.Enabled = false;
            this.pnlBLE.Location = new System.Drawing.Point(408, 11);
            this.pnlBLE.Name = "pnlBLE";
            this.pnlBLE.Size = new System.Drawing.Size(451, 80);
            this.pnlBLE.TabIndex = 122;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(863, 559);
            this.Controls.Add(this.pnlBLE);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.rtbInfo);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.cbScannedDevices);
            this.Controls.Add(this.btBLEscan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Onethinx BLE Demo App V1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.pnlBLE.ResumeLayout(false);
            this.pnlBLE.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btBLEscan;
        private System.Windows.Forms.ComboBox cbScannedDevices;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.TextBox tbSend;
        private System.Windows.Forms.Button btSend;
        private System.Windows.Forms.Button btSendLoRa;
        private System.Windows.Forms.Button btJoin;
        private System.Windows.Forms.Panel pnlBLE;
        private DoubleBufferedPanel pnlProgress;
    }
}