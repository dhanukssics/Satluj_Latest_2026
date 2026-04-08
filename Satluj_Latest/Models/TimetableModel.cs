
using Satluj_Latest.Data;
using Satluj_Latest.Models.Temp;

namespace Satluj_Latest.Models
{
    public class TimetableModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisonId { get; set; }
        public long TeacherId { get; set; }
        public long SubjectId { get; set; }
        public Days DayId { get; set; }
        public Periods Period { get; set; }
        public int PeriodId { get; set; }
        public long TableId { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
        public string SubjectName { get; set; }
        public long SeasonId { get; set; }
        public List<TbSeason> Seasons { get; set; }
        public List<TimetableListingModel> TimetableList { get; internal set; }
        public long Id { get; set; }
    }
}