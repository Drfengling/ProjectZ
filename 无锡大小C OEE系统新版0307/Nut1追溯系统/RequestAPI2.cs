using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace 卓汇数据追溯系统
{
    public class RequestAPI2
    {
        public static string strReturnURL = "";
        //public static bool RequestGet(string strURL1, string strURL2, string strIP, string strMac, string dSn, string code, string data, out string ReturnMsg)
        //{
        //    string strReturn = "";
        //    string errMsg = "";
        //    bool bRst = false;
        //    if (string.IsNullOrWhiteSpace(strReturnURL))
        //    {
        //        bRst = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
        //        strReturnURL = strReturn;
        //        if (!bRst)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }
        //    }

        //    var str = strReturnURL.Split(',');
        //    var bRst1 = GetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
        //    if (!bRst1)
        //    {
        //        ReturnMsg = errMsg;
        //        return false;
        //    }
        //    if (strReturn.Substring(0, 1) == "W")
        //    {
        //        var bRst2 = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
        //        if (!bRst2)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }

        //        var str2 = strReturn.Split(',');
        //        var bRstr = GetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
        //        if (!bRstr)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }
        //    }
        //    ReturnMsg = strReturn;
        //    return true;
        //}

        public static bool RequestGet(string strURL1, string strURL2, string strIP, string strMac, string dSn, string code, string data, out string ReturnMsg)
        {
            string strReturn = "";
            string errMsg = "";
            bool bRst = false;
            if (string.IsNullOrWhiteSpace(strReturnURL))
            {
                bRst = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
                strReturnURL = strReturn;
                if (!bRst)
                {
                    ReturnMsg = errMsg;
                    return false;
                }
            }

            var str = strReturnURL.Split(',');
            var bRst1 = GetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
            if (!bRst1)
            {
                ReturnMsg = errMsg;
                return false;
            }
            if (strReturn.Substring(0, 1) == "W")
            {
                var bRst2 = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
                if (!bRst2)
                {
                    ReturnMsg = errMsg;
                    return false;
                }

                var str2 = strReturn.Split(',');
                var bRstr = GetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
                if (!bRstr)
                {
                    ReturnMsg = errMsg;
                    return false;
                }
            }
            ReturnMsg = errMsg;
            return true;
        }

        //public static bool RequestSet(string strURL1, string strURL2, string strIP, string strMac, string dSn, string code, string data, out string ReturnMsg)
        //{
        //    string strReturn = "";
        //    string errMsg = "";
        //    bool bRst = false;
        //    if (string.IsNullOrWhiteSpace(strReturnURL))
        //    {
        //        bRst = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
        //        strReturnURL = strReturn;
        //        if (!bRst)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }
        //    }

        //    var str = strReturnURL.Split(',');
        //    var bRst1 = SetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
        //    if (!bRst1)
        //    {
        //        ReturnMsg = errMsg;
        //        return false;
        //    }
        //    if (strReturn.Substring(0, 1) == "W")
        //    {
        //        var bRst2 = RequireConnection(strURL1, strURL2, strIP, strMac, dSn, code, out strReturn, out errMsg);
        //        if (!bRst2)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }

        //        var str2 = strReturn.Split(',');
        //        var bRstr = SetData(str[0], str[1], dSn, code, data, out strReturn, out errMsg);
        //        if (!bRstr)
        //        {
        //            ReturnMsg = errMsg;
        //            return false;
        //        }
        //    }
        //    ReturnMsg = strReturn;
        //    return true;
        //}
        /// <summary>
        /// 请求链接
        /// </summary>
        /// <param name="strURL1">服务器URL1</param>
        /// <param name="strURL2">服务器URL2</param>
        /// <param name="strIP">本地IP</param>
        /// <param name="strMac">本地Mac</param>
        /// <param name="dSn">设备注册码</param>
        /// <param name="code">设备授权码</param>
        /// <param name="strReturnCode">返回结果</param>
        /// <param name="strMessage">返回信息</param>
        /// <returns></returns>
        public static bool RequireConnection(string strURL1, string strURL2, string strIP, string strMac, string dSn, string code, out string strReturnCode, out string strMessage)
        {
            strReturnCode = "";

            strURL1 = string.Concat(new string[]
            {
                strURL1,"/RequireConnection?dSn=",dSn,"&authCode=",code,"&mac=",strMac,"&ip=",strIP
            });
            strURL2 = string.Concat(new string[]
            {
                strURL2, "/RequireConnection?dSn=", dSn, "&authCode=",code,"&mac=",strMac, "&ip=",strIP
            });
            strURL1 = strURL1.Replace("+", "%2B");
            strURL2 = strURL2.Replace("+", "%2B");
            bool result;
            try
            {
                JsonConvert.SerializeObject(new MESRequestURL
                {
                    dSn = dSn,
                    authCode = code,
                    mac = strMac,
                    ip = strIP
                }).Replace(":", ": ");
                string value = "";
                string text = strURL1;
                if (!CallBobcat(text, "", out value, out strMessage))
                {
                    value = "";
                    text = strURL2;
                    if (!CallBobcat(text, "", out value, out strMessage))
                    {
                        strMessage = "RequireConnection." + strMessage;
                        //MessageBox.Show(errMsg);
                        strReturnCode = text;
                        result = false;
                        return result;
                    }
                }
                MESRespondURL mESRespondURL = JsonConvert.DeserializeObject<MESRespondURL>(value);
                strReturnCode = mESRespondURL.ReturnMsg;
                if (strReturnCode != "Pass")
                {
                    strMessage = "获取URL的列表list不为Pass";
                    //MessageBox.Show(strReturn);
                    result = false;
                }
                else
                {
                    string str = mESRespondURL.ListURL[0];
                    string str2 = mESRespondURL.ListURL[1];
                    strReturnCode = str + "," + str2;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "RequireConnection." + strMessage;
                result = false;
            }
            return result;
        }

        public static bool GetData(string strURL1, string strURL2, string dSn, string code, string data, out string strReturnCode, out string strMessage)
        {
            strReturnCode = "";
            strMessage = "";
            bool result;
            try
            {
                strURL1 = string.Concat(new string[]
                {
                    strURL1, "/GetData?dSn=",dSn,"&authCode=",code,"&data=",data
                });
                strURL2 = string.Concat(new string[]
                {
                    strURL2,"/GetData?dSn=", dSn,"&authCode=",code,"&data=",data
                });
                JsonConvert.SerializeObject(new MESRequestData
                {
                    dSn = dSn,
                    authCode = code,
                    data = data
                }).Replace(":", ": ");
                string value = "";
                string text = strURL1;
                if (!CallBobcat(text, "", out value, out strMessage))
                {
                    value = "";
                    text = strURL2;
                    if (!CallBobcat(text, "", out value, out strMessage))
                    {
                        strMessage = "GetData." + strMessage;
                        strReturnCode = "F";
                        result = false;
                        return result;
                    }
                }
                MESRespondData MESRespondData = JsonConvert.DeserializeObject<MESRespondData>(value);
                strMessage = MESRespondData.ReturnMsg;
                if (MESRespondData.ReturnCode == "F")
                {
                    result = false;
                }
                else if (MESRespondData.ReturnCode == "W")
                {
                    strReturnCode = "W" + strReturnCode;
                    result = true;
                }
                else
                {
                    strReturnCode = "P";
                    result = true;
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "GetData." + strMessage;
                result = false;
            }
            return result;
        }

        public static bool Trace_process_control(string strURL, string data, out string strMessage, out ProcessControlData Msg)
        {
            // strReturnCode = "";
            strMessage = "";
            Msg = null;
            bool result;
            try
            {
                strURL = strURL.Replace("*", data);
                string value = "";
                // string text = strURL;
                if (!CallBobcat(strURL, "", out value, out strMessage))
                {
                    value = "";
                    strMessage = "ProcessControlData:" + strMessage;
                    result = false;
                    return result;
                }
                strMessage = "ProcessControlData:" + value;
                ProcessControlData MESRespondData = JsonConvert.DeserializeObject<ProcessControlData>(value);
                Msg = MESRespondData;
                result = true;
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "ProcessControlData:" + strMessage;
                result = false;
            }
            return result;
        }


        public static bool GetTestResult(string SN, string strURL, out string strMessage, out string pass)
        {
            string strReturnCode = "";
            strMessage = "";
            pass = "";
            bool result;
            try
            {
                string value = "";
                if (!CallBobcat4(strURL, "", out value, out strMessage))
                {
                    value = "";
                    strMessage = "GetTestResult:" + strMessage;
                    result = false;
                    return result;
                }
                Log.WriteLog(SN + "," + value + "," + strMessage);
                Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + SN + "," + value + "," + strMessage, System.AppDomain.CurrentDomain.BaseDirectory + "\\JGP上传次数校验\\");
                strMessage = "GetTestResult:" + strMessage;
                if (value.Contains("no data found!"))
                {
                    pass = "0";
                }
                GetTestResult MESRespondData = JsonConvert.DeserializeObject<GetTestResult>(value);
                if (MESRespondData.status)
                {
                    result = true;
                    pass = MESRespondData.msg[0].pass;
                }
                else
                {
                    result = false;
                    pass = "0";
                    //pass = MESRespondData.msg[0].pass;
                }

            }
            catch (Exception ex)
            {
                strMessage = "GetTestResult:" + strMessage;
                result = false;
            }
            return result;
        }
        public static bool PIS_System(string strURL, out string strMessage, out string[] Msg)
        {
            string strReturnCode = "";
            strMessage = "";
            Msg = null;
            bool result;
            try
            {
                string value = "";
                if (!CallBobcat(strURL, "", out value, out strMessage))
                {
                    value = "";
                    strMessage = "PIS_System:" + strMessage;
                    result = false;
                    return result;
                }
                strMessage = "PIS_System:" + strMessage;
                string[] MESRespondData = JsonConvert.DeserializeObject<string[]>(value);
                Msg = MESRespondData;
                result = true;
            }
            catch (Exception ex)
            {
                strMessage = "PIS_System:" + strMessage;
                result = false;
            }
            return result;
        }

        public static bool IQC_System(string strURL, string data, out string strMessage, out string[] Msg)
        {
            strMessage = "IQC_System:";
            Msg = null;
            bool result;
            try
            {
                //strURL = strURL.Replace("*", data);
                //JsonConvert.SerializeObject(new MESRequestData
                //{
                //    data = data
                //}).Replace(":", ": ");
                string value = "";
                string text = strURL;
                if (!CallBobcat2(text, data, out value, out strMessage, false))
                {
                    value = "";
                    strMessage = "IQC_System:" + strMessage;
                    result = false;
                    Msg = null;
                    return result;
                }
                string[] MESRespondData = JsonConvert.DeserializeObject<string[]>(value);
                Msg = MESRespondData;
                result = true;
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "IQC_System:" + strMessage;
                Msg = null;
                result = false;
            }
            return result;
        }

        public static bool SendJPMes(string strURL, string data, out string strMessage, out JPRespondData Msg)
        {
            strMessage = "";
            Msg = null;
            bool result;
            try
            {
                strURL = strURL.Replace("*", data);
                string value = "";
                string text = strURL;
                if (!CallBobcat2(text, data, out value, out strMessage, true))
                {
                    value = "";
                    strMessage = "JPRespondData:" + strMessage;
                    result = false;
                    Msg = null;
                    return result;
                }
                JPRespondData MESRespondData = JsonConvert.DeserializeObject<JPRespondData>(value);
                Msg = MESRespondData;
                if (MESRespondData.status == "false")
                {
                    result = false;
                }
                else if (MESRespondData.status == "true")
                {
                    result = true;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "JPRespondData:" + strMessage;
                Msg = null;
                result = false;
            }
            return result;
        }

        public static bool Trcae_logs(string strURL, string data, out string strMessage, out TraceRespondID Msg)
        {
            strMessage = "TrcaeRespondData:";
            Msg = null;
            bool result;
            try
            {
                strURL = strURL.Replace("*", data);
                //JsonConvert.SerializeObject(new MESRequestData
                //{
                //    data = data
                //}).Replace(":", ": ");
                string value = "";
                string text = strURL;
                if (!CallBobcat2(text, data, out value, out strMessage, false))
                {
                    value = "";
                    strMessage = "TrcaeRespondData:" + strMessage;
                    result = false;
                    Msg = null;
                    return result;
                }
                TraceRespondID TraceRespondData = JsonConvert.DeserializeObject<TraceRespondID>(value);
                Msg = TraceRespondData;
                result = true;
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                strMessage = "TrcaeRespondData:" + strMessage;
                Msg = null;
                result = false;
            }
            return result;
        }

        public static bool CallBobcat(string url, string msg, out string callResult, out string errMsg)
        {
            bool result;
            try
            {
                url = url.Replace("+", "%2B");

                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "Get";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Timeout = 5000;
                httpWebRequest.KeepAlive = true;
                if (bytes != null && bytes.Length > 0)
                {
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                callResult = streamReader.ReadToEnd();
                errMsg = "";
                result = true;
            }
            catch (Exception ex)
            {
                callResult = "";
                errMsg = ex.Message;
                result = false;
            }
            return result;
        }

        public static bool CallBobcat2(string url, string msg, out string callResult, out string errMsg, bool b)
        {
            bool result;
            try
            {
                url = url.Replace("+", "%2B");

                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                if (b)
                {
                    httpWebRequest.Headers.Add("Authorization:Bearer c0beb2f1acc99ed57c1d97abb4fdfa400");
                }
                httpWebRequest.Timeout = 5000;
                httpWebRequest.KeepAlive = true;
                if (bytes != null && bytes.Length > 0)
                {
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                callResult = streamReader.ReadToEnd();
                errMsg = "";
                result = true;
            }
            catch (Exception ex)
            {
                callResult = "";
                errMsg = ex.Message;
                result = false;
            }
            return result;
        }

        public static bool CallBobcat3(string url, string msg, out string callResult, out string errMsg, bool b)
        {
            bool result;
            try
            {
                url = url.Replace("+", "%2B");

                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                if (b)
                {
                    httpWebRequest.Headers.Add("Authorization:Bearer c0beb2f1acc99ed57c1d97abb4fdfa400");
                }
                httpWebRequest.Timeout = 2000;
                httpWebRequest.KeepAlive = true;
                if (bytes != null && bytes.Length > 0)
                {
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                callResult = streamReader.ReadToEnd();
                errMsg = "";
                result = true;
            }
            catch (Exception ex)
            {
                callResult = "";
                errMsg = ex.Message;
                result = false;
            }
            return result;
        }

        public static bool CallBobcat4(string url, string msg, out string callResult, out string errMsg)
        {
            bool result;
            try
            {
                url = url.Replace("+", "%2B");

                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "Get";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Timeout = 2000;
                httpWebRequest.KeepAlive = true;
                if (bytes != null && bytes.Length > 0)
                {
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                callResult = streamReader.ReadToEnd();
                errMsg = "";
                result = true;
            }
            catch (Exception ex)
            {
                callResult = "";
                errMsg = ex.Message;
                result = false;
            }
            return result;
        }
    }
}
