using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace 卓汇数据追溯系统
{
    public class ProductConfig
    {

        #region 系统参数
        public string Plc_IP { get; set; }
        public string Plc_Port { get; set; }
        public string Plc_Port2 { get; set; }
        public string PDCA_UA_IP { get; set; }
        public string PDCA_UA_Port { get; set; }
        public string PDCA_LA_IP { get; set; }
        public string PDCA_LA_Port { get; set; }
        public string Barcode_IP { get; set; }
        public string Barcode_Port { get; set; }
        public string Hans_IP { get; set; }
        public string Hans_Port { get; set; }
        public string Trace_IP { get; set; }
        public string Trace_Port { get; set; }
        public string OEE_IP { get; set; }
        public string OEE_Port { get; set; }
        public string Precitec_IP { get; set; }
        public string Precitec_Port { get; set; }
        public string product_num_ok { get; set; }
        public string product_num_ng { get; set; }
        public string theory_product { get; set; }
        public string product_num_ua_ok { get; set; }
        public string product_num_ua_ng { get; set; }
        public string product_num_la_ok { get; set; }
        public string product_num_la_ng { get; set; }
        public string product_num_process_ok { get; set; }
        public string product_num_process_ng { get; set; }
        public string product_num_mes_ok { get; set; }
        public string product_num_mes_ng { get; set; }
        public string ThrowCount { get; set; }
        public string ThrowOKCount { get; set; }
        public string TotalThrowCount { get; set; }
        public string NutCount { get; set; }
        public string NutOKCount { get; set; }
        public string UACount { get; set; }
        public string LACount { get; set; }
        public string fixture_ok { get; set; }
        public string fixture_ng { get; set; }
        public string UA_OK_Count { get; set; }
        public string LA_OK_Count { get; set; }
        public string oee_fixture_ok { get; set; }
        public string oee_fixture_ng { get; set; }
        public string oee_ok { get; set; }
        public string oee_ng { get; set; }
        public string trace_ua_ok { get; set; }
        public string trace_ua_ng { get; set; }
        public string trace_la_ok { get; set; }
        public string trace_la_ng { get; set; }
        public string Product_Total_D { get; set; }
        public string Product_Total_N { get; set; }
        public string Product_OK_D { get; set; }
        public string Product_OK_N { get; set; }
        public string TraceUpLoad_Error_D { get; set; }
        public string TraceUpLoad_Error_N { get; set; }
        public string PDCAUpLoad_Error_D { get; set; }
        public string PDCAUpLoad_Error_N { get; set; }
        public string TracePVCheck_Error_D { get; set; }
        public string TracePVCheck_Error_N { get; set; }
        public string OEEUpLoad_Error_D { get; set; }
        public string OEEUpLoad_Error_N { get; set; }
        public string ReadBarcode_NG_N { get; set; }
        public string ReadBarcode_NG_D { get; set; }
        public string CCDCheck_Error_D { get; set; }
        public string CCDCheck_Error_N { get; set; }
        public string Welding_Error_D { get; set; }
        public string Welding_Error_N { get; set; }
        public string Smallmaterial_Input_D { get; set; }
        public string Smallmaterial_Input_N { get; set; }
        public string Smallmaterial_throwing_D { get; set; }
        public string Smallmaterial_throwing_N { get; set; }
        public string Location1_NG_D { get; set; }
        public string Location1_NG_N { get; set; }
        public string Location2_NG_D { get; set; }
        public string Location2_NG_N { get; set; }
        public string Location3_NG_D { get; set; }
        public string Location3_NG_N { get; set; }
        public string Location4_NG_D { get; set; }
        public string Location4_NG_N { get; set; }
        public string Location5_NG_D { get; set; }
        public string Location5_NG_N { get; set; }
        public string jgp_url { get; set; }
        public string jgp_online { get; set; }
        public string error_online { get; set; }
        public string trace_online { get; set; }
        public string process_online { get; set; }
        public string mes_online { get; set; }
        public string uploadpic_online { get; set; }
        public string IFactory_online { get; set; }
        public string code { get; set; }
        public string Threshold { get; set; }
        public string delete_time { get; set; }
        public string FixtureNumber { get; set; }
        public string MaxCT { get; set; }
        public string CT { get; set; }
        /// <summary>
        /// 0909
        /// </summary>
        public string EMT { set; get; }
        public string MAC { set; get; }
        public string IP { set; get; }

        public string HostName { set; get; }
        public string txtMQTTUserName { set; get; }
        public string txtMQTTPassword { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ICCom { get; set; }
        //MAC_MINI配置信息
        public string UA_site { get; set; }
        public string UA_product { get; set; }
        public string UA_station_type { get; set; }
        public string UA_location { get; set; }
        public string UA_line_number { get; set; }
        public string UA_station_number { get; set; }
        public string LA_site { get; set; }
        public string LA_product { get; set; }
        public string LA_station_type { get; set; }
        public string LA_location { get; set; }
        public string LA_line_number { get; set; }
        public string LA_station_number { get; set; }

        //BAil属性
        public string Head { get; set; }
        public string Test_head_id_la { get; set; }
        public string Test_head_id_ua { get; set; }
        //public string Swversion{ get; set; }
        public string Sw_version { get; set; }
        public string Station_id_ua { get; set; }
        public string Station_id_bt { get; set; }
        public string Line_id_ua { get; set; }
        public string Line_id_bt { get; set; }
        public string Line_type { get; set; }
        public string machine_id { get; set; }
        public string Air_pressure { get; set; }
        public string Sw_name_ua { get; set; }
        public string Sw_name_la { get; set; }
        public string TraceCheckParamURL { get; set; }
        public string Process_UA { get; set; }
        public string Process_LA { get; set; }
        public string TraceCheckParam_Online { get; set; }
        //Jgp属性
        public string Trace_Logs_UA { get; set; }
        public string Trace_Logs_LA { get; set; }
        public string Trace_CheakSN_UA { get; set; }
        public string Trace_CheakSN_Bracket { get; set; }
        public string OEE_URL1 { get; set; }
        public string OEE_URL2 { get; set; }
        public string OEE_Dsn { get; set; }
        public string OEE_authCode { get; set; }
        public string MES_URL1 { get; set; }
        public string MES_URL2 { get; set; }
        public string Headers_key { get; set; }
        public string Headers_value { get; set; }
        public string PIS_URL { get; set; }
        //public string SwVersion{ get; set; }
        //各级权限密码
        public string Operator_pwd { get; set; }
        public string Technician_pwd { get; set; }
        public string Administrator_pwd { get; set; }
        //CCD权限等级
        //public string Level{ get; set; }
        #endregion

    }

    public class IniProductFile
    {
        #region 初始化
        private string _path;
        private IniOperation _iniOperation;
        private ProductConfig _productconfig;

        #endregion

        #region Property
        public ProductConfig productconfig
        {
            get { return _productconfig; }
            set { _productconfig = value; }
        }

        #endregion

        public IniProductFile(string path)
        {
            this._path = path;
            _iniOperation = new IniOperation(_path);
            _productconfig = new ProductConfig();
            ReadProductConfigSection();

        }

        public void ReadProductConfigSection()
        {
            string sectionName = "ProductConfig";
            _productconfig.Plc_IP = _iniOperation.ReadValue(sectionName, "PLC_IP");
            _productconfig.Plc_Port = _iniOperation.ReadValue(sectionName, "PLC_PORT");
            _productconfig.Plc_Port2 = _iniOperation.ReadValue(sectionName, "PLC_PORT2");
            _productconfig.PDCA_UA_IP = _iniOperation.ReadValue(sectionName, "PDCA_UA_IP");
            _productconfig.PDCA_UA_Port = _iniOperation.ReadValue(sectionName, "PDCA_UA_Port");
            _productconfig.PDCA_LA_IP = _iniOperation.ReadValue(sectionName, "PDCA_LA_IP");
            _productconfig.PDCA_LA_Port = _iniOperation.ReadValue(sectionName, "PDCA_LA_Port");
            _productconfig.Barcode_IP = _iniOperation.ReadValue(sectionName, "Barcode_IP");
            _productconfig.Barcode_Port = _iniOperation.ReadValue(sectionName, "Barcode_Port");
            _productconfig.Hans_IP = _iniOperation.ReadValue(sectionName, "Hans_IP");
            _productconfig.Hans_Port = _iniOperation.ReadValue(sectionName, "Hans_Port");
            _productconfig.Trace_IP = _iniOperation.ReadValue(sectionName, "Trace_IP");
            _productconfig.Trace_Port = _iniOperation.ReadValue(sectionName, "Trace_Port");
            _productconfig.OEE_IP = _iniOperation.ReadValue(sectionName, "OEE_IP");
            _productconfig.OEE_Port = _iniOperation.ReadValue(sectionName, "OEE_Port");
            _productconfig.Precitec_IP = _iniOperation.ReadValue(sectionName, "Precitec_IP");
            _productconfig.Precitec_Port = _iniOperation.ReadValue(sectionName, "Precitec_Port");
            _productconfig.jgp_url = _iniOperation.ReadValue(sectionName, "JGP_URL");
            _productconfig.jgp_online = _iniOperation.ReadValue(sectionName, "JGP_Online");
            _productconfig.error_online = _iniOperation.ReadValue(sectionName, "DownTime_Online");
            _productconfig.trace_online = _iniOperation.ReadValue(sectionName, "Trace_Online");
            _productconfig.process_online = _iniOperation.ReadValue(sectionName, "ProcessControl_Online");
            _productconfig.IFactory_online = _iniOperation.ReadValue(sectionName, "IFactory_online");
            _productconfig.mes_online = _iniOperation.ReadValue(sectionName, "MES_Online");
            _productconfig.uploadpic_online = _iniOperation.ReadValue(sectionName, "UpLoadPic_Online");
            _productconfig.code = _iniOperation.ReadValue(sectionName, "Code");
            _productconfig.product_num_ok = _iniOperation.ReadValue(sectionName, "product_num_ok");
            _productconfig.product_num_ng = _iniOperation.ReadValue(sectionName, "product_num_ng");
            _productconfig.product_num_ua_ok = _iniOperation.ReadValue(sectionName, "product_num_ua_ok");
            _productconfig.product_num_ua_ng = _iniOperation.ReadValue(sectionName, "product_num_ua_ng");
            _productconfig.product_num_la_ok = _iniOperation.ReadValue(sectionName, "product_num_la_ok");
            _productconfig.product_num_la_ng = _iniOperation.ReadValue(sectionName, "product_num_la_ng");
            _productconfig.product_num_process_ok = _iniOperation.ReadValue(sectionName, "product_num_process_ok");
            _productconfig.product_num_process_ng = _iniOperation.ReadValue(sectionName, "product_num_process_ng");
            _productconfig.product_num_mes_ok = _iniOperation.ReadValue(sectionName, "product_num_mes_ok");
            _productconfig.product_num_mes_ng = _iniOperation.ReadValue(sectionName, "product_num_mes_ng");
            _productconfig.theory_product = _iniOperation.ReadValue(sectionName, "theory_product");
            _productconfig.oee_fixture_ok = _iniOperation.ReadValue(sectionName, "oee_fixture_ok");
            _productconfig.oee_fixture_ng = _iniOperation.ReadValue(sectionName, "oee_fixture_ng");
            _productconfig.oee_ok = _iniOperation.ReadValue(sectionName, "oee_ok");
            _productconfig.oee_ng = _iniOperation.ReadValue(sectionName, "oee_ng");
            _productconfig.trace_ua_ok = _iniOperation.ReadValue(sectionName, "trace_ua_ok");
            _productconfig.trace_ua_ng = _iniOperation.ReadValue(sectionName, "trace_ua_ng");
            _productconfig.trace_la_ok = _iniOperation.ReadValue(sectionName, "trace_la_ok");
            _productconfig.trace_la_ng = _iniOperation.ReadValue(sectionName, "trace_la_ng");
            _productconfig.fixture_ok = _iniOperation.ReadValue(sectionName, "fixture_ok");
            _productconfig.fixture_ng = _iniOperation.ReadValue(sectionName, "fixture_ng");
            _productconfig.Threshold = _iniOperation.ReadValue(sectionName, "threshold");
            _productconfig.delete_time = _iniOperation.ReadValue(sectionName, "delete_time");
            _productconfig.UA_site = _iniOperation.ReadValue(sectionName, "UA_site");
            _productconfig.UA_product = _iniOperation.ReadValue(sectionName, "UA_product");
            _productconfig.UA_station_type = _iniOperation.ReadValue(sectionName, "UA_station_type");
            _productconfig.UA_location = _iniOperation.ReadValue(sectionName, "UA_location");
            _productconfig.UA_line_number = _iniOperation.ReadValue(sectionName, "UA_line_number");
            _productconfig.UA_station_number = _iniOperation.ReadValue(sectionName, "UA_station_number");
            _productconfig.LA_site = _iniOperation.ReadValue(sectionName, "LA_site");
            _productconfig.LA_product = _iniOperation.ReadValue(sectionName, "LA_product");
            _productconfig.LA_station_type = _iniOperation.ReadValue(sectionName, "LA_station_type");
            _productconfig.LA_location = _iniOperation.ReadValue(sectionName, "LA_location");
            _productconfig.LA_line_number = _iniOperation.ReadValue(sectionName, "LA_line_number");
            _productconfig.LA_station_number = _iniOperation.ReadValue(sectionName, "LA_station_number");
            _productconfig.FixtureNumber = _iniOperation.ReadValue(sectionName, "FixtureNumber");
            _productconfig.ThrowCount = _iniOperation.ReadValue(sectionName, "ThrowCount");
            _productconfig.ThrowOKCount = _iniOperation.ReadValue(sectionName, "ThrowOKCount");
            _productconfig.TotalThrowCount = _iniOperation.ReadValue(sectionName, "TotalThrowCount");
            _productconfig.NutCount = _iniOperation.ReadValue(sectionName, "NutCount");
            _productconfig.NutOKCount = _iniOperation.ReadValue(sectionName, "NutOKCount");
            _productconfig.UACount = _iniOperation.ReadValue(sectionName, "UACount");
            _productconfig.LACount = _iniOperation.ReadValue(sectionName, "LACount");
            _productconfig.UA_OK_Count = _iniOperation.ReadValue(sectionName, "UA_OK_Count");
            _productconfig.LA_OK_Count = _iniOperation.ReadValue(sectionName, "LA_OK_Count");
            _productconfig.MaxCT = _iniOperation.ReadValue(sectionName, "MaxCT");
            _productconfig.CT = _iniOperation.ReadValue(sectionName, "CT");

            _productconfig.Product_Total_D = _iniOperation.ReadValue(sectionName, "Product_Total_D");
            _productconfig.Product_Total_N = _iniOperation.ReadValue(sectionName, "Product_Total_N");
            _productconfig.Product_OK_D = _iniOperation.ReadValue(sectionName, "Product_OK_D");
            _productconfig.Product_OK_N = _iniOperation.ReadValue(sectionName, "Product_OK_N");
            _productconfig.TraceUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "TraceUpLoad_Error_D");
            _productconfig.TraceUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "TraceUpLoad_Error_N");
            _productconfig.PDCAUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "PDCAUpLoad_Error_D");
            _productconfig.PDCAUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "PDCAUpLoad_Error_N");
            _productconfig.TracePVCheck_Error_D = _iniOperation.ReadValue(sectionName, "TracePVCheck_Error_D");
            _productconfig.TracePVCheck_Error_N = _iniOperation.ReadValue(sectionName, "TracePVCheck_Error_N");
            _productconfig.OEEUpLoad_Error_D = _iniOperation.ReadValue(sectionName, "OEEUpLoad_Error_D");
            _productconfig.OEEUpLoad_Error_N = _iniOperation.ReadValue(sectionName, "OEEUpLoad_Error_N");
            _productconfig.ReadBarcode_NG_D = _iniOperation.ReadValue(sectionName, "ReadBarcode_NG_D");
            _productconfig.ReadBarcode_NG_N = _iniOperation.ReadValue(sectionName, "ReadBarcode_NG_N");
            _productconfig.CCDCheck_Error_D = _iniOperation.ReadValue(sectionName, "CCDCheck_Error_D");
            _productconfig.CCDCheck_Error_N = _iniOperation.ReadValue(sectionName, "CCDCheck_Error_N");
            _productconfig.Welding_Error_D = _iniOperation.ReadValue(sectionName, "Welding_Error_D");
            _productconfig.Welding_Error_N = _iniOperation.ReadValue(sectionName, "Welding_Error_N");
            _productconfig.Smallmaterial_Input_D = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_D");
            _productconfig.Smallmaterial_Input_N = _iniOperation.ReadValue(sectionName, "Smallmaterial_Input_N");
            _productconfig.Smallmaterial_throwing_D = _iniOperation.ReadValue(sectionName, "Smallmaterial_throwing_D");
            _productconfig.Smallmaterial_throwing_N = _iniOperation.ReadValue(sectionName, "Smallmaterial_throwing_N");
            _productconfig.Location1_NG_D = _iniOperation.ReadValue(sectionName, "Location1_NG_D");
            _productconfig.Location1_NG_N = _iniOperation.ReadValue(sectionName, "Location1_NG_N");
            _productconfig.Location2_NG_D = _iniOperation.ReadValue(sectionName, "Location2_NG_D");
            _productconfig.Location2_NG_N = _iniOperation.ReadValue(sectionName, "Location2_NG_N");
            _productconfig.Location3_NG_D = _iniOperation.ReadValue(sectionName, "Location3_NG_D");
            _productconfig.Location3_NG_N = _iniOperation.ReadValue(sectionName, "Location3_NG_N");
            _productconfig.Location4_NG_D = _iniOperation.ReadValue(sectionName, "Location4_NG_D");
            _productconfig.Location4_NG_N = _iniOperation.ReadValue(sectionName, "Location4_NG_N");
            _productconfig.Location5_NG_D = _iniOperation.ReadValue(sectionName, "Location5_NG_D");
            _productconfig.Location5_NG_N = _iniOperation.ReadValue(sectionName, "Location5_NG_N");


            _productconfig.ICCom= _iniOperation.ReadValue(sectionName, "ICCom");
            ///0909
            /// 
            _productconfig.EMT = _iniOperation.ReadValue(sectionName, "EMT");
            _productconfig.MAC = _iniOperation.ReadValue(sectionName, "MAC");
            _productconfig.IP = _iniOperation.ReadValue(sectionName, "IP");
            _productconfig.HostName = _iniOperation.ReadValue(sectionName, "HostName");
            _productconfig.txtMQTTUserName = _iniOperation.ReadValue(sectionName, "txtMQTTUserName");
            _productconfig.txtMQTTPassword = _iniOperation.ReadValue(sectionName, "txtMQTTPassword");


            string sectionName1 = "BailConfig";
            _productconfig.Head = _iniOperation.ReadValue(sectionName1, "head");
            _productconfig.Test_head_id_la = _iniOperation.ReadValue(sectionName1, "test_head_id_la");
            _productconfig.Test_head_id_ua = _iniOperation.ReadValue(sectionName1, "test_head_id_ua");
            //_productconfig.Swversion = _iniOperation.ReadValue(sectionName1, "Swversion");
            _productconfig.Station_id_ua = _iniOperation.ReadValue(sectionName1, "station_id_ua");
            _productconfig.Station_id_bt = _iniOperation.ReadValue(sectionName1, "station_id_bt");
            _productconfig.Line_id_ua = _iniOperation.ReadValue(sectionName1, "Line_id_ua");
            _productconfig.Line_id_bt = _iniOperation.ReadValue(sectionName1, "Line_id_bt");
            _productconfig.Line_type = _iniOperation.ReadValue(sectionName1, "line_type");
            _productconfig.machine_id = _iniOperation.ReadValue(sectionName1, "machine_id");
            _productconfig.Sw_version = _iniOperation.ReadValue(sectionName1, "sw_version");
            _productconfig.Air_pressure = _iniOperation.ReadValue(sectionName1, "air_pressure");
            _productconfig.Sw_name_ua = _iniOperation.ReadValue(sectionName1, "Sw_name_ua");
            _productconfig.Sw_name_la = _iniOperation.ReadValue(sectionName1, "Sw_name_la");
            _productconfig.TraceCheckParamURL = _iniOperation.ReadValue(sectionName1, "TraceCheckParamURL");
            _productconfig.Process_UA = _iniOperation.ReadValue(sectionName1, "Process_UA");
            _productconfig.Process_LA = _iniOperation.ReadValue(sectionName1, "Process_LA");
            _productconfig.TraceCheckParam_Online = _iniOperation.ReadValue(sectionName1, "TraceCheckParam_Online");
            string sectionName2 = "JgpConfig";
            _productconfig.Trace_Logs_UA = _iniOperation.ReadValue(sectionName2, "Trace_Logs_UA");
            _productconfig.Trace_Logs_LA = _iniOperation.ReadValue(sectionName2, "Trace_Logs_LA");
            _productconfig.Trace_CheakSN_UA = _iniOperation.ReadValue(sectionName2, "Trace_CheakSN_UA");
            _productconfig.Trace_CheakSN_Bracket = _iniOperation.ReadValue(sectionName2, "Trace_CheakSN_Bracket");
            _productconfig.OEE_URL1 = _iniOperation.ReadValue(sectionName2, "OEE_URL1");
            _productconfig.OEE_URL2 = _iniOperation.ReadValue(sectionName2, "OEE_URL2");
            _productconfig.OEE_Dsn = _iniOperation.ReadValue(sectionName2, "OEE_Dsn");
            _productconfig.OEE_authCode = _iniOperation.ReadValue(sectionName2, "OEE_authCode");
            _productconfig.MES_URL1 = _iniOperation.ReadValue(sectionName2, "MES_URL1");
            _productconfig.MES_URL2 = _iniOperation.ReadValue(sectionName2, "MES_URL2");
            _productconfig.Headers_key = _iniOperation.ReadValue(sectionName2, "Headers_key");
            _productconfig.Headers_value = _iniOperation.ReadValue(sectionName2, "Headers_value");
            _productconfig.PIS_URL = _iniOperation.ReadValue(sectionName2, "PIS_URL");
            //_productconfig.SwVersion = _iniOperation.ReadValue(sectionName2, "SwVersion");
            string sectionName3 = "PassWord";
            _productconfig.Operator_pwd = _iniOperation.ReadValue(sectionName3, "Operator_pwd");
            _productconfig.Technician_pwd = _iniOperation.ReadValue(sectionName3, "Technician_pwd");
            _productconfig.Administrator_pwd = _iniOperation.ReadValue(sectionName3, "Administrator_pwd");
        }

        public void WriteProductConfigSection()
        {
            string sectionName = "ProductConfig";
            _iniOperation.WriteValue(sectionName, "PLC_IP", _productconfig.Plc_IP);
            _iniOperation.WriteValue(sectionName, "PLC_PORT", _productconfig.Plc_Port);
            _iniOperation.WriteValue(sectionName, "PLC_PORT2", _productconfig.Plc_Port2);
            _iniOperation.WriteValue(sectionName, "PDCA_UA_IP", _productconfig.PDCA_UA_IP);
            _iniOperation.WriteValue(sectionName, "PDCA_UA_Port", _productconfig.PDCA_UA_Port);
            _iniOperation.WriteValue(sectionName, "PDCA_LA_IP", _productconfig.PDCA_LA_IP);
            _iniOperation.WriteValue(sectionName, "PDCA_LA_Port", _productconfig.PDCA_LA_Port);
            _iniOperation.WriteValue(sectionName, "Barcode_IP", _productconfig.Barcode_IP);
            _iniOperation.WriteValue(sectionName, "Barcode_Port", _productconfig.Barcode_Port);
            _iniOperation.WriteValue(sectionName, "Trace_IP", _productconfig.Trace_IP);
            _iniOperation.WriteValue(sectionName, "Trace_Port", _productconfig.Trace_Port);
            _iniOperation.WriteValue(sectionName, "OEE_IP", _productconfig.OEE_IP);
            _iniOperation.WriteValue(sectionName, "OEE_Port", _productconfig.OEE_Port);
            _iniOperation.WriteValue(sectionName, "Hans_IP", _productconfig.Hans_IP);
            _iniOperation.WriteValue(sectionName, "Hans_Port", _productconfig.Hans_Port);
            _iniOperation.WriteValue(sectionName, "Precitec_IP", _productconfig.Precitec_IP);
            _iniOperation.WriteValue(sectionName, "Precitec_Port", _productconfig.Precitec_Port);
            _iniOperation.WriteValue(sectionName, "JGP_URL", _productconfig.jgp_url);
            _iniOperation.WriteValue(sectionName, "threshold", _productconfig.Threshold);
            _iniOperation.WriteValue(sectionName, "delete_time", _productconfig.delete_time);

            _iniOperation.WriteValue(sectionName, "UA_site", _productconfig.UA_site);
            _iniOperation.WriteValue(sectionName, "UA_product", _productconfig.UA_product);
            _iniOperation.WriteValue(sectionName, "UA_station_type", _productconfig.UA_station_type);
            _iniOperation.WriteValue(sectionName, "UA_location", _productconfig.UA_location);
            _iniOperation.WriteValue(sectionName, "UA_line_number", _productconfig.UA_line_number);
            _iniOperation.WriteValue(sectionName, "UA_station_number", _productconfig.UA_station_number);
            _iniOperation.WriteValue(sectionName, "LA_site", _productconfig.LA_site);
            _iniOperation.WriteValue(sectionName, "LA_product", _productconfig.LA_product);
            _iniOperation.WriteValue(sectionName, "LA_station_type", _productconfig.LA_station_type);
            _iniOperation.WriteValue(sectionName, "LA_location", _productconfig.LA_location);
            _iniOperation.WriteValue(sectionName, "LA_line_number", _productconfig.LA_line_number);
            _iniOperation.WriteValue(sectionName, "LA_station_number", _productconfig.LA_station_number);
            _iniOperation.WriteValue(sectionName, "FixtureNumber", _productconfig.FixtureNumber);

            _iniOperation.WriteValue(sectionName, "JGP_Online", _productconfig.jgp_online);
            _iniOperation.WriteValue(sectionName, "DownTime_Online", _productconfig.error_online);
            _iniOperation.WriteValue(sectionName, "Trace_Online", _productconfig.trace_online);
            _iniOperation.WriteValue(sectionName, "ProcessControl_Online", _productconfig.process_online);
            _iniOperation.WriteValue(sectionName, "MES_Online", _productconfig.mes_online);
            _iniOperation.WriteValue(sectionName, "UpLoadPic_Online", _productconfig.uploadpic_online);
            _iniOperation.WriteValue(sectionName, "IFactory_online", _productconfig.IFactory_online);
            _iniOperation.WriteValue(sectionName, "MaxCT", _productconfig.MaxCT);
            _iniOperation.WriteValue(sectionName, "CT", _productconfig.CT);

            string sectionName1 = "BailConfig";
            _iniOperation.WriteValue(sectionName1, "TraceCheckParamURL", _productconfig.TraceCheckParamURL);
            _iniOperation.WriteValue(sectionName1, "Process_UA", _productconfig.Process_UA);
            _iniOperation.WriteValue(sectionName1, "Process_LA", _productconfig.Process_LA);
            _iniOperation.WriteValue(sectionName1, "Station_id_ua", _productconfig.Station_id_ua);
            _iniOperation.WriteValue(sectionName1, "Station_id_bt", _productconfig.Station_id_bt);
            _iniOperation.WriteValue(sectionName1, "Line_id_ua", _productconfig.Line_id_ua);
            _iniOperation.WriteValue(sectionName1, "Line_id_bt", _productconfig.Line_id_bt);
            _iniOperation.WriteValue(sectionName1, "Sw_name_ua", _productconfig.Sw_name_ua);
            _iniOperation.WriteValue(sectionName1, "Sw_name_la", _productconfig.Sw_name_la);
            _iniOperation.WriteValue(sectionName1, "TraceCheckParam_Online", _productconfig.TraceCheckParam_Online);

            string sectionName2 = "PassWord";
            _iniOperation.WriteValue(sectionName2, "Operator_pwd", _productconfig.Operator_pwd);
            _iniOperation.WriteValue(sectionName2, "Technician_pwd", _productconfig.Technician_pwd);
            _iniOperation.WriteValue(sectionName2, "Administrator_pwd", _productconfig.Administrator_pwd);
        }

        public void WriteProductnumSection()
        {
            string sectionName = "ProductConfig";
            _iniOperation.WriteValue(sectionName, "product_num_ok", _productconfig.product_num_ok);
            _iniOperation.WriteValue(sectionName, "product_num_ng", _productconfig.product_num_ng);
            _iniOperation.WriteValue(sectionName, "theory_product", _productconfig.theory_product);
            _iniOperation.WriteValue(sectionName, "product_num_ua_ok", _productconfig.product_num_ua_ok);
            _iniOperation.WriteValue(sectionName, "product_num_ua_ng", _productconfig.product_num_ua_ng);
            _iniOperation.WriteValue(sectionName, "product_num_la_ok", _productconfig.product_num_la_ok);
            _iniOperation.WriteValue(sectionName, "product_num_la_ng", _productconfig.product_num_la_ng);
            _iniOperation.WriteValue(sectionName, "product_num_process_ok", _productconfig.product_num_process_ok);
            _iniOperation.WriteValue(sectionName, "product_num_process_ng", _productconfig.product_num_process_ng);
            _iniOperation.WriteValue(sectionName, "product_num_mes_ok", _productconfig.product_num_mes_ok);
            _iniOperation.WriteValue(sectionName, "product_num_mes_ng", _productconfig.product_num_mes_ng);
            _iniOperation.WriteValue(sectionName, "oee_fixture_ok", _productconfig.oee_fixture_ok);
            _iniOperation.WriteValue(sectionName, "oee_fixture_ng", _productconfig.oee_fixture_ng);
            _iniOperation.WriteValue(sectionName, "oee_ok", _productconfig.oee_ok);
            _iniOperation.WriteValue(sectionName, "oee_ng", _productconfig.oee_ng);
            _iniOperation.WriteValue(sectionName, "trace_ua_ok", _productconfig.trace_ua_ok);
            _iniOperation.WriteValue(sectionName, "trace_ua_ng", _productconfig.trace_ua_ng);
            _iniOperation.WriteValue(sectionName, "trace_la_ok", _productconfig.trace_la_ok);
            _iniOperation.WriteValue(sectionName, "trace_la_ng", _productconfig.trace_la_ng);
            _iniOperation.WriteValue(sectionName, "ThrowCount", _productconfig.ThrowCount);
            _iniOperation.WriteValue(sectionName, "ThrowOKCount", _productconfig.ThrowOKCount);
            _iniOperation.WriteValue(sectionName, "TotalThrowCount", _productconfig.TotalThrowCount);
            _iniOperation.WriteValue(sectionName, "NutCount", _productconfig.NutCount);
            _iniOperation.WriteValue(sectionName, "NutOKCount", _productconfig.NutOKCount);
            _iniOperation.WriteValue(sectionName, "UACount", _productconfig.UACount);
            _iniOperation.WriteValue(sectionName, "LACount", _productconfig.LACount);
            _iniOperation.WriteValue(sectionName, "UA_OK_Count", _productconfig.UA_OK_Count);
            _iniOperation.WriteValue(sectionName, "LA_OK_Count", _productconfig.LA_OK_Count);
            _iniOperation.WriteValue(sectionName, "fixture_ok", _productconfig.fixture_ok);
            _iniOperation.WriteValue(sectionName, "fixture_ng", _productconfig.fixture_ng);

            _iniOperation.WriteValue(sectionName, "Product_Total_D", _productconfig.Product_Total_D);
            _iniOperation.WriteValue(sectionName, "Product_Total_N", _productconfig.Product_Total_N);
            _iniOperation.WriteValue(sectionName, "Product_OK_D", _productconfig.Product_OK_D);
            _iniOperation.WriteValue(sectionName, "Product_OK_N", _productconfig.Product_OK_N);
            _iniOperation.WriteValue(sectionName, "TraceUpLoad_Error_D", _productconfig.TraceUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "TraceUpLoad_Error_N", _productconfig.TraceUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "PDCAUpLoad_Error_D", _productconfig.PDCAUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "PDCAUpLoad_Error_N", _productconfig.PDCAUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "TracePVCheck_Error_D", _productconfig.TracePVCheck_Error_D);
            _iniOperation.WriteValue(sectionName, "TracePVCheck_Error_N", _productconfig.TracePVCheck_Error_N);
            _iniOperation.WriteValue(sectionName, "OEEUpLoad_Error_D", _productconfig.OEEUpLoad_Error_D);
            _iniOperation.WriteValue(sectionName, "OEEUpLoad_Error_N", _productconfig.OEEUpLoad_Error_N);
            _iniOperation.WriteValue(sectionName, "ReadBarcode_NG_D", _productconfig.ReadBarcode_NG_D);
            _iniOperation.WriteValue(sectionName, "ReadBarcode_NG_N", _productconfig.ReadBarcode_NG_N);
            _iniOperation.WriteValue(sectionName, "CCDCheck_Error_D", _productconfig.CCDCheck_Error_D);
            _iniOperation.WriteValue(sectionName, "CCDCheck_Error_N", _productconfig.CCDCheck_Error_N);
            _iniOperation.WriteValue(sectionName, "Welding_Error_D", _productconfig.Welding_Error_D);
            _iniOperation.WriteValue(sectionName, "Welding_Error_N", _productconfig.Welding_Error_N);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_Input_D", _productconfig.Smallmaterial_Input_D);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_Input_N", _productconfig.Smallmaterial_Input_N);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_throwing_D", _productconfig.Smallmaterial_throwing_D);
            _iniOperation.WriteValue(sectionName, "Smallmaterial_throwing_N", _productconfig.Smallmaterial_throwing_N);
            _iniOperation.WriteValue(sectionName, "Location1_NG_D", _productconfig.Location1_NG_D);
            _iniOperation.WriteValue(sectionName, "Location1_NG_N", _productconfig.Location1_NG_N);
            _iniOperation.WriteValue(sectionName, "Location2_NG_D", _productconfig.Location2_NG_D);
            _iniOperation.WriteValue(sectionName, "Location2_NG_N", _productconfig.Location2_NG_N);
            _iniOperation.WriteValue(sectionName, "Location3_NG_D", _productconfig.Location3_NG_D);
            _iniOperation.WriteValue(sectionName, "Location3_NG_N", _productconfig.Location3_NG_N);
            _iniOperation.WriteValue(sectionName, "Location4_NG_D", _productconfig.Location4_NG_D);
            _iniOperation.WriteValue(sectionName, "Location4_NG_N", _productconfig.Location4_NG_N);
            _iniOperation.WriteValue(sectionName, "Location5_NG_D", _productconfig.Location5_NG_D);
            _iniOperation.WriteValue(sectionName, "Location5_NG_N", _productconfig.Location5_NG_N);
        }
    }
}
