using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 卓汇数据追溯系统
{
    //设置Com对外可访问
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class DataStatisticsFrm : Form
    {
        private MainFrm _mainparent;
        private delegate void Labelvision(string bl, string Name);
        private delegate void ShowTxt(string txt, string Name);
        private delegate void ShowDGV(int rows, int cells, string value);
        private delegate void DTP(string bl, DateTimePicker Name);
        delegate void RefreachTable(Chart chart, string[] Point_X, double[] Point_Y, int index);
        List<Control> List_Control = new List<Control>();
        SQLServer SQL = new SQLServer();
        double[] UPH_Y_OK = new double[24];
        double[] UPH_Y_NG = new double[24];
        double[] ChartDT_Run = new double[24];
        double[] ChartDT_Error = new double[24];
        double[] ChartDT_Pending = new double[24];
        string[] x = new string[] { "06:00-07:00", "07:00-08:00", "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00", "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00", "17:00-18:00"
                , "18:00-19:00", "19:00-20:00", "20:00-21:00", "21:00-22:00", "22:00-23:00", "23:00-00:00", "00:00-01:00", "01:00-02:00", "02:00-03:00", "03:00-04:00", "04:00-05:00", "05:00-06:00" };
        public DataStatisticsFrm(MainFrm mdiParent)
        {
            InitializeComponent();
            _mainparent = mdiParent;
            dgv_D.Rows.Add(19);
            dgv_N.Rows.Add(19);
            //初始化浏览器
            this.initWebBrowser();
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

        private void DataStatisticsFrm_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage10);
            List_Control = GetAllControls(this);//列表中添加所有窗体控件
            chart_DT.Titles.Add("Hour DT");
            chart_DT.Titles[0].ForeColor = Color.Blue;
            chart_DT.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
            chart_DT.Titles[0].Alignment = ContentAlignment.TopCenter;
            chart_DT.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart_DT.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart_DT.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart_DT.ChartAreas[0].AxisX.Title = "时间段";
            chart_DT.ChartAreas[0].AxisY.Title = "持续时间(min)";

            chart_UPH.Titles.Add("UPH统计");
            chart_UPH.Titles[0].ForeColor = Color.Blue;
            chart_UPH.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
            chart_UPH.Titles[0].Alignment = ContentAlignment.TopCenter;
            chart_UPH.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart_UPH.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart_UPH.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart_UPH.ChartAreas[0].AxisX.Title = "时间";
            chart_UPH.ChartAreas[0].AxisY.Title = "UPH";

            chart_FixtureErrorNumber.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart_FixtureErrorNumber.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            //chart_FixtureErrorNumber.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            //chart_FixtureErrorNumber.ChartAreas[0].AxisX.Title = "时间";
            chart_FixtureErrorNumber.ChartAreas[0].AxisY.Title = "抛料次数";
            chart_FixtureErrorNumber.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_FixtureErrorNumber.Series[0].Label = "#VAL";                //设置显示X Y的值    
            chart_FixtureErrorNumber.Series[0].LabelForeColor = Color.Black;
            chart_FixtureErrorNumber.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_FixtureErrorNumber.Series[0].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_FixtureErrorNumber.Series[0].Color = Color.Lime;
            //chart_FixtureErrorNumber.Series[0].LegendText = legend.Name;
            chart_FixtureErrorNumber.Series[0].IsValueShownAsLabel = true;
            chart_FixtureErrorNumber.Series[0].CustomProperties = "DrawingStyle = Cylinder";
            chart_FixtureErrorNumber.Series[0].Palette = ChartColorPalette.BrightPastel;

            Legend legend_run = new Legend("运行时间");
            legend_run.Title = "legendTitle";
            Legend legend_error = new Legend("异常时间");
            legend_error.Title = "legendTitle";
            Legend legend_pending = new Legend("待料时间");
            legend_pending.Title = "legendTitle";
            Legend legend_UPH_OK = new Legend("OK数量");
            legend_UPH_OK.Title = "legendTitle";
            Legend legend_UPH_NG = new Legend("NG数量");
            legend_UPH_NG.Title = "legendTitle";
            chart_DT.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_DT.Series[0].Label = "#VAL";                //设置显示X Y的值    
            chart_DT.Series[0].LabelForeColor = Color.Black;
            chart_DT.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_DT.Series[0].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_DT.Series[0].Color = Color.DarkSeaGreen;
            chart_DT.Series[0].LegendText = legend_run.Name;
            chart_DT.Series[0].IsValueShownAsLabel = true;
            chart_DT.Legends.Add(legend_run);
            chart_DT.Legends[0].Position.Auto = true;
            chart_DT.Series[1].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_DT.Series[1].Label = "#VAL";                //设置显示X Y的值    
            chart_DT.Series[1].LabelForeColor = Color.Black;
            chart_DT.Series[1].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_DT.Series[1].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_DT.Series[1].Color = Color.Red;
            chart_DT.Series[1].LegendText = legend_error.Name;
            chart_DT.Series[1].IsValueShownAsLabel = true;
            chart_DT.Legends.Add(legend_error);
            chart_DT.Legends[1].Position.Auto = true;
            chart_DT.Series[2].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_DT.Series[2].Label = "#VAL";                //设置显示X Y的值    
            chart_DT.Series[2].LabelForeColor = Color.Black;
            chart_DT.Series[2].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_DT.Series[2].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_DT.Series[2].Color = Color.LightSkyBlue;
            chart_DT.Series[2].LegendText = legend_pending.Name;
            chart_DT.Series[2].IsValueShownAsLabel = true;
            chart_DT.Legends.Add(legend_pending);
            chart_DT.Legends[2].Position.Auto = true;
            chart_UPH.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_UPH.Series[0].Label = "#VAL";                //设置显示X Y的值    
            chart_UPH.Series[0].LabelForeColor = Color.Black;
            chart_UPH.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_UPH.Series[0].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_UPH.Series[0].Color = Color.DarkSeaGreen;
            chart_UPH.Series[0].LegendText = legend_UPH_OK.Name;
            chart_UPH.Series[0].IsValueShownAsLabel = true;
            chart_UPH.Legends.Add(legend_UPH_OK);
            chart_UPH.Legends[0].Position.Auto = true;
            chart_UPH.Series[1].XValueType = ChartValueType.String;  //设置X轴上的值类型
            chart_UPH.Series[1].Label = "#VAL";                //设置显示X Y的值    
            chart_UPH.Series[1].LabelForeColor = Color.Black;
            chart_UPH.Series[1].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
            chart_UPH.Series[1].ChartType = SeriesChartType.Column;    //图类型(柱状)
            chart_UPH.Series[1].Color = Color.Red;
            chart_UPH.Series[1].LegendText = legend_UPH_NG.Name;
            chart_UPH.Series[1].IsValueShownAsLabel = true;
            chart_UPH.Legends.Add(legend_UPH_NG);
            chart_UPH.Legends[1].Position.Auto = true;
            chart_DT.Legends[0].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(80, 0, 10, 10);
            chart_UPH.Legends[0].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(80, 0, 10, 10);
            //绑定数据
            //chart_DT.Series[0].Points.DataBindXY(x, y1);
            //chart_DT.Series[1].Points.DataBindXY(x, y2);
            //chart_DT.Series[2].Points.DataBindXY(x, y3);
            //chart_UPH.Series[0].Points.DataBindXY(x, y4);
            //chart_UPH.Series[1].Points.DataBindXY(x, y5);
            //chart_DT.Series[0].Palette = ChartColorPalette.BrightPastel;

            dgv_D.Rows[0].Cells[0].Value = "投入数量";
            dgv_D.Rows[1].Cells[0].Value = "小料投入数量";

            dgv_D.Rows[3].DefaultCellStyle.BackColor = Color.BlueViolet;
            dgv_D.Rows[3].Cells[0].Value = "1st Pass Yield";
            dgv_D.Rows[4].Cells[0].Value = "焊接参数获取NG";
            dgv_D.Rows[5].Cells[0].Value = "location1焊接NG";
            dgv_D.Rows[6].Cells[0].Value = "location2焊接NG";
            dgv_D.Rows[7].Cells[0].Value = "location3焊接NG";
            dgv_D.Rows[8].Cells[0].Value = "location4焊接NG";
            dgv_D.Rows[9].Cells[0].Value = "location5焊接NG";

            dgv_D.Rows[11].DefaultCellStyle.BackColor = Color.BlueViolet;
            dgv_D.Rows[11].Cells[0].Value = "PF";
            dgv_D.Rows[12].Cells[0].Value = "Trace上传失败";
            dgv_D.Rows[13].Cells[0].Value = "PDCA上传失败";
            dgv_D.Rows[14].Cells[0].Value = "Trace PV检查NG";
            dgv_D.Rows[15].Cells[0].Value = "HSG扫码失败";
            dgv_D.Rows[16].Cells[0].Value = "Bracket定位CCD抓拍NG";
            dgv_D.Rows[17].Cells[0].Value = "Bracket吸取失败";

            dgv_N.Rows[0].Cells[0].Value = "投入数量";
            dgv_N.Rows[1].Cells[0].Value = "小料投入数量";

            dgv_N.Rows[3].DefaultCellStyle.BackColor = Color.BlueViolet;
            dgv_N.Rows[3].Cells[0].Value = "1st Pass Yield";
            dgv_N.Rows[4].Cells[0].Value = "焊接参数获取NG";
            dgv_N.Rows[5].Cells[0].Value = "location1焊接NG";
            dgv_N.Rows[6].Cells[0].Value = "location2焊接NG";
            dgv_N.Rows[7].Cells[0].Value = "location3焊接NG";
            dgv_N.Rows[8].Cells[0].Value = "location4焊接NG";
            dgv_N.Rows[9].Cells[0].Value = "location5焊接NG";

            dgv_N.Rows[11].DefaultCellStyle.BackColor = Color.BlueViolet;
            dgv_N.Rows[11].Cells[0].Value = "PF";
            dgv_N.Rows[12].Cells[0].Value = "Trace上传失败";
            dgv_N.Rows[13].Cells[0].Value = "PDCA上传失败";
            dgv_N.Rows[14].Cells[0].Value = "Trace PV检查NG";
            dgv_N.Rows[15].Cells[0].Value = "HSG扫码失败";
            dgv_N.Rows[16].Cells[0].Value = "Bracket定位CCD抓拍NG";
            dgv_N.Rows[17].Cells[0].Value = "Bracket吸取失败";

            //dgv_N.Rows[0].Cells[0].Value = "投入数量";
            //dgv_N.Rows[1].Cells[0].Value = "小料投入数量";
            //dgv_N.Rows[3].DefaultCellStyle.BackColor = Color.BlueViolet;
            //dgv_N.Rows[3].Cells[0].Value = "1st Pass Yield";
            //dgv_N.Rows[4].Cells[0].Value = "焊接NG";
            //dgv_N.Rows[5].Cells[0].Value = "Trace上传失败";
            //dgv_N.Rows[6].Cells[0].Value = "PDCA上传失败";
            //dgv_N.Rows[8].DefaultCellStyle.BackColor = Color.BlueViolet;
            //dgv_N.Rows[8].Cells[0].Value = "PF";
            //dgv_N.Rows[9].Cells[0].Value = "Trace PV检查NG";
            //dgv_N.Rows[10].Cells[0].Value = "HSG扫码失败";
            //dgv_N.Rows[11].Cells[0].Value = "Bracket定位CCD抓拍NG";
            //dgv_N.Rows[12].Cells[0].Value = "Bracket吸取失败";
            //dgv_N.Rows[12].Cells[0].Value = "Bracket组装复检NG";
            Global.Smallmaterial_Input_D = Convert.ToInt32(Global.inidata.productconfig.Smallmaterial_Input_D);
            UpDataDGV_D(1, 1, Global.inidata.productconfig.Smallmaterial_Input_D);
            Global.Smallmaterial_Input_N = Convert.ToInt32(Global.inidata.productconfig.Smallmaterial_Input_N);
            UpDataDGV_N(1, 1, Global.inidata.productconfig.Smallmaterial_Input_N);
            Global.Welding_Error_D = Convert.ToInt32(Global.inidata.productconfig.Welding_Error_D);
            UpDataDGV_D(4, 1, Global.inidata.productconfig.Welding_Error_D);
            Global.Welding_Error_N = Convert.ToInt32(Global.inidata.productconfig.Welding_Error_N);
            UpDataDGV_N(4, 1, Global.inidata.productconfig.Welding_Error_N);
            Global.TraceUpLoad_Error_D = Convert.ToInt32(Global.inidata.productconfig.TraceUpLoad_Error_D);
            UpDataDGV_D(12, 1, Global.inidata.productconfig.TraceUpLoad_Error_D);
            Global.TraceUpLoad_Error_N = Convert.ToInt32(Global.inidata.productconfig.TraceUpLoad_Error_N);
            UpDataDGV_N(12, 1, Global.inidata.productconfig.TraceUpLoad_Error_N);
            Global.PDCAUpLoad_Error_D = Convert.ToInt32(Global.inidata.productconfig.PDCAUpLoad_Error_D);
            UpDataDGV_D(13, 1, Global.inidata.productconfig.PDCAUpLoad_Error_D);
            Global.PDCAUpLoad_Error_N = Convert.ToInt32(Global.inidata.productconfig.PDCAUpLoad_Error_N);
            UpDataDGV_N(13, 1, Global.inidata.productconfig.PDCAUpLoad_Error_N);
            Global.TracePVCheck_Error_D = Convert.ToInt32(Global.inidata.productconfig.TracePVCheck_Error_D);
            UpDataDGV_D(14, 1, Global.inidata.productconfig.TracePVCheck_Error_D);
            Global.TracePVCheck_Error_N = Convert.ToInt32(Global.inidata.productconfig.TracePVCheck_Error_N);
            UpDataDGV_N(14, 1, Global.inidata.productconfig.TracePVCheck_Error_N);
            Global.Location1_NG_D = Convert.ToInt32(Global.inidata.productconfig.Location1_NG_D);
            UpDataDGV_D(5, 1, Global.inidata.productconfig.Location1_NG_D);
            Global.Location1_NG_N = Convert.ToInt32(Global.inidata.productconfig.Location1_NG_N);
            UpDataDGV_N(5, 1, Global.inidata.productconfig.Location1_NG_N);
            Global.Location2_NG_D = Convert.ToInt32(Global.inidata.productconfig.Location2_NG_D);
            UpDataDGV_D(6, 1, Global.inidata.productconfig.Location2_NG_D);
            Global.Location2_NG_N = Convert.ToInt32(Global.inidata.productconfig.Location2_NG_N);
            UpDataDGV_N(6, 1, Global.inidata.productconfig.Location2_NG_N);
            Global.Location3_NG_D = Convert.ToInt32(Global.inidata.productconfig.Location3_NG_D);
            UpDataDGV_D(7, 1, Global.inidata.productconfig.Location3_NG_D);
            Global.Location3_NG_N = Convert.ToInt32(Global.inidata.productconfig.Location3_NG_N);
            UpDataDGV_N(7, 1, Global.inidata.productconfig.Location3_NG_N);
            Global.Location4_NG_D = Convert.ToInt32(Global.inidata.productconfig.Location4_NG_D);
            UpDataDGV_D(8, 1, Global.inidata.productconfig.Location4_NG_D);
            Global.Location4_NG_N = Convert.ToInt32(Global.inidata.productconfig.Location4_NG_N);
            UpDataDGV_N(8, 1, Global.inidata.productconfig.Location4_NG_N);
            Global.Location5_NG_D = Convert.ToInt32(Global.inidata.productconfig.Location5_NG_D);
            UpDataDGV_D(9, 1, Global.inidata.productconfig.Location5_NG_D);
            Global.Location5_NG_N = Convert.ToInt32(Global.inidata.productconfig.Location5_NG_N);
            UpDataDGV_N(9, 1, Global.inidata.productconfig.Location5_NG_N);

            Global.ReadBarcode_NG_D = Convert.ToInt32(Global.inidata.productconfig.ReadBarcode_NG_D);
            UpDataDGV_D(15, 1, Global.inidata.productconfig.ReadBarcode_NG_D);
            Global.ReadBarcode_NG_N = Convert.ToInt32(Global.inidata.productconfig.ReadBarcode_NG_N);
            UpDataDGV_N(15, 1, Global.inidata.productconfig.ReadBarcode_NG_N);
            Global.CCDCheck_Error_D = Convert.ToInt32(Global.inidata.productconfig.CCDCheck_Error_D);
            UpDataDGV_D(16, 1, Global.inidata.productconfig.CCDCheck_Error_D);
            Global.CCDCheck_Error_N = Convert.ToInt32(Global.inidata.productconfig.CCDCheck_Error_N);
            UpDataDGV_N(16, 1, Global.inidata.productconfig.CCDCheck_Error_N);
            Global.Smallmaterial_throwing_D = Convert.ToInt32(Global.inidata.productconfig.Smallmaterial_throwing_D);
            UpDataDGV_D(17, 1, Global.inidata.productconfig.Smallmaterial_throwing_D);
            Global.Smallmaterial_throwing_N = Convert.ToInt32(Global.inidata.productconfig.Smallmaterial_throwing_N);
            UpDataDGV_N(17, 1, Global.inidata.productconfig.Smallmaterial_throwing_N);
            _mainparent.dgv_AutoSize(dgv_D);
            _mainparent.dgv_AutoSize(dgv_N);
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

        public void UpDataText(string str, string Name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTxt(UpDataText), new object[] { str, Name });
                return;
            }
            foreach (Control ctrl in List_Control)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if (ctrl.Name == Name)
                    {
                        ctrl.Text = str;
                    }
                }
            }
        }

        public void UpDataDGV_D(int j, int i, string value)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDGV(UpDataDGV_D), new object[] { j, i, value });
                return;
            }
            dgv_D.Rows[j].Cells[i].Value = value;
        }
        public void UpDataDGV_N(int j, int i, string value)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowDGV(UpDataDGV_N), new object[] { j, i, value });
                return;
            }
            dgv_N.Rows[j].Cells[i].Value = value;
        }

        public void RefreshDTP(string value, DateTimePicker dtp)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DTP(RefreshDTP), new object[] { value, dtp });
                return;
            }
            dtp.Text = value;
        }

        public void RefreachData(Chart chart, string[] Point_X, double[] Point_Y, int index)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new RefreachTable(RefreachData), new object[] { chart, Point_X, Point_Y, index });
                return;
            }
            switch (index)
            {
                case 0:
                    chart.Series[0].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 1:
                    chart.Series[1].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 2:
                    chart.Series[0].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 3:
                    chart.Series[1].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 4:
                    chart.Series[2].Points.DataBindXY(Point_X, Point_Y);
                    break;
                case 100:
                    double[] point_y = new double[24];
                    for (int i = 0; i < point_y.Length; i++)
                    {
                        point_y[i] = 0;
                    }
                    chart.Series[0].Points.DataBindXY(x, point_y);
                    chart.Series[1].Points.DataBindXY(x, point_y);
                    break;
                case 101:
                    double[] point_Y = new double[24];
                    for (int i = 0; i < point_Y.Length; i++)
                    {
                        point_Y[i] = 0;
                    }
                    chart.Series[0].Points.DataBindXY(x, point_Y);
                    chart.Series[1].Points.DataBindXY(x, point_Y);
                    chart.Series[2].Points.DataBindXY(x, point_Y);
                    break;
                default:
                    break;
            }
        }

        public void UD_DataTable(object ob)
        {
            while (true)
            {
                try
                {
                    if (DateTime.Now.ToString("yyyy-MM-dd") == (Convert.ToDateTime(dtp_UPH_Table.Value)).ToString("yyyy-MM-dd") && DateTime.Now.ToString("yyyy-MM-dd") == (Convert.ToDateTime(dtp_UPH_Data.Value)).ToString("yyyy-MM-dd"))
                    {
                        UPH_Y_OK[0] = Convert.ToDouble(lb_Product_Total_08_09.Text) - Convert.ToDouble(lb_Product_NG_08_09.Text);
                        UPH_Y_OK[1] = Convert.ToDouble(lb_Product_Total_09_10.Text) - Convert.ToDouble(lb_Product_NG_09_10.Text);
                        UPH_Y_OK[2] = Convert.ToDouble(lb_Product_Total_10_11.Text) - Convert.ToDouble(lb_Product_NG_10_11.Text);
                        UPH_Y_OK[3] = Convert.ToDouble(lb_Product_Total_11_12.Text) - Convert.ToDouble(lb_Product_NG_11_12.Text);
                        UPH_Y_OK[4] = Convert.ToDouble(lb_Product_Total_12_13.Text) - Convert.ToDouble(lb_Product_NG_12_13.Text);
                        UPH_Y_OK[5] = Convert.ToDouble(lb_Product_Total_13_14.Text) - Convert.ToDouble(lb_Product_NG_13_14.Text);
                        UPH_Y_OK[6] = Convert.ToDouble(lb_Product_Total_14_15.Text) - Convert.ToDouble(lb_Product_NG_14_15.Text);
                        UPH_Y_OK[7] = Convert.ToDouble(lb_Product_Total_15_16.Text) - Convert.ToDouble(lb_Product_NG_15_16.Text);
                        UPH_Y_OK[8] = Convert.ToDouble(lb_Product_Total_16_17.Text) - Convert.ToDouble(lb_Product_NG_16_17.Text);
                        UPH_Y_OK[9] = Convert.ToDouble(lb_Product_Total_17_18.Text) - Convert.ToDouble(lb_Product_NG_17_18.Text);
                        UPH_Y_OK[10] = Convert.ToDouble(lb_Product_Total_18_19.Text) - Convert.ToDouble(lb_Product_NG_18_19.Text);
                        UPH_Y_OK[11] = Convert.ToDouble(lb_Product_Total_19_20.Text) - Convert.ToDouble(lb_Product_NG_19_20.Text);
                        UPH_Y_OK[12] = Convert.ToDouble(lb_Product_Total_20_21.Text) - Convert.ToDouble(lb_Product_NG_20_21.Text);
                        UPH_Y_OK[13] = Convert.ToDouble(lb_Product_Total_21_22.Text) - Convert.ToDouble(lb_Product_NG_21_22.Text);
                        UPH_Y_OK[14] = Convert.ToDouble(lb_Product_Total_22_23.Text) - Convert.ToDouble(lb_Product_NG_22_23.Text);
                        UPH_Y_OK[15] = Convert.ToDouble(lb_Product_Total_23_00.Text) - Convert.ToDouble(lb_Product_NG_23_00.Text);
                        UPH_Y_OK[16] = Convert.ToDouble(lb_Product_Total_00_01.Text) - Convert.ToDouble(lb_Product_NG_00_01.Text);
                        UPH_Y_OK[17] = Convert.ToDouble(lb_Product_Total_01_02.Text) - Convert.ToDouble(lb_Product_NG_01_02.Text);
                        UPH_Y_OK[18] = Convert.ToDouble(lb_Product_Total_02_03.Text) - Convert.ToDouble(lb_Product_NG_02_03.Text);
                        UPH_Y_OK[19] = Convert.ToDouble(lb_Product_Total_03_04.Text) - Convert.ToDouble(lb_Product_NG_03_04.Text);
                        UPH_Y_OK[20] = Convert.ToDouble(lb_Product_Total_04_05.Text) - Convert.ToDouble(lb_Product_NG_04_05.Text);
                        UPH_Y_OK[21] = Convert.ToDouble(lb_Product_Total_05_06.Text) - Convert.ToDouble(lb_Product_NG_05_06.Text);
                        UPH_Y_OK[22] = Convert.ToDouble(lb_Product_Total_06_07.Text) - Convert.ToDouble(lb_Product_NG_06_07.Text);
                        UPH_Y_OK[23] = Convert.ToDouble(lb_Product_Total_07_08.Text) - Convert.ToDouble(lb_Product_NG_07_08.Text);
                        UPH_Y_NG[0] = Convert.ToDouble(lb_Product_NG_08_09.Text);
                        UPH_Y_NG[1] = Convert.ToDouble(lb_Product_NG_09_10.Text);
                        UPH_Y_NG[2] = Convert.ToDouble(lb_Product_NG_10_11.Text);
                        UPH_Y_NG[3] = Convert.ToDouble(lb_Product_NG_11_12.Text);
                        UPH_Y_NG[4] = Convert.ToDouble(lb_Product_NG_12_13.Text);
                        UPH_Y_NG[5] = Convert.ToDouble(lb_Product_NG_13_14.Text);
                        UPH_Y_NG[6] = Convert.ToDouble(lb_Product_NG_14_15.Text);
                        UPH_Y_NG[7] = Convert.ToDouble(lb_Product_NG_15_16.Text);
                        UPH_Y_NG[8] = Convert.ToDouble(lb_Product_NG_16_17.Text);
                        UPH_Y_NG[9] = Convert.ToDouble(lb_Product_NG_17_18.Text);
                        UPH_Y_NG[10] = Convert.ToDouble(lb_Product_NG_18_19.Text);
                        UPH_Y_NG[11] = Convert.ToDouble(lb_Product_NG_19_20.Text);
                        UPH_Y_NG[12] = Convert.ToDouble(lb_Product_NG_20_21.Text);
                        UPH_Y_NG[13] = Convert.ToDouble(lb_Product_NG_21_22.Text);
                        UPH_Y_NG[14] = Convert.ToDouble(lb_Product_NG_22_23.Text);
                        UPH_Y_NG[15] = Convert.ToDouble(lb_Product_NG_23_00.Text);
                        UPH_Y_NG[16] = Convert.ToDouble(lb_Product_NG_00_01.Text);
                        UPH_Y_NG[17] = Convert.ToDouble(lb_Product_NG_01_02.Text);
                        UPH_Y_NG[18] = Convert.ToDouble(lb_Product_NG_02_03.Text);
                        UPH_Y_NG[19] = Convert.ToDouble(lb_Product_NG_03_04.Text);
                        UPH_Y_NG[20] = Convert.ToDouble(lb_Product_NG_04_05.Text);
                        UPH_Y_NG[21] = Convert.ToDouble(lb_Product_NG_05_06.Text);
                        UPH_Y_NG[22] = Convert.ToDouble(lb_Product_NG_06_07.Text);
                        UPH_Y_NG[23] = Convert.ToDouble(lb_Product_NG_07_08.Text);
                        RefreachData(chart_UPH, x, UPH_Y_OK, 0);
                        RefreachData(chart_UPH, x, UPH_Y_NG, 1);
                    }
                    if (DateTime.Now.ToString("yyyy-MM-dd") == (Convert.ToDateTime(dtp_DT_Table.Value)).ToString("yyyy-MM-dd") && DateTime.Now.ToString("yyyy-MM-dd") == (Convert.ToDateTime(dtp_DT_Data.Value)).ToString("yyyy-MM-dd"))
                    {
                        ChartDT_Run[0] = Convert.ToDouble(lb_RunTime_08_09.Text);
                        ChartDT_Run[1] = Convert.ToDouble(lb_RunTime_09_10.Text);
                        ChartDT_Run[2] = Convert.ToDouble(lb_RunTime_10_11.Text);
                        ChartDT_Run[3] = Convert.ToDouble(lb_RunTime_11_12.Text);
                        ChartDT_Run[4] = Convert.ToDouble(lb_RunTime_12_13.Text);
                        ChartDT_Run[5] = Convert.ToDouble(lb_RunTime_13_14.Text);
                        ChartDT_Run[6] = Convert.ToDouble(lb_RunTime_14_15.Text);
                        ChartDT_Run[7] = Convert.ToDouble(lb_RunTime_15_16.Text);
                        ChartDT_Run[8] = Convert.ToDouble(lb_RunTime_16_17.Text);
                        ChartDT_Run[9] = Convert.ToDouble(lb_RunTime_17_18.Text);
                        ChartDT_Run[10] = Convert.ToDouble(lb_RunTime_18_19.Text);
                        ChartDT_Run[11] = Convert.ToDouble(lb_RunTime_19_20.Text);
                        ChartDT_Run[12] = Convert.ToDouble(lb_RunTime_20_21.Text);
                        ChartDT_Run[13] = Convert.ToDouble(lb_RunTime_21_22.Text);
                        ChartDT_Run[14] = Convert.ToDouble(lb_RunTime_22_23.Text);
                        ChartDT_Run[15] = Convert.ToDouble(lb_RunTime_23_00.Text);
                        ChartDT_Run[16] = Convert.ToDouble(lb_RunTime_00_01.Text);
                        ChartDT_Run[17] = Convert.ToDouble(lb_RunTime_01_02.Text);
                        ChartDT_Run[18] = Convert.ToDouble(lb_RunTime_02_03.Text);
                        ChartDT_Run[19] = Convert.ToDouble(lb_RunTime_03_04.Text);
                        ChartDT_Run[20] = Convert.ToDouble(lb_RunTime_04_05.Text);
                        ChartDT_Run[21] = Convert.ToDouble(lb_RunTime_05_06.Text);
                        ChartDT_Run[22] = Convert.ToDouble(lb_RunTime_06_07.Text);
                        ChartDT_Run[23] = Convert.ToDouble(lb_RunTime_07_08.Text);
                        ChartDT_Error[0] = Convert.ToDouble(lb_ErrorTime_08_09.Text);
                        ChartDT_Error[1] = Convert.ToDouble(lb_ErrorTime_09_10.Text);
                        ChartDT_Error[2] = Convert.ToDouble(lb_ErrorTime_10_11.Text);
                        ChartDT_Error[3] = Convert.ToDouble(lb_ErrorTime_11_12.Text);
                        ChartDT_Error[4] = Convert.ToDouble(lb_ErrorTime_12_13.Text);
                        ChartDT_Error[5] = Convert.ToDouble(lb_ErrorTime_13_14.Text);
                        ChartDT_Error[6] = Convert.ToDouble(lb_ErrorTime_14_15.Text);
                        ChartDT_Error[7] = Convert.ToDouble(lb_ErrorTime_15_16.Text);
                        ChartDT_Error[8] = Convert.ToDouble(lb_ErrorTime_16_17.Text);
                        ChartDT_Error[9] = Convert.ToDouble(lb_ErrorTime_17_18.Text);
                        ChartDT_Error[10] = Convert.ToDouble(lb_ErrorTime_18_19.Text);
                        ChartDT_Error[11] = Convert.ToDouble(lb_ErrorTime_19_20.Text);
                        ChartDT_Error[12] = Convert.ToDouble(lb_ErrorTime_20_21.Text);
                        ChartDT_Error[13] = Convert.ToDouble(lb_ErrorTime_21_22.Text);
                        ChartDT_Error[14] = Convert.ToDouble(lb_ErrorTime_22_23.Text);
                        ChartDT_Error[15] = Convert.ToDouble(lb_ErrorTime_23_00.Text);
                        ChartDT_Error[16] = Convert.ToDouble(lb_ErrorTime_00_01.Text);
                        ChartDT_Error[17] = Convert.ToDouble(lb_ErrorTime_01_02.Text);
                        ChartDT_Error[18] = Convert.ToDouble(lb_ErrorTime_02_03.Text);
                        ChartDT_Error[19] = Convert.ToDouble(lb_ErrorTime_03_04.Text);
                        ChartDT_Error[20] = Convert.ToDouble(lb_ErrorTime_04_05.Text);
                        ChartDT_Error[21] = Convert.ToDouble(lb_ErrorTime_05_06.Text);
                        ChartDT_Error[22] = Convert.ToDouble(lb_ErrorTime_06_07.Text);
                        ChartDT_Error[23] = Convert.ToDouble(lb_ErrorTime_07_08.Text);
                        ChartDT_Pending[0] = Convert.ToDouble(lb_PendingTime_08_09.Text);
                        ChartDT_Pending[1] = Convert.ToDouble(lb_PendingTime_09_10.Text);
                        ChartDT_Pending[2] = Convert.ToDouble(lb_PendingTime_10_11.Text);
                        ChartDT_Pending[3] = Convert.ToDouble(lb_PendingTime_11_12.Text);
                        ChartDT_Pending[4] = Convert.ToDouble(lb_PendingTime_12_13.Text);
                        ChartDT_Pending[5] = Convert.ToDouble(lb_PendingTime_13_14.Text);
                        ChartDT_Pending[6] = Convert.ToDouble(lb_PendingTime_14_15.Text);
                        ChartDT_Pending[7] = Convert.ToDouble(lb_PendingTime_15_16.Text);
                        ChartDT_Pending[8] = Convert.ToDouble(lb_PendingTime_16_17.Text);
                        ChartDT_Pending[9] = Convert.ToDouble(lb_PendingTime_17_18.Text);
                        ChartDT_Pending[10] = Convert.ToDouble(lb_PendingTime_18_19.Text);
                        ChartDT_Pending[11] = Convert.ToDouble(lb_PendingTime_19_20.Text);
                        ChartDT_Pending[12] = Convert.ToDouble(lb_PendingTime_20_21.Text);
                        ChartDT_Pending[13] = Convert.ToDouble(lb_PendingTime_21_22.Text);
                        ChartDT_Pending[14] = Convert.ToDouble(lb_PendingTime_22_23.Text);
                        ChartDT_Pending[15] = Convert.ToDouble(lb_PendingTime_23_00.Text);
                        ChartDT_Pending[16] = Convert.ToDouble(lb_PendingTime_00_01.Text);
                        ChartDT_Pending[17] = Convert.ToDouble(lb_PendingTime_01_02.Text);
                        ChartDT_Pending[18] = Convert.ToDouble(lb_PendingTime_02_03.Text);
                        ChartDT_Pending[19] = Convert.ToDouble(lb_PendingTime_03_04.Text);
                        ChartDT_Pending[20] = Convert.ToDouble(lb_PendingTime_04_05.Text);
                        ChartDT_Pending[21] = Convert.ToDouble(lb_PendingTime_05_06.Text);
                        ChartDT_Pending[22] = Convert.ToDouble(lb_PendingTime_06_07.Text);
                        ChartDT_Pending[23] = Convert.ToDouble(lb_PendingTime_07_08.Text);
                        RefreachData(chart_DT, x, ChartDT_Run, 2);
                        RefreachData(chart_DT, x, ChartDT_Error, 3);
                        RefreachData(chart_DT, x, ChartDT_Pending, 4);
                    }

                    if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 1)//定时更新日期
                    {
                        RefreshDTP(DateTime.Now.ToString("yyyy-MM-dd"), dtp_UPH_Table);
                        RefreshDTP(DateTime.Now.ToString("yyyy-MM-dd"), dtp_UPH_Data);
                        RefreshDTP(DateTime.Now.ToString("yyyy-MM-dd"), dtp_DT_Table);
                        RefreshDTP(DateTime.Now.ToString("yyyy-MM-dd"), dtp_DT_Data);
                        Log.WriteLog("刷新日期时间" + DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("更新数据异常" + ex.ToString().Replace("\r\n", ""));
                }
                Thread.Sleep(500);
            }
        }

        private void btn_SelectProductTable_Click(object sender, EventArgs e)
        {
            string SelectStr = string.Format("select * from Product where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_UPH_Table.Value)).ToString("yyyy-MM-dd"), "OK产量");
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            string SelectStr2 = string.Format("select * from Product where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_UPH_Table.Value)).ToString("yyyy-MM-dd"), "NG产量");
            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
            double[] point_OK = new double[24];
            double[] point_NG = new double[24];
            if (d1.Rows.Count > 0 && d2.Rows.Count > 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    point_OK[i] = Convert.ToDouble(d1.Rows[0][i + 3].ToString());
                    point_NG[i] = Convert.ToDouble(d2.Rows[0][i + 3].ToString());
                }
                chart_UPH.Series[0].Points.DataBindXY(x, point_OK);
                chart_UPH.Series[1].Points.DataBindXY(x, point_NG);
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    point_OK[i] = 0;
                    point_NG[i] = 0;
                }
                chart_UPH.Series[0].Points.DataBindXY(x, point_OK);
                chart_UPH.Series[1].Points.DataBindXY(x, point_NG);
            }
        }

        private void btn_SelectProductData_Click(object sender, EventArgs e)
        {
            string SelectStr = string.Format("select * from Product where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_UPH_Data.Value)).ToString("yyyy-MM-dd"), "OK产量");
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            string SelectStr2 = string.Format("select * from Product where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_UPH_Data.Value)).ToString("yyyy-MM-dd"), "NG产量");
            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
            string SelectStr3 = string.Format("select * from ErrorDataStatistics where DateTime = '{0}' ", (Convert.ToDateTime(dtp_UPH_Data.Value)).ToString("yyyy-MM-dd"));
            DataTable d3 = SQL.ExecuteQuery(SelectStr3);
            if (d1.Rows.Count > 0 && d2.Rows.Count > 0)
            {
                lb_Product_Total_08_09.Text = (Convert.ToDouble(d1.Rows[0][3].ToString()) + Convert.ToDouble(d2.Rows[0][3].ToString())).ToString();
                lb_Product_Total_09_10.Text = (Convert.ToDouble(d1.Rows[0][4].ToString()) + Convert.ToDouble(d2.Rows[0][4].ToString())).ToString();
                lb_Product_Total_10_11.Text = (Convert.ToDouble(d1.Rows[0][5].ToString()) + Convert.ToDouble(d2.Rows[0][5].ToString())).ToString();
                lb_Product_Total_11_12.Text = (Convert.ToDouble(d1.Rows[0][6].ToString()) + Convert.ToDouble(d2.Rows[0][6].ToString())).ToString();
                lb_Product_Total_12_13.Text = (Convert.ToDouble(d1.Rows[0][7].ToString()) + Convert.ToDouble(d2.Rows[0][7].ToString())).ToString();
                lb_Product_Total_13_14.Text = (Convert.ToDouble(d1.Rows[0][8].ToString()) + Convert.ToDouble(d2.Rows[0][8].ToString())).ToString();
                lb_Product_Total_14_15.Text = (Convert.ToDouble(d1.Rows[0][9].ToString()) + Convert.ToDouble(d2.Rows[0][9].ToString())).ToString();
                lb_Product_Total_15_16.Text = (Convert.ToDouble(d1.Rows[0][10].ToString()) + Convert.ToDouble(d2.Rows[0][10].ToString())).ToString();
                lb_Product_Total_16_17.Text = (Convert.ToDouble(d1.Rows[0][11].ToString()) + Convert.ToDouble(d2.Rows[0][11].ToString())).ToString();
                lb_Product_Total_17_18.Text = (Convert.ToDouble(d1.Rows[0][12].ToString()) + Convert.ToDouble(d2.Rows[0][12].ToString())).ToString();
                lb_Product_Total_18_19.Text = (Convert.ToDouble(d1.Rows[0][13].ToString()) + Convert.ToDouble(d2.Rows[0][13].ToString())).ToString();
                lb_Product_Total_19_20.Text = (Convert.ToDouble(d1.Rows[0][14].ToString()) + Convert.ToDouble(d2.Rows[0][14].ToString())).ToString();
                lb_Product_Total_20_21.Text = (Convert.ToDouble(d1.Rows[0][15].ToString()) + Convert.ToDouble(d2.Rows[0][15].ToString())).ToString();
                lb_Product_Total_21_22.Text = (Convert.ToDouble(d1.Rows[0][16].ToString()) + Convert.ToDouble(d2.Rows[0][16].ToString())).ToString();
                lb_Product_Total_22_23.Text = (Convert.ToDouble(d1.Rows[0][17].ToString()) + Convert.ToDouble(d2.Rows[0][17].ToString())).ToString();
                lb_Product_Total_23_00.Text = (Convert.ToDouble(d1.Rows[0][18].ToString()) + Convert.ToDouble(d2.Rows[0][18].ToString())).ToString();
                lb_Product_Total_00_01.Text = (Convert.ToDouble(d1.Rows[0][19].ToString()) + Convert.ToDouble(d2.Rows[0][19].ToString())).ToString();
                lb_Product_Total_01_02.Text = (Convert.ToDouble(d1.Rows[0][20].ToString()) + Convert.ToDouble(d2.Rows[0][20].ToString())).ToString();
                lb_Product_Total_02_03.Text = (Convert.ToDouble(d1.Rows[0][21].ToString()) + Convert.ToDouble(d2.Rows[0][21].ToString())).ToString();
                lb_Product_Total_03_04.Text = (Convert.ToDouble(d1.Rows[0][22].ToString()) + Convert.ToDouble(d2.Rows[0][22].ToString())).ToString();
                lb_Product_Total_04_05.Text = (Convert.ToDouble(d1.Rows[0][23].ToString()) + Convert.ToDouble(d2.Rows[0][23].ToString())).ToString();
                lb_Product_Total_05_06.Text = (Convert.ToDouble(d1.Rows[0][24].ToString()) + Convert.ToDouble(d2.Rows[0][24].ToString())).ToString();
                lb_Product_Total_06_07.Text = (Convert.ToDouble(d1.Rows[0][25].ToString()) + Convert.ToDouble(d2.Rows[0][25].ToString())).ToString();
                lb_Product_Total_07_08.Text = (Convert.ToDouble(d1.Rows[0][26].ToString()) + Convert.ToDouble(d2.Rows[0][26].ToString())).ToString();
                lb_Product_NG_08_09.Text = d2.Rows[0][3].ToString();
                lb_Product_NG_09_10.Text = d2.Rows[0][4].ToString();
                lb_Product_NG_10_11.Text = d2.Rows[0][5].ToString();
                lb_Product_NG_11_12.Text = d2.Rows[0][6].ToString();
                lb_Product_NG_12_13.Text = d2.Rows[0][7].ToString();
                lb_Product_NG_13_14.Text = d2.Rows[0][8].ToString();
                lb_Product_NG_14_15.Text = d2.Rows[0][9].ToString();
                lb_Product_NG_15_16.Text = d2.Rows[0][10].ToString();
                lb_Product_NG_16_17.Text = d2.Rows[0][11].ToString();
                lb_Product_NG_17_18.Text = d2.Rows[0][12].ToString();
                lb_Product_NG_18_19.Text = d2.Rows[0][13].ToString();
                lb_Product_NG_19_20.Text = d2.Rows[0][14].ToString();
                lb_Product_NG_20_21.Text = d2.Rows[0][15].ToString();
                lb_Product_NG_21_22.Text = d2.Rows[0][16].ToString();
                lb_Product_NG_22_23.Text = d2.Rows[0][17].ToString();
                lb_Product_NG_23_00.Text = d2.Rows[0][18].ToString();
                lb_Product_NG_00_01.Text = d2.Rows[0][19].ToString();
                lb_Product_NG_01_02.Text = d2.Rows[0][20].ToString();
                lb_Product_NG_02_03.Text = d2.Rows[0][21].ToString();
                lb_Product_NG_03_04.Text = d2.Rows[0][22].ToString();
                lb_Product_NG_04_05.Text = d2.Rows[0][23].ToString();
                lb_Product_NG_05_06.Text = d2.Rows[0][24].ToString();
                lb_Product_NG_06_07.Text = d2.Rows[0][25].ToString();
                lb_Product_NG_07_08.Text = d2.Rows[0][26].ToString();


                ///20210814
                /// 
                DayJd.Text = (Convert.ToDouble(d1.Rows[0][27])/100).ToString()+"%";
                NightJd.Text = (Convert.ToDouble(d1.Rows[0][28])/100).ToString()+"%";
                

                if (lb_Product_Total_08_09.Text == "0")
                {
                    lb_Product_Lianglv_08_09.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_08_09.Text = ((Convert.ToDouble(d1.Rows[0][3].ToString()) / (Convert.ToDouble(d1.Rows[0][3].ToString()) + Convert.ToDouble(d2.Rows[0][3].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_09_10.Text == "0")
                {
                    lb_Product_Lianglv_09_10.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_09_10.Text = ((Convert.ToDouble(d1.Rows[0][4].ToString()) / (Convert.ToDouble(d1.Rows[0][4].ToString()) + Convert.ToDouble(d2.Rows[0][4].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_10_11.Text == "0")
                {
                    lb_Product_Lianglv_10_11.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_10_11.Text = ((Convert.ToDouble(d1.Rows[0][5].ToString()) / (Convert.ToDouble(d1.Rows[0][5].ToString()) + Convert.ToDouble(d2.Rows[0][5].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_11_12.Text == "0")
                {
                    lb_Product_Lianglv_11_12.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_11_12.Text = ((Convert.ToDouble(d1.Rows[0][6].ToString()) / (Convert.ToDouble(d1.Rows[0][6].ToString()) + Convert.ToDouble(d2.Rows[0][6].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_12_13.Text == "0")
                {
                    lb_Product_Lianglv_12_13.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_12_13.Text = ((Convert.ToDouble(d1.Rows[0][7].ToString()) / (Convert.ToDouble(d1.Rows[0][7].ToString()) + Convert.ToDouble(d2.Rows[0][7].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_13_14.Text == "0")
                {
                    lb_Product_Lianglv_13_14.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_13_14.Text = ((Convert.ToDouble(d1.Rows[0][8].ToString()) / (Convert.ToDouble(d1.Rows[0][8].ToString()) + Convert.ToDouble(d2.Rows[0][8].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_14_15.Text == "0")
                {
                    lb_Product_Lianglv_14_15.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_14_15.Text = ((Convert.ToDouble(d1.Rows[0][9].ToString()) / (Convert.ToDouble(d1.Rows[0][9].ToString()) + Convert.ToDouble(d2.Rows[0][9].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_15_16.Text == "0")
                {
                    lb_Product_Lianglv_15_16.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_15_16.Text = ((Convert.ToDouble(d1.Rows[0][10].ToString()) / (Convert.ToDouble(d1.Rows[0][10].ToString()) + Convert.ToDouble(d2.Rows[0][10].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_16_17.Text == "0")
                {
                    lb_Product_Lianglv_16_17.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_16_17.Text = ((Convert.ToDouble(d1.Rows[0][11].ToString()) / (Convert.ToDouble(d1.Rows[0][11].ToString()) + Convert.ToDouble(d2.Rows[0][11].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_17_18.Text == "0")
                {
                    lb_Product_Lianglv_17_18.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_17_18.Text = ((Convert.ToDouble(d1.Rows[0][12].ToString()) / (Convert.ToDouble(d1.Rows[0][12].ToString()) + Convert.ToDouble(d2.Rows[0][12].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_18_19.Text == "0")
                {
                    lb_Product_Lianglv_18_19.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_18_19.Text = ((Convert.ToDouble(d1.Rows[0][13].ToString()) / (Convert.ToDouble(d1.Rows[0][13].ToString()) + Convert.ToDouble(d2.Rows[0][13].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_19_20.Text == "0")
                {
                    lb_Product_Lianglv_19_20.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_19_20.Text = ((Convert.ToDouble(d1.Rows[0][14].ToString()) / (Convert.ToDouble(d1.Rows[0][14].ToString()) + Convert.ToDouble(d2.Rows[0][14].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_20_21.Text == "0")
                {
                    lb_Product_Lianglv_20_21.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_20_21.Text = ((Convert.ToDouble(d1.Rows[0][15].ToString()) / (Convert.ToDouble(d1.Rows[0][15].ToString()) + Convert.ToDouble(d2.Rows[0][15].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_21_22.Text == "0")
                {
                    lb_Product_Lianglv_21_22.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_21_22.Text = ((Convert.ToDouble(d1.Rows[0][16].ToString()) / (Convert.ToDouble(d1.Rows[0][16].ToString()) + Convert.ToDouble(d2.Rows[0][16].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_22_23.Text == "0")
                {
                    lb_Product_Lianglv_22_23.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_22_23.Text = ((Convert.ToDouble(d1.Rows[0][17].ToString()) / (Convert.ToDouble(d1.Rows[0][17].ToString()) + Convert.ToDouble(d2.Rows[0][17].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_23_00.Text == "0")
                {
                    lb_Product_Lianglv_23_00.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_23_00.Text = ((Convert.ToDouble(d1.Rows[0][18].ToString()) / (Convert.ToDouble(d1.Rows[0][18].ToString()) + Convert.ToDouble(d2.Rows[0][18].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_00_01.Text == "0")
                {
                    lb_Product_Lianglv_00_01.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_00_01.Text = ((Convert.ToDouble(d1.Rows[0][19].ToString()) / (Convert.ToDouble(d1.Rows[0][19].ToString()) + Convert.ToDouble(d2.Rows[0][19].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_01_02.Text == "0")
                {
                    lb_Product_Lianglv_01_02.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_01_02.Text = ((Convert.ToDouble(d1.Rows[0][20].ToString()) / (Convert.ToDouble(d1.Rows[0][20].ToString()) + Convert.ToDouble(d2.Rows[0][20].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_02_03.Text == "0")
                {
                    lb_Product_Lianglv_02_03.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_02_03.Text = ((Convert.ToDouble(d1.Rows[0][21].ToString()) / (Convert.ToDouble(d1.Rows[0][21].ToString()) + Convert.ToDouble(d2.Rows[0][21].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_03_04.Text == "0")
                {
                    lb_Product_Lianglv_03_04.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_03_04.Text = ((Convert.ToDouble(d1.Rows[0][22].ToString()) / (Convert.ToDouble(d1.Rows[0][22].ToString()) + Convert.ToDouble(d2.Rows[0][22].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_04_05.Text == "0")
                {
                    lb_Product_Lianglv_04_05.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_04_05.Text = ((Convert.ToDouble(d1.Rows[0][23].ToString()) / (Convert.ToDouble(d1.Rows[0][23].ToString()) + Convert.ToDouble(d2.Rows[0][23].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_05_06.Text == "0")
                {
                    lb_Product_Lianglv_05_06.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_05_06.Text = ((Convert.ToDouble(d1.Rows[0][24].ToString()) / (Convert.ToDouble(d1.Rows[0][24].ToString()) + Convert.ToDouble(d2.Rows[0][24].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_06_07.Text == "0")
                {
                    lb_Product_Lianglv_06_07.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_06_07.Text = ((Convert.ToDouble(d1.Rows[0][25].ToString()) / (Convert.ToDouble(d1.Rows[0][25].ToString()) + Convert.ToDouble(d2.Rows[0][25].ToString()))) * 100).ToString("0.00") + "%";
                }
                if (lb_Product_Total_07_08.Text == "0")
                {
                    lb_Product_Lianglv_07_08.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_07_08.Text = ((Convert.ToDouble(d1.Rows[0][26].ToString()) / (Convert.ToDouble(d1.Rows[0][26].ToString()) + Convert.ToDouble(d2.Rows[0][26].ToString()))) * 100).ToString("0.00") + "%";
                }
                double total_day = 0;
                double ok_day = 0;
                double ng_day = 0;
                double total_night = 0;
                double ok_night = 0;
                double ng_night = 0;
                for (int i = 0; i < 12; i++)
                {
                    total_day += Convert.ToDouble(d1.Rows[0][i + 3].ToString()) + Convert.ToDouble(d2.Rows[0][i + 3].ToString());
                    total_night += Convert.ToDouble(d1.Rows[0][i + 15].ToString()) + Convert.ToDouble(d2.Rows[0][i + 15].ToString());
                    ok_day += Convert.ToDouble(d1.Rows[0][i + 3].ToString());
                    ok_night += Convert.ToDouble(d1.Rows[0][i + 15].ToString());
                    ng_day += Convert.ToDouble(d2.Rows[0][i + 3].ToString());
                    ng_night += Convert.ToDouble(d2.Rows[0][i + 15].ToString());
                }
                lb_Product_Total_08_20.Text = total_day.ToString();
                lb_Product_NG_08_20.Text = ng_day.ToString();
                if (total_day == 0)
                {
                    lb_Product_Lianglv_08_20.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_08_20.Text = ((ok_day / total_day) * 100).ToString("0.00") + "%";
                }
                lb_Product_Total_20_08.Text = total_night.ToString();
                lb_Product_NG_20_08.Text = ng_night.ToString();
                if (total_night == 0)
                {
                    lb_Product_Lianglv_20_08.Text = "0.00%";
                }
                else
                {
                    lb_Product_Lianglv_20_08.Text = ((ok_night / total_night) * 100).ToString("0.00") + "%";
                }


            }
            else
            {
                lb_Product_Total_08_09.Text = "0";
                lb_Product_Total_09_10.Text = "0";
                lb_Product_Total_10_11.Text = "0";
                lb_Product_Total_11_12.Text = "0";
                lb_Product_Total_12_13.Text = "0";
                lb_Product_Total_13_14.Text = "0";
                lb_Product_Total_14_15.Text = "0";
                lb_Product_Total_15_16.Text = "0";
                lb_Product_Total_16_17.Text = "0";
                lb_Product_Total_17_18.Text = "0";
                lb_Product_Total_18_19.Text = "0";
                lb_Product_Total_19_20.Text = "0";
                lb_Product_Total_20_21.Text = "0";
                lb_Product_Total_21_22.Text = "0";
                lb_Product_Total_22_23.Text = "0";
                lb_Product_Total_23_00.Text = "0";
                lb_Product_Total_00_01.Text = "0";
                lb_Product_Total_01_02.Text = "0";
                lb_Product_Total_02_03.Text = "0";
                lb_Product_Total_03_04.Text = "0";
                lb_Product_Total_04_05.Text = "0";
                lb_Product_Total_05_06.Text = "0";
                lb_Product_Total_06_07.Text = "0";
                lb_Product_Total_07_08.Text = "0";
                lb_Product_NG_08_09.Text = "0";
                lb_Product_NG_09_10.Text = "0";
                lb_Product_NG_10_11.Text = "0";
                lb_Product_NG_11_12.Text = "0";
                lb_Product_NG_12_13.Text = "0";
                lb_Product_NG_13_14.Text = "0";
                lb_Product_NG_14_15.Text = "0";
                lb_Product_NG_15_16.Text = "0";
                lb_Product_NG_16_17.Text = "0";
                lb_Product_NG_17_18.Text = "0";
                lb_Product_NG_18_19.Text = "0";
                lb_Product_NG_19_20.Text = "0";
                lb_Product_NG_20_21.Text = "0";
                lb_Product_NG_21_22.Text = "0";
                lb_Product_NG_22_23.Text = "0";
                lb_Product_NG_23_00.Text = "0";
                lb_Product_NG_00_01.Text = "0";
                lb_Product_NG_01_02.Text = "0";
                lb_Product_NG_02_03.Text = "0";
                lb_Product_NG_03_04.Text = "0";
                lb_Product_NG_04_05.Text = "0";
                lb_Product_NG_05_06.Text = "0";
                lb_Product_NG_06_07.Text = "0";
                lb_Product_NG_07_08.Text = "0";
                lb_Product_Lianglv_08_09.Text = "0.00%";
                lb_Product_Lianglv_09_10.Text = "0.00%";
                lb_Product_Lianglv_10_11.Text = "0.00%";
                lb_Product_Lianglv_11_12.Text = "0.00%";
                lb_Product_Lianglv_12_13.Text = "0.00%";
                lb_Product_Lianglv_13_14.Text = "0.00%";
                lb_Product_Lianglv_14_15.Text = "0.00%";
                lb_Product_Lianglv_15_16.Text = "0.00%";
                lb_Product_Lianglv_16_17.Text = "0.00%";
                lb_Product_Lianglv_17_18.Text = "0.00%";
                lb_Product_Lianglv_18_19.Text = "0.00%";
                lb_Product_Lianglv_19_20.Text = "0.00%";
                lb_Product_Lianglv_20_21.Text = "0.00%";
                lb_Product_Lianglv_21_22.Text = "0.00%";
                lb_Product_Lianglv_22_23.Text = "0.00%";
                lb_Product_Lianglv_23_00.Text = "0.00%";
                lb_Product_Lianglv_00_01.Text = "0.00%";
                lb_Product_Lianglv_01_02.Text = "0.00%";
                lb_Product_Lianglv_02_03.Text = "0.00%";
                lb_Product_Lianglv_03_04.Text = "0.00%";
                lb_Product_Lianglv_04_05.Text = "0.00%";
                lb_Product_Lianglv_05_06.Text = "0.00%";
                lb_Product_Lianglv_06_07.Text = "0.00%";
                lb_Product_Lianglv_07_08.Text = "0.00%";
                lb_Product_Total_08_20.Text = "0";
                lb_Product_NG_08_20.Text = "0";
                lb_Product_Lianglv_08_20.Text = "0.00%";
                lb_Product_Total_20_08.Text = "0";
                lb_Product_NG_20_08.Text = "0";
                lb_Product_Lianglv_20_08.Text = "0.00%";
            }
            if (d3.Rows.Count > 0)
            {
                double Welding_Error_D = 0;
                double TraceUpLoad_Error_D = 0;
                double PDCAUpLoad_Error_D = 0;
                double TracePVCheck_Error_D = 0;
                double Location1_NG_D = 0;
                double Location1_NG_N = 0;
                double Location2_NG_D = 0;
                double Location2_NG_N = 0;
                double Location3_NG_D = 0;
                double Location3_NG_N = 0;
                double Location4_NG_D = 0;
                double Location4_NG_N = 0;
                double Location5_NG_D = 0;
                double Location5_NG_N = 0;
                double ReadBarcode_NG_D = 0;
                double CCDCheck_Error_D = 0;
                double Smallmaterial_throwing_D = 0;
                double Welding_Error_N = 0;
                double TraceUpLoad_Error_N = 0;
                double PDCAUpLoad_Error_N = 0;
                double TracePVCheck_Error_N = 0;
                double ReadBarcode_NG_N = 0;
                double CCDCheck_Error_N = 0;
                double Smallmaterial_throwing_N = 0;
                UpDataDGV_D(0, 1, d3.Rows[0][2].ToString());
                UpDataDGV_N(0, 1, d3.Rows[0][3].ToString());
                UpDataDGV_D(1, 1, d3.Rows[0][4].ToString());
                UpDataDGV_N(1, 1, d3.Rows[0][5].ToString());
                UpDataDGV_D(4, 1, d3.Rows[0][6].ToString());
                UpDataDGV_N(4, 1, d3.Rows[0][7].ToString());

                UpDataDGV_D(12, 1, d3.Rows[0][8].ToString());
                UpDataDGV_N(12, 1, d3.Rows[0][9].ToString());
                UpDataDGV_D(13, 1, d3.Rows[0][10].ToString());
                UpDataDGV_N(13, 1, d3.Rows[0][11].ToString());
                UpDataDGV_D(14, 1, d3.Rows[0][12].ToString());
                UpDataDGV_N(14, 1, d3.Rows[0][13].ToString());

                UpDataDGV_D(5, 1, d3.Rows[0][14].ToString());
                UpDataDGV_N(5, 1, d3.Rows[0][15].ToString());
                UpDataDGV_D(6, 1, d3.Rows[0][16].ToString());
                UpDataDGV_N(6, 1, d3.Rows[0][17].ToString());
                UpDataDGV_D(7, 1, d3.Rows[0][18].ToString());
                UpDataDGV_N(7, 1, d3.Rows[0][19].ToString());
                UpDataDGV_D(8, 1, d3.Rows[0][20].ToString());
                UpDataDGV_N(8, 1, d3.Rows[0][21].ToString());
                UpDataDGV_D(9, 1, d3.Rows[0][22].ToString());
                UpDataDGV_N(9, 1, d3.Rows[0][23].ToString());

                UpDataDGV_D(15, 1, d3.Rows[0][24].ToString());
                UpDataDGV_N(15, 1, d3.Rows[0][25].ToString());
                UpDataDGV_D(16, 1, d3.Rows[0][26].ToString());
                UpDataDGV_N(16, 1, d3.Rows[0][27].ToString());
                UpDataDGV_D(17, 1, d3.Rows[0][28].ToString());
                UpDataDGV_N(17, 1, d3.Rows[0][29].ToString());

                try
                {

                
                if (d3.Rows[0][2].ToString() != "0")
                {
                    Welding_Error_D = (Convert.ToDouble(d3.Rows[0][6].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    TraceUpLoad_Error_D = (Convert.ToDouble(d3.Rows[0][8].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    PDCAUpLoad_Error_D = (Convert.ToDouble(d3.Rows[0][10].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    TracePVCheck_Error_D = (Convert.ToDouble(d3.Rows[0][12].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    Location1_NG_D= (Convert.ToDouble(d3.Rows[0][14].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    Location2_NG_D = (Convert.ToDouble(d3.Rows[0][16].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    Location3_NG_D = (Convert.ToDouble(d3.Rows[0][18].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    Location4_NG_D = (Convert.ToDouble(d3.Rows[0][20].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    Location5_NG_D = (Convert.ToDouble(d3.Rows[0][22].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                    ReadBarcode_NG_D = (Convert.ToDouble(d3.Rows[0][24].ToString()) / Convert.ToDouble(d3.Rows[0][2].ToString())) * 100;
                }
                if (d3.Rows[0][3].ToString() != "0")
                {
                    Welding_Error_N = (Convert.ToDouble(d3.Rows[0][7].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    TraceUpLoad_Error_N = (Convert.ToDouble(d3.Rows[0][9].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    PDCAUpLoad_Error_N = (Convert.ToDouble(d3.Rows[0][11].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    TracePVCheck_Error_N = (Convert.ToDouble(d3.Rows[0][13].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    Location1_NG_N = (Convert.ToDouble(d3.Rows[0][15].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    Location2_NG_N = (Convert.ToDouble(d3.Rows[0][17].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    Location3_NG_N = (Convert.ToDouble(d3.Rows[0][19].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    Location4_NG_N = (Convert.ToDouble(d3.Rows[0][21].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    Location5_NG_N = (Convert.ToDouble(d3.Rows[0][23].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                    ReadBarcode_NG_N = (Convert.ToDouble(d3.Rows[0][25].ToString()) / Convert.ToDouble(d3.Rows[0][3].ToString())) * 100;
                }
                if (d3.Rows[0][4].ToString() != "0")
                {
                    CCDCheck_Error_D = (Convert.ToDouble(d3.Rows[0][26].ToString()) / Convert.ToDouble(d3.Rows[0][4].ToString())) * 100;
                    Smallmaterial_throwing_D = (Convert.ToDouble(d3.Rows[0][28].ToString()) / Convert.ToDouble(d3.Rows[0][4].ToString())) * 100;
                }
                if (d3.Rows[0][5].ToString() != "0")
                {
                    CCDCheck_Error_N = (Convert.ToDouble(d3.Rows[0][27].ToString()) / Convert.ToDouble(d3.Rows[0][5].ToString())) * 100;
                    Smallmaterial_throwing_N = (Convert.ToDouble(d3.Rows[0][29].ToString()) / Convert.ToDouble(d3.Rows[0][5].ToString())) * 100;
                }
                UpDataDGV_D(4, 2, Welding_Error_D.ToString("0.00") + "%");
                UpDataDGV_N(4, 2, Welding_Error_N.ToString("0.00") + "%");
                UpDataDGV_D(12, 2, TraceUpLoad_Error_D.ToString("0.00") + "%");
                UpDataDGV_N(12, 2, TraceUpLoad_Error_N.ToString("0.00") + "%");
                UpDataDGV_D(13, 2, PDCAUpLoad_Error_D.ToString("0.00") + "%");
                UpDataDGV_N(13, 2, PDCAUpLoad_Error_N.ToString("0.00") + "%");
                UpDataDGV_D(14, 2, TracePVCheck_Error_D.ToString("0.00") + "%");
                UpDataDGV_N(14, 2, TracePVCheck_Error_N.ToString("0.00") + "%");
                UpDataDGV_D(5, 2, Location1_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(5, 2, Location1_NG_N.ToString("0.00") + "%");
                UpDataDGV_D(6, 2, Location2_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(6, 2, Location2_NG_N.ToString("0.00") + "%");
                UpDataDGV_D(7, 2, Location3_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(7, 2, Location3_NG_N.ToString("0.00") + "%");
                UpDataDGV_D(8, 2, Location4_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(8, 2, Location4_NG_N.ToString("0.00") + "%");
                UpDataDGV_D(9, 2, Location5_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(9, 2, Location5_NG_N.ToString("0.00") + "%");
                
                UpDataDGV_D(15, 2, ReadBarcode_NG_D.ToString("0.00") + "%");
                UpDataDGV_N(15, 2, ReadBarcode_NG_N.ToString("0.00") + "%");
                UpDataDGV_D(16, 2, CCDCheck_Error_D.ToString("0.00") + "%");
                UpDataDGV_N(16, 2, CCDCheck_Error_N.ToString("0.00") + "%");
                UpDataDGV_D(17, 2, Smallmaterial_throwing_D.ToString("0.00") + "%");
                UpDataDGV_N(17, 2, Smallmaterial_throwing_N.ToString("0.00") + "%");

                }
                catch (Exception EX)
                {

                   
                }

                ///20210819
                /// 
                UpDataDGV_D(0,1, lb_Product_Total_08_20.Text);
                UpDataDGV_N(0,1, lb_Product_Total_20_08.Text);
            }
            else
            {
                UpDataDGV_D(0, 1, "0");
                UpDataDGV_N(0, 1, "0");
                UpDataDGV_D(1, 1, "0");
                UpDataDGV_N(1, 1, "0");
                UpDataDGV_D(4, 1, "0");
                UpDataDGV_N(4, 1, "0");
                UpDataDGV_D(5, 1, "0");
                UpDataDGV_N(5, 1, "0");
                UpDataDGV_D(6, 1, "0");
                UpDataDGV_N(6, 1, "0");
                UpDataDGV_D(7, 1, "0");
                UpDataDGV_N(7, 1, "0");
                UpDataDGV_D(8, 1, "0");
                UpDataDGV_N(8, 1, "0");
                UpDataDGV_D(9, 1, "0");
                UpDataDGV_N(9, 1, "0");
                UpDataDGV_D(12, 1, "0");
                UpDataDGV_N(12, 1, "0");
                UpDataDGV_D(13, 1, "0");
                UpDataDGV_N(13, 1, "0");
                UpDataDGV_D(14, 1, "0");
                UpDataDGV_N(14, 1, "0");

                UpDataDGV_D(15, 1, "0");
                UpDataDGV_N(15, 1, "0");
                UpDataDGV_D(16, 1, "0");
                UpDataDGV_N(16, 1, "0");
                UpDataDGV_D(17, 1, "0");
                UpDataDGV_N(17, 1, "0");
                UpDataDGV_D(4, 2, "0.00%");
                UpDataDGV_N(4, 2, "0.00%");
                UpDataDGV_D(5, 2, "0.00%");
                UpDataDGV_N(5, 2, "0.00%");
                UpDataDGV_D(6, 2, "0.00%");
                UpDataDGV_N(6, 2, "0.00%");
                UpDataDGV_D(7, 2, "0.00%");
                UpDataDGV_N(7, 2, "0.00%");
                UpDataDGV_D(8, 2, "0.00%");
                UpDataDGV_N(8, 2, "0.00%");
                UpDataDGV_D(9, 2, "0.00%");
                UpDataDGV_N(9, 2, "0.00%");

                UpDataDGV_D(12, 2, "0.00%");
                UpDataDGV_N(12, 2, "0.00%");
                UpDataDGV_D(13, 2, "0.00%");
                UpDataDGV_N(13, 2, "0.00%");
                UpDataDGV_D(14, 2, "0.00%");
                UpDataDGV_N(14, 2, "0.00%");
                UpDataDGV_D(15, 2, "0.00%");
                UpDataDGV_N(15, 2, "0.00%");
                UpDataDGV_D(16, 2, "0.00%");
                UpDataDGV_N(16, 2, "0.00%");
                UpDataDGV_D(17, 2, "0.00%");
                UpDataDGV_N(17, 2, "0.00%");
            }
        }

        private void btn_SelectDTTable_Click(object sender, EventArgs e)
        {
            string SelectStr = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Table.Value)).ToString("yyyy-MM-dd"), "运行时间");
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            string SelectStr2 = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Table.Value)).ToString("yyyy-MM-dd"), "异常时间");
            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
            string SelectStr3 = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Table.Value)).ToString("yyyy-MM-dd"), "待料时间");
            DataTable d3 = SQL.ExecuteQuery(SelectStr3);
            double[] point_RUN = new double[24];
            double[] point_ERROR = new double[24];
            double[] point_PENDING = new double[24];
            if (d1.Rows.Count > 0 && d2.Rows.Count > 0 && d3.Rows.Count > 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    point_RUN[i] = Convert.ToDouble(d1.Rows[0][i + 3].ToString());
                    point_ERROR[i] = Convert.ToDouble(d2.Rows[0][i + 3].ToString());
                    point_PENDING[i] = Convert.ToDouble(d3.Rows[0][i + 3].ToString());
                }
                chart_DT.Series[0].Points.DataBindXY(x, point_RUN);
                chart_DT.Series[1].Points.DataBindXY(x, point_ERROR);
                chart_DT.Series[2].Points.DataBindXY(x, point_PENDING);
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    point_RUN[i] = 0;
                    point_ERROR[i] = 0;
                    point_PENDING[i] = 0;
                }
                chart_DT.Series[0].Points.DataBindXY(x, point_RUN);
                chart_DT.Series[1].Points.DataBindXY(x, point_ERROR);
                chart_DT.Series[2].Points.DataBindXY(x, point_PENDING);
            }
        }

        private void btn_SelectDTData_Click(object sender, EventArgs e)
        {
            string SelectStr = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Data.Value)).ToString("yyyy-MM-dd"), "运行时间");
            DataTable d1 = SQL.ExecuteQuery(SelectStr);
            string SelectStr2 = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Data.Value)).ToString("yyyy-MM-dd"), "异常时间");
            DataTable d2 = SQL.ExecuteQuery(SelectStr2);
            string SelectStr3 = string.Format("select * from HourDT where DateTime = '{0}' and Status='{1}'", (Convert.ToDateTime(dtp_DT_Data.Value)).ToString("yyyy-MM-dd"), "待料时间");
            DataTable d3 = SQL.ExecuteQuery(SelectStr3);
            if (d1.Rows.Count > 0 && d2.Rows.Count > 0 && d3.Rows.Count > 0)
            {
                lb_RunTime_08_09.Text = d1.Rows[0][3].ToString();
                lb_RunTime_09_10.Text = d1.Rows[0][4].ToString();
                lb_RunTime_10_11.Text = d1.Rows[0][5].ToString();
                lb_RunTime_11_12.Text = d1.Rows[0][6].ToString();
                lb_RunTime_12_13.Text = d1.Rows[0][7].ToString();
                lb_RunTime_13_14.Text = d1.Rows[0][8].ToString();
                lb_RunTime_14_15.Text = d1.Rows[0][9].ToString();
                lb_RunTime_15_16.Text = d1.Rows[0][10].ToString();
                lb_RunTime_16_17.Text = d1.Rows[0][11].ToString();
                lb_RunTime_17_18.Text = d1.Rows[0][12].ToString();
                lb_RunTime_18_19.Text = d1.Rows[0][13].ToString();
                lb_RunTime_19_20.Text = d1.Rows[0][14].ToString();
                lb_RunTime_20_21.Text = d1.Rows[0][15].ToString();
                lb_RunTime_21_22.Text = d1.Rows[0][16].ToString();
                lb_RunTime_22_23.Text = d1.Rows[0][17].ToString();
                lb_RunTime_23_00.Text = d1.Rows[0][18].ToString();
                lb_RunTime_00_01.Text = d1.Rows[0][19].ToString();
                lb_RunTime_01_02.Text = d1.Rows[0][20].ToString();
                lb_RunTime_02_03.Text = d1.Rows[0][21].ToString();
                lb_RunTime_03_04.Text = d1.Rows[0][22].ToString();
                lb_RunTime_04_05.Text = d1.Rows[0][23].ToString();
                lb_RunTime_05_06.Text = d1.Rows[0][24].ToString();
                lb_RunTime_06_07.Text = d1.Rows[0][25].ToString();
                lb_RunTime_07_08.Text = d1.Rows[0][26].ToString();
                lb_ErrorTime_08_09.Text = d2.Rows[0][3].ToString();
                lb_ErrorTime_09_10.Text = d2.Rows[0][4].ToString();
                lb_ErrorTime_10_11.Text = d2.Rows[0][5].ToString();
                lb_ErrorTime_11_12.Text = d2.Rows[0][6].ToString();
                lb_ErrorTime_12_13.Text = d2.Rows[0][7].ToString();
                lb_ErrorTime_13_14.Text = d2.Rows[0][8].ToString();
                lb_ErrorTime_14_15.Text = d2.Rows[0][9].ToString();
                lb_ErrorTime_15_16.Text = d2.Rows[0][10].ToString();
                lb_ErrorTime_16_17.Text = d2.Rows[0][11].ToString();
                lb_ErrorTime_17_18.Text = d2.Rows[0][12].ToString();
                lb_ErrorTime_18_19.Text = d2.Rows[0][13].ToString();
                lb_ErrorTime_19_20.Text = d2.Rows[0][14].ToString();
                lb_ErrorTime_20_21.Text = d2.Rows[0][15].ToString();
                lb_ErrorTime_21_22.Text = d2.Rows[0][16].ToString();
                lb_ErrorTime_22_23.Text = d2.Rows[0][17].ToString();
                lb_ErrorTime_23_00.Text = d2.Rows[0][18].ToString();
                lb_ErrorTime_00_01.Text = d2.Rows[0][19].ToString();
                lb_ErrorTime_01_02.Text = d2.Rows[0][20].ToString();
                lb_ErrorTime_02_03.Text = d2.Rows[0][21].ToString();
                lb_ErrorTime_03_04.Text = d2.Rows[0][22].ToString();
                lb_ErrorTime_04_05.Text = d2.Rows[0][23].ToString();
                lb_ErrorTime_05_06.Text = d2.Rows[0][24].ToString();
                lb_ErrorTime_06_07.Text = d2.Rows[0][25].ToString();
                lb_ErrorTime_07_08.Text = d2.Rows[0][26].ToString();
                lb_PendingTime_08_09.Text = d3.Rows[0][3].ToString();
                lb_PendingTime_09_10.Text = d3.Rows[0][4].ToString();
                lb_PendingTime_10_11.Text = d3.Rows[0][5].ToString();
                lb_PendingTime_11_12.Text = d3.Rows[0][6].ToString();
                lb_PendingTime_12_13.Text = d3.Rows[0][7].ToString();
                lb_PendingTime_13_14.Text = d3.Rows[0][8].ToString();
                lb_PendingTime_14_15.Text = d3.Rows[0][9].ToString();
                lb_PendingTime_15_16.Text = d3.Rows[0][10].ToString();
                lb_PendingTime_16_17.Text = d3.Rows[0][11].ToString();
                lb_PendingTime_17_18.Text = d3.Rows[0][12].ToString();
                lb_PendingTime_18_19.Text = d3.Rows[0][13].ToString();
                lb_PendingTime_19_20.Text = d3.Rows[0][14].ToString();
                lb_PendingTime_20_21.Text = d3.Rows[0][15].ToString();
                lb_PendingTime_21_22.Text = d3.Rows[0][16].ToString();
                lb_PendingTime_22_23.Text = d3.Rows[0][17].ToString();
                lb_PendingTime_23_00.Text = d3.Rows[0][18].ToString();
                lb_PendingTime_00_01.Text = d3.Rows[0][19].ToString();
                lb_PendingTime_01_02.Text = d3.Rows[0][20].ToString();
                lb_PendingTime_02_03.Text = d3.Rows[0][21].ToString();
                lb_PendingTime_03_04.Text = d3.Rows[0][22].ToString();
                lb_PendingTime_04_05.Text = d3.Rows[0][23].ToString();
                lb_PendingTime_05_06.Text = d3.Rows[0][24].ToString();
                lb_PendingTime_06_07.Text = d3.Rows[0][25].ToString();
                lb_PendingTime_07_08.Text = d3.Rows[0][26].ToString();
                double RunTime_day = 0;
                double ErrorTime_day = 0;
                double PendingTime_day = 0;
                double RunTime_night = 0;
                double ErrorTime_night = 0;
                double PendingTime_night = 0;
                for (int i = 0; i < 12; i++)
                {
                    RunTime_day += Convert.ToDouble(d1.Rows[0][i + 3].ToString());
                    ErrorTime_day += Convert.ToDouble(d2.Rows[0][i + 3].ToString());
                    PendingTime_day += Convert.ToDouble(d3.Rows[0][i + 3].ToString());
                    RunTime_night += Convert.ToDouble(d1.Rows[0][i + 15].ToString());
                    ErrorTime_night += Convert.ToDouble(d2.Rows[0][i + 15].ToString());
                    PendingTime_night += Convert.ToDouble(d3.Rows[0][i + 15].ToString());
                }
                lb_RunTime_08_20.Text = RunTime_day.ToString("0.00");
                lb_ErrorTime_08_20.Text = ErrorTime_day.ToString("0.00");
                lb_PendingTime_08_20.Text = PendingTime_day.ToString("0.00");
                lb_RunTime_20_08.Text = RunTime_night.ToString("0.00");
                lb_ErrorTime_20_08.Text = ErrorTime_night.ToString("0.00");
                lb_PendingTime_20_08.Text = PendingTime_night.ToString("0.00");
                
                ///20210814update
                DayTime.Text = ((RunTime_day / (RunTime_day + ErrorTime_day + PendingTime_day))*100).ToString("0.00")+"%";
                NightTime.Text=((RunTime_night/(RunTime_night+ErrorTime_night+PendingTime_night))*100).ToString("0.00")+"%";

            }
            else
            {
                lb_RunTime_08_09.Text = "0";
                lb_RunTime_09_10.Text = "0";
                lb_RunTime_10_11.Text = "0";
                lb_RunTime_11_12.Text = "0";
                lb_RunTime_12_13.Text = "0";
                lb_RunTime_13_14.Text = "0";
                lb_RunTime_14_15.Text = "0";
                lb_RunTime_15_16.Text = "0";
                lb_RunTime_16_17.Text = "0";
                lb_RunTime_17_18.Text = "0";
                lb_RunTime_18_19.Text = "0";
                lb_RunTime_19_20.Text = "0";
                lb_RunTime_20_21.Text = "0";
                lb_RunTime_21_22.Text = "0";
                lb_RunTime_22_23.Text = "0";
                lb_RunTime_23_00.Text = "0";
                lb_RunTime_00_01.Text = "0";
                lb_RunTime_01_02.Text = "0";
                lb_RunTime_02_03.Text = "0";
                lb_RunTime_03_04.Text = "0";
                lb_RunTime_04_05.Text = "0";
                lb_RunTime_05_06.Text = "0";
                lb_RunTime_06_07.Text = "0";
                lb_RunTime_07_08.Text = "0";
                lb_ErrorTime_08_09.Text = "0";
                lb_ErrorTime_09_10.Text = "0";
                lb_ErrorTime_10_11.Text = "0";
                lb_ErrorTime_11_12.Text = "0";
                lb_ErrorTime_12_13.Text = "0";
                lb_ErrorTime_13_14.Text = "0";
                lb_ErrorTime_14_15.Text = "0";
                lb_ErrorTime_15_16.Text = "0";
                lb_ErrorTime_16_17.Text = "0";
                lb_ErrorTime_17_18.Text = "0";
                lb_ErrorTime_18_19.Text = "0";
                lb_ErrorTime_19_20.Text = "0";
                lb_ErrorTime_20_21.Text = "0";
                lb_ErrorTime_21_22.Text = "0";
                lb_ErrorTime_22_23.Text = "0";
                lb_ErrorTime_23_00.Text = "0";
                lb_ErrorTime_00_01.Text = "0";
                lb_ErrorTime_01_02.Text = "0";
                lb_ErrorTime_02_03.Text = "0";
                lb_ErrorTime_03_04.Text = "0";
                lb_ErrorTime_04_05.Text = "0";
                lb_ErrorTime_05_06.Text = "0";
                lb_ErrorTime_06_07.Text = "0";
                lb_ErrorTime_07_08.Text = "0";
                lb_PendingTime_08_09.Text = "0";
                lb_PendingTime_09_10.Text = "0";
                lb_PendingTime_10_11.Text = "0";
                lb_PendingTime_11_12.Text = "0";
                lb_PendingTime_12_13.Text = "0";
                lb_PendingTime_13_14.Text = "0";
                lb_PendingTime_14_15.Text = "0";
                lb_PendingTime_15_16.Text = "0";
                lb_PendingTime_16_17.Text = "0";
                lb_PendingTime_17_18.Text = "0";
                lb_PendingTime_18_19.Text = "0";
                lb_PendingTime_19_20.Text = "0";
                lb_PendingTime_20_21.Text = "0";
                lb_PendingTime_21_22.Text = "0";
                lb_PendingTime_22_23.Text = "0";
                lb_PendingTime_23_00.Text = "0";
                lb_PendingTime_00_01.Text = "0";
                lb_PendingTime_01_02.Text = "0";
                lb_PendingTime_02_03.Text = "0";
                lb_PendingTime_03_04.Text = "0";
                lb_PendingTime_04_05.Text = "0";
                lb_PendingTime_05_06.Text = "0";
                lb_PendingTime_06_07.Text = "0";
                lb_PendingTime_07_08.Text = "0";
                lb_RunTime_08_20.Text = "0";
                lb_ErrorTime_08_20.Text = "0";
                lb_PendingTime_08_20.Text = "0";
                lb_RunTime_20_08.Text = "0";
                lb_ErrorTime_20_08.Text = "0";
                lb_PendingTime_20_08.Text = "0";

                ///20210814update
                /// 
                DayTime.Text = "0.00%";
                NightTime.Text = "0.00%";
            }
        }

        private void dtp_UPH_Data_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.SelectDateTime = Convert.ToDateTime(dtp_UPH_Data.Value);
            }
            catch
            { }
        }

        private void dtp_DT_Data_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Global.SelectDTTime = Convert.ToDateTime(dtp_DT_Data.Value);
            }
            catch
            { }
        }

        private void btn_SelectFixtureError_Click(object sender, EventArgs e)
        {
            string[] FixtureErrorNameTOP5 = new string[] { "", "", "", "", "" };
            double[] FixtureErrorNumberTOP5 = new double[] { 0, 0, 0, 0, 0 };
            string Select = string.Format("SELECT TOP 5 count(Fixture) as number,Fixture FROM FixtureNG  where cast(DateTime as datetime) >='{0}' and cast(DateTime as datetime) <='{1}' ", Convert.ToDateTime(dtp_FixtureErrorTOP5Time.Text).ToString("yyyy/MM/dd") + " 06:00:00", Convert.ToDateTime(dtp_FixtureErrorTOP5Time.Text).AddDays(1).ToString("yyyy/MM/dd") + " 06:00:00")
                + "group by Fixture order by COUNT(Fixture) DESC";
            DataTable dt = SQL.ExecuteQuery(Select);//1、查找选择当天6：00-隔天6：00所有数据
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FixtureErrorNameTOP5[i] = dt.Rows[i][1].ToString();
                    FixtureErrorNumberTOP5[i] = Convert.ToDouble((Convert.ToDouble(dt.Rows[i][0].ToString()).ToString("0.00")));
                }
            }
            chart_FixtureErrorNumber.Series[0].Points.DataBindXY(FixtureErrorNameTOP5, FixtureErrorNumberTOP5);
            UpDatalabel(FixtureErrorNameTOP5[0], "lb_FixtureCode1");
            UpDatalabel(FixtureErrorNameTOP5[1], "lb_FixtureCode2");
            UpDatalabel(FixtureErrorNameTOP5[2], "lb_FixtureCode3");
            UpDatalabel(FixtureErrorNameTOP5[3], "lb_FixtureCode4");
            UpDatalabel(FixtureErrorNameTOP5[4], "lb_FixtureCode5");
            UpDatalabel(FixtureErrorNumberTOP5[0].ToString(), "lb_Number1");
            UpDatalabel(FixtureErrorNumberTOP5[1].ToString(), "lb_Number2");
            UpDatalabel(FixtureErrorNumberTOP5[2].ToString(), "lb_Number3");
            UpDatalabel(FixtureErrorNumberTOP5[3].ToString(), "lb_Number4");
            UpDatalabel(FixtureErrorNumberTOP5[4].ToString(), "lb_Number5");
        }

        /// <summary>
        /// 初始化浏览器
        /// </summary>
        private void initWebBrowser()
        {
            //防止 WebBrowser 控件打开拖放到其上的文件。
            webBrowser1.AllowWebBrowserDrop = false;

            //防止 WebBrowser 控件在用户右击它时显示其快捷菜单.
            webBrowser1.IsWebBrowserContextMenuEnabled = false;

            //以防止 WebBrowser 控件响应快捷键。
            webBrowser1.WebBrowserShortcutsEnabled = false;

            //以防止 WebBrowser 控件显示脚本代码问题的错误信息。    
            webBrowser1.ScriptErrorsSuppressed = true;

            //（这个属性比较重要，可以通过这个属性，把WINFROM中的变量，传递到JS中，供内嵌的网页使用；但设置到的类型必须是COM可见的，所以要设置     [System.Runtime.InteropServices.ComVisibleAttribute(true)]，因为我的值设置为this,所以这个特性要加载窗体类上）
            webBrowser1.ObjectForScripting = this;
        }

        public void CreateHtml(string path)
        {
            try
            {
                string fileName = path + ".html";
                if (File.Exists(fileName))
                    File.Delete(fileName);
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    sw.WriteLine("<html lang=\"en\" > ");
                    sw.WriteLine("<head>");
                    sw.WriteLine("<meta charset=\"UTF-8\" >");
                    sw.WriteLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> ");
                    sw.WriteLine("<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\" > ");
                    sw.WriteLine("<script src=\"echarts.js\"></script>");
                    sw.WriteLine("<title>project</title>");
                    sw.WriteLine("</head>");
                    sw.WriteLine("<body>");
                    sw.WriteLine("<div id=\"main\" style=\"width: 100 %; height: 400px; \"></div>");
                    sw.WriteLine("<script>");
                    sw.WriteLine("var opt = {");
                    sw.WriteLine("title: {");
                    //sw.WriteLine("text: '花费时间甘特图'");
                    sw.WriteLine("},");
                    sw.WriteLine("legend: {");
                    sw.WriteLine("y: 'bottom',");
                    sw.WriteLine("data: ['花费时间']");
                    sw.WriteLine("},");
                    sw.WriteLine("xAxis: {");
                    sw.WriteLine("type: 'time',");
                    sw.WriteLine("position: \"top\"");
                    sw.WriteLine("},");
                    sw.WriteLine("yAxis: {");
                    sw.WriteLine("type: \"category\",");
                    sw.WriteLine("data: ['Setup', 'IQ', 'OQ', 'PQ', 'MQ', 'Production']");
                    sw.WriteLine("},");
                    sw.WriteLine("series: [");
                    sw.WriteLine("{");
                    sw.WriteLine("name: '时间',");
                    sw.WriteLine("type: 'bar',");
                    sw.WriteLine("stack: '总量',");
                    sw.WriteLine("itemStyle: {");
                    sw.WriteLine("normal: {");
                    sw.WriteLine("color: 'rgba(0,0,0,0)'");
                    sw.WriteLine("}");
                    sw.WriteLine("},");
                    sw.WriteLine("data: [");
                    if (Global.Setuplist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.Setuplist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.IQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.IQlist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.OQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.OQlist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.PQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.PQlist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.MQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.MQlist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.Productionlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.Productionlist[0]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    sw.WriteLine("]");
                    sw.WriteLine("},");
                    sw.WriteLine("{");
                    sw.WriteLine("name: '花费时间',");
                    sw.WriteLine("type: 'bar',");
                    sw.WriteLine("stack: '总量',");
                    sw.WriteLine("itemStyle: {");
                    sw.WriteLine("normal: {");
                    sw.WriteLine("color: 'OrangeRed'");
                    sw.WriteLine("}");
                    sw.WriteLine("},");
                    sw.WriteLine("data: [");
                    if (Global.Setuplist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.Setuplist[Global.Setuplist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.IQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.IQlist[Global.IQlist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.OQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.OQlist[Global.OQlist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.PQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.PQlist[Global.PQlist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.MQlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.MQlist[Global.MQlist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    if (Global.Productionlist.Count > 0)
                    {
                        sw.WriteLine(string.Format("new Date(\"{0}\"),", Global.Productionlist[Global.Productionlist.Count - 1]));
                    }
                    else
                    {
                        sw.WriteLine("new Date(\"\"),");
                    }
                    sw.WriteLine("]");
                    sw.WriteLine("}");
                    sw.WriteLine("]");
                    sw.WriteLine("};");
                    sw.WriteLine("echarts.init(document.getElementById('main')).setOption(opt);");
                    sw.WriteLine("</script>");
                    sw.WriteLine("</body>");
                    sw.WriteLine("</html>");
                    sw.Close();
                }
                webBrowser1.Url = new Uri(fileName);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
            }
        }
    }
}
