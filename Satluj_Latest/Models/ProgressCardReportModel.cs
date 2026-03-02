using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ProgressCardReportModel
    {
        public string SchoolName { get; set; }
        public string SchoolLogo { get; set; }
        public string SchoolAddress { get; set; }
        public string ClassName { get; set; }
        public string AccademicSession { get; set; }
        public string StudentName { get; set; }
        public string StudentAddress { get; set; }
        public string Parent { get; set; }
        public string DateOfBirth { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassNumber { get; set; }
        public string ExamOne { get; set; }
        public string ExamTwo { get; set; }
        public List<StudentProgressCardMarks> Marks { get; set; }
        public string Overall { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; }
        public string Rank { get; set; }
        public string Status { get; set; }
        public string Attendance { get; set; }
        public string CurrentDate { get; set; }
    }
    public class StudentProgressCardMarks
    {
        public string Subject { get; set; }
        public decimal InternalOne { get; set; }
        public decimal ExternalOne { get; set; }
        public decimal TotalOne { get; set; }
        public decimal InternalTwo { get; set; }
        public decimal ExternalTwo { get; set; }
        public decimal TotalTwo { get; set; }
        public decimal GrandTotal { get; set; }
        public string Grade { get; set; }
        public string Rank { get; set; }


        public decimal InternalTotalOne { get; set; }
        public decimal ExternalTotalOne { get; set; }
        public decimal GrandTotalOne { get; set; }

        public decimal InternalTotalTwo { get; set; }
        public decimal ExternalTotalTwo { get; set; }
        public decimal GrandTotalTwo { get; set; }

        public decimal GrandGrandTotal { get; set; }

    }
}