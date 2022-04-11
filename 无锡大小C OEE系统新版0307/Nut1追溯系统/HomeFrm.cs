using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 卓汇数据追溯系统
{
    public partial class HomeFrm : Form
    {
        private MainFrm _mainparent;
        delegate void AddItemToListBoxDelegate(string str, string Name);
        delegate void ShowDataGridView(DataGridView dgv, DataTable dt, int index);
        delegate void ShowDataGridView2(string fixture);
        delegate void DGVAutoSize(DataGridView dgv);
        private delegate void Labelvision(string bl, string Name);
        private delegate void Labelcolor(Color color, string bl, string Name);
        private delegate void ShowTxt(string txt, string Name);
        string Conn = "provider=microsoft.jet.oledb.4.0;data source=mydata.mdb;";
        SQLServer SQL = new SQLServer();
        private delegate void UpdateDataGridView1();
        List<Control> List_Control = new List<Control>();
        int rth_Number = 0;
        public HomeFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;

        }

        private void DGV_Refrush()
        {
            //this.dgv_SpareParts.Refresh();
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
            e.Graphics.DrawString(text, new Font("微软雅黑", 15), brush, e.Bounds, sf);
        }

        private void HomeFrm_Load(object sender, EventArgs e)
        {
            //panel1.Controls.Add(_mainparent.maindis);
            //_mainparent.maindis.Dock = DockStyle.Fill;
            //ShowSparePartData();
            List_Control = GetAllControls(this);//列表中添加所有窗体控件
            //Global.Trace_ua_ok = Convert.ToInt32(Global.inidata.productconfig.trace_ua_ok);
            //lb_TraceUAOK.Text = Global.inidata.productconfig.trace_ua_ok;
            //Global.Trace_ua_ng = Convert.ToInt32(Global.inidata.productconfig.trace_ua_ng);
            //lb_TraceUANG.Text = Global.inidata.productconfig.trace_ua_ng;
            //Global.Product_num_ua_ok = Convert.ToInt32(Global.inidata.productconfig.product_num_ua_ok);
            //lb_PDCAUAOK.Text = Global.inidata.productconfig.product_num_ua_ok;
            //Global.Product_num_ua_ng = Convert.ToInt32(Global.inidata.productconfig.product_num_ua_ng);
            //lb_PDCAUANG.Text = Global.inidata.productconfig.product_num_ua_ng;


            Global.oee_fixture_ok = Convert.ToInt32(Global.inidata.productconfig.oee_fixture_ok);
            //lb_FixtureOK.Text = Global.inidata.productconfig.oee_fixture_ok;
            Global.oee_fixture_ng = Convert.ToInt32(Global.inidata.productconfig.oee_fixture_ng);
            //lb_FixtureNG.Text = Global.inidata.productconfig.oee_fixture_ng;
            Global.oee_ok = Convert.ToInt32(Global.inidata.productconfig.oee_ok);
            lb_OEEOK.Text = Global.inidata.productconfig.oee_ok;
            Global.oee_ng = Convert.ToInt32(Global.inidata.productconfig.oee_ng);
            lb_OEENG.Text = Global.inidata.productconfig.oee_ng;
            //Global.ThrowCount = Convert.ToInt32(Global.inidata.productconfig.ThrowCount);
            //lb_Materiel_AllNut.Text = Global.inidata.productconfig.ThrowCount;
            //Global.ThrowOKCount = Convert.ToInt32(Global.inidata.productconfig.ThrowOKCount);
            //lb_Materiel_AllOK.Text = Global.inidata.productconfig.ThrowOKCount;
            //Global.TotalThrowCount = Convert.ToInt32(Global.inidata.productconfig.TotalThrowCount);
            //lb_Materiel_Total.Text = Global.inidata.productconfig.TotalThrowCount;
            //Global.NutCount = Convert.ToInt32(Global.inidata.productconfig.NutCount);
            //lb_Materiel_Nut.Text = Global.inidata.productconfig.NutCount;
            //Global.NutOKCount = Convert.ToInt32(Global.inidata.productconfig.NutOKCount);
            //lb_Materiel_OK.Text = Global.inidata.productconfig.NutOKCount;
            //Global.Product_num_Process_ok = Convert.ToInt32(Global.inidata.productconfig.product_num_process_ok);
            //lb_ProcessControlOK.Text = Global.inidata.productconfig.product_num_process_ok;
            //Global.Product_num_Process_ng = Convert.ToInt32(Global.inidata.productconfig.product_num_process_ng);
            //lb_ProcessControlNG.Text = Global.inidata.productconfig.product_num_process_ng;
            Thread.Sleep(200);
        }

      

        private void dgv_Dispaly_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //int i, j; float k = 0.00F; j = dgv_Dispaly.Rows.Count;
            //for (i = 0; i < j; i++)
            //{
            //    k = float.Parse(dgv_Dispaly.Rows[i].Cells["部件寿命残值"].Value.ToString());
            //    if ((k < 0.7) && (k > 0.3))
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
            //    }
            //    else if (k >= 0.7)
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
            //    }
            //    else if (k <= 0.3)
            //    {
            //        dgv_Dispaly.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            //    }
            //}
        }

        public void ShowSparePartData()
        {
            //DataTable d1 = new DataTable();
            //string myStr1 = string.Format("select ID,类别,品名,规格,上次更换时间,标准寿命,实际使用次数,部件寿命残值 from SparePartData where 1=1");//sql查询语句
            //d1 = server.ExecuteQuery(myStr1);
            //ShowDGV(dgv_Dispaly, d1, 1);

        }

        private void ShowDGV(DataGridView dgv, DataTable dt, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDataGridView(ShowDGV), new object[] { dgv, dt, index });
                return;
            }
            switch (index)
            {
                case 0:
                    dgv.DataSource = dt;
                    break;
                case 1:
                    dgv.DataSource = dt;
                    dgv.Columns["部件寿命残值"].DefaultCellStyle.Format = "p3";//设定datagridview寿命残值显示为百分比
                    dgv_AutoSize(dgv);
                    dgv.Sort(dgv.Columns["部件寿命残值"], ListSortDirection.Ascending);//升序排列
                    break;
                default:
                    break;
            }
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

        public void UpDatalabel(string txt, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelvision(UpDatalabel), new object[] { txt, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = txt;
                    }
                }
            }
        }

        public void UpDatalabelcolor(Color color, string str, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Labelcolor(UpDatalabelcolor), new object[] { color, str, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(Label))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.BackColor = color;
                        ctrl.Text = str;
                    }
                }
            }
        }

        public void AppendRichText(string msg, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AddItemToListBoxDelegate(AppendRichText), new object[] { msg, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(RichTextBox))
                {
                    if (ctrl.Name == Name)
                    {
                        if (msg != "N/A")
                        {
                            ((RichTextBox)ctrl).AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":  " + msg + "\r\n");
                            //将光标位置设置到当前内容的末尾
                            ((RichTextBox)ctrl).SelectionStart = ((RichTextBox)ctrl).Text.Length;
                            //滚动到光标位置
                            ((RichTextBox)ctrl).ScrollToCaret();
                            if (ctrl.Name == "rtx_TraceMsg" || ctrl.Name == "rtx_PDCAMsg")//判断trace和pdca上传多少次后清空Rth控件
                            {
                                rth_Number++;
                                if (rth_Number > 15)
                                {
                                    rth_Number = 0;
                                    //rtx_TraceMsg.Clear();
                                    //rtx_PDCAMsg.Clear();
                                }
                            }
                        }
                        else
                        {
                            ((RichTextBox)ctrl).Clear();
                        }
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
                            if (Name != "list_AllFixture")
                            {
                                ((ListBox)ctrl).Items.Add(msg + "\r\n");
                            }
                            else
                            {
                                //if (!list_AllFixture.Items.Contains(msg))//当列表中不存在该笔治具时，添加治具号
                                //{
                                //    list_AllFixture.Items.Add(msg);
                                //}
                            }
                        }
                        else
                        {
                            ((ListBox)ctrl).Items.Clear();
                        }
                    }
                }
            }
        }

        public void UiText(string str1, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UiText), new object[] { str1, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = str1;
                    }
                }
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

        private void lb_TraceUAOK_TextChanged(object sender, EventArgs e)
        {
            //Global.Trace_ua_ok = Convert.ToInt32(lb_TraceUAOK.Text);
            //Global.inidata.productconfig.trace_ua_ok = lb_TraceUAOK.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void lb_TraceUANG_TextChanged(object sender, EventArgs e)
        {
            //Global.Trace_ua_ng = Convert.ToInt32(lb_TraceUANG.Text);
            //Global.inidata.productconfig.trace_ua_ng = lb_TraceUANG.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void lb_PDCAUAOK_TextChanged(object sender, EventArgs e)
        {
            //Global.Product_num_ua_ok = Convert.ToInt32(lb_PDCAUAOK.Text);
            //Global.inidata.productconfig.product_num_ua_ok = lb_PDCAUAOK.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void lb_PDCAUANG_TextChanged(object sender, EventArgs e)
        {
            //Global.Product_num_ua_ng = Convert.ToInt32(lb_PDCAUANG.Text);
            //Global.inidata.productconfig.product_num_ua_ng = lb_PDCAUANG.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void lb_OEEOK_TextChanged(object sender, EventArgs e)
        {
            Global.oee_ok = Convert.ToInt32(lb_OEEOK.Text);
            Global.inidata.productconfig.oee_ok = lb_OEEOK.Text;
            Global.inidata.WriteProductnumSection();
        }

        private void lb_OEENG_TextChanged(object sender, EventArgs e)
        {
            Global.oee_ng = Convert.ToInt32(lb_OEENG.Text);
            Global.inidata.productconfig.oee_ng = lb_OEENG.Text;
            Global.inidata.WriteProductnumSection();
        }

        //private void lb_Materiel_Total_TextChanged(object sender, EventArgs e)
        //{
        //    Global.TotalThrowCount = Convert.ToInt32(lb_Materiel_Total.Text);
        //    Global.inidata.productconfig.TotalThrowCount = lb_Materiel_Total.Text;
        //    Global.inidata.WriteProductnumSection();
        //}

        //private void lb_Materiel_AllNut_TextChanged(object sender, EventArgs e)
        //{
        //    Global.ThrowCount = Convert.ToInt32(lb_Materiel_AllNut.Text);
        //    Global.inidata.productconfig.ThrowCount = lb_Materiel_AllNut.Text;
        //    Global.inidata.WriteProductnumSection();
        //}

        //private void lb_Materiel_Nut_TextChanged(object sender, EventArgs e)
        //{
        //    Global.NutCount = Convert.ToInt32(lb_Materiel_Nut.Text);
        //    Global.inidata.productconfig.NutCount = lb_Materiel_Nut.Text;
        //    Global.inidata.WriteProductnumSection();
        //}

        //private void lb_Materiel_AllOK_TextChanged(object sender, EventArgs e)
        //{
        //    Global.ThrowOKCount = Convert.ToInt32(lb_Materiel_AllOK.Text);
        //    Global.inidata.productconfig.ThrowOKCount = lb_Materiel_AllOK.Text;
        //    Global.inidata.WriteProductnumSection();
        //}

        //private void lb_Materiel_OK_TextChanged(object sender, EventArgs e)
        //{
        //    Global.NutOKCount = Convert.ToInt32(lb_Materiel_OK.Text);
        //    Global.inidata.productconfig.NutOKCount = lb_Materiel_OK.Text;
        //    Global.inidata.WriteProductnumSection();
        //}

        private void lb_ProcessControlOK_TextChanged(object sender, EventArgs e)
        {
            //Global.Product_num_Process_ok = Convert.ToInt32(lb_ProcessControlOK.Text);
            //Global.inidata.productconfig.product_num_process_ok = lb_ProcessControlOK.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void lb_ProcessControlNG_TextChanged(object sender, EventArgs e)
        {
            //Global.Product_num_Process_ng = Convert.ToInt32(lb_ProcessControlNG.Text);
            //Global.inidata.productconfig.product_num_process_ng = lb_ProcessControlNG.Text;
            //Global.inidata.WriteProductnumSection();
        }

        private void OEE_upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(OEE_index.Text) > 0 && Convert.ToInt32(OEE_index.Text) <= Global.ed.Count)
                {
                    string IP = string.Empty;
                    string Mac = string.Empty;
                    string OEE_DT = "";
                    string msg = "";
                    string OEE_DT1 = "";
                    string msg1 = "";
                    IP = _mainparent.GetIp();
                    Mac = _mainparent.GetMac();
                    int i = Convert.ToInt32(OEE_index.Text);
                    if (Global.ed[i].errorinfo != "")
                    {
                        DateTime time = DateTime.Now;
                        OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[i].errorStatus, Global.ed[i].errorCode, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), Global.ed[i].ModuleCode);
                        var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);
                        if (rst)
                        {
                            //rtx_OEEMsg.AppendText(OEE_DT + ",自动发送成功" + "\r\n");
                            AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送成功", "rtx_OEEMsg");
                            //Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                            //Global.ConnectOEEFlag = true;
                        }
                        else
                        {
                            //rtx_OEEMsg.AppendText(OEE_DT + ",自动发送失败" + "\r\n");
                            AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送失败", "rtx_OEEMsg");

                            //rtx_OEEMsg.AppendText(Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",自动发送失败" + "\r\n");                           
                        }
                        Thread.Sleep(1000);
                        OEE_DT1 = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", "2", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "");
                        var rst1 = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT1, out msg1);
                    }
                    else
                    {
                        MessageBox.Show("请输入其他的整数！");
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(("请输入1至{0}的整数！"), Global.ed.Count));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("OEE手动上传失败，" + ex.ToString());
            }
        }

        private void OEE_uploadAll_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(UploadAll);
            th.IsBackground = true;
            th.Start();
        }

        private void UploadAll(object ob)
        {
            try
            {
                for (int i = 1; i < Global.ed.Count + 1; i++)
                {
                    string IP = string.Empty;
                    string Mac = string.Empty;
                    string OEE_DT = "";
                    string msg = "";
                    string OEE_DT1 = "";
                    string msg1 = "";
                    IP = _mainparent.GetIp();
                    Mac = _mainparent.GetMac();
                    if (Global.ed[i].errorinfo != "")
                    {
                        DateTime time = DateTime.Now;
                        OEE_DT = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", Global.ed[i].errorStatus, Global.ed[i].errorCode, time.ToString("yyyy-MM-dd HH:mm:ss.fff"), Global.ed[i].ModuleCode);
                        var rst = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT, out msg);
                        if (rst)
                        {
                            //rtx_OEEMsg.AppendText(OEE_DT + ",自动发送成功" + "\r\n");
                            AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送成功", "rtx_OEEMsg");
                            //Log.WriteLog("OEE_DT安全门打开自动errorCode发送成功" + ",OEELog");
                            //Global.ConnectOEEFlag = true;
                        }
                        else
                        {
                            //rtx_OEEMsg.AppendText(OEE_DT + ",自动发送失败" + "\r\n");
                            AppendRichText("故障代码=" + Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",模组代码:" + Global.ed[i].ModuleCode + ",自动发送失败", "rtx_OEEMsg");

                            //rtx_OEEMsg.AppendText(Global.ed[i].errorCode + ",触发时间=" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + ",运行状态:" + Global.ed[i].errorStatus + ",故障描述:" + Global.ed[i].errorinfo + ",自动发送失败" + "\r\n");                           
                        }
                        Thread.Sleep(1000);
                        //OEE_DT1 = string.Format("{{\"Status\":\"{0}\",\"ErrorCode\":\"{1}\",\"EventTime\":\"{2}\",\"ModuleCode\":\"{3}\"}}", "2", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "");
                        //var rst1 = RequestAPI.Request(Global.inidata.productconfig.OEE_URL1, Global.inidata.productconfig.OEE_URL2, IP, Mac, Global.inidata.productconfig.OEE_Dsn, Global.inidata.productconfig.OEE_authCode, 2, OEE_DT1, out msg1);
                    }
                    else
                    {
                        //MessageBox.Show("请输入其他的整数！");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("OEE手动上传失败，" + ex.ToString());
            }
        }

        private void btn_FixtureOut_Click(object sender, EventArgs e)
        {
            //if (list_AllFixture.SelectedIndex != -1)
            //{
            //    Global.FixtureOutID = list_AllFixture.SelectedItems[0].ToString();
            //    MessageBox.Show("操作成功！请等待治具排出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("请在下方列表中选中需要排出的治具！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //    if (Global.DataGridView_Select_RowIndex != -1)
            //    {
            //        string fixture_ng = grid_AllFixture.Rows[Global.DataGridView_Select_RowIndex].Cells[1].EditedFormattedValue.ToString();
            //        if (!Global._fixture_ng.Contains(fixture_ng))
            //        {
            //            Global._fixture_ng.Add(fixture_ng);
            //        }
            //        Txt.WriteLine2(Global._fixture_ng);
            //        string insertStr = string.Format("UPDATE [FixtureStatus] SET [Status] = '待保养' where [FixtureID] = '{0}'", fixture_ng);
            //        int r1 = SQL.ExecuteUpdate(insertStr);
            //        UpdateDataGridView();
            //        Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture_ng + "," + "治具手动排出", @"D:\ZHH\治具小保养记录\");
            //    }
        }

        private void btn_SelectFixture_Click(object sender, EventArgs e)
        {
            //if (txt_SelectFixture.Text != string.Empty)
            //{
            //    string Select = string.Format("SELECT * FROM FixtureNG  where Fixture='{0}' and cast(DateTime as datetime) >='{1}' and cast(DateTime as datetime) <='{2}' ", txt_SelectFixture.Text, Convert.ToDateTime(dtp_SelectFixture.Text).ToString("yyyy/MM/dd") + " 06:00:00", Convert.ToDateTime(dtp_SelectFixture.Text).AddDays(1).ToString("yyyy/MM/dd") + " 06:00:00");
            //    DataTable dt = SQL.ExecuteQuery(Select);//1、查找选择当天6：00-隔天6：00所有数据
            //    if (dt.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            list_FixtureNG.Items.Add(string.Format("{0}[治具码：{1},NG]", dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString()));
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("需要查询的治具SN不能为空！");
            //}
        }

        private void grid_AllFixture_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Global.DataGridView_Select_RowIndex = e.RowIndex;
            //for (int i = 0; i < grid_AllFixture.RowCount; i++)
            //{
            //    if (grid_AllFixture.Rows[i].Cells[5].EditedFormattedValue.ToString() == "待保养")
            //    {
            //        grid_AllFixture.Rows[i].Cells[5].Style.BackColor = Color.Red;
            //    }
            //}
            //if (grid_AllFixture.Columns[e.ColumnIndex].Name == "BtnModify" && grid_AllFixture.RowCount > 0 && e.RowIndex >= 0)
            //{
            //    //点击确认则会更新该治具为初始状态  
            //    string fixture = grid_AllFixture[1, e.RowIndex].Value.ToString();
            //    Global._fixture_ng.Remove(fixture);//在list中删除该治具
            //    Txt.WriteLine2(Global._fixture_ng);//在TXT中删除该治具
            //    string insertStr1 = string.Format("UPDATE [FixtureStatus] SET Time = '{0}',UsingTimes = '{1}', CountDown = '{2}', [Status] = '正常使用中' where [FixtureID] = '{3}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0", Global.Fixture_maintance_time, fixture);
            //    int r1 = SQL.ExecuteUpdate(insertStr1);
            //    UpdateDataGridView();//更新DataGridView
            //    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具保养完成", @"D:\ZHH\治具小保养记录\");
            //}
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //Global.project = CB_project.Text;
            //Global.station = CB_Station.Text;
            //Global.type = CB_Type.Text;
        }

        private void btn_Setting_Count_Click(object sender, EventArgs e)
        {
            //Global.Fixture_maintance_times = Convert.ToInt16(txt_Fixture_Mantain_Count.Text);
        }

        private void btn_Setting_Limit_Click(object sender, EventArgs e)
        {
            //Global.Fixture_ContinuationNG = Convert.ToInt16(txt_Fixture_ContinuationNG.Text);
            //Global.Fixture_CountNG = Convert.ToInt16(txt_Fixture_CountNG.Text);
        }

        private void btn_Outputfixture_Click(object sender, EventArgs e)
        {
            //if (list_FixtureNG.Items.Count == 0)
            //{
            //    MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //SaveFileDialog sf = new SaveFileDialog();
            //sf.Title = "文档导出";
            //sf.Filter = "文档(*.csv)|*.csv";
            //sf.FileName = DateTime.Now.Date.ToString("yyyyMMdd");
            //if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string str = sf.FileName;
            //    using (StreamWriter sw = new StreamWriter(str, false, Encoding.Default))
            //    {
            //        try
            //        {
            //            sw.WriteLine("时间,治具号,结果");
            //            for (int t = 0; t < list_FixtureNG.Items.Count; t++)
            //            {
            //                string oeestr = "";
            //                oeestr += list_FixtureNG.Items[t].ToString().Replace("[", ",");
            //                sw.WriteLine(oeestr);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        }
            //        sw.Close();
            //        sw.Dispose();
            //    }
            //}
        }

        private void grid_TossingFixture_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (grid_TossingFixture.Columns[e.ColumnIndex].Name == "BtnModify" && grid_TossingFixture.RowCount > 0 && e.RowIndex >= 0)
            //{
            //    //点击确认则会更新该治具为初始状态  
            //    string fixture = grid_TossingFixture[0, e.RowIndex].Value.ToString();
            //    Global._fixture_tossing_ng.Remove(fixture);//在list中删除该治具
            //    Txt.WriteLine3(Global._fixture_tossing_ng);//在TXT中删除该治具
            //    grid_TossingFixture.Rows[e.RowIndex].Cells[1].Value = "";
            //    grid_TossingFixture.Rows[e.RowIndex].Cells[2].Value = "0";
            //    grid_TossingFixture.Rows[e.RowIndex].Cells[3].Value = "0";
            //    grid_TossingFixture.Rows[e.RowIndex].Cells[4].Value = "OK";
            //    grid_TossingFixture.Rows[e.RowIndex].Cells[5].Value = "OK";
            //    string insertStr1 = string.Format("UPDATE [FixtureTossing] SET [TossingTime] = '{0}',[TossingContinuation] = '{1}', [TossingCount] = '{2}',[ContinuationNG] = '{3}',[CountNG] = '{4}' where [Fixture] = '{5}'",
            //                               "", "0", "0", "OK", "OK", fixture);
            //    DataTable dt = SQL.ExecuteQuery(insertStr1);
            //    Log.WriteCSV(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + fixture + "," + "治具维修完成", @"D:\ZHH\治具维修记录\");
            //    UpdateDataGridView();
            //}
        }
    }
}
