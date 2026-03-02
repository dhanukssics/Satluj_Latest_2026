
using Satluj_Latest.Models;

namespace Satluj_Latest.Data
{
    public class V_Assesment : BaseReference
    {
        private TbVAssesment ass;
        public V_Assesment(TbVAssesment obj) { ass = obj; }
        public V_Assesment(long Id) { ass = _Entities.TbVAssesments.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return ass.Id; } }
        public int AssesmentId { get { return ass.AssesmentId; } }
        public long PeriodId { get { return ass.PeriodId; } }
        public long SchoolId { get { return ass.SchoolId; } }
        public bool IsActive { get { return ass.IsActive; } }
        public System.DateTime TimeStamp { get { return ass.TimeStamp; } }
        public string Assesment { get { return Convert.ToString((Assesments)ass.AssesmentId); } }
        public string AssesmentName()
        {
            string AssesmentName = "";
            if (ass.AssesmentId == 0)
                AssesmentName = "Assesment I";
            else if (ass.AssesmentId == 1)
                AssesmentName = "Assesment II";
            else
                AssesmentName = "Assesment III";
            return AssesmentName;
        }
        public string PeriodicName { get { return ass.Period.PeriodsName; } }
    }
}
