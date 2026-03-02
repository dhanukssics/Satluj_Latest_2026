using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AssessmentModel
    {

        public DateTime maxDate { get; set; }
        public DateTime minDate { get; set; }
        public int classId { get; set; }//long
        //public long studentId { get; set; }
        public int divisionId { get; set; }//long
        public int shift { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string StudentName { get; set; }
        public bool IsAdminUser { get; set; }
        public int studentId { get; set; }//newly added on 9/20/2020

    }
}