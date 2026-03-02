using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class StudentRemark : BaseReference
    {
        private TbStudentRemark stdRemark;
        public StudentRemark(TbStudentRemark obj) { stdRemark = obj; }
        public StudentRemark(long id) { stdRemark = _Entities.TbStudentRemarks.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return stdRemark.Id; } }
        public long StudentId { get { return stdRemark.StudentId; } }
        public long ExamId { get { return stdRemark.ExamId; } }
        public string Remark { get { return stdRemark.Remark; } }
        public bool IsActive { get { return stdRemark.IsActive; } }
        public System.DateTime TimeStamp { get { return stdRemark.TimeStamp; } }

    }
}
