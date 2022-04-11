using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 卓汇数据追溯系统
{
    public partial class UserLoginFrm : Form
    {
        private MainFrm _mainparent;
        private delegate void LabelText(Label lb, string bl);
        private delegate void Labelcolor(Label lb, Color color, string bl);
        private delegate void ShowTxt(TextBox tb, string txt);
        private delegate void ShowLabel(string lb, string txt);
        private delegate void ShowPanelVisible(bool b, string txt);
        private delegate void cboSelect(int i);
        delegate void AddItemToListBoxDelegate(string str, string Name);
        SQLServer SQL = new SQLServer();
        List<Control> List_Control = new List<Control>();
        public UserLoginFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;
            Global.inidata = new IniProductFile(System.AppDomain.CurrentDomain.BaseDirectory + "setting.ini");
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)//自定义绘制Tab标题
        {
            string text = ((TabControl)sender).TabPages[e.Index].Text;
            //标签背景填充颜色
            SolidBrush BackBrush = new SolidBrush(Color.Gray);
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat();
            //设置文字对齐方式
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            if (e.Index == this.tabControl1.SelectedIndex)//当前Tab页的样式
            {
                BackBrush = new SolidBrush(Color.DarkSeaGreen);
            }
            //绘制标签头背景颜色
            e.Graphics.FillRectangle(BackBrush, e.Bounds);
            //绘制标签头文字
            //e.Graphics.DrawString(text, SystemInformation.MenuFont, brush, e.Bounds, sf);
            e.Graphics.DrawString(text, new Font("微软雅黑", 13), brush, e.Bounds, sf);
        }

        private void btn_Login_Click(object sender, EventArgs e)//用户登录
        {
            if (cbo_Login.SelectedIndex == 0)
            {
                if (txt_pwd.Text == Global.Operator_pwd)
                {
                    lbl_LoginLevel.Text = "操作员";
                    Global.Login = Global.LoginLevel.Operator;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "操作员登录成功");
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
            else if (cbo_Login.SelectedIndex == 1)
            {
                if (txt_pwd.Text == Global.Technician_pwd)
                {
                    lbl_LoginLevel.Text = "技术员";
                    Global.Login = Global.LoginLevel.Technician;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "技术员登录成功");
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
            else
            {
                if (txt_pwd.Text == Global.Administrator_pwd)
                {
                    lbl_LoginLevel.Text = "工程师";
                    Global.Login = Global.LoginLevel.Administrator;
                    labelcolor(lbl_LoginMes, Color.LimeGreen, "工程师登录成功");
                }
                else
                {
                    labelcolor(lbl_LoginMes, Color.Red, "密码错误,请重新输入！");
                    UiText(txt_pwd, "");
                }
            }
        }

        public void labelText(Label lb, string txt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new LabelText(labelText), new object[] { lb, txt });
                return;
            }
            lb.Text = txt;
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

        public void UiText(TextBox tb, string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { tb, str });
                return;
            }
            tb.Text = str;
        }

        public void UiLabel(string str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowLabel(UiLabel), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = str1;
                    }
                }
            }
        }

        public void AddList(string msg, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AddList), new object[] { msg, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(ListBox))
                {
                    if (ctrl.Name == Name)
                    {
                        if (msg != "N/A")
                        {
                            ((ListBox)ctrl).Items.Add(msg + "\r\n");
                        }
                        else
                        {
                            ((ListBox)ctrl).Items.Clear();
                        }
                    }
                }
            }

        }

        public void PanelVisible(bool str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowPanelVisible(PanelVisible), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(TableLayoutPanel))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Visible = str1;
                    }
                }
            }
        }

        private void chk_DisplayPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayPwd.Checked)
            {
                txt_pwd.PasswordChar = new char();
            }
            else
            {
                txt_pwd.PasswordChar = '*';
            }
        }

        private void chk_DisplayOldPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayOldPwd.Checked)
            {
                txt_oldPwd.PasswordChar = new char();
            }
            else
            {
                txt_oldPwd.PasswordChar = '*';
            }
        }

        private void chk_DisplayNewPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_DisplayNewPwd.Checked)
            {
                txt_NewPwd.PasswordChar = new char();
            }
            else
            {
                txt_NewPwd.PasswordChar = '*';
            }
        }

        private void cbo_Login_SelectedIndexChanged(object sender, EventArgs e)
        {
            UiText(txt_pwd, "");
            txt_pwd.Focus();
        }

        private void btn_ChangePwd_Click(object sender, EventArgs e)
        {
            if (Global.Login == Global.LoginLevel.Operator)
            {
                if (txt_oldPwd.Text == Global.Operator_pwd)
                {
                    Global.inidata.productconfig.Operator_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Operator_pwd = Global.inidata.productconfig.Operator_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "操作员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
            else if (Global.Login == Global.LoginLevel.Technician)
            {
                if (txt_oldPwd.Text == Global.Technician_pwd)
                {
                    Global.inidata.productconfig.Technician_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Technician_pwd = Global.inidata.productconfig.Technician_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "技术员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
            else if (Global.Login == Global.LoginLevel.Administrator)
            {
                if (txt_oldPwd.Text == Global.Administrator_pwd)
                {
                    Global.inidata.productconfig.Administrator_pwd = txt_NewPwd.Text;
                    Global.inidata.WriteProductConfigSection();
                    Global.Administrator_pwd = Global.inidata.productconfig.Administrator_pwd;
                    labelcolor(lbl_ChangePwdMes, Color.LimeGreen, "管理员密码更改成功");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                }
                else
                {
                    labelcolor(lbl_ChangePwdMes, Color.Red, "旧密码不正确,请重新输入!");
                    UiText(txt_oldPwd, "");
                    UiText(txt_NewPwd, "");
                    txt_oldPwd.Focus();
                }
            }
        }

        private void UserLoginFrm_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage1);//移除1，2选项卡
            tabControl1.TabPages.Remove(tabPage2);
            List_Control = GetAllControls(this);//列表中添加所有窗体控件
            if (Global.Login == Global.LoginLevel.Operator)
            {
                lbl_LoginLevel.Text = "操作员";
                cbo_Login.SelectedIndex = 0;
            }
            if (Global.Login == Global.LoginLevel.Technician)
            {
                lbl_LoginLevel.Text = "技术员";
                cbo_Login.SelectedIndex = 1;
            }
            if (Global.Login == Global.LoginLevel.Administrator)
            {
                lbl_LoginLevel.Text = "工程师";
                cbo_Login.SelectedIndex = 2;
            }

            ////读取PQ状态
            //string Msg_pq;
            //string str_pq = "";
            //RequestAPI2.CallBobcat2(string.Format("http://10.128.10.7/Webapi/api/WorkTime/GetPqStatus?dsn={0}", Global.inidata.productconfig.OEE_Dsn), "", out str_pq, out Msg_pq, false);
            //PqStatus status = JsonConvert.DeserializeObject<PqStatus>(str_pq);
            //if (status.msg[0].isPQ)
            //{
            //    Global.IfPQStatus = true;
            //    PanelVisible(true, "panel_PQ_Y");
            //    PanelVisible(false, "panel_PQ_N");
            //}
            //else
            //{
            //    Global.IfPQStatus = false;
            //    PanelVisible(false, "panel_PQ_Y");
            //    PanelVisible(true, "panel_PQ_N");
            //}
            //Log.WriteLog("获取PQ状态接收:" + JsonConvert.SerializeObject(str_pq));
        }

        public void ComboBoxSelect(int i)//切换为作业员等级
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new cboSelect(ComboBoxSelect), new object[] { i });
                return;
            }
            cbo_Login.SelectedIndex = i;
            lbl_LoginLevel.Text = "操作员";
            lbl_LoginMes.BackColor = Color.Transparent;
            lbl_LoginMes.Text = "";
        }

        public void serial1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Global._serial1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
            {
                //输出当前时间
                Thread.Sleep(500);
                try
                {
                    Byte[] receivedData = new Byte[Global._serial1.BytesToRead];        //创建接收字节数组
                    Global._serial1.Read(receivedData, 0, receivedData.Length);         //读取数据
                                                                                        //string text = sp1.Read();   //Encoding.ASCII.GetString(receivedData);
                    Global._serial1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer
                                                                                        //这是用以显示字符串
                    string strRcv = null;
                    for (int i = 0; i < receivedData.Length; i++)
                    {
                        strRcv += ((char)Convert.ToInt32(receivedData[i]));
                    }
                    strRcv = strRcv.Substring(1, 10);
                    if (Global.IfLoginbtn)
                    {
                        Invoke(new Action(() => txt_UserID.Text = strRcv));             //显示参数修改界面刷卡信息
                    }
                    Invoke(new Action(() => lb_CardNo.Text = strRcv));                  //显示上班登入界面刷卡信息
                    Invoke(new Action(() => lb_CardNo.ForeColor = Color.Black));
                    Global.IfReadUserID = true;
                    Global.UserLoginMouseMoveTime = DateTime.Now;
                    Thread.Sleep(50);
                    if (txt_UserID.Text.Length == 10 && !Global.IfUserLogin)
                    {
                        Invoke(new Action(() => lb_remind.Text = "刷卡成功"));
                        Invoke(new Action(() => lb_remind.ForeColor = Color.Green));
                        string Err_Msg;
                        string UserInfo = "";
                        //-------------中控系统----------
                        try
                        {
                            string RecData = string.Empty;
                            string errorMsg = string.Empty;
                            JsonSerializerSettings jsetting = new JsonSerializerSettings();
                            jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                            Login login = new Login();
                            login.stationID = Global.inidata.productconfig.Station_id_ua;
                            login.moduleCode = "1";
                            login.idType = "idCard";
                            login.username = strRcv;
                            string SendData = JsonConvert.SerializeObject(login, Formatting.None, jsetting);
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "请求登录：" + SendData)));
                            Log.WriteLog("请求登录：" + SendData);
                            CentralControlAPI.RequestPost("http://10.143.20.222:80/openApi/v1/machine/login?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE0NDc0NDAwMDY4MjkzMTQwNDgiLCJzbiI6MTQ0NzQ0NTI0NDkwNjgzNTk2OH0.pnTP-0hcDzjTPA5yg3PQLyd96yfmosSU0dyE-Bpc8sg", SendData, out RecData, out errorMsg);
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登录结果：" + RecData)));
                            Log.WriteLog("登录结果：" + RecData);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog("中控系统登录异常：" + ex.ToString());
                        }
                        string URL = string.Format("http://10.143.24.3/assyroleapi/Home/RequireConnection?field0006={0}", strRcv);
                        var result = RequestAPI2.CallBobcat(URL, "", out UserInfo, out Err_Msg);
                        Log.WriteLog("刷卡获取用户信息：" + JsonConvert.SerializeObject(UserInfo));
                        UserInfo userinfo = JsonConvert.DeserializeObject<UserInfo>(UserInfo);
                        if (userinfo.ReturnMsg == "Pass")
                        {
                            Global.IfUserLogin = true;
                            Global.Emp = userinfo.field0001;
                            Global.Name = userinfo.field0002;
                            Global.Title = userinfo.field0004;
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登录人：" + Global.Name + ",权限：" + Global.Title)));
                            Log.WriteLog("登录人：" + Global.Name + ",权限：" + Global.Title);
                            Global.UserLoginTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            //判断登录人的权限等级
                            if (Global.Title == "Operator")
                            {
                                Global.PLC_Client2.WritePLC_D(16000, new short[] { 1 });//发送权限等级给PLC
                                Global.Login = Global.LoginLevel.Operator;// 切换追溯软件权限
                                Global.inidata2.productconfig.Name = Global.Name;
                                Global.inidata2.productconfig.ID = Global.Emp;
                                Global.inidata2.productconfig.Level = "1";
                                Global.inidata2.WriteProductnumSection();// 发送权限等级给CCD
                                Global.client1.Send(string.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                Global.client2.Send(string.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                            }
                            else if (Global.Title == "Technician" || Global.Title == "Vendor_Technician")
                            {
                                Global.PLC_Client2.WritePLC_D(16000, new short[] { 2 });
                                Global.Login = Global.LoginLevel.Technician;
                                Global.inidata2.productconfig.Name = Global.Name;
                                Global.inidata2.productconfig.ID = Global.Emp;
                                Global.inidata2.productconfig.Level = "2";
                                Global.inidata2.WriteProductnumSection();
                                Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV2", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                            }
                            else if (Global.Title == "Vendor_Engineer" || Global.Title == "Vendor_Administrator")
                            {
                                Global.PLC_Client2.WritePLC_D(16000, new short[] { 3 });
                                Global.Login = Global.LoginLevel.Administrator;
                                Global.inidata2.productconfig.Name = Global.Name;
                                Global.inidata2.productconfig.ID = Global.Emp;
                                Global.inidata2.productconfig.Level = "3";
                                Global.inidata2.WriteProductnumSection();
                                Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV3", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                            }
                            else
                            {
                                Global.PLC_Client2.WritePLC_D(16000, new short[] { 0 });
                                Global.Login = Global.LoginLevel.Operator;
                                Global.inidata2.productconfig.Name = Global.Name;
                                Global.inidata2.productconfig.ID = Global.Emp;
                                Global.inidata2.productconfig.Level = "1";
                                Global.inidata2.WriteProductnumSection();
                                Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-1
                                Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", Global.Name, Global.Emp));// 发送权限等级给大族焊接机-2
                            }
                        }
                        else
                        {
                            Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + JsonConvert.SerializeObject(UserInfo))));
                            Log.WriteLog("获取用户信息错误！" + JsonConvert.SerializeObject(UserInfo));
                        }
                    }
                    else
                    {
                        if (txt_UserID.Text.Length != 10)
                        {
                            Invoke(new Action(() => lb_remind.Text = "请先点击【登入】按钮！"));
                            Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                        }
                        if (Global.IfUserLogin)
                        {
                            Invoke(new Action(() => lb_remind.Text = "已登入！请先登出！"));
                            Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                        }
                    }
                    //}
                    //string strRcv = null;
                    //int decNum = 0;//存储十进制
                    //for (int i = 0; i < receivedData.Length; i++) //窗体显示
                    //{
                    //    strRcv += receivedData[i].ToString("X2");  //16进制显示
                    //}

                }
                catch (System.Exception ex)
                {
                    Log.WriteLog("刷卡信息读取异常" + ex.ToString());
                    //MessageBox.Show(ex.Message, "出错提示");
                }
            }
        }

        public void btn_UserLogin_Click(object sender, EventArgs e)
        {
            if (!Global.IfLoginbtn)
            {
                Global.IfLoginbtn = true;
                Invoke(new Action(() => btn_UserLogin.Text = "登出"));
                Invoke(new Action(() => lb_remind.Text = "请刷卡"));
                Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
            }
            else
            {
                if (Global.Login == Global.LoginLevel.Operator)
                {
                    Invoke(new Action(() => lb_remind.Text = "无权限,请刷卡！"));
                    Invoke(new Action(() => lb_remind.ForeColor = Color.Red));
                    return;
                }
                Global.IfLoginbtn = false;
                //-------中控系统-------
                try
                {
                    string RecData = string.Empty;
                    string errorMsg = string.Empty;
                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                    jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                    Logout logOut = new Logout();
                    logOut.stationID = Global.inidata.productconfig.Station_id_ua;
                    logOut.moduleCode = "1";
                    logOut.idType = "idCard";
                    Invoke(new Action(() => logOut.username = txt_UserID.Text));
                    string SendData = JsonConvert.SerializeObject(logOut, Formatting.None, jsetting);
                    Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "请求退出：" + SendData)));
                    Log.WriteLog("请求退出：" + SendData);
                    CentralControlAPI.RequestPost("http://10.143.20.222:80/openApi/v1/machine/logout?access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE0NDc0NDAwMDY4MjkzMTQwNDgiLCJzbiI6MTQ0NzQ0NTI0NDkwNjgzNTk2OH0.pnTP-0hcDzjTPA5yg3PQLyd96yfmosSU0dyE-Bpc8sg", SendData, out RecData, out errorMsg);
                    Rec_Logout rec_Logout = JsonConvert.DeserializeObject<Rec_Logout>(RecData);
                    Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "退出结果：" + rec_Logout.ok)));
                    Log.WriteLog("退出结果：" + rec_Logout.ok);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("中控系统退出异常：" + ex.ToString());
                }
                Invoke(new Action(() => btn_UserLogin.Text = "登入"));
                Invoke(new Action(() => lb_remind.Text = ""));
                Invoke(new Action(() => txt_UserID.Text = ""));
                Invoke(new Action(() => list_UserLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "登出人：" + Global.Name + ",权限：" + Global.Title)));
                string InsertStr = "insert into UserLogin([用户名称],[用户ID],[权限等级],[登入时间],[登出时间])" + " " + "values(" + "'" + Global.Name + "'" + "," + "'" + Global.Emp + "'" + "," + "'" + Global.Title + "'" + "," + "'" + Global.UserLoginTime + "'" + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ")";
                SQL.ExecuteUpdate(InsertStr);
                Log.WriteLog("登出人：" + Global.Name + ",权限：" + Global.Title);
                Global.Emp = string.Empty;
                Global.Name = "操作员";
                Global.Title = "Operator";
                Global.IfUserLogin = false;//标志位 权限已登出
                Global.PLC_Client.WritePLC_D(16000, new short[] { 0 });//发送权限等级给PLC
                Global.Login = Global.LoginLevel.Operator;// 切换追溯软件权限
                Global.inidata2.productconfig.Name = "";
                Global.inidata2.productconfig.ID = "";
                Global.inidata2.productconfig.Level = "1";
                Global.inidata2.WriteProductnumSection();// 发送权限等级给CCD
                if (Global.client1.Connected && Global.client2.Connected)
                {
                    Global.client1.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-1
                    Global.client2.Send(String.Format("USER;{0};{1};{2};\r\n", "LEV1", "", ""));// 发送权限等级给大族焊接机-2
                }
            }
        }

        private void btn_UpLoadLogin_Click(object sender, EventArgs e)
        {
            if ((!cb_Setup.Checked && !cb_IQ.Checked && !cb_OQ.Checked && !cb_PQ.Checked) && (!rb_MQ.Checked && !rb_Production.Checked))
            {
                Invoke(new Action(() => lb_CardNo.Text = "请选择LQ阶段！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
                return;
            }
            if (cb_UserClass.SelectedIndex != 0 && cb_UserClass.SelectedIndex != 1)
            {
                Invoke(new Action(() => lb_CardNo.Text = "请选择白夜班次！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
                return;
            }
            if (lb_CardNo.Text.Length == 10)
            {
                //上传打卡数据
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;//Json不输出空值
                IDCard CardData = new IDCard();
                CardData.cardNo = lb_CardNo.Text;                   //用户卡号
                CardData.dsn = Global.inidata.productconfig.OEE_Dsn;//机台dsn
                CardData.shift = 1;                                 //班别代号，从接口二获取的id
                CardData.machineType = 1;                           //打卡机类型。1：固定式刷卡机。0：刷卡式
                CardData.swipeType = 0;                             //0:上班，1：下班
                CardData.stage = new string[] { };                  //阶段，类型为数组 Setup，IQ，OQ，PQ，MQ，Productio
                List<string> _list = new List<string>();
                if (panel_PQ_N.Visible)
                {
                    if (cb_Setup.Checked)
                    {
                        _list.Add("Setup");
                    }
                    if (cb_IQ.Checked)
                    {
                        _list.Add("IQ");
                    }
                    if (cb_OQ.Checked)
                    {
                        _list.Add("OQ");
                    }
                    if (cb_PQ.Checked)
                    {
                        _list.Add("PQ");
                    }
                }
                if (panel_PQ_Y.Visible)
                {
                    if (rb_MQ.Checked)
                    {
                        _list.Add("MQ");
                    }
                    if (rb_Production.Checked)
                    {
                        _list.Add("Production");
                    }
                }
                CardData.stage = _list.ToArray();
                string SendCardData = JsonConvert.SerializeObject(CardData, Formatting.None, jsetting);
                string Msg_ua;
                string Trace_str_ua = "";
                list_UploadLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + SendCardData + "\r\n");
                Log.WriteLog("上传打卡数据:" + SendCardData);
                RequestAPI2.CallBobcat2("http://10.128.10.7/Webapi/api/WorkTime/UpdateData", SendCardData, out Trace_str_ua, out Msg_ua, false);
                list_UploadLogin.Items.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + JsonConvert.SerializeObject(Trace_str_ua) + "\r\n");
                Log.WriteLog("打卡数据接收:" + JsonConvert.SerializeObject(Trace_str_ua));
                Invoke(new Action(() => lb_CardNo.Text = ""));
            }
            else
            {
                Invoke(new Action(() => lb_CardNo.Text = "请刷卡！"));
                Invoke(new Action(() => lb_CardNo.ForeColor = Color.Red));
            }
        }

        public List<Control> GetAllControls(Control control)
        {
            var list = new List<Control>();
            foreach (Control con in control.Controls)
            {
                list.Add(con);
                if (con.Controls.Count > 0)
                {
                    list.AddRange(GetAllControls(con));
                }
            }
            return list;
        }

        private void UserLoginFrm_MouseEnter(object sender, EventArgs e)
        {
            Global.UserLoginMouseMoveTime = DateTime.Now;
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chk_DisplayPwd_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chk_DisplayPwd.Checked)
            {
                txt_pwd.PasswordChar = new char();
            }
            else
            {
                txt_pwd.PasswordChar = '*';
            }
        }
    }
}
