
namespace WakeOnLANClient
{
	partial class FormMain
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.textBox_Server_IP = new System.Windows.Forms.TextBox();
            this.label_server_IP = new System.Windows.Forms.Label();
            this.label_MAC_Address = new System.Windows.Forms.Label();
            this.textBox_MAC_Address = new System.Windows.Forms.TextBox();
            this.label_log = new System.Windows.Forms.Label();
            this.richTextBox_log = new System.Windows.Forms.RichTextBox();
            this.button_wake = new System.Windows.Forms.Button();
            this.timer_server_ping = new System.Windows.Forms.Timer(this.components);
            this.label_serverStatus = new System.Windows.Forms.Label();
            this.timer_machine_ping = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textBox_Server_IP
            // 
            this.textBox_Server_IP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Server_IP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox_Server_IP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Server_IP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.textBox_Server_IP.Location = new System.Drawing.Point(135, 4);
            this.textBox_Server_IP.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Server_IP.Name = "textBox_Server_IP";
            this.textBox_Server_IP.Size = new System.Drawing.Size(324, 23);
            this.textBox_Server_IP.TabIndex = 1;
            this.textBox_Server_IP.Text = "192.168.177.123";
            // 
            // label_server_IP
            // 
            this.label_server_IP.AutoSize = true;
            this.label_server_IP.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_server_IP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_server_IP.Location = new System.Drawing.Point(4, 3);
            this.label_server_IP.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_server_IP.Name = "label_server_IP";
            this.label_server_IP.Size = new System.Drawing.Size(86, 24);
            this.label_server_IP.TabIndex = 1;
            this.label_server_IP.Text = "Server IP";
            // 
            // label_MAC_Address
            // 
            this.label_MAC_Address.AutoSize = true;
            this.label_MAC_Address.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_MAC_Address.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_MAC_Address.Location = new System.Drawing.Point(4, 23);
            this.label_MAC_Address.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MAC_Address.Name = "label_MAC_Address";
            this.label_MAC_Address.Size = new System.Drawing.Size(127, 24);
            this.label_MAC_Address.TabIndex = 2;
            this.label_MAC_Address.Text = "MAC Address";
            // 
            // textBox_MAC_Address
            // 
            this.textBox_MAC_Address.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_MAC_Address.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox_MAC_Address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_MAC_Address.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.textBox_MAC_Address.Location = new System.Drawing.Point(135, 26);
            this.textBox_MAC_Address.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_MAC_Address.Name = "textBox_MAC_Address";
            this.textBox_MAC_Address.Size = new System.Drawing.Size(324, 23);
            this.textBox_MAC_Address.TabIndex = 2;
            this.textBox_MAC_Address.Text = "00-00-00-00-00-00";
            // 
            // label_log
            // 
            this.label_log.AutoSize = true;
            this.label_log.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_log.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_log.Location = new System.Drawing.Point(4, 58);
            this.label_log.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_log.Name = "label_log";
            this.label_log.Size = new System.Drawing.Size(28, 15);
            this.label_log.TabIndex = 4;
            this.label_log.Text = "Log";
            // 
            // richTextBox_log
            // 
            this.richTextBox_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_log.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox_log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_log.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.richTextBox_log.Location = new System.Drawing.Point(8, 75);
            this.richTextBox_log.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox_log.Name = "richTextBox_log";
            this.richTextBox_log.ReadOnly = true;
            this.richTextBox_log.Size = new System.Drawing.Size(452, 154);
            this.richTextBox_log.TabIndex = 5;
            this.richTextBox_log.TabStop = false;
            this.richTextBox_log.Text = "";
            // 
            // button_wake
            // 
            this.button_wake.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_wake.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button_wake.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_wake.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button_wake.Location = new System.Drawing.Point(382, 52);
            this.button_wake.Margin = new System.Windows.Forms.Padding(2);
            this.button_wake.Name = "button_wake";
            this.button_wake.Size = new System.Drawing.Size(76, 20);
            this.button_wake.TabIndex = 0;
            this.button_wake.Text = "Wake";
            this.button_wake.UseVisualStyleBackColor = false;
            this.button_wake.Click += new System.EventHandler(this.button_wake_Click);
            // 
            // timer_server_ping
            // 
            this.timer_server_ping.Interval = 1000;
            this.timer_server_ping.Tick += new System.EventHandler(this.timer_server_ping_Tick);
            // 
            // label_serverStatus
            // 
            this.label_serverStatus.AutoSize = true;
            this.label_serverStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label_serverStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_serverStatus.Location = new System.Drawing.Point(258, 54);
            this.label_serverStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_serverStatus.Name = "label_serverStatus";
            this.label_serverStatus.Size = new System.Drawing.Size(104, 15);
            this.label_serverStatus.TabIndex = 6;
            this.label_serverStatus.Text = "Server Connected";
            // 
            // timer_machine_ping
            // 
            this.timer_machine_ping.Interval = 1000;
            this.timer_machine_ping.Tick += new System.EventHandler(this.timer_machine_ping_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(467, 235);
            this.Controls.Add(this.label_serverStatus);
            this.Controls.Add(this.button_wake);
            this.Controls.Add(this.richTextBox_log);
            this.Controls.Add(this.label_log);
            this.Controls.Add(this.textBox_MAC_Address);
            this.Controls.Add(this.label_MAC_Address);
            this.Controls.Add(this.label_server_IP);
            this.Controls.Add(this.textBox_Server_IP);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.Text = "Wake On LAN Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_Server_IP;
		private System.Windows.Forms.Label label_server_IP;
		private System.Windows.Forms.Label label_MAC_Address;
		private System.Windows.Forms.TextBox textBox_MAC_Address;
		private System.Windows.Forms.Label label_log;
		private System.Windows.Forms.RichTextBox richTextBox_log;
		private System.Windows.Forms.Button button_wake;
        private System.Windows.Forms.Timer timer_server_ping;
        private System.Windows.Forms.Label label_serverStatus;
        private System.Windows.Forms.Timer timer_machine_ping;
    }
}

