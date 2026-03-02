using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPUnassignedTeachers:BaseReference
    {
        private SP_UnassignedTeachers_Result freeClassList;
        public SPUnassignedTeachers()
            {

            }
        public SPUnassignedTeachers(SP_UnassignedTeachers_Result obj) { freeClassList = obj; }
        public string ClassName { get { return freeClassList.ClassName; } }
        public long ClassId { get { return freeClassList.ClassId; } }
    }
}
