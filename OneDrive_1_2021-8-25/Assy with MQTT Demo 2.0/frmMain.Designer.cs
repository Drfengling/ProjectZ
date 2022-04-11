namespace Demo
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnMQTTDisconnect = new System.Windows.Forms.Button();
            this.btnMQTTConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMQTTPassword = new System.Windows.Forms.TextBox();
            this.txtMQTTUserName = new System.Windows.Forms.TextBox();
            this.txtMQTTHostName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtMAC = new System.Windows.Forms.TextBox();
            this.txtEMT = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtOEETopic = new System.Windows.Forms.TextBox();
            this.btnUploadOEE = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtOEECavity = new System.Windows.Forms.TextBox();
            this.txtOEEErrorCode = new System.Windows.Forms.TextBox();
            this.txtOEEScanCount = new System.Windows.Forms.TextBox();
            this.txtOEESwVersion = new System.Windows.Forms.TextBox();
            this.txtOEEActualCT = new System.Windows.Forms.TextBox();
            this.txtOEEStatus = new System.Windows.Forms.TextBox();
            this.txtOEEEndTime = new System.Windows.Forms.TextBox();
            this.txtOEEStartTime = new System.Windows.Forms.TextBox();
            this.txtOEEFixture = new System.Windows.Forms.TextBox();
            this.txtOEEBGBarcode = new System.Windows.Forms.TextBox();
            this.txtOeeSerialNumber = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtDTTopic = new System.Windows.Forms.TextBox();
            this.btnUploadDowntime = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtDTModuleCode = new System.Windows.Forms.TextBox();
            this.txtDTErrorCode = new System.Windows.Forms.TextBox();
            this.txtDTStatus = new System.Windows.Forms.TextBox();
            this.txtDTTotalNum = new System.Windows.Forms.TextBox();
            this.txtDTPoorNum = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.btnUploadPlant = new System.Windows.Forms.Button();
            this.txtPlantTopic = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.lbGetServerTime = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnMQTTDisconnect);
            this.groupBox1.Controls.Add(this.btnMQTTConnect);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtMQTTPassword);
            this.groupBox1.Controls.Add(this.txtMQTTUserName);
            this.groupBox1.Controls.Add(this.txtMQTTHostName);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(306, 5);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(443, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MQTT 设定";
            // 
            // btnMQTTDisconnect
            // 
            this.btnMQTTDisconnect.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnMQTTDisconnect.Enabled = false;
            this.btnMQTTDisconnect.Location = new System.Drawing.Point(337, 71);
            this.btnMQTTDisconnect.Name = "btnMQTTDisconnect";
            this.btnMQTTDisconnect.Size = new System.Drawing.Size(92, 31);
            this.btnMQTTDisconnect.TabIndex = 2;
            this.btnMQTTDisconnect.Text = "断线";
            this.btnMQTTDisconnect.UseVisualStyleBackColor = false;
            this.btnMQTTDisconnect.Click += new System.EventHandler(this.btnMQTTDisconnect_Click);
            // 
            // btnMQTTConnect
            // 
            this.btnMQTTConnect.BackColor = System.Drawing.Color.Teal;
            this.btnMQTTConnect.Location = new System.Drawing.Point(337, 25);
            this.btnMQTTConnect.Name = "btnMQTTConnect";
            this.btnMQTTConnect.Size = new System.Drawing.Size(92, 32);
            this.btnMQTTConnect.TabIndex = 2;
            this.btnMQTTConnect.Text = "联机";
            this.btnMQTTConnect.UseVisualStyleBackColor = false;
            this.btnMQTTConnect.Click += new System.EventHandler(this.btnMQTTConnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 81);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "UserName";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "HostName";
            // 
            // txtMQTTPassword
            // 
            this.txtMQTTPassword.Location = new System.Drawing.Point(118, 78);
            this.txtMQTTPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtMQTTPassword.Name = "txtMQTTPassword";
            this.txtMQTTPassword.Size = new System.Drawing.Size(210, 21);
            this.txtMQTTPassword.TabIndex = 0;
            this.txtMQTTPassword.Text = "jgp123";
            // 
            // txtMQTTUserName
            // 
            this.txtMQTTUserName.Location = new System.Drawing.Point(118, 50);
            this.txtMQTTUserName.Margin = new System.Windows.Forms.Padding(2);
            this.txtMQTTUserName.Name = "txtMQTTUserName";
            this.txtMQTTUserName.Size = new System.Drawing.Size(210, 21);
            this.txtMQTTUserName.TabIndex = 0;
            this.txtMQTTUserName.Text = "assembly";
            // 
            // txtMQTTHostName
            // 
            this.txtMQTTHostName.Location = new System.Drawing.Point(118, 22);
            this.txtMQTTHostName.Margin = new System.Windows.Forms.Padding(2);
            this.txtMQTTHostName.Name = "txtMQTTHostName";
            this.txtMQTTHostName.Size = new System.Drawing.Size(210, 21);
            this.txtMQTTHostName.TabIndex = 0;
            this.txtMQTTHostName.Text = "CNWXMQTTAP01";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtIP);
            this.groupBox2.Controls.Add(this.txtMAC);
            this.groupBox2.Controls.Add(this.txtEMT);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(5, 5);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(297, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基本机台信息";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 85);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "IP";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 57);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "MAC";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 29);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "EMT";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(74, 81);
            this.txtIP.Margin = new System.Windows.Forms.Padding(2);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(210, 21);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "192.168.0.13";
            // 
            // txtMAC
            // 
            this.txtMAC.Location = new System.Drawing.Point(74, 53);
            this.txtMAC.Margin = new System.Windows.Forms.Padding(2);
            this.txtMAC.Name = "txtMAC";
            this.txtMAC.Size = new System.Drawing.Size(210, 21);
            this.txtMAC.TabIndex = 2;
            this.txtMAC.Text = "64-51-06-55-C2-55";
            // 
            // txtEMT
            // 
            this.txtEMT.Location = new System.Drawing.Point(74, 25);
            this.txtEMT.Margin = new System.Windows.Forms.Padding(2);
            this.txtEMT.Name = "txtEMT";
            this.txtEMT.Size = new System.Drawing.Size(210, 21);
            this.txtEMT.TabIndex = 2;
            this.txtEMT.Text = "T202202160001";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.txtOEETopic);
            this.groupBox3.Controls.Add(this.btnUploadOEE);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtOEECavity);
            this.groupBox3.Controls.Add(this.txtOEEErrorCode);
            this.groupBox3.Controls.Add(this.txtOEEScanCount);
            this.groupBox3.Controls.Add(this.txtOEESwVersion);
            this.groupBox3.Controls.Add(this.txtOEEActualCT);
            this.groupBox3.Controls.Add(this.txtOEEStatus);
            this.groupBox3.Controls.Add(this.txtOEEEndTime);
            this.groupBox3.Controls.Add(this.txtOEEStartTime);
            this.groupBox3.Controls.Add(this.txtOEEFixture);
            this.groupBox3.Controls.Add(this.txtOEEBGBarcode);
            this.groupBox3.Controls.Add(this.txtOeeSerialNumber);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(4, 133);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(741, 219);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "OEE";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(8, 22);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(65, 15);
            this.label23.TabIndex = 8;
            this.label23.Text = "OEE Topic";
            // 
            // txtOEETopic
            // 
            this.txtOEETopic.BackColor = System.Drawing.Color.Khaki;
            this.txtOEETopic.Enabled = false;
            this.txtOEETopic.Location = new System.Drawing.Point(141, 19);
            this.txtOEETopic.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEETopic.Name = "txtOEETopic";
            this.txtOEETopic.Size = new System.Drawing.Size(577, 21);
            this.txtOEETopic.TabIndex = 7;
            // 
            // btnUploadOEE
            // 
            this.btnUploadOEE.Location = new System.Drawing.Point(585, 183);
            this.btnUploadOEE.Name = "btnUploadOEE";
            this.btnUploadOEE.Size = new System.Drawing.Size(135, 27);
            this.btnUploadOEE.TabIndex = 6;
            this.btnUploadOEE.Text = "上传 oee";
            this.btnUploadOEE.UseVisualStyleBackColor = true;
            this.btnUploadOEE.Click += new System.EventHandler(this.btnUploadOEE_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(430, 162);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(38, 15);
            this.label17.TabIndex = 5;
            this.label17.Text = "Cavity";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(430, 134);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 15);
            this.label16.TabIndex = 5;
            this.label16.Text = "ErrorCode";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(430, 106);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 15);
            this.label15.TabIndex = 5;
            this.label15.Text = "ScanCount";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(430, 77);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 15);
            this.label14.TabIndex = 5;
            this.label14.Text = "SwVersion";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(430, 51);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 15);
            this.label13.TabIndex = 5;
            this.label13.Text = "ActualCT";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 188);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "Status";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 161);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 15);
            this.label11.TabIndex = 5;
            this.label11.Text = "EndTime";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 133);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 15);
            this.label10.TabIndex = 5;
            this.label10.Text = "StartTime";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 106);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 15);
            this.label9.TabIndex = 5;
            this.label9.Text = "Fixture";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 79);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 15);
            this.label8.TabIndex = 5;
            this.label8.Text = "BGBarcode";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 50);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "SerialNumber";
            // 
            // txtOEECavity
            // 
            this.txtOEECavity.Location = new System.Drawing.Point(560, 158);
            this.txtOEECavity.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEECavity.Name = "txtOEECavity";
            this.txtOEECavity.Size = new System.Drawing.Size(160, 21);
            this.txtOEECavity.TabIndex = 4;
            this.txtOEECavity.Text = "1";
            // 
            // txtOEEErrorCode
            // 
            this.txtOEEErrorCode.Location = new System.Drawing.Point(560, 130);
            this.txtOEEErrorCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEErrorCode.Name = "txtOEEErrorCode";
            this.txtOEEErrorCode.Size = new System.Drawing.Size(160, 21);
            this.txtOEEErrorCode.TabIndex = 4;
            this.txtOEEErrorCode.Text = "12012003";
            // 
            // txtOEEScanCount
            // 
            this.txtOEEScanCount.Location = new System.Drawing.Point(560, 102);
            this.txtOEEScanCount.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEScanCount.Name = "txtOEEScanCount";
            this.txtOEEScanCount.Size = new System.Drawing.Size(160, 21);
            this.txtOEEScanCount.TabIndex = 4;
            this.txtOEEScanCount.Text = "1";
            // 
            // txtOEESwVersion
            // 
            this.txtOEESwVersion.Location = new System.Drawing.Point(560, 73);
            this.txtOEESwVersion.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEESwVersion.Name = "txtOEESwVersion";
            this.txtOEESwVersion.Size = new System.Drawing.Size(160, 21);
            this.txtOEESwVersion.TabIndex = 4;
            this.txtOEESwVersion.Text = "V1.113";
            // 
            // txtOEEActualCT
            // 
            this.txtOEEActualCT.Location = new System.Drawing.Point(560, 47);
            this.txtOEEActualCT.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEActualCT.Name = "txtOEEActualCT";
            this.txtOEEActualCT.Size = new System.Drawing.Size(160, 21);
            this.txtOEEActualCT.TabIndex = 4;
            this.txtOEEActualCT.Text = "11.5";
            // 
            // txtOEEStatus
            // 
            this.txtOEEStatus.Location = new System.Drawing.Point(141, 184);
            this.txtOEEStatus.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEStatus.Name = "txtOEEStatus";
            this.txtOEEStatus.Size = new System.Drawing.Size(268, 21);
            this.txtOEEStatus.TabIndex = 4;
            this.txtOEEStatus.Text = "OK";
            // 
            // txtOEEEndTime
            // 
            this.txtOEEEndTime.Location = new System.Drawing.Point(141, 157);
            this.txtOEEEndTime.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEEndTime.Name = "txtOEEEndTime";
            this.txtOEEEndTime.Size = new System.Drawing.Size(268, 21);
            this.txtOEEEndTime.TabIndex = 4;
            this.txtOEEEndTime.Text = "2020-11-17 23:59:52.123";
            // 
            // txtOEEStartTime
            // 
            this.txtOEEStartTime.Location = new System.Drawing.Point(141, 129);
            this.txtOEEStartTime.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEStartTime.Name = "txtOEEStartTime";
            this.txtOEEStartTime.Size = new System.Drawing.Size(268, 21);
            this.txtOEEStartTime.TabIndex = 4;
            this.txtOEEStartTime.Text = "2020-11-17 23:59:09.324";
            // 
            // txtOEEFixture
            // 
            this.txtOEEFixture.Location = new System.Drawing.Point(141, 102);
            this.txtOEEFixture.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEFixture.Name = "txtOEEFixture";
            this.txtOEEFixture.Size = new System.Drawing.Size(268, 21);
            this.txtOEEFixture.TabIndex = 4;
            this.txtOEEFixture.Text = "BT-AAF-A024-000010";
            // 
            // txtOEEBGBarcode
            // 
            this.txtOEEBGBarcode.Location = new System.Drawing.Point(141, 75);
            this.txtOEEBGBarcode.Margin = new System.Windows.Forms.Padding(2);
            this.txtOEEBGBarcode.Name = "txtOEEBGBarcode";
            this.txtOEEBGBarcode.Size = new System.Drawing.Size(268, 21);
            this.txtOEEBGBarcode.TabIndex = 4;
            // 
            // txtOeeSerialNumber
            // 
            this.txtOeeSerialNumber.Location = new System.Drawing.Point(141, 47);
            this.txtOeeSerialNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txtOeeSerialNumber.Name = "txtOeeSerialNumber";
            this.txtOeeSerialNumber.Size = new System.Drawing.Size(268, 21);
            this.txtOeeSerialNumber.TabIndex = 4;
            this.txtOeeSerialNumber.Text = "FM7045600SDPQY0AN23";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.txtDTTopic);
            this.groupBox4.Controls.Add(this.btnUploadDowntime);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txtDTModuleCode);
            this.groupBox4.Controls.Add(this.txtDTErrorCode);
            this.groupBox4.Controls.Add(this.txtDTStatus);
            this.groupBox4.Controls.Add(this.txtDTTotalNum);
            this.groupBox4.Controls.Add(this.txtDTPoorNum);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(4, 366);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(741, 136);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Downtime";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(8, 23);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 15);
            this.label24.TabIndex = 10;
            this.label24.Text = "DT Topic";
            // 
            // txtDTTopic
            // 
            this.txtDTTopic.BackColor = System.Drawing.Color.Khaki;
            this.txtDTTopic.Enabled = false;
            this.txtDTTopic.Location = new System.Drawing.Point(141, 19);
            this.txtDTTopic.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTTopic.Name = "txtDTTopic";
            this.txtDTTopic.Size = new System.Drawing.Size(577, 21);
            this.txtDTTopic.TabIndex = 9;
            // 
            // btnUploadDowntime
            // 
            this.btnUploadDowntime.Location = new System.Drawing.Point(560, 101);
            this.btnUploadDowntime.Name = "btnUploadDowntime";
            this.btnUploadDowntime.Size = new System.Drawing.Size(160, 27);
            this.btnUploadDowntime.TabIndex = 8;
            this.btnUploadDowntime.Text = "上传 Downtime";
            this.btnUploadDowntime.UseVisualStyleBackColor = true;
            this.btnUploadDowntime.Click += new System.EventHandler(this.btnUploadDowntime_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(369, 78);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(78, 15);
            this.label22.TabIndex = 7;
            this.label22.Text = "ModuleCode";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(369, 49);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(63, 15);
            this.label21.TabIndex = 7;
            this.label21.Text = "ErrorCode";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 104);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 15);
            this.label20.TabIndex = 7;
            this.label20.Text = "Status";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(8, 77);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(61, 15);
            this.label19.TabIndex = 7;
            this.label19.Text = "TotalNum";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(8, 49);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(60, 15);
            this.label18.TabIndex = 7;
            this.label18.Text = "PoorNum";
            // 
            // txtDTModuleCode
            // 
            this.txtDTModuleCode.Location = new System.Drawing.Point(508, 74);
            this.txtDTModuleCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTModuleCode.Name = "txtDTModuleCode";
            this.txtDTModuleCode.Size = new System.Drawing.Size(210, 21);
            this.txtDTModuleCode.TabIndex = 6;
            this.txtDTModuleCode.Text = "21001";
            // 
            // txtDTErrorCode
            // 
            this.txtDTErrorCode.Location = new System.Drawing.Point(508, 45);
            this.txtDTErrorCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTErrorCode.Name = "txtDTErrorCode";
            this.txtDTErrorCode.Size = new System.Drawing.Size(210, 21);
            this.txtDTErrorCode.TabIndex = 6;
            this.txtDTErrorCode.Text = "12012003";
            // 
            // txtDTStatus
            // 
            this.txtDTStatus.Location = new System.Drawing.Point(141, 101);
            this.txtDTStatus.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTStatus.Name = "txtDTStatus";
            this.txtDTStatus.Size = new System.Drawing.Size(210, 21);
            this.txtDTStatus.TabIndex = 6;
            this.txtDTStatus.Text = "1";
            // 
            // txtDTTotalNum
            // 
            this.txtDTTotalNum.Location = new System.Drawing.Point(141, 73);
            this.txtDTTotalNum.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTTotalNum.Name = "txtDTTotalNum";
            this.txtDTTotalNum.Size = new System.Drawing.Size(210, 21);
            this.txtDTTotalNum.TabIndex = 6;
            this.txtDTTotalNum.Text = "35";
            // 
            // txtDTPoorNum
            // 
            this.txtDTPoorNum.Location = new System.Drawing.Point(141, 45);
            this.txtDTPoorNum.Margin = new System.Windows.Forms.Padding(2);
            this.txtDTPoorNum.Name = "txtDTPoorNum";
            this.txtDTPoorNum.Size = new System.Drawing.Size(210, 21);
            this.txtDTPoorNum.TabIndex = 6;
            this.txtDTPoorNum.Text = "11";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label25);
            this.groupBox5.Controls.Add(this.btnUploadPlant);
            this.groupBox5.Controls.Add(this.txtPlantTopic);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(4, 516);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(741, 52);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "上传心跳";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(14, 22);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(61, 15);
            this.label25.TabIndex = 12;
            this.label25.Text = "心跳Topic";
            // 
            // btnUploadPlant
            // 
            this.btnUploadPlant.Location = new System.Drawing.Point(602, 14);
            this.btnUploadPlant.Name = "btnUploadPlant";
            this.btnUploadPlant.Size = new System.Drawing.Size(122, 27);
            this.btnUploadPlant.TabIndex = 7;
            this.btnUploadPlant.Text = "上传心跳";
            this.btnUploadPlant.UseVisualStyleBackColor = true;
            this.btnUploadPlant.Click += new System.EventHandler(this.btnUploadPlant_Click);
            // 
            // txtPlantTopic
            // 
            this.txtPlantTopic.BackColor = System.Drawing.Color.Khaki;
            this.txtPlantTopic.Enabled = false;
            this.txtPlantTopic.Location = new System.Drawing.Point(141, 15);
            this.txtPlantTopic.Margin = new System.Windows.Forms.Padding(2);
            this.txtPlantTopic.Name = "txtPlantTopic";
            this.txtPlantTopic.Size = new System.Drawing.Size(439, 21);
            this.txtPlantTopic.TabIndex = 11;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(0, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(60, 20);
            this.label26.TabIndex = 3;
            this.label26.Text = "label26";
            // 
            // lbGetServerTime
            // 
            this.lbGetServerTime.AutoSize = true;
            this.lbGetServerTime.Location = new System.Drawing.Point(146, 580);
            this.lbGetServerTime.Name = "lbGetServerTime";
            this.lbGetServerTime.Size = new System.Drawing.Size(36, 20);
            this.lbGetServerTime.TabIndex = 14;
            this.lbGetServerTime.Text = "123";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(13, 580);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(120, 20);
            this.label27.TabIndex = 15;
            this.label27.Text = "GetServerTime:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 642);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.lbGetServerTime);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMain";
            this.Text = "Assy with MQTT Demo v2.0";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMQTTHostName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMQTTPassword;
        private System.Windows.Forms.TextBox txtMQTTUserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEMT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtMAC;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtOeeSerialNumber;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtOEECavity;
        private System.Windows.Forms.TextBox txtOEEErrorCode;
        private System.Windows.Forms.TextBox txtOEEScanCount;
        private System.Windows.Forms.TextBox txtOEESwVersion;
        private System.Windows.Forms.TextBox txtOEEActualCT;
        private System.Windows.Forms.TextBox txtOEEStatus;
        private System.Windows.Forms.TextBox txtOEEEndTime;
        private System.Windows.Forms.TextBox txtOEEStartTime;
        private System.Windows.Forms.TextBox txtOEEFixture;
        private System.Windows.Forms.TextBox txtOEEBGBarcode;
        private System.Windows.Forms.Button btnUploadOEE;
        private System.Windows.Forms.Button btnUploadDowntime;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtDTModuleCode;
        private System.Windows.Forms.TextBox txtDTErrorCode;
        private System.Windows.Forms.TextBox txtDTStatus;
        private System.Windows.Forms.TextBox txtDTTotalNum;
        private System.Windows.Forms.TextBox txtDTPoorNum;
        private System.Windows.Forms.Button btnUploadPlant;
        private System.Windows.Forms.Button btnMQTTDisconnect;
        private System.Windows.Forms.Button btnMQTTConnect;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtOEETopic;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtDTTopic;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtPlantTopic;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lbGetServerTime;
        private System.Windows.Forms.Label label27;
    }
}

