using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class TeacherClassSubject : BaseReference
    {
        private TbTeacherClassSubject tcs;
        public TeacherClassSubject(TbTeacherClassSubject obj) { tcs = obj; }
        public TeacherClassSubject(long id) { tcs = _Entities.TbTeacherClassSubjects.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return tcs.Id; } }
        public long SchoolId { get { return tcs.SchoolId; } }
        public long TeacherId { get { return tcs.TeacherId; } }
        public long ClassId { get { return tcs.ClassId; } }
        public long DivisionId { get { return tcs.DivisionId; } }
        public long SubjectId { get { return tcs.SubjectId; } }
        public bool IsActive { get { return tcs.IsActive; } }
        public System.DateTime TimeStamp { get { return tcs.TimeStamp; } }
        public string ClassName { get { return tcs.Class.Class; } }
        public string DivisionName { get { return tcs.Division.Division; } }
        public string Subject { get{ return tcs.Subject.SubjectName; } }
    }
}
