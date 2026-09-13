using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingManager.ViewModels.Accounting
{
    public class ReportViewModel
    {
        public int Receive { get; set; }
        public int Pay { get; set; }
        public int AccountBalance { get; set; }
    }
}
