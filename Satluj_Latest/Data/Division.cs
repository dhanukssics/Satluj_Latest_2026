using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Division : BaseReference
    {
        private TbDivision division;
        public Division(TbDivision obj) { division = obj; }
        public Division(long id) { division = _Entities.TbDivisions.FirstOrDefault(z => z.DivisionId == id); }
        public long DivisionId { get { return division.DivisionId; } }
        public long ClassId { get { return division.ClassId; } }
        public string ClassName { get { return division.Class.Class; } }
        public string DivisionName { get { return division.Division; } }
        public System.DateTime TimeStamp { get { return division.TimeStamp; } }
        public System.Guid DivisionGuid { get { return division.DivisionGuid; } }
        public bool IsActive { get { return division.IsActive; } }
        public Class Class { get { return new Class(division.Class); } }

        public List<Student> GetStudentDetails()
        {


            var asd = division.TbStudents.Where(z => z.IsActive).ToList().Select(q => new Student(q)).OrderBy(x => x.StundentName).ToList();

            return division.TbStudents.Where(z => z.IsActive).ToList().Select(q => new Student(q)).OrderBy(x=>x.StundentName).ToList();
        }
        public int GetStudentCount()
        {
            return division.TbStudents.Where(z => z.IsActive).Count();
        }
        public List<TbFeeDiscount> GetStudentDiscountList()
        {
            return _Entities.TbFeeDiscounts
                    .Where(z => z.IsActive && z.Student.DivisionId == division.DivisionId)
                    .Include(z => z.Student)
                        .ThenInclude(s => s.Class)
                    .Include(z => z.Student)
                        .ThenInclude(s => s.Division)
                    .Include(z => z.Fee)
                    .OrderBy(z => z.Student.StundentName)
                    .ToList();
        }
        public List<FeeStudent> GetSpecialFeeStudentList(long feeId)
        {
            return division.TbStudents.SelectMany(z => z.TbFeeStudents).Where(z => z.IsActive && z.FeeId == feeId).ToList().Select(q => new FeeStudent(q)).ToList();
        }
        public string getTeacherClass()
        {
            var data = division.TbTeacherClasses.ToList().Select(z => new Teacher(z.Teacher)).FirstOrDefault();
            if (data != null)
                return data.TeacherName;
            else
                return string.Empty;
        }

        public List<TbAttendance> GetAttendance(DateTime maxDate, DateTime minDate, int shift)
        {
            return division.TbAttendances.Where(z =>z.AttendanceDate >= minDate && z.AttendanceDate <= maxDate && z.ShiftStatus == shift).ToList().Select(z => new TbAttendance(z)).ToList();
        }
        public List<TbAttendance> GetAttendanceCount(int shift)
        {
            string Maxdate = CurrentTime.Date.ToString("MM-dd-yyyy") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(Maxdate);
            DateTime minDate= Convert.ToDateTime(CurrentTime.Date);
            return division.TbAttendances.Where(z => z.AttendanceDate >= minDate && z.AttendanceDate <= maxDate && z.ShiftStatus == shift).ToList().Select(z => new TbAttendance(z)).ToList();
        }
    }
}
