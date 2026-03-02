using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using static Satluj_Latest.Models.SchoolValue;

namespace Satluj_Latest.Controllers
{
    public class PerformanceController : BaseController
    {
        private readonly DropdownData _dropdown;
        public PerformanceController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        public IActionResult TeacherRoleManagement()
        {
            PerformanceModel model = new PerformanceModel();
            model.SchoolId = _user.SchoolId;
            model.ClassList = new List<ClassMainList>();
            var classList = _Entities.TbClasses.Where(x => x.SchoolId == model.SchoolId && x.IsActive && x.PublishStatus).OrderBy(x => x.ClassOrder).ToList();
            foreach (var item in classList)
            {
                ClassMainList one = new ClassMainList();
                one.ClassId = item.ClassId;
                one.ClassName = item.Class;
                one.DivisionList = new List<ClassDivisiomList>();
                one.DivisionListIdString = "";
                var divisionList = _Entities.TbDivisions.Where(x => x.ClassId == item.ClassId && x.IsActive).OrderBy(x => x.Division).ToList();
                foreach (var item2 in divisionList)
                {
                    ClassDivisiomList two = new ClassDivisiomList();
                    two.DivId = item2.DivisionId;
                    two.DivisionName = item2.Division;
                    one.DivisionList.Add(two);
                    if (one.DivisionListIdString == "")
                        one.DivisionListIdString = two.DivId.ToString();
                    else
                        one.DivisionListIdString = one.DivisionListIdString + "," + two.DivId;
                }
                model.ClassList.Add(one);
            }
            ViewBag.teacherlist = _dropdown.GetTeachers(model.SchoolId);
            ViewBag.subjectlist = _dropdown.GetSubjectss(model.SchoolId);
            return View(model);
        }
        public object SubmitAddTeacherClassSubject(PerformanceModel model)
        {
            string msg = "Failed";
            bool status = false;
            try
            {
                var IsAdminTeacher = _Entities.TbTeachers.Where(x => x.TeacherId == model.TeacherId && x.UserTypeNavigation.IsAdmin == true && x.IsActive && x.UserTypeNavigation.IsActive).FirstOrDefault();
                if (IsAdminTeacher != null)
                {
                    msg = "The teacher is an Admin";
                }
                else
                {
                    var old = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.IsActive).ToList();
                    if (old.Count > 0 && old != null)
                    {
                        msg = "Already exists";
                    }
                    else
                    {
                        foreach (var item in model.ClassList)
                        {
                            var teacher = new TbTeacherClassSubject();
                            teacher.TeacherId = model.TeacherId;
                            teacher.SubjectId = model.SubjectId;
                            teacher.SchoolId = _user.SchoolId;
                            teacher.DivisionId = item.ClassId;
                            var divisionData = new Satluj_Latest.Data.Division(item.ClassId);
                            teacher.ClassId = divisionData.ClassId;
                            teacher.IsActive = true;
                            teacher.TimeStamp = CurrentTime;
                            _Entities.TbTeacherClassSubjects.Add(teacher);
                            status = _Entities.SaveChanges() > 0;
                        }
                        if (status)
                            msg = "Successful";
                    }
                }
            }
            catch
            {
                var old = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.IsActive).ToList();
                if (old.Count > 0 && old != null)
                {
                    msg = "Already exists";
                }
                else
                {
                    foreach (var item in model.ClassList)
                    {
                        var teacher = new TbTeacherClassSubject();
                        teacher.TeacherId = model.TeacherId;
                        teacher.SubjectId = model.SubjectId;
                        teacher.SchoolId = _user.SchoolId;
                        teacher.DivisionId = item.ClassId;
                        var divisionData = new Satluj_Latest.Data.Division(item.ClassId);
                        teacher.ClassId = divisionData.ClassId;
                        teacher.IsActive = true;
                        teacher.TimeStamp = CurrentTime;
                        _Entities.TbTeacherClassSubjects.Add(teacher);
                        status = _Entities.SaveChanges() > 0;
                    }
                    if (status)
                        msg = "Successful";
                }
            }
            return Json(new { status = status, msg = msg }, JsonRequestBehavior.AllowGet);
        }
        public IActionResult TeacherRoleManagementEditHome()
        {
            PerformanceModel model = new PerformanceModel();
            model.SchoolId = _user.SchoolId;
            model.ClassList = new List<ClassMainList>();
            ViewBag.teacherlist = _dropdown.GetTeachers(model.SchoolId);
            ViewBag.subjectlist = _dropdown.GetSubjectss(model.SchoolId);

            return View(model);
        }
        public IActionResult EditTeacherRoleManagement(string id)
        {
            string[] splitData = id.Split('~');
            var teacherId = Convert.ToInt64(splitData[0]);
            var subjectId = Convert.ToInt64(splitData[1]);
            var teacher = _Entities.TbTeachers.Where(x => x.TeacherId == teacherId).FirstOrDefault();
            PerformanceModel model = new PerformanceModel();
            if (teacher.UserType != null && teacher.UserTypeNavigation.IsAdmin == true)
            {
                model.SchoolId = _user.SchoolId;
                model.TeacherId = teacherId;
                model.SubjectId = subjectId;
                model.ClassList = new List<ClassMainList>();
                var classOnly = _Entities.TbClasses.Where(x => x.SchoolId == _user.SchoolId && x.PublishStatus == true && x.IsActive).ToList();
                foreach (var item in classOnly)
                {
                    ClassMainList one = new ClassMainList();
                    one.ClassId = item.ClassId;
                    one.ClassName = item.Class;
                    one.IsFullExists = true;
                    one.DivisionListIdString = "";
                    one.DivisionList = new List<ClassDivisiomList>();
                    var fullDivisionList = _Entities.TbDivisions.Where(x => x.ClassId == one.ClassId && x.IsActive).ToList();
                    foreach (var div in fullDivisionList)
                    {
                        ClassDivisiomList two = new ClassDivisiomList();
                        two.DivId = div.DivisionId;
                        two.DivisionName = div.Division;
                        two.IsExits = true;
                        one.DivisionList.Add(two);
                        if (one.DivisionListIdString == "")
                            one.DivisionListIdString = two.DivId.ToString();
                        else
                            one.DivisionListIdString = one.DivisionListIdString + "," + two.DivId;
                    }
                    model.ClassList.Add(one);
                }
            }
            else
            {
                model.SchoolId = _user.SchoolId;
                model.TeacherId = teacherId;
                model.SubjectId = subjectId;
                model.ClassList = new List<ClassMainList>();
                var data = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == teacherId && x.SubjectId == subjectId && x.IsActive).ToList();
                var classOnly = _Entities.TbClasses.Where(x => x.SchoolId == _user.SchoolId && x.PublishStatus == true && x.IsActive).ToList();
                foreach (var item in classOnly)
                {
                    ClassMainList one = new ClassMainList();
                    one.ClassId = item.ClassId;
                    one.ClassName = item.Class;
                    var fullDivisionList = _Entities.TbDivisions.Where(x => x.ClassId == one.ClassId && x.IsActive).ToList();
                    var divisionList = data.Where(x => x.ClassId == one.ClassId).ToList();
                    if (fullDivisionList.Count == divisionList.Count)
                        one.IsFullExists = true;
                    else
                        one.IsFullExists = false;
                    one.DivisionListIdString = "";
                    one.DivisionList = new List<ClassDivisiomList>();
                    foreach (var div in fullDivisionList)
                    {
                        ClassDivisiomList two = new ClassDivisiomList();
                        two.DivId = div.DivisionId;
                        two.DivisionName = div.Division;
                        var isHave = divisionList.Where(x => x.DivisionId == two.DivId).FirstOrDefault();
                        if (isHave != null)
                            two.IsExits = true;
                        else
                            two.IsExits = false;
                        one.DivisionList.Add(two);
                        if (one.DivisionListIdString == "")
                            one.DivisionListIdString = two.DivId.ToString();
                        else
                            one.DivisionListIdString = one.DivisionListIdString + "," + two.DivId;
                    }
                    model.ClassList.Add(one);
                }
            }
            return PartialView("~/Views/Performance/_pv_TeacherRoleEditView.cshtml", model);
        }
        public object SubmitEditTeacherClassSubject(PerformanceModel model)
        {
            string msg = "Failed";
            bool status = false;
            try
            {
                var IsAdminTeacher = _Entities.TbTeachers.Where(x => x.TeacherId == model.TeacherId && x.UserTypeNavigation.IsAdmin == true && x.IsActive && x.UserTypeNavigation.IsActive).FirstOrDefault();
                if (IsAdminTeacher != null)
                {
                    msg = "The teacher is an Admin";
                }
                else
                {
                    var old = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.IsActive).ToList();
                    if (old.Count > 0 && old != null)
                    {
                        foreach (var item in old)
                        {
                            var isExistsThis = model.ClassList.Where(x => x.ClassId == item.DivisionId).FirstOrDefault();
                            if (isExistsThis != null)
                            {

                            }
                            else
                            {
                                var remove = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.DivisionId == item.DivisionId && x.IsActive).FirstOrDefault();
                                remove.IsActive = false;
                                status = _Entities.SaveChanges() > 0;
                            }
                        }
                        var distinctDivs = model.ClassList.GroupBy(x => x.ClassId).ToList();
                        foreach (var item in distinctDivs)
                        {
                            var isNew = old.Where(x => x.DivisionId == item.Key).FirstOrDefault();
                            if (isNew != null)
                            {

                            }
                            else
                            {
                                var teacher = new TbTeacherClassSubject();
                                teacher.TeacherId = model.TeacherId;
                                teacher.SubjectId = model.SubjectId;
                                teacher.SchoolId = _user.SchoolId;
                                teacher.DivisionId = item.Key;
                                var divisionData = new Data.Division(item.Key);
                                teacher.ClassId = divisionData.ClassId;
                                teacher.IsActive = true;
                                teacher.TimeStamp = CurrentTime;
                                _Entities.TbTeacherClassSubjects.Add(teacher);
                                status = _Entities.SaveChanges() > 0;
                            }

                        }
                        if (status)
                            msg = "Successful";
                    }
                    else
                    {
                        #region
                        var distinctDivs = model.ClassList.GroupBy(x => x.ClassId).ToList();
                        foreach (var item in distinctDivs)
                        {
                            var teacher = new TbTeacherClassSubject();
                            teacher.TeacherId = model.TeacherId;
                            teacher.SubjectId = model.SubjectId;
                            teacher.SchoolId = _user.SchoolId;
                            teacher.DivisionId = item.Key;
                            var divisionData = new Satluj_Latest.Data.Division(item.Key);
                            teacher.ClassId = divisionData.ClassId;
                            teacher.IsActive = true;
                            teacher.TimeStamp = CurrentTime;
                            _Entities.TbTeacherClassSubjects.Add(teacher);
                            status = _Entities.SaveChanges() > 0;
                        }
                        if (status)
                            msg = "Successful";
                        #endregion
                    }
                }
            }
            catch
            {
                var old = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.IsActive).ToList();
                if (old.Count > 0 && old != null)
                {
                    foreach (var item in old)
                    {
                        var isExistsThis = model.ClassList.Where(x => x.ClassId == item.DivisionId).FirstOrDefault();
                        if (isExistsThis != null)
                        {

                        }
                        else
                        {
                            var remove = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.DivisionId == item.DivisionId && x.IsActive).FirstOrDefault();
                            remove.IsActive = false;
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                    var distinctDivs = model.ClassList.GroupBy(x => x.ClassId).ToList();
                    foreach (var item in distinctDivs)
                    {
                        var isNew = old.Where(x => x.DivisionId == item.Key).FirstOrDefault();
                        if (isNew != null)
                        {

                        }
                        else
                        {
                            var teacher = new TbTeacherClassSubject();
                            teacher.TeacherId = model.TeacherId;
                            teacher.SubjectId = model.SubjectId;
                            teacher.SchoolId = _user.SchoolId;
                            teacher.DivisionId = item.Key;
                            var divisionData = new Data.Division(item.Key);
                            teacher.ClassId = divisionData.ClassId;
                            teacher.IsActive = true;
                            teacher.TimeStamp = CurrentTime;
                            _Entities.TbTeacherClassSubjects.Add(teacher);
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                    if (status)
                        msg = "Successful";
                }
                else
                {
                    #region
                    var distinctDivs = model.ClassList.GroupBy(x => x.ClassId).ToList();
                    foreach (var item in distinctDivs)
                    {
                        var teacher = new TbTeacherClassSubject();
                        teacher.TeacherId = model.TeacherId;
                        teacher.SubjectId = model.SubjectId;
                        teacher.SchoolId = _user.SchoolId;
                        teacher.DivisionId = item.Key;
                        var divisionData = new Satluj_Latest.Data.Division(item.Key);
                        teacher.ClassId = divisionData.ClassId;
                        teacher.IsActive = true;
                        teacher.TimeStamp = CurrentTime;
                        _Entities.TbTeacherClassSubjects.Add(teacher);
                        status = _Entities.SaveChanges() > 0;
                    }
                    if (status)
                        msg = "Successful";
                    #endregion
                }
            }
            return Json(new { status = status, msg = msg }, JsonRequestBehavior.AllowGet);
        }
        public IActionResult PerformanceHome()
        {
            PerformanceGraphModel model = new PerformanceGraphModel();
            model.SchoolId = _user.SchoolId;
            model.UserId = _user.UserId;
            ViewBag.classlist = _dropdown.GetClasses(model.SchoolId);
            ViewBag.classwise = _dropdown.GetClassesUserWise(model.SchoolId, model.UserId);
            bool admin= Convert.ToBoolean(HttpContext.Session.GetString("IsAdmin"));
            ViewBag.IsAdmin = admin;
            return View(model);
        }
        public IActionResult PerformancePieChart(string id)
        {
            PerformanceGraphModel model = new PerformanceGraphModel();
            return PartialView("~/Views/Performance/_pv_PerformancePieChartView.cshtml", model);
        }
        public IActionResult PieChart(string id)
        {
            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            long DivisionId = Convert.ToInt64(splitData[1]);
            long ExamId = Convert.ToInt64(splitData[2]);
            long SubjectId = Convert.ToInt64(splitData[3]);
            int count = 1;
            Chart model = new Chart();
            model.list = new List<SingleCharts>();
            if (DivisionId == 0)
            {
                model.SchoolId = _user.SchoolId;
                if (SubjectId == -1)// Total Subjects Marks
                {
                    #region Total
                    var fullDivisions = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.ClassId == ClassId).ToList();
                    var data = _Entities.spStudentMarkPercentage_Result
        .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                    ExamId)
        .ToList();

    //                _Entities.spStudentMarkPercentage_Result
    //                    .FromSqlRaw(
    //    "EXEC sp_StudentMarkPercentage @ExamId",
    //    new SqlParameter("@ExamId", ExamId)
    //)
    //.AsNoTracking()
    //.ToList();

                    foreach (var item in fullDivisions)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Division : " + item.Division;
                        var markData = data.Where(x => x.DivisionId == item.DivisionId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == item.DivisionId && x.IsActive).ToList();
                        var groupedData = markData.GroupBy(d => d.StudentId).Select(
                                g => new
                                {
                                    Key = g.Key,
                                    StudentId = g.First().StudentId,
                                    Percentage = g.Average(s => s.Percentage)
                                });

                        decimal NTP = groupedData.Where(x => x.Percentage < 41).Count();
                        decimal Average = groupedData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = groupedData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = groupedData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == fullDivisions.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }
                    #endregion Total
                }
                else // Particular Subject
                {
                    #region Particular Subject
                    var fullDivisions = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.ClassId == ClassId).ToList();
                    var data = _Entities.spStudentMarkPercentage_Result
                          .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                      ExamId)
                          .ToList();
                    foreach (var item in fullDivisions)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Division : " + item.Division;
                        var markData = data.Where(x => x.DivisionId == item.DivisionId && x.SubjectId == SubjectId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == item.DivisionId && x.IsActive).ToList();

                        decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                        decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = markData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == fullDivisions.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }
                    #endregion Total
                }
            }
            else
            {
                //var data = _Entities.sp_StudentMarkPercentage(ExamId).ToList();
                var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  ExamId)
      .ToList();

                if (SubjectId == 0)//All Subjects 
                {
                    var allSubjects = _Entities.TbExamSubjects.Where(x => x.ExamId == ExamId && x.IsActive).ToList();
                    foreach (var item in allSubjects)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Subject : " + item.SubjectNavigation.SubjectName;
                        var thisDivData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == item.SubId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();

                        decimal NTP = thisDivData.Where(x => x.Percentage < 41).Count();
                        decimal Average = thisDivData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = thisDivData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = thisDivData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == allSubjects.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }

                }
                else if (SubjectId == -1)// Total Subjects 
                {
                    var DivisionData = _Entities.TbDivisions.Where(x => x.DivisionId == DivisionId && x.IsActive).FirstOrDefault();
                    SingleCharts one = new SingleCharts();
                    one.ChartName = "Division : " + DivisionData.Division;

                    var markData = data.Where(x => x.DivisionId == DivisionId).ToList();
                    var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();
                    var groupedData = markData.GroupBy(d => d.StudentId).Select(
                            g => new
                            {
                                Key = g.Key,
                                StudentId = g.First().StudentId,
                                Percentage = g.Average(s => s.Percentage)
                            });

                    decimal NTP = groupedData.Where(x => x.Percentage < 41).Count();
                    decimal Average = groupedData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                    decimal Merit = groupedData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                    decimal Super = groupedData.Where(x => x.Percentage > 92).Count();

                    if (totalStudents.Count > 0 && totalStudents != null)
                    {
                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                        one.Average = ((Average / totalStudents.Count()) * 360);
                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                        one.Super = ((Super / totalStudents.Count()) * 360);
                    }
                    else
                    {
                        one.NTP = 0;
                        one.Average = 0;
                        one.Merit = 0;
                        one.Super = 0;
                    }
                    one.last = false;
                    model.list.Add(one);
                    count = count + 1;
                }
                else
                {

                    var DivisionData = _Entities.TbDivisions.Where(x => x.DivisionId == DivisionId && x.IsActive).FirstOrDefault();
                    SingleCharts one = new SingleCharts();
                    one.ChartName = "Division :" + DivisionData.Division;

                    var markData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == SubjectId).ToList();
                    var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();

                    decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                    decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                    decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                    decimal Super = markData.Where(x => x.Percentage > 92).Count();

                    if (totalStudents.Count > 0 && totalStudents != null)
                    {
                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                        one.Average = ((Average / totalStudents.Count()) * 360);
                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                        one.Super = ((Super / totalStudents.Count()) * 360);
                    }
                    else
                    {
                        one.NTP = 0;
                        one.Average = 0;
                        one.Merit = 0;
                        one.Super = 0;
                    }
                    one.last = false;
                    model.list.Add(one);
                    count = count + 1;
                }
            }
            //var jsonSerialiser = new JavaScriptSerializer();
            //model.fullList = jsonSerialiser.Serialize(model.list);
            model.fullList = JsonConvert.SerializeObject(model.list);

            return View(model);
        }
        public IActionResult PieChartPartial(string id)
        {
            string[] splitData = id.Split('~');
            long ClassId = Convert.ToInt64(splitData[0]);
            long DivisionId = Convert.ToInt64(splitData[1]);
            long ExamId = Convert.ToInt64(splitData[2]);
            long SubjectId = Convert.ToInt64(splitData[3]);
            int count = 1;
            Chart model = new Chart();
            model.list = new List<SingleCharts>();
            var ClassDetails = new Satluj_Latest.Data.Class(ClassId);
            if (DivisionId == 0)
            {
                model.SchoolId = _user.SchoolId;
                if (SubjectId == -1)// Total Subjects Marks
                {
                    model.Heading = ClassDetails.ClassName + " ( All Division ) Total Subjects";
                    #region Total
                    var fullDivisions = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.ClassId == ClassId).ToList();
                    var data = _Entities.spStudentMarkPercentage_Result
                          .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                      ExamId)
                          .ToList();
                    foreach (var item in fullDivisions)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Division : " + item.Division;
                        var markData = data.Where(x => x.DivisionId == item.DivisionId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == item.DivisionId && x.IsActive).ToList();
                        var groupedData = markData.GroupBy(d => d.StudentId).Select(
                                g => new
                                {
                                    Key = g.Key,
                                    StudentId = g.First().StudentId,
                                    Percentage = g.Average(s => s.Percentage)
                                });

                        decimal NTP = groupedData.Where(x => x.Percentage < 41).Count();
                        decimal Average = groupedData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = groupedData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = groupedData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == fullDivisions.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }
                    #endregion Total
                }
                else // Particular Subject
                {
                    //var subject = new TrackTap.DataLibrary.Data.Subjects(SubjectId);
                    var subject = new Satluj_Latest.Data.ExamSubjects(SubjectId);
                    model.Heading = ClassDetails.ClassName + " ( All Division ) " + subject.SubjectName;
                    #region Particular Subject
                    var fullDivisions = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.ClassId == ClassId).ToList();
                    var data = _Entities.spStudentMarkPercentage_Result
                          .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                      ExamId)
                          .ToList();
                    foreach (var item in fullDivisions)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Division : " + item.Division;
                        var markData = data.Where(x => x.DivisionId == item.DivisionId && x.SubjectId == SubjectId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == item.DivisionId && x.IsActive).ToList();

                        decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                        decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = markData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == fullDivisions.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }
                    #endregion Total
                }
            }
            else
            {
                var DivisionData = new Satluj_Latest.Data.Division(DivisionId);
                //var data = _Entities.sp_StudentMarkPercentage(ExamId).ToList();
                var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  ExamId)
      .ToList();

                if (SubjectId == 0)//All Subjects 
                {
                    var allSubjects = _Entities.TbExamSubjects.Where(x => x.ExamId == ExamId && x.IsActive).ToList();
                    model.Heading = ClassDetails.ClassName + " ( " + DivisionData.DivisionName + " ) All Subjects";

                    foreach (var item in allSubjects)
                    {
                        SingleCharts one = new SingleCharts();
                        one.ChartName = "Subject : " + item.SubjectNavigation.SubjectName;
                        var thisDivData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == item.SubId).ToList();
                        var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();

                        decimal NTP = thisDivData.Where(x => x.Percentage < 41).Count();
                        decimal Average = thisDivData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                        decimal Merit = thisDivData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                        decimal Super = thisDivData.Where(x => x.Percentage > 92).Count();

                        if (totalStudents.Count > 0 && totalStudents != null)
                        {
                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                            one.Average = ((Average / totalStudents.Count()) * 360);
                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                            one.Super = ((Super / totalStudents.Count()) * 360);
                        }
                        else
                        {
                            one.NTP = 0;
                            one.Average = 0;
                            one.Merit = 0;
                            one.Super = 0;
                        }
                        if (count == allSubjects.Count)
                            one.last = true;
                        else
                            one.last = false;
                        model.list.Add(one);
                        count = count + 1;
                    }

                }
                else if (SubjectId == -1)// Total Subjects 
                {
                    model.Heading = ClassDetails.ClassName + " ( " + DivisionData.DivisionName + " ) Total Subjects";
                    SingleCharts one = new SingleCharts();
                    one.ChartName = "Division : " + DivisionData.DivisionName;

                    var markData = data.Where(x => x.DivisionId == DivisionId).ToList();
                    var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();
                    var groupedData = markData.GroupBy(d => d.StudentId).Select(
                            g => new
                            {
                                Key = g.Key,
                                StudentId = g.First().StudentId,
                                Percentage = g.Average(s => s.Percentage)
                            });

                    decimal NTP = groupedData.Where(x => x.Percentage < 41).Count();
                    decimal Average = groupedData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                    decimal Merit = groupedData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                    decimal Super = groupedData.Where(x => x.Percentage > 92).Count();

                    if (totalStudents.Count > 0 && totalStudents != null)
                    {
                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                        one.Average = ((Average / totalStudents.Count()) * 360);
                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                        one.Super = ((Super / totalStudents.Count()) * 360);
                    }
                    else
                    {
                        one.NTP = 0;
                        one.Average = 0;
                        one.Merit = 0;
                        one.Super = 0;
                    }
                    one.last = false;
                    model.list.Add(one);
                    count = count + 1;
                }
                else
                {
                    //var subDetails = new TrackTap.DataLibrary.Data.Subjects(SubjectId);
                    var subDetails = new Satluj_Latest.Data.ExamSubjects(SubjectId);
                    model.Heading = ClassDetails.ClassName + " ( " + DivisionData.DivisionName + " ) " + subDetails.SubjectName;
                    SingleCharts one = new SingleCharts();
                    one.ChartName = "Division :" + DivisionData.DivisionName;

                    var markData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == SubjectId).ToList();
                    var totalStudents = _Entities.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).ToList();

                    decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                    decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                    decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                    decimal Super = markData.Where(x => x.Percentage > 92).Count();

                    if (totalStudents.Count > 0 && totalStudents != null)
                    {
                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                        one.Average = ((Average / totalStudents.Count()) * 360);
                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                        one.Super = ((Super / totalStudents.Count()) * 360);
                    }
                    else
                    {
                        one.NTP = 0;
                        one.Average = 0;
                        one.Merit = 0;
                        one.Super = 0;
                    }
                    one.last = false;
                    model.list.Add(one);
                    count = count + 1;
                }
            }
            model.fullList = JsonConvert.SerializeObject(model.list);
            return PartialView("~/Views/Performance/_pv_ChartPartialView.cshtml", model);
        }
        public IActionResult TeacherSubject()
        {
            Satluj_Latest.Models.TeacherSubjectRelationModel model = new Satluj_Latest.Models.TeacherSubjectRelationModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Teacherlist = _dropdown.GetTeachers(model.SchoolId);
            return View(model);
        }
        public IActionResult TeacherSubjectList(string id)
        {
           Models.TeacherSubjectRelationModel model = new Models.TeacherSubjectRelationModel();
            model.SchoolId = _user.SchoolId;
            model.TeacherId = Convert.ToInt64(id);
            model.TeacherListModel = new List<TeacherListModel>();
            if (model.TeacherId == 0) // All teachers 
            {
                var data = _Entities.TbTeacherClassSubjects.Where(x => x.IsActive && x.SchoolId == model.SchoolId && x.Class.IsActive && x.Class.PublishStatus).ToList();
                var dataTeachersOnly = data.Select(x => x.TeacherId).Distinct().ToList();
                foreach (var item in dataTeachersOnly)
                {
                    var oneTeacher = data.Where(x => x.TeacherId == item).ToList();
                    TeacherListModel one = new TeacherListModel();
                    one.TeacherName = oneTeacher.FirstOrDefault().Teacher.TeacherName;
                    one.TeacherSubjectList = new List<TeacherSubjectList>();
                    var FullSubjects = oneTeacher.Select(x => x.SubjectId).Distinct();
                    foreach (var item2 in FullSubjects)
                    {
                        TeacherSubjectList two = new TeacherSubjectList();
                        two.Subject = data.Where(x => x.SubjectId == item2).FirstOrDefault().Subject.SubjectName;
                        var FullClass = data.Where(x => x.TeacherId == item && x.SubjectId == item2).OrderBy(x => x.Division.Division).OrderBy(x => x.Class.ClassOrder).ToList();
                        foreach (var item3 in FullClass)
                        {
                            if (two.ClassList == null)
                            {
                                two.ClassList = item3.Class.Class + "  " + item3.Division.Division;
                            }
                            else
                            {
                                two.ClassList = two.ClassList + "  ,  " + item3.Class.Class + " " + item3.Division.Division;
                            }
                        }
                        one.TeacherSubjectList.Add(two);
                    }
                    model.TeacherListModel.Add(one);
                }
            }
            else// Particular Teacher
            {
                var data = _Entities.TbTeacherClassSubjects.Where(x => x.IsActive && x.SchoolId == model.SchoolId && x.TeacherId == model.TeacherId && x.Class.IsActive && x.Class.PublishStatus).ToList();
                var dataTeachersOnly = data.Select(x => x.TeacherId).Distinct().ToList();
                foreach (var item in dataTeachersOnly)
                {
                    var oneTeacher = data.Where(x => x.TeacherId == item).ToList();
                    TeacherListModel one = new TeacherListModel();
                    one.TeacherName = oneTeacher.FirstOrDefault().Teacher.TeacherName;
                    one.TeacherSubjectList = new List<TeacherSubjectList>();
                    var FullSubjects = oneTeacher.Select(x => x.SubjectId).Distinct();
                    foreach (var item2 in FullSubjects)
                    {
                        TeacherSubjectList two = new TeacherSubjectList();
                        two.Subject = data.Where(x => x.SubjectId == item2).FirstOrDefault().Subject.SubjectName;
                        var FullClass = data.Where(x => x.TeacherId == item && x.SubjectId == item2).OrderBy(x => x.Division.Division).OrderBy(x => x.Class.ClassOrder).ToList();
                        foreach (var item3 in FullClass)
                        {
                            if (two.ClassList == null)
                            {
                                two.ClassList = item3.Class.Class + "  " + item3.Division.Division;
                            }
                            else
                            {
                                two.ClassList = two.ClassList + "  ,  " + item3.Class.Class + " " + item3.Division.Division;
                            }
                        }
                        one.TeacherSubjectList.Add(two);
                    }
                    model.TeacherListModel.Add(one);
                }
            }
            return PartialView("~/Views/Performance/_pv_TeacherSubject.cshtml", model);
        }
        public IActionResult TeacherPerformanceHome()
        {
            PerformanceGraphModel model = new PerformanceGraphModel();
            model.SchoolId = _user.SchoolId;
            model.UserId = _user.UserId;
            ViewBag.Teacherlist = _dropdown.GetTeachers(model.SchoolId);
            
            return View(model);
        }
        public IActionResult TeacherPieChartPartial(string id)
        {
            Chart model = new Chart();
            model.list = new List<SingleCharts>();
            string[] splitData = id.Split('~');
            long TeacherId = Convert.ToInt64(splitData[0]);
            long ClassId = Convert.ToInt64(splitData[1]);
            long DivisionId = Convert.ToInt64(splitData[2]);
            long ExamId = Convert.ToInt64(splitData[3]);
            long SubjectId = Convert.ToInt64(splitData[4]);

            var FullData = _Entities.TbTeacherClassSubjects.Where(x => x.SchoolId == _user.SchoolId && x.TeacherId == TeacherId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus).ToList();
            if (FullData == null && FullData.Count == 0)// Nothing allocated to this teacher
            {
                //NO PERMISSION
            }
            else
            {
                if (ClassId == 0)// All Classes 
                {
                    #region
                    var allclasss = FullData.Where(x => x.SchoolId == _user.SchoolId && x.TeacherId == TeacherId && x.IsActive).OrderBy(x => x.Class.ClassOrder).ToList().Select(x => new { x.ClassId, x.Class.Class }).Distinct().ToList();
                    if (DivisionId == 0)// All Divisions
                    {
                        #region
                        foreach (var item in allclasss)
                        {
                            var allDivisions = FullData.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == item.ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => new { x.DivisionId, x.Division.Division }).Distinct().ToList();
                            if (allDivisions != null && allDivisions.Count > 0)// Check that the teacher have the division permission
                            {
                                if (ExamId == 0)//All Exam 
                                {
                                    #region 
                                    var allExams = _Entities.TbExams.Where(x => x.ClassId == item.ClassId && x.SchoolId == _user.SchoolId && x.IsActive).ToList();
                                    if (allExams != null && allExams.Count > 0)
                                    {
                                        if (SubjectId == 0)//All Subjects
                                        {
                                            #region
                                            foreach (var item0 in allExams)
                                            {
                                                var data = _Entities.spStudentMarkPercentage_Result
                                                      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                                  ExamId)
                                                      .ToList();
                                                foreach (var item1 in allDivisions)
                                                {
                                                    var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == item1.DivisionId && x.ClassId == item.ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                                    var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item0.ExamId && x.IsActive).ToList();
                                                    var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                                    if (input != null && input.Count() > 0)
                                                    {
                                                        foreach (var item2 in input)
                                                        {
                                                            if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                            {
                                                                var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                                SingleCharts one = new SingleCharts();
                                                                one.ChartName = item.Class + "(" + item1.Division + ")" + "-" + item0.ExamName + "(" + item2.Subject + ")";
                                                                var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                                var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == item.ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                                decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                                decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                                decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                                decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                                if (totalStudents != null && totalStudents.Count > 0)
                                                                {
                                                                    one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                                    one.Average = ((Average / totalStudents.Count()) * 360);
                                                                    one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                                    one.Super = ((Super / totalStudents.Count()) * 360);
                                                                }
                                                                else
                                                                {
                                                                    one.NTP = 0;
                                                                    one.Average = 0;
                                                                    one.Merit = 0;
                                                                    one.Super = 0;
                                                                }
                                                                model.list.Add(one);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // NO PERMISSION FOR SUBJECTS 
                                                    }
                                                }

                                            }
                                            #endregion
                                        }
                                        else//Particular Subjects 
                                        {
                                            #region
                                            foreach (var item0 in allExams)
                                            {
                                                var data = _Entities.spStudentMarkPercentage_Result
                                                      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                                  item0.ExamId)
                                                      .ToList();
                                                var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item0.ExamId && x.IsActive && x.SubId == SubjectId).FirstOrDefault();
                                                if (exams != null)
                                                {
                                                    foreach (var item1 in allDivisions)
                                                    {
                                                        SingleCharts one = new SingleCharts();
                                                        one.ChartName = item.Class + "(" + item1.Division + ")" + "-" + item0.ExamName + "(" + exams.Subject + ")";
                                                        var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == exams.SubId).ToList();
                                                        var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == item.ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                        decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                        decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                        decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                        decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                        if (totalStudents != null && totalStudents.Count > 0)
                                                        {
                                                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                            one.Average = ((Average / totalStudents.Count()) * 360);
                                                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                            one.Super = ((Super / totalStudents.Count()) * 360);
                                                        }
                                                        else
                                                        {
                                                            one.NTP = 0;
                                                            one.Average = 0;
                                                            one.Merit = 0;
                                                            one.Super = 0;
                                                        }
                                                        model.list.Add(one);
                                                    }
                                                }
                                                else
                                                {
                                                    //NO THIS SUBJECT EXAM
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        //NO EXAM DECLARED IN THIS CLASS
                                    }
                                    #endregion
                                }
                                else// Particular Exam  
                                {
                                    #region
                                    var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  ExamId)
      .ToList();

                                    if (SubjectId == 0)//All Subjects
                                    {
                                        #region
                                        foreach (var item1 in allDivisions)
                                        {
                                            var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == item1.DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                            var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == ExamId && x.IsActive).ToList();
                                            var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                            if (input != null && input.Count() > 0)
                                            {
                                                foreach (var item2 in input)
                                                {
                                                    var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                    SingleCharts one = new SingleCharts();
                                                    one.ChartName = item.Class + "(" + item1.Division + ") - " + exams.FirstOrDefault().Exam.ExamName + "(" + item2.Subject + ")";
                                                    var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                    var totalStudents = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == item.ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                    decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                    decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                    decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                    decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                    if (totalStudents.Count > 0 && totalStudents != null)
                                                    {
                                                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                        one.Average = ((Average / totalStudents.Count()) * 360);
                                                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                        one.Super = ((Super / totalStudents.Count()) * 360);
                                                    }
                                                    else
                                                    {
                                                        one.NTP = 0;
                                                        one.Average = 0;
                                                        one.Merit = 0;
                                                        one.Super = 0;
                                                    }
                                                    model.list.Add(one);
                                                }
                                            }
                                            else
                                            {
                                                // NO PERMISSION FOR THIS SUBJECTS
                                            }
                                        }
                                        #endregion
                                    }
                                    else//Particular Subjects
                                    {
                                        #region
                                        var SubjectName = _Entities.TbSubjects.Where(x => x.SubId == SubjectId && x.IsActive).FirstOrDefault();
                                        var ExamName = _Entities.TbExams.Where(x => x.ExamId == ExamId && x.IsActive).FirstOrDefault();
                                        foreach (var item0 in allDivisions)
                                        {
                                            var subId = _Entities.TbExamSubjects.Where(x => x.ExamId == ExamName.ExamId && x.SubjectId == SubjectName.SubId).FirstOrDefault();
                                            SingleCharts one = new SingleCharts();
                                            one.ChartName = item.Class + "(" + item0.Division + ") - " + ExamName.ExamName + "(" + SubjectName.SubjectName + ")";
                                            var markData = data.Where(x => x.SubjectId == subId.SubId && x.DivisionId == item0.DivisionId).ToList();
                                            var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == item.ClassId && x.DivisionId == item0.DivisionId && x.IsActive == true).ToList();

                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                            if (totalStudents.Count > 0 && totalStudents != null)
                                            {
                                                one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                one.Average = ((Average / totalStudents.Count()) * 360);
                                                one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                one.Super = ((Super / totalStudents.Count()) * 360);
                                            }
                                            else
                                            {
                                                one.NTP = 0;
                                                one.Average = 0;
                                                one.Merit = 0;
                                                one.Super = 0;
                                            }
                                            model.list.Add(one);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                // NO PERMISSION FOR DIVISION
                            }
                        }
                        #endregion
                    }
                    else // Particular Division
                    {
                        #region
                        var classData = _Entities.TbDivisions.Where(x => x.DivisionId == DivisionId && x.IsActive).FirstOrDefault();
                        if (ExamId == 0)//All Exams
                        {
                            var allExams = _Entities.TbExams.Where(x => x.ClassId == classData.ClassId && x.SchoolId == _user.SchoolId && x.IsActive).ToList();
                            if (allExams != null && allExams.Count > 0)
                            {
                                foreach (var item0 in allExams)
                                {

                                    if (SubjectId == 0)//All Subjects
                                    {
                                        var data = _Entities.spStudentMarkPercentage_Result
                                              .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                          item0.ExamId)
                                              .ToList();
                                        var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                        var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item0.ExamId && x.IsActive).ToList();
                                        var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                        #region
                                        foreach (var item1 in input)
                                        {
                                            var subId = exams.Where(x => x.SubjectId == item1.SubjectId).FirstOrDefault();
                                            SingleCharts one = new SingleCharts();
                                            one.ChartName = classData.Class.Class + "(" + classData.Division + ") - " + item0.ExamName + "(" + item1.Subject + ")";
                                            var markData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == subId.SubId).ToList();
                                            var totalStudent = _Entities.TbStudents.Where(x => x.ClassId == classData.ClassId && x.DivisionId == DivisionId && x.IsActive).ToList();
                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();

                                            if (totalStudent.Count > 0 && totalStudent != null)
                                            {
                                                one.NTP = ((NTP / totalStudent.Count()) * 360);
                                                one.Average = ((Average / totalStudent.Count()) * 360);
                                                one.Merit = ((Merit / totalStudent.Count()) * 360);
                                                one.Super = ((Super / totalStudent.Count()) * 360);
                                            }
                                            else
                                            {
                                                one.NTP = 0;
                                                one.Average = 0;
                                                one.Merit = 0;
                                                one.Super = 0;
                                            }
                                            model.list.Add(one);
                                        }
                                        #endregion
                                    }
                                    else//Particular Subjects 
                                    {
                                        #region
                                        var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  item0.ExamId)
      .ToList();

                                        var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                        var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item0.ExamId && x.IsActive && x.SubId == SubjectId).ToList();
                                        var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                        var subject = input.Where(x => x.SubjectId == SubjectId).FirstOrDefault();
                                        if (subject != null)
                                        {
                                            var subId = _Entities.TbExamSubjects.Where(x => x.SubjectId == subject.SubjectId && x.ExamId == item0.ExamId).FirstOrDefault();
                                            SingleCharts one = new SingleCharts();
                                            one.ChartName = classData.Class.Class + "(" + classData.Division + ") - " + item0.ExamName + "(" + subject.Subject + ")";
                                            var markData = data.Where(x => x.DivisionId == DivisionId && x.SubjectId == subId.SubId).ToList();
                                            var totalStudent = _Entities.TbStudents.Where(x => x.ClassId == classData.ClassId && x.DivisionId == DivisionId && x.IsActive).ToList();
                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();

                                            if (totalStudent.Count > 0 && totalStudent != null)
                                            {
                                                one.NTP = ((NTP / totalStudent.Count()) * 360);
                                                one.Average = ((Average / totalStudent.Count()) * 360);
                                                one.Merit = ((Merit / totalStudent.Count()) * 360);
                                                one.Super = ((Super / totalStudent.Count()) * 360);
                                            }
                                            else
                                            {
                                                one.NTP = 0;
                                                one.Average = 0;
                                                one.Merit = 0;
                                                one.Super = 0;
                                            }
                                            model.list.Add(one);
                                        }
                                        else
                                        {
                                            // NO PERMISSION FOR SUBJECTS IN THIS EXAMS 
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                // NO EXAMS IN THIS CLASS AND DIVISON
                            }
                        }
                        else//Particular Exam
                        {
                            var exam = _Entities.TbExams.Where(x => x.ClassId == classData.ClassId && x.SchoolId == _user.SchoolId && x.IsActive && x.ExamId == ExamId).FirstOrDefault();
                            if (SubjectId == 0)// All Subjects
                            {

                            }
                            else// Particular Subjects 
                            {

                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else// PARTICULAR CLASS **************************************************************************************************************
                {
                    #region
                    var allclasss = FullData.Where(x => x.SchoolId == _user.SchoolId && x.TeacherId == TeacherId && x.IsActive && x.ClassId == ClassId).OrderBy(x => x.Class.ClassOrder).ToList().Select(x => new { x.ClassId, x.Class.Class }).Distinct().FirstOrDefault();
                    if (DivisionId == 0)//All Divisions
                    {
                        #region
                        var allDivisions = FullData.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => new { x.DivisionId, x.Division.Division }).Distinct().ToList();
                        if (allDivisions != null && allDivisions.Count > 0)
                        {
                            if (ExamId == 0)//All exams
                            {
                                #region
                                var allExams = _Entities.TbExams.Where(x => x.ClassId == ClassId && x.SchoolId == _user.SchoolId && x.IsActive).ToList();
                                if (allExams != null && allExams.Count > 0)
                                {
                                    if (SubjectId == 0)// All Subjects
                                    {
                                        foreach (var item in allExams)
                                        {
                                            foreach (var item1 in allDivisions)
                                            {
                                                var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  item.ExamId)
      .ToList();

                                                var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                                var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item.ExamId && x.IsActive).ToList();
                                                var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                                if (input != null && input.Count() > 0)
                                                {
                                                    foreach (var item2 in input)
                                                    {
                                                        if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                        {
                                                            #region 
                                                            var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                            SingleCharts one = new SingleCharts();
                                                            one.ChartName = allclasss.Class + "(" + item1.Division + ")" + "-" + item.ExamName + "(" + item2.Subject + ")";
                                                            var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                            var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                            if (totalStudents != null && totalStudents.Count > 0)
                                                            {
                                                                one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                                one.Average = ((Average / totalStudents.Count()) * 360);
                                                                one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                                one.Super = ((Super / totalStudents.Count()) * 360);
                                                            }
                                                            else
                                                            {
                                                                one.NTP = 0;
                                                                one.Average = 0;
                                                                one.Merit = 0;
                                                                one.Super = 0;
                                                            }
                                                            model.list.Add(one);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // NO PERMISSION FOR SUBJECTS 
                                                }
                                            }
                                        }
                                    }
                                    else // Particular Subjects 
                                    {
                                        foreach (var item in allExams)
                                        {
                                            foreach (var item1 in allDivisions)
                                            {
                                                var data = _Entities.spStudentMarkPercentage_Result
                                                      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                                  item.ExamId)
                                                      .ToList();
                                                var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive && x.SubjectId == SubjectId).ToList().Select(x => x.SubjectId).ToList();
                                                var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item.ExamId && x.IsActive && x.SubId == SubjectId).ToList();
                                                var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                                if (input != null && input.Count() > 0)
                                                {
                                                    foreach (var item2 in input)
                                                    {
                                                        if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                        {
                                                            #region
                                                            var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                            SingleCharts one = new SingleCharts();
                                                            one.ChartName = allclasss.Class + "(" + item1.Division + ")" + "-" + item.ExamName + "(" + item2.Subject + ")";
                                                            var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                            var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                            if (totalStudents != null && totalStudents.Count > 0)
                                                            {
                                                                one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                                one.Average = ((Average / totalStudents.Count()) * 360);
                                                                one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                                one.Super = ((Super / totalStudents.Count()) * 360);
                                                            }
                                                            else
                                                            {
                                                                one.NTP = 0;
                                                                one.Average = 0;
                                                                one.Merit = 0;
                                                                one.Super = 0;
                                                            }
                                                            model.list.Add(one);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // NO PERMISSION FOR SUBJECTS 
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // NO EXAMS DECLARED 
                                }
                                #endregion
                            }
                            else// Particular Exam
                            {
                                #region
                                var allExams = _Entities.TbExams.Where(x => x.ClassId == ClassId && x.SchoolId == _user.SchoolId && x.IsActive && x.ExamId == ExamId).FirstOrDefault();
                                if (allExams != null)
                                {
                                    if (SubjectId == 0)//All Subjects 
                                    {
                                        foreach (var item1 in allDivisions)
                                        {
                                            var data = _Entities.spStudentMarkPercentage_Result
                                                  .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                              allExams.ExamId)
                                                  .ToList();
                                            var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                            var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == allExams.ExamId && x.IsActive).ToList();
                                            var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                            if (input != null && input.Count() > 0)
                                            {
                                                foreach (var item2 in input)
                                                {
                                                    if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                    {
                                                        #region
                                                        var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                        SingleCharts one = new SingleCharts();
                                                        one.ChartName = allclasss.Class + "(" + item1.Division + ")" + "-" + allExams.ExamName + "(" + item2.Subject + ")";
                                                        var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                        var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                        decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                        decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                        decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                        decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                        if (totalStudents != null && totalStudents.Count > 0)
                                                        {
                                                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                            one.Average = ((Average / totalStudents.Count()) * 360);
                                                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                            one.Super = ((Super / totalStudents.Count()) * 360);
                                                        }
                                                        else
                                                        {
                                                            one.NTP = 0;
                                                            one.Average = 0;
                                                            one.Merit = 0;
                                                            one.Super = 0;
                                                        }
                                                        model.list.Add(one);
                                                        #endregion
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // NO SUBJECTS ALLOWDED
                                            }
                                        }
                                    }
                                    else //Particular Subjects 
                                    {
                                        foreach (var item1 in allDivisions)
                                        {
                                            var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  allExams.ExamId)
      .ToList();

                                            var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == item1.DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                            var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == allExams.ExamId && x.IsActive && x.SubId == SubjectId).ToList();
                                            var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                            if (input != null && input.Count() > 0)
                                            {
                                                foreach (var item2 in input)
                                                {
                                                    if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                    {
                                                        #region
                                                        var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                        SingleCharts one = new SingleCharts();
                                                        one.ChartName = allclasss.Class + "(" + item1.Division + ")" + "-" + allExams.ExamName + "(" + item2.Subject + ")";
                                                        var markData = data.Where(x => x.DivisionId == item1.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                        var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == item1.DivisionId && x.IsActive).ToList();
                                                        decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                        decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                        decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                        decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                        if (totalStudents != null && totalStudents.Count > 0)
                                                        {
                                                            one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                            one.Average = ((Average / totalStudents.Count()) * 360);
                                                            one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                            one.Super = ((Super / totalStudents.Count()) * 360);
                                                        }
                                                        else
                                                        {
                                                            one.NTP = 0;
                                                            one.Average = 0;
                                                            one.Merit = 0;
                                                            one.Super = 0;
                                                        }
                                                        model.list.Add(one);
                                                        #endregion
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // NO SUBJECTS ALLOWDED
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // NO EXAMS DECLARED
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            // NO DIVISIONS 
                        }
                        #endregion
                    }
                    else // Particular Divisions
                    {
                        #region
                        var allDivisions = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => new { x.DivisionId, x.Division.Division }).Distinct().FirstOrDefault();
                        if (ExamId == 0) // All Exams 
                        {
                            #region
                            var allExams = _Entities.TbExams.Where(x => x.ClassId == ClassId && x.SchoolId == _user.SchoolId && x.IsActive).ToList();
                            if (allExams != null && allExams.Count > 0)
                            {
                                foreach (var item in allExams)
                                {
                                    if (SubjectId == 0) // All Subjects 
                                    {
                                        var data = _Entities.spStudentMarkPercentage_Result
                                              .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                                                          item.ExamId)
                                              .ToList();
                                        var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                        var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item.ExamId && x.IsActive).ToList();
                                        var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                        if (input != null && input.Count() > 0)
                                        {
                                            foreach (var item2 in input)
                                            {
                                                if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                {
                                                    #region
                                                    var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                    SingleCharts one = new SingleCharts();
                                                    one.ChartName = allclasss.Class + "(" + allDivisions.Division + ")" + "-" + item.ExamName + "(" + item2.Subject + ")";
                                                    var markData = data.Where(x => x.DivisionId == allDivisions.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                    var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == allDivisions.DivisionId && x.IsActive).ToList();
                                                    decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                    decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                    decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                    decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                    if (totalStudents != null && totalStudents.Count > 0)
                                                    {
                                                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                        one.Average = ((Average / totalStudents.Count()) * 360);
                                                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                        one.Super = ((Super / totalStudents.Count()) * 360);
                                                    }
                                                    else
                                                    {
                                                        one.NTP = 0;
                                                        one.Average = 0;
                                                        one.Merit = 0;
                                                        one.Super = 0;
                                                    }
                                                    model.list.Add(one);
                                                    #endregion
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // NO SUBJECTS 
                                        }
                                    }
                                    else // Particular Subjects 
                                    {
                                        var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                  ExamId)
      .ToList();

                                        var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                        var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == item.ExamId && x.IsActive && x.SubId == SubjectId).ToList();
                                        var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                        if (input != null && input.Count() > 0)
                                        {
                                            foreach (var item2 in input)
                                            {
                                                if (exams.Any(x => x.SubjectId == item2.SubjectId))
                                                {
                                                    #region
                                                    var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                                    SingleCharts one = new SingleCharts();
                                                    one.ChartName = allclasss.Class + "(" + allDivisions.Division + ")" + "-" + item.ExamName + "(" + item2.Subject + ")";
                                                    var markData = data.Where(x => x.DivisionId == allDivisions.DivisionId && x.SubjectId == subId.SubId).ToList();
                                                    var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == allDivisions.DivisionId && x.IsActive).ToList();
                                                    decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                                    decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                                    decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                                    decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                                    if (totalStudents != null && totalStudents.Count > 0)
                                                    {
                                                        one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                        one.Average = ((Average / totalStudents.Count()) * 360);
                                                        one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                        one.Super = ((Super / totalStudents.Count()) * 360);
                                                    }
                                                    else
                                                    {
                                                        one.NTP = 0;
                                                        one.Average = 0;
                                                        one.Merit = 0;
                                                        one.Super = 0;
                                                    }
                                                    model.list.Add(one);
                                                    #endregion
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // NO SUBJECTS 
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // NO EXAMS DECLARED 
                            }
                            #endregion
                        }
                        else // Particular Exams
                        {
                            #region
                            var allExams = _Entities.TbExams.Where(x => x.ClassId == ClassId && x.SchoolId == _user.SchoolId && x.IsActive && x.ExamId == ExamId).FirstOrDefault();
                            if (allExams != null)
                            {
                                if (SubjectId == 0)// All subjects 
                                {
                                    var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                 allExams.ExamId)
      .ToList();

                                    var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                    var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == allExams.ExamId && x.IsActive).ToList();
                                    var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                    if (input != null && input.Count() > 0)
                                    {
                                        foreach (var item2 in input)
                                        {
                                            #region
                                            var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                            SingleCharts one = new SingleCharts();
                                            one.ChartName = allclasss.Class + "(" + allDivisions.Division + ")" + "-" + allExams.ExamName + "(" + item2.Subject + ")";
                                            var markData = data.Where(x => x.DivisionId == allDivisions.DivisionId && x.SubjectId == subId.SubId).ToList();
                                            var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == allDivisions.DivisionId && x.IsActive).ToList();
                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                            if (totalStudents != null && totalStudents.Count > 0)
                                            {
                                                one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                one.Average = ((Average / totalStudents.Count()) * 360);
                                                one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                one.Super = ((Super / totalStudents.Count()) * 360);
                                            }
                                            else
                                            {
                                                one.NTP = 0;
                                                one.Average = 0;
                                                one.Merit = 0;
                                                one.Super = 0;
                                            }
                                            model.list.Add(one);
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        // NO SUBJECTS 
                                    }
                                }
                                else // Particular Subjects 
                                {
                                    var data = _Entities.spStudentMarkPercentage_Result
      .FromSqlRaw("EXEC sp_StudentMarkPercentage {0}",
                 allExams.ExamId)
      .ToList();

                                    var alloded = FullData.Where(x => x.SchoolId == _user.SchoolId && x.DivisionId == DivisionId && x.ClassId == ClassId && x.TeacherId == TeacherId && x.IsActive).ToList().Select(x => x.SubjectId).ToList();
                                    var exams = _Entities.TbExamSubjects.Where(x => x.ExamId == allExams.ExamId && x.IsActive && x.SubId == SubjectId).ToList();
                                    var input = from a in exams where (from o in alloded select o).Contains(a.SubjectId) select a;
                                    if (input != null && input.Count() > 0)
                                    {
                                        foreach (var item2 in input)
                                        {
                                            #region
                                            var subId = exams.Where(x => x.SubjectId == item2.SubjectId).FirstOrDefault();
                                            SingleCharts one = new SingleCharts();
                                            one.ChartName = allclasss.Class + "(" + allDivisions.Division + ")" + "-" + allExams.ExamName + "(" + item2.Subject + ")";
                                            var markData = data.Where(x => x.DivisionId == allDivisions.DivisionId && x.SubjectId == subId.SubId).ToList();
                                            var totalStudents = _Entities.TbStudents.Where(x => x.ClassId == ClassId && x.DivisionId == allDivisions.DivisionId && x.IsActive).ToList();
                                            decimal NTP = markData.Where(x => x.Percentage < 41).Count();
                                            decimal Average = markData.Where(x => x.Percentage > 42 && x.Percentage < 71).Count();
                                            decimal Merit = markData.Where(x => x.Percentage > 72 && x.Percentage < 91).Count();
                                            decimal Super = markData.Where(x => x.Percentage > 92).Count();
                                            if (totalStudents != null && totalStudents.Count > 0)
                                            {
                                                one.NTP = ((NTP / totalStudents.Count()) * 360);
                                                one.Average = ((Average / totalStudents.Count()) * 360);
                                                one.Merit = ((Merit / totalStudents.Count()) * 360);
                                                one.Super = ((Super / totalStudents.Count()) * 360);
                                            }
                                            else
                                            {
                                                one.NTP = 0;
                                                one.Average = 0;
                                                one.Merit = 0;
                                                one.Super = 0;
                                            }
                                            model.list.Add(one);
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        //NO SUBJECTS 
                                    }
                                }
                            }
                            else
                            {
                                // NO EXAMS DECLARED 
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
            }

            model.fullList = JsonConvert.SerializeObject(model.list);
            return PartialView("~/Views/Performance/_pv_ChartPartialView.cshtml", model);
        }
        public object DeleteAllTeachersAndParticularSubject(PerformanceModel model)
        {
            bool status = true;
            string msg = "Success";
            var data = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == model.TeacherId && x.SubjectId == model.SubjectId && x.IsActive == true).ToList();
            foreach (var item in data)
            {
                var deleteItem = _Entities.TbTeacherClassSubjects.Where(x => x.Id == item.Id).FirstOrDefault();
                deleteItem.IsActive = false;
                _Entities.SaveChanges();
            }
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg }, JsonRequestBehavior.AllowGet);
        }
    }
}
