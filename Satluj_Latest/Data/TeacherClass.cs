using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class TeacherClass :BaseReference
    {
          private TbTeacherClass  teacherClass;
          public TeacherClass(TbTeacherClass obj) { teacherClass = obj; }
          public TeacherClass(long id) { teacherClass = _Entities.TbTeacherClasses.FirstOrDefault(z => z.TeacherClassId == id); }
          public long TeacherClassId { get { return teacherClass.TeacherClassId; } }
          public long TeacherId { get { return teacherClass.TeacherId; } }
          public long ClassId { get { return teacherClass.ClassId; } }
          public long DivisionId { get { return teacherClass.DivisionId; } }
          public string ClassName { get { return teacherClass.Class.Class; } }
          public string DivisionName { get { return teacherClass.Division.Division; } }

    }
}
