using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;

namespace 卓汇数据追溯系统
{
    class RequestAPI3
    {
        public static string HttpPostWebService(string url, string IP, string Process, string Line_id, string Station_id, string SoftwareName,out string ErrMsg)
        {
            string result = string.Empty;
            string param = string.Empty;
            byte[] bytes = null;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            param = HttpUtility.UrlEncode("IP") + "=" + IP + "&" + HttpUtility.UrlEncode("Process") + "=" + Process + "&" + HttpUtility.UrlEncode("Line_id") + "=" + Line_id + "&" + HttpUtility.UrlEncode("Station_id") + "=" + Station_id + "&" + HttpUtility.UrlEncode("SoftwareName") + "=" + SoftwareName;
            bytes = Encoding.UTF8.GetBytes(param);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 3000;
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();        //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";
               
            }
            writer.Write(bytes, 0, bytes.Length);       //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();      //获得响应
            }
            catch (WebException ex)
            {
                ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";
               
            }
            Stream stream = response.GetResponseStream();        //获取响应流
            XmlTextReader Reader = new XmlTextReader(stream);
            Reader.MoveToContent();
            result = Reader.ReadInnerXml();
            response.Close();
            Reader.Close();
            stream.Dispose();
            stream.Close();
            ErrMsg = string.Empty;
            return result;
        }


        public static string Add(string url,int a,int b)
        {
            string result = string.Empty;
            string param = string.Empty;
            byte[] bytes = null;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            param = HttpUtility.UrlEncode("a") + "=" + a + "&" + HttpUtility.UrlEncode("b") + "=" + b;
            bytes = Encoding.UTF8.GetBytes(param);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 3000;
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();        //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
               // ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";

            }
            writer.Write(bytes, 0, bytes.Length);       //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();      //获得响应
            }
            catch (WebException ex)
            {
               // ErrMsg = ex.Message;
                Log.WriteLog(ex.ToString());
                return "";

            }
            Stream stream = response.GetResponseStream();        //获取响应流
            XmlTextReader Reader = new XmlTextReader(stream);
            Reader.MoveToContent();
            result = Reader.ReadInnerXml();
            response.Close();
            Reader.Close();
            stream.Dispose();
            stream.Close();
           // ErrMsg = string.Empty;
            return result;
        }

        public static bool CallBobcat(string url, string msg, string Username, string Password, out string callResult, out string errMsg)
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
                //设置请求Credentials
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(Username, Password));
                httpWebRequest.Credentials = credentialCache;
                //设置Headers Authorization
                httpWebRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password)));
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
    }
}
