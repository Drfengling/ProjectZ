using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Demo
{
    public partial class frmMain : Form
    {

        public class Respond
        {
            /// <summary>
            /// GUID
            /// </summary>
            public Guid GUID { get; set; }
            /// <summary>
            /// 狀態
            /// </summary>
            public string Result { get; set; }
            /// <summary>
            /// 異常說明
            /// </summary>
            public string ErrorCode { get; set; }
            //public DateTime EventTime { get; set; }
        }
        public class BaseUpload
        {
            /// <summary>
            /// GUID
            /// </summary>
            public Guid GUID { get; set; }
            /// <summary>
            /// EMT 設備編號
            /// </summary>
            public string EMT { get; set; }
            /// <summary>
            /// 上报设备的PC Name
            /// </summary>
            public string ClientPcName { get; set; }
            /// <summary>
            /// 上报设备的MAC地址
            /// </summary>
            public string MAC { get; set; }
            /// <summary>
            /// 上报设备的IP地址
            /// </summary>
            public string IP { get; set; }
            /// <summary>
            /// 机台上报时间
            /// </summary>
            public DateTime EventTime { get; set; }
        }

        public class OeeUpload : BaseUpload
        {
            /// <summary>
            /// 产品厂内码
            /// </summary>
            public string SerialNumber { get; set; }
            /// <summary>
            /// 产品玻璃码
            /// </summary>
            public string BGBarcode { get; set; }
            /// <summary>
            /// 产品所用夹具号
            /// </summary>
            public string Fixture { get; set; }
            /// <summary>
            /// 站点开始时间
            /// </summary>
            public DateTime StartTime { get; set; }
            /// <summary>
            /// 站点结束时间
            /// </summary>
            public DateTime EndTime { get; set; }
            /// <summary>
            /// 产品状态(OK/NG)
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// 单次产品实际CT(cycle time)
            /// </summary>
            public string ActualCT { get; set; }
            /// <summary>
            /// 版本/软件设置版本
            /// </summary>
            public string SwVersion { get; set; }
            /// <summary>
            /// 扫码次数
            /// </summary>
            public int ScanCount { get; set; }
            /// <summary>
            /// 错误代号
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// 产品所用夹具腔号
            /// </summary>
            public string Cavity { get; set; }
        }

        public class DowntimeUpload : BaseUpload
        {
            /// <summary>
            /// 不良品数
            /// </summary>
            public int PoorNum { get; set; }
            /// <summary>
            /// 机台总产出
            /// </summary>
            public int TotalNum { get; set; }
            /// <summary>
            /// 机台状态
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// 报警代码
            /// </summary>
            public string ErrorCode { get; set; }
            /// <summary>
            /// 模块代号
            /// </summary>
            public string ModuleCode { get; set; }
        }
        public class PantUpload : BaseUpload
        {

        }

        private MqttClient _mqttClient;
        private Respond _respond;
        bool _isResponse = false;

        public frmMain()
        {
            InitializeComponent();

            lbGetServerTime.Text = string.Empty;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void btnUploadOEE_Click(object sender, EventArgs e)
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                MessageBox.Show("MQTT 请联机");
                return;
            }
            (sender as Button).Enabled = false;
            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                OeeUpload oeeUpload = new OeeUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = txtEMT.Text,
                    SerialNumber = txtOeeSerialNumber.Text,
                    BGBarcode = txtOEEBGBarcode.Text,
                    Fixture = txtOEEFixture.Text,
                    StartTime = DateTime.Parse(txtOEEStartTime.Text),
                    EndTime = DateTime.Parse(txtOEEEndTime.Text),
                    Status = txtOEEStatus.Text,
                    ActualCT = txtOEEActualCT.Text,
                    SwVersion = txtOEESwVersion.Text,
                    ScanCount = int.Parse(txtOEEScanCount.Text),
                    ErrorCode = txtOEEErrorCode.Text,
                    Cavity = txtOEECavity.Text,
                    ClientPcName = System.Environment.MachineName,
                    MAC = txtMAC.Text,
                    IP = txtIP.Text,
                    EventTime = DateTime.Now
                };

                _isResponse = false;

                sendString = JsonConvert.SerializeObject(oeeUpload);
                sendTopic = $"{txtEMT.Text}/upload/oee";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }

                if (_isResponse)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond.GUID.Equals(oeeUpload.GUID))
                    {
                        if (_respond.Result.Equals("OK"))
                        {
                            MessageBox.Show($"Result : {_respond.Result}");
                        }
                        else
                        {
                            MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("TimeOut!!");
                }



            }
            finally
            {
                (sender as Button).Enabled = true;

            }
        }

        private void btnUploadDowntime_Click(object sender, EventArgs e)
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                MessageBox.Show("MQTT 请联机");
                return;
            }
            (sender as Button).Enabled = false;
            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                DowntimeUpload downtimeUpload = new DowntimeUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = txtEMT.Text,
                    PoorNum = int.Parse(txtDTPoorNum.Text),
                    TotalNum = int.Parse(txtDTTotalNum.Text),
                    Status = txtDTStatus.Text,
                    ErrorCode = txtDTErrorCode.Text,
                    ModuleCode = txtDTModuleCode.Text,
                    ClientPcName = System.Environment.MachineName,
                    MAC = txtMAC.Text,
                    IP = txtIP.Text,
                    EventTime = DateTime.Now
                };
                _isResponse = false;

                sendString = JsonConvert.SerializeObject(downtimeUpload);
                sendTopic = $"{txtEMT.Text}/upload/downtime";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }

                if (_isResponse)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond.GUID.Equals(downtimeUpload.GUID))
                    {
                        if (_respond.Result.Equals("OK"))
                        {
                            MessageBox.Show($"Result : {_respond.Result}");
                        }
                        else
                        {
                            MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("TimeOut!!");
                }



            }
            finally
            {
                (sender as Button).Enabled = true;

            }

        }

        private void btnUploadPlant_Click(object sender, EventArgs e)
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                MessageBox.Show("MQTT 请联机");
                return;
            }
            (sender as Button).Enabled = false;
            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                PantUpload pantUpload = new PantUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = txtEMT.Text,
                    ClientPcName = System.Environment.MachineName,
                    MAC = txtMAC.Text,
                    IP = txtIP.Text,
                    EventTime = DateTime.Now
                };

                _isResponse = false;

                sendString = JsonConvert.SerializeObject(pantUpload);
                sendTopic = $"{txtEMT.Text}/upload/pant";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }

                if (_isResponse)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond.GUID.Equals(pantUpload.GUID))
                    {
                        if (_respond.Result .Equals("OK"))
                        {
                            MessageBox.Show($"Result : {_respond.Result}");
                        }
                        else
                        {
                            MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                        }
                        
                    }
                }
                else
                {
                    MessageBox.Show("TimeOut!!");
                }



            }
            finally
            {
                (sender as Button).Enabled = true;

            }

        }

        private void btnMQTTConnect_Click(object sender, EventArgs e)
        {
            if (MQTT_Connect())
            {
                txtOEETopic.Text = $"{txtEMT.Text}/upload/oee";
                txtDTTopic.Text = $"{txtEMT.Text}/upload/downtime";
                txtPlantTopic.Text = $"{txtEMT.Text}/upload/pant";

                string[] responseTopics = new string[4]
                {
                 $"{txtEMT.Text}/respond/oee",
                 $"{txtEMT.Text}/respond/downtime",
                 $"{txtEMT.Text}/respond/pant",
                 $"getservertime"
                };
                byte[] qosLevels = new byte[4]
                {
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE
                };
                _mqttClient.Subscribe(responseTopics, qosLevels);

                btnMQTTConnect.Enabled = false;
                btnMQTTDisconnect.Enabled = true;
                groupBox2.Enabled = false;
                MessageBox.Show("MQTT Connect success !!");
            }
            else
            {
                MessageBox.Show("MQTT Connect Fail ");
            }
        }

        private void btnMQTTDisconnect_Click(object sender, EventArgs e)
        {
            MQTT_Disconnect();
            btnMQTTConnect.Enabled = true;
            btnMQTTDisconnect.Enabled = false;
            groupBox2.Enabled = true;
        }

        #region " Method - Private "
        /// <summary>
        /// MQTT connect
        /// </summary>
        /// <returns></returns>
        private bool MQTT_Connect()
        {
            bool result = false;

            try
            {
                _mqttClient = new MqttClient(txtMQTTHostName.Text);  //Host Name  
                _mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                _mqttClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                _mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
                _mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;

                _mqttClient.Connect(Guid.NewGuid().ToString(),
                                    txtMQTTUserName.Text,
                                    txtMQTTPassword.Text,
                                    false,
                                    30);  //username, password

                result = _mqttClient.IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{txtMQTTHostName.Text} Connect Fail, {ex.Message}!");
            }
            return result;
        }


        private void MQTT_Disconnect()
        {
            _mqttClient.Disconnect();
            _mqttClient = null;
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine($"MQTT Disconnect!");
        }

        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine($"IsPublished:{e.IsPublished}, MessageID:{e.MessageId}");
        }

        private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            //
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //需加入断线重连机制,防止断线订阅不到信息
            if(!_mqttClient.IsConnected)
            {
                MQTT_Connect();
            }

            string topic = e.Topic; //訂閱接收到訊息的 Topic
            Console.WriteLine($"Received Topic : {topic}");
            string receivedString = Encoding.UTF8.GetString(e.Message);

            if (topic.Equals("getservertime"))
            {
                LabelSetText(lbGetServerTime, receivedString);

            }
            else
            {
                _respond = JsonConvert.DeserializeObject<Respond>(receivedString);
                _isResponse = true;
            }
        }

        private delegate void LabelSetTextCallBack(Label labObject, string text);
        public static void LabelSetText(Label labObject, string text)
        {
            try
            {
                if (labObject.InvokeRequired)
                {
                    LabelSetTextCallBack update = new LabelSetTextCallBack(LabelSetText);	// 這一行記得更新==> 委派物件(本方法名稱)
                    labObject.BeginInvoke(update, labObject, text);
                }
                else
                {
                    labObject.Text = text;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
