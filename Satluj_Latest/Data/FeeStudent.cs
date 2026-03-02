using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public class FeeStudent:BaseReference
    {
          private TbFeeStudent feeStudent;
        public FeeStudent(TbFeeStudent obj) { feeStudent = obj; }
        public FeeStudent(long id) { feeStudent = _Entities.TbFeeStudents.FirstOrDefault(z => z.FeeStudentId == id); }
        public long feeStudentId { get { return feeStudent.FeeStudentId; } }
        public decimal amount { get { return feeStudent.Amount; } }
        public long studentId { get { return feeStudent.StudentId; } }
        public long feeId { get { return feeStudent.FeeId; } }
        public string feename { get { return feeStudent.Fee.FeesName ?? string.Empty; } }
        public System.DateTime Timestamp { get { return feeStudent.TimeStamp; } }
        public System.DateTime DueDate { get { return feeStudent.DueDate; } }
        public int instalment { get { return feeStudent.Instalment; } }
        public bool isActive { get { return feeStudent.IsActive; } }
        public Fee feeDetail { get { return new Fee(feeStudent.Fee); } }
        public Student student { get { return new Student(feeStudent.StudentId); } }

       // public Student student { get { return new Student(feeStudent.StudentId); } }

    }
}
