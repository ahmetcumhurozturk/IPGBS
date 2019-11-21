using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPGBS
{
    class SensitiveTransactionTable
    {
        int sId, spCount;
        string sp;
        public int SId
        {
            get { return this.sId; }
            set { this.sId = value; }
        }
        public string Sp
        {
            get { return this.sp; }
            set { this.sp = value; }
        }
        public int SpCount
        {
            get { return this.spCount; }
            set { this.spCount = value; }
        }

        public SensitiveTransactionTable(int _sid, string _sp, int _spCount)
        {
            sId = _sid;
            sp = _sp;
            spCount = _spCount;
        }
    }
}
