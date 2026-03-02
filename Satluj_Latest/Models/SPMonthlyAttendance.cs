using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
   public class SPMonthlyAttendance:BaseReference
    {
        public SPMonthlyAttendance()
        {
            
        }
        private sp_MonthlyAttendance_Result month;
        public SPMonthlyAttendance(sp_MonthlyAttendance_Result obj) { month = obj; }


        public long ClassId { get { return month.ClassId; } }
        public long DivisionId { get { return month.DivisionId; } }
        public string StundentName { get { return month.StundentName; } }
        public int? C1_ { get { return month.C1; } }
        public int? C2_ { get { return month.C2; } }
        public int? C3_ { get { return month.C3; } }
        public int? C4_ { get { return month.C4; } }
        public int? C5_ { get { return month.C5; } }
        public int? C6_ { get { return month.C6; } }
        public int? C7_ { get { return month.C7; } }
        public int? C8_ { get { return month.C8; } }
        public int? C9_ { get { return month.C9; } }
        public int? C10 { get { return month.C10; } }
        public int? C11 { get { return month.C11; } }
        public int? C12 { get { return month.C12; } }
        public int? C13 { get { return month.C13; } }
        public int? C14 { get { return month.C14; } }
        public int? C15 { get { return month.C15; } }
        public int? C16 { get { return month.C16; } }
        public int? C17 { get { return month.C17; } }
        public int? C18 { get { return month.C18; } }
        public int? C19 { get { return month.C19; } }
        public int? C20 { get { return month.C20; } }
        public int? C21 { get { return month.C21; } }
        public int? C22 { get { return month.C22; } }
        public int? C23 { get { return month.C23; } }
        public int? C24 { get { return month.C24; } }
        public int? C25 { get { return month.C25; } }
        public int? C26 { get { return month.C26; } }
        public int? C27 { get { return month.C27; } }
        public int? C28 { get { return month.C28; } }
        public int? C29 { get { return month.C29; } }
        public int? C30 { get { return month.C30; } }
        public int? C31 { get { return month.C31; } }

        public sp_MonthlyAttendance_Result X { get; }
    }
}
