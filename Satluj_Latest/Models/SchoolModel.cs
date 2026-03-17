using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class SchoolModel
    {
        public long schoolId { get; set; }
        public long classId { get; set; }
        public long studentId { get; set; }
        public long teacherId { get; set; }
        public long busId { get; set; }
        public DateTime attendanceDate { get; set; }
        public DateTime curredntDateTime { get; set; }

        public int shiftStatus { get; set; }
        public long userId { get; set; }
        public List<TbStudent> Students { get; set; }
        public long divisionId { get; set; }
        public string studentName { get; set; }
        public string division { get; set; }
        public string rollNo { get; set; }
        public string className { get; set; }
        public string classInCharge { get; set; }
        public string classNumber { get; set; }//Archana

        public bool IsSendSMS { get; set; }
        public List<Student> studentList { get; set; }
        public DateTime Selecteddate { get; set; }
        public string Selecteddate_From { get; set; }
        public string Selecteddate_To { get; set; }
        public IFormFile fileData { get; set; }
        public int opr { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public long CurrentAddedStudent { get; set; }
        public string DivisionStringId { get; set; }
        public string TeacherName { get; set; }
        public DateTime Currenttime { get; set; }

    }
    public partial class Student
    {
        public string studentId { get; set; }
        public long StudentId { get; internal set; }
        public string attendaneStatus { get; set; } //Check In=1 Check Out =0;
    }
    public class SchoolModelForId
    {
        public long SchoolId { get; set; }
        public string Numbers { get; set; }
        public string StudentId { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public int MessageSentPerStudent { get; set; }
    }

    public class IncomeData
    {
        public string HeadValue { get; set; }
        public string ParticularValue { get; set; }
        public string Amount { get; set; }
        public string SelectedDate { get; set; }
        public long Id { get; set; }
    }
    public class SendMessage
    {
        public string Description { get; set; }
        public string Phone { get; set; }
        public List<PhoneNumbers> list { get; set; }
    }
    public class PhoneNumbers
    {
        public string Number { get; set; }
        public string StudentId { get; set; }
        public string SendStatus { get; set; }
    }
    public class DateClass
    {
        public DateTime Selecteddate { get; set; }
        public long SchoolId { get; set; }
    }
    public class SendStaffMessage
    {
        public string Description { get; set; }
        public string Phone { get; set; }
        public List<StaffPhoneNumbers> list { get; set; }
    }
    public class StaffPhoneNumbers
    {
        public string Number { get; set; }
        public string StaffId { get; set; }
        public string SendStatus { get; set; }
        public string Type { get; set; }
    }
}