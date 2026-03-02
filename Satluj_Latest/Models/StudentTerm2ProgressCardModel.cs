using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentTerm2ProgressCardModel
    {
        public bool IsOneTerm { get; set; }
        public long StudentId { get; set; }
        public long ExamId { get; set; }
        public string SchoolName { get; set; }
        public string AccademicSession { get; set; }
        public string ReportName { get; set; }
        public string RollNo { get; set; }
        public string StudentName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string DateOfBirth { get; set; }
        public string ClassDivision { get; set; }
        public string RegionName { get; set; }
        public string SchoolAdddress { get; set; }
        public long RegionId { get; set; }
        public List<TermList> TermDataList { get; set; }
        public bool IsIssued { get; set; }
        public string Today { get; set; }
        public string Remark { get; set; }
        public string SchoolLogo { get; set; }
        public string SchoolEmail { get; set; }
        public string SchoolWebsite { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentImage { get; set; }
        public string Periods { get; set; }
        public List<ComputerScienceResultModel> csList { get; set; }
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public string Remark2 { get; set; }
        public long ExamId2 { get; set; }

    }
    public class TermList
    {
        public string TermName { get; set; }
        public List<ScolasticAreaList> scholasticList { get; set; }
        public List<CoscholasticAreaList> ColasticAreaResult { get; set; }
    }
    public class ScolasticAreaList
    {
        public string ScolasticArea { get; set; }
        public string TotalScore { get; set; }
        public List<SubjectDetails> subjectList { get; set; }
    }
    public class SubjectDetails
    {
        public string SubjectName { get; set; }
        public string Mark { get; set; }
        public string PracticalScore { get; set; }
        public string MarkTotal { get; set; }
        public string PracticalScoreTotal { get; set; }
    }
    public class CoscholasticAreaList
    {
        public string CoScholasticData { get; set; }
        public string CoScholasticResult { get; set; }
    }
    public class ComputerScienceResultModel
    {
        public string SubjectName { get; set; }
        public string TheoryScore { get; set; }
        public string PracticalScore { get; set; }
        public string TheoryTotal { get; set; }
        public string PracticalTotal { get; set; }
    }
}