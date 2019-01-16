using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPerformanceDataExtraction.Data
{
    public class GLHistory
    {
        public string FinPeriodID { get; set; }
        public int LedgerID { get; set; }
        public int BranchID { get; set; }
        public int AccountID { get; set; }
        public int SubaccountID { get; set; }
        public int FinYear { get; set; }
        public decimal CuryFinBegBalance { get; set; }
        public decimal CuryFinPtdCredit { get; set; }
        public decimal CuryFinPtdDebit { get; set; }
        public decimal CuryFinYtdBalance { get; set; }
        public string CuryID { get; set; }
        public decimal FinBegBalance { get; set; }
        public decimal FinPtdCredit { get; set; }
        public decimal FinPtdDebit { get; set; }
        public decimal FinYtdBalance { get; set; }
        public string Timestamp { get; set; }
    }
}
