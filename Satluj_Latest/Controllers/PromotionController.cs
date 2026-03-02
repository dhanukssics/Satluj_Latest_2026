using Microsoft.AspNetCore.Mvc;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Satluj_Latest.Models.SchoolValue;
using PromoteStudents = Satluj_Latest.Models.PromoteStudents;
using UpPublishedClassList = Satluj_Latest.Models.UpPublishedClassList;

namespace Satluj_Latest.Controllers
{

    //Sathluj
    public class PromotionController : BaseController
    {
        public PromotionController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        // GET: Promotion
        public ActionResult PromoteClassHome()
        {
            SchoolValue model = new SchoolValue();
            model.schoolId = _user.SchoolId;
            DropdownData dropdown = new DropdownData();
            model.UnPublishedClasses = dropdown.GetUnPublishedClasses(model.schoolId);
            return View(model);
        }
        public PartialViewResult UnpublishedClasses(string id)
        {
            long Classid = Convert.ToInt64(id);
            int slNo = 0;
            UpPublishedClassList model = new UpPublishedClassList();
            model.list = new List<UpPublishedClassItem>();
            if (Classid == 0)
            {
                var classList = _Entities.TbDivisions.Where(x => x.Class.SchoolId == _user.SchoolId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus == false).ToList();
                if (classList.Count > 0 && classList != null)
                {
                    foreach (var item in classList)
                    {
                        UpPublishedClassItem one = new UpPublishedClassItem();
                        one.ClassId = item.ClassId;
                        one.ClassName = item.Class.Class;
                        one.DivisionId = item.DivisionId;
                        one.DivisionName = item.Division;
                        one.AccademicYear = item.Class.AcademicYear.AcademicYear;
                        one.CurrentYearStatus = item.Class.AcademicYear.CurrentYear ?? false;
                        one.SlNo = slNo + 1;
                        model.list.Add(one);
                        slNo = slNo + 1;
                    }
                }
            }
            else
            {
                var classList = _Entities.TbDivisions.Where(x => x.Class.SchoolId == _user.SchoolId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus == false && x.Class.ClassId == Classid).ToList();
                if (classList.Count > 0 && classList != null)
                {
                    foreach (var item in classList)
                    {
                        UpPublishedClassItem one = new UpPublishedClassItem();
                        one.ClassId = item.ClassId;
                        one.ClassName = item.Class.Class;
                        one.DivisionId = item.DivisionId;
                        one.DivisionName = item.Division;
                        one.AccademicYear = item.Class.AcademicYear.AcademicYear;
                        one.CurrentYearStatus = item.Class.AcademicYear.CurrentYear ?? false;
                        one.SlNo = slNo + 1;
                        model.list.Add(one);
                        slNo = slNo + 1;
                    }
                }
            }

            return PartialView("~/Views/Promotion/_pv_UnPublishedClassList.cshtml", model);
        }
        public PartialViewResult AddUnPublishedClassParialView()
        {
            AddClassModel model = new AddClassModel();
            model.SchoolId = _user.SchoolId;
            DropdownData dropdown = new DropdownData();
            model.ClassList = dropdown.GetClassAllList();
            model.currentacademiclist = dropdown.GetCurrentAcademicYear();
            return PartialView("~/Views/Promotion/_pv_AddClassUnPublished.cshtml", model);
        }
        public object AddClassUnPublished(AddClassModel model)
        {
            bool status = false;
            string msg = "Failed";
            long ClassId = 0;
            var div = _Entities.TbDivisions.Where(x => x.Class.Class == model.ClassName && x.Division == model.Division && x.Class.AcademicYearId == model.AcademicYearId && x.IsActive && x.Class.IsActive && x.Class.SchoolId == _user.SchoolId).FirstOrDefault();
            if (div != null)
            {
                msg = "Division and Class is exists in this Academic Year!";
            }
            else
            {
                var classOld = _Entities.TbClasses.Where(x => x.Class == model.ClassName && x.SchoolId == _user.SchoolId && x.IsActive && x.AcademicYearId == model.AcademicYearId).FirstOrDefault();
                if (classOld != null)
                {
                    var newDiv = new TbDivision();
                    newDiv.ClassId = classOld.ClassId;
                    newDiv.Division = model.Division.ToUpper();
                    newDiv.DivisionGuid = Guid.NewGuid();
                    newDiv.IsActive = true;
                    newDiv.TimeStamp = CurrentTime;
                    _Entities.TbDivisions.Add(newDiv);
                    status = _Entities.SaveChanges() > 0;
                    msg = status ? "success" : "failed";
                    ClassId = classOld.ClassId;
                }
                else
                {
                    var newClass = new  TbClass();
                    newClass.Class = model.ClassName;
                    newClass.ClassGuild = Guid.NewGuid();
                    newClass.Timestamp = CurrentTime;
                    newClass.SchoolId = model.SchoolId;
                    newClass.IsActive = true;
                    //newClass.ClassOrder = model.OrderValue;
                    newClass.ClassOrder = _Entities.TbClassLists.Where(x => x.ClassName == model.ClassName).Select(x => x.OrderValue).FirstOrDefault();
                    newClass.AcademicYearId = model.AcademicYearId;
                    newClass.PublishStatus = false;
                    _Entities.TbClasses.Add(newClass);

                    var newDiv = new TbDivision();
                    newDiv.ClassId = newClass.ClassId;
                    newDiv.Division = model.Division.ToUpper();
                    newDiv.DivisionGuid = Guid.NewGuid();
                    newDiv.IsActive = true;
                    newDiv.TimeStamp = CurrentTime;
                    _Entities.TbDivisions.Add(newDiv);

                    status = _Entities.SaveChanges() > 0;
                    msg = status ? "success" : "failed";
                    ClassId = newClass.ClassId;
                }
            }
            return Json(new { status = status, msg = msg, classId = ClassId, list = new Data.DropdownData().RefreshClasses(model.SchoolId) });
        }
        public object PublishUnPublishedClass(string id)
        {
            bool status = false;
            string message = "Failed";
            long DivisionId = Convert.ToInt64(id);
            var Division = _Entities.TbDivisions.FirstOrDefault(z => z.DivisionId == DivisionId);
            if (Division != null)
            {
                var Class = _Entities.TbClasses.FirstOrDefault(z => z.ClassId == Division.ClassId);
                Class.PublishStatus = true;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    var prevClass = _Entities.TbClasses.Where(z => z.Class == Class.Class && z.AcademicYearId != Class.AcademicYearId && z.SchoolId == _user.SchoolId && z.IsActive).ToList();
                    {
                        foreach (var data in prevClass)
                        {
                            data.PublishStatus = false;
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                }
            }
            message = status ? "Published" : "failed";
            return Json(new { status = status, msg = message, list = new Data.DropdownData().RefreshClassesUnPublished(_user.SchoolId) });
        }
        public ActionResult PromoteStudents()
        {
            SchoolValue model = new SchoolValue();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public ActionResult StudentPromotionHome()
        {
            Models.PromoteStudents model = new Models.PromoteStudents();
            model.SchoolId = _user.SchoolId;
            DropdownData dropdown = new DropdownData();
            model.AcademicYearList = dropdown.GetOtherAcademicYear();
            return View(model);
        }
        public PartialViewResult StudentListForPromotion(string id)
        {
            string[] splitData = id.Split('~');
            long AcademicYearId = Convert.ToInt64(splitData[0]);
            long ClassId = Convert.ToInt64(splitData[1]);
            long DivisionId = Convert.ToInt64(splitData[2]);
            int count = 0;
            var studentList = _Entities.TbStudents.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.ClassId == ClassId && x.DivisionId == DivisionId).ToList();
            PromoteStudents model = new Models.PromoteStudents();
            model.OldAcademicyearId = AcademicYearId;
            model.OldClassId = ClassId;
            model.OldDivId = DivisionId;
            model.SchoolId = _user.SchoolId;
            model.StudentList = new List<StudentListForPromote>();
            foreach (var item in studentList)
            {
                StudentListForPromote one = new StudentListForPromote();
                one.StudentId = item.StudentId;
                one.StudentName = item.StundentName;
                one.SpecialId = item.StudentSpecialId;
                one.ContactNumber = item.ContactNumber;
                one.SlNo = count + 1;
                model.StudentList.Add(one);
                count = count + 1;
            }
            return PartialView("~/Views/Promotion/_pv_StudentListForPromotion.cshtml", model);
        }
        public PartialViewResult PromoteStudentsToAnotherParialView(Models.PromoteStudents model)
        {
            model.SchoolId = _user.SchoolId;
                DropdownData dropdown = new DropdownData();
            ViewBag.OtherAcademicYears = dropdown.GetOtherAcademicYear();
            foreach (var item in model.StudentList)
            {
                if (model.StudentListString == null)
                    model.StudentListString = item.StudentId.ToString();
                else
                    model.StudentListString = model.StudentListString + ',' + item.StudentId.ToString();

            }
            return PartialView("~/Views/Promotion/_pv_PromoteStudentsToAnotherParialView.cshtml", model);
        }

        public object AddPromoteStudents(Models.PromoteStudents model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                string[] listOfStudents = model.StudentListString.Split(',');
                foreach (var item in listOfStudents)
                {
                    var studentId = Convert.ToInt64(item);
                    var student = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
                    var oldPromotion = _Entities.TbStudentPremotions.Where(x => x.StudentId == studentId && x.IsActive == 1).ToList();
                    foreach (var item1 in oldPromotion)
                    {
                        item1.LastUpdate = false;
                        _Entities.SaveChanges();
                    }
                    var newPromotion = new TbStudentPremotion();
                    newPromotion.StudentId = student.StudentId;
                    newPromotion.FromDivision = student.DivisionId;
                    newPromotion.ToDivision = model.NewDivId;
                    newPromotion.TimeStamp = CurrentTime;
                    newPromotion.SchoolId = _user.SchoolId;
                    newPromotion.IsActive = 1;
                    newPromotion.LastUpdate = true;
                    newPromotion.OldClass = student.ClassId;
                    _Entities.TbStudentPremotions.Add(newPromotion);
                    status = _Entities.SaveChanges() > 0;


                    student.ClassId = model.NewCLassId;
                    student.DivisionId = model.NewDivId;
                    _Entities.SaveChanges();
                }
                if (status)
                    msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }

        public object DemoteStudents(Models.PromoteStudents model)
        {
            bool status = false;
            string msg = "Failed";
            int totalCount = 0;
            try
            {
                foreach (var item in model.StudentList)
                {
                    var oldPromotionDetails = _Entities.TbStudentPremotions.Where(x => x.StudentId == item.StudentId && x.SchoolId == _user.SchoolId && x.LastUpdate == true && x.IsActive == 1).FirstOrDefault();
                    if (oldPromotionDetails != null)
                    {
                        oldPromotionDetails.LastUpdate = false;
                        oldPromotionDetails.TimeStamp = CurrentTime;
                        _Entities.SaveChanges();
                        var student = _Entities.TbStudents.Where(x => x.StudentId == item.StudentId && x.IsActive).FirstOrDefault();
                        student.ClassId = oldPromotionDetails.OldClass ?? 0;
                        student.DivisionId = oldPromotionDetails.FromDivision;
                        status = _Entities.SaveChanges() > 0;
                        if (status)
                            msg = "Successful";
                    }
                    else
                    {
                        var student = _Entities.TbStudents.Where(x => x.StudentId == item.StudentId && x.IsActive).FirstOrDefault();
                        totalCount = totalCount + 1;
                    }
                }
                if (totalCount != 0)
                {
                    status = false;
                    int success = model.StudentList.Count - totalCount;
                    msg = success + " Students has demoted and " + totalCount + " Students has not demoted. Since their previous informations not available .";
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
    }
}