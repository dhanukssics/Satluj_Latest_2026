using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class Common
    {
    }
    public class SchoolValue
    {
        public long schoolId { get; set; }
        public string className { get; set; }
        public long classId { get; set; }
        public long divId { get; set; }
        public string DivisionName { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public IEnumerable<SelectListItem> UnPublishedClasses
        {
            get;
            set;

        }
        public class FilterModel
        {
            public long schoolId { get; set; }
            public string ClassName { get; set; }
            public string DivName { get; set; }
            public long FeeId { get; set; }

        }
        public class SchoolData
        {
            public SchoolValue value { get; set; }
            public Satluj_Latest.Data.School Data { get; set; }
        }
        public class AddFee
        {
            public long SchoolId { get; set; }
            [Required(ErrorMessage = "Required")]
            public string FeeName { get; set; }
            public int FeeType { get; set; }
            public bool CheckInterval { get; set; }
            public int Interval { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime EndDate { get; set; }
            public string DueDateString { get; set; }
            public string EndDateString { get; set; }
            public int IsReccuring { get; set; }
            public int IsDueDate { get; set; }
            public DateTime HaveFineDate { get; set; }//New
            public string HaveFineDateString { get; set; }//New
            [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
            public decimal FineAmount { get; set; }//New
            [RegularExpression("^[0-9]*$", ErrorMessage = "FineDays must be numeric")]
            public int FineDays { get; set; }//New
        }
        public class AddClassFees
        {
            public long SchoolId { get; set; }


            //[Required(ErrorMessage = "Required")]
            public string FeeId { get; set; }
            public string DataList { get; set; }
            public int Interval { get; set; }
            public DateTime DueDate { get; set; }
        }
        public class ViewFeeClass
        {
            public long SchoolId { get; set; }
            public long FeeId { get; set; }
        }
        public class Datalist
        {
            public long classId { get; set; }
            public long feeStudentId { get; set; }

            public string amount { get; set; }
        }
        public class FeeDetails
        {
            public long feeStudentId { get; set; }
            public Decimal amount { get; set; }
        }
        public class Datavalue
        {
            public List<Datalist> list { get; set; }
        }
        public class ListFee
        {
            public long schoolId { get; set; }
        }
        public class EditFee
        {
            //public long schoolId { get; set; }
            public long feeId { get; set; }
            [Required(ErrorMessage = "Required")]
            public string feename { get; set; }
            public int? feeType { get; set; }
        }
        public class FeeclassList
        {
            public long schoolId { get; set; }
            public long feeId { get; set; }
        }
        public class FeeBilling
        {
            public long feeId { get; set; }
            public decimal amount { get; set; }
            public string feeName { get; set; }
            public long studentFeeId { get; set; }


            public DateTime timeStamp { get; set; }
        }
        public class EditStudent
        {
            public string BusId { get; set; }
            public long schoolId { get; set; }

            [Required(ErrorMessage = "Class Required")]
            public long classId { get; set; }
            public long studentId { get; set; }
            [Required(ErrorMessage = "Division Required")]
            public long divisionId { get; set; }
            [Required(ErrorMessage = "Name Required")]
            [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
            public string studentName { get; set; }
            public string division { get; set; }
            public string rollNo { get; set; }
            public string className { get; set; }
            [StringLength(10, ErrorMessage = "Number cannot be longer than 10 characters.")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Contact must be numeric")]
            public string contactNo { get; set; }
            [Required(ErrorMessage = "Parent Name Required")]
            public string parentName { get; set; }
            public string tripNumber { get; set; }
            public string address { get; set; }
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
            public string state { get; set; }
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
            public string city { get; set; }
            public string classInCharge { get; set; }
            public string profilePic { get; set; }
            public string filePath { get; set; }
        }
        public class EditFeeClass
        {
            public long feeId { get; set; }
            public long feeClassId { get; set; }
            public string classname { get; set; }
            public string amount { get; set; }
            public DateTime DueDate { get; set; }

        }
        public class DiscountDetails
        {
            public long StudentId { get; set; }
        }
        public class TeacherAddModel
        {
            public long schoolId { get; set; }
            [Required(ErrorMessage = "Required")]
            public string teacherName { get; set; }
            public string classId { get; set; }
            public string classs { get; set; }
            public string divisionId { get; set; }
            [Required(ErrorMessage = "Required")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Contact must be numeric")]
            public string contactNumber { get; set; }
            [Required(ErrorMessage = "Required")]
            [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
            public string emailId { get; set; }
            public string image { get; set; }
            public string filePath { get; set; }
            public long? DesignationId { get; set; }
            public long? DepartmentId { get; set; }
            public long RoleId { get; set; }
            public string RolesData { get; set; }
            public long? UserTypeId { get; set; }
            public List<SelectListItem> Designations { get; set; }
            public List<SelectListItem> Departments { get; set; }
            public List<SelectListItem> UserTypes { get; set; }
            public List<SelectListItem> Classes { get; set; }

        }

        public class TeacherEditModel
        {
            public long schoolId { get; set; }
            public long teacherId { get; set; }

            [Required(ErrorMessage = "Required")]
            public string teacherName { get; set; }

            public string classId { get; set; }
            public string classs { get; set; }
            public string divisionId { get; set; }

            [Required(ErrorMessage = "Required")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Contact must be numeric")]
            public string contactNumber { get; set; }

            [Required(ErrorMessage = "Required")]
            [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
            public string emailId { get; set; }

            public string image { get; set; }
            public string filePath { get; set; }

            public long DepartmentId { get; set; }
            public long DesignationId { get; set; }


            public string RolesData { get; set; }

            public long? UserTypeId { get; set; }


            public IEnumerable<TbRoleDetail> RolesList { get; set; }

            public IEnumerable<TbDepartment> DepartmentList { get; set; }

            public IEnumerable<TbDesignation> DesignationList { get; set; }

            public IEnumerable<TbUserRole> UserTypeList { get; set; }
        }

        public class SchoolId
        {
            public long schoolId { get; set; }
        }
        public class TeacherId
        {
            public long teacherId { get; set; }
        }
        public class PrintBill
        {
            public long studentId { get; set; }
            public long billNumber { get; set; }


        }
        public class CircularList
        {
            public long schoolId { get; set; }
        }
        public class TrialBalanceModel
        {
            public DateTime Today { get; set; }
            public DateTime StartDate { get; set; }
            public long SchoolId { get; set; }
        }
        public class ClassDivisionDetailsForPrint
        {
            public string SchoolName { get; set; }
            public string SchoolAddress { get; set; }
            public string SchoolLogo { get; set; }
            public string ReportHead { get; set; }
            public List<SchoolValue> Details { get; set; }
        }
        public class TeacherSubjectRelationModel
        {
            public long SchoolId { get; set; }
            public long TeacherId { get; set; }
            public List<TeacherListModel> TeacherListModel { get; set; }
        }
        public class TeacherListModel
        {
            public string TeacherName { get; set; }
            public List<TeacherSubjectList> TeacherSubjectList { get; set; }
        }
        public class TeacherSubjectList
        {
            public string Subject { get; set; }
            public string ClassList { get; set; }
        }
        public class RemarkModel
        {
            public long SchoolId { get; set; }
            [Required(ErrorMessage = "Required")]
            public string Remark { get; set; }
            public long RemarkId { get; set; }
        }
        public class StudentExamTotalSccore
        {
            public long SchoolIdModel { get; set; }
            public long ClassIdModel { get; set; }
            public long DivisionIdModel { get; set; }
            public long ExamIdModel { get; set; }
            public List<StudentListForTotalSccore> list { get; set; }
        }
        public class StudentListForTotalSccore
        {
            public string StudentName { get; set; }
            public long StudentId { get; set; }
            public int RollNo { get; set; }
            public decimal MarkObtained { get; set; }
            public decimal MarkPercentage { get; set; }
        }


        public class UpPublishedClassList
        {
            public long SchoolId { get; set; }
            public List<UnPublishedClass> list { get; set; }
        }

        public class UnPublishedClass
        {
            public long ClassId { get; set; }
            public string ClassName { get; set; }
            public long DivisionId { get; set; }
            public string DivisionName { get; set; }
            public string AccademicYear { get; set; }
            public bool CurrentYearStatus { get; set; }
            public int SlNo { get; set; }
        }

        public class PromoteStudents
        {
            public long SchoolId { get; set; }
            public long OldClassId { get; set; }
            public long OldDivId { get; set; }
            public long OldAcademicyearId { get; set; }
            public long NewCLassId { get; set; }
            public long NewDivId { get; set; }
            public long NewAcademicYearId { get; set; }
            public List<StudentListForPromote> StudentList { get; set; }
            public string StudentListString { get; set; }
        }

        public class StudentListForPromote
        {
            public int SlNo { get; set; }
            public long StudentId { get; set; }
            public string StudentName { get; set; }
            public string SpecialId { get; set; }
            public string ContactNumber { get; set; }
        }


    }
}
