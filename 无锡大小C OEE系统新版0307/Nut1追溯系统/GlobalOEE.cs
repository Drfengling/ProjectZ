using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace 卓汇数据追溯系统
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
        /// 错误代号
        /// </summary>
        public string PFErrorCode { get; set; }
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

   
    public class GlobalOEE
    {
        private MqttClient _mqttClient;
        private Respond _respond_pant;
        private Respond _respond_downtime;
        private Respond _respond_oee;

        bool _isResponse_pant = false;
        bool _isResponse_downtime = false;
        bool _isResponse_oee = false;


        public string EMT { set; get; }
        public string MAC { set; get; }
        public string IP { set; get; }

        public string HostName { set; get; }
        public string txtMQTTUserName { set; get; }
        public string txtMQTTPassword { set; get; }
        /// <summary>
        /// 20211118
        /// </summary>
        public Action<string, string> AddTxt { set; get; }
        /// <summary>
        /// 20211118
        /// </summary>
        public Action<string, string> UiText { set; get; }
        /// <summary>
        /// 20211118
        /// </summary>
        public Action<Color,string, string> UpDatalabelcolor { set; get; }
        /// <summary>
        /// 20211118
        /// </summary>
        public Action<string, string> UpDatalabel { set; get; }
        SQLServer SQL = new SQLServer();
        public GlobalOEE(string EMT, string MAC, string IP,string HostName, string txtMQTTUserName, string txtMQTTPassword) {
           this.EMT = EMT;
            this.MAC = MAC;
            this.IP = IP;
            this.HostName = HostName;
            this.txtMQTTUserName = txtMQTTUserName;
            this.txtMQTTPassword = txtMQTTPassword;
        }
        /// <summary>
        /// OEE 心跳
        /// </summary>
        /// <param name="EMT"></param>
        /// <param name="MAC"></param>
        /// <param name="IP"></param>
        public void UploadPlant()
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                //MQTTConnect();
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT 请联机", @"D:\ZHH\OEE心跳记录\");
                MQTTConnect();
            }

            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                PantUpload pantUpload = new PantUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = EMT,
                    ClientPcName = System.Environment.MachineName,
                    MAC = MAC,
                    IP = IP,
                    EventTime = DateTime.Now
                };

                _isResponse_pant = false;

                sendString = JsonConvert.SerializeObject(pantUpload);
                sendTopic = $"{EMT}/upload/pant";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse_pant.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }

                if (_isResponse_pant)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond_pant.GUID.Equals(pantUpload.GUID))
                    {
                        if (_respond_pant.Result.Equals("OK"))
                        {
                            //MessageBox.Show($"Result : {_respond.Result}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ","+"OEE_HeartBeat上传OK:"+ $"Data: {sendString } " + "," + $"Result : {_respond_pant.Result}", @"D:\ZHH\OEE心跳记录\");
                            
                            Global.ConnectOEEFlag = true;
                            AddTxt("OEE_HeartBeat上传OK:" + sendString + ",OEELog","rtx_HeartBeatMsg");
                        }
                        else
                        {
                            //MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_HeartBeat上传NG:" + $"Data: {sendString } " + "," + $"Result : {_respond_pant.Result} / Error : {_respond_pant.ErrorCode}", @"D:\ZHH\OEE心跳记录\");
                            Global.ConnectOEEFlag = false;
                            AddTxt("OEE_HeartBeat上传NG:" + sendString + ",OEELog", "rtx_HeartBeatMsg");
                            //MessageBox.Show("OEE_HeartBeat上传NG:" + $"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                        }
                    }
                    else
                    {
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_HeartBeat上传错误:" + $"Data: {sendString } " + "," + "TimeOut!!", @"D:\ZHH\OEE心跳记录\");
                        Global.ConnectOEEFlag = false;
                        AddTxt("OEE_HeartBeat上传错误:GUID不匹配", "rtx_HeartBeatMsg");
                        MessageBox.Show("OEE_HeartBeat上传错误:GUID不匹配");
                    }
                }
                else
                {
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_HeartBeat上传超时:" + $"Data: {sendString } " + "," + "TimeOut!!", @"D:\ZHH\OEE心跳记录\");
                    Global.ConnectOEEFlag = false;
                    AddTxt("OEE_HeartBeat上传超时:请联系MES排查网络", "rtx_HeartBeatMsg");
                    MessageBox.Show("OEE_HeartBeat上传超时:请联系MES排查网络");
                }
            }
            catch (Exception ex)
            {
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_HeartBeat上传异常:" + $"Data: {sendString } " + "," + ex.Message, @"D:\ZHH\OEE心跳记录\");
                Global.ConnectOEEFlag = false;
                AddTxt("OEE_HeartBeat上传异常:" + ex.Message,"rtx_HeartBeatMsg");
                MessageBox.Show("OEE_HeartBeat上传异常:" + ex.Message);
            }

        }
        /// <summary>
        /// MQTTConnect
        /// </summary>
        public void MQTTConnect()
        {
           
            string txtOEETopic = string.Empty;
            string txtDTTopic = string.Empty;
            string txtPlantTopic = string.Empty;

            if (MQTT_Connect())
            {
                txtOEETopic = $"{EMT}/upload/oee";
                txtDTTopic = $"{EMT}/upload/downtime";
                txtPlantTopic = $"{EMT}/upload/pant";

                string[] responseTopics = new string[5]
                {
                 $"{EMT}/respond/oee",
                 $"{EMT}/respond/downtime",
                 $"{EMT}/respond/pant",
                 $"getservertime",
                 $"{EMT}/respond/connectivity"
                };
                byte[] qosLevels = new byte[5]
                {
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                };
                _mqttClient.Subscribe(responseTopics, qosLevels);


                //MessageBox.Show("MQTT Connect success !!");
                Log.WriteLog("MQTT Connect success !!");
            }
            else
            {
                //MessageBox.Show("MQTT Connect Fail ");
                Log.WriteLog("MQTT Connect Fail ");
            }
        }
        /// <summary>
        /// OEEDownTIme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool UploadDowntime(string PoorNum,string TotalNum,string Status,string ErrorCode,string ModuleCode,bool IsUploaded=true,string date=null,string ShowTxt=null)
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT 请联机", @"D:\ZHH\OEE心跳记录\");
                MQTTConnect();
                //return;
            }
           
            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                DowntimeUpload downtimeUpload = new DowntimeUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = EMT,
                    PoorNum = int.Parse(PoorNum),
                    TotalNum = int.Parse(TotalNum),
                    Status = Status,
                    ErrorCode = ErrorCode,
                    ModuleCode = ModuleCode,
                    ClientPcName = System.Environment.MachineName,
                    MAC = MAC,
                    IP = IP,
                    EventTime = IsUploaded == true? DateTime.Now:DateTime.Parse(date)
                };
                _isResponse_downtime = false;

                sendString = JsonConvert.SerializeObject(downtimeUpload);
                sendTopic = $"{EMT}/upload/downtime";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse_downtime.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }
                ////2021118
                AddTxt($"{downtimeUpload.ErrorCode}" + ", 触发时间 = " + date + ", 运行状态: " + Status + ", 故障描述: " + ShowTxt + $", 自动发送{_respond_downtime.Result}", "rtx_DownTimeMsg");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_downtime.Result}", @"E:\ZHH\OEEDowntime界面记录\");
                if (_isResponse_downtime)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond_downtime.GUID.Equals(downtimeUpload.GUID))
                    {
                        if (_respond_downtime.Result.Equals("OK"))
                        {
                            //MessageBox.Show($"Result : {_respond.Result}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_downtime.Result}", @"D:\ZHH\OEEDowntime记录\");
                        }
                        else
                        {
                            //MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_downtime.Result} / Error : {_respond_downtime.ErrorCode}", @"D:\ZHH\OEEDowntime记录\");
                            try
                            {
                                string s = "insert into MQTT_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[PoorNum],[ModuleCode],[TotalNum])" + " " + "values(" + "'" + downtimeUpload.EventTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "MQTT_DT" + "'" + "," + "'" + downtimeUpload.Status + "'" + "," + "'" + downtimeUpload.ErrorCode + "'" + "," + "'" + downtimeUpload.PoorNum + "'" + ","
                                                    + "'" + downtimeUpload.ModuleCode + "'" + "," + "'" + downtimeUpload.TotalNum + "'" + ")";
                                int r = SQL.ExecuteUpdate(s);

                                Log.WriteCSV(string.Format("插入了{0}行MQTT_DownTime缓存数据{1}", r, s), System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime数据\\");
                            }
                            catch (Exception EX)
                            {

                                Log.WriteCSV(EX.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime缓存数据\\");
                            }


                        }
                        return true;
                    }
                    else
                    {
                        AddTxt("OEE_DownTime上传错误:GUID不匹配", "rtx_DownTimeMsg");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + "TimeOut!!", @"D:\ZHH\OEEDowntime记录\");
                        try
                        {
                            string s = "insert into MQTT_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[PoorNum],[ModuleCode],[TotalNum])" + " " + "values(" + "'" + downtimeUpload.EventTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "MQTT_DT" + "'" + "," + "'" + downtimeUpload.Status + "'" + "," + "'" + downtimeUpload.ErrorCode + "'" + "," + "'" + downtimeUpload.PoorNum + "'" + ","
                                                + "'" + downtimeUpload.ModuleCode + "'" + "," + "'" + downtimeUpload.TotalNum + "'" + ")";
                            int r = SQL.ExecuteUpdate(s);
                            Log.WriteCSV(string.Format("插入了{0}行MQTT_DownTime缓存数据{1}", r, s), System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime数据\\");
                        }
                        catch (Exception EX)
                        {
                            Log.WriteCSV(EX.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime缓存数据\\");
                        }
                        return false;
                    }
                }
                else
                {
                    AddTxt("OEE_DownTime上传超时:请联系MES排查网络", "rtx_DownTimeMsg");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + "TimeOut!!", @"D:\ZHH\OEEDowntime记录\");
                    try
                    {
                        string s = "insert into MQTT_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[PoorNum],[ModuleCode],[TotalNum])" + " " + "values(" + "'" + downtimeUpload.EventTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "MQTT_DT" + "'" + "," + "'" + downtimeUpload.Status + "'" + "," + "'" + downtimeUpload.ErrorCode + "'" + "," + "'" + downtimeUpload.PoorNum + "'" + ","
                                            + "'" + downtimeUpload.ModuleCode + "'" + "," + "'" + downtimeUpload.TotalNum + "'" + ")";
                        int r = SQL.ExecuteUpdate(s);
                        Log.WriteCSV(string.Format("插入了{0}行MQTT_DownTime缓存数据{1}", r, s), System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime数据\\");
                    }
                    catch (Exception EX)
                    {

                        Log.WriteCSV(EX.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DownTime缓存数据\\");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("OEE_DownTime上传异常" + ex.ToString());
                return false;
            }
        }


        public bool UploadOEE(OeeUpload oeeUploadIn,bool IsUploaded=true)
        {
            if (_mqttClient == null || _mqttClient.IsConnected.Equals(false))
            {
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT 请联机", @"D:\ZHH\OEE心跳记录\");
                MQTTConnect();
                //return;
            }
           
            string sendTopic = string.Empty;
            string sendString = string.Empty;

            try
            {
                OeeUpload OEEData = new OeeUpload()
                {
                    GUID = Guid.NewGuid(),
                    EMT = EMT,
                    SerialNumber = String.IsNullOrEmpty(oeeUploadIn.SerialNumber) == true ? oeeUploadIn.StartTime.ToString("yyyy-MM-dd HH:mm:ss") : oeeUploadIn.SerialNumber,
                    BGBarcode = oeeUploadIn.BGBarcode,
                    Fixture = oeeUploadIn.Fixture,
                    StartTime = oeeUploadIn.StartTime,
                    EndTime = oeeUploadIn.EndTime,
                    Status = oeeUploadIn.Status,
                    ActualCT = oeeUploadIn.ActualCT,
                    SwVersion = oeeUploadIn.SwVersion,
                    ScanCount = oeeUploadIn.ScanCount,
                    ErrorCode = oeeUploadIn.ErrorCode,
                    PFErrorCode = oeeUploadIn.PFErrorCode,
                    Cavity = oeeUploadIn.Cavity,
                    ClientPcName = System.Environment.MachineName,
                    MAC = MAC,
                    IP = IP,
                    EventTime = IsUploaded == true? DateTime.Now: oeeUploadIn.StartTime
                };
                string mes = IsUploaded == true ? "" : "缓存";
                _isResponse_oee = false;

                sendString = JsonConvert.SerializeObject(OEEData);
                AddTxt(sendString, "rtx_OEEDefaultMsg");
                
                sendTopic = $"{EMT}/upload/oee";

                _mqttClient.Publish(sendTopic, Encoding.UTF8.GetBytes(sendString),
                          MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                int cnt = 0;
                while (_isResponse_oee.Equals(false))
                {
                    if (cnt > 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                    cnt++;
                }

                if (_isResponse_oee)
                {
                    //確認收到回覆的訊息是對的
                    if (_respond_oee.GUID.Equals(OEEData.GUID))
                    {
                        if (_respond_oee.Result.Equals("OK"))
                        {
                            Global.oee_ok++;
                            UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                            UiText(OEEData.SerialNumber, "txtOEE_SerialNumber");
                            UiText(OEEData.Fixture, "txtOEE_Fixture");
                            UiText(OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                            UiText(OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                            UiText(OEEData.Status.ToString(), "txtOEE_Status");
                            UiText(OEEData.ActualCT.ToString(), "txtOEE_ActualCT");
                            UiText(OEEData.SwVersion, "txtOEE_sw");
                            UiText(OEEData.ScanCount.ToString(), "txtOEE_ScanCount");
                            AddTxt(_respond_oee.Result, "rtx_OEEDefaultMsg");
                            
                            UpDatalabelcolor(Color.Green, $"OEE_Default{mes}发送成功", "lb_OEE_UA_SendStatus");
                            //MessageBox.Show($"Result : {_respond.Result}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_oee.Result}", @"D:\ZHH\OEE上传记录\");
                            try
                            {
                                string DeleteStr = $"delete from MQTT_DefaultSendNG where SerialNumber ='{OEEData.SerialNumber}'";
                                SQL.ExecuteUpdate(DeleteStr);                                
                            }
                            catch (Exception EX)
                            {
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {OEEData.SerialNumber}{EX.Message}" + "," + $"Result : {_respond_oee.Result} / Error : {_respond_oee.ErrorCode}", @"D:\ZHH\OEE上传数据库操作记录\");
                            }
                        }
                        else
                        {
                            Global.oee_ng++;
                            UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                            AddTxt(_respond_oee.Result, "rtx_OEEDefaultMsg");
                            UpDatalabelcolor(Color.Red, $"OEE_Default{mes}发送失败加入缓存", "lb_OEE_UA_SendStatus");
                            //MessageBox.Show($"Result : {_respond.Result} / Error : {_respond.ErrorCode}");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_oee.Result} / Error : {_respond_oee.ErrorCode}", @"D:\ZHH\OEE上传记录\");
                            try
                            {
                                ///MQTT缓存
                                string s = "insert into MQTT_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount],[ErrorCode],[PFErrorCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                             + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + "," + "'" + OEEData.ErrorCode + "'" + "," + "'" + OEEData.PFErrorCode + "'" + ")";
                                int r = SQL.ExecuteUpdate(s);
                                Log.WriteLog(string.Format("插入了{0}行MQTT_DefaultSendNG缓存数据", r) + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT_DefaultSendNG" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + OEEData.ErrorCode + "," + OEEData.PFErrorCode + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                            }
                            catch (Exception ex)
                            {
                                Log.WriteCSV(ex.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                            }
                        }
                        return true;
                    }
                    else
                    {
                        AddTxt("OEE_Default上传错误:GUID不匹配", "rtx_OEEDefaultMsg");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond_oee.Result}/ GUID : {OEEData.GUID}/ SerialNumber : {OEEData.SerialNumber}/ErrorCode :{OEEData.ErrorCode}", @"D:\ZHH\OEE上传记录\");
                        //Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + $"Result : {_respond.Result}/ GUID : {OEEData.GUID}/ SerialNumber : {OEEData.SerialNumber}/ErrorCode :{OEEData.ErrorCode}", @"D:\ZHH\MQTTGUID\");
                        Global.oee_ng++;
                        UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                        UpDatalabelcolor(Color.Red, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                        ///MQTT缓存
                        /// 
                        try
                        {
                            string s = "insert into MQTT_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount],[ErrorCode],[PFErrorCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                                    + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + "," + "'" + OEEData.ErrorCode + "'" + "," + "'" + OEEData.PFErrorCode + "'" + ")";
                            int r = SQL.ExecuteUpdate(s);
                            Log.WriteLog(string.Format("插入了{0}行MQTT_DefaultSendNG超时缓存数据", r) + ",OEELog");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT_DefaultSendNG超时" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + OEEData.ErrorCode + "," + OEEData.PFErrorCode + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                        }
                        catch (Exception ex)
                        {
                            Log.WriteCSV(ex.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                        }
                        return false;
                    }
                }
                else
                {
                    Global.oee_ng++;
                    UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                    AddTxt("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                    UpDatalabelcolor(Color.Red, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                    
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + $"Data : {sendString}" + "," + "TimeOut!!", @"D:\ZHH\OEE上传记录\");
                    ///MQTT缓存
                    /// 
                    try
                    {
                        string s = "insert into MQTT_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount],[ErrorCode],[PFErrorCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                                + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + "," + "'" + OEEData.ErrorCode + "'" + "," + "'" + OEEData.PFErrorCode + "'" + ")";
                        int r = SQL.ExecuteUpdate(s);
                        Log.WriteLog(string.Format("插入了{0}行MQTT_DefaultSendNG超时缓存数据", r) + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "MQTT_DefaultSendNG超时" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + OEEData.ErrorCode + "," + OEEData.PFErrorCode + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                    }
                    catch (Exception ex)
                    {
                        Log.WriteCSV(ex.Message, System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG数据\\");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("OEE_Default上传异常" + ex.ToString());
                return false;
            }
        }



        private bool MQTT_Connect()
        {
            bool result = false;

            try
            {
                _mqttClient = new MqttClient(HostName);  //Host Name  
                _mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                _mqttClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
                _mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
                _mqttClient.ConnectionClosed += MqttClient_ConnectionClosed;


                //// 20211118  _mqttClient.Connect(EMT,txtMQTTUserName,txtMQTTPassword,false,30);  //username, password
                _mqttClient.Connect(Guid.NewGuid().ToString(),
                                    txtMQTTUserName,
                                    txtMQTTPassword,
                                    false,
                                    30);  //username, password

                result = _mqttClient.IsConnected;

            }
            catch (Exception ex)
            {
                Log.WriteLog($"{HostName} Connect Fail, {ex.Message}!");
            }
            return result;
        }
        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Log.WriteLog($"MQTT Disconnect!");
        }

        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Log.WriteLog($"IsPublished:{e.IsPublished}, MessageID:{e.MessageId}");
        }

        private void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            //
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //需加入断线重连机制,防止断线订阅不到信息
            if (!_mqttClient.IsConnected)
            {
                MQTT_Connect();
            }

            string topic = e.Topic; //訂閱接收到訊息的 Topic
            Log.WriteLog($"Received Topic : {topic}");
            string receivedString = Encoding.UTF8.GetString(e.Message);

            if (topic.Equals("getservertime"))
            {
                //LabelSetText(lbGetServerTime, receivedString);

            }
            else if(topic.Equals(Global.inidata.productconfig.EMT + "/respond/pant"))//心跳回传
            {
                _respond_pant = JsonConvert.DeserializeObject<Respond>(receivedString);
                _isResponse_pant = true;
            }
            else if (topic.Equals(Global.inidata.productconfig.EMT + "/respond/downtime"))//DT回传
            {
                _respond_downtime = JsonConvert.DeserializeObject<Respond>(receivedString);
                _isResponse_downtime = true;
            }
            else if (topic.Equals(Global.inidata.productconfig.EMT + "/respond/oee"))//oee回传
            {
                _respond_oee = JsonConvert.DeserializeObject<Respond>(receivedString);
                _isResponse_oee = true;
            }

        }

        public void MQTT_Disconnect()
        {
            _mqttClient?.Disconnect();
            _mqttClient = null;
        }
    }
}
