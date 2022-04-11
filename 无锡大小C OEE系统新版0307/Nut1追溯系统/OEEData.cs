using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 卓汇数据追溯系统
{
    public class OEEData
    {
        public string OEE_Dsn;
        public string OEE_authCode;
        public string SerialNumber;
        public string Fixture;
        public DateTime StartTime;
        public DateTime EndTime;
        public string ActualCT;
        public string Status;
        public string SwVersion;
        public string ScanCount;
        //public string DefectCode;
        public int auto_send = 0;
        public int station = 0;
    }
}
