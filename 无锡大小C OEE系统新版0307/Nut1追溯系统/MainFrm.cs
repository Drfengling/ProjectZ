using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace 卓汇数据追溯系统
{
    public partial class MainFrm : Form
    {
        #region 声明
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, byte[] mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        public class ThreadInfo
        {
            public string FileName { get; set; }
            public int SelectedIndex { get; set; }
            public string SN { get; set; }
            public string FixtureID { get; set; }
        }

        public class PrecitecData
        {
            public string precitec_grading = "0";
            public string precitec_rev = "0";
            public string precitec_value = "0";
        }

        //public class OEEDiscardLog
        //{
        //    public string SN;
        //    public string SystemType;
        //    public string JSONBody;
        //    public string Error;
        //    public string Frenquency;
        //}
        //public class DiscardData
        //{
        //    public string sn;
        //    public string SystemType;
        //    public string uptime;
        //    public string JSONBody;
        //    public string error;
        //    public string frenquency;
        //}
        public class CheckTraceMachine
        {
            public string mode { get; set; }
            public Data data { get; set; }
        }
        public class Data
        {
            public string plant { get; set; }
            public string dsn { get; set; }
            public string traceIP { get; set; }
            public string machine { get; set; }
        }
        public class Data2
        {
            public string status { get; set; }
            public string message { get; set; }
        }
        public class CheckTraceMachineRespond
        {
            public string success { get; set; }
            public Data2 data { get; set; }
        }
        public class MaterielData
        {
            public string date;
            public string count;
            public string totalcount;
            public string parttype;
        }
        public struct JgpData
        {
            public string SN;
            public string SN2;
            public string start_Time;
            public string end_Time;
            public string ct;
            public string result;
        }
        public HomeFrm _homefrm;
        public ManualFrm _manualfrm;
        public SettingFrm _sttingfrm;
        public AbnormalFrm _Abnormalfrm;
        public UserLoginFrm _userloginfrm;
        public HelpFrm _helpfrm;
        public MachineFrm _machinefrm;
        public DataStatisticsFrm _datastatisticsfrm;
        public IOMonitorFrm _iomonitorfrm;
        string LogPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\bmp\\";
        Image[] img = new Image[16];
        delegate void DGVAutoSize(DataGridView dgv);
        delegate void ShowDataTable(DataGridView dgv, DataTable dt, int index);
        delegate void ShowPlcStatue(string txt, Color color, int id);
        private delegate void tssLabelcolor(ToolStripStatusLabel tsslbl, Color color, string bl);
        private delegate void btnEnable(ToolStripButton btn, bool b);
        private delegate void AddItemToListBoxDelegate(ListBox listbox, string str, int index);
        private delegate void ShowTxt(string txt, TextBox tb);
        private delegate void Labelcolor(Label lb, Color color, string bl);
        private delegate void Labelvision(Label lb, string bl);
        private delegate void buttonflag(bool flag, Button bt);
        List<JgpData> jd1 = new List<JgpData>();
        public List<string> Out_ua = new List<string>();
        List<string> Out_ua_NG = new List<string>();
        List<string> Out_la = new List<string>();
        List<string> Out_oee = new List<string>();
        List<string> Out_oee_NG = new List<string>();
        List<string> Out_oee_discard = new List<string>();
        public List<string> Out_Trace_ua = new List<string>();
        List<string> Out_Trace_ua2 = new List<string>();
        List<string> Out_Trace_la = new List<string>();
        public Dictionary<string, BAilData> bail_ua = new Dictionary<string, BAilData>();
        Dictionary<string, BAilData> bail_ua_NG = new Dictionary<string, BAilData>();
        Dictionary<string, BAilData> bail_la = new Dictionary<string, BAilData>();
        Dictionary<string, OEEData> OEE = new Dictionary<string, OEEData>();
        Dictionary<string, OEEData> OEE_NG = new Dictionary<string, OEEData>();
        //Dictionary<string, OEEDiscardLog> OEE_discard = new Dictionary<string, OEEDiscardLog>();
        public Dictionary<string, TraceMesRequest_ua> Trace_ua = new Dictionary<string, TraceMesRequest_ua>();
        Dictionary<string, TraceMesRequest_ua> Trace_ua2 = new Dictionary<string, TraceMesRequest_ua>();
        Dictionary<string, TraceMesRequest_la> Trace_la = new Dictionary<string, TraceMesRequest_la>();
        //List<string> Out_oee_discard_pdca = new List<string>();
        List<string> Out_oee_discard_trace = new List<string>();
        //Dictionary<string, OEEDiscardLog> OEE_discard_PDCA = new Dictionary<string, OEEDiscardLog>();
        //Dictionary<string, OEEDiscardLog> OEE_discard_Trace = new Dictionary<string, OEEDiscardLog>();
        Dictionary<string, HansData_U_Bracket> HansDatas_ua = new Dictionary<string, HansData_U_Bracket>();
        Dictionary<string, HansData_U_Bracket> HansDatas_PDCA_ua = new Dictionary<string, HansData_U_Bracket>();
        Dictionary<string, PrecitecData> PrecitecData_ua = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_la = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_PDCA_ua = new Dictionary<string, PrecitecData>();
        Dictionary<string, PrecitecData> PrecitecData_PDCA_la = new Dictionary<string, PrecitecData>();
        BailCilent bc = new BailCilent();
        BailCilent bc2 = new BailCilent();
        private static object Lock1 = new object();
        private static object Lock2 = new object();
        private static object LockHans = new object();
        private static object Lock = new object();
        private static object LockUA = new object();
        private static object LockLA = new object();
        short[] ReadStatus = new short[20];
        short[] ReadTestRunStatus = new short[20];
        short[] ReadOpenDoorStatus = new short[20];
        //public MainDisplay maindis;
        bool bclose = true;
        bool ison = false;//PLC联机信号取反     
        bool ConnectPLC = false;
        string IP = string.Empty;
        string Mac = string.Empty;
        double timenum = 5;
        string getMd5 = string.Empty;//获取软件版本md5的字符串

        short[] mqttNgList = new short[10];
        short[] process = new short[1];//前站校验触发标志位
        short[] t1 = new short[1];//1#焊接机触发标志位
        short[] t2 = new short[1];//2#焊接机触发标志位
        short[] t3 = new short[1];//3#焊接机触发标志位
        short[] t4 = new short[1];//4#焊接机触发标志位
        short[] t5 = new short[1];//4#焊接机触发标志位
        short[] t_Code = new short[1];//读码器触发标志位
        short[] la_Trg = new short[1];//1#焊接机la工站触发标志位
        short[] ua_Trg1 = new short[1];//1#焊接机ua工站触发标志位
        short[] ua_Trg2 = new short[1];//1#焊接机ua工站NG触发标志位
        short[] ProductWayOut1 = new short[1];//产品出流道1触发标志位
        short[] ProductWayOut2 = new short[1];//产品出流道2触发标志位
        short[] OK_Trg = new short[1];//产品OK触发
        short[] NG_Trg1 = new short[1];//产品NG触发
        short[] NG_Trg2 = new short[1];//产品NG触发
        short[] NG_Trg3 = new short[1];//产品NG触发
        short[] P = new short[1];//PIS治具保养触发标志位
        short[] Nut1_trg = new short[1];//抛小料触发标志位
        short[] Nut2_trg = new short[1];//小料放置成功触发标志位
        short[] CT_LOG = new short[35];//CT LOG触发标志位
        short[] oldTrg = new short[1];//PLC参数修改前触发标志位
        short[] newTrg = new short[1];//PLC参数修改后触发标志位
        short[] NGTossing = new short[20];//NG明细统计
        short[] SmallmaterialTrg = new short[1];//小料抛料触发标志位
        short[] CCDCheckNGTrg = new short[1];//CCD检测NG触发标志位
        short[] ReadBarcodeNGTrg = new short[1];//CCD读码NG触发标志位
        string[] hansdatas = new string[15];//大族焊接参数
        string Trace_str_ua = "";
        string Trace_str_la = "";
        string Fullsn1 = "";
        string FixtureCode = "";
        string Fullsn2 = "";
        string Fullsn3 = "";
        string Fullsn4 = "";
        string Fullsn5 = "";

        bool Mac_mini_server_ua = true;
        bool Mac_mini_server_la = true;
        bool OEE_Default_flag = true;
        bool Trace_Logs_flag = true;
        bool Trace_check_flag = true;
        bool production_num_falg = true;
        bool listBox_flag = true;
        bool DeleteFile_flag = true;
        bool Link_PLC = true;
        bool Link_Mac_Mini_Server = true; //Ping Mac mini的服务器的返回值
        bool TCPconnected = true;
        bool PrecitecTCPconnected = true;
        bool isopen = false;//连接状态判读开启
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = true;
        bool flag4 = true;
        bool flag5 = false;
        bool Call_PIS_API_flag = true;
        bool InsertSQLFlag = true;
        bool PLCHeart = true;
        int number = 0;
        int i = 0, l = 0;   //i,l为UA、LA发送失败次数
        int Product_num_Mes_NG;
        //int time = 0;
        int time1 = 0;
        //PLC读写地址
        public const int Address_errorCode = 23000;//OEE-机台状态(1待料2运行3宕机)
        public const int Address_OEE_errorCode = 23010;//OEE-机台状态(1待料2运行3宕机)
        public const int Address_O_WatchDog = 24000;
        public const int Address_data = 19000;
        public const int Address_sndata = 5850;
        SQLServer SQL = new SQLServer();

        double Product_Lianglv_08_09 = 0;
        double Product_Lianglv_09_10 = 0;
        double Product_Lianglv_10_11 = 0;
        double Product_Lianglv_11_12 = 0;
        double Product_Lianglv_12_13 = 0;
        double Product_Lianglv_13_14 = 0;
        double Product_Lianglv_14_15 = 0;
        double Product_Lianglv_15_16 = 0;
        double Product_Lianglv_16_17 = 0;
        double Product_Lianglv_17_18 = 0;
        double Product_Lianglv_18_19 = 0;
        double Product_Lianglv_19_20 = 0;
        double Product_Lianglv_20_21 = 0;
        double Product_Lianglv_21_22 = 0;
        double Product_Lianglv_22_23 = 0;
        double Product_Lianglv_23_00 = 0;
        double Product_Lianglv_00_01 = 0;
        double Product_Lianglv_01_02 = 0;
        double Product_Lianglv_02_03 = 0;
        double Product_Lianglv_03_04 = 0;
        double Product_Lianglv_04_05 = 0;
        double Product_Lianglv_05_06 = 0;
        double Product_Lianglv_06_07 = 0;
        double Product_Lianglv_07_08 = 0;
        double Product_Lianglv_08_20 = 0;
        double Product_Lianglv_20_08 = 0;
        #endregion

        #region 初始化
        public MainFrm()
        {
            InitializeComponent();
            //MessageManager.gInit();
            //MessageManagerLogger.gInit("Log\\", "DcckVision", 30);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //maindis = new MainDisplay();//CCD程序实例化
            //读取开机配置文件
            Ctnum = 0;
            try
            {
                string dataPath = System.AppDomain.CurrentDomain.BaseDirectory + "setting.ini";
                if (File.Exists(dataPath))
                {
                    Global.inidata = new IniProductFile(dataPath);
                    //foreach (System.Reflection.PropertyInfo p in Global.inidata.productconfig.GetType().GetProperties())//获取所有配置文件名称和参数
                    //{
                    //    Global._listName.Add(p.Name);
                    //    Global._listValue.Add(p.GetValue(Global.inidata.productconfig).ToString());
                    //}
                    //Log.WriteLog("读取参数成功");
                }
                else
                {
                    MessageBox.Show("配置文件不存在");
                    Log.WriteLog("配置文件不存在");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("读取配置文件异常" + ex.ToString());
            }
        
            //连接PLC
            try
            {
                Global.PLC_Client.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port);
                //Global.PLC_Client2.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port2);
                Global.PLC_Client.Connect();
                //Global.PLC_Client2.Connect();
                if (Global.PLC_Client.IsConnected)
                {
                    //timer1.Enabled = true;
                    Log.WriteLog("已连接PLC");
                    isopen = true;
                    ConnectPLC = true;
                }
                else
                {
                    MessageBox.Show("PLC通信无法连接");
                    Log.WriteLog("PLC通信无法连接");
                    ShowStatus("与PLC断开连接", Color.Red, 0);
                    ConnectPLC = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC通信无法连接");
                Log.WriteLog("PLC通信无法连接");
                Environment.Exit(1);
            }

            //读取已配置数据并显示
            Global.Operator_pwd = Global.inidata.productconfig.Operator_pwd;
            Global.Technician_pwd = Global.inidata.productconfig.Technician_pwd;
            Global.Administrator_pwd = Global.inidata.productconfig.Administrator_pwd;
            Global.Threshold = Global.inidata.productconfig.Threshold;
            //MDI父窗体
            _homefrm = new HomeFrm(this);
            _manualfrm = new ManualFrm(this);
            _sttingfrm = new SettingFrm(this);
            _Abnormalfrm = new AbnormalFrm(this);
            _userloginfrm = new UserLoginFrm(this);
            _helpfrm = new HelpFrm(this);
            _machinefrm = new MachineFrm(this);
            _datastatisticsfrm = new DataStatisticsFrm(this);
            _iomonitorfrm = new IOMonitorFrm(this);
            ShowView();
            //导入报警信息表&PLC参数信息
            try
            {
                FileStream fs = new FileStream("报警目录.csv", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string lineData;
                while ((lineData = sr.ReadLine()) != null)
                {
                    ErrorData er = new ErrorData();
                    int ed_key = Convert.ToInt32(lineData.Split(',')[0]);
                    er.errorCode = lineData.Split(',')[1];//载入OEE异常代码
                    er.errorinfo = lineData.Split(',')[2];//载入OEE异常信息
                    er.errorStatus = lineData.Split(',')[3];//载入OEE异常状态
                    er.ModuleCode = lineData.Split(',')[4].Replace("|", "");//载入OEE异常模组代码
                    er.Moduleinfo = lineData.Split(',')[5];//载入OEE异常模组状态
                    Global.ed.Add(ed_key, er);
                    Global.ED.Add(ed_key, er);
                }
                sr.Close();
                fs.Close();
                Log.WriteLog("导入报警信息表成功");          
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入报警信息表/PLC参数信息表失败！" + ex.ToString().Replace("\n", ""));
                Log.WriteLog("导入报警信息表/PLC参数信息表失败！" + ex.ToString().Replace("\n", ""));
                Environment.Exit(1);
            }
            //   Thread.Sleep(10000);
            InitTimer();//初始化定时器

            ///MQTT联机
            Goee = new GlobalOEE(Global.inidata.productconfig.EMT,
                                    Global.inidata.productconfig.MAC,
                                    Global.inidata.productconfig.IP,
                                    Global.inidata.productconfig.HostName,
                                    Global.inidata.productconfig.txtMQTTUserName, Global.inidata.productconfig.txtMQTTPassword);
            Goee.AddTxt += this._homefrm.AppendRichText;
            Goee.UiText += this._homefrm.UiText;
            Goee.UpDatalabelcolor += this._homefrm.UpDatalabelcolor;
            Goee.UpDatalabel += this._homefrm.UpDatalabel;
            Goee.MQTTConnect();

            Worker_thread();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;//初始化窗体最大化

        }
        #endregion
        public static GlobalOEE Goee { get; set; }

        public int Ctnum { set; get; }
        public double CtSum { get; private set; }

        #region 开启工作线程
        private void Worker_thread()
        {
            Thread plcDeal = new Thread(EthPolling);//PLC交互
            plcDeal.IsBackground = true;
            plcDeal.Start();
            //Thread plcData = new Thread(EthPolling_Data);//PLC 产能统计
            //plcData.IsBackground = true;
            //plcData.Start();

            ///2021118
            Thread HeartBeat = new Thread(EthOEEHeartBeat);//发送OEE 心跳信号
            HeartBeat.IsBackground = true;
            HeartBeat.Start();
            Thread DownTime = new Thread(EthDownTime);//发送OEE DOWN TIME
            DownTime.IsBackground = true;
            DownTime.Start();
            //Thread errortime = new Thread(Errortime);//待料时间
            //errortime.IsBackground = true;
            //errortime.Start();
            /////2021118
            //Thread DownTime_Retry = new Thread(OEE_DownTime_Retry);//缓存重传OEE DT
            //DownTime_Retry.IsBackground = true;
            //DownTime_Retry.Start();
            /////2021118
            //Thread Default_Retry = new Thread(OEE_Default_Retry);//缓存重传OEE 过站数据
            //Default_Retry.IsBackground = true;
            //Default_Retry.Start();

            Thread MQTTDefault_Retry = new Thread(MQTT_Default_Retry);//缓存重传MQTTTOEE 过站数据
            MQTTDefault_Retry.IsBackground = true;
            MQTTDefault_Retry.Start();

            Thread MQTTDownTime_Retry = new Thread(MQTT_DownTime_Retry);//缓存重传MQTT DT
            MQTTDownTime_Retry.IsBackground = true;
            MQTTDownTime_Retry.Start();
            //Thread UD_dt = new Thread(_datastatisticsfrm.UD_DataTable);
            //UD_dt.IsBackground = true;
            //UD_dt.Start();
            ThreadPool.QueueUserWorkItem(On_Time_doing);//按时做某事
            ThreadPool.QueueUserWorkItem(AutoStopBreak);//机台状态改变时自动结束吃饭休息        
            ThreadPool.QueueUserWorkItem(PLC_autolink);//PLC自动重连
            ThreadPool.QueueUserWorkItem(Ping_ip);//检测PLC与Macmini是否连接
            ThreadPool.QueueUserWorkItem(CheckConnected);//连接状态判断
            //ThreadPool.QueueUserWorkItem(ReadOperatorID);//固定式刷卡机读卡
            Thread.Sleep(10);
        }

        private void MQTT_DownTime_Retry()
        {
            while (true)
            {
                if (Global.ConnectOEEFlag == true)
                {
                    string msg = "";
                    try
                    {
                        string SelectStr = string.Format("select count(*) from MQTT_DTSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("MQTT_DTSendNG上传失败数据重新上传" + ",OEELog");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from MQTT_DTSendNG where ID =(SELECT MIN(ID) FROM MQTT_DTSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);

                                ////20210909
                                if (Goee.UploadDowntime(d2.Rows[0][5].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][6].ToString(), false, d2.Rows[0][1].ToString(), "缓存"))
                                {
                                    Log.WriteCSV("MQTT_Default_UI更新成功" + ",OEELog", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG重传\\");
                                    string DeleteStr = "delete from MQTT_DTSendNG where ID = (SELECT MIN(ID) FROM MQTT_DTSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                };


                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteCSV("MQTT_Default数据缓存重传异常" + ex.ToString().Replace("\n", "") + ",OEELog", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DT重传\\");
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void MQTT_Default_Retry()
        {
            while (true)
            {
                if (Global.ConnectOEEFlag == true)
                {
                    string msg = "";
                    try
                    {
                        string SelectStr = string.Format("select count(*) from MQTT_DefaultSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("MQTT_Default上传失败数据重新上传" + ",OEELog");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from MQTT_DefaultSendNG where ID =(SELECT MIN(ID) FROM MQTT_DefaultSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                                ////20210909
                                if (Goee.UploadOEE(new OeeUpload()
                                {
                                    SerialNumber = string.IsNullOrEmpty(d2.Rows[0][2].ToString()) ? DateTime.Parse(d2.Rows[0][4].ToString()).ToString("yyyyMMddHHmmss") : d2.Rows[0][2].ToString(),
                                    BGBarcode = "",
                                    Fixture = d2.Rows[0][3].ToString(),
                                    StartTime = DateTime.Parse(d2.Rows[0][4].ToString()),
                                    EndTime = DateTime.Parse(d2.Rows[0][5].ToString()),
                                    Status = d2.Rows[0][6].ToString(),
                                    ActualCT = d2.Rows[0][7].ToString(),
                                    SwVersion = d2.Rows[0][8].ToString(),
                                    ScanCount = 1,
                                    Cavity = "1",
                                    ErrorCode = d2.Rows[0][10].ToString(),
                                    PFErrorCode = d2.Rows[0][11].ToString()
                                }, false))
                                {

                                    Log.WriteCSV("MQTT_Default_UI更新成功" + ",OEELog", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG重传\\");
                                    string DeleteStr = "delete from MQTT_DefaultSendNG where ID = (SELECT MIN(ID) FROM MQTT_DefaultSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                };


                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteCSV("MQTT_Default数据缓存重传异常" + ex.ToString().Replace("\n", "") + ",OEELog", System.AppDomain.CurrentDomain.BaseDirectory + "\\MQTT_DefaultSendNG重传\\");
                    }
                }
                Thread.Sleep(100);
            }
        }



        #endregion
        #region 连接状态判断
        private void CheckConnected(object ob)
        {
            while (isopen)
            {
                try
                {
                    //------------------------PLC------------------------------
                    if (Link_PLC && ConnectPLC)
                    {
                        ShowStatus("已连接PLC", Color.DarkSeaGreen, 0);
                    }
                    else
                    {
                        ShowStatus("与PLC断开连接", Color.Red, 0);
                    }
                    //------------------------OEE-----------------------------             
                    if (Global.oeeSend_ng == 0 && OEE_Default_flag == true && Global.ConnectOEEFlag)
                    {
                        ShowStatus("已连接OEE", Color.DarkSeaGreen, 3);
                        Global.PLC_Client.WritePLC_D(427, new short[] { 0 });//正常连接OEE
                    }
                    else if (!Global.ConnectOEEFlag)
                    {
                        ShowStatus("与OEE断开连接", Color.Red, 3);
                        Global.PLC_Client.WritePLC_D(427, new short[] { 1 });//提示PLC OEE断线
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("显示连接状态异常失败！" + ex.ToString().Replace("\n", ""));
                }

                Thread.Sleep(500);
            }
        }


        #endregion
        #region PLC交互处理  MES-校验前站 Trace-校验前站


        private void EthPolling()
        {
            while (true && bclose)
            {
                try
                {
                    //与PLC心跳信号
                    if (PLCHeart)
                    {
                        if (ison)
                        {
                            try
                            {
                                Global.PLC_Client.WritePLC_D(8028, new short[] { 1 });
                            }
                            catch
                            {
                            }
                            ison = false;
                        }
                        else
                        {
                            try
                            {
                                Global.PLC_Client.WritePLC_D(8028, new short[] { 0 });
                            }
                            catch
                            {
                            }
                            ison = true;
                        }
                    }
                    OK_Trg = Global.PLC_Client.ReadPLC_D(428, 1);//产品OK触发上传
                    NG_Trg1 = Global.PLC_Client.ReadPLC_D(422, 1);//NUT吸真空抛料
                    NG_Trg2 = Global.PLC_Client.ReadPLC_D(423, 1);//二次定位台抛料
                    NG_Trg3 = Global.PLC_Client.ReadPLC_D(424, 1);//产品焊接NG抛料
                    if (OK_Trg[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(428, new short[] { 0 });
                        var OEEDATA = new OeeUpload()
                        {
                            SerialNumber = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            BGBarcode = "",
                            Fixture = "",
                            StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-9).ToString("yyyy-MM-dd HH:mm:ss")),
                            EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            Status = "OK",
                            ActualCT = Global.inidata.productconfig.CT,
                            SwVersion = Global.inidata.productconfig.Sw_version,
                            ScanCount = 1,
                            Cavity = "1",
                            ErrorCode = "",
                            PFErrorCode = "",
                        };
                        Task T1 = new Task(() =>
                        {
                            Goee.UploadOEE(OEEDATA);

                            ///2021118
                            //SendOEEDEFAULT(OEEDATA);
                        }
                        );
                        T1.Start();
                    }
                    if (NG_Trg1[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(422, new short[] { 0 });
                        var OEEDATA = new OeeUpload()
                        {
                            SerialNumber = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            BGBarcode = "",
                            Fixture = "",
                            StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-9).ToString("yyyy-MM-dd HH:mm:ss")),
                            EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            Status = "NG",
                            ActualCT = Global.inidata.productconfig.CT,
                            SwVersion = Global.inidata.productconfig.Sw_version,
                            ScanCount = 1,
                            Cavity = "1",
                            ErrorCode = "",
                            PFErrorCode = "001001",
                        };
                        Task T1 = new Task(() =>
                        {
                            Goee.UploadOEE(OEEDATA);

                            ///2021118
                            //SendOEEDEFAULT(OEEDATA);
                        }
                        );
                        T1.Start();
                    }
                    if (NG_Trg2[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(423, new short[] { 0 });
                        var OEEDATA = new OeeUpload()
                        {
                            SerialNumber = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            BGBarcode = "",
                            Fixture = "",
                            StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-9).ToString("yyyy-MM-dd HH:mm:ss")),
                            EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            Status = "NG",
                            ActualCT = Global.inidata.productconfig.CT,
                            SwVersion = Global.inidata.productconfig.Sw_version,
                            ScanCount = 1,
                            Cavity = "1",
                            ErrorCode = "",
                            PFErrorCode = "001002",
                        };
                        Task T1 = new Task(() =>
                        {
                            Goee.UploadOEE(OEEDATA);

                            ///2021118
                            //SendOEEDEFAULT(OEEDATA);
                        }
                        );
                        T1.Start();
                    }
                    if (NG_Trg3[0] == 1)
                    {
                        Global.PLC_Client.WritePLC_D(424, new short[] { 0 });
                        var OEEDATA = new OeeUpload()
                        {
                            SerialNumber = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            BGBarcode = "",
                            Fixture = "",
                            StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-9).ToString("yyyy-MM-dd HH:mm:ss")),
                            EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            Status = "NG",
                            ActualCT = Global.inidata.productconfig.CT,
                            SwVersion = Global.inidata.productconfig.Sw_version,
                            ScanCount = 1,
                            Cavity = "1",
                            ErrorCode = "",
                            PFErrorCode = "001003",
                        };
                        Task T1 = new Task(() =>
                        {
                            Goee.UploadOEE(OEEDATA);

                            ///2021118
                            //SendOEEDEFAULT(OEEDATA);
                        }
                        );
                        T1.Start();
                    }

                }

                catch (Exception ex)
                {
                    Log.WriteLog("PC与PLC通讯异常：" + ex.ToString().Replace("\n", ""));
                }
                Thread.Sleep(50);
            }
        }

        private void EthPolling_Data()//产能统计 DT统计
        {
            while (true && bclose)
            {
                try
                {
                    Product_DataStatistics();//产能统计
                    DT_DataStatistics();//运行状态统计
                }
                catch (Exception ex)
                {
                    Log.WriteLog("PC与PLC通讯异常：" + ex.ToString().Replace("\n", ""));
                }
                Thread.Sleep(500);
            }

        }
        #endregion


        #region OEE Down Time
        private void EthDownTime()
        {
            try
            {
                //-----记录OEE 关闭软件时长
                string SelectStr = "select * from OEE_MCOff where  1=1";
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                string StartTime = string.Empty;
                if (d1.Rows.Count > 0)//判断上一次是否正常关闭软件-有正常关机时间
                {
                    StartTime = d1.Rows[0][1].ToString();
                    DateTime T1 = Convert.ToDateTime(StartTime);
                    DateTime T2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    string TS = (T2 - T1).TotalMinutes.ToString("0.00");
                    string InsertOEEStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "10010001" + "'" + "," + "'" + (Convert.ToDateTime(StartTime)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ","
                                   + "'" + "" + "'" + "," + "'" + "6" + "'" + "," + "'" + "软件关闭" + "'" + "," + "'" + TS + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);

                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + "自动发送成功" + "," + "6" + "," + "软件关闭" + "," + TS, @"E:\装机软件\系统配置\System_ini\");
                }
                else//非正常关机
                {
                    string SelectStr2 = "select * from OEE_StartTime where ID =(SELECT MAX(ID) from OEE_StartTime)";
                    DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                    string StartTime2 = string.Empty;
                    if (d2.Rows.Count > 0)//搜索非正常关机前的最后一次状态开始时间,记录写入OEE数据库中
                    {
                        StartTime = d2.Rows[0][3].ToString();
                        DateTime T11 = Convert.ToDateTime(StartTime);
                        DateTime T12 = Convert.ToDateTime(DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                        string TS2 = (T12 - T11).TotalMinutes.ToString("0.00");
                        string InsertOEEStr4 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + d2.Rows[0][2].ToString() + "'" + "," + "'" + (Convert.ToDateTime(StartTime)).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ","
                                       + "'" + d2.Rows[0][4].ToString() + "'" + "," + "'" + d2.Rows[0][1].ToString() + "'" + "," + "'" + d2.Rows[0][5].ToString() + "'" + "," + "'" + TS2 + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr4);
                        Log.WriteCSV(DateTime.Now.AddMinutes(-1).ToString("HH:mm:ss") + "," + d2.Rows[0][2].ToString() + "," + DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + d2.Rows[0][4].ToString() + "," + "自动发送成功" + "," + d2.Rows[0][1].ToString() + "," + d2.Rows[0][5].ToString() + "," + TS2, @"E:\装机软件\系统配置\System_ini\");
                    }
                    //非正常关机后，默认下一次开机时间的之前一分钟为关机开始时间
                    string OEEDownTime = "";
                    string DownTimemsg = "";

                    ///1
                    /// 
                    string date = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    OEEDownTime = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", "6", "10010001", date, "");
                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                                          + "'" + date + "'" + "," + "'" + "" + "'" + ")";
                    SQL.ExecuteUpdate(InsertStr);
                    ///20211118
                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEEDownTime, out DownTimemsg);
                    /////0909
                    ///// 
                    /////Night
                    ///// 
                    string poorNum = string.Empty;
                    string TotalNum = string.Empty;
                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                    {
                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                        TotalNum = Global.Product_Total_N.ToString();
                    }
                    else
                    {
                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                        TotalNum = Global.Product_Total_D.ToString();
                    }
                    Goee.UploadDowntime(poorNum, TotalNum, "6", "10010001", "", false, date, "关机");
                    /////

                    //if (rst)
                    //{
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + date + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送成功", "rtx_DownTimeMsg");
                    //    Log.WriteLog("OEE_DT补传关机errorCode发送成功");
                    //}
                    //else
                    //{
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + date + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送失败", "rtx_DownTimeMsg");
                    //    Log.WriteLog("OEE_DT补传关机errorCode发送失败");
                    //    Global.ConnectOEEFlag = false;
                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                    // + "'" + date + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + "关机" + "'" + ")";
                    //    int r = SQL.ExecuteUpdate(s);
                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime补传关机缓存数据", r));
                    //}
                    Log.WriteLog("OEE_DT:" + OEEDownTime);

                    DateTime T1 = Convert.ToDateTime(DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                    DateTime T2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    string TS = (T2 - T1).TotalMinutes.ToString("0.00");
                    string InsertOEEStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "10010001" + "'" + "," + "'" + date + "'" + ","
                                   + "'" + "" + "'" + "," + "'" + "6" + "'" + "," + "'" + "软件关闭" + "'" + "," + "'" + TS + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);
                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "10010001" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + "" + "," + "自动发送成功" + "," + "6" + "," + "软件关闭" + "," + TS, @"E:\装机软件\系统配置\System_ini\");
                }
                string DeleteOEEStr = "delete OEE_MCOff";
                SQL.ExecuteUpdate(DeleteOEEStr);//清空关机时间
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString() + ",OEELog");
            }
            //-----------------------------------------------------------------------------------------------------

            while (true)
            {
                ReadStatus = Global.PLC_Client.ReadPLC_D(Address_errorCode, 4);
                ReadTestRunStatus = Global.PLC_Client.ReadPLC_D(23004, 1);
                ReadOpenDoorStatus = Global.PLC_Client.ReadPLC_D(23005, 1);//待料中开启安全门或者暂停标志位
                if (ReadStatus != null)
                {
                    try
                    {
                        if (!Global.SelectFirstModel)//当前是否属于手动(首件)状态
                        {
                            if (ReadTestRunStatus[0] != 1)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                            {
                                if (Global.SelectTestRunModel == true && Global.ed[211].start_time != null)//空运行结束写入OEE_DT数据表中
                                {
                                    Global.ed[211].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    DateTime t1 = Convert.ToDateTime(Global.ed[211].start_time);
                                    DateTime t2 = Convert.ToDateTime(Global.ed[211].stop_time);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[211].errorCode + "'" + "," + "'" + Global.ed[211].start_time + "'" + ","
                                       + "'" + Global.ed[211].ModuleCode + "'" + "," + "'" + Global.ed[211].errorStatus + "'" + "," + "'" + Global.ed[211].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[211].errorCode + "," + Global.ed[211].start_time + "," + Global.ed[211].ModuleCode + "," + "自动发送成功" + "," + Global.ed[211].errorStatus + "," + Global.ed[211].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                    Global.ed[211].start_time = null;
                                    Global.ed[211].stop_time = null;
                                }
                                Global.SelectTestRunModel = false;
                                if ((ReadStatus[0] == 1 || ReadStatus[0] == 2 || ReadStatus[0] == 3 || ReadStatus[0] == 4) && !Global.BreakStatus)//j为机台运行大状态（-1初始值、1待料、2运行、3宕机、4人工停止），判断是否是吃饭休息
                                {
                                    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    if (Global.j == -1)//判断是否初始化/结束首件状态
                                    {
                                        if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                        {
                                            StopStatus();
                                        }
                                        else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                        {
                                            ErrorStatus();
                                        }
                                        else if (ReadStatus[0] == 2)//机台处于运行2状态
                                        {
                                            RunStatus();
                                        }
                                        else if (ReadStatus[0] == 1)//处于待料1状态
                                        {
                                            PendingStatus();
                                        }
                                    }
                                    else//上一个状态与当前状态发生变动且上一个状态为非宕机状态时1、2
                                    {
                                        if (Global.STOP)//机台运行中人工停止
                                        {
                                            if (ReadStatus[2] == 7 || ReadStatus[2] == 10)//并且打开安全门
                                            {
                                                Global.STOP = false;
                                                Global.PLC_Client.WritePLC_D(23030, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
                                            }
                                        }
                                        ///20210814 && Global.j != ReadStatus[0]
                                        if (ReadStatus[0] == 1)//判断当前状态为待料状态时1
                                        {
                                            if (ReadOpenDoorStatus[0] == 1)//10040 判断是否在待料中开安全门或者按下暂停按钮
                                            {
                                                Global.PLC_Client.WritePLC_D(23005, new short[] { 0 });
                                                Global.PLC_Client.WritePLC_D(23030, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
                                                Global.Error_PendingStatus = true;
                                                Global.ed[Global.Error_PendingNum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum].errorCode;
                                                Log.WriteLog(c + ",OEELog");
                                                if (Global.ed[Global.Error_PendingNum].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].start_time + "'" + ","
                                                        + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum].errorCode + "," + Global.ed[Global.Error_PendingNum].start_time + "," + Global.ed[Global.Error_PendingNum].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum].errorStatus + "," + Global.ed[Global.Error_PendingNum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                }
                                                Log.WriteLog(Global.ed[Global.Error_PendingNum].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum].stop_time);
                                                Global.ed[Global.Error_PendingNum].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//待料中开启安全门或者按下暂停后,待料结束,进入手动选择异常原因状态,待料结束时间为手动选择状态的开始时间
                                            }
                                        }

                                        if (Global.j != ReadStatus[0] && Global.j == 1)//上一个状态与当前状态发生变动且上一个状态为待料状态时1
                                        {
                                            string date = Global.ed[Global.Error_PendingNum].start_time;
                                            if (Global.Error_PendingStatus)//判断待料时是否打开安全门/按下暂停键
                                            {
                                                Global.Error_PendingStatus = false;
                                                Global.ed[Global.Error_PendingNum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                if (Global.ed[Global.Error_PendingNum].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string OEE_DT = "";
                                                    string msg = "";

                                                    ///2
                                                    /// 

                                                    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.errorStatus, Global.errorcode, date, "");
                                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog("OEE_DT安全门打开:" + OEE_DT + ",OEELog");
                                                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

                                                    /////0909
                                                    ///// 
                                                    /////Night
                                                    ///// 
                                                    string poorNum = string.Empty;
                                                    string TotalNum = string.Empty;
                                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                                    {
                                                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                        TotalNum = Global.Product_Total_N.ToString();
                                                    }
                                                    else
                                                    {
                                                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                        TotalNum = Global.Product_Total_D.ToString();
                                                    }
                                                    Goee.UploadDowntime(poorNum, TotalNum, Global.errorStatus, Global.errorcode, Global.ed[Global.Error_PendingNum].ModuleCode, false, date, Global.errorinfo);
                                                    /////

                                                    //if (rst)
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + date + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + date + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //     + "'" + date + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + date + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + date + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                    Log.WriteLog("" + "_" + Global.errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum].stop_time);
                                                }
                                            }
                                            else
                                            {
                                                Global.ed[Global.Error_PendingNum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                //_manualfrm.ButtonFlag(false, "btnManualOEEStatus");
                                                string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_PendingNum].start_time + "&stop_time=" + Global.ed[Global.Error_PendingNum].stop_time + "&ec=" + Global.ed[Global.Error_PendingNum].errorCode;
                                                Log.WriteLog(c + ",OEELog");
                                                if (Global.ed[Global.Error_PendingNum].start_time != null)
                                                {
                                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].start_time);
                                                    DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].stop_time);
                                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                    string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + "," + "'" + date + "'" + ","
                                                       + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum].errorCode + "," + Global.ed[Global.Error_PendingNum].start_time + "," + Global.ed[Global.Error_PendingNum].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum].errorStatus + "," + Global.ed[Global.Error_PendingNum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                }
                                                Log.WriteLog(Global.ed[Global.Error_PendingNum].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum].errorinfo + "：结束计时 " + Global.ed[Global.Error_PendingNum].stop_time + ",OEELog");
                                            }
                                            Global.ed[Global.Error_PendingNum].start_time = null;
                                            Global.ed[Global.Error_PendingNum].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 2)//上一个状态与当前状态发生变动且上一个状态为运行状态时2
                                        {
                                            Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            if (Global.ed[Global.j].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                string InsertStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                                   + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertStr);
                                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + Global.ed[Global.j].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                            }
                                            Log.WriteLog(Global.ed[Global.j].Moduleinfo + "_" + Global.ed[Global.j].errorinfo + "：结束计时 " + Global.ed[Global.j].stop_time);
                                            Global.ed[Global.j].start_time = null;
                                            Global.ed[Global.j].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 3)//上一个状态与当前状态发生变动且上一个状态为宕机状态3时
                                        {
                                            Global.ed[Global.Error_num].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_num].start_time + "&stop_time=" + Global.ed[Global.Error_num].stop_time + "&ec=" + Global.ed[Global.Error_num].errorCode;
                                            Log.WriteLog(c + ",OEELog");
                                            if (Global.ed[Global.Error_num].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                string date = Global.ed[Global.Error_num].start_time;
                                                if (Global.Error_num == 7 || Global.Error_num == 10)//机台打开安全门
                                                {
                                                    string OEE_DT = "";
                                                    string msg = "";

                                                    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.errorStatus, Global.errorcode, date, "");
                                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog("OEE_DT安全门打开:" + OEE_DT + ",OEELog");

                                                    /////0909
                                                    ///// 
                                                    /////Night
                                                    ///// 
                                                    string poorNum = string.Empty;
                                                    string TotalNum = string.Empty;
                                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                                    {
                                                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                        TotalNum = Global.Product_Total_N.ToString();
                                                    }
                                                    else
                                                    {
                                                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                        TotalNum = Global.Product_Total_D.ToString();
                                                    }
                                                    Goee.UploadDowntime(poorNum, TotalNum, Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num].ModuleCode, false, date, Global.errorinfo + ",安全门打开");
                                                    /////


                                                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);
                                                    //if (rst)
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + date + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + date + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //     + "'" + date + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + date + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_num].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                }
                                                else//机台处于其它异常状态中
                                                {
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + "," + "'" + date + "'" + ","
                                                 + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num].errorCode + "," + date + "," + Global.ed[Global.Error_num].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_num].errorStatus + "," + Global.ed[Global.Error_num].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                }
                                            }
                                            Log.WriteLog(Global.ed[Global.Error_num].Moduleinfo + "_" + Global.ed[Global.Error_num].errorinfo + "：结束计时 " + Global.ed[Global.Error_num].stop_time + ",OEELog");
                                            Global.ed[Global.Error_num].start_time = null;
                                            Global.ed[Global.Error_num].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else if (Global.j != ReadStatus[0] && Global.j == 4)//上一个状态与当前状态发生变动且上一个状态为人工停止状态4时
                                        {
                                            Global.ed[Global.Error_Stopnum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            string c = "c=UPLOAD_DOWNTIME&tsn=Test_station&mn=Machine#1&start_time=" + Global.ed[Global.Error_Stopnum].start_time + "&stop_time=" + Global.ed[Global.Error_Stopnum].stop_time + "&ec=" + Global.ed[Global.Error_Stopnum].errorCode;
                                            Log.WriteLog(c + ",OEELog");
                                            if (Global.ed[Global.Error_Stopnum].start_time != null)
                                            {
                                                DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum].start_time);
                                                DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum].stop_time);
                                                string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                                if (ReadStatus[2] == 7 || ReadStatus[2] == 10 || Global.SelectManualErrorCode)//机台打开安全门或者手动选择ErrorCode状态开启
                                                {
                                                    string OEE_DT = "";
                                                    string msg = "";

                                                    string date = Global.ed[Global.Error_Stopnum].start_time;
                                                    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.errorStatus, Global.errorcode, date, "");
                                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].ModuleCode + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog("OEE_DT安全门打开:" + OEE_DT + ",OEELog");
                                                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

                                                    /////0909
                                                    ///// 
                                                    /////Night
                                                    ///// 
                                                    string poorNum = string.Empty;
                                                    string TotalNum = string.Empty;
                                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                                    {
                                                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                        TotalNum = Global.Product_Total_N.ToString();
                                                    }
                                                    else
                                                    {
                                                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                        TotalNum = Global.Product_Total_D.ToString();
                                                    }
                                                    Goee.UploadDowntime(poorNum, TotalNum, Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num].ModuleCode, false, date, Global.errorinfo + ",安全门打开");
                                                    /////

                                                    //if (rst)
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                    //     + "'" + date + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + date + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_Stopnum].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                }
                                                else if (Global.Error_Stopnum == 210)//机台人工停止
                                                {
                                                    string OEE_DT = "";
                                                    string msg = "";
                                                    string date = Global.ed[Global.Error_Stopnum].start_time;
                                                    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[Global.Error_Stopnum].errorStatus, Global.ed[Global.Error_Stopnum].errorCode, date, "");
                                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + ","
                                                    + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].ModuleCode + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertStr);
                                                    Log.WriteLog("OEE_DT安全门打开:" + OEE_DT + ",OEELog");
                                                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

                                                    ///0909
                                                    /// 
                                                    ///Night
                                                    /// 
                                                    string poorNum = string.Empty;
                                                    string TotalNum = string.Empty;
                                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                                    {
                                                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                        TotalNum = Global.Product_Total_N.ToString();
                                                    }
                                                    else
                                                    {
                                                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                        TotalNum = Global.Product_Total_D.ToString();
                                                    }
                                                    Goee.UploadDowntime(poorNum, TotalNum, Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num].ModuleCode, false, date, Global.ed[Global.Error_Stopnum].errorinfo + ",安全门打开");
                                                    ///

                                                    //if (rst)
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum].errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum].errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                    //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败" + ",OEELog");
                                                    //    Global.ConnectOEEFlag = false;
                                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + ","
                                                    //     + "'" + Global.ed[Global.Error_Stopnum].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + ")";
                                                    //    int r = SQL.ExecuteUpdate(s);
                                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                                                    //}
                                                    _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                                                    string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + "," + "'" + date + "'" + ","
                                                   + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                    SQL.ExecuteUpdate(InsertOEEStr);
                                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum].errorCode + "," + date + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum].errorStatus + "," + Global.ed[Global.Error_Stopnum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                                }
                                                else//机台处于其它异常状态中
                                                {
                                                    Log.WriteLog("PLC人工停止ErrorCode异常" + Global.Error_Stopnum + ",OEELog");
                                                }
                                            }
                                            Log.WriteLog(Global.ed[Global.Error_Stopnum].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum].errorinfo + "：结束计时 " + Global.ed[Global.Error_Stopnum].stop_time + ",OEELog");
                                            Global.ed[Global.Error_Stopnum].start_time = null;
                                            Global.ed[Global.Error_Stopnum].stop_time = null;
                                            Global.j = ReadStatus[0];
                                            //-------------上一个状态结束，当前状态开始计时--------------
                                            if (ReadStatus[0] == 4)//机台处于人工停止4状态
                                            {
                                                StopStatus();
                                            }
                                            else if (ReadStatus[0] == 3)//机台处于宕机3状态
                                            {
                                                ErrorStatus();
                                            }
                                            else if (ReadStatus[0] == 2)//机台处于运行2状态
                                            {
                                                RunStatus();
                                            }
                                            else if (ReadStatus[0] == 1)//机台处于待机待料1状态
                                            {
                                                PendingStatus();
                                            }
                                        }
                                        else
                                        { }
                                    }
                                }
                                else
                                {
                                }
                            }
                            else//处于空跑(PLC屏蔽部分功能)状态
                            {
                                if (Global.SelectTestRunModel == false)
                                {
                                    Global.SelectTestRunModel = true;
                                    var IP = GetIp();
                                    var Mac = GetMac();
                                    if (!Global.BreakStatus)//不是吃饭休息时
                                    {
                                        if (Global.j == 1)//处于待料状态
                                        {
                                            Global.ed[Global.Error_PendingNum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].start_time);
                                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].stop_time);
                                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].start_time + "'" + ","
                                                               + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum].errorCode + "," + Global.ed[Global.Error_PendingNum].start_time + "," + "'" + "" + "'" + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum].errorStatus + "," + Global.ed[Global.Error_PendingNum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                        }
                                        else if (Global.j == 2)//处于运行状态
                                        {
                                            Global.ed[Global.j].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.j].stop_time);
                                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                                               + "'" + "" + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + "'" + "" + "'" + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                        }
                                        else if (Global.j == 3)//处于宕机状态
                                        {
                                            Global.ed[Global.Error_num].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num].start_time);
                                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_num].stop_time);
                                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                            if (Global.Error_num == 7 || Global.Error_num == 10)//机台打开安全门
                                            {
                                                string OEE_DT2 = "";
                                                string msg2 = "";
                                                string date = Global.ed[Global.Error_num].start_time;
                                                OEE_DT2 = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.errorStatus, Global.errorcode, date, "");
                                                string InsertStr2 = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + ")";
                                                SQL.ExecuteUpdate(InsertStr2);
                                                Log.WriteLog("OEE_DT安全门打开:" + OEE_DT2);
                                                //var rst2 = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT2, out msg2);

                                                ///0909
                                                /// 
                                                ///Night
                                                /// 
                                                string poorNum1 = string.Empty;
                                                string TotalNum1 = string.Empty;
                                                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                                {
                                                    poorNum1 = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                    TotalNum1 = Global.Product_Total_N.ToString();
                                                }
                                                else
                                                {
                                                    poorNum1 = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                    TotalNum1 = Global.Product_Total_D.ToString();
                                                }
                                                Goee.UploadDowntime(poorNum1, TotalNum1, Global.errorStatus, Global.errorcode, Global.ed[Global.Error_num].ModuleCode, false, date, Global.errorinfo + ",安全门打开");
                                                ///



                                                //if (rst2)
                                                //{
                                                //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送成功", "rtx_DownTimeMsg");
                                                //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功");
                                                //    Global.ConnectOEEFlag = true;
                                                //}
                                                //else
                                                //{
                                                //    _homefrm.AppendRichText(Global.errorcode + ",触发时间=" + Global.ed[Global.Error_num].start_time + ",运行状态:" + Global.errorStatus + ",故障描述:" + Global.errorinfo + ",安全门打开自动发送失败", "rtx_DownTimeMsg");
                                                //    Log.WriteLog("OEE_DT安全门打开自动errorCode发送失败");
                                                //    Global.ConnectOEEFlag = false;
                                                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorcode + "'" + ","
                                                //     + "'" + Global.ed[Global.Error_num].start_time + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + ")";
                                                //    int r = SQL.ExecuteUpdate(s);
                                                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                                                //}
                                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errorcode + "'" + "," + "'" + date + "'" + ","
                                               + "'" + "" + "'" + "," + "'" + Global.errorStatus + "'" + "," + "'" + Global.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errorcode + "," + Global.ed[Global.Error_num].start_time + "," + "" + "," + "自动发送成功" + "," + Global.errorStatus + "," + Global.errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                            }
                                            else//机台处于其它异常状态中
                                            {
                                                string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + "," + "'" + Global.ed[Global.Error_num].start_time + "'" + ","
                                                               + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                                SQL.ExecuteUpdate(InsertOEEStr);
                                                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num].errorCode + "," + Global.ed[Global.Error_num].start_time + "," + "自动发送成功" + "," + Global.ed[Global.Error_num].errorStatus + "," + Global.ed[Global.Error_num].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                            }
                                        }
                                        else if (Global.j == 4)//处于人工停止状态
                                        {
                                            Global.ed[Global.Error_Stopnum].stop_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum].start_time);
                                            DateTime t2 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum].stop_time);
                                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                            string OEE_DT2 = "";
                                            string msg2 = "";
                                            string date = Global.ed[Global.Error_Stopnum].start_time;
                                            OEE_DT2 = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[Global.Error_Stopnum].errorStatus, Global.ed[Global.Error_Stopnum].errorCode, date, "");
                                            string InsertStr2 = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + ","
                                            + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].ModuleCode + "'" + ")";
                                            SQL.ExecuteUpdate(InsertStr2);
                                            Log.WriteLog("OEE_DT人工停止复位:" + OEE_DT2);
                                            //var rst2 = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT2, out msg2);

                                            ///0909
                                            /// 
                                            ///Night
                                            /// 
                                            string poorNum2 = string.Empty;
                                            string TotalNum2 = string.Empty;
                                            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                            {
                                                poorNum2 = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                                TotalNum2 = Global.Product_Total_N.ToString();
                                            }
                                            else
                                            {
                                                poorNum2 = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                                TotalNum2 = Global.Product_Total_D.ToString();
                                            }
                                            Goee.UploadDowntime(poorNum2, TotalNum2, Global.ed[Global.Error_Stopnum].errorStatus, Global.ed[Global.Error_Stopnum].errorCode, Global.ed[Global.Error_Stopnum].ModuleCode, false, date, Global.ed[Global.Error_Stopnum].errorinfo + ",人工停止复位");
                                            ///




                                            //if (rst2)
                                            //{
                                            //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum].errorinfo + ",人工停止复位自动发送成功", "rtx_DownTimeMsg");
                                            //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送成功");
                                            //    Global.ConnectOEEFlag = true;
                                            //}
                                            //else
                                            //{
                                            //    _homefrm.AppendRichText(Global.ed[Global.Error_Stopnum].errorCode + ",触发时间=" + Global.ed[Global.Error_Stopnum].start_time + ",运行状态:" + Global.ed[Global.Error_Stopnum].errorStatus + ",故障描述:" + Global.ed[Global.Error_Stopnum].errorinfo + ",人工停止复位自动发送失败", "rtx_DownTimeMsg");
                                            //    Log.WriteLog("OEE_DT人工停止复位自动errorCode发送失败");
                                            //    Global.ConnectOEEFlag = false;
                                            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + ","
                                            //     + "'" + date + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + ")";
                                            //    int r = SQL.ExecuteUpdate(s);
                                            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                                            //}
                                            string InsertOEEStr = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + "," + "'" + date + "'" + ","
                                           + "'" + "" + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                            SQL.ExecuteUpdate(InsertOEEStr);
                                            Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum].errorCode + "," + Global.ed[Global.Error_Stopnum].start_time + "," + "" + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum].errorStatus + "," + Global.ed[Global.Error_Stopnum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                        }
                                    }
                                    else
                                    {
                                        _manualfrm.Btn_UpLoad_break_Click(null, null);
                                    }

                                    string OEE_DT = "";
                                    string msg = "";
                                    string EventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                    Global.ed[211].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//空跑开始时间
                                    OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", "5", Global.ed[211].errorCode, EventTime, "");
                                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "5" + "'" + "," + "'" + Global.ed[211].errorCode + "'" + ","
                                             + "'" + EventTime + "'" + "," + "'" + "" + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr);
                                    Log.WriteLog("OEE_DT空跑:" + OEE_DT);
                                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);


                                    ///0909
                                    /// 
                                    ///Night
                                    /// 
                                    string poorNum = string.Empty;
                                    string TotalNum = string.Empty;
                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                    {
                                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                        TotalNum = Global.Product_Total_N.ToString();
                                    }
                                    else
                                    {
                                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                        TotalNum = Global.Product_Total_D.ToString();
                                    }
                                    Goee.UploadDowntime(poorNum, TotalNum, "5", Global.ed[211].errorCode, "", false, EventTime, "空跑");
                                    ///



                                    //if (rst)
                                    //{
                                    //    _homefrm.AppendRichText(Global.ed[211].errorCode + ",触发时间=" + EventTime + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",自动发送成功", "rtx_DownTimeMsg");
                                    //    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[211].errorCode + "," + EventTime + "," + "手动发送成功" + "," + "5" + "," + "空跑", @"F:\装机软件\系统配置\System_ini\");
                                    //    Log.WriteLog("OEE_DT机台空跑发送成功");
                                    //    Global.ConnectOEEFlag = true;
                                    //}
                                    //else
                                    //{
                                    //    _homefrm.AppendRichText(Global.ed[211].errorCode + ",触发时间=" + EventTime + ",运行状态:" + "5" + ",故障描述:" + "空跑" + ",自动发送失败", "rtx_DownTimeMsg");
                                    //    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[211].errorCode + "," + EventTime + "," + "手动发送失败" + "," + "5" + "," + "空跑", @"F:\装机软件\系统配置\System_ini\");
                                    //    Log.WriteLog("OEE_DT机台空跑发送失败");
                                    //    Global.ConnectOEEFlag = false;
                                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + "5" + "'" + "," + "'" + Global.ed[211].errorCode + "'" + ","
                                    //        + "'" + EventTime + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + "空跑" + "'" + ")";
                                    //    int r = SQL.ExecuteUpdate(s);
                                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                                    //}
                                    Global.j = -1;
                                    string InsertOEEStr3 = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + "5" + "'" + "," + "'" + Global.ed[211].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + "," + "'" + Global.ed[211].errorinfo + "'" + ")";
                                    SQL.ExecuteUpdate(InsertOEEStr3);//插入空跑开始时间
                                }
                            }
                        }
                        else//处于手动(首件)状态
                        {
                            Global.j = -1;
                            Global.SelectTestRunModel = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE-DwonTime异常！" + ex.ToString() + ",OEELog");
                        _homefrm.AppendRichText("OEE-DwonTime异常！", "rtx_DownTimeMsg");
                    }
                }
                Thread.Sleep(100);
            }
        }
        #endregion
        #region  OEE-心跳信号


        private void EthOEEHeartBeat()
        {
            while (true)
            {
                ///2021118
                Goee.UploadPlant();




                //try
                //{
                //    short[] PLCError = Global.PLC_Client.ReadPLC_D(Address_OEE_errorCode, 4);
                //    try
                //    {
                //        string HeartBeatmsg = "";
                //        string OEEHeartBeat = "";
                //        var IP = GetIp();
                //        var Mac = GetMac();
                //        if (Global.errorTime1 == false)//首件状态未开启
                //        {
                //            if (ReadTestRunStatus[0] != 1)//空跑状态未开启
                //            {
                //                if (PLCError[0] == 1)//机台待料状态时
                //                {
                //                    i = PLCError[2];
                //                    OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", Global.ED[i + 1].errorStatus, Global.ED[i + 1].errorCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                }
                //                else if (PLCError[0] == 2)//机台运行状态，ErrorCode为空
                //                {
                //                    i = 1;
                //                    OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", Global.ED[i + 1].errorStatus, "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                }
                //                else if (PLCError[0] == 3)//机台宕机状态
                //                {
                //                    i = PLCError[1];
                //                    OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", Global.ed[i + 1].errorStatus, Global.ed[i + 1].errorCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                }
                //                else if (PLCError[0] == 4)//机台人工停止复位状态
                //                {
                //                    i = PLCError[3];
                //                    OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", Global.ed[i + 1].errorStatus, Global.ed[i + 1].errorCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                }
                //                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 4, OEEHeartBeat, out HeartBeatmsg);
                //                Log.WriteLog("OEE_HeartBeat:" + OEEHeartBeat + ",OEELog");


                //                if (rst)
                //                {
                //                    _homefrm.AppendRichText(Global.ed[i + 1].errorStatus + "," + Global.ed[i + 1].errorCode + "," + Global.ed[i + 1].errorinfo + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                    Log.WriteLog("OEE_HeartBeat上传OK:" + HeartBeatmsg + ",OEELog");
                //                    Global.ConnectOEEFlag = true;
                //                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[i + 1].errorStatus + "," + Global.ed[i + 1].errorCode + "," + Global.ed[i + 1].errorinfo + "," + "OK-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //                    Global.PLC_Client.WritePLC_D(10108, new short[] { 1 });
                //                }
                //                else
                //                {
                //                    _homefrm.AppendRichText(Global.ed[i + 1].errorStatus + "," + Global.ed[i + 1].errorCode + "," + Global.ed[i + 1].errorinfo + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                    Log.WriteLog("OEE_HeartBeat上传NG:" + HeartBeatmsg + ",OEELog");
                //                    Global.ConnectOEEFlag = false;
                //                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[i + 1].errorStatus + "," + Global.ed[i + 1].errorCode + "," + Global.ed[i + 1].errorinfo + "," + "NG-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //                    Global.PLC_Client.WritePLC_D(10108, new short[] { 2 });
                //                }
                //            }
                //            else//空跑状态中
                //            {
                //                OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", "5", Global.ed[211].errorCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 4, OEEHeartBeat, out HeartBeatmsg);
                //                Log.WriteLog("OEE_HeartBeat:" + OEEHeartBeat + ",OEELog");
                //                if (rst)
                //                {
                //                    _homefrm.AppendRichText("5" + "," + Global.ed[211].errorCode + "," + "空跑" + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                    Log.WriteLog("OEE_HeartBeat上传OK:" + HeartBeatmsg + ",OEELog");
                //                    Global.ConnectOEEFlag = true;
                //                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "5" + "," + Global.ed[211].errorCode + "," + "空跑" + "," + "OK-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //                }
                //                else
                //                {
                //                    _homefrm.AppendRichText("5" + "," + Global.ed[211].errorCode + "," + "空跑" + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                    Log.WriteLog("OEE_HeartBeat上传NG:" + HeartBeatmsg + ",OEELog");
                //                    Global.ConnectOEEFlag = false;
                //                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + "5" + "," + Global.ed[211].errorCode + "," + "空跑" + "," + "NG-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //                }
                //            }
                //        }
                //        else //首件状态中
                //        {
                //            OEEHeartBeat = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\"}}", Global.errordata.errorStatus, Global.errordata.errorCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //            var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 4, OEEHeartBeat, out HeartBeatmsg);
                //            Log.WriteLog("OEE_HeartBeat:" + OEEHeartBeat + ",OEELog");
                //            if (rst)
                //            {
                //                _homefrm.AppendRichText(Global.errordata.errorStatus + "," + Global.errordata.errorCode + "," + Global.errordata.errorinfo + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                Log.WriteLog("OEE_HeartBeat上传OK:" + HeartBeatmsg + ",OEELog");
                //                Global.ConnectOEEFlag = true;
                //                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorStatus + "," + Global.errordata.errorCode + "," + Global.errordata.errorinfo + "," + "OK-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //            }
                //            else
                //            {
                //                _homefrm.AppendRichText(Global.errordata.errorStatus + "," + Global.errordata.errorCode + "," + Global.errordata.errorinfo + "," + HeartBeatmsg, "rtx_HeartBeatMsg");
                //                Log.WriteLog("OEE_HeartBeat上传NG:" + HeartBeatmsg + ",OEELog");
                //                Global.ConnectOEEFlag = false;
                //                Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorStatus + "," + Global.errordata.errorCode + "," + Global.errordata.errorinfo + "," + "NG-OEE_HeartBeat", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //            }
                //        }
                //    }
                //    catch (Exception EX)
                //    {
                //        _homefrm.AppendRichText("OEE_HeartBeat上传异常，请检查OEE网络！", "rtx_HeartBeatMsg");
                //        Log.WriteLog("OEE_HeartBeat上传异常:" + EX.ToString() + ",OEELog");
                //        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[i + 1].errorStatus + "," + Global.ed[i + 1].errorCode + "," + Global.ed[i + 1].errorinfo + "," + "NG-OEE_HeartBeat网络异常", @"F:\装机软件\系统配置\UpLoad_HeartBeat\");
                //    }

                //    Goee.UploadPlant();
                //}
                //catch
                //{
                //    Log.WriteLog("读取PLC运行状态错误，请检查PC与PLC通讯！" + ",OEELog");
                //    _homefrm.AppendRichText(DateTime.Now.ToString("读取PLC运行状态错误，请检查PC与PLC通讯！"), "rtx_HeartBeatMsg");
                //}
                //////////////////////////////////////////////////////////////
                //Global.PLC_Client.WritePLC_D(Address_errorCode, plcerror);
                //errordeal = false;


                Thread.Sleep(60000);//每分钟上传心跳信息
            }
        }
        #endregion
        #region  OEE Default上传
        private void EthOEEDefault()
        {
            while (true)
            {
                if (OEE.Count > 0)
                {
                    string item = Out_oee[0];
                    string msg = "";
                    string OEE_Data = "";
                    OEE[item].SwVersion = Global.inidata.productconfig.Sw_version;
                    try
                    {
                        if (item.Contains("DRD") && item.Length == 19 && !Regex.IsMatch(item, "[a-z]"))
                        {
                            string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}' and Station='L_Bracket'", item);//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (OEE[item].Status == "OK")
                            {
                                if (d1 != null && d1.Rows.Count > 3)
                                {
                                    OEE[item].Status = "OK";
                                }
                                else
                                {

                                    OEE[item].Status = "NG";
                                }
                            }
                            if (OEE[item].SerialNumber == "" || OEE[item].Fixture == "")
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", DateTime.Now.ToString("yyyyMMddHHmmss"), "", "", OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE[item].Status, OEE[item].ActualCT, OEE[item].SwVersion, "1");
                            }
                            else
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", OEE[item].SerialNumber, "", OEE[item].Fixture, OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE[item].Status, OEE[item].ActualCT, OEE[item].SwVersion, "1");
                            }
                            Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                            _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");
                            var IP = GetIp();
                            var Mac = GetMac();
                            //string errcode = string.Empty;
                            //if (ReadStatus[0] == 4)//机台处于人工停止4状态
                            //{
                            //    errcode = Global.ed[Global.Error_Stopnum].errorCode; 
                            //}
                            //else if (ReadStatus[0] == 3)//机台处于宕机3状态
                            //{
                            //    errcode = Global.ed[Global.Error_num].errorCode;
                            //}
                            //else if (ReadStatus[0] == 2)//机台处于运行2状态
                            //{
                            //    errcode = "";
                            //}
                            //else if (ReadStatus[0] == 1)//处于待料1状态
                            //{
                            //    errcode = Global.ed[Global.Error_PendingNum].errorCode;
                            //}
                            //////20210907 FEIQI
                            //Goee.UploadOEE(new OeeUpload() {
                            //    SerialNumber= OEE[item].SerialNumber,
                            //    BGBarcode="",
                            //    Fixture= OEE[item].Fixture,
                            //    StartTime=DateTime.Parse(OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            //    EndTime= DateTime.Parse(OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            //    Status= OEE[item].Status,
                            //    ActualCT= OEE[item].ActualCT,
                            //    SwVersion= OEE[item].SwVersion,
                            //    ScanCount=1,
                            //    Cavity="1",
                            //    ErrorCode=""
                            //});


                            var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Data, out msg);
                            if (rst)
                            {
                                Global.oee_ok++;
                                _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                                Global.oeeSend_ng = 0;
                                OEE_Default_flag = true;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = true;
                                Global.PLC_Client.WritePLC_D(10304, new short[] { 1 });
                                Log.WriteLog("OEE_Default返回结果-OK" + msg + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime + "," + OEE[item].EndTime + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                _homefrm.UiText(OEE[item].SerialNumber, "txtOEE_SerialNumber");
                                _homefrm.UiText(OEE[item].Fixture, "txtOEE_Fixture");
                                _homefrm.UiText(OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                                _homefrm.UiText(OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                                _homefrm.UiText(OEE[item].Status.ToString(), "txtOEE_Status");
                                _homefrm.UiText(OEE[item].ActualCT.ToString(), "txtOEE_ActualCT");
                                _homefrm.UiText(OEE[item].SwVersion, "txtOEE_sw");
                                _homefrm.UiText(OEE[item].ScanCount, "txtOEE_ScanCount");
                                Out_oee.RemoveAt(0);
                                OEE.Remove(item);
                                Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                            }
                            else
                            {
                                Global.oee_ng++;
                                _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                                Global.oeeSend_ng++;
                                //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                                OEE_Default_flag = false;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = false;
                                Global.PLC_Client.WritePLC_D(10304, new short[] { 2 });
                                Log.WriteLog("OEE_Default返回结果-NG" + msg + ",OEELog");
                                //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + OEE[item].ScanCount + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                                string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].SerialNumber + "'" + "," + "'" + OEE[item].Fixture + "'" + ","
                                                     + "'" + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].Status + "'" + "," + "'" + OEE[item].ActualCT + "'" + "," + "'" + OEE[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                                int r = SQL.ExecuteUpdate(s);
                                Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                                Out_oee.RemoveAt(0);
                                OEE.Remove(item);
                            }
                        }
                        else
                        {
                            Log.WriteLog("OEE_Default上传的SN格式不正确:" + item + ",OEELog");
                            _homefrm.AppendRichText("OEE_Default上传的SN格式不正确", "rtx_OEEDefaultMsg");
                            Global.PLC_Client.WritePLC_D(10304, new short[] { 2 });
                            Out_oee.RemoveAt(0);
                            OEE.Remove(item);
                        }
                    }
                    catch
                    {
                        Global.oee_ng++;
                        _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                        Global.oeeSend_ng++;
                        //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                        OEE_Default_flag = false;
                        _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                        _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                        Global.PLC_Client.WritePLC_D(10304, new short[] { 2 });
                        Log.WriteLog("数据发送OEE_Default异常_fail" + ",OEELog");
                        //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + OEE[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                        string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].SerialNumber + "'" + "," + "'" + OEE[item].Fixture + "'" + ","
                                                      + "'" + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE[item].Status + "'" + "," + "'" + OEE[item].ActualCT + "'" + "," + "'" + OEE[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                        int r = SQL.ExecuteUpdate(s);
                        Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + OEE[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE[item].SerialNumber + "," + OEE[item].Fixture + "," + OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE[item].Status + "," + OEE[item].ActualCT + "," + OEE[item].SwVersion + "," + OEE[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                        Out_oee.RemoveAt(0);
                        OEE.Remove(item);
                    }
                }
                Thread.Sleep(30);
            }
        }

        private void SendOEEDEFAULT(OeeUpload OEEData)
        {
            Log.WriteLog("SendOEEDEFAULT" + JsonConvert.SerializeObject(OEEData));
            try
            {

                string msg = "";
                string OEE_Data = "";
                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", String.IsNullOrEmpty(OEEData.SerialNumber) == true ? OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") : OEEData.SerialNumber, "", OEEData.Fixture, OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEEData.Status, OEEData.ActualCT, OEEData.SwVersion, "1");
                Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");
                var IP = GetIp();
                var Mac = GetMac();
                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Data, out msg);
                if (rst)
                {
                    Global.oee_ok++;
                    _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                    Global.oeeSend_ng = 0;
                    OEE_Default_flag = true;


                    _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                    Global.ConnectOEEFlag = true;
                    //Global.PLC_Client.WritePLC_D(10304, new short[] { 1 });
                    Log.WriteLog("OEE_Default返回结果-OK" + msg + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime + "," + OEEData.EndTime + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                    _homefrm.UiText(OEEData.SerialNumber, "txtOEE_SerialNumber");
                    _homefrm.UiText(OEEData.Fixture, "txtOEE_Fixture");
                    _homefrm.UiText(OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                    _homefrm.UiText(OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                    _homefrm.UiText(OEEData.Status.ToString(), "txtOEE_Status");
                    _homefrm.UiText(OEEData.ActualCT.ToString(), "txtOEE_ActualCT");
                    _homefrm.UiText(OEEData.SwVersion, "txtOEE_sw");
                    _homefrm.UiText(OEEData.ScanCount.ToString(), "txtOEE_ScanCount");

                    Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                }
                else
                {
                    Global.oee_ng++;
                    _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                    Global.oeeSend_ng++;
                    //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                    OEE_Default_flag = false;

                    _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                    Global.ConnectOEEFlag = false;
                    //Global.PLC_Client.WritePLC_D(10304, new short[] { 2 });
                    Log.WriteLog("OEE_Default返回结果-NG" + msg + ",OEELog");
                    //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                    string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                         + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                    int r = SQL.ExecuteUpdate(s);
                    Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");

                }

            }
            catch
            {
                Global.oee_ng++;
                _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                Global.oeeSend_ng++;
                //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                OEE_Default_flag = false;

                _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                //Global.PLC_Client.WritePLC_D(10304, new short[] { 2 });
                Log.WriteLog("数据发送OEE_Default异常_fail" + ",OEELog");
                //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                              + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                int r = SQL.ExecuteUpdate(s);
                Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");

            }
        }
        #endregion

        #region  OEE Default_NG上传
        private void EthOEEDefault_NG()
        {
            while (true)
            {
                if (OEE_NG.Count > 0)
                {
                    string item = Out_oee_NG[0];
                    string msg = "";
                    string OEE_Data = "";
                    OEE_NG[item].SwVersion = Global.inidata.productconfig.Sw_version;
                    try
                    {
                        if (item.Contains("DRD") && item.Length == 19 && !Regex.IsMatch(item, "[a-z]"))
                        {

                            string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}' and Station='L_Bracket'", item);//sql查询语句
                            DataTable d1 = SQL.ExecuteQuery(SelectStr);
                            if (OEE_NG[item].Status == "OK")
                            {
                                if (d1 != null && d1.Rows.Count > 3)
                                {
                                    OEE_NG[item].Status = "OK";
                                }
                                else
                                {

                                    OEE_NG[item].Status = "NG";
                                }
                            }
                            if (OEE_NG[item].SerialNumber == "" || OEE_NG[item].Fixture == "")
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", DateTime.Now.ToString("yyyyMMddHHmmss"), "", "", OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].Status, OEE_NG[item].ActualCT, OEE_NG[item].SwVersion, "1");
                            }
                            else
                            {
                                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", OEE_NG[item].SerialNumber, "", OEE_NG[item].Fixture, OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEE_NG[item].Status, OEE_NG[item].ActualCT, OEE_NG[item].SwVersion, "1");
                            }
                            Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                            _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");

                            //string errcode = string.Empty;
                            //if (ReadStatus[0] == 4)//机台处于人工停止4状态
                            //{
                            //    errcode = Global.ed[Global.Error_Stopnum].errorCode;
                            //}
                            //else if (ReadStatus[0] == 3)//机台处于宕机3状态
                            //{
                            //    errcode = Global.ed[Global.Error_num].errorCode;
                            //}
                            //else if (ReadStatus[0] == 2)//机台处于运行2状态
                            //{
                            //    errcode = "";
                            //}
                            //else if (ReadStatus[0] == 1)//处于待料1状态
                            //{
                            //    errcode = Global.ed[Global.Error_PendingNum].errorCode;
                            //}
                            //////20210907 FEIQI
                            //Goee.UploadOEE(new OeeUpload()
                            //{
                            //    SerialNumber = OEE[item].SerialNumber,
                            //    BGBarcode = "",
                            //    Fixture = OEE[item].Fixture,
                            //    StartTime = DateTime.Parse(OEE[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            //    EndTime = DateTime.Parse(OEE[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss")),
                            //    Status = OEE[item].Status,
                            //    ActualCT = OEE[item].ActualCT,
                            //    SwVersion = OEE[item].SwVersion,
                            //    ScanCount = 1,
                            //    Cavity = "1",
                            //    ErrorCode = ""
                            //});

                            var IP = GetIp();
                            var Mac = GetMac();
                            var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Data, out msg);




                            if (rst)
                            {
                                Global.oee_ok++;
                                _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                                Global.oeeSend_ng = 0;
                                OEE_Default_flag = true;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = true;
                                Global.PLC_Client.WritePLC_D(10504, new short[] { 1 });
                                Log.WriteLog("OEE_Default返回结果-OK" + msg + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime + "," + OEE_NG[item].EndTime + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                _homefrm.UiText(OEE_NG[item].SerialNumber, "txtOEE_SerialNumber");
                                _homefrm.UiText(OEE_NG[item].Fixture, "txtOEE_Fixture");
                                _homefrm.UiText(OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                                _homefrm.UiText(OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                                _homefrm.UiText(OEE_NG[item].Status.ToString(), "txtOEE_Status");
                                _homefrm.UiText(OEE_NG[item].ActualCT.ToString(), "txtOEE_ActualCT");
                                _homefrm.UiText(OEE_NG[item].SwVersion, "txtOEE_sw");
                                _homefrm.UiText(OEE_NG[item].ScanCount, "txtOEE_ScanCount");
                                Out_oee_NG.RemoveAt(0);
                                OEE_NG.Remove(item);
                                Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                            }
                            else
                            {
                                Global.oee_ng++;
                                _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                                Global.oeeSend_ng++;
                                //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                                OEE_Default_flag = false;
                                _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                                Global.ConnectOEEFlag = false;
                                Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                                Log.WriteLog("OEE_Default返回结果-NG" + msg + ",OEELog");
                                //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                                string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].SerialNumber + "'" + "," + "'" + OEE_NG[item].Fixture + "'" + ","
                                                     + "'" + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].Status + "'" + "," + "'" + OEE_NG[item].ActualCT + "'" + "," + "'" + OEE_NG[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                                int r = SQL.ExecuteUpdate(s);
                                Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                                Out_oee_NG.RemoveAt(0);
                                OEE_NG.Remove(item);
                            }
                        }
                        else
                        {
                            Log.WriteLog("OEE_Default上传的SN格式不正确:" + item + ",OEELog");
                            _homefrm.AppendRichText("OEE_Default上传的SN格式不正确", "rtx_OEEDefaultMsg");
                            Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                            Out_oee_NG.RemoveAt(0);
                            OEE_NG.Remove(item);
                        }


                    }
                    catch
                    {
                        try
                        {
                            Global.oee_ng++;
                            _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                            Global.oeeSend_ng++;
                            //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                            OEE_Default_flag = false;
                            _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                            _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                            Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                            Log.WriteLog("数据发送OEE_Default异常_fail" + ",OEELog");
                            //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                            string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].SerialNumber + "'" + "," + "'" + OEE_NG[item].Fixture + "'" + ","
                                                          + "'" + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEE_NG[item].Status + "'" + "," + "'" + OEE_NG[item].ActualCT + "'" + "," + "'" + OEE_NG[item].SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                            int r = SQL.ExecuteUpdate(s);
                            Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEE_NG[item].SerialNumber + "," + OEE_NG[item].Fixture + "," + OEE_NG[item].StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEE_NG[item].Status + "," + OEE_NG[item].ActualCT + "," + OEE_NG[item].SwVersion + "," + OEE_NG[item].ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");
                            Out_oee_NG.RemoveAt(0);
                            OEE_NG.Remove(item);
                        }
                        catch (Exception EX)
                        {

                            Log.WriteLog(EX.Message); ;
                        }

                    }
                }
                Thread.Sleep(30);
            }
        }


        private void SendoeedataNG(OeeUpload OEEData)
        {

            string msg = "";
            string OEE_Data = "";
            try
            {
                OEE_Data = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", String.IsNullOrEmpty(OEEData.SerialNumber) == true ? OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") : OEEData.SerialNumber, "", OEEData.Fixture, OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), OEEData.Status, OEEData.ActualCT, OEEData.SwVersion, "1");


                Log.WriteLog("OEE_Default:" + OEE_Data + ",OEELog");
                _homefrm.AppendRichText(OEE_Data, "rtx_OEEDefaultMsg");
                var IP = GetIp();
                var Mac = GetMac();
                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Data, out msg);




                if (rst)
                {
                    Global.oee_ok++;
                    _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                    Global.oeeSend_ng = 0;
                    OEE_Default_flag = true;
                    _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                    Global.ConnectOEEFlag = true;
                    //Global.PLC_Client.WritePLC_D(10504, new short[] { 1 });
                    Log.WriteLog("OEE_Default返回结果-OK" + msg + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime + "," + OEEData.EndTime + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                    _homefrm.UiText(OEEData.SerialNumber, "txtOEE_SerialNumber");
                    _homefrm.UiText(OEEData.Fixture, "txtOEE_Fixture");
                    _homefrm.UiText(OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_StartTime");
                    _homefrm.UiText(OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), "txtOEE_EndTime");
                    _homefrm.UiText(OEEData.Status.ToString(), "txtOEE_Status");
                    _homefrm.UiText(OEEData.ActualCT.ToString(), "txtOEE_ActualCT");
                    _homefrm.UiText(OEEData.SwVersion, "txtOEE_sw");
                    _homefrm.UiText(OEEData.ScanCount.ToString(), "txtOEE_ScanCount");

                    Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                }
                else
                {
                    Global.oee_ng++;
                    _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                    Global.oeeSend_ng++;
                    //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                    OEE_Default_flag = false;
                    _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送失败", "lb_OEE_UA_SendStatus");
                    Global.ConnectOEEFlag = false;
                    ////Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                    Log.WriteLog("OEE_Default返回结果-NG" + msg + ",OEELog");
                    //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," +OEEData.SerialNumber + "," +OEEData.Fixture + "," +OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," +OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," +OEEData.Status + "," +OEEData.ActualCT + "," +OEEData.SwVersion + "," +OEEData.ScanCount + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                    string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                         + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                    int r = SQL.ExecuteUpdate(s);
                    Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + "1" + "," + "NG-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");

                }
            }
            catch (Exception)
            {
                try
                {
                    Global.oee_ng++;
                    _homefrm.UpDatalabel(Global.oee_ng.ToString(), "lb_OEENG");
                    Global.oeeSend_ng++;
                    //_homefrm.UpDatalabel(Global.oeeSend_ng.ToString(), "lb_OEESendNG");
                    OEE_Default_flag = false;
                    _homefrm.AppendRichText("OEE_Default-网络异常", "rtx_OEEDefaultMsg");
                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default-网络异常", "lb_OEE_UA_SendStatus");
                    //Global.PLC_Client.WritePLC_D(10504, new short[] { 2 });
                    Log.WriteLog("数据发送OEE_Default异常_fail" + ",OEELog");
                    //Log.WriteCSV_NG(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," +OEEData.SerialNumber + "," +OEEData.Fixture + "," +OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," +OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," +OEEData.Status + "," +OEEData.ActualCT + "," +OEEData.SwVersion + "," +OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_DefaultSendNG数据\\");
                    string s = "insert into OEE_DefaultSendNG([DateTime],[SerialNumber],[Fixture],[StartTime],[EndTime],[Status],[ActualCT],[SwVersion],[ScanCount])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.SerialNumber + "'" + "," + "'" + OEEData.Fixture + "'" + ","
                                                  + "'" + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + OEEData.Status + "'" + "," + "'" + OEEData.ActualCT + "'" + "," + "'" + OEEData.SwVersion + "'" + "," + "'" + "1" + "'" + ")";
                    int r = SQL.ExecuteUpdate(s);
                    Log.WriteLog(string.Format("插入了{0}行OEEData_SendNG缓存数据", r) + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + OEEData.SerialNumber + "," + OEEData.Fixture + "," + OEEData.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + OEEData.Status + "," + OEEData.ActualCT + "," + OEEData.SwVersion + "," + OEEData.ScanCount + "," + "NG-OEE_Default网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default上传NG数据\\");

                }
                catch (Exception EX)
                {

                    Log.WriteLog(EX.Message); ;
                }
            }
        }




        #endregion


        #region  OEE Machine上传
        public void txtFixtureNumber_TextChanged(object sender, EventArgs e)
        {
            string OEEMachine = "";
            string OEEMachineMsg = "";
            string FixtureNumber = Global.inidata.productconfig.FixtureNumber;
            try
            {
                var IP = GetIp();
                var Mac = GetMac();
                OEEMachine = string.Format("{{\"FixtureNum\":\"{0}\"}}", FixtureNumber);
                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 3, OEEMachine, out OEEMachineMsg);
                Log.WriteLog("OEE_Machine：" + OEEMachine);
                if (rst)
                {
                    _homefrm.AppendRichText("OEE_Machine上传成功" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                    Log.WriteLog("OEE_Machine上传OK:" + OEEMachineMsg);
                    Global.ConnectOEEFlag = true;
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "OK-OEE_Machine", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine数据\\");
                }
                else
                {
                    _homefrm.AppendRichText("OEE_Machine上传失败" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                    Log.WriteLog("OEE_Machine上传NG:" + OEEMachineMsg);
                    Global.ConnectOEEFlag = false;
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "NG-OEE_Machine", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine上传NG数据\\");
                }
            }
            catch
            {
                _homefrm.AppendRichText("OEE_Machine上传异常,请检查OEE网络！" + ":  " + OEEMachineMsg, "rtx_HeartBeatMsg");
                Log.WriteLog("OEE_Machine上传异常");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Machine" + "," + FixtureNumber + "," + "NG-OEE_Machine网络异常", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Machine上传NG数据\\");
            }
        }
        #endregion
        #region  OEE NG抛料日志上传上传
        private void EthOEEDiscardLog()
        {
            //JsonSerializerSettings jsetting = new JsonSerializerSettings();
            //jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
            //while (true)
            //{
            //    if (Out_oee_discard.Count > 0)
            //    {
            //        if (OEE_discard.Count > 0)
            //        {
            //            string msg = "";
            //            string item = Out_oee_discard[0];
            //            string uptime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //            try
            //            {
            //                DiscardData NG_data = new DiscardData();
            //                NG_data.sn = OEE_discard[item].SN;
            //                NG_data.SystemType = OEE_discard[item].SystemType;
            //                NG_data.uptime = uptime;
            //                NG_data.JSONBody = OEE_discard[item].JSONBody;
            //                NG_data.error = OEE_discard[item].Error;
            //                NG_data.frenquency = OEE_discard[item].Frenquency;
            //                string OEE_NG_log = JsonConvert.SerializeObject(NG_data, Formatting.None, jsetting);
            //                Log.WriteLog("OEE_DiscardLog:" + OEE_NG_log);
            //                var IP = GetIp();
            //                var Mac = GetMac();
            //                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 5, OEE_NG_log, out msg);
            //                if (rst)
            //                {
            //                    Log.WriteLog("OEE_DiscardLog上传OK:" + msg);
            //                    Global.ConnectOEEFlag = true;
            //                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "OK-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                }
            //                else
            //                {
            //                    Log.WriteLog("OEE_DiscardLog上传NG:" + msg);
            //                    Global.ConnectOEEFlag = false;
            //                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "NG-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                }
            //                Out_oee_discard.RemoveAt(0);
            //                OEE_discard.Remove(item);
            //            }
            //            catch (Exception ex)
            //            {
            //                Log.WriteLog("发送DiscardLog异常_fail" + ex.ToString().Replace("\n",""));
            //                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + item.Split(',')[0] + "," + OEE_discard[item].SystemType + "," + uptime + "," + OEE_discard[item].JSONBody + "," + OEE_discard[item].Error + "," + OEE_discard[item].Frenquency + "," + "NG-OEE_DiscardLog", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_抛料日志数据\\");
            //                Out_oee_discard.RemoveAt(0);
            //                OEE_discard.Remove(item);
            //            }
            //        }
            //    }
            //    Thread.Sleep(200);
            //}
        }

        #endregion
        #region  OEE 小料抛料计数上传
        private void EthOEEMateriel()
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
            while (true)
            {
                string msg = "";
                MaterielData data = new MaterielData();
                Global.TotalThrowCount = Global.TotalThrowCount + Global.ThrowCount + Global.ThrowOKCount;
                try
                {
                    data.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    data.count = (Global.NutCount).ToString();
                    data.totalcount = Global.TotalThrowCount.ToString();
                    data.parttype = string.Format("{0}", Global.NutCount);
                    _homefrm.UpDatalabel(Global.TotalThrowCount.ToString(), "lb_Materiel_Total");
                    string Materiel_data = JsonConvert.SerializeObject(data, Formatting.None, jsetting);
                    Log.WriteLog("OEE_MaterielData:" + Materiel_data + ",OEELog");
                    _homefrm.AppendRichText(Materiel_data, "rtx_OEEMateriel");
                    var IP = GetIp();
                    var Mac = GetMac();
                    var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 6, Materiel_data, out msg);
                    if (rst)
                    {
                        _homefrm.AppendRichText(Materiel_data, msg);
                        Global.ConnectOEEFlag = true;
                        Log.WriteLog("OEE_MaterielData上传OK:" + msg + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "OK-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                    }
                    else
                    {
                        _homefrm.AppendRichText(Materiel_data, msg);
                        Global.ConnectOEEFlag = false;
                        Log.WriteLog("OEE_MaterielData上传NG:" + msg + ",OEELog");
                        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "NG-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                        //Access.InsertData_OEE_Materiel("PDCA", "OEE_MaterielData", data.date, data.count, data.totalcount, UACount.ToString(), LACount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("发送MaterielData异常_fail" + ex.ToString().Replace("\n", "") + ",OEELog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + data.parttype + "," + "NG-OEE_MaterielData", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
                }
                _homefrm.UpDatalabel("0", "lb_Materiel_AllNut");
                _homefrm.UpDatalabel("0", "lb_Materiel_Nut");
                _homefrm.UpDatalabel("0", "lb_Materiel_AllOK");
                _homefrm.UpDatalabel("0", "lb_Materiel_OK");
                Thread.Sleep(60000);
            }
        }
        #endregion

        #region 数据操作
        private void ProcessControl(object obj)//Trace前站校验
        {
            ProcessControlData Msg_bracket;
            string Trace_str_bracket = "";
            ThreadInfo threadInfo = (ThreadInfo)obj;
            try
            {
                if (threadInfo.SN.Length == 19 && threadInfo.SN.Remove(3) == "DRD")
                {
                    string RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _homefrm.AppendRichText("SendProcessControl_SN:" + threadInfo.SN, "rtx_ProcessControl");
                    _homefrm.UiText(threadInfo.SN, "txt_ProcessControl_SN");
                    RequestAPI2.Trace_process_control(Global.inidata.productconfig.Trace_CheakSN_Bracket, threadInfo.SN, out Trace_str_bracket, out Msg_bracket);//Trace_bracket校验前站
                    Log.WriteLog("Trace校验bt前站SN：" + threadInfo.SN + "  " + "结果：" + Trace_str_bracket + JsonConvert.SerializeObject(Msg_bracket));
                    string ResponseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (Msg_bracket.Pass == "True")//先校验bt前站
                    {

                        ///20211018
                        /// 
                        if (Global.inidata.productconfig.IFactory_online == "2")
                        {

                            Trace_check_flag = true;
                            string callresult = "";
                            string errmsg = "";
                            string URL = "http://17.80.194.10/api/v2/parts?serial_type=band&serial=*&process_name=bt-wld&last_log=true";
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            var rst = RequestAPI3.CallBobcat(URL.Replace("*", threadInfo.SN), "", "zhh", "DgQT4Thy", out callresult, out errmsg);
                            sw.Stop();
                            Log.WriteLog("获取本站上传数据耗时：" + sw.ElapsedMilliseconds.ToString());
                            Log.WriteLog("已查询到本站上传数据：" + JsonConvert.SerializeObject(callresult));
                            //Log.WriteCSV(threadInfo.SN+":"+callresult, System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                            if (callresult != "")
                            {
                                BT_History MESRespondData = JsonConvert.DeserializeObject<BT_History>(callresult);

                                ////1202
                                //|| MESRespondData.history[0].data.insight.test_attributes.test_result == "fail"
                                if (MESRespondData.history == null || MESRespondData.history[0].data.insight.test_attributes.test_result == "fail")
                                {
                                    Global.PLC_Client.WritePLC_D(10122, new short[] { 1 });
                                    //Log.WriteCSV(threadInfo.SN + ":10122结果:1", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                                    _homefrm.UpDatalabelcolor(Color.Green, "Trace校验SN成功", "txt_ProcessControl_Status");
                                    Global.Product_num_Process_ok++;
                                    _homefrm.UpDatalabel(Global.Product_num_Process_ok.ToString(), "lb_ProcessControlOK");
                                    _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                                    _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                                }
                                //if (MESRespondData.history[0].data.insight.test_attributes.test_result == "pass")
                                else if (MESRespondData.history[0].data.insight.test_attributes.test_result == "pass")
                                {
                                    Global.PLC_Client.WritePLC_D(10122, new short[] { 2 });
                                    //Log.WriteCSV(threadInfo.SN + ":10122结果:2", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                                    _homefrm.UpDatalabelcolor(Color.Red, "Trace已上传本站", "txt_ProcessControl_Status");
                                    ///tRACEpV检查NG1 
                                    /// 
                                    var OEEDATA = new OeeUpload()
                                    {
                                        SerialNumber = threadInfo.SN,
                                        BGBarcode = "",
                                        Fixture = "",
                                        StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")),
                                        EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                                        Status = "NG",
                                        ActualCT = Global.ActualCT,
                                        SwVersion = Global.inidata.productconfig.Sw_version,
                                        ScanCount = 1,
                                        Cavity = "1",
                                        ErrorCode = "080001"
                                    };
                                    Task T1 = new Task(() =>
                                    {
                                        Goee.UploadOEE(OEEDATA);
                                        ///2021118
                                        //SendoeedataNG(OEEDATA);
                                    });
                                    T1.Start();
                                    //Task T2 = new Task(() =>
                                    //SendoeedataNG(OEEDATA));
                                    //T2.Start();
                                    Global.Product_num_Process_ok++;

                                    //Global.TracePVCheck_Error++;

                                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                    {
                                        Global.TracePVCheck_Error_D++;//白班前站校验失败计数累加
                                        _datastatisticsfrm.UpDataDGV_D(4, 1, Global.TracePVCheck_Error_D.ToString());
                                        Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                                        Global.inidata.WriteProductnumSection();
                                    }
                                    else
                                    {
                                        Global.TracePVCheck_Error_N++;//夜班前站校验失败计数累加
                                        _datastatisticsfrm.UpDataDGV_N(4, 1, Global.TracePVCheck_Error_N.ToString());
                                        Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                                        Global.inidata.WriteProductnumSection();
                                    }

                                    _homefrm.UpDatalabel(Global.Product_num_Process_ok.ToString(), "lb_ProcessControlOK");
                                    _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                                    _homefrm.AppendRichText("本站已上传记录：" + callresult, "rtx_ProcessControlErrorMsg");
                                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "OK料重投", @"D:\OK料重投记录\");
                                }
                                else
                                {
                                    Global.PLC_Client.WritePLC_D(10122, new short[] { 1 });
                                    //Log.WriteCSV(threadInfo.SN + ":10122结果:1", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                                    _homefrm.UpDatalabelcolor(Color.Green, "Trace校验SN成功", "txt_ProcessControl_Status");
                                    Global.Product_num_Process_ok++;
                                    _homefrm.UpDatalabel(Global.Product_num_Process_ok.ToString(), "lb_ProcessControlOK");
                                    _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                                    _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                                }
                            }
                            else
                            {
                                Global.PLC_Client.WritePLC_D(10122, new short[] { 2 });
                                //Log.WriteCSV(threadInfo.SN + ":10122结果:2", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                                _homefrm.UpDatalabelcolor(Color.Red, "Trace校验SN失败", "txt_ProcessControl_Status");
                                ///tRACEpV检查NG2 
                                /// 
                                var OEEDATA = new OeeUpload()
                                {
                                    SerialNumber = threadInfo.SN,
                                    BGBarcode = "",
                                    Fixture = "",
                                    StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")),
                                    EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                                    Status = "NG",
                                    ActualCT = Global.ActualCT,
                                    SwVersion = Global.inidata.productconfig.Sw_version,
                                    ScanCount = 1,
                                    Cavity = "1",
                                    ErrorCode = "080001"
                                };
                                Task T1 = new Task(() =>
                                {
                                    Goee.UploadOEE(OEEDATA);
                                    ///2021118
                                        //SendoeedataNG(OEEDATA);
                                });
                                T1.Start();
                                Global.Product_num_Process_ng++;


                                //Global.TracePVCheck_Error++;

                                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                                {
                                    Global.TracePVCheck_Error_D++;//白班前站校验失败计数累加
                                    _datastatisticsfrm.UpDataDGV_D(4, 1, Global.TracePVCheck_Error_D.ToString());
                                    Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                                    Global.inidata.WriteProductnumSection();
                                }
                                else
                                {
                                    Global.TracePVCheck_Error_N++;//夜班前站校验失败计数累加
                                    _datastatisticsfrm.UpDataDGV_N(4, 1, Global.TracePVCheck_Error_N.ToString());
                                    Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                                    Global.inidata.WriteProductnumSection();
                                }

                                _homefrm.UpDatalabel(Global.Product_num_Process_ng.ToString(), "lb_ProcessControlNG");
                                _homefrm.AppendRichText(errmsg, "rtx_ProcessControl");
                                Log.WriteLog("查询本站记录失败:" + errmsg);
                            }


                        }
                        else
                        {
                            Global.PLC_Client.WritePLC_D(10122, new short[] { 1 });
                            Log.WriteCSV(threadInfo.SN + ":10122结果:1", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                            _homefrm.UpDatalabelcolor(Color.Green, "Trace校验SN成功.", "txt_ProcessControl_Status");
                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "屏蔽IFactory前站", @"D:\屏蔽IFactory前站\");
                            Global.Product_num_Process_ok++;
                            _homefrm.UpDatalabel(Global.Product_num_Process_ok.ToString(), "lb_ProcessControlOK");
                            _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                            _homefrm.AppendRichText("N/A", "rtx_ProcessControlErrorMsg");
                        }
                    }
                    else
                    {
                        _homefrm.UpDatalabelcolor(Color.Red, "Trace校验SN失败", "txt_ProcessControl_Status");

                        ///tRACEpV检查NG3 
                        /// 

                        var OEEDATA = new OeeUpload()
                        {
                            SerialNumber = threadInfo.SN,
                            BGBarcode = "",
                            Fixture = "",
                            StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")),
                            EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            Status = "NG",
                            ActualCT = Global.ActualCT,
                            SwVersion = Global.inidata.productconfig.Sw_version,
                            ScanCount = 1,
                            Cavity = "1",
                            ErrorCode = "080001"
                        };
                        Task T1 = new Task(() =>
                        {
                            Goee.UploadOEE(OEEDATA);
                            ///2021118
                                        //SendoeedataNG(OEEDATA);
                        });
                        T1.Start();
                        Global.Product_num_Process_ng++;

                        //Global.TracePVCheck_Error++;

                        if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                        {
                            Global.TracePVCheck_Error_D++;//白班前站校验失败计数累加
                            _datastatisticsfrm.UpDataDGV_D(4, 1, Global.TracePVCheck_Error_D.ToString());
                            Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                            Global.inidata.WriteProductnumSection();
                        }
                        else
                        {
                            Global.TracePVCheck_Error_N++;//夜班前站校验失败计数累加
                            _datastatisticsfrm.UpDataDGV_N(4, 1, Global.TracePVCheck_Error_N.ToString());
                            Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                            Global.inidata.WriteProductnumSection();
                        }
                        _homefrm.UpDatalabel(Global.Product_num_Process_ng.ToString(), "lb_ProcessControlNG");
                        _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                        Global.PLC_Client.WritePLC_D(10122, new short[] { 2 });
                        string FalseData = JsonConvert.SerializeObject(Msg_bracket, Formatting.Indented);
                        _homefrm.AppendRichText(FalseData, "rtx_ProcessControlErrorMsg");
                    }
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "前站Trace", System.AppDomain.CurrentDomain.BaseDirectory + "\\前站Trace数据\\");
                }
                else
                {
                    Global.PLC_Client.WritePLC_D(10122, new short[] { 2 });
                    Log.WriteCSV(threadInfo.SN + ":10122结果:2", System.AppDomain.CurrentDomain.BaseDirectory + "\\Trace校验SN日志\\");
                    Global.Product_num_Process_ng++;

                    //Global.TracePVCheck_Error++;

                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                    {
                        Global.TracePVCheck_Error_D++;//白班前站校验失败计数累加
                        _datastatisticsfrm.UpDataDGV_D(4, 1, Global.TracePVCheck_Error_D.ToString());
                        Global.inidata.productconfig.TracePVCheck_Error_D = Global.TracePVCheck_Error_D.ToString();
                        Global.inidata.WriteProductnumSection();
                    }
                    else
                    {
                        Global.TracePVCheck_Error_N++;//夜班前站校验失败计数累加
                        _datastatisticsfrm.UpDataDGV_N(4, 1, Global.TracePVCheck_Error_N.ToString());
                        Global.inidata.productconfig.TracePVCheck_Error_N = Global.TracePVCheck_Error_N.ToString();
                        Global.inidata.WriteProductnumSection();
                    }

                    _homefrm.UpDatalabel(Global.Product_num_Process_ng.ToString(), "lb_ProcessControlNG");
                    //Product_num_Mes_NG++;
                    Log.WriteLog("校验前站SN格式不正确," + threadInfo.SN);
                    ///tRACEpV检查NG4
                    /// 

                    var OEEDATA = new OeeUpload()
                    {
                        SerialNumber = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        BGBarcode = "",
                        Fixture = "",
                        StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")),
                        EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        Status = "NG",
                        ActualCT = Global.ActualCT,
                        SwVersion = Global.inidata.productconfig.Sw_version,
                        ScanCount = 1,
                        Cavity = "1",
                        ErrorCode = "080001"
                    };
                    Task T1 = new Task(() =>
                    {
                        Goee.UploadOEE(OEEDATA);
                        ///2021118
                                        //SendoeedataNG(OEEDATA);
                    });
                    T1.Start();
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + threadInfo.SN + "," + "," + "," + "," + "," + "," + "," + "," + "前站Trace/JP异常,BED001", System.AppDomain.CurrentDomain.BaseDirectory + "\\前站Trace/JP异常数据\\");
                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("卡上一站失败:" + Trace_str_bracket);
                Log.WriteLog("卡上一站失败:" + EX.ToString());
                //_homefrm.AppendRichText(Trace_str_ua, "rtx_ProcessControl");
                _homefrm.AppendRichText(Trace_str_bracket, "rtx_ProcessControl");
                _homefrm.UpDatalabelcolor(Color.Red, "Trace校验异常，请检查网络", "txt_ProcessControl_Status");
                ///tRACEpV检查NG5 
                /// 

                var OEEDATA = new OeeUpload()
                {
                    SerialNumber = threadInfo.SN,
                    BGBarcode = "",
                    Fixture = "",
                    StartTime = DateTime.Parse(DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss")),
                    EndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    Status = "NG",
                    ActualCT = Global.ActualCT,
                    SwVersion = Global.inidata.productconfig.Sw_version,
                    ScanCount = 1,
                    Cavity = "1",
                    ErrorCode = "080001"
                };
                Task T1 = new Task(() =>
                {
                    Goee.UploadOEE(OEEDATA);
                    ///2021118
                                        //SendoeedataNG(OEEDATA);
                });
                T1.Start();
                Trace_check_flag = false;
                Global.PLC_Client.WritePLC_D(10122, new short[] { 2 });
                string JSONBody = Trace_str_bracket.Replace(",", ";").Replace("\n", "");
                Log.WriteCSV_DiscardLog("TracePV" + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "" + "," + Global.inidata.productconfig.OEE_Dsn + "," + threadInfo.SN + "," + threadInfo.SN + "," + JSONBody + "," + "1");
            }
        }

        private void SendDataToHans(object obj)//发送SN给大族焊接机和普雷斯特
        {
            lock (LockHans)
            {
                try
                {
                    int result = 0;
                    AsyncTcpClient client = null;
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    switch (threadInfo.SelectedIndex)
                    {
                        case 1:
                            client = Global.client1;
                            result = 10251;
                            break;
                        case 2:
                            client = Global.client2;
                            result = 10253;
                            break;
                        default:
                            break;
                    }
                    if (TCPconnected)
                    {
                        if (threadInfo.SN.Contains("DRD") && threadInfo.SN.Length == 19)
                        {
                            client.Send(threadInfo.SN + "\r\n");
                            Log.WriteLog(string.Format("发送大族SN{0}:{1}", threadInfo.SelectedIndex, threadInfo.SN));
                        }
                        else
                        {
                            Log.WriteLog(string.Format("发送大族的SN{0}格式异常:{1}", threadInfo.SelectedIndex, threadInfo.SN));
                            Global.PLC_Client2.WritePLC_D(result, new short[] { 2 });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("发送SN给大族异常失败！" + ex.ToString().Replace("\n", ""));
                }
            }
        }

        private void GetDataForHans(object obj)//获取UA大族焊接机和普雷斯特参数
        {
            lock (Lock)
            {
                try
                {
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    switch (threadInfo.SelectedIndex)
                    {
                        case 1://
                            if (TCPconnected)
                            {
                                if (threadInfo.SN.Contains("DRD") && threadInfo.SN.Length == 19)
                                {
                                    Global.client1.Send("D1," + threadInfo.SN + "\r\n");//请求获取大族焊接参数
                                    Log.WriteLog(string.Format("请求获取大族1参数SN:{0}", threadInfo.SN));
                                }
                                else
                                {
                                    Log.WriteLog(string.Format("请求获取大族1的SN格式异常:{0}", threadInfo.SN));
                                }
                            }
                            break;
                        case 2://
                            if (TCPconnected)
                            {
                                if (threadInfo.SN.Contains("DRD") && threadInfo.SN.Length == 19)
                                {
                                    Global.client2.Send("D1," + threadInfo.SN + "\r\n");//请求获取大族焊接参数
                                    Log.WriteLog(string.Format("请求获取大族2参数SN:{0}", threadInfo.SN));
                                }
                                else
                                {
                                    Log.WriteLog(string.Format("请求获取大族2的SN格式异常:{0}", threadInfo.SN));
                                }
                            }
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    //Log.WriteLog("发送SN给大族/普雷斯特异常失败！" + ex.ToString().Replace("\n", ""));
                    Log.WriteLog("请求获取大族焊接参数异常失败！" + ex.ToString().Replace("\n", ""));
                }
            }
        }

        private void UploadData_UA1(object obj)//Trace/PDCA上传数据
        {
            lock (LockUA)
            {
                try
                {
                    string Full_SN1 = "";
                    string Fixture_id = "";
                    string tossing_item = "NA";
                    short[] Start_time = new short[6];
                    short[] Weld_start = new short[6];
                    short[] Weld_stop = new short[6];
                    short[] Stop_time = new short[6];
                    string Start_t = string.Empty;
                    string Weld_st = string.Empty;
                    string Weld_sp = string.Empty;
                    string Stop_t = string.Empty;
                    string station = string.Empty;
                    short[] Status = new short[1];
                    short[] HansUA_NGStation = new short[5];
                    OEEData oeedata = new OEEData();
                    TraceMesRequest_ua TraceData = new TraceMesRequest_ua();
                    BAilData bail = new BAilData();
                    TraceData.serials = new SN();
                    TraceData.data = new data();
                    TraceData.data.insight = new Insight();
                    TraceData.data.insight.test_attributes = new Test_attributes();
                    TraceData.data.insight.test_station_attributes = new Test_station_attributes();
                    TraceData.data.insight.uut_attributes = new Uut_attributes();
                    TraceData.data.insight.results = new Result[63];
                    TraceData.data.items = new ExpandoObject();
                    for (int i = 0; i < TraceData.data.insight.results.Length; i++)
                    {
                        TraceData.data.insight.results[i] = new Result();
                    }
                    int ActualCT = 0;//OEE使用
                    short DefectCode = 0;//OEE使用
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    Dictionary<string, TraceMesRequest_ua> Trace = new Dictionary<string, TraceMesRequest_ua>();
                    //if (TraceLogs_ChK.Checked || bailcheckbox.Checked)
                    //{
                    Global.PLC_Client2.WritePLC_D(10301, new short[] { 0 });//Trace上传结果清零
                    Global.PLC_Client2.WritePLC_D(10302, new short[] { 0 });//PDCA上传结果清零
                    try
                    {
                        Full_SN1 = Global.PLC_Client2.ReadPLC_Dstring(10330, 15).Replace(" ", "").Replace("\0", "");
                        if (Fullsn1.Length > 19)
                        {
                            Fullsn1 = Fullsn1.Remove(19);
                        }
                        Fixture_id = Global.PLC_Client2.ReadPLC_Dstring(10310, 15).Trim().Replace("\0", "");
                        Start_time = Global.PLC_Client2.ReadPLC_D(10350, 6);
                        Weld_start = Global.PLC_Client2.ReadPLC_D(10360, 6);
                        Weld_stop = Global.PLC_Client2.ReadPLC_D(10370, 6);
                        Stop_time = Global.PLC_Client2.ReadPLC_D(10380, 6);
                        Status = Global.PLC_Client2.ReadPLC_D(10390, 1);// 焊接总结果
                        ActualCT = Global.PLC_Client2.ReadPLC_DD(10400, 2)[0];//CT时间
                        HansUA_NGStation = Global.PLC_Client2.ReadPLC_D(10391, 5);//中是否有那个小料焊接NG
                        for (int i = 0; i < HansUA_NGStation.Length; i++)
                        {
                            if (HansUA_NGStation[i] != 1)
                            {
                                tossing_item = string.Format("location{0} CCD NG", i + 1);
                                if (tossing_item.Contains("NA"))
                                {
                                    tossing_item = tossing_item.Replace("NA", "");
                                }
                                break;
                            }
                        }
                        for (int t = 0; t < 6; t++)
                        {
                            if (t < 2)
                            {
                                Start_t += Start_time[t].ToString() + "-";
                                Weld_st += Weld_start[t].ToString() + "-";
                                Weld_sp += Weld_stop[t].ToString() + "-";
                                Stop_t += Stop_time[t].ToString() + "-";
                            }
                            else if (t == 2)
                            {
                                Start_t += Start_time[t].ToString() + " ";
                                Weld_st += Weld_start[t].ToString() + " ";
                                Weld_sp += Weld_stop[t].ToString() + " ";
                                Stop_t += Stop_time[t].ToString() + " ";
                            }
                            else if (t > 2 && t < 5)
                            {
                                Start_t += Start_time[t].ToString() + ":";
                                Weld_st += Weld_start[t].ToString() + ":";
                                Weld_sp += Weld_stop[t].ToString() + ":";
                                Stop_t += Stop_time[t].ToString() + ":";
                            }
                            else if (t == 5)
                            {
                                Start_t += Start_time[t].ToString();
                                Weld_st += Weld_start[t].ToString();
                                Weld_sp += Weld_stop[t].ToString();
                                Stop_t += Stop_time[t].ToString();
                            }
                        }
                        string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.FileName + "," + Full_SN1 + "," + Fixture_id + "," + Start_t.ToString() + "," + Weld_st.ToString() + "," + Weld_sp.ToString() + "," + Stop_t.ToString() + "," + Status[0].ToString();
                        Log.WriteCSV(str, System.AppDomain.CurrentDomain.BaseDirectory + string.Format("\\{0}完整数据\\", threadInfo.FileName));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString().Replace("\n", ""));
                    }
                    if (Full_SN1 != "" && Fixture_id != "" && Full_SN1.Contains("DRD"))
                    {
                        if (!Trace_ua.ContainsKey(Full_SN1))
                        {
                            bail.full_sn = Full_SN1;
                            bail.Fixture_id = Fixture_id.ToString();
                            oeedata.SerialNumber = Full_SN1;
                            oeedata.Fixture = Fixture_id;
                            if (ActualCT > (Convert.ToDouble(Global.inidata.productconfig.MaxCT) * 10))
                            {
                                double ct = (Convert.ToDouble(Global.inidata.productconfig.CT) * 10);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }
                            else
                            {
                                double ct = Convert.ToDouble(ActualCT);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }

                            TraceData.serials.band = Full_SN1;
                            TraceData.data.insight.test_attributes.unit_serial_number = Full_SN1.Remove(17);
                            TraceData.data.insight.uut_attributes.band_sn = Full_SN1;
                            TraceData.data.insight.test_station_attributes.fixture_id = Fixture_id;
                            TraceData.data.insight.uut_attributes.fixture_id = Fixture_id;
                            TraceData.data.insight.uut_attributes.tossing_item = tossing_item;
                            TraceData.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", oeedata.ActualCT, "1");
                            bail.tossing_item = tossing_item;
                            bail.auto_send = 1;
                            oeedata.auto_send = 1;
                            if (Start_t == "0-0-0 0:0:0" || Weld_st == "0-0-0 0:0:0" || Weld_sp == "0-0-0 0:0:0" || Stop_t == "0-0-0 0:0:0")
                            {
                                bail.Start_Time = DateTime.Now.AddSeconds(-35);
                                bail.Weld_start_time = DateTime.Now.AddSeconds(-28);
                                bail.Weld_stop_time = DateTime.Now.AddSeconds(-8);
                                bail.Stop_Time = DateTime.Now;
                                oeedata.StartTime = DateTime.Now.AddSeconds(-35);
                                oeedata.EndTime = DateTime.Now;
                                TraceData.data.insight.test_attributes.uut_start = DateTime.Now.AddSeconds(-35).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_start_time = DateTime.Now.AddSeconds(-28).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_stop_time = DateTime.Now.AddSeconds(-8).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                bail.Start_Time = Convert.ToDateTime(Start_t);
                                bail.Weld_start_time = Convert.ToDateTime(Weld_st);
                                bail.Weld_stop_time = Convert.ToDateTime(Weld_sp);
                                bail.Stop_Time = Convert.ToDateTime(Stop_t);
                                oeedata.StartTime = Convert.ToDateTime(Start_t);
                                oeedata.EndTime = Convert.ToDateTime(Stop_t);
                                TraceData.data.insight.test_attributes.uut_start = (Convert.ToDateTime(Start_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(Stop_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_start_time = (Convert.ToDateTime(Weld_st)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_stop_time = (Convert.ToDateTime(Weld_sp)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (Status[0] == 1)
                            {
                                bail.test_fail = true;
                                bail.test_result = "PASS";
                                bail.rework_item = "0";
                                bail.rework_label = "N";
                                TraceData.data.insight.test_attributes.test_result = "pass";
                                TraceData.data.insight.uut_attributes.rework_item = "0";
                                TraceData.data.insight.uut_attributes.rework_label = "N";
                                oeedata.Status = "OK";
                                oeedata.ScanCount = "1";
                            }
                            else
                            {
                                bail.test_fail = false;
                                bail.test_result = "FAIL";
                                bail.rework_item = "miss_welding";
                                bail.rework_label = "Y";
                                TraceData.data.insight.test_attributes.test_result = "fail";
                                TraceData.data.insight.uut_attributes.rework_item = "miss_welding";
                                TraceData.data.insight.uut_attributes.rework_label = "Y";
                                oeedata.Status = "NG";
                                oeedata.ScanCount = "1";
                                //_homefrm.AddList(string.Format("{0}[治具码：{1},NG]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Fixture_id), "list_FixtureNG");
                                string InsertStr = "insert into FixtureNG([DateTime],[Fixture])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'" + "," + "'" + Fixture_id + "'" + ")";
                                SQL.ExecuteUpdate(InsertStr);

                            }
                            Out_Trace_ua.Add(Full_SN1);
                            Trace_ua.Add(Full_SN1, TraceData);
                            TraceData = null;
                            Out_ua.Add(Full_SN1);
                            bail_ua.Add(Full_SN1, bail);
                            bail = null;
                            Out_oee.Add(Full_SN1);
                            OEE.Add(Full_SN1, oeedata);
                            oeedata = null;
                        }
                        else
                        {
                            Log.WriteLog("PLC重复触发UA,不上传！" + Full_SN1);
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_TraceMsg");
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_PDCAMsg");
                            Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                            Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                        }
                    }
                    else
                    {
                        Log.WriteLog("产品二维码或载具二维码格式不正确" + "1#焊机" + Full_SN1 + "," + Fixture_id);
                        Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传NG
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("获取参数异常失败:" + ex.ToString().Replace("\n", ""));
                    Global.PLC_Client2.WritePLC_D(10301, new short[] { 2 });
                    Global.PLC_Client2.WritePLC_D(10302, new short[] { 2 });
                    Global.PLC_Client2.WritePLC_D(10303, new short[] { 2 });//图片上传NG
                }
            }
        }

        private void UploadData_UA2(object obj)//Trace/PDCA上传数据
        {
            lock (LockUA)
            {
                try
                {
                    string Full_SN1 = "";
                    string Fixture_id = "";
                    string tossing_item = "NA";
                    short[] Start_time = new short[6];
                    short[] Weld_start = new short[6];
                    short[] Weld_stop = new short[6];
                    short[] Stop_time = new short[6];
                    string Start_t = string.Empty;
                    string Weld_st = string.Empty;
                    string Weld_sp = string.Empty;
                    string Stop_t = string.Empty;
                    string station = string.Empty;
                    short[] Status = new short[1];
                    short[] HansUA_NGStation = new short[5];
                    OEEData oeedata = new OEEData();
                    TraceMesRequest_ua TraceData = new TraceMesRequest_ua();
                    BAilData bail = new BAilData();
                    TraceData.serials = new SN();
                    TraceData.data = new data();
                    TraceData.data.insight = new Insight();
                    TraceData.data.insight.test_attributes = new Test_attributes();
                    TraceData.data.insight.test_station_attributes = new Test_station_attributes();
                    TraceData.data.insight.uut_attributes = new Uut_attributes();
                    TraceData.data.insight.results = new Result[63];
                    TraceData.data.items = new ExpandoObject();
                    for (int i = 0; i < TraceData.data.insight.results.Length; i++)
                    {
                        TraceData.data.insight.results[i] = new Result();
                    }
                    Int32 ActualCT = 0;//OEE使用
                    short DefectCode = 0;//OEE使用
                    ThreadInfo threadInfo = obj as ThreadInfo;
                    Dictionary<string, TraceMesRequest_ua> Trace = new Dictionary<string, TraceMesRequest_ua>();
                    //if (TraceLogs_ChK.Checked || bailcheckbox.Checked)
                    //{
                    Global.PLC_Client2.WritePLC_D(10501, new short[] { 0 });//Trace上传结果清零
                    Global.PLC_Client2.WritePLC_D(10502, new short[] { 0 });//PDCA上传结果清零
                    try
                    {
                        Full_SN1 = Global.PLC_Client2.ReadPLC_Dstring(10530, 15).Replace(" ", "").Replace("\0", "");
                        if (Fullsn1.Length > 19)
                        {
                            Fullsn1 = Fullsn1.Remove(19);
                        }
                        Fixture_id = Global.PLC_Client2.ReadPLC_Dstring(10510, 15).Trim().Replace("\0", "");
                        Start_time = Global.PLC_Client2.ReadPLC_D(10550, 6);
                        Weld_start = Global.PLC_Client2.ReadPLC_D(10560, 6);
                        Weld_stop = Global.PLC_Client2.ReadPLC_D(10570, 6);
                        Stop_time = Global.PLC_Client2.ReadPLC_D(10580, 6);
                        Status = Global.PLC_Client2.ReadPLC_D(10590, 1);// 焊接总结果
                        ActualCT = Global.PLC_Client2.ReadPLC_DD(10600, 2)[0];//CT时间
                        HansUA_NGStation = Global.PLC_Client2.ReadPLC_D(10591, 5);//中是否有那个小料焊接NG
                        for (int i = 0; i < HansUA_NGStation.Length; i++)
                        {
                            if (HansUA_NGStation[i] != 1)
                            {
                                tossing_item = string.Format("location{0} CCD NG", i + 1);
                                if (tossing_item.Contains("NA"))
                                {
                                    tossing_item = tossing_item.Replace("NA", "");
                                }
                                break;
                            }
                        }
                        for (int t = 0; t < 6; t++)
                        {
                            if (t < 2)
                            {
                                Start_t += Start_time[t].ToString() + "-";
                                Weld_st += Weld_start[t].ToString() + "-";
                                Weld_sp += Weld_stop[t].ToString() + "-";
                                Stop_t += Stop_time[t].ToString() + "-";
                            }
                            else if (t == 2)
                            {
                                Start_t += Start_time[t].ToString() + " ";
                                Weld_st += Weld_start[t].ToString() + " ";
                                Weld_sp += Weld_stop[t].ToString() + " ";
                                Stop_t += Stop_time[t].ToString() + " ";
                            }
                            else if (t > 2 && t < 5)
                            {
                                Start_t += Start_time[t].ToString() + ":";
                                Weld_st += Weld_start[t].ToString() + ":";
                                Weld_sp += Weld_stop[t].ToString() + ":";
                                Stop_t += Stop_time[t].ToString() + ":";
                            }
                            else if (t == 5)
                            {
                                Start_t += Start_time[t].ToString();
                                Weld_st += Weld_start[t].ToString();
                                Weld_sp += Weld_stop[t].ToString();
                                Stop_t += Stop_time[t].ToString();
                            }
                        }
                        string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.FileName + "," + Full_SN1 + "," + Fixture_id + "," + Start_t.ToString() + "," + Weld_st.ToString() + "," + Weld_sp.ToString() + "," + Stop_t.ToString() + "," + Status[0].ToString();
                        Log.WriteCSV(str, System.AppDomain.CurrentDomain.BaseDirectory + string.Format("\\{0}完整数据\\", threadInfo.FileName));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString().Replace("\n", ""));
                    }
                    if (Full_SN1 != "" && Fixture_id != "" && Full_SN1.Contains("DRD"))
                    {
                        if (!Trace_ua.ContainsKey(Full_SN1))
                        {
                            bail.full_sn = Full_SN1;
                            bail.Fixture_id = Fixture_id.ToString();
                            oeedata.SerialNumber = Full_SN1;
                            oeedata.Fixture = Fixture_id;
                            if (ActualCT > (Convert.ToDouble(Global.inidata.productconfig.MaxCT) * 10))
                            {
                                double ct = (Convert.ToDouble(Global.inidata.productconfig.CT) * 10);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }
                            else
                            {
                                double ct = Convert.ToDouble(ActualCT);
                                oeedata.ActualCT = (ct / 10).ToString("0.0");
                            }

                            TraceData.serials.band = Full_SN1;
                            TraceData.data.insight.test_attributes.unit_serial_number = Full_SN1.Remove(17);
                            TraceData.data.insight.uut_attributes.band_sn = Full_SN1;
                            TraceData.data.insight.test_station_attributes.fixture_id = Fixture_id;
                            TraceData.data.insight.uut_attributes.fixture_id = Fixture_id;
                            TraceData.data.insight.uut_attributes.tossing_item = tossing_item;
                            TraceData.data.insight.uut_attributes.STATION_STRING = string.Format("{{\"ActualCT \":\"{0}\",\"ScanCount \":\"{1}\"}}", oeedata.ActualCT, "1");
                            bail.tossing_item = tossing_item;
                            bail.auto_send = 1;
                            oeedata.auto_send = 1;
                            if (Start_t == "0-0-0 0:0:0" || Weld_st == "0-0-0 0:0:0" || Weld_sp == "0-0-0 0:0:0" || Stop_t == "0-0-0 0:0:0")
                            {
                                bail.Start_Time = DateTime.Now.AddSeconds(-35);
                                bail.Weld_start_time = DateTime.Now.AddSeconds(-28);
                                bail.Weld_stop_time = DateTime.Now.AddSeconds(-8);
                                bail.Stop_Time = DateTime.Now;
                                oeedata.StartTime = DateTime.Now.AddSeconds(-35);
                                oeedata.EndTime = DateTime.Now;
                                TraceData.data.insight.test_attributes.uut_start = DateTime.Now.AddSeconds(-35).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_start_time = DateTime.Now.AddSeconds(-28).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_stop_time = DateTime.Now.AddSeconds(-8).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                bail.Start_Time = Convert.ToDateTime(Start_t);
                                bail.Weld_start_time = Convert.ToDateTime(Weld_st);
                                bail.Weld_stop_time = Convert.ToDateTime(Weld_sp);
                                bail.Stop_Time = Convert.ToDateTime(Stop_t);
                                oeedata.StartTime = Convert.ToDateTime(Start_t);
                                oeedata.EndTime = Convert.ToDateTime(Stop_t);
                                TraceData.data.insight.test_attributes.uut_start = (Convert.ToDateTime(Start_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(Stop_t)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_start_time = (Convert.ToDateTime(Weld_st)).ToString("yyyy-MM-dd HH:mm:ss");
                                TraceData.data.insight.uut_attributes.weld_stop_time = (Convert.ToDateTime(Weld_sp)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (Status[0] == 1)
                            {
                                bail.test_fail = true;
                                bail.test_result = "PASS";
                                bail.rework_item = "0";
                                bail.rework_label = "N";
                                TraceData.data.insight.test_attributes.test_result = "pass";
                                TraceData.data.insight.uut_attributes.rework_item = "0";
                                TraceData.data.insight.uut_attributes.rework_label = "N";
                                oeedata.Status = "OK";
                                oeedata.ScanCount = "1";
                            }
                            else
                            {
                                bail.test_fail = false;
                                bail.test_result = "FAIL";
                                bail.rework_item = "miss_welding";
                                bail.rework_label = "Y";
                                TraceData.data.insight.test_attributes.test_result = "fail";
                                TraceData.data.insight.uut_attributes.rework_item = "miss_welding";
                                TraceData.data.insight.uut_attributes.rework_label = "Y";
                                oeedata.Status = "NG";
                                oeedata.ScanCount = "1";
                                //_homefrm.AddList(string.Format("{0}[治具码：{1},NG]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Fixture_id), "list_FixtureNG");
                                string InsertStr = "insert into FixtureNG([DateTime],[Fixture])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'" + "," + "'" + Fixture_id + "'" + ")";
                                SQL.ExecuteUpdate(InsertStr);

                            }
                            Out_Trace_ua2.Add(Full_SN1);
                            Trace_ua2.Add(Full_SN1, TraceData);
                            TraceData = null;
                            Out_ua_NG.Add(Full_SN1);
                            bail_ua_NG.Add(Full_SN1, bail);
                            bail = null;
                            Out_oee_NG.Add(Full_SN1);
                            OEE_NG.Add(Full_SN1, oeedata);
                            oeedata = null;
                        }
                        else
                        {
                            Log.WriteLog("PLC重复触发UA,不上传！" + Full_SN1);
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_TraceMsg");
                            _homefrm.AppendRichText("PLC重复触发UA,不上传！" + Full_SN1, "rtx_PDCAMsg");
                            Global.PLC_Client2.WritePLC_D(10501, new short[] { 2 });
                            Global.PLC_Client2.WritePLC_D(10502, new short[] { 2 });
                        }
                    }
                    else
                    {
                        Log.WriteLog("产品二维码或载具二维码格式不正确" + "2#焊机" + Full_SN1 + "," + Fixture_id);
                        Global.PLC_Client2.WritePLC_D(10501, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(10502, new short[] { 2 });
                        Global.PLC_Client2.WritePLC_D(10503, new short[] { 2 });//图片上传NG
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("获取参数异常失败:" + ex.ToString().Replace("\n", ""));
                    Global.PLC_Client2.WritePLC_D(10501, new short[] { 2 });
                    Global.PLC_Client2.WritePLC_D(10502, new short[] { 2 });
                    Global.PLC_Client2.WritePLC_D(10503, new short[] { 2 });//图片上传NG
                }
            }
        }

        private void CheckFixtureID(object obj)//校验治具是否逾期保养
        {
            ThreadInfo threadInfo = (ThreadInfo)obj;
            try
            {
                bool IfTimeOut = Txt.FixtureTimeOut(threadInfo.FixtureID);
                bool IfIQCContains = Txt.IQCFixtrue(threadInfo.FixtureID);
                if (IfTimeOut && IfIQCContains)
                {
                    Global.oee_fixture_ok++;
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 1 });//该治具已定时保养
                    Log.WriteLog(threadInfo.FixtureID + "治具正常生产!");
                }
                else if (!IfIQCContains)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具不在IQC治具列表中
                    _homefrm.AddList(threadInfo.FixtureID, "list_IQCFixtureNG");
                    Log.WriteLog(threadInfo.FixtureID + "治具未录入IQC系统！");
                }
                else if (!IfTimeOut)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具逾期保养
                    _homefrm.AddList(threadInfo.FixtureID, "list_FixtureMsgNG");
                    Log.WriteLog(threadInfo.FixtureID + "治具逾期保养！");
                }
                else if (Global.FixtureOutID == threadInfo.FixtureID)
                {
                    Global.PLC_Client.WritePLC_D(10132, new short[] { 2 });//该治具手动排出
                    Log.WriteLog(threadInfo.FixtureID + "手动排出！");
                    Global.FixtureOutID = string.Empty;
                }

                //if (!Global._fixture_ng.Contains(threadInfo.FixtureID))//判断是否在小保养名单内
                //{
                //    Global.PLC_Client.WritePLC_D(12612, new short[] { 1 });//该治具OK
                //}
                //else
                //{
                //    Global.PLC_Client.WritePLC_D(12612, new short[] { 2 });//该治具待保养
                //}

                //if (!Global._fixture_tossing_ng.Contains(threadInfo.FixtureID))//判断是否在维修治具名单内
                //{
                //    Global.PLC_Client.WritePLC_D(12613, new short[] { 1 });//该治具OK
                //}
                //else
                //{
                //    Global.PLC_Client.WritePLC_D(12613, new short[] { 2 });//该治具待维修
                //}

                if (threadInfo.FixtureID.Length >= 15)//对治具号进行检查
                {
                    if (Global._fixture.Contains(threadInfo.FixtureID))//判断治具是否存在数据库中，如果存在更新使用时间和次数，不存在则录入数据库
                    {
                        //string insertStr = string.Format("UPDATE [FixtureStatus] SET [CountDown] = [CountDown]- datediff(SS,Time,GETDATE()) / 60.0 ,[UsingTimes] = [UsingTimes] - 1 where [FixtureID] = {0}", threadInfo.FixtureID);
                        //int r = SQL.ExecuteUpdate(insertStr);
                        string SelectStr = "SELECT * FROM FixtureStatus";//sql查询语句
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        for (int i = 0; i < d1.Rows.Count; i++)
                        {
                            if (d1.Rows[i][1].ToString() == threadInfo.FixtureID)
                            {
                                int times = Convert.ToInt16(d1.Rows[i][3]) + 1;
                                double countDowm = Global.Fixture_maintance_time - Convert.ToDouble((DateTime.Now - Convert.ToDateTime(d1.Rows[i][2])).TotalMinutes);
                                if (times > Global.Fixture_maintance_times || countDowm <= 0)//治具待保养
                                {
                                    string insertStr1 = string.Format("UPDATE [FixtureStatus] SET [CountDown] = {0} ,[UsingTimes] = [UsingTimes] + 1 ,[Status] = '待保养' where [FixtureID] = '{1}'", countDowm.ToString("0.00"), threadInfo.FixtureID);
                                    int r1 = SQL.ExecuteUpdate(insertStr1);
                                    if (!Global._fixture_ng.Contains(threadInfo.FixtureID))
                                    {
                                        Global._fixture_ng.Add(threadInfo.FixtureID);//待保养治具写入list中
                                    }
                                    Txt.WriteLine2(Global._fixture_ng);//待保养治具写入TXT中
                                    Log.WriteLog("治具自动排出" + threadInfo.FixtureID + "使用次数为" + times + "使用倒计时为" + countDowm);
                                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + threadInfo.FixtureID + "," + "治具自动排出", @"D:\ZHH\治具小保养记录\");
                                }
                                else
                                {
                                    string insertStr2 = string.Format("UPDATE [FixtureStatus] SET [CountDown] = {0} ,[UsingTimes] = [UsingTimes] + 1 where [FixtureID] = '{1}'", countDowm.ToString("0.00"), threadInfo.FixtureID);
                                    int r2 = SQL.ExecuteUpdate(insertStr2);
                                }
                            }
                        }
                    }
                    else
                    {
                        Global._fixture.Add(threadInfo.FixtureID);
                        string insertStr = string.Format("insert into FixtureStatus([FixtureID],[Time],[UsingTimes],[CountDown],[Status]) values('{0}','{1}','{2}','{3}','{4}')",
                          threadInfo.FixtureID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0", Global.Fixture_maintance_time, "正常使用中");
                        int r = SQL.ExecuteUpdate(insertStr);
                        string insertStr1 = string.Format("insert into FixtureTossing([Fixture],[TossingTime],[TossingContinuation],[TossingCount],[ContinuationNG],[CountNG]) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                          threadInfo.FixtureID, "", "0", "0", "OK", "OK");
                        int r1 = SQL.ExecuteUpdate(insertStr1);//插入抛料治具表
                    }
                    //_homefrm.UpdateDataGridView();//更新DataGridView显示
                }

                //_homefrm.AddList(threadInfo.FixtureID, "list_AllFixture");//把治具导入列表中

            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString().Replace("\r\n", ""));
            }
        }

        private void OEEMateriel(object obj)//OEE抛小料
        {
            lock (Lock1)
            {
                ThreadInfo threadInfo = obj as ThreadInfo;
                switch (threadInfo.SelectedIndex)
                {
                    case 1:
                        Global.NutCount = Global.NutCount + 1;
                        break;
                    case 2:
                        Global.NutOKCount = Global.NutOKCount + 1;
                        break;
                    default:
                        break;
                }
                if (threadInfo.SelectedIndex == 1)
                {
                    Global.ThrowCount = Global.NutCount;
                    _homefrm.UpDatalabel(Global.NutCount.ToString(), "lb_Materiel_Nut");
                    _homefrm.UpDatalabel(Global.ThrowCount.ToString(), "lb_Materiel_AllNut");
                    Log.WriteLog("小料抛料：" + Global.NutCount + "个" + " , " + "一天总计抛料：" + Global.TotalThrowCount + "个");
                }
                else
                {
                    Global.ThrowOKCount = Global.NutOKCount;
                    _homefrm.UpDatalabel(Global.NutOKCount.ToString(), "lb_Materiel_OK");
                    _homefrm.UpDatalabel(Global.ThrowOKCount.ToString(), "lb_Materiel_AllOK");
                    Log.WriteLog("小料OK：" + Global.NutOKCount + "个");
                }
            }
        }
        #endregion

        private void WritePLCData(object obj)//记录PLC参数
        {
            ThreadInfo threadInfo = obj as ThreadInfo;
            switch (threadInfo.SelectedIndex)
            {
                case 1:
                    Thread.Sleep(50);
                    Global.oldData = Global.PLC_Client2.ReadPLC_DD(20000, 700);
                    break;
                case 2:
                    Thread.Sleep(50);
                    Global.newData = Global.PLC_Client2.ReadPLC_DD(20000, 700);
                    for (int i = 0; i < Global.newData.Length - 1; i++)
                    {
                        if (Global.oldData[i] != Global.newData[i])
                        {
                            if (Global.PLC_DataName[i * 2 + 1].PLC_Name == "Trace模式更改(正常/调机)")
                            {
                                if (Global.newData[i] == 2)
                                {
                                    Log.WriteLog(Global.Name + "；权限等级：" + Global.Title + "；参数名称：" + Global.PLC_DataName[i * 2 + 1].PLC_Name + "参数修改：" + "正常模式" + "→" + "调机模式");
                                    string InsertStr1 = string.Format("insert into PLCData([用户名称],[权限等级],[修改时间],[参数名称],[原参数],[现参数]) values('{0}','{1}','{2}','{3}','{4}','{5}')", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, "正常模式", "调机模式");
                                    SQL.ExecuteUpdate(InsertStr1);
                                    Log.WriteCSV(String.Format("{0},{1},{2},{3},{4},{5}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, "正常模式", "调机模式"), System.AppDomain.CurrentDomain.BaseDirectory + "\\参数修改记录\\");
                                }
                                else if (Global.newData[i] == 1)
                                {
                                    Log.WriteLog(Global.Name + "；权限等级：" + Global.Title + "；参数名称：" + Global.PLC_DataName[i * 2 + 1].PLC_Name + "参数修改：" + "调机模式" + "→" + "正常模式");
                                    string InsertStr1 = string.Format("insert into PLCData([用户名称],[权限等级],[修改时间],[参数名称],[原参数],[现参数]) values('{0}','{1}','{2}','{3}','{4}','{5}')", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, "调机模式", "正常模式");
                                    SQL.ExecuteUpdate(InsertStr1);
                                    Log.WriteCSV(String.Format("{0},{1},{2},{3},{4},{5}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, "调机模式", "正常模式"), System.AppDomain.CurrentDomain.BaseDirectory + "\\参数修改记录\\");
                                }
                            }
                            else
                            {
                                Log.WriteLog(Global.Name + "；权限等级：" + Global.Title + "；参数名称：" + Global.PLC_DataName[i * 2 + 1].PLC_Name + "参数修改：" + Global.oldData[i] + "→" + Global.newData[i]);
                                string InsertStr = string.Format("insert into PLCData([用户名称],[权限等级],[修改时间],[参数名称],[原参数],[现参数]) values('{0}','{1}','{2}','{3}','{4}','{5}')", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, Global.oldData[i], Global.newData[i]);
                                SQL.ExecuteUpdate(InsertStr);
                                Log.WriteCSV(String.Format("{0},{1},{2},{3},{4},{5}", Global.Name, Global.Title, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Global.PLC_DataName[i * 2 + 1].PLC_Name, Global.oldData[i], Global.newData[i]), System.AppDomain.CurrentDomain.BaseDirectory + "\\参数修改记录\\");

                                //------中控系统上传关键日志-----
                                try
                                {
                                    string RecData = string.Empty;
                                    string errorMsg = string.Empty;
                                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                                    jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                                    //关键日志上传
                                    KeyLog keyLog = new KeyLog();
                                    keyLog.logs = new Logs[1];
                                    keyLog.logs[0] = new Logs();
                                    //只有初始化类数组里面的每一个类，才能给类的成员赋值。
                                    //keyLog[0] = new KeyLog();
                                    keyLog.stationID = Global.inidata.productconfig.Station_id_ua;
                                    keyLog.moduleCode = "1";
                                    keyLog.logs[0].time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    keyLog.logs[0].operatorId = Global.Emp;
                                    keyLog.logs[0].operatorName = Global.Name;
                                    keyLog.logs[0].machineCode = "emt编号";
                                    keyLog.logs[0].action = "param.set";
                                    keyLog.logs[0].data = new Data_KeyLog();
                                    keyLog.logs[0].data.name = Global.PLC_DataName[i * 2 + 1].PLC_Name;
                                    keyLog.logs[0].data.old = Global.oldData[i].ToString();
                                    keyLog.logs[0].data.new1 = Global.newData[i].ToString();
                                    string SendData = JsonConvert.SerializeObject(keyLog, Formatting.None, jsetting);
                                    SendData = SendData.Replace("new1", "new");
                                    Log.WriteLog("中控系统关键日志上传：" + SendData);
                                    CentralControlAPI.RequestPost("http://10.143.20.222:80/openApi/v1/machine/log/operation?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE0NDc0NDAwMDY4MjkzMTQwNDgiLCJzbiI6MTQ0NzQ0NTI0NDkwNjgzNTk2OH0.pnTP-0hcDzjTPA5yg3PQLyd96yfmosSU0dyE-Bpc8sg", SendData, out RecData, out errorMsg);

                                    Rec_KeyLog rec_log = JsonConvert.DeserializeObject<Rec_KeyLog>(RecData);
                                    Log.WriteLog("中控系统关键日志上传结果：" + rec_log.ok);
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteLog("中控系统上传关键日志异常：" + ex.ToString());
                                }
                            }
                        }
                    }
                    break;
            }
        }

        #region 数据缓存重传
        private void Trace_Retry_UA(object ob)
        {
            while (true)
            {
                if (Trace_Logs_flag == true && Global.TraceSendua_ng == 0)
                {
                    try
                    {
                        string insStr = string.Format("select count(*) from TraceUASendNG");
                        DataTable d1 = SQL.ExecuteQuery(insStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("Trace_UA上传失败数据重新上传");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                TraceMesRequest_ua tracedata = new TraceMesRequest_ua();
                                tracedata.serials = new SN();
                                tracedata.data = new data();
                                tracedata.data.insight = new Insight();
                                tracedata.data.insight.test_attributes = new Test_attributes();
                                tracedata.data.insight.test_station_attributes = new Test_station_attributes();
                                tracedata.data.insight.uut_attributes = new Uut_attributes();
                                tracedata.data.insight.results = new Result[26];
                                tracedata.data.items = new ExpandoObject();
                                for (int i = 0; i < tracedata.data.insight.results.Length; i++)
                                {
                                    tracedata.data.insight.results[i] = new Result();
                                }
                                string SelectStr = "select * from TraceUASendNG where ID =(SELECT MIN(ID) FROM TraceUASendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr);
                                if (d2.Rows.Count > 0)
                                {
                                    tracedata.data.insight.test_attributes.test_result = d2.Rows[0][3].ToString();
                                    tracedata.serials.band = d2.Rows[0][2].ToString();
                                    tracedata.data.insight.test_attributes.unit_serial_number = d2.Rows[0][2].ToString().Remove(17);
                                    tracedata.data.insight.uut_attributes.band_sn = d2.Rows[0][2].ToString();
                                    tracedata.data.insight.test_station_attributes.fixture_id = d2.Rows[0][4].ToString();
                                    tracedata.data.insight.uut_attributes.fixture_id = d2.Rows[0][4].ToString();
                                    tracedata.data.insight.uut_attributes.tossing_item = d2.Rows[0][5].ToString();
                                    tracedata.data.insight.uut_attributes.STATION_STRING = d2.Rows[0][6].ToString();
                                    tracedata.data.insight.test_attributes.uut_start = (Convert.ToDateTime(d2.Rows[0][7].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                                    tracedata.data.insight.uut_attributes.weld_start_time = (Convert.ToDateTime(d2.Rows[0][8].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                                    tracedata.data.insight.uut_attributes.weld_stop_time = (Convert.ToDateTime(d2.Rows[0][9].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                                    tracedata.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(d2.Rows[0][10].ToString())).ToString("yyyy-MM-dd HH:mm:ss");

                                    string DeleteStr = "delete from TraceUASendNG where ID = (SELECT MIN(ID) FROM TraceUASendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                }
                                if (!Trace_ua.ContainsKey(d2.Rows[0][2].ToString()))
                                {
                                    Trace_ua.Add(tracedata.serials.band, tracedata);
                                    Out_Trace_ua.Add(tracedata.serials.band);
                                }
                                tracedata = null;
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("Trace_UA数据重传异常" + ex.ToString().Replace("\n", ""));
                    }
                }
                Thread.Sleep(100);
            }
        }

        //private void Trace_Retry_LA(object ob)
        //{
        //    while (true)
        //    {
        //        if (Trace_Logs_flag == true && TraceSendLA_ng == 0)
        //        {
        //            PDCA_Data data = new PDCA_Data();
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "Trace_LA_SendNG") > 0)
        //                {
        //                    Log.WriteLog("Trace_LA上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "Trace_LA_SendNG"); j++)
        //                    {
        //                        TraceMesRequest tracedata = new TraceMesRequest();
        //                        HansData hd = new HansData();
        //                        tracedata.serials = new SN();
        //                        tracedata.data = new data2();
        //                        tracedata.data.insight = new Insight();
        //                        tracedata.data.insight.test_attributes = new Test_attributes();
        //                        tracedata.data.insight.test_station_attributes = new Test_station_attributes();
        //                        tracedata.data.insight.uut_attributes = new Uut_attributes();
        //                        tracedata.data.insight.results = new Result[12];
        //                        tracedata.data.insight.results[0] = new Result();
        //                        tracedata.data.insight.results[1] = new Result();
        //                        tracedata.data.insight.results[2] = new Result();
        //                        tracedata.data.insight.results[3] = new Result();
        //                        tracedata.data.insight.results[4] = new Result();
        //                        tracedata.data.insight.results[5] = new Result();
        //                        tracedata.data.insight.results[6] = new Result();
        //                        tracedata.data.insight.results[7] = new Result();
        //                        tracedata.data.insight.results[8] = new Result();
        //                        tracedata.data.insight.results[9] = new Result();
        //                        tracedata.data.insight.results[10] = new Result();
        //                        tracedata.data.insight.results[11] = new Result();
        //                        Access.QueryLastData("PDCA", "Trace_LA_SendNG", out data);
        //                        if (data.full_sn != null)
        //                        {
        //                            tracedata.data.insight.test_attributes.test_result = data.test_result;
        //                            tracedata.serials.band = data.full_sn;
        //                            tracedata.data.insight.test_attributes.unit_serial_number = data.full_sn.Remove(17);
        //                            tracedata.data.insight.test_station_attributes.fixture_id = data.Fixture_id;
        //                            tracedata.data.insight.test_attributes.uut_start = (Convert.ToDateTime(data.Start_Time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.uut_attributes.la_weld_start_time = (Convert.ToDateTime(data.Weld_start_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.uut_attributes.la_weld_stop_time = (Convert.ToDateTime(data.Weld_stop_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            tracedata.data.insight.test_attributes.uut_stop = (Convert.ToDateTime(data.Stop_time)).ToString("yyyy-MM-dd HH:mm:ss");
        //                            hd.power_ll = data.power_ll;
        //                            hd.power_ul = data.power_ul;
        //                            hd.pattern_type = data.pattern_type;
        //                            hd.frequency = data.frequency;
        //                            hd.linear_speed = data.linear_speed;
        //                            hd.spot_size = data.spot_size;
        //                            hd.pulse_energy = data.pulse_energy;
        //                            hd.power = data.power;
        //                            hd.filling_pattern = data.filling_pattern;
        //                            hd.hatch = data.hatch;
        //                            Access.DeleteData("PDCA", "Trace_LA_SendNG");
        //                        }
        //                        if (!Trace_la.ContainsKey(data.full_sn))
        //                        {
        //                            Trace_la.Add(data.full_sn, tracedata);
        //                            Out_trace_la.Add(data.full_sn);
        //                            HansDatas_la.Add(data.full_sn, hd);
        //                        }
        //                        tracedata = null;
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("Trace_LA数据重传异常");
        //            }
        //        }
        //        //for (int m = 0; m < 10; m++)
        //        //{
        //        //    Thread.Sleep(1000);
        //        //}
        //    }
        //}

        //private void OEE_Default_Retry(object ob)//OEE上传失败重新上传
        //{
        //    while (true)
        //    {
        //        if (OEE_Default_flag == true && oeeSend_ng == 0)
        //        {
        //            OEE_Data data = new OEE_Data();
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "OEEData_SendNG") > 0)
        //                {
        //                    Log.WriteLog("OEEData上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "OEEData_SendNG"); j++)
        //                    {
        //                        OEEData oee_default = new OEEData();
        //                        Access.QueryLastData_oee("PDCA", "OEEData_SendNG", out data);
        //                        if (data.SerialNumber != null)
        //                        {
        //                            oee_default.SerialNumber = data.SerialNumber;
        //                            oee_default.Fixture = data.Fixture;
        //                            oee_default.StartTime = Convert.ToDateTime(data.StartTime);
        //                            oee_default.EndTime = Convert.ToDateTime(data.EndTime);
        //                            oee_default.Status = data.Status;
        //                            oee_default.ActualCT = data.ActualCT;
        //                            oee_default.SwVersion = data.SwVersion;
        //                            oee_default.DefectCode = data.DefectCode;
        //                            Access.DeleteData("PDCA", "OEEData_SendNG");
        //                        }
        //                        if (!OEE.ContainsKey(data.SerialNumber))
        //                        {
        //                            OEE.Add(data.SerialNumber, oee_default);
        //                            Out_oee.Add(data.SerialNumber);
        //                        }
        //                        oee_default = null;
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("OEEData数据重传异常");
        //            }
        //        }
        //    }
        //    //for (int m = 0; m < 10; m++)
        //    //{
        //    //    Thread.Sleep(1000);
        //    //}
        //}

        private void OEE_DownTime_Retry(object ob)//OEE_DT上传失败重新上传
        {
            var IP = GetIp();
            var Mac = GetMac();
            while (true)
            {
                if (Global.ConnectOEEFlag == true)
                {
                    OEE_DownTime data = new OEE_DownTime();
                    string msg = "";
                    try
                    {
                        string SelectStr = string.Format("select count(*) from OEE_DTSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("OEE_DT上传失败数据重新上传" + ",OEELog");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from OEE_DTSendNG where ID =(SELECT MIN(ID) FROM OEE_DTSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                                string OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString());
                                Log.WriteLog("发送缓存OEE_DT:" + OEE_DT + ",OEELog");
                                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

                                ///0909
                                /// 
                                ///Night
                                /// 
                                //string poorNum = string.Empty;
                                //string TotalNum = string.Empty;
                                //if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                                //{
                                //    poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                                //    TotalNum = Global.Product_Total_N.ToString();
                                //}
                                //else
                                //{
                                //    poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                                //    TotalNum = Global.Product_Total_D.ToString();
                                //}
                                //Goee.UploadDowntime(poorNum, TotalNum, d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][6].ToString());
                                /////

                                if (rst)
                                {
                                    _homefrm.AppendRichText(d2.Rows[0][4].ToString() + ",触发时间=" + d2.Rows[0][5].ToString() + ",运行状态:" + d2.Rows[0][3].ToString() + ",故障描述:" + d2.Rows[0][8].ToString() + ",缓存发送成功", "rtx_DownTimeMsg");
                                    //Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + data.ErrorCode + "," + data.EventTime + "," + data.CreateTime + "," + "缓存发送成功" + "," + data.Status + "," + data.errorinfo + "," + data.TimeSpan, @"F:\装机软件\系统配置\System_ini\");
                                    Global.ConnectOEEFlag = true;
                                    Log.WriteLog("OEE_DT自动errorCode缓存发送成功" + ",OEELog");
                                    string DeleteStr = "delete from OEE_DTSendNG where ID = (SELECT MIN(ID) FROM OEE_DTSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                }
                                else
                                {
                                    Global.ConnectOEEFlag = false;
                                    _homefrm.AppendRichText(d2.Rows[0][4].ToString() + ",触发时间=" + d2.Rows[0][5].ToString() + ",运行状态:" + d2.Rows[0][3].ToString() + ",故障描述:" + d2.Rows[0][8].ToString() + ",缓存发送失败", "rtx_DownTimeMsg");
                                    //Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + data.ErrorCode + "," + data.EventTime + "," + data.CreateTime + "," + "缓存发送失败" + "," + data.Status + "," + data.errorinfo + "," + data.TimeSpan, @"F:\装机软件\系统配置\System_ini\");
                                    Log.WriteLog("OEE_DT自动errorCode缓存发送失败" + ",OEELog");
                                }
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE_DT数据缓存重传异常" + ex.ToString().Replace("\n", "") + ",OEELog");
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void OEE_Default_Retry(object ob)//OEE_default上传失败重新上传
        {
            var IP = GetIp();
            var Mac = GetMac();
            while (true)
            {
                if (Global.ConnectOEEFlag == true)
                {
                    string msg = "";
                    try
                    {
                        string SelectStr = string.Format("select count(*) from OEE_DefaultSendNG");
                        DataTable d1 = SQL.ExecuteQuery(SelectStr);
                        if (Convert.ToInt32(d1.Rows[0][0].ToString()) > 0)
                        {
                            Log.WriteLog("OEE_Default上传失败数据重新上传" + ",OEELog");
                            for (int j = 0; j < Convert.ToInt32(d1.Rows[0][0].ToString()); j++)
                            {
                                string SelectStr2 = "select * from OEE_DefaultSendNG where ID =(SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                DataTable d2 = SQL.ExecuteQuery(SelectStr2);
                                string OEE_Default = string.Format("{{\"SerialNumber\":\"{0}\",\"BGBarcode\":\"{1}\",\"Fixture\":\"{2}\",\"StartTime\":\"{3}\",\"EndTime\":\"{4}\",\"Status\":\"{5}\",\"ActualCT\":\"{6}\",\"SwVersion\":\"{7}\",\"ScanCount\":\"{8}\"}}", string.IsNullOrEmpty(d2.Rows[0][2].ToString()) ? DateTime.Parse(d2.Rows[0][4].ToString()).ToString("yyyyMMddHHmmss") : d2.Rows[0][2].ToString(), "", d2.Rows[0][3].ToString(), d2.Rows[0][4].ToString(), d2.Rows[0][5].ToString(), d2.Rows[0][6].ToString(), d2.Rows[0][7].ToString(), d2.Rows[0][8].ToString(), d2.Rows[0][9].ToString());
                                Log.WriteLog("发送缓存OEE_Default:" + OEE_Default + ",OEELog");
                                var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 1, OEE_Default, out msg);

                                ////20210909
                                //Goee.UploadOEE(new OeeUpload()
                                //{
                                //    SerialNumber =string.IsNullOrEmpty(d2.Rows[0][2].ToString())? DateTime.Parse(d2.Rows[0][4].ToString()).ToString("yyyyMMddHHmmss"): d2.Rows[0][2].ToString(),
                                //    BGBarcode = "",
                                //    Fixture = d2.Rows[0][3].ToString(),
                                //    StartTime = DateTime.Parse(d2.Rows[0][4].ToString()),
                                //    EndTime = DateTime.Parse(d2.Rows[0][5].ToString()),
                                //    Status = d2.Rows[0][6].ToString(),
                                //    ActualCT = d2.Rows[0][7].ToString(),
                                //    SwVersion = d2.Rows[0][8].ToString(),
                                //    ScanCount = 1,
                                //    Cavity = "1",
                                //    ErrorCode = ""
                                //});

                                if (rst)
                                {
                                    Global.oee_ok++;
                                    _homefrm.UpDatalabel(Global.oee_ok.ToString(), "lb_OEEOK");
                                    Global.oeeSend_ng = 0;
                                    OEE_Default_flag = true;
                                    _homefrm.AppendRichText(msg, "rtx_OEEDefaultMsg");
                                    _homefrm.UpDatalabelcolor(Color.Green, "OEE_Default发送成功", "lb_OEE_UA_SendStatus");
                                    Global.ConnectOEEFlag = true;
                                    Global.PLC_Client.WritePLC_D(26104, new short[] { 1 });
                                    Log.WriteLog("OEE_Default自动缓存发送成功" + msg + ",OEELog");
                                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "OEE_Default" + "," + d2.Rows[0][2].ToString() + "," + d2.Rows[0][3].ToString() + "," + d2.Rows[0][4].ToString() + "," + d2.Rows[0][5].ToString() + "," + d2.Rows[0][6].ToString() + "," + d2.Rows[0][7].ToString() + "," + d2.Rows[0][8].ToString() + "," + "1" + "," + "OK-OEE_Default", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_Default数据\\");
                                    _homefrm.UiText(d2.Rows[0][2].ToString(), "txtOEE_SerialNumber");
                                    _homefrm.UiText(d2.Rows[0][3].ToString(), "txtOEE_Fixture");
                                    _homefrm.UiText(d2.Rows[0][4].ToString(), "txtOEE_StartTime");
                                    _homefrm.UiText(d2.Rows[0][5].ToString(), "txtOEE_EndTime");
                                    _homefrm.UiText(d2.Rows[0][6].ToString(), "txtOEE_Status");
                                    _homefrm.UiText(d2.Rows[0][7].ToString(), "txtOEE_ActualCT");
                                    _homefrm.UiText(d2.Rows[0][8].ToString(), "txtOEE_sw");
                                    _homefrm.UiText(d2.Rows[0][9].ToString(), "txtOEE_ScanCount");
                                    Log.WriteLog("OEE_Default_UI更新成功" + ",OEELog");
                                    string DeleteStr = "delete from OEE_DefaultSendNG where ID = (SELECT MIN(ID) FROM OEE_DefaultSendNG)";
                                    SQL.ExecuteUpdate(DeleteStr);
                                }
                                else
                                {
                                    Global.ConnectOEEFlag = false;
                                    _homefrm.AppendRichText("缓存发送失败" + msg, "rtx_OEEDefaultMsg");
                                    Log.WriteLog("OEE_Default自动缓存发送失败" + msg + ",OEELog");
                                }
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("OEE_Default数据缓存重传异常" + ex.ToString().Replace("\n", "") + ",OEELog");
                    }
                }
                Thread.Sleep(100);
            }
        }

        //private void OEE_Materiel_Retry(object ob)//OEE 小料抛料计数上传失败重新上传
        //{
        //    var IP = GetIp();
        //    var Mac = GetMac();
        //    while (true)
        //    {
        //        if (ConnectOEEFlag == true)
        //        {
        //            OEE_MaterielData data = new OEE_MaterielData();
        //            string msg = "";
        //            try
        //            {
        //                if (Access.QueryNumber("PDCA", "OEE_MaterielData") > 0)
        //                {
        //                    Log.WriteLog("OEE_MaterielData上传失败数据重新上传");
        //                    for (int j = 0; j < Access.QueryNumber("PDCA", "OEE_MaterielData"); j++)
        //                    {
        //                        Access.QueryLastData_OEE_Materiel("PDCA", "OEE_MaterielData", out data);
        //                        string parttype = data.uacount.ToString() + "," + data.lacount.ToString();
        //                        string OEE_MaterielData = string.Format("{{\"date\":\"{0}\",\"count\":\"{1}\",\"totalcount\":\"{2}\",\"parttype\":\"{3}\"}}", data.date, data.count, data.totalcount, parttype);
        //                        Log.WriteLog("OEE_MaterielData缓存:" + OEE_MaterielData);
        //                        AppendText(listBox8, OEE_MaterielData);
        //                        var rst = RequestAPI.Request(inidata.productconfig.URL1, inidata.productconfig.URL2, IP, Mac, inidata.productconfig.Dsn, inidata.productconfig.authCode, 6, OEE_MaterielData, out msg);
        //                        if (rst)
        //                        {
        //                            AppendText(listBox8, msg);
        //                            ConnectOEEFlag = true;
        //                            Log.WriteLog("OEE_MaterielData自动缓存上传OK:" + msg);
        //                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + parttype + "," + "OK-OEE_MaterielData缓存", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
        //                            Access.DeleteData("PDCA", "OEE_MaterielData");
        //                        }
        //                        else
        //                        {
        //                            AppendText(listBox8, msg);
        //                            ConnectOEEFlag = false;
        //                            Log.WriteLog("OEE_MaterielData自动缓存上传NG:" + msg);
        //                            Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + data.count + "," + data.totalcount + "," + parttype + "," + "NG-OEE_MaterielData缓存", System.AppDomain.CurrentDomain.BaseDirectory + "\\OEE_小料抛料计数数据\\");
        //                        }
        //                        Thread.Sleep(1000);
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                Log.WriteLog("OEE_MaterielData数据缓存重传异常");
        //            }
        //        }
        //        Thread.Sleep(20);
        //    }
        //}
        #endregion

        #region 断线重连
        private void PLC_autolink(object ob)
        {
            while (true)
            {
                if (Link_PLC == true && (Global.PLC_Client.client == null || !Global.PLC_Client.IsConnected))
                {
                    //连接PLC
                    try
                    {
                        Global.PLC_Client.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port);
                        Global.PLC_Client.Connect();
                        Log.WriteLog("PLC通信已建立");
                        isopen = true;
                    }
                    catch
                    {
                        Log.WriteLog("PLC通信无法连接");
                        Environment.Exit(1);
                    }
                }
                //if (Link_PLC == true && (Global.PLC_Client2.client == null || !Global.PLC_Client2.IsConnected))
                //{
                //    //连接PLC
                //    try
                //    {
                //        Global.PLC_Client2.sClient(Global.inidata.productconfig.Plc_IP, Global.inidata.productconfig.Plc_Port2);
                //        Global.PLC_Client2.Connect();
                //        Log.WriteLog("PLC2通信已建立");
                //        isopen = true;
                //    }
                //    catch
                //    {
                //        Log.WriteLog("PLC2通信无法连接");
                //        Environment.Exit(1);
                //    }
                //}
                Thread.Sleep(100);
            }
        }
        #endregion

        #region Timer定时方法
        private void timer1_Tick(object sender, EventArgs e)
        {
            tsslabelcolor(tsslbl_time, Color.Black, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //switch (Global.Login)
            //{
            //    case Global.LoginLevel.Operator:
            //        tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：操作员");
            //        Btn_IfEnable(btn_Manual, false);
            //        Btn_IfEnable(btn_Setting, false);
            //        break;
            //    case Global.LoginLevel.Technician:
            //        tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：技术员");
            //        Btn_IfEnable(btn_Manual, true);
            //        Btn_IfEnable(btn_Setting, false);
            //        break;
            //    case Global.LoginLevel.Administrator:
            //        tsslabelcolor(tsslbl_UserLogin, Color.Black, "当前用户：工程师");
            //        Btn_IfEnable(btn_Manual, true);
            //        Btn_IfEnable(btn_Setting, true);
            //        break;
            //}
            //SetText(Global.ErrorIndex);
            //Global.ErrorIndex++;
        }

        private void InitTimer()//初始化定时器
        {
            int interval = 1000;
            Global.timer = new System.Timers.Timer(interval);
            Global.timer.AutoReset = true;
            Global.timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
        }

        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)//定时执行事件-首件模式持续时间过久提示！
        {
            try
            {
                Global.currentCount += 1;
                //this.Invoke(new SetControlValue(SetTextBoxText), Global.currentCount.ToString());
                if (Global.currentCount == 900)//15分钟
                {
                    Global.timer.Stop();
                    Global.currentCount = 0;
                    MessageBox.Show("首件开始已持续15分钟！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行定时到点事件失败：" + ex.Message);
            }
        }
        #endregion

        #region 手动按钮
        private void btn_home_Click(object sender, EventArgs e)
        {
            btn_home.Image = Global.ReadImageFile(LogPath + "home1" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_homefrm.Name))
            {
                this._homefrm.MdiParent = this;
                this._homefrm.Dock = DockStyle.Fill;
                this._homefrm.Show();
            }
            else
            {
                ShowView();
                this._homefrm.Activate();
            }
            //Cursor.Current = Cursors.Arrow;

        }

        private void btn_DataStatistics_Click(object sender, EventArgs e)
        {
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu1" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_datastatisticsfrm.Name))
            {
                this._datastatisticsfrm.MdiParent = this;
                this._datastatisticsfrm.Dock = DockStyle.Fill;
                this._datastatisticsfrm.Show();
            }
            else
            {
                _datastatisticsfrm.Show();
                this._datastatisticsfrm.Activate();
            }
        }

        private void btn_IOMonitor_Click(object sender, EventArgs e)
        {
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor1" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_iomonitorfrm.Name))
            {
                this._iomonitorfrm.MdiParent = this;
                this._iomonitorfrm.Dock = DockStyle.Fill;
                this._iomonitorfrm.Show();
            }
            else
            {
                _iomonitorfrm.Show();
                this._iomonitorfrm.Activate();
            }
        }

        private void btn_Manual_Click(object sender, EventArgs e)
        {
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man1" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_manualfrm.Name))
            {
                this._manualfrm.MdiParent = this;
                this._manualfrm.Dock = DockStyle.Fill;
                this._manualfrm.Show();
            }
            else
            {
                _manualfrm.Show();
                this._manualfrm.Activate();
            }
        }

        private void btn_Setting_Click(object sender, EventArgs e)
        {
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set1" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_sttingfrm.Name))
            {
                this._sttingfrm.MdiParent = this;
                this._sttingfrm.Dock = DockStyle.Fill;
                this._sttingfrm.Show();
            }
            else
            {
                _sttingfrm.Show();
                this._sttingfrm.Activate();
            }
        }

        private void btn_Abnormal_Click(object sender, EventArgs e)
        {
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm1" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_Abnormalfrm.Name))
            {
                this._Abnormalfrm.MdiParent = this;
                this._Abnormalfrm.Dock = DockStyle.Fill;
                this._Abnormalfrm.Show();
            }
            else
            {
                _Abnormalfrm.Show();
                this._Abnormalfrm.Activate();
            }
        }

        private void btn_UserLogin_Click(object sender, EventArgs e)
        {
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user1" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_userloginfrm.Name))
            {
                this._userloginfrm.MdiParent = this;
                this._userloginfrm.Dock = DockStyle.Fill;
                this._userloginfrm.Show();
            }
            else
            {
                _userloginfrm.Show();
                this._userloginfrm.Activate();
            }
        }

        private void btn_Help_Click(object sender, EventArgs e)
        {
            btn_Help.Image = Global.ReadImageFile(LogPath + "help1" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            label_MachineName.BackColor = Color.White;
            if (!ExistsMdiChildrenInstance(_helpfrm.Name))
            {
                this._helpfrm.MdiParent = this;
                this._helpfrm.Dock = DockStyle.Fill;
                this._helpfrm.Show();
            }
            else
            {
                _helpfrm.Show();
                this._helpfrm.Activate();
            }
        }

        private void label_MachineName_Click(object sender, EventArgs e)
        {
            label_MachineName.BackColor = Color.DarkSeaGreen;
            btn_Help.Image = Global.ReadImageFile(LogPath + "help" + ".bmp");
            btn_UserLogin.Image = Global.ReadImageFile(LogPath + "user" + ".bmp");
            btn_Abnormal.Image = Global.ReadImageFile(LogPath + "alarm" + ".bmp");
            btn_Setting.Image = Global.ReadImageFile(LogPath + "set" + ".bmp");
            btn_Manual.Image = Global.ReadImageFile(LogPath + "man" + ".bmp");
            btn_IOMonitor.Image = Global.ReadImageFile(LogPath + "monitor" + ".bmp");
            btn_DataStatistics.Image = Global.ReadImageFile(LogPath + "tu" + ".bmp");
            btn_home.Image = Global.ReadImageFile(LogPath + "home" + ".bmp");
            if (!ExistsMdiChildrenInstance(_machinefrm.Name))
            {
                this._machinefrm.MdiParent = this;
                this._machinefrm.Dock = DockStyle.Fill;
                this._machinefrm.Show();
            }
            else
            {
                _machinefrm.Show();
                this._machinefrm.Activate();
            }
        }

        public void ButtonFlag(bool Flag, Button bt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new buttonflag(ButtonFlag), new object[] { Flag, bt });
                return;
            }
            bt.Enabled = Flag;
        }
        #endregion

        #region 委托显示
        public void Btn_IfEnable(ToolStripButton btn, bool b)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new btnEnable(Btn_IfEnable), new object[] { btn, b });
                return;
            }
            btn.Enabled = b;
        }

        public void tsslabelcolor(ToolStripStatusLabel lb, Color color, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new tssLabelcolor(tsslabelcolor), new object[] { lb, color, str });
                return;
            }
            lb.ForeColor = color;
            lb.Text = str;
        }

        public void dgv_AutoSize(DataGridView dgv)//dgv表格自适应
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DGVAutoSize(dgv_AutoSize), new object[] { dgv });
                return;
            }
            int width = 0;
            //对于DataGridView的每一个列都调整
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                //将每一列都调整为自动适应模式
                dgv.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                //记录整个DataGridView的宽度
                width += dgv.Columns[i].Width;
            }
            //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
            //则将DataGridView的列自动调整模式设置为显示的列即可，             //如果是小于原来设定的宽度，将模式改为填充。
            if (width > dgv.Size.Width)
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            //设置表格字体居中
            dgv.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //设置表格列字体居中
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 12, FontStyle.Bold);
            dgv.RowsDefaultCellStyle.Font = new Font("微软雅黑", 9);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;//禁止列标题换行
        }

        public void ShowData(DataGridView dgv, DataTable dt, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataTable(ShowData), new object[] { dt, index });
                return;
            }
            switch (index)
            {
                case 0:
                    dgv.DataSource = dt;
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        public void AppendText(ListBox listbox1, string msg, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AppendText), new object[] { listbox1, msg, index });
                return;
            }
            listbox1.SelectedItem = listbox1.Items.Count;
            switch (index)
            {
                case 0:
                    listbox1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + msg);
                    break;
                case 1:
                    listbox1.Items.Add(msg);
                    break;
            }
            listbox1.TopIndex = listbox1.Items.Count - 1;
        }

        public void UiText(string str1, TextBox tb)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { str1, tb });
                return;
            }
            tb.Text = str1;
        }

        public void labelcolor(Label lb, Color color, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelcolor(labelcolor), new object[] { lb, color, str });
                return;
            }
            lb.BackColor = color;
            lb.Text = str;
        }

        public void labelenvision(Label lb, string txt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelvision(labelenvision), new object[] { lb, txt });
                return;
            }
            lb.Text = txt;
        }

        public void ShowStatus(string txt, Color color, int id)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowPlcStatue(ShowStatus), new object[] { txt, color, id });
                return;
            }
            switch (id)
            {
                case 0:
                    tssl_PLCStatus.Text = txt;
                    tssl_PLCStatus.BackColor = color;
                    break;
                case 1:
                    tssl_PDCAStatus.Text = txt;
                    tssl_PDCAStatus.BackColor = color;
                    break;
                case 2:
                    tssl_TraceStatus.Text = txt;
                    tssl_TraceStatus.BackColor = color;
                    break;
                case 3:
                    tssl_OEEStatus.Text = txt;
                    tssl_OEEStatus.BackColor = color;
                    break;
                case 4:
                    tssl_HansStatus.Text = txt;
                    tssl_HansStatus.BackColor = color;
                    break;
                case 5:
                    tssl_ReaderStatus.Text = txt;
                    tssl_ReaderStatus.BackColor = color;
                    break;
                case 6:
                    //bailStatus3.Text = txt;
                    //bailStatus3.BackColor = color;
                    break;
                case 7:
                    TraceParamStatus.Text = txt;
                    TraceParamStatus.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        public void ListData(string str)
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new ShowList(ListData), new object[] { str });
            //    return;
            //}
            //listBox1.Items.Clear();
            //listBox1.Items.Add(str);
        }
        #endregion

        # region 删除长期文件
        private void ClearPic()
        {
            try
            {
                while (true)
                {
                    if (DateTime.Now.Minute == 15 && DateTime.Now.Second == 0)
                    {
                        ClearOverdueFile cof1;
                        ClearOverdueFile cof2;
                        cof1 = new ClearOverdueFile("F:\\public", int.Parse(Global.inidata.productconfig.delete_time));     //超过设定时间3天
                        cof1.FileDeal();
                        Thread.Sleep(500);
                        cof2 = new ClearOverdueFile("F:\\SendPicture", int.Parse(Global.inidata.productconfig.delete_time));     //超过设定时间3天
                        cof2.FileDeal();
                    }
                    Thread.Sleep(1000);//1s循环扫描
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("循环删除文件时间出现错误：" + ex.Message);
            }
        }
        # endregion

        #region TCP/IP通讯 事件方法
        //与TCP服务器已连接
        void client1_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Log.WriteLog("Client1已连接");
            //Global.PLC_Client.WritePLC_D(11320, new short[] { 1 });
        }
        void client2_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            Log.WriteLog("Client2已连接");
            //Global.PLC_Client.WritePLC_D(11321, new short[] { 1 });
        }

        //与TCP服务器断开连接
        void client1_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //Log.WriteLog("Client1已断开");
            // Global.PLC_Client.WritePLC_D(11320, new short[] { 2 });
        }
        void client2_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            //Log.WriteLog("Client2已断开");
            //  Global.PLC_Client.WritePLC_D(11321, new short[] { 2 });
        }
        //接收到TCP服务器反馈
        void client1_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            Log.WriteLog("接收到Server1数据：" + e.Datagram.Replace("\r\n", ""));
            if (e.Datagram == "OK")
            {
                Global.PLC_Client.WritePLC_D(10251, new short[] { 1 });
                flag1 = true;
            }
            try
            {
                if (e.Datagram.Contains("DRD"))
                {
                    string[] data = e.Datagram.Replace("\r\n", "").Split(';');
                    string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[20], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[21], data[22], data[23], data[24], data[25], data[26], data[27], data[28], data[29], "L_Bracket");
                    int r = SQL.ExecuteUpdate(insertStr1);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[20]));
                    string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[48], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[49], data[50], data[51], data[52], data[53], data[54], data[55], data[56], data[57], "L_Bracket");
                    int r2 = SQL.ExecuteUpdate(insertStr2);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[48]));
                    string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[76], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[77], data[78], data[79], data[80], data[81], data[82], data[83], data[84], data[85], "L_Bracket");
                    int r3 = SQL.ExecuteUpdate(insertStr3);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[76]));
                    string insertStr4 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[104], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[105], data[106], data[107], data[108], data[109], data[110], data[111], data[112], data[113], "L_Bracket");
                    int r4 = SQL.ExecuteUpdate(insertStr4);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r4, data[104]));

                    string insertStr5 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[132], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[133], data[134], data[135], data[136], data[137], data[138], data[139], data[140], data[141], "L_Bracket");
                    int r5 = SQL.ExecuteUpdate(insertStr5);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r5, data[132]));
                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("大族参数格式异常" + EX.ToString());
                PLCHeart = false;
                if (MessageBox.Show("大族焊接参数格式异常，请检查DMS软件参数版本") == DialogResult.OK)
                {
                    PLCHeart = true;
                }
            }
        }
        void client2_PlaintextReceived(object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            Log.WriteLog("接收到Server2数据：" + e.Datagram.Replace("\r\n", ""));
            if (e.Datagram == "OK")
            {
                Global.PLC_Client.WritePLC_D(10253, new short[] { 1 });
                flag2 = true;
            }
            try
            {
                if (e.Datagram.Contains("DRD"))
                {
                    string[] data = e.Datagram.Replace("\r\n", "").Split(';');
                    string insertStr1 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[20], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[21], data[22], data[23], data[24], data[25], data[26], data[27], data[28], data[29], "L_Bracket");
                    int r = SQL.ExecuteUpdate(insertStr1);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r, data[20]));
                    string insertStr2 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[48], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[49], data[50], data[51], data[52], data[53], data[54], data[55], data[56], data[57], "L_Bracket");
                    int r2 = SQL.ExecuteUpdate(insertStr2);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r2, data[48]));
                    string insertStr3 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[76], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[77], data[78], data[79], data[80], data[81], data[82], data[83], data[84], data[85], "L_Bracket");
                    int r3 = SQL.ExecuteUpdate(insertStr3);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r3, data[76]));
                    string insertStr4 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[104], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[105], data[106], data[107], data[108], data[109], data[110], data[111], data[112], data[113], "L_Bracket");
                    int r4 = SQL.ExecuteUpdate(insertStr4);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r4, data[104]));

                    string insertStr5 = string.Format("insert into HansData([DateTime],[SN],[Nut],[power_ll],[power_ul],[pattern_type],[spot_size],[hatch],[swing_amplitude],[swing_freq],[JudgeResult],[MeasureTime],[MachineSN],[PulseProfile_measure],[ActualPower],[Power_measure],[WaveForm_measure],[Frequency_measure],[LinearSpeed_measure],[QRelease_measure],[PulseEnergy_measure],[PeakPower_measure],[pulse_profile],[laser_power],[frequency],[waveform],[pulse_energy],[laser_speed],[jump_speed],[jump_delay],[scanner_delay],[Station]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data[0], data[132], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15], data[16], data[17], data[18], data[19], data[133], data[134], data[135], data[136], data[137], data[138], data[139], data[140], data[141], "L_Bracket");
                    int r5 = SQL.ExecuteUpdate(insertStr5);
                    Log.WriteLog(string.Format("插入了{0}行{1}焊接参数", r5, data[132]));


                }
            }
            catch (Exception EX)
            {
                Log.WriteLog("大族参数格式异常" + EX.ToString());
                PLCHeart = false;
                if (MessageBox.Show("大族焊接参数格式异常，请检查DMS软件参数版本") == DialogResult.OK)
                {
                    PLCHeart = true;

                }
            }
        }

        #endregion

        private bool ExistsMdiChildrenInstance(string mdiChildrenClassName)//检测子窗体是否存在
        {
            foreach (Form childForm in this.MdiChildren)
            {
                if (mdiChildrenClassName == childForm.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShowView()//窗体显示
        {
            _homefrm.MdiParent = this;
            _homefrm.Dock = DockStyle.Fill;
            _homefrm.Show();
        }

        private void SetText(int index)
        {
            if (index <= Global.ErrorMsg.Length)
            {
                tsslabelcolor(tsslbl_ErrorMeg, Color.Red, Global.ErrorMsg.Substring(Global.ErrorIndex, Global.ErrorMsg.Length - Global.ErrorIndex));
            }
            else
            {
                Global.ErrorIndex = -1;
            }
        }

        private void Errortime()
        {
            while (true)
            {
                while (Global.errordisplay)
                {
                    double a = timenum / 10;
                    _manualfrm.UpDatalabel(a.ToString("0.00"), "lb_WaitingTime");
                    timenum++;
                    Thread.Sleep(6000);
                }
                Thread.Sleep(3);
            }
        }

        public string GetIp()//获取本机IP
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            //IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            IPAddress localaddr = localhost.AddressList[0];
            return localaddr.ToString();
        }

        public string GetTraceIp()//获取本机Trace_IP
        {
            //string hostName = Dns.GetHostName();   //获取本机名
            //IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            ////IPHostEntry localhost = Dns.GetHostEntry(hostName);   //获取IPv6地址
            //IPAddress localaddr = localhost.AddressList[0];
            //return localaddr.ToString();
            NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            string mip = "";
            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
                if (ni.Name == "0Trace")
                {
                    IPInterfaceProperties property = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in property.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            mip = ip.Address.ToString();
                        }
                    }
                }
            }
            return mip;
        }

        public static string GetMACByIP(string ip)//根据IP获取对应MAC地址
        {
            try
            {
                byte[] aa = new byte[6];

                Int32 ldest = inet_addr(ip); //目的地的ip

                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, aa, ref len);

                return BitConverter.ToString(aa, 0, 6); ;
            }
            catch (Exception err)
            {
                throw err;
            }

        }

        public string GetMac()//获取本机MAC地址
        {
            string strMac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    strMac = mo["MacAddress"].ToString();
                    mo.Dispose();
                    break;
                }
            }
            moc = null;
            mc = null;
            return strMac;
        }

        public void SendPicture(string filename, string sn, int station)//发送PDCA图片
        {
            try
            {
                string SendPicPath = string.Format(@"F:\SendPicture\{0}", sn);
                string[] files1 = null;
                //string[] files2 = null;
                //string[] files3 = null;
                //string[] files4 = null;
                int i = 0;
                try
                {
                    files1 = Directory.GetFiles(string.Format(@"F:\public\{0}", sn), "*.jpg", SearchOption.AllDirectories);
                }
                catch
                {
                    Log.WriteLog(string.Format("本机不存在或缺少{0}文件夹", sn) + ",PDCALog");
                }
                //string[] Files = files1.Concat(files2).Concat(files3).Concat(files4).ToArray();
                if (files1.Length >= 7)
                {
                    foreach (string path in files1)
                    {
                        i++;
                        if (!Directory.Exists(filename))
                        {
                            Directory.CreateDirectory(filename);
                        }
                        if (!Directory.Exists(SendPicPath))
                        {
                            Directory.CreateDirectory(SendPicPath);
                        }
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            //将图片以文件流的形式进行保存
                            BinaryReader br = new BinaryReader(fs);
                            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
                            using (MemoryStream ms = new MemoryStream(imgBytesIn))
                            {
                                Image image = System.Drawing.Image.FromStream(ms);
                                Bitmap bmp = new Bitmap(image);
                                bmp.Save(filename + "\\" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                bmp.Save(SendPicPath + "\\" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                    //PDCA_Pic_OK++;
                    //labelenvision(LB_Pic_ok, PDCA_Pic_OK.ToString());
                    //Log.WriteLog("PDCA图片上传成功数量：" + PDCA_Pic_OK + "Pcs");
                    Log.WriteLog(string.Format("已成功发送{0}图片", sn) + ",PDCALog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传成功数据\\");
                    _homefrm.AppendRichText(string.Format("已成功发送{0}图片", sn), "rtx_PDCAMsg");
                    if (station == 1)
                    {
                        Global.PLC_Client.WritePLC_D(10303, new short[] { 1 });//图片上传成功给plc置1
                    }
                    else
                    {
                        Global.PLC_Client.WritePLC_D(10503, new short[] { 1 });//图片上传成功给plc置1
                    }
                }
                else
                {
                    //PDCA_Pic_NG++;
                    //labelenvision(LB_Pic_ng, PDCA_Pic_NG.ToString());
                    //Log.WriteLog("PDCA图片上传失败数量：" + PDCA_Pic_NG + "Pcs");
                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                    {
                        Global.PDCAUpLoad_Error_D++;//白班PDCA上传异常计数累加
                        _datastatisticsfrm.UpDataDGV_D(13, 1, Global.PDCAUpLoad_Error_D.ToString());
                        Global.inidata.productconfig.PDCAUpLoad_Error_D = Global.PDCAUpLoad_Error_D.ToString();
                        Global.inidata.WriteProductnumSection();
                    }
                    else
                    {
                        Global.PDCAUpLoad_Error_N++;//夜班PDCA上传异常计数累加
                        _datastatisticsfrm.UpDataDGV_N(13, 1, Global.PDCAUpLoad_Error_N.ToString());
                        Global.inidata.productconfig.PDCAUpLoad_Error_N = Global.PDCAUpLoad_Error_N.ToString();
                        Global.inidata.WriteProductnumSection();
                    }
                    Log.WriteLog(string.Format("{0}图片数量不符，有{1}张", sn, files1.Length) + ",PDCALog");
                    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传异常数据\\");
                    _homefrm.AppendRichText(string.Format("{0}图片数量不符，有{1}张", sn, files1.Length), "rtx_PDCAMsg");
                    if (station == 1)
                    {
                        Global.PLC_Client.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                    }
                    else
                    {
                        Global.PLC_Client.WritePLC_D(10503, new short[] { 2 });//图片上传失败给plc置2
                    }
                }
            }
            catch (Exception ex)
            {
                //PDCA_Pic_NG++;
                //labelenvision(LB_Pic_ng, PDCA_Pic_NG.ToString());
                //Log.WriteLog("PDCA图片上传失败数量：" + PDCA_Pic_NG + "Pcs");
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) >= 0 && Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) < 0)
                {
                    Global.PDCAUpLoad_Error_D++;//白班PDCA上传异常计数累加
                    _datastatisticsfrm.UpDataDGV_D(13, 1, Global.PDCAUpLoad_Error_D.ToString());
                    Global.inidata.productconfig.PDCAUpLoad_Error_D = Global.PDCAUpLoad_Error_D.ToString();
                    Global.inidata.WriteProductnumSection();
                }
                else
                {
                    Global.PDCAUpLoad_Error_N++;//夜班PDCA上传异常计数累加
                    _datastatisticsfrm.UpDataDGV_N(13, 1, Global.PDCAUpLoad_Error_N.ToString());
                    Global.inidata.productconfig.PDCAUpLoad_Error_N = Global.PDCAUpLoad_Error_N.ToString();
                    Global.inidata.WriteProductnumSection();
                }
                Log.WriteLog(ex.ToString().Replace("\n", "") + ",PDCALog");
                Log.WriteLog(string.Format("发送SN:{0}图片给MAC mini异常失败！", sn) + ",PDCALog");
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + "U-Bracket" + "," + sn, System.AppDomain.CurrentDomain.BaseDirectory + "\\PDCA图片上传异常数据\\");
                _homefrm.AppendRichText(string.Format("发送SN:{0}图片给MAC mini异常失败！", sn), "rtx_PDCAMsg");
                if (station == 1)
                {
                    Global.PLC_Client.WritePLC_D(10303, new short[] { 2 });//图片上传失败给plc置2
                }
                else
                {
                    Global.PLC_Client.WritePLC_D(10503, new short[] { 2 });//图片上传失败给plc置2
                }
            }
        }

        public void SendCSVFile(string toFilePath, string sn)
        {
            try
            {
                string fromFilePath = string.Format(@"O:\{0}", sn) + ".csv";
                if (File.Exists(fromFilePath))
                {
                    if (File.Exists(toFilePath))
                    {
                        File.Delete(toFilePath);
                    }
                    File.Copy(fromFilePath, toFilePath + ".csv");
                    Log.WriteLog(sn + "普雷斯特CSV上传Macmini成功");
                }
                else
                {
                    Log.WriteLog(string.Format("本机不存在普雷斯特{0}CSV文件", sn));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("发送普雷斯特CSV文档异常！" + ex.ToString().Replace("\n", ""));
            }
        }

        private void Ping_ip(object ob) //PLC、MAC mini连线检测
        {
            while (true)
            {
                try
                {
                    using (System.Net.NetworkInformation.Ping PingSender = new System.Net.NetworkInformation.Ping())
                    {
                        PingOptions Options = new PingOptions();
                        Options.DontFragment = true;
                        string Data = "test";
                        byte[] DataBuffer = Encoding.ASCII.GetBytes(Data);
                        PingReply Reply = PingSender.Send(Global.inidata.productconfig.Plc_IP, 1000, DataBuffer, Options);
                        if (Reply.Status == IPStatus.Success)
                        {
                            Link_PLC = true;
                        }
                        else
                        {
                            Link_PLC = false;
                        }
                    }
                }
                catch
                {
                    Log.WriteLog("Ping PLC IP ERROR!!!");
                }

                //try
                //{
                //    using (System.Net.NetworkInformation.Ping PingSender = new System.Net.NetworkInformation.Ping())
                //    {
                //        PingOptions Options = new PingOptions();
                //        Options.DontFragment = true;
                //        string Data = "test";
                //        byte[] DataBuffer = Encoding.ASCII.GetBytes(Data);
                //        PingReply Reply = PingSender.Send(Global.inidata.productconfig.PDCA_UA_IP, 1000, DataBuffer, Options);
                //        if (Reply.Status == IPStatus.Success)
                //        {
                //            Link_Mac_Mini_Server = true;
                //        }
                //        else
                //        {
                //            Link_Mac_Mini_Server = false;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Log.WriteLog("Ping Mac Mini server IP ERROR!!!");
                //    Link_Mac_Mini_Server = false;
                //}
                Thread.Sleep(5000);
            }
        }

        private void On_Time_doing(object ob)//按时做某事
        {
            while (true)
            {
                try
                {
                   
                  
                    if ((DateTime.Now.Hour == 6 || DateTime.Now.Hour == 18) && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        Log.WriteCSV_NUM(DateTime.Now.ToString("yyyy-MM-dd") + "," + Global.Trace_ua_ok.ToString() + "," + Global.Trace_ua_ng.ToString());
                        Log.WriteCSV_PDCA(DateTime.Now.ToString("yyyy-MM-dd") + "," + (Global.Trace_ua_ok + Global.Trace_ua_ng).ToString());
                        _homefrm.UpDatalabel("0", "lb_TraceUAOK");
                        _homefrm.UpDatalabel("0", "lb_TraceUANG");
                        _homefrm.UpDatalabel("0", "lb_TraceLAOK");
                        _homefrm.UpDatalabel("0", "lb_TraceLANG");
                        _homefrm.UpDatalabel("0", "lb_PDCAUAOK");
                        _homefrm.UpDatalabel("0", "lb_PDCAUANG");
                        _homefrm.UpDatalabel("0", "lb_PDCALAOK");
                        _homefrm.UpDatalabel("0", "lb_PDCALANG");
                        _homefrm.UpDatalabel("0", "lb_OEEOK");
                        _homefrm.UpDatalabel("0", "lb_OEENG");
                        _homefrm.UpDatalabel("0", "lb_FixtureOK");
                        _homefrm.UpDatalabel("0", "lb_FixtureNG");
                        _homefrm.UpDatalabel("0", "lb_ProcessControlOK");
                        _homefrm.UpDatalabel("0", "lb_ProcessControlNG");
                        _homefrm.AppendRichText("N/A", "rtx_TraceMsg");
                        _homefrm.AppendRichText("N/A", "rtx_PDCAMsg");
                        _homefrm.AppendRichText("N/A", "rtx_OEEDefaultMsg");
                        _homefrm.AppendRichText("N/A", "rtx_HeartBeatMsg");
                        _homefrm.AppendRichText("N/A", "rtx_DownTimeMsg");
                        _homefrm.AppendRichText("N/A", "rtx_OEEMateriel");
                        _homefrm.AppendRichText("N/A", "rtx_ProcessControl");
                        //_homefrm.AddList("N/A", "list_FixtureMsg");
                        //_homefrm.AddList("N/A", "list_IQCFixture");
                        //_homefrm.AddList("N/A", "list_FixtureMsgNG");
                        //_homefrm.AddList("N/A", "list_IQCFixtureNG");
                        _userloginfrm.AddList("N/A", "list_UploadLogin");
                        _userloginfrm.AddList("N/A", "list_UserLogin");
                        _userloginfrm.AddList("N/A", "list_FixtureNG");
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(10);
                }
                catch (Exception EX)
                {
                    Log.WriteLog("按时做某事线程异常" + EX.ToString());
                }
            }
        }
        private void AutoStopBreak(object ob)
        {
            while (true)
            {
                if (Global.BreakStatus && ReadStatus[0] != 1)//机台吃饭休息状态并且不是待料时
                {
                    _manualfrm.Btn_UpLoad_break_Click(null, null);
                }
                Thread.Sleep(1000);
            }
        }

        private void UpdateDataGridView_CountDowm(object ob)
        {
            //while (true)
            //{
            //    if (DateTime.Now.Hour != time1)
            //    {
            //        time1 = DateTime.Now.Hour;
            //        string InsertStr1 = string.Format("UPDATE [FixtureStatus] SET [CountDown] = {0} - datediff(SS,Time,GETDATE())/60", Global.Fixture_maintance_time);
            //        SQL.ExecuteUpdate(InsertStr1);
            //        _homefrm.UpdateDataGridView();
            //    }
            //    _homefrm.UpdateDataGridView_CountDowm();
            //    Thread.Sleep(60000);
            //}

        }
        public void Permission_switch(object ob)//按时切换权限
        {
            while (true)
            {
                try
                {
                    if (Global.IfLoginbtn && Global.Login != Global.LoginLevel.Operator)
                    {
                        if ((DateTime.Now - Global.UserLoginMouseMoveTime).TotalMinutes > 5)
                        {
                            _userloginfrm.btn_UserLogin_Click(null, null);
                            Invoke(new Action(() => btn_home_Click(null, null)));
                            _userloginfrm.ComboBoxSelect(0);
                        }
                    }
                    if (Global.IfReadUserID)
                    {
                        if ((DateTime.Now - Global.UserLoginMouseMoveTime).TotalMinutes > 5)
                        {
                            Global.IfReadUserID = false;
                            _userloginfrm.UiLabel("", "lb_CardNo");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog(ex.ToString());
                }
                Thread.Sleep(100);
            }
        }

        //public void ReadOperatorID(object ob)//固定式打卡机读取操作员卡号
        //{
        //    while (Global.IfPQStatus)//每隔5分钟读取一次操作员卡号
        //    {
        //        Log.WriteLog("固定式刷卡机开始读卡！");
        //        FixedRead();
        //        Thread.Sleep(300000);
        //    }
        //}

        //private void FixedRead()
        //{
        //    try
        //    {
        //        string CardNo = string.Empty;
        //        short icdev = 0x0000;
        //        int status;
        //        byte mode = 0x52;
        //        byte bcnt = 0x04;
        //        byte[] pSnr = new byte[10];
        //        ushort tagtype = 0;
        //        byte pRLength = 0;
        //        byte srcLen = 0;
        //        sbyte Size = 0;

        //        if (!Global.bConnectedDevice)
        //        {
        //            MessageBox.Show("固定式刷卡机串口未连接成功！");
        //            return;
        //        }

        //        status = rf_request(icdev, mode, ref tagtype);
        //        if (0 != status)
        //        {
        //            //MessageBox.Show("固定式刷卡机读卡失败！");
        //            Log.WriteLog("固定式刷卡机读卡失败！");
        //            //txtCardNo.Text = "";
        //            return;
        //        }
        //        //lblInfo.Text = "寻卡成功";
        //        status = rf_anticoll(icdev, bcnt, ref pSnr[0], ref pRLength);
        //        if (0 != status)
        //        {
        //            //MessageBox.Show("固定式刷卡机防冲突失败！");
        //            Log.WriteLog("固定式刷卡机防冲突失败！");
        //            return;
        //        }
        //        else
        //        {
        //            //txtCardNo.Text = "";
        //            string temp = string.Empty;
        //            for (int i = 0; i < pRLength; i++)
        //            {
        //                temp = pSnr[i].ToString("X");
        //                temp = temp.Length == 1 ? "0" + temp : temp;        //如果返回的是1字节的1个位，前面需要补0
        //                CardNo = CardNo + temp;
        //            }
        //            //lblInfo.Text = "防冲突操作成功";
        //            string var = CardNo.Substring(6, 2) + CardNo.Substring(4, 2) + CardNo.Substring(2, 2) + CardNo.Substring(0, 2);
        //            UInt32 x = Convert.ToUInt32(var, 16);//字符串转16进制32位无符号整数
        //            CardNo = x.ToString();
        //            //MessageBox.Show("固定式刷卡机读卡成功" + x.ToString());
        //            Log.WriteLog("固定式刷卡机读卡成功,卡号: " + x.ToString());
        //        }
        //        status = rf_select(icdev, ref pSnr[0], srcLen, ref Size);
        //        if (0 != status)
        //        {
        //            //MessageBox.Show("固定式刷卡机选卡失败！");
        //            Log.WriteLog("固定式刷卡机防冲突失败！");
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteLog(ex.ToString());
        //    }
        //}

        public bool dataGridViewToCSV(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = null;
            saveFileDialog.Title = "保存";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += dataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";
                    //表的内容
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            if (k > 0 && k < colCount)
                                strLine += ",";
                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                cell = "\"" + cell + "\"";
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }
                    sw.Close();
                    stream.Close();
                    MessageBox.Show("数据被导出到：" + saveFileDialog.FileName.ToString(), "导出完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }

        private void Timing(object ob)             //和PLC对表
        {
            while (true)
            {
                //if (DateTime.Now.Hour != time)
                //{
                Global.PLC_Client.WritePLC_D(10280, new short[] { 0 });
                //time = DateTime.Now.Hour;
                Global.PLC_Client.WritePLC_D(10270, new short[] { (short)DateTime.Now.Year });
                Global.PLC_Client.WritePLC_D(10271, new short[] { (short)DateTime.Now.Month });
                Global.PLC_Client.WritePLC_D(10272, new short[] { (short)DateTime.Now.Day });
                Global.PLC_Client.WritePLC_D(10273, new short[] { (short)DateTime.Now.Hour });
                Global.PLC_Client.WritePLC_D(10274, new short[] { (short)DateTime.Now.Minute });
                Global.PLC_Client.WritePLC_D(10275, new short[] { (short)DateTime.Now.Second });
                Global.PLC_Client.WritePLC_D(10276, new short[] { (short)DateTime.Now.DayOfWeek });
                Thread.Sleep(500);
                Global.PLC_Client.WritePLC_D(10280, new short[] { 1 });

                //}
                Thread.Sleep(5000);
            }
        }

        private void TimedUpload(object obj)  //定时上传版本信息给中控系统
        {
            while (true)
            {
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    SendVersion();
                }
                Thread.Sleep(1000);
            }
        }
        private void SendVersion()//发送版本信息给---中控系统----
        {
            try
            {
                string RecData = string.Empty;
                string errorMsg = string.Empty;
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值

                //版本信息上传
                VersionData versionData = new VersionData();
                versionData.versions = new Versions[2];
                versionData.versions[0] = new Versions();
                versionData.versions[1] = new Versions();
                //只有初始化类数组里面的每一个类，才能给类的成员赋值。
                versionData.stationID = Global.inidata.productconfig.Station_id_ua;
                versionData.moduleCode = "1";
                versionData.versions[0].type = "ZHH-Bracket-数据处理程序";
                versionData.versions[0].version = getMd5;
                versionData.versions[0].description = "更新中控系统版本上传-2021-12-21";
                versionData.versions[1].type = "ZHH-Bracket-PLC运动控制程序";
                versionData.versions[1].version = getMd5;
                versionData.versions[1].description = "更新中控系统版本上传-2021-12-21";
                string SendData = JsonConvert.SerializeObject(versionData, Formatting.None, jsetting);
                Log.WriteLog("中控软件版本请求上传：" + SendData);
                CentralControlAPI.RequestPost("http://10.143.20.222:80/openApi/v1/machine/software/version?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE0NDc0NDAwMDY4MjkzMTQwNDgiLCJzbiI6MTQ0NzQ0NTI0NDkwNjgzNTk2OH0.pnTP-0hcDzjTPA5yg3PQLyd96yfmosSU0dyE-Bpc8sg", SendData, out RecData, out errorMsg);

                Rec_VersionData rec_Version = JsonConvert.DeserializeObject<Rec_VersionData>(RecData);
                Global.version = rec_Version.data[0].version;
                Log.WriteLog("中控软件版本上传结果：" + rec_Version.ok);
                Log.WriteLog("软件名称：" + rec_Version.data[0].type + "，版本号：" + rec_Version.data[0].version);
                Log.WriteLog("软件名称：" + rec_Version.data[1].type + "，版本号：" + rec_Version.data[1].version);
            }
            catch (Exception ex)
            {
                Log.WriteLog("发送版本信息给中控系统失败，" + ex.ToString());
            }

        }

        public void SaveASToWord(DataGridView datagridview1)
        {
            //if (datagridview1.CurrentRow == null)
            //{
            //    MessageBox.Show("无数据可导出！", "来自系统的消息");
            //}
            //else
            //{
            //    Microsoft.Office.Interop.Word.ApplicationClass wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            //    Microsoft.Office.Interop.Word.Document document;
            //    Microsoft.Office.Interop.Word.Table wordTable;

            //    Microsoft.Office.Interop.Word.Selection wordSelection;
            //    object wordObj = System.Reflection.Missing.Value;

            //    document = wordApp.Documents.Add(ref wordObj, ref wordObj, ref wordObj, ref wordObj);
            //    wordSelection = wordApp.Selection;//显示word文档
            //    wordApp.Visible = true;
            //    if (wordApp == null)
            //    {
            //        MessageBox.Show("本地Word程序无法启动!请检查您的Microsoft Office正确安装并能正常使用", "提示");
            //        return;
            //    }
            //    document.Select();
            //    wordTable = document.Tables.Add(wordSelection.Range, datagridview1.Rows.Count, datagridview1.Columns.Count - 1, ref wordObj, ref wordObj);
            //    //设置列宽
            //    wordTable.Columns.SetWidth(50.0F, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustSameWidth);
            //    //标题数据
            //    for (int i = 1; i < datagridview1.Columns.Count; i++)
            //    {
            //        wordTable.Cell(1, i).Range.InsertAfter(datagridview1.Columns[i].HeaderText);
            //    }
            //    //输出表中数据
            //    try
            //    {
            //        for (int i = 0; i <= datagridview1.Rows.Count - 1; i++)
            //        {
            //            for (int j = 1; j < datagridview1.Columns.Count; j++)
            //            {
            //                wordTable.Cell(i + 2, j).Range.InsertAfter(datagridview1.Rows[i].Cells[j].Value.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show("导出成功！", "来自系统的消息");
            //    }
            //    //wordTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            //    //wordTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
            //}
        }//另存为Word


        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                PassWordFrm pw = new PassWordFrm();
                //pw.PermissionIndex += new Form1.PermissionEventHandler(SetPermissionIndex);
                pw.ShowDialog();
                if (pw.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    var IP = GetIp();
                    var Mac = GetMac();
                    string msg = "";
                    string StopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    if (!Global.SelectFirstModel)//当前是否属于手动(首件)状态
                    {
                        if (!Global.SelectTestRunModel)//判断是否处于空跑状态（PLC屏蔽部分功能如：安全门，扫码枪，机械手）
                        {
                            if ((Global.j == 1 || Global.j == 2 || Global.j == 3 || Global.j == 4) && !Global.BreakStatus)//j为机台运行大状态（-1初始值、1待料、2运行、3宕机、4人工停止）
                            {
                                if (Global.j == 1 && Global.ed[Global.Error_PendingNum].start_time != null)//当前是否属于待料状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_PendingNum].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr2 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].start_time + "'" + ","
                                       + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr2);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_PendingNum].errorCode + "," + Global.ed[Global.Error_PendingNum].start_time + "," + Global.ed[Global.Error_PendingNum].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_PendingNum].errorStatus + "," + Global.ed[Global.Error_PendingNum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                }
                                else if (Global.j == 2 && Global.ed[Global.j].start_time != null)//当前是否属于运行状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.j].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr2 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + Global.ed[Global.j].start_time + "'" + ","
                                       + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr2);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.j].errorCode + "," + Global.ed[Global.j].start_time + "," + Global.ed[Global.j].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.j].errorStatus + "," + Global.ed[Global.j].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                }
                                else if (Global.j == 3 && Global.ed[Global.Error_num].start_time != null)//当前是否属于宕机状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_num].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr3 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + "," + "'" + Global.ed[Global.Error_num].start_time + "'" + ","
                                       + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr3);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_num].errorCode + "," + Global.ed[Global.Error_num].start_time + "," + Global.ed[Global.Error_num].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_num].errorStatus + "," + Global.ed[Global.Error_num].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                }
                                else if (Global.j == 4 && Global.ed[Global.Error_Stopnum].start_time != null)//当前是否属于人工停止复位状态
                                {
                                    DateTime t1 = Convert.ToDateTime(Global.ed[Global.Error_Stopnum].start_time);
                                    DateTime t2 = Convert.ToDateTime(StopTime);
                                    string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                                    string InsertStr4 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].start_time + "'" + ","
                                       + "'" + Global.ed[Global.Error_Stopnum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                                    SQL.ExecuteUpdate(InsertStr4);
                                    Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[Global.Error_Stopnum].errorCode + "," + Global.ed[Global.Error_Stopnum].start_time + "," + Global.ed[Global.Error_Stopnum].ModuleCode + "," + "自动发送成功" + "," + Global.ed[Global.Error_Stopnum].errorStatus + "," + Global.ed[Global.Error_Stopnum].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                                }
                            }
                            else
                            {
                                _manualfrm.Btn_UpLoad_break_Click(null, null);
                            }
                        }
                        else//当前状态为（空跑）状态
                        {
                            DateTime t1 = Convert.ToDateTime(Global.ed[211].start_time);
                            DateTime t2 = Convert.ToDateTime(StopTime);
                            string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                            string InsertStr5 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[211].errorCode + "'" + "," + "'" + Global.ed[211].start_time + "'" + ","
                               + "'" + Global.ed[211].ModuleCode + "'" + "," + "'" + Global.ed[211].errorStatus + "'" + "," + "'" + Global.ed[211].errorinfo + "'" + "," + "'" + ts + "'" + ")";
                            SQL.ExecuteUpdate(InsertStr5);
                            Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.ed[211].errorCode + "," + Global.ed[211].start_time + "," + Global.ed[211].ModuleCode + "," + "自动发送成功" + "," + Global.ed[211].errorStatus + "," + Global.ed[211].errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                        }
                    }
                    else//当前状态为（首件）状态
                    {
                        DateTime t1 = Convert.ToDateTime(Global.errordata.start_time);
                        DateTime t2 = Convert.ToDateTime(StopTime);
                        string ts = (t2 - t1).TotalMinutes.ToString("0.00");
                        string InsertOEEStr6 = "insert into OEE_DT([DateTime],[ErrorCode],[EventTime],[ModuleCode],[RunStatus],[ErrorInfo],[TimeSpan])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.errordata.errorCode + "'" + "," + "'" + Global.errordata.start_time + "'" + ","
                                             + "'" + "" + "'" + "," + "'" + Global.errordata.errorStatus + "'" + "," + "'" + Global.errordata.errorinfo + "'" + "," + "'" + ts + "'" + ")";
                        SQL.ExecuteUpdate(InsertOEEStr6);
                        Log.WriteCSV(DateTime.Now.ToString("HH:mm:ss") + "," + Global.errordata.errorCode + "," + Global.errordata.start_time + "," + "手动发送成功" + "," + Global.errordata.errorStatus + "," + Global.errordata.errorinfo + "," + ts, @"F:\装机软件\系统配置\System_ini\");
                    }

                    string OEEDownTime = "";
                    string DownTimemsg = "";
                    OEEDownTime = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", "6", "10010001", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "");
                    string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                                          + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + ")";
                    SQL.ExecuteUpdate(InsertStr);
                    //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEEDownTime, out DownTimemsg);
                    /////1025
                    ///// 
                    /////Night
                    ///// 
                    string poorNum = string.Empty;
                    string TotalNum = string.Empty;
                    if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                    {
                        poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                        TotalNum = Global.Product_Total_N.ToString();
                    }
                    else
                    {
                        poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                        TotalNum = Global.Product_Total_D.ToString();
                    }
                    Goee.UploadDowntime(poorNum, TotalNum, "6", "10010001", "", false, StopTime, "关机");
                    /////
                    //if (rst)
                    //{
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送成功", "rtx_DownTimeMsg");
                    //    Log.WriteLog("OEE_DT自动errorCode发送成功");
                    //}
                    //else
                    //{
                    //    _homefrm.AppendRichText("10010001" + ",触发时间=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + "6" + ",故障描述:" + "关机" + ",自动发送失败", "rtx_DownTimeMsg");
                    //    Log.WriteLog("OEE_DT自动errorCode发送失败");
                    //    Global.ConnectOEEFlag = false;
                    //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + "6" + "'" + "," + "'" + "10010001" + "'" + ","
                    // + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "" + "'" + "," + "'" + "" + "'" + "," + "'" + "关机" + "'" + ")";
                    //    int r = SQL.ExecuteUpdate(s);
                    //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r));
                    //}
                    Log.WriteLog("OEE_DT:" + OEEDownTime);
                    string InsertOEEStr3 = "insert into OEE_MCOff([DateTime],[Name])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + "软件关闭" + "'" + ")";
                    SQL.ExecuteUpdate(InsertOEEStr3);//插入关机时间
                    //_userloginfrm.btn_UserLogin_Click(null, null); //关机前权限登出
                    //maindis.Shutdown();
                    this.FormClosing -= new FormClosingEventHandler(this.MainFrm_FormClosing);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    this.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }

                Goee?.MQTT_Disconnect();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                //maindis.Shutdown();
                this.FormClosing -= new FormClosingEventHandler(this.MainFrm_FormClosing);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                this.Dispose();
            }
        }

        private byte[] ConvertPrecitecData(string Head, int Length, string sn)//发送precitec数据
        {
            //普雷斯特数据长度格式转换
            string binaryNum = Convert.ToString(Length, 16);
            string result = string.Empty;
            if (binaryNum.Length < 8)
            {
                int length = 8 - binaryNum.Length;
                for (int i = 0; i < length; i++)
                {
                    result += "0";
                }
                result += binaryNum;
            }
            //普雷斯特数据长度格式高低位转换
            string SNlength = result.Substring(6, 2) + result.Substring(4, 2) + result.Substring(2, 2) + result.Substring(0, 2);
            //普雷斯特sn格式转换ASCII码
            byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(sn);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(b.ToString("x"));
            }
            //所有数据格式转换16进制
            string SendData = Head + SNlength + sb.ToString();
            Log.WriteLog(string.Format("发送普雷斯特SN:{0}", SendData));
            byte[] buffer = new byte[SendData.Length / 2];
            for (int i = 0; i < SendData.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(SendData.Substring(i, 2), 16);
            return buffer;

        }

        public static byte[] HexStringToByteArray(string s)//字符串转化16进制
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexSring(byte[] data)//16进制转化字符串
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();

        }

        public static float HexStringToFloat(string s)//字符串转化float
        {
            UInt32 x = Convert.ToUInt32(s, 16);//字符串转16进制32位无符号整数
            float fy = BitConverter.ToSingle(BitConverter.GetBytes(x), 0);//IEEE754 字节转换float
            return fy;
        }

        private string ASCIITo16(String str) //ASCII字符串转16进制数
        {
            byte[] ba = System.Text.ASCIIEncoding.Default.GetBytes(str);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ba)
            {
                sb.Append(b.ToString("x"));
            }
            return sb.ToString();
        }

        private void StopStatus()//OEE 处于人工停止状态
        {
            IP = GetIp();
            Mac = GetMac();
            Global.STOP = true;
            Global.j = ReadStatus[0];
            Global.Error_Stopnum = ReadStatus[2];
            Global.ed[Global.Error_Stopnum].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_Stopnum].Moduleinfo + "_" + Global.ed[Global.Error_Stopnum].errorinfo + "：开始计时 " + Global.ed[Global.Error_Stopnum].start_time);
            if (Global.Error_Stopnum == 7 || Global.Error_Stopnum == 10)//机台打开安全门
            {
                Global.PLC_Client.WritePLC_D(23030, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行                
            }          
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_Stopnum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_Stopnum].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入人工停止复位开始时间
        }
        private void ErrorStatus()//OEE 处于异常状态
        {
            IP = GetIp();
            Mac = GetMac();
            Global.j = ReadStatus[0];
            Global.Error_num = ReadStatus[1];
            Global.ed[Global.Error_num].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_num].Moduleinfo + "_" + Global.ed[Global.Error_num].errorinfo + "：开始计时 " + Global.ed[Global.Error_num].start_time + ",OEELog");
            if (Global.Error_num == 7 || Global.Error_num == 10)//机台打开安全门
            {
                Global.PLC_Client.WritePLC_D(23030, new short[] { 2 });//未手动选择打开安全门原因，机台不能运行
            }
            else
            {
                string OEE_DT = "";
                string msg = "";
                string date = Global.ed[Global.Error_num].start_time;
                OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[Global.Error_num].errorStatus, Global.ed[Global.Error_num].errorCode, date, Global.ed[Global.Error_num].ModuleCode);
                string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + ","
                         + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + ")";
                SQL.ExecuteUpdate(InsertStr);
                Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
                //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);
                ///0924
                string poorNum = string.Empty;
                string TotalNum = string.Empty;
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                    TotalNum = Global.Product_Total_N.ToString();
                }
                else
                {
                    poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                    TotalNum = Global.Product_Total_D.ToString();
                }
                Goee.UploadDowntime(poorNum, TotalNum, Global.ed[Global.Error_num].errorStatus, Global.ed[Global.Error_num].errorCode, Global.ed[Global.Error_num].ModuleCode, false, date, Global.ed[Global.Error_num].errorinfo);
                ///0924


                //if (rst)
                //{
                //    _homefrm.AppendRichText(Global.ed[Global.Error_num].ModuleCode + "," + Global.ed[Global.Error_num].errorCode + ",触发时间=" + Global.ed[Global.Error_num].start_time + ",运行状态:" + Global.ed[Global.Error_num].errorStatus + ",故障描述:" + Global.ed[Global.Error_num].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
                //    Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
                //    Global.ConnectOEEFlag = true;
                //}
                //else
                //{
                //    _homefrm.AppendRichText(Global.ed[Global.Error_num].ModuleCode + "," + Global.ed[Global.Error_num].errorCode + ",触发时间=" + Global.ed[Global.Error_num].start_time + ",运行状态:" + Global.ed[Global.Error_num].errorStatus + ",故障描述:" + Global.ed[Global.Error_num].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
                //    Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
                //    Global.ConnectOEEFlag = false;
                //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + ","
                //       + "'" + Global.ed[Global.Error_num].start_time + "'" + "," + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + ")";
                //    int r = SQL.ExecuteUpdate(s);
                //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
                //}
                _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
                //ButtonFlag(false, errortime_But);
                //ButtonFlag(false, button22);
                string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_num].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_num].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_num].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_num].errorinfo + "'" + ")";
                SQL.ExecuteUpdate(InsertOEEStr);//插入异常开始时间
            }
        }
        private void RunStatus()//OEE 处于运行状态
        {
            Global.SelectManualErrorCode = false;//结束手动选择ErrorCode状态
            IP = GetIp();
            Mac = GetMac();
            Global.j = ReadStatus[0];
            Global.ed[Global.j].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.j].Moduleinfo + "_" + Global.ed[Global.j].errorinfo + "：开始计时 " + Global.ed[Global.j].start_time + ",OEELog");
            string OEE_DT = "";
            string msg = "";
            string date = Global.ed[Global.j].start_time;
            OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[Global.j].errorStatus, "", date, Global.ed[Global.j].ModuleCode);
            string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + "" + "'" + ","
                         + "'" + date + "'" + "," + "'" + Global.ed[Global.j].ModuleCode + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
            //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

            ///0924
            string poorNum = string.Empty;
            string TotalNum = string.Empty;
            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
            {
                poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                TotalNum = Global.Product_Total_N.ToString();
            }
            else
            {
                poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                TotalNum = Global.Product_Total_D.ToString();
            }
            Goee.UploadDowntime(poorNum, TotalNum, Global.ed[Global.j].errorStatus, "", Global.ed[Global.j].ModuleCode, false, date, Global.ed[Global.j].errorinfo);
            ///0924


            //if (rst)
            //{
            //    _homefrm.AppendRichText(Global.ed[Global.j].ModuleCode + "," + "Run" + ",触发时间=" + Global.ed[Global.j].start_time + ",运行状态:" + Global.ed[Global.j].errorStatus + ",故障描述:" + Global.ed[Global.j].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
            //    Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
            //    Global.ConnectOEEFlag = true;
            //}
            //else
            //{
            //    _homefrm.AppendRichText(Global.ed[Global.j].ModuleCode + "," + "Run" + ",触发时间=" + Global.ed[Global.j].start_time + ",运行状态:" + Global.ed[Global.j].errorStatus + ",故障描述:" + Global.ed[Global.j].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //    Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
            //    Global.ConnectOEEFlag = false;
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + ","
            //           + "'" + Global.ed[Global.j].start_time + "'" + "," + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].Moduleinfo + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
            //ButtonFlag(true, errortime_But);
            //ButtonFlag(true, button22);
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.j].errorStatus + "'" + "," + "'" + Global.ed[Global.j].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.j].ModuleCode + "'" + "," + "'" + Global.ed[Global.j].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入运行开始时间
        }
        private void PendingStatus()//OEE 处于待料状态
        {
            IP = GetIp();
            Mac = GetMac();
            Global.j = ReadStatus[0];
            Global.Error_PendingNum = ReadStatus[3];//待料细节字
            Global.ed[Global.Error_PendingNum].start_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Log.WriteLog(Global.ed[Global.Error_PendingNum].Moduleinfo + "_" + Global.ed[Global.Error_PendingNum].errorinfo + "：开始计时 " + Global.ed[Global.Error_PendingNum].start_time + ",OEELog");
            string OEE_DT = "";
            string msg = "";
            string date = Global.ed[Global.Error_PendingNum].start_time;
            OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[Global.Error_PendingNum].errorStatus, Global.ed[Global.Error_PendingNum].errorCode, date, Global.ed[Global.Error_PendingNum].ModuleCode);
            string InsertStr = "insert into OEE_TraceDT([DateTime],[Status],[ErrorCode],[EventTime],[ModuleCode])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + ","
                    + "'" + date + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + ")";
            SQL.ExecuteUpdate(InsertStr);
            Log.WriteLog("OEE_DT:" + OEE_DT + ",OEELog");
            //var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);

            ///0924
            string poorNum = string.Empty;
            string TotalNum = string.Empty;
            if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
            {
                poorNum = (Global.Product_Total_N - Global.Product_OK_N).ToString();
                TotalNum = Global.Product_Total_N.ToString();
            }
            else
            {
                poorNum = (Global.Product_Total_D - Global.Product_OK_D).ToString();
                TotalNum = Global.Product_Total_D.ToString();
            }
            Goee.UploadDowntime(poorNum, TotalNum, Global.ed[Global.Error_PendingNum].errorStatus, Global.ed[Global.Error_PendingNum].errorCode, Global.ed[Global.Error_PendingNum].ModuleCode, false, date, Global.ed[Global.Error_PendingNum].errorinfo);
            ///0924

            //if (rst)
            //{
            //    _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum].ModuleCode + "," + Global.ed[Global.Error_PendingNum].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum].errorinfo + ",自动发送成功", "rtx_DownTimeMsg");
            //    Log.WriteLog("OEE_DT自动errorCode发送成功" + ",OEELog");
            //    Global.ConnectOEEFlag = true;
            //}
            //else
            //{
            //    _homefrm.AppendRichText(Global.ed[Global.Error_PendingNum].ModuleCode + "," + Global.ed[Global.Error_PendingNum].errorCode + ",触发时间=" + Global.ed[Global.Error_PendingNum].start_time + ",运行状态:" + Global.ed[Global.Error_PendingNum].errorStatus + ",故障描述:" + Global.ed[Global.Error_PendingNum].errorinfo + ",自动发送失败", "rtx_DownTimeMsg");
            //    Log.WriteLog("OEE_DT自动errorCode发送失败" + ",OEELog");
            //    Global.ConnectOEEFlag = false;
            //    string s = "insert into OEE_DTSendNG([DateTime],[Product],[Status],[ErrorCode],[EventTime],[ModuleCode],[Moduleinfo],[errorinfo])" + " " + "values(" + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + "'" + "OEE_DT" + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + ","
            //          + "'" + Global.ed[Global.Error_PendingNum].start_time + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].Moduleinfo + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + ")";
            //    int r = SQL.ExecuteUpdate(s);
            //    Log.WriteLog(string.Format("插入了{0}行OEE_DownTime缓存数据", r) + ",OEELog");
            //}
            _manualfrm.labelcolor(Color.Transparent, "未选择", "LB_ManualSelect");
            //ButtonFlag(false, errortime_But);
            //ButtonFlag(false, button22);
            string InsertOEEStr = "insert into OEE_StartTime([Status],[ErrorCode],[EventTime],[ModuleCode],[Name])" + " " + "values(" + "'" + Global.ed[Global.Error_PendingNum].errorStatus + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorCode + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].ModuleCode + "'" + "," + "'" + Global.ed[Global.Error_PendingNum].errorinfo + "'" + ")";
            SQL.ExecuteUpdate(InsertOEEStr);//插入待料开始时间
        }

        private void CacheQuantity(object ob)//上传失败缓存数量
        {
            try
            {
                while (true)
                {
                    DataTable dt = SQL.ExecuteQuery("select * from  Trace_UA_SendNG");
                    _homefrm.UpDatalabel(dt.Rows.Count.ToString(), "lb_TraceSendNG");
                    DataTable dt2 = SQL.ExecuteQuery("select * from  PDCA_SendNG");
                    _homefrm.UpDatalabel(dt2.Rows.Count.ToString(), "lb_PDCASendNG");
                    DataTable dt3 = SQL.ExecuteQuery("select * from  OEE_DefaultSendNG");
                    DataTable dt4 = SQL.ExecuteQuery("select * from  OEE_DTSendNG");
                    _homefrm.UpDatalabel((dt3.Rows.Count + dt4.Rows.Count).ToString(), "lb_OEESendNG");
                    //label_text(label112, Access.QueryNumber("PDCA", "OEEData_SendNG").ToString());
                    //label_text(label172, Access.QueryNumber("PDCA", "Trace_Washer_SendNG").ToString());
                    //label_text(label174, Access.QueryNumber("PDCA", "PDCA_Washer_SendNG").ToString());
                    //label_text(label3, Access.QueryNumber("PDCA", "OEE_DownTime").ToString());
                    //label_text(label35, Access.QueryNumber("PDCA", "OEE_MaterielData").ToString());
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString().Replace("\n", ""));
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="fromFilePath">文件的路径</param>
        /// <param name="toFilePath">文件要拷贝到的路径</param>
        private bool CopyFile(string fromFilePath, string toFilePath)
        {
            try
            {
                if (File.Exists(fromFilePath))
                {
                    if (File.Exists(toFilePath))
                    {
                        File.Delete(toFilePath);
                    }
                    File.Copy(fromFilePath, toFilePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送CSV文件给Macmini解析
        /// </summary>
        /// <param name="url">MacminiURL路径</param>
        /// <param name="timeOut">超时</param>
        /// <param name="fileKeyName">Key</param>
        /// <param name="filePath">Value</param>
        /// <param name="stringDict">键值对集合</param>
        /// <returns></returns>
        public string HttpPostData(string url, int timeOut, string fileKeyName, string filePath, NameValueCollection stringDict)
        {
            string responseContent;
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            // 写入文件
            const string filePartHeader =
        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
        "Content-Type: application/octet-stream\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, filePath);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            // 写入字符串的Key
            var stringKeyHeader = "\r\n--" + boundary +
                 "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                 "\r\n\r\n{1}\r\n";

            foreach (byte[] formitembytes in from string key in stringDict.Keys
                                             select string.Format(stringKeyHeader, key, stringDict[key])
                                                 into formitem
                                             select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // 写入最后的结束边界符
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            var requestStream = webRequest.GetRequestStream();

            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                    Encoding.GetEncoding("utf-8")))
            {
                responseContent = httpStreamReader.ReadToEnd();
            }

            fileStream.Close();
            httpWebResponse.Close();
            webRequest.Abort();

            return responseContent;
        }

        private void GetAllHansData(string SN, out HansData_U_Bracket data, int station)
        {
            lock (LockLA)
            {
                int HansDataResult = 0;
                switch (station)
                {
                    case 1:
                        HansDataResult = 10305;
                        break;
                    case 2:
                        HansDataResult = 10505;
                        break;
                    default:
                        break;
                }
                HansData_U_Bracket UA_data = new HansData_U_Bracket();
                string SelectStr = string.Format("SELECT * FROM HansData WHERE SN='{0}'", SN);//sql查询语句
                DataTable d1 = SQL.ExecuteQuery(SelectStr);
                if (d1 != null && d1.Rows.Count > 0)
                {
                    if (d1.Rows.Count < 5)
                    {
                        UA_data.HanDataUAResult = false;
                        Log.WriteLog(string.Format(("UA获取焊接参数数量不足，有{0}笔，SN为{1}"), d1.Rows.Count, SN));
                        Global.PLC_Client.WritePLC_D(HansDataResult, new short[] { 2 });

                    }
                    else
                    {
                        UA_data.HanDataUAResult = true;
                        Global.PLC_Client.WritePLC_D(HansDataResult, new short[] { 1 });
                    }
                    UA_data.power_ll = d1.Rows[0][4].ToString();
                    UA_data.power_ul = d1.Rows[0][5].ToString();
                    UA_data.pattern_type = d1.Rows[0][6].ToString();
                    UA_data.spot_size = d1.Rows[0][7].ToString();
                    UA_data.hatch = d1.Rows[0][8].ToString();
                    UA_data.swing_amplitude = d1.Rows[0][9].ToString();
                    UA_data.swing_freq = d1.Rows[0][10].ToString();
                    UA_data.JudgeResult = d1.Rows[0][11].ToString();
                    UA_data.MeasureTime = d1.Rows[0][12].ToString();
                    UA_data.MachineSN = d1.Rows[0][13].ToString();
                    UA_data.PulseProfile_measure = d1.Rows[0][14].ToString();
                    UA_data.ActualPower = d1.Rows[0][15].ToString();
                    UA_data.Power_measure = d1.Rows[0][16].ToString();
                    UA_data.WaveForm_measure = d1.Rows[0][17].ToString();
                    UA_data.Frequency_measure = d1.Rows[0][18].ToString();
                    UA_data.LinearSpeed_measure = d1.Rows[0][19].ToString();
                    UA_data.QRelease_measure = d1.Rows[0][20].ToString();
                    UA_data.PulseEnergy_measure = d1.Rows[0][21].ToString();
                    UA_data.PeakPower_measure = d1.Rows[0][22].ToString();
                    for (int i = 0; i < d1.Rows.Count; i++)
                    {
                        #region old
                        //if (d1.Rows[i][3].ToString() == "U-Z")
                        //{
                        //    //UA_data.pulse_profile= d1.Rows[i][23].ToString();
                        //    UA_data.location1_layer1_laser_power = d1.Rows[i][24].ToString();
                        //    UA_data.location1_layer1_frequency = d1.Rows[i][25].ToString();
                        //    UA_data.location1_layer1_waveform = d1.Rows[i][26].ToString();
                        //    UA_data.location1_layer1_pulse_energy = d1.Rows[i][27].ToString();
                        //    UA_data.location1_layer1_laser_speed = d1.Rows[i][28].ToString();
                        //    UA_data.location1_layer1_jump_speed = d1.Rows[i][29].ToString();
                        //    UA_data.location1_layer1_jump_delay = d1.Rows[i][30].ToString();
                        //    UA_data.location1_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        //}
                        //if (d1.Rows[i][3].ToString() == "L-Z")
                        //{
                        //    //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                        //    UA_data.location2_layer1_laser_power = d1.Rows[i][24].ToString();
                        //    UA_data.location2_layer1_frequency = d1.Rows[i][25].ToString();
                        //    UA_data.location2_layer1_waveform = d1.Rows[i][26].ToString();
                        //    UA_data.location2_layer1_pulse_energy = d1.Rows[i][27].ToString();
                        //    UA_data.location2_layer1_laser_speed = d1.Rows[i][28].ToString();
                        //    UA_data.location2_layer1_jump_speed = d1.Rows[i][29].ToString();
                        //    UA_data.location2_layer1_jump_delay = d1.Rows[i][30].ToString();
                        //    UA_data.location2_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        //}
                        //if (d1.Rows[i][3].ToString() == "L-X")
                        //{
                        //    //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                        //    UA_data.location3_layer1_laser_power = d1.Rows[i][24].ToString();
                        //    UA_data.location3_layer1_frequency = d1.Rows[i][25].ToString();
                        //    UA_data.location3_layer1_waveform = d1.Rows[i][26].ToString();
                        //    UA_data.location3_layer1_pulse_energy = d1.Rows[i][27].ToString();
                        //    UA_data.location3_layer1_laser_speed = d1.Rows[i][28].ToString();
                        //    UA_data.location3_layer1_jump_speed = d1.Rows[i][29].ToString();
                        //    UA_data.location3_layer1_jump_delay = d1.Rows[i][30].ToString();
                        //    UA_data.location3_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        //}
                        //if (d1.Rows[i][3].ToString() == "U-X")
                        //{
                        //    //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                        //    UA_data.location4_layer1_laser_power = d1.Rows[i][24].ToString();
                        //    UA_data.location4_layer1_frequency = d1.Rows[i][25].ToString();
                        //    UA_data.location4_layer1_waveform = d1.Rows[i][26].ToString();
                        //    UA_data.location4_layer1_pulse_energy = d1.Rows[i][27].ToString();
                        //    UA_data.location4_layer1_laser_speed = d1.Rows[i][28].ToString();
                        //    UA_data.location4_layer1_jump_speed = d1.Rows[i][29].ToString();
                        //    UA_data.location4_layer1_jump_delay = d1.Rows[i][30].ToString();
                        //    UA_data.location4_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        //} 
                        #endregion

                        if (d1.Rows[i][3].ToString() == "L-X")
                        {
                            //UA_data.pulse_profile= d1.Rows[i][23].ToString();
                            UA_data.location1_layer1_laser_power = d1.Rows[i][24].ToString();
                            UA_data.location1_layer1_frequency = d1.Rows[i][25].ToString();
                            UA_data.location1_layer1_waveform = d1.Rows[i][26].ToString();
                            UA_data.location1_layer1_pulse_energy = d1.Rows[i][27].ToString();
                            UA_data.location1_layer1_laser_speed = d1.Rows[i][28].ToString();
                            UA_data.location1_layer1_jump_speed = d1.Rows[i][29].ToString();
                            UA_data.location1_layer1_jump_delay = d1.Rows[i][30].ToString();
                            UA_data.location1_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "U-X")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                            UA_data.location2_layer1_laser_power = d1.Rows[i][24].ToString();
                            UA_data.location2_layer1_frequency = d1.Rows[i][25].ToString();
                            UA_data.location2_layer1_waveform = d1.Rows[i][26].ToString();
                            UA_data.location2_layer1_pulse_energy = d1.Rows[i][27].ToString();
                            UA_data.location2_layer1_laser_speed = d1.Rows[i][28].ToString();
                            UA_data.location2_layer1_jump_speed = d1.Rows[i][29].ToString();
                            UA_data.location2_layer1_jump_delay = d1.Rows[i][30].ToString();
                            UA_data.location2_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "L-Z")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                            UA_data.location3_layer1_laser_power = d1.Rows[i][24].ToString();
                            UA_data.location3_layer1_frequency = d1.Rows[i][25].ToString();
                            UA_data.location3_layer1_waveform = d1.Rows[i][26].ToString();
                            UA_data.location3_layer1_pulse_energy = d1.Rows[i][27].ToString();
                            UA_data.location3_layer1_laser_speed = d1.Rows[i][28].ToString();
                            UA_data.location3_layer1_jump_speed = d1.Rows[i][29].ToString();
                            UA_data.location3_layer1_jump_delay = d1.Rows[i][30].ToString();
                            UA_data.location3_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "U-Z-1")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                            UA_data.location4_layer1_laser_power = d1.Rows[i][24].ToString();
                            UA_data.location4_layer1_frequency = d1.Rows[i][25].ToString();
                            UA_data.location4_layer1_waveform = d1.Rows[i][26].ToString();
                            UA_data.location4_layer1_pulse_energy = d1.Rows[i][27].ToString();
                            UA_data.location4_layer1_laser_speed = d1.Rows[i][28].ToString();
                            UA_data.location4_layer1_jump_speed = d1.Rows[i][29].ToString();
                            UA_data.location4_layer1_jump_delay = d1.Rows[i][30].ToString();
                            UA_data.location4_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        }
                        if (d1.Rows[i][3].ToString() == "U-Z-2")
                        {
                            //UA_data.pulse_profile = d1.Rows[i][23].ToString();
                            UA_data.location5_layer1_laser_power = d1.Rows[i][24].ToString();
                            UA_data.location5_layer1_frequency = d1.Rows[i][25].ToString();
                            UA_data.location5_layer1_waveform = d1.Rows[i][26].ToString();
                            UA_data.location5_layer1_pulse_energy = d1.Rows[i][27].ToString();
                            UA_data.location5_layer1_laser_speed = d1.Rows[i][28].ToString();
                            UA_data.location5_layer1_jump_speed = d1.Rows[i][29].ToString();
                            UA_data.location5_layer1_jump_delay = d1.Rows[i][30].ToString();
                            UA_data.location5_layer1_scanner_delay = d1.Rows[i][31].ToString();
                        }
                    }
                }
                else
                {
                    UA_data.HanDataUAResult = false;
                    Global.PLC_Client.WritePLC_D(HansDataResult, new short[] { 2 });
                    Log.WriteLog(string.Format(("UA获取焊接参数数量不足，有{0}笔，SN为{1}"), d1.Rows.Count, SN));

                }
                data = UA_data;
            }
        }

        private void Product_DataStatistics()//产能数据统计
        {
            //-------------------------------白班产能统计--------------------------------------------
            Global.Product_Total = Global.PLC_Client.ReadPLC_D(2030, 12);//白班总产能
            Global.Product_NG = Global.PLC_Client.ReadPLC_D(2056, 12);//白班NG产能
            Global.Product_OK = Global.PLC_Client.ReadPLC_D(2006, 12);//白班OK产能
            short Product_Total_08_09 = Global.Product_Total[0];//白班8-9点总产能
            short Product_Total_09_10 = Global.Product_Total[1];
            short Product_Total_10_11 = Global.Product_Total[2];
            short Product_Total_11_12 = Global.Product_Total[3];
            short Product_Total_12_13 = Global.Product_Total[4];
            short Product_Total_13_14 = Global.Product_Total[5];
            short Product_Total_14_15 = Global.Product_Total[6];
            short Product_Total_15_16 = Global.Product_Total[7];
            short Product_Total_16_17 = Global.Product_Total[8];
            short Product_Total_17_18 = Global.Product_Total[9];
            short Product_Total_18_19 = Global.Product_Total[10];
            short Product_Total_19_20 = Global.Product_Total[11];//白班19-20点总产能
            short Product_NG_08_09 = Global.Product_NG[0];//白班8-9点NG产能
            short Product_NG_09_10 = Global.Product_NG[1];
            short Product_NG_10_11 = Global.Product_NG[2];
            short Product_NG_11_12 = Global.Product_NG[3];
            short Product_NG_12_13 = Global.Product_NG[4];
            short Product_NG_13_14 = Global.Product_NG[5];
            short Product_NG_14_15 = Global.Product_NG[6];
            short Product_NG_15_16 = Global.Product_NG[7];
            short Product_NG_16_17 = Global.Product_NG[8];
            short Product_NG_17_18 = Global.Product_NG[9];
            short Product_NG_18_19 = Global.Product_NG[10];
            short Product_NG_19_20 = Global.Product_NG[11];//白班19-20点NG产能

            ///20210814update
            /// 
            Global.DT_jiadonglv_08_20 = Global.PLC_Client.ReadPLC_D(4224, 1)[0];//白班稼动率

            if (Product_Total_08_09 == 0)
            {
                Product_Lianglv_08_09 = 0;
            }
            else
            {
                Product_Lianglv_08_09 = ((double)(Product_Total_08_09 - Product_NG_08_09) / (double)Product_Total_08_09) * 100;//白班8-9点良率
            }
            if (Product_Total_09_10 == 0)
            {
                Product_Lianglv_09_10 = 0;
            }
            else
            {
                Product_Lianglv_09_10 = ((double)(Product_Total_09_10 - Product_NG_09_10) / (double)Product_Total_09_10) * 100;
            }
            if (Product_Total_10_11 == 0)
            {
                Product_Lianglv_10_11 = 0;
            }
            else
            {
                Product_Lianglv_10_11 = ((double)(Product_Total_10_11 - Product_NG_10_11) / (double)Product_Total_10_11) * 100;
            }
            if (Product_Total_11_12 == 0)
            {
                Product_Lianglv_11_12 = 0;
            }
            else
            {
                Product_Lianglv_11_12 = ((double)(Product_Total_11_12 - Product_NG_11_12) / (double)Product_Total_11_12) * 100;
            }
            if (Product_Total_12_13 == 0)
            {
                Product_Lianglv_12_13 = 0;
            }
            else
            {
                Product_Lianglv_12_13 = ((double)(Product_Total_12_13 - Product_NG_12_13) / (double)Product_Total_12_13) * 100;
            }
            if (Product_Total_13_14 == 0)
            {
                Product_Lianglv_13_14 = 0;
            }
            else
            {
                Product_Lianglv_13_14 = ((double)(Product_Total_13_14 - Product_NG_13_14) / (double)Product_Total_13_14) * 100;
            }
            if (Product_Total_14_15 == 0)
            {
                Product_Lianglv_14_15 = 0;
            }
            else
            {
                Product_Lianglv_14_15 = ((double)(Product_Total_14_15 - Product_NG_14_15) / (double)Product_Total_14_15) * 100;
            }
            if (Product_Total_15_16 == 0)
            {
                Product_Lianglv_15_16 = 0;
            }
            else
            {
                Product_Lianglv_15_16 = ((double)(Product_Total_15_16 - Product_NG_15_16) / (double)Product_Total_15_16) * 100;
            }
            if (Product_Total_16_17 == 0)
            {
                Product_Lianglv_16_17 = 0;
            }
            else
            {
                Product_Lianglv_16_17 = ((double)(Product_Total_16_17 - Product_NG_16_17) / (double)Product_Total_16_17) * 100;
            }
            if (Product_Total_17_18 == 0)
            {
                Product_Lianglv_17_18 = 0;
            }
            else
            {
                Product_Lianglv_17_18 = ((double)(Product_Total_17_18 - Product_NG_17_18) / (double)Product_Total_17_18) * 100;
            }
            if (Product_Total_18_19 == 0)
            {
                Product_Lianglv_18_19 = 0;
            }
            else
            {
                Product_Lianglv_18_19 = ((double)(Product_Total_18_19 - Product_NG_18_19) / (double)Product_Total_18_19) * 100;
            }
            if (Product_Total_19_20 == 0)
            {
                Product_Lianglv_19_20 = 0;
            }
            else
            {
                Product_Lianglv_19_20 = ((double)(Product_Total_19_20 - Product_NG_19_20) / (double)Product_Total_19_20) * 100;//白班19-20点良率
            }

            short Product_Total_08_20 = Global.PLC_Client.ReadPLC_D(2950, 1)[0];//白班总产能
            short Product_NG_08_20 = Global.PLC_Client.ReadPLC_D(2952, 1)[0];//白班NG总产能
            if (Global.PLC_Client.ReadPLC_D(2950, 1)[0] == 0)
            {
                Product_Lianglv_08_20 = 0;
            }
            else
            {
                Product_Lianglv_08_20 = ((double)(Global.PLC_Client.ReadPLC_D(2950, 1)[0] - Global.PLC_Client.ReadPLC_D(2952, 1)[0]) / (double)Global.PLC_Client.ReadPLC_D(2950, 1)[0]) * 100;//白班总良率
            }
            Global.Product_Total_D = Product_Total_08_20;
            Global.Product_OK_D = Product_Total_08_20 - Product_NG_08_20;
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                _datastatisticsfrm.UpDatalabel(Product_Total_08_09.ToString(), "lb_Product_Total_08_09");
                _datastatisticsfrm.UpDatalabel(Product_Total_09_10.ToString(), "lb_Product_Total_09_10");
                _datastatisticsfrm.UpDatalabel(Product_Total_10_11.ToString(), "lb_Product_Total_10_11");
                _datastatisticsfrm.UpDatalabel(Product_Total_11_12.ToString(), "lb_Product_Total_11_12");
                _datastatisticsfrm.UpDatalabel(Product_Total_12_13.ToString(), "lb_Product_Total_12_13");
                _datastatisticsfrm.UpDatalabel(Product_Total_13_14.ToString(), "lb_Product_Total_13_14");
                _datastatisticsfrm.UpDatalabel(Product_Total_14_15.ToString(), "lb_Product_Total_14_15");
                _datastatisticsfrm.UpDatalabel(Product_Total_15_16.ToString(), "lb_Product_Total_15_16");
                _datastatisticsfrm.UpDatalabel(Product_Total_16_17.ToString(), "lb_Product_Total_16_17");
                _datastatisticsfrm.UpDatalabel(Product_Total_17_18.ToString(), "lb_Product_Total_17_18");
                _datastatisticsfrm.UpDatalabel(Product_Total_18_19.ToString(), "lb_Product_Total_18_19");
                _datastatisticsfrm.UpDatalabel(Product_Total_19_20.ToString(), "lb_Product_Total_19_20");

                _datastatisticsfrm.UpDatalabel(Product_NG_08_09.ToString(), "lb_Product_NG_08_09");
                _datastatisticsfrm.UpDatalabel(Product_NG_09_10.ToString(), "lb_Product_NG_09_10");
                _datastatisticsfrm.UpDatalabel(Product_NG_10_11.ToString(), "lb_Product_NG_10_11");
                _datastatisticsfrm.UpDatalabel(Product_NG_11_12.ToString(), "lb_Product_NG_11_12");
                _datastatisticsfrm.UpDatalabel(Product_NG_12_13.ToString(), "lb_Product_NG_12_13");
                _datastatisticsfrm.UpDatalabel(Product_NG_13_14.ToString(), "lb_Product_NG_13_14");
                _datastatisticsfrm.UpDatalabel(Product_NG_14_15.ToString(), "lb_Product_NG_14_15");
                _datastatisticsfrm.UpDatalabel(Product_NG_15_16.ToString(), "lb_Product_NG_15_16");
                _datastatisticsfrm.UpDatalabel(Product_NG_16_17.ToString(), "lb_Product_NG_16_17");
                _datastatisticsfrm.UpDatalabel(Product_NG_17_18.ToString(), "lb_Product_NG_17_18");
                _datastatisticsfrm.UpDatalabel(Product_NG_18_19.ToString(), "lb_Product_NG_18_19");
                _datastatisticsfrm.UpDatalabel(Product_NG_19_20.ToString(), "lb_Product_NG_19_20");

                _datastatisticsfrm.UpDatalabel(Product_Lianglv_08_09.ToString("0.00") + "%", "lb_Product_Lianglv_08_09");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_09_10.ToString("0.00") + "%", "lb_Product_Lianglv_09_10");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_10_11.ToString("0.00") + "%", "lb_Product_Lianglv_10_11");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_11_12.ToString("0.00") + "%", "lb_Product_Lianglv_11_12");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_12_13.ToString("0.00") + "%", "lb_Product_Lianglv_12_13");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_13_14.ToString("0.00") + "%", "lb_Product_Lianglv_13_14");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_14_15.ToString("0.00") + "%", "lb_Product_Lianglv_14_15");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_15_16.ToString("0.00") + "%", "lb_Product_Lianglv_15_16");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_16_17.ToString("0.00") + "%", "lb_Product_Lianglv_16_17");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_17_18.ToString("0.00") + "%", "lb_Product_Lianglv_17_18");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_18_19.ToString("0.00") + "%", "lb_Product_Lianglv_18_19");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_19_20.ToString("0.00") + "%", "lb_Product_Lianglv_19_20");

                _datastatisticsfrm.UpDatalabel(Product_Total_08_20.ToString(), "lb_Product_Total_08_20");
                _datastatisticsfrm.UpDatalabel(Product_NG_08_20.ToString(), "lb_Product_NG_08_20");
                _datastatisticsfrm.UpDatalabel(Product_Lianglv_08_20.ToString("0.00") + "%", "lb_Product_Lianglv_08_20");


                ///20210814
                /// 
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(Global.DT_jiadonglv_08_20) / 100).ToString("0.00") + "%", "DayJd");

                _datastatisticsfrm.UpDataDGV_D(0, 1, Global.Product_Total_D.ToString());
                _datastatisticsfrm.UpDataDGV_D(1, 1, Global.inidata.productconfig.Smallmaterial_Input_D.ToString());
                _datastatisticsfrm.UpDataDGV_D(4, 1, Global.inidata.productconfig.Welding_Error_D);
                double Welding_Error_D = (Convert.ToDouble(Global.inidata.productconfig.Welding_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(4, 2, Welding_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(12, 1, Global.inidata.productconfig.TraceUpLoad_Error_D);
                double TraceUpLoad_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TraceUpLoad_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(12, 2, TraceUpLoad_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(13, 1, Global.inidata.productconfig.PDCAUpLoad_Error_D);
                double PDCAUpLoad_Error_D = (Convert.ToDouble(Global.inidata.productconfig.PDCAUpLoad_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(13, 2, PDCAUpLoad_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(14, 1, Global.inidata.productconfig.TracePVCheck_Error_D);
                double TracePVCheck_Error_D = (Convert.ToDouble(Global.inidata.productconfig.TracePVCheck_Error_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(14, 2, TracePVCheck_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(5, 1, Global.inidata.productconfig.Location1_NG_D);
                double Location1_NG_D = (Convert.ToDouble(Global.inidata.productconfig.Location1_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(5, 2, Location1_NG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(6, 1, Global.inidata.productconfig.Location2_NG_D);
                double Location2_NG_D = (Convert.ToDouble(Global.inidata.productconfig.Location2_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(6, 2, Location2_NG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(7, 1, Global.inidata.productconfig.Location3_NG_D);
                double Location3_NG_D = (Convert.ToDouble(Global.inidata.productconfig.Location3_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(7, 2, Location3_NG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(8, 1, Global.inidata.productconfig.Location4_NG_D);
                double Location4_NG_D = (Convert.ToDouble(Global.inidata.productconfig.Location4_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(8, 2, Location4_NG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(9, 1, Global.inidata.productconfig.Location5_NG_D);
                double Location5_NG_D = (Convert.ToDouble(Global.inidata.productconfig.Location5_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(9, 2, Location5_NG_D.ToString("0.00") + "%");

                _datastatisticsfrm.UpDataDGV_D(15, 1, Global.inidata.productconfig.ReadBarcode_NG_D);
                double ReadBarcode_NG_D = (Convert.ToDouble(Global.inidata.productconfig.ReadBarcode_NG_D) / Global.Product_Total_D) * 100;
                _datastatisticsfrm.UpDataDGV_D(15, 2, ReadBarcode_NG_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(16, 1, Global.inidata.productconfig.CCDCheck_Error_D);
                double CCDCheck_Error_D = (Convert.ToDouble(Global.inidata.productconfig.CCDCheck_Error_D) / Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_Input_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(16, 2, CCDCheck_Error_D.ToString("0.00") + "%");
                _datastatisticsfrm.UpDataDGV_D(17, 1, Global.inidata.productconfig.Smallmaterial_throwing_D);
                double Smallmaterial_throwing_D = (Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_throwing_D) / Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_Input_D)) * 100;
                _datastatisticsfrm.UpDataDGV_D(17, 2, Smallmaterial_throwing_D.ToString("0.00") + "%");

            }
            //-------------------------------夜班产能统计--------------------------------------------
            Global.Product_Total_N_1 = Global.PLC_Client.ReadPLC_D(2042, 6);//夜班产能1
            Global.Product_Total_N_2 = Global.PLC_Client.ReadPLC_D(2024, 6);//夜班产能2
            Global.Product_NG_N_1 = Global.PLC_Client.ReadPLC_D(2068, 6);//夜班NG产能1
            Global.Product_NG_N_2 = Global.PLC_Client.ReadPLC_D(2050, 6);//夜班NG产能2
            Global.Product_OK_N_1 = Global.PLC_Client.ReadPLC_D(2018, 6);//夜班OK产能1
            Global.Product_OK_N_2 = Global.PLC_Client.ReadPLC_D(2000, 6);//夜班OK产能2
            short Product_Total_20_21 = Global.Product_Total_N_1[0];//夜班17:30-5:30点总产能
            short Product_Total_21_22 = Global.Product_Total_N_1[1];
            short Product_Total_22_23 = Global.Product_Total_N_1[2];
            short Product_Total_23_00 = Global.Product_Total_N_1[3];
            short Product_Total_00_01 = Global.Product_Total_N_1[4];
            short Product_Total_01_02 = Global.Product_Total_N_1[5];
            short Product_Total_02_03 = Global.Product_Total_N_2[0];
            short Product_Total_03_04 = Global.Product_Total_N_2[1];
            short Product_Total_04_05 = Global.Product_Total_N_2[2];
            short Product_Total_05_06 = Global.Product_Total_N_2[3];
            short Product_Total_06_07 = Global.Product_Total_N_2[4];
            short Product_Total_07_08 = Global.Product_Total_N_2[5];//夜班4:30-5:30点总产能
            short Product_NG_20_21 = Global.Product_NG_N_1[0];//夜班8-9点NG产能
            short Product_NG_21_22 = Global.Product_NG_N_1[1];
            short Product_NG_22_23 = Global.Product_NG_N_1[2];
            short Product_NG_23_00 = Global.Product_NG_N_1[3];
            short Product_NG_00_01 = Global.Product_NG_N_1[4];
            short Product_NG_01_02 = Global.Product_NG_N_1[5];
            short Product_NG_02_03 = Global.Product_NG_N_2[0];
            short Product_NG_03_04 = Global.Product_NG_N_2[1];
            short Product_NG_04_05 = Global.Product_NG_N_2[2];
            short Product_NG_05_06 = Global.Product_NG_N_2[3];
            short Product_NG_06_07 = Global.Product_NG_N_2[4];
            short Product_NG_07_08 = Global.Product_NG_N_2[5];//夜班19-20点NG产能

            ///20210814
            /// 
            Global.DT_jiadonglv_20_08 = Global.PLC_Client.ReadPLC_D(4254, 1)[0];//夜班稼动率

            if (Product_Total_20_21 == 0)
            {
                Product_Lianglv_20_21 = 0;
            }
            else
            {
                Product_Lianglv_20_21 = ((double)(Product_Total_20_21 - Product_NG_20_21) / (double)Product_Total_20_21) * 100;//夜班20-21点良率
            }
            if (Product_Total_21_22 == 0)
            {
                Product_Lianglv_21_22 = 0;
            }
            else
            {
                Product_Lianglv_21_22 = ((double)(Product_Total_21_22 - Product_NG_21_22) / (double)Product_Total_21_22) * 100;
            }
            if (Product_Total_22_23 == 0)
            {
                Product_Lianglv_22_23 = 0;
            }
            else
            {
                Product_Lianglv_22_23 = ((double)(Product_Total_22_23 - Product_NG_22_23) / (double)Product_Total_22_23) * 100;
            }
            if (Product_Total_23_00 == 0)
            {
                Product_Lianglv_23_00 = 0;
            }
            else
            {
                Product_Lianglv_23_00 = ((double)(Product_Total_23_00 - Product_NG_23_00) / (double)Product_Total_23_00) * 100;
            }
            if (Product_Total_00_01 == 0)
            {
                Product_Lianglv_00_01 = 0;
            }
            else
            {
                Product_Lianglv_00_01 = ((double)(Product_Total_00_01 - Product_NG_00_01) / (double)Product_Total_00_01) * 100;
            }
            if (Product_Total_01_02 == 0)
            {
                Product_Lianglv_01_02 = 0;
            }
            else
            {
                Product_Lianglv_01_02 = ((double)(Product_Total_01_02 - Product_NG_01_02) / (double)Product_Total_01_02) * 100;
            }
            if (Product_Total_02_03 == 0)
            {
                Product_Lianglv_02_03 = 0;
            }
            else
            {
                Product_Lianglv_02_03 = ((double)(Product_Total_02_03 - Product_NG_02_03) / (double)Product_Total_02_03) * 100;
            }
            if (Product_Total_03_04 == 0)
            {
                Product_Lianglv_03_04 = 0;
            }
            else
            {
                Product_Lianglv_03_04 = ((double)(Product_Total_03_04 - Product_NG_03_04) / (double)Product_Total_03_04) * 100;
            }
            if (Product_Total_04_05 == 0)
            {
                Product_Lianglv_04_05 = 0;
            }
            else
            {
                Product_Lianglv_04_05 = ((double)(Product_Total_04_05 - Product_NG_04_05) / (double)Product_Total_04_05) * 100;
            }
            if (Product_Total_05_06 == 0)
            {
                Product_Lianglv_05_06 = 0;
            }
            else
            {
                Product_Lianglv_05_06 = ((double)(Product_Total_05_06 - Product_NG_05_06) / (double)Product_Total_05_06) * 100;
            }
            if (Product_Total_06_07 == 0)
            {
                Product_Lianglv_06_07 = 0;
            }
            else
            {
                Product_Lianglv_06_07 = ((double)(Product_Total_06_07 - Product_NG_06_07) / (double)Product_Total_06_07) * 100;
            }
            if (Product_Total_07_08 == 0)
            {
                Product_Lianglv_07_08 = 0;
            }
            else
            {
                Product_Lianglv_07_08 = ((double)(Product_Total_07_08 - Product_NG_07_08) / (double)Product_Total_07_08) * 100;//夜班07-08点良率
            }

            short Product_Total_20_08 = Global.PLC_Client.ReadPLC_D(2960, 1)[0];//夜班总产能
            short Product_NG_20_08 = Global.PLC_Client.ReadPLC_D(2962, 1)[0];//夜班NG总产能
            if (Global.PLC_Client.ReadPLC_D(2960, 1)[0] == 0)
            {
                Product_Lianglv_20_08 = 0;
            }
            else
            {
                Product_Lianglv_20_08 = ((double)(Global.PLC_Client.ReadPLC_D(2960, 1)[0] - Global.PLC_Client.ReadPLC_D(2962, 1)[0]) / (double)Global.PLC_Client.ReadPLC_D(2960, 1)[0]) * 100;//夜班总良率
            }
            Global.Product_Total_N = Product_Total_20_08;
            Global.Product_OK_N = Product_Total_20_08 - Product_NG_20_08;
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDateTime.ToString("yyyy-MM-dd"))
            {
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    _datastatisticsfrm.UpDatalabel(Product_Total_20_21.ToString(), "lb_Product_Total_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_Total_21_22.ToString(), "lb_Product_Total_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_Total_22_23.ToString(), "lb_Product_Total_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_Total_23_00.ToString(), "lb_Product_Total_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_Total_00_01.ToString(), "lb_Product_Total_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_Total_01_02.ToString(), "lb_Product_Total_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_Total_02_03.ToString(), "lb_Product_Total_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_Total_03_04.ToString(), "lb_Product_Total_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_Total_04_05.ToString(), "lb_Product_Total_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_Total_05_06.ToString(), "lb_Product_Total_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_Total_06_07.ToString(), "lb_Product_Total_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_Total_07_08.ToString(), "lb_Product_Total_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_NG_20_21.ToString(), "lb_Product_NG_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_NG_21_22.ToString(), "lb_Product_NG_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_NG_22_23.ToString(), "lb_Product_NG_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_NG_23_00.ToString(), "lb_Product_NG_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_NG_00_01.ToString(), "lb_Product_NG_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_NG_01_02.ToString(), "lb_Product_NG_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_NG_02_03.ToString(), "lb_Product_NG_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_NG_03_04.ToString(), "lb_Product_NG_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_NG_04_05.ToString(), "lb_Product_NG_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_NG_05_06.ToString(), "lb_Product_NG_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_NG_06_07.ToString(), "lb_Product_NG_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_NG_07_08.ToString(), "lb_Product_NG_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_20_21.ToString("0.00") + "%", "lb_Product_Lianglv_20_21");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_21_22.ToString("0.00") + "%", "lb_Product_Lianglv_21_22");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_22_23.ToString("0.00") + "%", "lb_Product_Lianglv_22_23");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_23_00.ToString("0.00") + "%", "lb_Product_Lianglv_23_00");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_00_01.ToString("0.00") + "%", "lb_Product_Lianglv_00_01");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_01_02.ToString("0.00") + "%", "lb_Product_Lianglv_01_02");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_02_03.ToString("0.00") + "%", "lb_Product_Lianglv_02_03");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_03_04.ToString("0.00") + "%", "lb_Product_Lianglv_03_04");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_04_05.ToString("0.00") + "%", "lb_Product_Lianglv_04_05");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_05_06.ToString("0.00") + "%", "lb_Product_Lianglv_05_06");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_06_07.ToString("0.00") + "%", "lb_Product_Lianglv_06_07");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_07_08.ToString("0.00") + "%", "lb_Product_Lianglv_07_08");

                    _datastatisticsfrm.UpDatalabel(Product_Total_20_08.ToString(), "lb_Product_Total_20_08");
                    _datastatisticsfrm.UpDatalabel(Product_NG_20_08.ToString(), "lb_Product_NG_20_08");
                    _datastatisticsfrm.UpDatalabel(Product_Lianglv_20_08.ToString("0.00") + "%", "lb_Product_Lianglv_20_08");

                    ///20210814
                    /// 
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(Global.DT_jiadonglv_20_08) / 100).ToString("0.00") + "%", "NightJd");

                    _datastatisticsfrm.UpDataDGV_N(0, 1, Global.Product_Total_N.ToString());
                    _datastatisticsfrm.UpDataDGV_N(1, 1, Global.inidata.productconfig.Smallmaterial_Input_N.ToString());
                    _datastatisticsfrm.UpDataDGV_N(4, 1, Global.inidata.productconfig.Welding_Error_N);
                    double Welding_Error_N = (Convert.ToDouble(Global.inidata.productconfig.Welding_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(4, 2, Welding_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(12, 1, Global.inidata.productconfig.TraceUpLoad_Error_N);
                    double TraceUpLoad_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TraceUpLoad_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(12, 2, TraceUpLoad_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(13, 1, Global.inidata.productconfig.PDCAUpLoad_Error_N);
                    double PDCAUpLoad_Error_N = (Convert.ToDouble(Global.inidata.productconfig.PDCAUpLoad_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(13, 2, PDCAUpLoad_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(14, 1, Global.inidata.productconfig.TracePVCheck_Error_N);
                    double TracePVCheck_Error_N = (Convert.ToDouble(Global.inidata.productconfig.TracePVCheck_Error_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(14, 2, TracePVCheck_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(5, 1, Global.inidata.productconfig.Location1_NG_N);
                    double Location1_NG_N = (Convert.ToDouble(Global.inidata.productconfig.Location1_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(5, 2, Location1_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(6, 1, Global.inidata.productconfig.Location2_NG_N);
                    double Location2_NG_N = (Convert.ToDouble(Global.inidata.productconfig.Location2_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(6, 2, Location2_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(7, 1, Global.inidata.productconfig.Location3_NG_N);
                    double Location3_NG_N = (Convert.ToDouble(Global.inidata.productconfig.Location3_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(7, 2, Location3_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(8, 1, Global.inidata.productconfig.Location4_NG_N);
                    double Location4_NG_N = (Convert.ToDouble(Global.inidata.productconfig.Location4_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(8, 2, Location4_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(9, 1, Global.inidata.productconfig.Location5_NG_N);
                    double Location5_NG_N = (Convert.ToDouble(Global.inidata.productconfig.Location5_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(9, 2, Location5_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(15, 1, Global.inidata.productconfig.ReadBarcode_NG_N);
                    double ReadBarcode_NG_N = (Convert.ToDouble(Global.inidata.productconfig.ReadBarcode_NG_N) / Global.Product_Total_N) * 100;
                    _datastatisticsfrm.UpDataDGV_N(15, 2, ReadBarcode_NG_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(16, 1, Global.inidata.productconfig.CCDCheck_Error_N);
                    double CCDCheck_Error_N = (Convert.ToDouble(Global.inidata.productconfig.CCDCheck_Error_N) / Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_Input_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(16, 2, CCDCheck_Error_N.ToString("0.00") + "%");
                    _datastatisticsfrm.UpDataDGV_N(17, 1, Global.inidata.productconfig.Smallmaterial_throwing_N);
                    double Smallmaterial_throwing_N = (Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_throwing_N) / Convert.ToDouble(Global.inidata.productconfig.Smallmaterial_Input_N)) * 100;
                    _datastatisticsfrm.UpDataDGV_N(17, 2, Smallmaterial_throwing_N.ToString("0.00") + "%");
                }
                else
                {
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_07_08");

                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_20_21");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_21_22");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_22_23");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_23_00");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_00_01");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_01_02");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_02_03");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_03_04");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_04_05");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_05_06");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_06_07");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_Total_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_Product_NG_20_08");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_Product_Lianglv_20_08");

                    ///20210814
                    /// 
                    _datastatisticsfrm.UpDatalabel("0.00%", "NightJd");

                    _datastatisticsfrm.UpDataDGV_N(0, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(1, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(4, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(4, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(5, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(5, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(6, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(6, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(7, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(7, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(8, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(8, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(9, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(9, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(12, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(12, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(13, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(13, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(14, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(14, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(15, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(15, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(16, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(16, 2, "0.00%");
                    _datastatisticsfrm.UpDataDGV_N(17, 1, "0");
                    _datastatisticsfrm.UpDataDGV_N(17, 2, "0.00%");
                }

            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            ClearMemory();
        }
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //FrmMain为我窗体的类名
                MainFrm.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion
        private void DT_DataStatistics()//运行状态统计
        {
            //-------------------------------白班DT统计--------------------------------------------
            Global.DT_RunTime = Global.PLC_Client.ReadPLC_D(2156, 12);//白班运行时间
            Global.DT_ErrorTime = Global.PLC_Client.ReadPLC_D(2206, 12);//白班异常时间
            Global.DT_PendingTime = Global.PLC_Client.ReadPLC_D(2256, 12);//白班待料时间
            short DT_RunTime_08_09 = Global.DT_RunTime[0];//白班8-9点运行时间
            short DT_RunTime_09_10 = Global.DT_RunTime[1];
            short DT_RunTime_10_11 = Global.DT_RunTime[2];
            short DT_RunTime_11_12 = Global.DT_RunTime[3];
            short DT_RunTime_12_13 = Global.DT_RunTime[4];
            short DT_RunTime_13_14 = Global.DT_RunTime[5];
            short DT_RunTime_14_15 = Global.DT_RunTime[6];
            short DT_RunTime_15_16 = Global.DT_RunTime[7];
            short DT_RunTime_16_17 = Global.DT_RunTime[8];
            short DT_RunTime_17_18 = Global.DT_RunTime[9];
            short DT_RunTime_18_19 = Global.DT_RunTime[10];
            short DT_RunTime_19_20 = Global.DT_RunTime[11];//白班19-20点运行时间
            short DT_ErrorTime_08_09 = Global.DT_ErrorTime[0];//白班8-9点异常时间
            short DT_ErrorTime_09_10 = Global.DT_ErrorTime[1];
            short DT_ErrorTime_10_11 = Global.DT_ErrorTime[2];
            short DT_ErrorTime_11_12 = Global.DT_ErrorTime[3];
            short DT_ErrorTime_12_13 = Global.DT_ErrorTime[4];
            short DT_ErrorTime_13_14 = Global.DT_ErrorTime[5];
            short DT_ErrorTime_14_15 = Global.DT_ErrorTime[6];
            short DT_ErrorTime_15_16 = Global.DT_ErrorTime[7];
            short DT_ErrorTime_16_17 = Global.DT_ErrorTime[8];
            short DT_ErrorTime_17_18 = Global.DT_ErrorTime[9];
            short DT_ErrorTime_18_19 = Global.DT_ErrorTime[10];
            short DT_ErrorTime_19_20 = Global.DT_ErrorTime[11];//白班19-20点异常时间
            short DT_PendingTime_08_09 = Global.DT_PendingTime[0];//白班8-9点待料时间
            short DT_PendingTime_09_10 = Global.DT_PendingTime[1];
            short DT_PendingTime_10_11 = Global.DT_PendingTime[2];
            short DT_PendingTime_11_12 = Global.DT_PendingTime[3];
            short DT_PendingTime_12_13 = Global.DT_PendingTime[4];
            short DT_PendingTime_13_14 = Global.DT_PendingTime[5];
            short DT_PendingTime_14_15 = Global.DT_PendingTime[6];
            short DT_PendingTime_15_16 = Global.DT_PendingTime[7];
            short DT_PendingTime_16_17 = Global.DT_PendingTime[8];
            short DT_PendingTime_17_18 = Global.DT_PendingTime[9];
            short DT_PendingTime_18_19 = Global.DT_PendingTime[10];
            short DT_PendingTime_19_20 = Global.DT_PendingTime[11];//白班19-20点待料时间
            short DT_RunTime_08_20 = Global.PLC_Client.ReadPLC_D(2176, 1)[0];//白班总运行时间
            short DT_ErrorTime_08_20 = Global.PLC_Client.ReadPLC_D(2226, 1)[0];//白班总异常时间
            short DT_PendingTime_08_20 = Global.PLC_Client.ReadPLC_D(2276, 1)[0];//白班总待料时间
            //short DT_jiadonglv_08_20 = Global.PLC_Client.ReadPLC_D(4224, 1)[0];//白班稼动率时间
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDTTime.ToString("yyyy-MM-dd"))
            {
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_08_09)).ToString("0.00"), "lb_RunTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_09_10)).ToString("0.00"), "lb_RunTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_10_11)).ToString("0.00"), "lb_RunTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_11_12)).ToString("0.00"), "lb_RunTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_12_13)).ToString("0.00"), "lb_RunTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_13_14)).ToString("0.00"), "lb_RunTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_14_15)).ToString("0.00"), "lb_RunTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_15_16)).ToString("0.00"), "lb_RunTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_16_17)).ToString("0.00"), "lb_RunTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_17_18)).ToString("0.00"), "lb_RunTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_18_19)).ToString("0.00"), "lb_RunTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_19_20)).ToString("0.00"), "lb_RunTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_08_09)).ToString("0.00"), "lb_ErrorTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_09_10)).ToString("0.00"), "lb_ErrorTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_10_11)).ToString("0.00"), "lb_ErrorTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_11_12)).ToString("0.00"), "lb_ErrorTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_12_13)).ToString("0.00"), "lb_ErrorTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_13_14)).ToString("0.00"), "lb_ErrorTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_14_15)).ToString("0.00"), "lb_ErrorTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_15_16)).ToString("0.00"), "lb_ErrorTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_16_17)).ToString("0.00"), "lb_ErrorTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_17_18)).ToString("0.00"), "lb_ErrorTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_18_19)).ToString("0.00"), "lb_ErrorTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_19_20)).ToString("0.00"), "lb_ErrorTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_08_09)).ToString("0.00"), "lb_PendingTime_08_09");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_09_10)).ToString("0.00"), "lb_PendingTime_09_10");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_10_11)).ToString("0.00"), "lb_PendingTime_10_11");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_11_12)).ToString("0.00"), "lb_PendingTime_11_12");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_12_13)).ToString("0.00"), "lb_PendingTime_12_13");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_13_14)).ToString("0.00"), "lb_PendingTime_13_14");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_14_15)).ToString("0.00"), "lb_PendingTime_14_15");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_15_16)).ToString("0.00"), "lb_PendingTime_15_16");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_16_17)).ToString("0.00"), "lb_PendingTime_16_17");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_17_18)).ToString("0.00"), "lb_PendingTime_17_18");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_18_19)).ToString("0.00"), "lb_PendingTime_18_19");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_19_20)).ToString("0.00"), "lb_PendingTime_19_20");

                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_08_20)).ToString("0.00"), "lb_RunTime_08_20");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_08_20)).ToString("0.00"), "lb_ErrorTime_08_20");
                _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_08_20)).ToString("0.00"), "lb_PendingTime_08_20");

                ///20210814
                /// 
                _datastatisticsfrm.UpDatalabel(((DT_RunTime_08_20 / (DT_RunTime_08_20 + DT_ErrorTime_08_20 + DT_PendingTime_08_20)) * 100).ToString("0.00") + "%", "DayTime");
                //_datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_jiadonglv_08_20) / 100).ToString("0.00") + "%", "lb_jiadonglv_08_20");
            }
            //-------------------------------夜班DT统计--------------------------------------------
            Global.DT_RunTime_N1 = Global.PLC_Client.ReadPLC_D(2168, 6);//夜班运行时间
            Global.DT_RunTime_N2 = Global.PLC_Client.ReadPLC_D(2150, 6);//夜班运行时间
            Global.DT_ErrorTime_N1 = Global.PLC_Client.ReadPLC_D(2218, 6);//夜班异常时间
            Global.DT_ErrorTime_N2 = Global.PLC_Client.ReadPLC_D(2200, 6);//夜班异常时间
            Global.DT_PendingTime_N1 = Global.PLC_Client.ReadPLC_D(2268, 6);//夜班待料时间
            Global.DT_PendingTime_N2 = Global.PLC_Client.ReadPLC_D(2250, 6);//夜班待料时间
            short DT_RunTime_20_21 = Global.DT_RunTime_N1[0];
            short DT_RunTime_21_22 = Global.DT_RunTime_N1[1];
            short DT_RunTime_22_23 = Global.DT_RunTime_N1[2];
            short DT_RunTime_23_00 = Global.DT_RunTime_N1[3];
            short DT_RunTime_00_01 = Global.DT_RunTime_N1[4];
            short DT_RunTime_01_02 = Global.DT_RunTime_N1[5];
            short DT_RunTime_02_03 = Global.DT_RunTime_N2[0];
            short DT_RunTime_03_04 = Global.DT_RunTime_N2[1];
            short DT_RunTime_04_05 = Global.DT_RunTime_N2[2];
            short DT_RunTime_05_06 = Global.DT_RunTime_N2[3];
            short DT_RunTime_06_07 = Global.DT_RunTime_N2[4];
            short DT_RunTime_07_08 = Global.DT_RunTime_N2[5];
            short DT_ErrorTime_20_21 = Global.DT_ErrorTime_N1[0];
            short DT_ErrorTime_21_22 = Global.DT_ErrorTime_N1[1];
            short DT_ErrorTime_22_23 = Global.DT_ErrorTime_N1[2];
            short DT_ErrorTime_23_00 = Global.DT_ErrorTime_N1[3];
            short DT_ErrorTime_00_01 = Global.DT_ErrorTime_N1[4];
            short DT_ErrorTime_01_02 = Global.DT_ErrorTime_N1[5];
            short DT_ErrorTime_02_03 = Global.DT_ErrorTime_N2[0];
            short DT_ErrorTime_03_04 = Global.DT_ErrorTime_N2[1];
            short DT_ErrorTime_04_05 = Global.DT_ErrorTime_N2[2];
            short DT_ErrorTime_05_06 = Global.DT_ErrorTime_N2[3];
            short DT_ErrorTime_06_07 = Global.DT_ErrorTime_N2[4];
            short DT_ErrorTime_07_08 = Global.DT_ErrorTime_N2[5];
            short DT_PendingTime_20_21 = Global.DT_PendingTime_N1[0];
            short DT_PendingTime_21_22 = Global.DT_PendingTime_N1[1];
            short DT_PendingTime_22_23 = Global.DT_PendingTime_N1[2];
            short DT_PendingTime_23_00 = Global.DT_PendingTime_N1[3];
            short DT_PendingTime_00_01 = Global.DT_PendingTime_N1[4];
            short DT_PendingTime_01_02 = Global.DT_PendingTime_N1[5];
            short DT_PendingTime_02_03 = Global.DT_PendingTime_N2[0];
            short DT_PendingTime_03_04 = Global.DT_PendingTime_N2[1];
            short DT_PendingTime_04_05 = Global.DT_PendingTime_N2[2];
            short DT_PendingTime_05_06 = Global.DT_PendingTime_N2[3];
            short DT_PendingTime_06_07 = Global.DT_PendingTime_N2[4];
            short DT_PendingTime_07_08 = Global.DT_PendingTime_N2[5];
            short DT_RunTime_20_08 = Global.PLC_Client.ReadPLC_D(2178, 1)[0];//夜班总运行时间
            short DT_ErrorTime_20_08 = Global.PLC_Client.ReadPLC_D(2228, 1)[0];//夜班总异常时间
            short DT_PendingTime_20_08 = Global.PLC_Client.ReadPLC_D(2278, 1)[0];//夜班总待料时间
            //short DT_jiadonglv_20_08 = Global.PLC_Client.ReadPLC_D(4254, 1)[0];//夜班稼动率时间
            if (DateTime.Now.ToString("yyyy-MM-dd") == Global.SelectDTTime.ToString("yyyy-MM-dd"))
            {
                if (Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("18:00")) >= 0 || Convert.ToDateTime(DateTime.Now.ToString("HH:mm")).CompareTo(Convert.ToDateTime("6:00")) < 0)
                {
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_20_21)).ToString("0.00"), "lb_RunTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_21_22)).ToString("0.00"), "lb_RunTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_22_23)).ToString("0.00"), "lb_RunTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_23_00)).ToString("0.00"), "lb_RunTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_00_01)).ToString("0.00"), "lb_RunTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_01_02)).ToString("0.00"), "lb_RunTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_02_03)).ToString("0.00"), "lb_RunTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_03_04)).ToString("0.00"), "lb_RunTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_04_05)).ToString("0.00"), "lb_RunTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_05_06)).ToString("0.00"), "lb_RunTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_06_07)).ToString("0.00"), "lb_RunTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_07_08)).ToString("0.00"), "lb_RunTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_20_21)).ToString("0.00"), "lb_ErrorTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_21_22)).ToString("0.00"), "lb_ErrorTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_22_23)).ToString("0.00"), "lb_ErrorTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_23_00)).ToString("0.00"), "lb_ErrorTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_00_01)).ToString("0.00"), "lb_ErrorTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_01_02)).ToString("0.00"), "lb_ErrorTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_02_03)).ToString("0.00"), "lb_ErrorTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_03_04)).ToString("0.00"), "lb_ErrorTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_04_05)).ToString("0.00"), "lb_ErrorTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_05_06)).ToString("0.00"), "lb_ErrorTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_06_07)).ToString("0.00"), "lb_ErrorTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_07_08)).ToString("0.00"), "lb_ErrorTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_20_21)).ToString("0.00"), "lb_PendingTime_20_21");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_21_22)).ToString("0.00"), "lb_PendingTime_21_22");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_22_23)).ToString("0.00"), "lb_PendingTime_22_23");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_23_00)).ToString("0.00"), "lb_PendingTime_23_00");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_00_01)).ToString("0.00"), "lb_PendingTime_00_01");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_01_02)).ToString("0.00"), "lb_PendingTime_01_02");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_02_03)).ToString("0.00"), "lb_PendingTime_02_03");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_03_04)).ToString("0.00"), "lb_PendingTime_03_04");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_04_05)).ToString("0.00"), "lb_PendingTime_04_05");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_05_06)).ToString("0.00"), "lb_PendingTime_05_06");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_06_07)).ToString("0.00"), "lb_PendingTime_06_07");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_07_08)).ToString("0.00"), "lb_PendingTime_07_08");

                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_RunTime_20_08)).ToString("0.00"), "lb_RunTime_20_08");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_ErrorTime_20_08)).ToString("0.00"), "lb_ErrorTime_20_08");
                    _datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_PendingTime_20_08)).ToString("0.00"), "lb_PendingTime_20_08");

                    ///20210814
                    /// 
                    _datastatisticsfrm.UpDatalabel(((DT_RunTime_20_08 / (DT_RunTime_20_08 + DT_ErrorTime_20_08 + DT_PendingTime_20_08)) * 100).ToString("0.00") + "%", "NightTime");
                    //_datastatisticsfrm.UpDatalabel((Convert.ToDouble(DT_jiadonglv_20_08) / 100).ToString("0.00") + "%", "lb_jiadonglv_20_08");
                }
                else
                {
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_20_21");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_21_22");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_22_23");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_23_00");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_00_01");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_01_02");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_02_03");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_03_04");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_04_05");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_05_06");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_06_07");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_07_08");

                    _datastatisticsfrm.UpDatalabel("0", "lb_RunTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_ErrorTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0", "lb_PendingTime_20_08");
                    _datastatisticsfrm.UpDatalabel("0.00%", "lb_jiadonglv_20_08");
                }
            }
        }

        /// <summary>
        /// 生成MD5码版本号：读取目前软件执行档然后产生MD5码，作为软件版本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private string GetFileMd5Code(string filePath)
        {
            StringBuilder builder = new StringBuilder();
            if (filePath != "")
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    File.Copy(filePath, filePath + "e");//复制一份，防止占用
                    //利用复制的执行档建立MD5码
                    using (FileStream fs = new FileStream(filePath + "e", FileMode.Open))
                    {
                        byte[] bt = md5.ComputeHash(fs);
                        for (int i = 0; i < bt.Length; i++)
                        {
                            builder.Append(bt[i].ToString("x2"));
                        }
                    }
                    File.Delete(filePath + "e");
                }
            }
            return builder.ToString();

        }
    }
}
