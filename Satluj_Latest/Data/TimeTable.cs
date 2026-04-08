using Satluj_Latest.Models;
using System;
using System.Linq;

namespace Satluj_Latest.Data
{
    public class TimeTable : BaseReference
    {
        private TbTimeTable table;

        public TimeTable(TbTimeTable obj)
        {
            table = obj;
        }

        public TimeTable(long id)
        {
            table = _Entities.TbTimeTables.FirstOrDefault(z => z.Id == id);
        }

        public long Id { get { return table?.Id ?? 0; } }
        public long SchoolId { get { return table?.SchoolId ?? 0; } }
        public long ClassId { get { return table?.ClassId ?? 0; } }
        public long DivisionId { get { return table?.DivisionId ?? 0; } }
        public long SeasonId { get { return table?.SeasonId ?? 0; } }
        public long TeacherId { get { return table?.TeacherId ?? 0; } }
        public long SubjectId { get { return table?.SubjectId ?? 0; } }
        public bool IsActive { get { return table?.IsActive ?? false; } }
        public int DayId { get { return table?.DayId ?? 0; } }
        public int Periods { get { return table?.Periods ?? 0; } }

        public string ClassName { get { return table?.Class?.Class ?? ""; } }
        public string DivisionName { get { return table?.Division?.Division ?? ""; } }
        public string Teacher { get { return table?.Teacher?.TeacherName ?? ""; } }
        public string Subject { get { return table?.Subject?.SubjectName ?? ""; } }
        public string SubjectAbbreviation { get { return table?.Subject?.Abbreviation ?? ""; } }

        public string DayName { get { return Day(); } }

        public TbTimeTable X { get; }

        public string Day()
        {
            if (table == null)
                return "";

            string Day = "";

            if (table.DayId == 0)
                Day = "Monday";
            else if (table.DayId == 1)
                Day = "Tuesday";
            else if (table.DayId == 2)
                Day = "Wednesday";
            else if (table.DayId == 3)
                Day = "Thursday";
            else if (table.DayId == 4)
                Day = "Friday";
            else if (table.DayId == 5)
                Day = "Saturday";

            return Day;
        }
    }
}