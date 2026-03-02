using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SP_StudentHistory
    {
        public SP_StudentHistory()
        {
            
        }
        private SP_StudentHistory_Result studentHistory;
        public SP_StudentHistory(SP_StudentHistory_Result obj) { studentHistory = obj; }
        public long FeeId { get { return studentHistory.FeeId; } }
        public decimal Amount { get { return studentHistory.Amount; } }
        public Guid FeeGuid { get { return studentHistory.FeeGuid; } }
        public DateTime DueDate  { get { return studentHistory.DueDate; } }
        public string Feename { get { return studentHistory.Feename; } }
    }
}
