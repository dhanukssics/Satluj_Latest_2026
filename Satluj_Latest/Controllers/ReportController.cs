
using Satluj_Latest.Controllers;
using Satluj_Latest.Repository;
using Satluj_Latest.Models;
using Microsoft.AspNetCore.Mvc;
using Satluj_Latest.Data;

namespace Satluj_Latest.Controllers
{
    public class ReportController : BaseController
    {
        private readonly DropdownData _dropdown;
        public ReportController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        // GET: Report
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OutstandingReport()
        {
            OutstandingReportModel model = new OutstandingReportModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.getclasses = _dropdown.GetClasses(model.SchoolId);
            ViewBag.fees = _dropdown.GetFeeses(model.SchoolId);
            return View(model);
        }
        public PartialViewResult DatatableOutStandingReport(string id)
        {
            #region
            //OutstandingReportModel model = new OutstandingReportModel();
            //model.SchoolId = _user.SchoolId;
            //string[] splitData = id.Split('~');
            //long schoolId = _user.SchoolId;
            //long classId = Convert.ToInt64(splitData[0]);
            //if (classId != 0)
            //{
            //    long divisionId = Convert.ToInt64(splitData[1]);
            //    model.ReportList = new List<ReportDateList>();
            //    var data = _Entities.Sp_OutStandingReport(schoolId, classId, divisionId).ToList();
            //    if (data != null)
            //    {
            //        var students = data.Select(o => o.StudentId).Distinct().ToList();
            //        foreach (var item in students)
            //        {
            //            var isNeedStatus = false;
            //            var Amount = data.Where(z => z.StudentId == item).Sum(z => z.Amount);
            //            var advance = _Entities.tb_StudentBalance.Where(z => z.IsActive == true && z.StudentId == item).FirstOrDefault();
            //            if (advance != null)
            //            {
            //                if (advance.Amount > Amount)
            //                {
            //                    isNeedStatus = true;
            //                }
            //                else
            //                {
            //                    Amount = Amount - advance.Amount;
            //                }
            //            }
            //            if (isNeedStatus == false)
            //            {
            //                var student = _Entities.tb_Student.Where(x => x.StudentId == item && x.IsActive).FirstOrDefault();
            //                ReportDateList one = new ReportDateList();
            //                one.StudentId = student.StudentId;
            //                one.StudentName = student.StundentName;
            //                one.ClassId = student.ClassId;
            //                one.ClassName = student.tb_Class.Class;
            //                one.DivisionId = student.DivisionId;
            //                one.DivisionName = student.tb_Division.Division;
            //                one.Amount = Amount ?? 0;
            //                one.ContactNumber = student.ContactNumber;
            //                one.ClassOrder = student.tb_Class.ClassOrder;
            //                model.ReportList.Add(one);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    model.ReportList = new List<ReportDateList>();
            //    var data = _Entities.Sp_OutStandingReport(schoolId, 0, 0).ToList();
            //    if (data != null)
            //    {
            //        var students = data.Select(o => o.StudentId).Distinct().ToList();
            //        foreach (var item in students)
            //        {
            //            var isNeedStatus = false;
            //            var Amount = data.Where(z => z.StudentId == item).Sum(z => z.Amount);
            //            var advance = _Entities.tb_StudentBalance.Where(z => z.IsActive == true && z.StudentId == item).FirstOrDefault();
            //            if (advance != null)
            //            {
            //                if (advance.Amount > Amount)
            //                {
            //                    isNeedStatus = true;
            //                }
            //                else
            //                {
            //                    Amount = Amount - advance.Amount;
            //                }
            //            }
            //            if (isNeedStatus == false)
            //            {
            //                var student = _Entities.tb_Student.Where(x => x.StudentId == item && x.IsActive).FirstOrDefault();
            //                ReportDateList one = new ReportDateList();
            //                one.StudentId = student.StudentId;
            //                one.StudentName = student.StundentName;
            //                one.ClassId = student.ClassId;
            //                one.ClassName = student.tb_Class.Class;
            //                one.DivisionId = student.DivisionId;
            //                one.DivisionName = student.tb_Division.Division;
            //                one.Amount = Amount ?? 0;
            //                one.ContactNumber = student.ContactNumber;
            //                one.ClassOrder = student.tb_Class.ClassOrder;
            //                model.ReportList.Add(one);
            //            }
            //        }
            //    }
            //}
            //List<ReportDateList> newDataCollection = new List<ReportDateList>();
            //newDataCollection = model.ReportList.OrderBy(x => x.ClassOrder).ThenBy(x => x.DivisionId).ThenBy(x => x.StudentName).ToList();
            //model.ReportList = new List<ReportDateList>();
            //model.ReportList = newDataCollection;
            //return PartialView("~/Views/Report/_pv_OutstandingReportList.cshtml", model);
            #endregion

            OutstandingReportModel model = new OutstandingReportModel();
            string[] splitData = id.Split('~');
            model.SchoolId = _user.SchoolId; 
            model.ClassId = Convert.ToInt64(splitData[0]);
            if (model.ClassId == 0)
                model.DivisionId = 0;
            else
                model.DivisionId = Convert.ToInt64(splitData[1]);
            try
            {
            model.FeeId = Convert.ToInt64(splitData[2]);
            }
            catch
            {
                model.FeeId = 0;
            }
            return PartialView("~/Views/Report/_OutstandingReportMultipleList.cshtml", model);
        }

        public IActionResult DetailedCollectionReport()
        {
            FeeModel model = new FeeModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = CurrentTime;
            model.EndDate = CurrentTime;
            return View(model);
        }
        public object SearchDetailedData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            FeeModel model = new FeeModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = start;
            model.EndDate = end;
            return PartialView("~/Views/Report/_pv_DetailedCollectionReportList.cshtml", model);
        }


        public IActionResult MonthlyAttendanceReport()
        {
            MonthlyAttandanceModel model = new MonthlyAttandanceModel();
            model.SchoolId = _user.SchoolId;
            model.AttandanceDate = CurrentTime;
            model.ShiftId = 0;
            ViewBag.Classlist = _dropdown.GetClasses(model.SchoolId);
            return View(model);
        }
        public PartialViewResult DatatableMonthlyAttendanceReport(string id)
        {
            MonthlyAttandanceModel model = new MonthlyAttandanceModel();
            model.SchoolId = _user.SchoolId;
            string[] splitData = id.Split('~');
            try
            {
                model.ClassId = Convert.ToInt64(splitData[0]);
                model.DivisionId = Convert.ToInt64(splitData[1]);
                if (splitData[2] == "0")
                    model.ShiftId = AttendanceShift.Morning;
                else
                    model.ShiftId = AttendanceShift.Evening;
                model.AttandanceDate = Convert.ToDateTime(splitData[3]);
                model.shiftData = Convert.ToInt32(splitData[2]);
            }
            catch (Exception ex)
            {

            }
            return PartialView("~/Views/Report/_pv_MonthlyAttendanceList.cshtml", model);
        }
        public IActionResult StudentList()
        {
            StudentModel model = new StudentModel();
            model.schoolId = _user.SchoolId;
            model.SchoolName = _user.School.SchoolName;
            ViewBag.classlist = _dropdown.GetClasses(model.schoolId);
            return View(model);
        }

        public PartialViewResult DatatableStudentsListReport(string id)
        {
            StudentModel model = new StudentModel();
            model.schoolId = _user.SchoolId;
            string[] splitData = id.Split('~');
            //long schoolId = _user.SchoolId;
            model.classId = Convert.ToInt64(splitData[0]);
            model.divisionId = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/Report/_pv_StudentList.cshtml", model);
        }


        public IActionResult BilledReport()
        {
            FeeModel model = new FeeModel();
            model.StartDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult BilledReportByDate(string id)
        {
            FeeModel model = new FeeModel();

            string[] splitData = id.Split('~');


            DateTime FDate = Convert.ToDateTime(splitData[0]);
            DateTime LDate = Convert.ToDateTime(splitData[1]);

            //string[] splitFDate = FDate.Split('-');
            //string startDate = splitFDate[1] + '/' + splitFDate[0] + '/' + splitFDate[2];

            //string[] splitLDate = LDate.Split('-');
            //string endDate = splitLDate[1] + '/' + splitLDate[0] + '/' + splitLDate[2] + ' ' + "11:59:00 PM";

            string endDate = LDate.ToString("MM-dd-yyyy") + ' ' + "11:59:00 PM";
            model.StartDate = Convert.ToDateTime(FDate);
            model.EndDate = Convert.ToDateTime(endDate);
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Report/_BilledReport_Grid.cshtml", model);
        }
    }
}