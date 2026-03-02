using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class TimeTable : BaseReference
    {
        private TbTimeTable table;
        public TimeTable(TbTimeTable obj) { table = obj; }
        public TimeTable(long id) { table = _Entities.TbTimeTables.FirstOrDefault(z => z.Id == id); }

        

        public long Id { get { return table.Id; } }
        public long SchoolId { get { return table.SchoolId; } }
        public long ClassId { get { return table.ClassId; } }
        public long DivisionId { get { return table.DivisionId; } }
        public long TeacherId { get { return table.TeacherId; } }
        public long SubjectId { get { return table.SubjectId; } }
        public bool IsActive { get { return table.IsActive; } }
        public int DayId { get { return table.DayId; } }
        public int Periods { get { return table.Periods; } }
        public string ClassName { get { return table.Class.Class; } }
        public string DivisionName { get { return table.Division.Division; } }
        public string Teacher { get { return table.Teacher.TeacherName; } }
        public string Subject { get { return table.Subject.SubjectName; } }
        public string SubjectAbbreviation { get { return table.Subject.Abbreviation; } }
        public string DayName { get { return Day(); } }

        public TbTimeTable X { get; }

        public string Day()
        {
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
            if (table.DayId == 5)
                Day = "Saturday";

            return Day;
        }
    }
}
