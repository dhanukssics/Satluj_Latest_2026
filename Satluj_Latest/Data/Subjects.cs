using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public class Subjects:BaseReference
    {
        private TbSubject sub;
        public Subjects(TbSubject obj) { sub = obj; }
        public Subjects(long id) { sub = _Entities.TbSubjects.FirstOrDefault(z => z.SubId == id); }

        

        public long SubId { get { return sub.SubId; } }
        public long SchoolI { get { return sub.SchoolI; } }
        public string SubjectName { get { return sub.SubjectName; } }
        public bool IsActive { get { return sub.IsActive; } }
        public System.DateTime TmeStamp { get { return sub.TmeStamp; } }
        public bool IsOptonal { get { return sub.IsOptonal; } }
        public string Abbreviation { get { return sub.Abbreviation; } }
        public string Code { get { return sub.Code; } }
        public Nullable<int> EnumTypeId { get { return sub.EnumTypeId; } }

        public TbSubject Z { get; }
    }
}
