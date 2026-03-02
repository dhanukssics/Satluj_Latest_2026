using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPUnassignedDivisions:BaseReference
    {
        public SPUnassignedDivisions()
        {
            
        }
        private SP_UnassignedDivisions_Result freeDivisionList;
       
        public SPUnassignedDivisions(SP_UnassignedDivisions_Result obj) { freeDivisionList = obj; }
         public string Division { get { return freeDivisionList.Division; } }
         public long DivisionId { get { return freeDivisionList.DivisionId; } }
    }
}
