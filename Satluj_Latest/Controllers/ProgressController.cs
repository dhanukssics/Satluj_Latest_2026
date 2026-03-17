using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using static Satluj_Latest.PostModel.TeacherAttendancePostModel;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Filters;
using static Satluj_Latest.Models.SchoolValue;
using StudentExamTotalSccore = Satluj_Latest.Models.SchoolValue.StudentExamTotalSccore;
using RemarkModel = Satluj_Latest.Models.RemarkModel;


namespace Satluj_Latest.Controllers
{
    public class ProgressController : BaseController
    {
        private readonly DropdownData _dropdown;
        // GET: Progress
        public ProgressController(
        SchoolRepository schoolRepository,
        ParentRepository parentRepository,
        TeacherRepository teacherRepository,
        SchoolDbContext context,
        DropdownData dropdown)
        : base(schoolRepository, parentRepository, teacherRepository, context)
        {
            _dropdown = dropdown;
        }
        protected bool IsAdmin { get; private set; }
        //public DropdownData dropdown = new DropdownData();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IsAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            ViewBag.IsAdmin = IsAdmin;

            base.OnActionExecuting(context);
        }

        public IActionResult GenerateCertificate()
        {
            ProgressCardModel model = new ProgressCardModel();
            model.SchoolId = _user.SchoolId;
            model.ProgressCardName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            //var dropdownData = new DropdownData();
            model.RegionList = _dropdown.GetRegion(model.SchoolId);
            return View(model);
        }
        public PartialViewResult GetStudentList(string id)
        {
            string[] splitData = id.Split('~');
            long classId = Convert.ToInt64(splitData[0]);
            long examDeclaredId = Convert.ToInt64(splitData[1]);
            long DivisionId = Convert.ToInt64(splitData[2]); ;
            StudentListForProgressCardModel model = new StudentListForProgressCardModel();
            model.Class = new Class(classId).ClassName;
            model.ClassId = classId;
            var studentLis = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == classId && x.DivisionId == DivisionId && x.IsActive).ToList();
            if (studentLis != null && studentLis.Count > 0)
            {
                model.ExamId = examDeclaredId;
                model.SchoolId = _user.SchoolId;
                model._list = new List<StudentListWithExam>();
                foreach (var item in studentLis)
                {
                    StudentListWithExam one = new StudentListWithExam();
                    one.StudnetId = item.StudentId;
                    one.StudnetName = item.StundentName;
                    //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                    one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                    model._list.Add(one);
                }
            }
            model._list = model._list.OrderBy(x => x.RollNo).ThenBy(x => x.StudnetName).ToList();
            return PartialView("~/Views/Progress/_pv_StudentListForProgressCardGeneration.cshtml", model);
        }
        public IActionResult ProgressCardGeneration(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.StudentId == studentId && x.IsActive).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).OrderBy(x => x.OrderNo).ThenBy(x => x.TimeStamp).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == _user.SchoolId && x.RoleId == 1).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == StudentDetails.ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.SchoolLogo = _user.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = _user.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.ToString();
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == _user.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == _user.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = _user.School.Address;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                        model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == _user.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.ExamId == allExamTerms[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().OrderBy(x => x.SubjectName).ToList();

                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).FirstOrDefault();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            z.TotalScore = Math.Round(data.TotalScore).ToString();
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    var main = scholastics.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    if (main == 0)
                                    {
                                        a.Mark = "0";
                                    }
                                    else
                                    {
                                        a.Mark = Convert.ToString(main);
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            a.Mark = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == sub.SubId).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == sub.SubId).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.SchoolLogo = _user.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = _user.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == _user.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == _user.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + " " + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = _user.School.Address;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    model.Remark2 = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId2).Select(x => x.Remark).FirstOrDefault();//razi
                    if (StudentDetails.Dob != null)
                        model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == _user.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).ToList();

                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).ToList();
                    var term2Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().Union(term2Subjects.Select(x => x.Subject).ToList()).OrderBy(x => x.SubjectName).ToList();

                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).FirstOrDefault();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            z.TotalScore = Math.Round(data.TotalScore).ToString();
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    var main = scholastics.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    if (main == 0)
                                    {
                                        a.Mark = "0";
                                    }
                                    else
                                    {
                                        a.Mark = Convert.ToString(main);
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            a.Mark = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == sub.SubId).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == sub.SubId).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                    }
                    #endregion Term II
                    model.IsOneTerm = false;
                }
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                //Anual 
                return View();
            }
        }
        //region
        public ActionResult Region()
        {
            RegionModel model = new RegionModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.IsAdmin = (_user.RoleId == 1);
            return View(model);
        }
        public object AddRegion(string id)
        {
            bool status = false;
            string msg = "Failed";

            try
            {


                string[] splitData = id.Split('~');
                string regionName = splitData[0];
                string ClassListString = splitData[1];
                List<long> ClassList = ClassListString.Split(',').Select(long.Parse).ToList();
                if (_Entities.TbRegionss.Any(x => x.RegionName == regionName.Trim() && x.SchoolId == _user.SchoolId && x.IsActive))
                {
                    msg = "Region already exists!";
                }
                else
                {
                    var region =new TbRegions();
                    region.RegionName = regionName.Trim().ToUpper();
                    region.SchoolId = _user.SchoolId;
                    region.IsActive = true;
                    region.TimeStamp = CurrentTime;
                    _Entities.TbRegionss.Add(region);
                    status = _Entities.SaveChanges() > 0;
                    if (status == true)
                    {
                        foreach (var item in ClassList)
                        {
                            var regionClass =new TbRegionsClass();
                            regionClass.RegionId = region.Id;
                            regionClass.ClassId = item;
                            regionClass.IsActive = true;
                            regionClass.TimeStamp = CurrentTime;
                            _Entities.TbRegionsClasses.Add(regionClass);
                            
                        }
                        status = _Entities.SaveChanges() > 0;
                        if (status)
                        {
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Can't allocate this classes to this region !";
                        }
                    }
                    else
                    {
                        msg = "Can't create this Region !";
                    }

                }
                return Json(new { status = status, msg = msg });
            }
            catch (Exception ex)
            {
                status = false;
                return Json(new { status = status, msg = ex.Message });

            }


        }

        //Scholastic Area

        public ActionResult ScholasticArea()
        {
            ScholasticAreaModel model = new ScholasticAreaModel();
            model.SchoolId = _user.SchoolId;
            //DropdownData dropdown = new DropdownData();
            model.RegionList = _dropdown.GetRegion(model.SchoolId);
            return View(model);
        }

        public object AddScholasticArea(ScholasticAreaModel model)
        {
            bool status = false;
            string message = "Failed";
            if (_Entities.TbScholasticAreas.Any(x => x.SchoolId == _user.SchoolId && x.ItemName.ToUpper() == model.AreaName.Trim().ToUpper() && x.RegionId == model.RegionId && x.IsActive))
            {
                message = "Already exists !";
            }
            else if (_Entities.TbScholasticAreas.Where(x => x.SchoolId == _user.SchoolId && x.RegionId == model.RegionId && x.IsActive).Count() >= 5) // For IX  Class, wants 5 scholastic area
            {
                message = "No more Scholastic Area should be added to this!";
            }
            else
            {
                var newScholasticArea = new TbScholasticArea();
                newScholasticArea.ItemName = model.AreaName.Trim();
                newScholasticArea.SchoolId = _user.SchoolId;
                newScholasticArea.RegionId = model.RegionId;
                newScholasticArea.TotalScore = model.Score;
                newScholasticArea.IsActive = true;
                newScholasticArea.TimeStamp = CurrentTime;
                newScholasticArea.DividedBy = 1;
                _Entities.TbScholasticAreas.Add(newScholasticArea);
                status = _Entities.SaveChanges() > 0;
                if (status)
                    message = "Successful";
                else
                    message = "error occured !";
            }
            return Json(new { status = status, msg = message });
        }

        //C-Scholastic Area
        public ActionResult CoScholasticArea()
        {
            CoScholasticAreaModel model = new CoScholasticAreaModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Regions =_dropdown.GetRegion(model.SchoolId);

            return View(model);
        }

        public object AddCoScholasticArea(CoScholasticAreaModel model)
        {
            bool status = false;
            string message = "Failed";
            if (_Entities.TbCoScholasticAreas.Any(x => x.SchoolId == _user.SchoolId && x.ItemName.ToUpper() == model.Item.Trim().ToUpper() && x.RegionId == model.RegionId))
            {
                message = "Already exists!";
            }
            else
            {
                var coSArea =new  TbCoScholasticArea();
                coSArea.RegionId = model.RegionId;
                coSArea.SchoolId = _user.SchoolId;
                coSArea.ItemName = model.Item.Trim();
                coSArea.IsActive = true;
                coSArea.TimeStamp = CurrentTime;
                _Entities.TbCoScholasticAreas.Add(coSArea);
                status = _Entities.SaveChanges() > 0;
                if (status)
                    message = "Successful";
                else
                    message = "Error occured !";
            }
            return Json(new { status = status, msg = message });
        }

        //Certificate Name

        public ActionResult CertificateName()
        {
            CertificateNameModel model = new CertificateNameModel();
            model.SchoolId = _user.SchoolId;
            var data = _Entities.TbCertificateNames.Where(x => x.SchoolId == model.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.Name = data.CertificateName;
            }
            return View(model);
        }
        public object AddCertificateName(CertificateNameModel model)
        {
            bool status = false;
            string message = "Failed";
            if (_Entities.TbCertificateNames.Any(x => x.CertificateName.ToUpper() == model.Name.Trim().ToUpper() && x.IsActive && x.SchoolId == _user.SchoolId))
            {
                message = "Already exists!";
            }
            else
            {
                var name =new TbCertificateName();
                name.CertificateName = model.Name.Trim();
                name.SchoolId = _user.SchoolId;
                name.IsActive = true;
                name.TimeStamp = CurrentTime;
                _Entities.TbCertificateNames.Add(name);
                status = _Entities.SaveChanges() > 0;
                if (status)
                    message = "Successful";
                else
                    message = "Error occured !";
            }
            return Json(new { status = status, msg = message });
        }
        public object EditCertificateName(CertificateNameModel model)
        {
            bool status = false;
            string msg = "Failed";
            //if(model.Id!=0)
            //{
            var data = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            data.CertificateName = model.Name.Trim();
            status = _Entities.SaveChanges() > 0;
            if (status)
                msg = "successful";
            else
                msg = "Failed";
            //}
            return Json(new { status = status, msg = msg });
        }
        //Exam Declaration  
        public ActionResult ExamDeclaration()
        {
            ExamDeclarationModel model = new ExamDeclarationModel();
            model.SchoolId = _user.SchoolId;
            model.StartDateString = CurrentTime.ToShortDateString();
            model.EndDateString = CurrentTime.ToShortDateString();
            //DropdownData dropdownData = new DropdownData();
            ViewBag.ClassList = _dropdown.GetClasses(model.SchoolId);

            return View(model);
        }

        public object AddDeclaredExams(ExamDeclarationModel model)
        {
            bool status = false;
            string msg = "Failed";
            bool flag = false;
            //----Date Checking ----------
            try
            {
                if (model.StartDateString != string.Empty && model.StartDateString != null)
                {
                    string[] splitData = model.StartDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.StartDate = Convert.ToDateTime(date);
                }
            }
            catch
            {
                msg = "Invalid Start Date !";
                flag = true;
            }
            try
            {
                if (model.EndDateString != string.Empty && model.EndDateString != null)
                {
                    string[] splitData = model.EndDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.EndDate = Convert.ToDateTime(date);
                }
            }
            catch
            {
                msg = "Invalid Start Date !";
                flag = true;
            }
            //-------------------------------
            if (flag == false)
            {
                if (model.StartDate >= model.EndDate)
                {
                    msg = "Wrong start and end date !!";
                }
                else
                {
                    var book = new TbExamBook();
                    book.ExamName = model.ExamName;
                    book.TermId = model.TermId;
                    book.SchoolId = _user.SchoolId;
                    book.IsActive = true;
                    book.TimeStamp = CurrentTime;
                    _Entities.TbExamBooks.Add(book);
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                    {
                        var exam = new TbDeclaredExam();
                        exam.SchoolId = _user.SchoolId;
                        exam.ClassId = model.ClassId;
                        exam.TermId = model.TermId;
                        exam.ExamId = book.Id;
                        exam.StartDate = model.StartDate;
                        exam.EndDate = model.EndDate;
                        exam.TimeStamp = CurrentTime;
                        exam.IsActive = true;
                        _Entities.TbDeclaredExams.Add(exam);
                        status = _Entities.SaveChanges() > 0;
                        msg = "Successful";
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }

        [HttpGet]
        public ActionResult DeclaredExamSubject(string id)
        {
            DeclaredExamSubjectModel model = new DeclaredExamSubjectModel();
            model.SchoolId = _user.SchoolId;
            model.ExamId = Convert.ToInt64(id);
            var exam = _Entities.TbDeclaredExams.Where(x => x.Id == model.ExamId).FirstOrDefault();
            //-----------------------------------------
            try
            {
                string[] splitData = exam.StartDate.ToShortDateString().Split('/');
                var dd = splitData[1];
                var mm = splitData[0];
                var yy = splitData[2];
                var date = dd + "-" + mm + "-" + yy;
                model.ExamTimeString = date;
            }
            catch
            {
                model.ExamTimeString = exam.StartDate.ToShortDateString();
            }
            //-----------------------------------------
            //var input = _Entities.sp_UnDeclaredExamSubjects(_user.SchoolId, model.ExamId).ToList();
          var input =  _Entities.sp_UnDeclaredExamSubjects
           .FromSqlRaw("EXEC sp_UnDeclaredExamSubjects {0}, {1}",
                       _user.SchoolId, model.ExamId)
           .ToList();

            ViewBag.store = input.Select(x => new SelectListItem { Text = x.SubjectName, Value = x.SubId.ToString() }).ToList();
            return View(model);
        }
        public object AddDeclaredExamsSubject(DeclaredExamSubjectModel model)
        {
            bool status = false;
            string msg = "Failed";
            bool flag = false;
            try
            {
                if (model.ExamTimeString != string.Empty && model.ExamTimeString != null)
                {
                    string[] splitData = model.ExamTimeString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.ExamTime = Convert.ToDateTime(date);
                }
            }
            catch
            {
                msg = "Invalid Date !";
                flag = true;
            }
            if (flag == false)
            {
                var exam = _Entities.TbDeclaredExams.Where(x => x.Id == model.ExamId && x.IsActive).FirstOrDefault();
                if (exam != null)
                {
                    if (model.ExamTime < exam.StartDate)
                    {
                        msg = "The date is not in the exam start and end date time !";
                        flag = true;
                    }
                    else if (model.ExamTime > exam.EndDate)
                    {
                        msg = "The date is not in the exam start and end date time !";
                        flag = true;
                    }
                }
                var sameDayOtherExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == model.ExamId && x.ExamDate.Date == model.ExamTime.Date && x.IsActive).ToList();
                if (sameDayOtherExam != null && sameDayOtherExam.Count > 2)
                {
                    msg = "We can't set more than two exams in a day  !";
                    flag = true;
                }
                if (flag == false)
                {
                    var sub = new TbDeclaredExamSubject();
                    sub.DeclaredExamId = model.ExamId;
                    sub.SubjectId = model.SubjectId;
                    sub.ExamDate = model.ExamTime;
                    sub.TotalScore = model.TotalScore;
                    sub.Remark = model.Remark ?? "";
                    sub.IsActive = true;
                    sub.TimeStamp = CurrentTime;
                    _Entities.TbDeclaredExamSubjects.Add(sub);
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Succesful";
                    else
                        msg = "Error Occured!";
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public ActionResult ExamResult()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            model.UserId = _user.UserId;
           // DropdownData dropdownData = new DropdownData();
            ViewBag.ClassListwise = _dropdown.GetClassesUserWise(model.SchoolId,model.UserId);

            return View(model);
        }
        public PartialViewResult SudentListForEnterResult(string id)
        {
            StudentsDeclaredExamResultModel model = new StudentsDeclaredExamResultModel();
            string[] splitData = id.Split('~');
            long subjectId = Convert.ToInt64(splitData[0]);
            long examId = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            long divisionId = Convert.ToInt64(splitData[3]);

            model.SchoolId = _user.SchoolId;
            model.SubjectId = subjectId;
            model.ExamId = examId;
            model.ClassId = classId;
            model.DivisionId = divisionId;
            model.SchoolId = _user.SchoolId;
            model.ListData = new List<StudentMarkList>();
            var subDetails = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExam.SchoolId == model.SchoolId && x.DeclaredExam.Id == model.ExamId && x.SubjectId == model.SubjectId && x.IsActive).FirstOrDefault();
            model.TotalScore = subDetails.TotalScore;
            bool IsOptionalSubject = _Entities.TbSubjects.Where(x => x.SubId == model.SubjectId && x.SchoolI == _user.SchoolId).Select(x => x.IsOptonal).FirstOrDefault();
            var data = _Entities.TbExamResults.Where(x => x.SchoolId == model.SchoolId && x.SubjectId == subDetails.Id && x.ExamId == model.ExamId && x.Student.ClassId == model.ClassId && x.Student.DivisionId == model.DivisionId && x.Student.IsActive == true && x.Exam.IsActive == true && x.IsActive == true).ToList();
            if (data != null && data.Count > 0)
            {
                model.IsEntered = true;
                var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();
                foreach (var item in data)
                {
                    if (item.Student.StundentName == "Jashank Randev")
                    {
                        string sss = "";
                    }

                    StudentMarkList one = new StudentMarkList();
                    one.StudentName = item.Student.StundentName;
                    one.StudentId = item.StudentId;
                    one.Mark = Convert.ToString(item.StudentScore);
                    one.PracticalScore = item.PracticalScore == null ? "" : Convert.ToString(item.PracticalScore);
                    // one.ROllNo = item.TbStudents.ClasssNumber == null ? 0 : Convert.ToInt32(item.TbStudents.ClasssNumber);
                    one.ROllNo = int.TryParse(item.Student.ClasssNumber, out var rn) ? rn : 0;

                    model.ListData.Add(one);
                }
                //if (studentList.Count < model.ListData.Count)// Check that we have a new student after the exam mark entry 
                //{ Commented 
                    var balanceStudents = studentList.Where(x => !model.ListData.Select(y => y.StudentId).Contains(x.StudentId)).ToList();

                    foreach (var item1 in balanceStudents)
                    {
                        if (IsOptionalSubject)
                        {
                            if (_Entities.TbOptionalSubjectStudents.Any(x => x.StudentId == item1.StudentId && x.SubjectId == model.SubjectId && x.IsActive))
                            {
                                StudentMarkList one = new StudentMarkList();
                                one.StudentId = item1.StudentId;
                                one.StudentName = item1.StundentName;
                                one.Mark = "";
                                one.PracticalScore = string.Empty;
                                //one.ROllNo = item1.ClasssNumber == null ? 0 : Convert.ToInt32(item1.ClasssNumber);
                                one.ROllNo = int.TryParse(item1.ClasssNumber, out var rn) ? rn : 0;

                            model.ListData.Add(one);
                            }
                        }
                        else
                        {
                            StudentMarkList one = new StudentMarkList();
                            one.StudentId = item1.StudentId;
                            one.StudentName = item1.StundentName;
                            one.Mark = "";
                            one.PracticalScore = String.Empty;
                            //one.ROllNo = item1.ClasssNumber == null ? 0 : Convert.ToInt32(item1.ClasssNumber);
                            one.ROllNo = int.TryParse(item1.ClasssNumber, out var rn) ? rn : 0;
                            model.ListData.Add(one);
                        }

                    //}
                }
            }
            else
            {
                model.IsEntered = false;
                var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();
                foreach (var item in studentList)
                {
                    if (IsOptionalSubject)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.StudentId == item.StudentId && x.SubjectId == model.SubjectId && x.IsActive))
                        {
                            StudentMarkList one = new StudentMarkList();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one.Mark = "";
                            one.PracticalScore = string.Empty;
                            //one.ROllNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            one.ROllNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                            model.ListData.Add(one);
                        }
                    }
                    else
                    {
                        StudentMarkList one = new StudentMarkList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one.Mark = "";
                        one.PracticalScore = string.Empty;
                        //one.ROllNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.ROllNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        model.ListData.Add(one);
                    }
                }
            }

            //model.ListData.OrderBy(x => x.StudentName).ToList();
            //if (model.SubjectId == 10050 || model.SubjectId == 20076) // These are the Subject Id of Computer Science of Test School and the Real Satluj School 
            //{//10050 is Computer Science of Test School and 20076 is Computer Science of Satluj School 
            //    model.IsComputerScience = true;
            //}
            //else
            //    model.IsComputerScience = false;

            if (_Entities.TbSubjects.Any(x => x.HavePractical == true && x.SubId == model.SubjectId))
            {
                model.IsComputerScience = true;
                model.IsDance = true;
                model.IsLegalStudies = true;
            }
            else
                model.IsComputerScience = false;
                model.IsDance = false;
                model.IsLegalStudies = false;
            model.ListData = model.ListData.OrderBy(x => x.ROllNo).ThenBy(x => x.StudentName).ToList();

            List<StudentMarkList> DuplicateRemove_List = new List<StudentMarkList>();
            foreach (var a1 in model.ListData)
            {
                StudentMarkList mo1 = new StudentMarkList();
                mo1.StudentName = a1.StudentName;
                mo1.Mark = a1.Mark;
                mo1.PracticalScore = a1.PracticalScore;
                mo1.StudentId = a1.StudentId;
                mo1.ROllNo = a1.ROllNo;
                var chek = DuplicateRemove_List.Where(x => x.StudentId == a1.StudentId).FirstOrDefault();
                if (chek == null)
                {
                    DuplicateRemove_List.Add(mo1);
                }
            }

            model.ListData = DuplicateRemove_List;


            return PartialView("~/Views/Progress/_pv_Student_DeclaredExam_Mark.cshtml", model);
        }

        [HttpPost]
        public object SubmitAddStudentsDeclaredMarks(StudentsDeclaredExamResultModel model)
        {
            bool status = false;
            string msg = "Failed";
            var declaredExamSubject = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == model.ExamId && x.SubjectId == model.SubjectId && x.IsActive).FirstOrDefault();
            //var maxGivenMark = model.ListData.Max(x => x.Mark);
            var maxGivenMark = (from m in model.ListData select m).Where(x => x.Mark != null).OrderByDescending(a => decimal.Parse(a.Mark)).FirstOrDefault();
            var maxGivenPracticalMark = (from m in model.ListData select m).Where(x => x.PracticalScore != null).OrderByDescending(a => decimal.Parse(a.PracticalScore)).FirstOrDefault();
            if (Convert.ToDecimal(maxGivenMark.Mark) > model.TotalScore)
            {
                msg = "The mark should be lower than the total mark";
            }
            //else if (Convert.ToDecimal(model.ListData.Max(x => x.PracticalScore)) > (100 - model.TotalScore))
            else if (maxGivenPracticalMark != null && Convert.ToDecimal(maxGivenPracticalMark.PracticalScore) > (100 - model.TotalScore))
            {
                msg = "The mark should be lower than the total mark";
            }
            else
            {
                if (model.IsEntered == false)//Add the result
                {
                    foreach (var item in model.ListData)
                    {
                        try
                        {
                            var result = new TbExamResult();
                            result.SchoolId = model.SchoolId;
                            result.StudentId = item.StudentId;
                            result.ExamId = model.ExamId;
                            //result.SubjectId = model.SubjectId;
                            result.SubjectId = declaredExamSubject.Id; // Exam table related to declared exam result and that is connected to the Subject table 
                            result.StudentScore = Convert.ToDecimal(item.Mark);
                            result.IsActive = true;
                            result.TimeStamp = CurrentTime;
                            if (item.PracticalScore != string.Empty && item.PracticalScore != null)
                                result.PracticalScore = Convert.ToDecimal(item.PracticalScore);// The CComputer Science subject only having the Practical Score, This is the Main requirement of Satluj School VI to IX standard students progress card format
                            else
                                result.PracticalScore = 0;
                            _Entities.TbExamResults.Add(result);
                            status = _Entities.SaveChanges() > 0;
                            if (status)
                            {
                                msg = "Successful";
                            }
                            else
                            {
                                msg = "Failed";
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else// Update the mark
                {

                    foreach (var item in model.ListData)
                    {
                        var oldData = _Entities.TbExamResults.Where(x => x.SchoolId == model.SchoolId && x.ExamId == model.ExamId && x.SubjectId == declaredExamSubject.Id && x.StudentId == item.StudentId).FirstOrDefault();
                        if (oldData != null)
                        {
                            if (oldData.StudentScore != Convert.ToDecimal(item.Mark))
                            {
                                oldData.StudentScore = Convert.ToDecimal(item.Mark);
                                if (item.PracticalScore != null && item.PracticalScore != string.Empty)
                                    oldData.PracticalScore = Convert.ToDecimal(item.PracticalScore);
                                oldData.TimeStamp = CurrentTime;
                                status = _Entities.SaveChanges() > 0;
                            }
                            else
                            {
                                if (item.PracticalScore != null && item.PracticalScore != string.Empty)
                                    oldData.PracticalScore = Convert.ToDecimal(item.PracticalScore);
                                _Entities.SaveChanges();
                                status = true;
                            }
                        }
                        else
                        {
                            var result = new TbExamResult();
                            result.SchoolId = model.SchoolId;
                            result.StudentId = item.StudentId;
                            result.ExamId = model.ExamId;
                            //result.SubjectId = model.SubjectId; declaredExamSubject.Id;
                            result.SubjectId = declaredExamSubject.Id; // the examresult table is connected to declared exam table and that table is directy connected to the subject table 
                            result.StudentScore = Convert.ToDecimal(item.Mark);
                            if (item.PracticalScore != null && item.PracticalScore != string.Empty)
                                result.PracticalScore = Convert.ToDecimal(item.PracticalScore);
                            else
                                result.PracticalScore = 0;
                            result.IsActive = true;
                            result.TimeStamp = CurrentTime;
                            _Entities.TbExamResults.Add(result);
                            status = _Entities.SaveChanges() > 0;
                        }

                        if (status)
                        {
                            msg = "Successful";
                        }
                        else
                        {
                            msg = "Failed";
                        }
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public ActionResult ScholasticAreaResult()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            model.UserId = _user.UserId;
            //DropdownData dropdown = new DropdownData();
            model.ClassList = (List<SelectListItem>)_dropdown.GetClasses(model.SchoolId);
            model.ClassList_wise = _dropdown.GetClassesUserWise(model.SchoolId,model.UserId);
            return View(model);
        }

        public PartialViewResult SudentListForEnterScolasticAreaResult(string id)
        {
            StudentsScholasticAreaResultModel model = new StudentsScholasticAreaResultModel();
            string[] splitData = id.Split('~');
            long subjectId = Convert.ToInt64(splitData[0]);
            long examId = Convert.ToInt64(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            long divisionId = Convert.ToInt64(splitData[3]);
            long schola = Convert.ToInt64(splitData[4]);

            model.SchoolId = _user.SchoolId;
            model.SubjectId = subjectId;
            model.ExamId = examId;
            model.ClassId = classId;
            model.DivisionId = divisionId;
            model.SchoolId = _user.SchoolId;
            model.ScholasticId = schola;
            model.ListData = new List<StudentScholasticMarkList>();
            bool IsOptionalSubjects = _Entities.TbSubjects.Where(x => x.SubId == model.SubjectId).Select(x => x.IsOptonal).FirstOrDefault();
            model.TotalScore = _Entities.TbScholasticAreas.Where(x => x.Region.SchoolId == model.SchoolId && x.Id == model.ScholasticId && x.IsActive && x.Region.IsActive).Select(x => x.TotalScore).FirstOrDefault();
            var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();
            var dataMain = _Entities.TbScholasticResultMains.Where(x => x.SchoolId == model.SchoolId && x.ScholasticId == model.ScholasticId && x.ExamId == model.ExamId && x.Student.ClassId == model.ClassId && x.Student.DivisionId == model.DivisionId && x.Student.IsActive == true && x.Exam.IsActive == true && x.IsActive == true).ToList();
            if (dataMain != null && dataMain.Count > 0)
            {
                model.IsEntered = true;
                foreach (var item in dataMain)
                {
                    var result = item.TbScolasticAreaResultDetails.Where(x => x.SubjectId == model.SubjectId && x.Main.StudentId == item.StudentId && x.MainId == item.Id && item.IsActive && x.IsActive).FirstOrDefault();
                    if (result != null)
                    {
                        StudentScholasticMarkList one = new StudentScholasticMarkList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.Student.StundentName;
                        one.Mark = Convert.ToString(result.Score);
                        //one.RollNo = item.TbStudents.ClasssNumber == null ? 0 : Convert.ToInt32(item.TbStudents.ClasssNumber);
                        one.RollNo = int.TryParse(item.Student.ClasssNumber, out var rn) ? rn : 0;

                        var chekDuplicate = model.ListData.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                        if (chekDuplicate == null)
                        {
                            model.ListData.Add(one);
                        }

                    }
                }
                if (studentList.Count > model.ListData.Count)// Check that we have a new student after the exam mark entry 
                {
                    var balanceStudents = studentList.Where(x => !model.ListData.Select(y => y.StudentId).Contains(x.StudentId)).ToList();
                    foreach (var item1 in balanceStudents)
                    {
                        if (IsOptionalSubjects)
                        {
                            if (_Entities.TbOptionalSubjectStudents.Any(x => x.StudentId == item1.StudentId && x.SubjectId == model.SubjectId && x.IsActive))
                            {
                                StudentScholasticMarkList one = new StudentScholasticMarkList();
                                one.StudentId = item1.StudentId;
                                one.StudentName = item1.StundentName;
                                one.Mark = "";
                                //one.RollNo = item1.ClasssNumber == null ? 0 : Convert.ToInt32(item1.ClasssNumber);
                                one.RollNo = int.TryParse(item1.ClasssNumber, out var rn) ? rn : 0;

                                //model.ListData.Add(one);
                                var chekDuplicate = model.ListData.Where(x => x.StudentId == item1.StudentId).FirstOrDefault();
                                if (chekDuplicate == null)
                                {
                                    model.ListData.Add(one);
                                }

                            }
                        }
                        else
                        {
                            StudentScholasticMarkList one = new StudentScholasticMarkList();
                            one.StudentId = item1.StudentId;
                            one.StudentName = item1.StundentName;
                            one.Mark = "";
                            // one.RollNo = item1.ClasssNumber == null ? 0 : Convert.ToInt32(item1.ClasssNumber);
                            one.RollNo = int.TryParse(item1.ClasssNumber, out var rn) ? rn : 0;

                            //model.ListData.Add(one);
                            var chekDuplicate = model.ListData.Where(x => x.StudentId == item1.StudentId).FirstOrDefault();
                            if (chekDuplicate == null)
                            {
                                model.ListData.Add(one);
                            }
                        }
                    }
                }
            }
            else
            {
                model.IsEntered = false;
                foreach (var item in studentList)
                {
                    if (IsOptionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.StudentId == item.StudentId && x.SubjectId == model.SubjectId && x.IsActive))
                        {
                            StudentScholasticMarkList one = new StudentScholasticMarkList();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one.Mark = "";
                            //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                            one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                            //model.ListData.Add(one);
                            var chekDuplicate = model.ListData.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                            if (chekDuplicate == null)
                            {
                                model.ListData.Add(one);
                            }
                        }
                    }
                    else
                    {
                        StudentScholasticMarkList one = new StudentScholasticMarkList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one.Mark = "";
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        //model.ListData.Add(one);
                        var chekDuplicate = model.ListData.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                        if (chekDuplicate == null)
                        {
                            model.ListData.Add(one);
                        }
                    }
                }
            }
            model.ListData = model.ListData.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).Distinct().ToList();
            //model.ListData = model.ListData.OrderBy(x => x.StudentName).ToList();

            return PartialView("~/Views/Progress/_pv_Student_ScholasticArea_Mark.cshtml", model);
        }

        [HttpPost]
        public object SubmitStudentsScholasticAreaMarks(StudentsScholasticAreaResultModel model)
        {
            bool status = false;
            string msg = "Failed";
            var maxGivenMark = (from m in model.ListData select m).OrderByDescending(a => decimal.Parse(a.Mark)).FirstOrDefault();
            //if (Convert.ToDecimal(model.ListData.Max(x => x.Mark)) > model.TotalScore)
            if (Convert.ToDecimal(maxGivenMark.Mark) > model.TotalScore)
            {
                msg = "The mark should be higher than the total mark";
            }
            else
            {
                if (model.IsEntered == false)//Add the result
                {
                    foreach (var item in model.ListData)
                    {
                        var main = new TbScholasticResultMain();
                        main.SchoolId = model.SchoolId;
                        main.StudentId = item.StudentId;
                        main.ExamId = model.ExamId;
                        main.ScholasticId = model.ScholasticId;
                        main.IsActive = true;
                        main.TimeStamp = CurrentTime;
                        _Entities.TbScholasticResultMains.Add(main);
                        status = _Entities.SaveChanges() > 0;
                        if (status)
                        {
                            var details = new  TbScolasticAreaResultDetail();
                            details.MainId = main.Id;
                            details.SubjectId = model.SubjectId;
                            details.Score = Convert.ToDecimal(item.Mark);
                            details.IsActive = true;
                            details.TimeStamp = CurrentTime;
                            _Entities.TbScolasticAreaResultDetails.Add(details);
                            status = _Entities.SaveChanges() > 0;
                        }
                        if (status)
                            msg = "Successful";
                        else
                            msg = "Failed";
                    }
                }
                else// Update the mark
                {
                    foreach (var item in model.ListData)
                    {
                        var mainData = _Entities.TbScholasticResultMains.Where(x => x.ExamId == model.ExamId && x.TbScolasticAreaResultDetails.Where(y => y.SubjectId == model.SubjectId && y.IsActive).FirstOrDefault().SubjectId == model.SubjectId && x.SchoolId == model.SchoolId && x.ScholasticId == model.ScholasticId && x.StudentId == item.StudentId && x.IsActive).FirstOrDefault();
                        if (mainData != null)
                        {
                            var dataScore = mainData.TbScolasticAreaResultDetails.Where(x => x.MainId == mainData.Id && x.SubjectId == model.SubjectId && x.IsActive).FirstOrDefault();
                            if (dataScore != null && dataScore.Score != Convert.ToDecimal(item.Mark))
                            {
                                dataScore.Score = Convert.ToDecimal(item.Mark);
                                status = _Entities.SaveChanges() > 0;
                            }
                        }
                        else
                        {
                            var main = new TbScholasticResultMain ();
                            main.SchoolId = model.SchoolId;
                            main.StudentId = item.StudentId;
                            main.ExamId = model.ExamId;
                            main.ScholasticId = model.ScholasticId;
                            main.IsActive = true;
                            main.TimeStamp = CurrentTime;
                            _Entities.TbScholasticResultMains.Add(main);
                            status = _Entities.SaveChanges() > 0;
                            if (status)
                            {
                                var details =new  TbScolasticAreaResultDetail();
                                details.MainId = main.Id;
                                details.SubjectId = model.SubjectId;
                                details.Score = Convert.ToDecimal(item.Mark);
                                details.IsActive = true;
                                details.TimeStamp = CurrentTime;
                                _Entities.TbScolasticAreaResultDetails.Add(details);
                                status = _Entities.SaveChanges() > 0;
                            }
                        }

                        if (status)
                        {
                            msg = "Successful";
                        }
                        else
                        {
                            msg = "Failed";
                        }
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public ActionResult CoScholasticAreaResult()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Classes = _dropdown.GetClasses(model.SchoolId);
            return View(model);
        }
        public PartialViewResult SudentListForEnterCoScolasticAreaResult(string id)
        {
            StudentsCoScholasticAreaResultModel model = new StudentsCoScholasticAreaResultModel();
            string[] splitData = id.Split('~');
            long examId = Convert.ToInt64(splitData[0]);
            long classId = Convert.ToInt64(splitData[1]);
            long divisionId = Convert.ToInt64(splitData[2]);
            long schola = Convert.ToInt64(splitData[3]);

            model.SchoolId = _user.SchoolId;
            model.ExamId = examId;
            model.ClassId = classId;
            model.DivisionId = divisionId;
            model.SchoolId = _user.SchoolId;
            model.CoScholasticId = schola;
            model.ListData = new List<StudentCoScholasticMarkList>();
            List<StudentCoScholasticMarkList> li_a1 = new List<StudentCoScholasticMarkList>();
            var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();

            var dataMain = _Entities.TbCoScholasticResultmains.Where(x => x.SchoolId == model.SchoolId && x.CoScholasticId == model.CoScholasticId && x.ExamId == model.ExamId && x.IsActive == true && x.Student.DivisionId == model.DivisionId && x.Student.IsActive).ToList();
            if (dataMain != null && dataMain.Count > 0)
            {
                model.IsEntered = true;
                foreach (var item in dataMain)
                {
                    //var sss =   Model.ListData.where
                    var sss = li_a1.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                    if (sss == null)
                    {
                        StudentCoScholasticMarkList one = new StudentCoScholasticMarkList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.Student.StundentName;
                        one.Mark = item.Score;
                        //one.RollNo = item.TbStudents.ClasssNumber == null ? 0 : Convert.ToInt32(item.TbStudents.ClasssNumber);
                        one.RollNo = int.TryParse(item.Student.ClasssNumber, out var rn) ? rn : 0;

                        li_a1.Add(one);
                        //model.ListData.Add(one);
                    }
                    model.ListData = li_a1;

                }
                if (studentList.Count > model.ListData.Count)// Check that we have a new student after the exam mark entry 
                {
                    var balanceStudents = studentList.Where(x => !model.ListData.Select(y => y.StudentId).Contains(x.StudentId)).ToList();
                    foreach (var item1 in balanceStudents)
                    {
                        StudentCoScholasticMarkList one = new StudentCoScholasticMarkList();
                        one.StudentId = item1.StudentId;
                        one.StudentName = item1.StundentName;
                        one.Mark = "";
                        //one.RollNo = item1.ClasssNumber == null ? 0 : Convert.ToInt32(item1.ClasssNumber);
                        one.RollNo = int.TryParse(item1.ClasssNumber, out var rn) ? rn : 0;

                        model.ListData.Add(one);
                    }
                }
            }
            else
            {
                model.IsEntered = false;
                foreach (var item in studentList)
                {
                    StudentCoScholasticMarkList one = new StudentCoScholasticMarkList();
                    one.StudentId = item.StudentId;
                    one.StudentName = item.StundentName;
                    one.Mark = "";
                    //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                    one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                    model.ListData.Add(one);
                }
            }
            model.ListData = model.ListData.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();

            return PartialView("~/Views/Progress/_pv_Student_CoScholasticArea_Mark.cshtml", model);
        }

        [HttpPost]
        public object SubmitStudentsCoScholasticAreaMarks(StudentsCoScholasticAreaResultModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (model.IsEntered == false)//Add the result
            {
                foreach (var item in model.ListData)
                {
                    var main = new TbCoScholasticResultmain();
                    main.SchoolId = model.SchoolId;
                    main.StudentId = item.StudentId;
                    main.ExamId = model.ExamId;
                    main.CoScholasticId = model.CoScholasticId;
                    if (item.Mark == null)
                    {
                        item.Mark = "-";
                    }
                    main.Score = item.Mark;
                    main.IsActive = true;
                    main.TimeStamp = CurrentTime;
                    _Entities.TbCoScholasticResultmains.Add(main);
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Successful";
                    else
                        msg = "Failed";
                }
            }
            else// Update the mark
            {
                foreach (var item in model.ListData)
                {
                    var mainData = _Entities.TbCoScholasticResultmains.Where(x => x.ExamId == model.ExamId && x.SchoolId == model.SchoolId && x.CoScholasticId == model.CoScholasticId && x.StudentId == item.StudentId && x.IsActive).FirstOrDefault();
                    if (mainData != null)
                    {
                        if (mainData.Score == item.Mark)
                        {
                            status = true;
                        }
                        else
                        {
                            mainData.Score = item.Mark.ToUpper();
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                    else
                    {
                        if (item.Mark != null)
                        {
                            var main = new TbCoScholasticResultmain();
                            main.SchoolId = model.SchoolId;
                            main.StudentId = item.StudentId;
                            main.ExamId = model.ExamId;
                            main.CoScholasticId = model.CoScholasticId;
                            main.Score = item.Mark.ToUpper();
                            main.IsActive = true;
                            main.TimeStamp = CurrentTime;
                            try
                            {
                                _Entities.TbCoScholasticResultmains.Add(main);
                                status = _Entities.SaveChanges() > 0;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    if (status)
                    {
                        msg = "Successful";
                    }
                    else
                    {
                        msg = "Failed";
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public ActionResult OptionalSubjects()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            //var dropdownData = new DropdownData();
            model.SubjectList = _dropdown.GetAllOptionalSubjects(model.SchoolId);
            ViewBag.ClassList = _dropdown.GetClasses(model.SchoolId);
            return View(model);
        }

        public PartialViewResult SudentListForOptionalStubjects(string id)
        {

            string[] splitData = id.Split('~');
            long SubjectId = Convert.ToInt64(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            long ClassId = Convert.ToInt64(splitData[2]);
            long DivisionId = Convert.ToInt64(splitData[3]);

            var studnetList = _Entities.Students
        .FromSqlRaw(
            "EXEC sp_StudentsWithNoOptionalSubjects @SchoolId, @ClassId, @DivisionId, @SubjectId",
            new SqlParameter("@SchoolId", SchoolId),
            new SqlParameter("@ClassId", ClassId),
            new SqlParameter("@DivisionId", DivisionId),
            new SqlParameter("@SubjectId", SubjectId)
        )
        .Select(x => new Satluj_Latest.Data.Student(x.StudentId))
        .ToList();

            //var studnetList = _Entities.sp_StudentsWithNoOptionalSubjects(SchoolId, ClassId, DivisionId, SubjectId).Select(x => new TrackTap.DataLibrary.Data.Student(x.StudentId)).ToList();
            AddOptionalSubjectsToStudentsModel model = new AddOptionalSubjectsToStudentsModel();
            model.SchoolId = SchoolId;
            model.ClassId = ClassId;
            model.DivisionId = DivisionId;
            model.SubjectId = SubjectId;
            model.Data = new List<StudentListData>();
            foreach (var item in studnetList)
            {
                StudentListData one = new StudentListData();
                one.StudentId = item.StudentId;
                one.StudentName = item.StundentName;
                one.SpecialId = item.StudentSpecialId;
                one.FilePath = item.FilePath;
                one.ParentName = item.ParentName;
                one.ContactNumber = item.ContactNumber;
                one.Address = item.Address;
                model.Data.Add(one);
            }
            return PartialView("~/Views/Progress/_pv_Student_WithoutOptionalSubjects.cshtml", model);
        }

        [HttpPost]
        public object SubmitStudentsForOptionalSubjects(AddOptionalSubjectsToStudentsModel model)
        {
            bool status = false;
            string msg = "Failed";
            foreach (var item in model.Data)
            {
                var data =new TbOptionalSubjectStudent();
                data.SchoolId = _user.SchoolId;
                data.StudentId = item.StudentId;
                data.SubjectId = model.SubjectId;
                data.IsActive = true;
                data.TimeStamp = CurrentTime;
                _Entities.TbOptionalSubjectStudents.Add(data);
                status = _Entities.SaveChanges() > 0;
            }
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }
        public ActionResult OptionalSubjectsList()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            //var dropdownData = new DropdownData();
            model.OptionalSubjectList =
                _dropdown.GetAllOptionalSubjects(model.SchoolId);
            ViewBag.ClassList = _dropdown.GetClasses(model.SchoolId);

            return View(model);
        }

        public PartialViewResult SudentListForOptionalStubjectsListing(string id)
        {

            string[] splitData = id.Split('~');
            long SubjectId = Convert.ToInt64(splitData[0]);
            long SchoolId = Convert.ToInt64(splitData[1]);
            long ClassId = Convert.ToInt64(splitData[2]);
            long DivisionId = Convert.ToInt64(splitData[3]);

            //var studnetList = _Entities.sp_StudentsWithOptionalSubjects(SchoolId, ClassId, DivisionId, SubjectId).Select(x => new TrackTap.DataLibrary.Data.Student(x.StudentId)).ToList();

            var studnetList = _Entities.Students
    .FromSqlRaw(
        "EXEC sp_StudentsWithOptionalSubjects @SchoolId, @ClassId, @DivisionId, @SubjectId",
        new SqlParameter("@SchoolId", SchoolId),
        new SqlParameter("@ClassId", ClassId),
        new SqlParameter("@DivisionId", DivisionId),
        new SqlParameter("@SubjectId", SubjectId)
    )
    .Select(x => new Satluj_Latest.Data.Student(x.StudentId))
    .ToList();

            AddOptionalSubjectsToStudentsModel model = new AddOptionalSubjectsToStudentsModel();
            model.SchoolId = SchoolId;
            model.ClassId = ClassId;
            model.DivisionId = DivisionId;
            model.SubjectId = SubjectId;
            model.Data = new List<StudentListData>();
            foreach (var item in studnetList)
            {
                StudentListData one = new StudentListData();
                one.StudentId = item.StudentId;
                one.StudentName = item.StundentName;
                one.SpecialId = item.StudentSpecialId;
                one.FilePath = item.FilePath;
                one.ParentName = item.ParentName;
                one.ContactNumber = item.ContactNumber;
                one.Address = item.Address;
                one.SubjectId = SubjectId;
                model.Data.Add(one);
            }
            return PartialView("~/Views/Progress/_pv_Student_WithOptionalSubjects.cshtml", model);
        }
        public ActionResult StudentRemark()
        {
            ExamResultMainModel model = new ExamResultMainModel();
            model.SchoolId = _user.SchoolId;
            //DropdownData dropdownData = new DropdownData();
            model.ClassList = (List<SelectListItem>)_dropdown.GetClasses(model.SchoolId);
            
            return View(model);
        }
        public PartialViewResult StudentsResultResult(string id)
        {
            string[] splitData = id.Split('~');
            StudentRemarkModel model = new StudentRemarkModel();
            model.SchoolId = _user.SchoolId;
            model.ExamId = Convert.ToInt64(splitData[0]);
            model.ClassId = Convert.ToInt64(splitData[1]);
            model.DivisionId = Convert.ToInt64(splitData[2]);
            model.list = new List<Satluj_Latest.Models.StudentRemark>();
            var studentList = _Entities.TbStudents.Where(x => x.SchoolId == model.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).OrderBy(x => x.StundentName).ToList();
            foreach (var item in studentList)
            {
                Satluj_Latest.Models.StudentRemark one = new Satluj_Latest.Models.StudentRemark();
                one.StudentId = item.StudentId;
                one.StudentName = item.StundentName;
                var remark = _Entities.TbStudentRemarks.Where(x => x.StudentId == item.StudentId && x.ExamId == model.ExamId && x.IsActive).FirstOrDefault();
                if (remark != null)
                {
                    one.Remark = remark.Remark == null ? "" : remark.Remark;
                    one.AdditionalRemark = remark.AdditionalRemarks == null ? "" : remark.AdditionalRemarks;
                }
                else
                {
                    one.Remark = "";
                    one.AdditionalRemark = "";
                }
                //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                model.list.Add(one);
            }
            model.list = model.list.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
            ViewBag.Remarks = _dropdown.GetAllRemarks(model.SchoolId);
            return PartialView("~/Views/Progress/_pv_StudentsRemarkResult.cshtml", model);
        }
        [HttpPost]
        public object SubmitStudentsRemarks(StudentRemarkModel model)
        {
            bool status = false;
            string msg = "Failed";
            foreach (var item in model.list)
            {
                var old = _Entities.TbStudentRemarks.Where(x => x.StudentId == item.StudentId && x.ExamId == model.ExamId && x.IsActive).FirstOrDefault();
                if (old != null)
                {
                    if (old.Remark == item.Remark && old.AdditionalRemarks == item.AdditionalRemark)
                    {
                        status = true;
                    }
                    else
                    {
                        if (old.Remark != "" && item.Remark == null)
                        {

                        }
                        else
                        {
                            if (item.Remark == "--Choose--")
                                old.Remark = "";
                            else
                                old.Remark = item.Remark == null ? "" : item.Remark;
                        }
                        old.AdditionalRemarks = item.AdditionalRemark == null ? "" : item.AdditionalRemark;
                        status = _Entities.SaveChanges() > 0;
                        status = true;
                    }
                }
                else
                {
                    try
                    {
                        var data = new TbStudentRemark();
                        data.StudentId = item.StudentId;
                        data.ExamId = model.ExamId;
                        if (item.Remark != "--Choose--")
                            data.Remark = item.Remark == null ? " " : item.Remark.Trim();
                        else
                            data.Remark = "";
                        data.AdditionalRemarks = item.AdditionalRemark == null ? " " : item.AdditionalRemark.Trim();
                        data.IsActive = true;
                        data.TimeStamp = CurrentTime;
                        _Entities.TbStudentRemarks.Add(data);
                        status = _Entities.SaveChanges() > 0;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }

        public object DeleteScholastic(string id)
        {
            bool status = false;
            string msg = "Failed";
            long ScholasticId = Convert.ToInt64(id);
            var result = _Entities.TbScholasticResultMains.Where(x => x.ScholasticId == ScholasticId && x.IsActive).ToList();
            if (result.Count > 0 && result != null)
            {
                msg = "The Scholastic Area have the results";
            }
            else
            {
                var data = _Entities.TbScholasticAreas.Where(x => x.Id == ScholasticId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, msg = msg });
        }
        public object DeleteResultEnteredScholasticArea(string id)
        {
            bool status = false;
            string msg = "Failed";

            try
            {
                long ScholasticId = Convert.ToInt64(id);
                long schoolid = _user.SchoolId;
                var result = _Entities.TbScholasticResultMains.Where(x => x.ScholasticId == ScholasticId && x.IsActive).ToList();
                if (result.Count > 0 && result != null)
                {
                    //foreach (var item in result)
                    //{
                    //    item.IsActive = false;
                    //    _Entities.SaveChanges();
                    //}
                    //var datas = _Entities.sp_DeleteResultEnteredScholasticArea(schoolid, ScholasticId);
                    var datas = _Entities.deleteresultentered
                               .FromSqlRaw("EXEC sp_DeleteResultEnteredScholasticArea {0}, {1}",
                                           schoolid, ScholasticId);
                            


                }
                var data = _Entities.TbScholasticAreas.Where(x => x.Id == ScholasticId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
                return Json(new { status = status, msg = msg });
            }
            catch (Exception ex)
            {
                status = false;
                return Json(new { status = status, msg = ex.Message });
            }


        }

        public PartialViewResult ScholasticEditPartial(string id)
        {
            ScholasticAreaModel model = new ScholasticAreaModel();
            long scoId = Convert.ToInt64(id);
            var data = _Entities.TbScholasticAreas.Where(x => x.Id == scoId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.RegionId = data.RegionId;
                model.SchoolId = data.SchoolId;
                model.AreaName = data.ItemName;
                model.Score = data.TotalScore;
                model.RegionName = data.Region.RegionName;
            }
            return PartialView("~/Views/Progress/_pv_ScolasticAreaEdit.cshtml", model);
        }
        public object EditScholasticArea(ScholasticAreaModel model)
        {
            bool status = false;
            string msg = "Failed";
            var data1 = _Entities.TbScholasticAreas.Where(x => x.IsActive && x.Id == model.Id).FirstOrDefault();

            var resultData = _Entities.TbScolasticAreaResultDetails.Where(x => x.Main.ScholasticId == model.Id && x.Score > model.Score && x.Main.IsActive && x.IsActive).ToList();
            if (resultData.Count > 0 && resultData != null)
            {
                msg = "More score have been given to some children, so Total score should be give above";
            }
            else
            {
                var sameRegionsHave = _Entities.TbScholasticAreas.Where(x => x.RegionId == model.RegionId && x.IsActive && x.Id != model.Id).ToList();
                if (sameRegionsHave.Count > 4)
                {
                    msg = "There are 4 Scholastic Areas in this Region already. It can not be reached at this time";
                }
                else
                {
                    var data = _Entities.TbScholasticAreas.Where(x => x.IsActive && x.Id == model.Id).FirstOrDefault();
                    //data.RegionId = model.RegionId;
                    data.ItemName = model.AreaName;
                    data.TotalScore = model.Score;
                    status = _Entities.SaveChanges() > 0;
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public object DeleteCoScholastic(string id)
        {
            bool status = false;
            string msg = "Failed";
            long CoScholasticId = Convert.ToInt64(id);
          //  Session["CoScholasticId_session"] = CoScholasticId;
            HttpContext.Session.SetInt32("CoScholasticId_session", (int)CoScholasticId);

            var result = _Entities.TbCoScholasticResultmains.Where(x => x.CoScholasticId == CoScholasticId && x.IsActive).ToList();
            if (result.Count > 0 && result != null)
            {
                msg = "The Co-Scholastic Area have the results";
            }
            else
            {
                var data = _Entities.TbCoScholasticAreas.Where(x => x.Id == CoScholasticId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, msg = msg });
        }

        public object DeleteResultEnteredCoScholasticArea(string id)
        {
            bool status = false;
            string msg = "Failed";

            try
            {

                //if (id == null)
                //{
                //id = Session["CoScholasticId_session"].ToString();
                id = HttpContext.Session.GetString("CoScholasticId_session");

                //}

                long CoScholasticId = Convert.ToInt64(id);
                long schoolid = _user.SchoolId;
                var result = _Entities.TbCoScholasticResultmains.Where(x => x.CoScholasticId == CoScholasticId && x.IsActive).ToList();
                if (result.Count > 0 && result != null)
                {
                    //foreach (var item in result)
                    //{
                    //    item.IsActive = false;
                    //    _Entities.SaveChanges();
                    //}

                    //var datas = _Entities.sp_DeleteResultEnteredCoScholasticArea(schoolid, CoScholasticId);
                    var datas= _Entities.deleteresultentered
        .FromSqlRaw("EXEC sp_DeleteResultEnteredCoScholasticArea {0}, {1}",
                    schoolid, CoScholasticId)
        .ToList();



                }
                var data = _Entities.TbCoScholasticAreas.Where(x => x.Id == CoScholasticId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
                return Json(new { status = status, msg = msg });
            }
            catch (Exception ex)
            {
                status = false;
                return Json(new { status = status, msg = ex.Message });
            }


        }

        public PartialViewResult CoScholasticEditPartial(string id)
        {
            CoScholasticAreaModel model = new CoScholasticAreaModel();
            long scoId = Convert.ToInt64(id);
            var data = _Entities.TbCoScholasticAreas.Where(x => x.Id == scoId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.RegionId = data.RegionId;
                model.SchoolId = data.SchoolId;
                model.Item = data.ItemName;
                model.RegionName = data.Region.RegionName;
            }
            return PartialView("~/Views/Progress/_pv_CoScolasticAreaEdit.cshtml", model);
        }
        public object EditCoScholasticArea(CoScholasticAreaModel model)
        {
            bool status = false;
            string msg = "Failed";
            var data = _Entities.TbCoScholasticAreas.Where(x => x.IsActive && x.Id == model.Id).FirstOrDefault();
            //data.RegionId = model.RegionId;
            data.ItemName = model.Item;
            status = _Entities.SaveChanges() > 0;
            if (status)
                msg = "Success";
            return Json(new { status = status, msg = msg });
        }

        public object DeleteDeclaredExam(string id)
        {
            bool status = false;
            string msg = "Failed";
            long ExamId = Convert.ToInt64(id);
            var result = _Entities.TbExamResults.Where(x => x.ExamId == ExamId && x.IsActive).ToList();
            if (result.Count > 0 && result != null)
            {
                msg = "The result of this exam was given to children. Therefore it can not be avoided";
            }
            else
            {
                var data = _Entities.TbDeclaredExams.Where(x => x.Id == ExamId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, msg = msg });
        }

        public object DeleteResultEnteredDeclaredExams(string id)
        {
            bool status = false;
            string msg = "Failed";
            long Id = Convert.ToInt64(id);
            var result = _Entities.TbExamResults.Where(x => x.ExamId == Id && x.IsActive).ToList();
            if (result.Count > 0 && result != null)
            {
                foreach (var item in result)
                {
                    item.IsActive = false;
                    _Entities.SaveChanges();
                }
            }
            var data = _Entities.TbDeclaredExams.Where(x => x.Id == Id && x.IsActive).FirstOrDefault();
            data.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            if (status)
                msg = "Success";
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult DeclaredExamEditPartial(string id)
        {
            ExamDeclarationModel model = new ExamDeclarationModel();
            long scoId = Convert.ToInt64(id);
            var data = _Entities.TbDeclaredExams.Where(x => x.Id == scoId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.SchoolId = _user.SchoolId;
                model.TermId = data.TermId ?? 0;
                model.ClassId = data.ClassId;
                model.EndDate = data.EndDate;
                model.EndDateString = data.EndDate.ToString("dd-MM-yyyy");
                model.ExamName = data.TbExamBook.ExamName;
                model.StartDate = data.StartDate;
                model.StartDateString = data.StartDate.ToString("dd-MM-yyyy");
                model.Id = data.Id;
                model.TermName = data.TbExamTerm.DefaultExam;
                model.ClassName = data.Class.Class;
            }
            return PartialView("~/Views/Progress/_pv_DeclaredExamEdit.cshtml", model);
        }
        public object EditDeclaredExam(ExamDeclarationModel model)
        {
            bool status = false;
            string msg = "Failed";
            var resultData = _Entities.TbDeclaredExams.Where(x => x.Id == model.Id && x.IsActive).FirstOrDefault();
            if (resultData != null)
            {
                var exam = _Entities.TbExamBooks.Where(x => x.Id == resultData.ExamId && x.IsActive).FirstOrDefault();
                if (exam.ExamName != model.ExamName)
                {
                    exam.ExamName = model.ExamName;
                    status = _Entities.SaveChanges() > 0;
                }
                else
                {
                    status = true;
                }
                try
                {
                    if (model.StartDateString != string.Empty && model.StartDateString != null)
                    {
                        string[] splitData = model.StartDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        //model.StartDate = Convert.ToDateTime(date);
                        model.StartDate = DateTime.ParseExact(date, "MM-dd-yyyy", new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                    }
                }
                catch
                {
                    msg = "Invalid Start Date !";
                }
                try
                {
                    if (model.EndDateString != string.Empty && model.EndDateString != null)
                    {
                        string[] splitData = model.EndDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        //model.EndDate = Convert.ToDateTime(date);
                        model.EndDate = DateTime.ParseExact(date, "MM-dd-yyyy", new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                    }
                }
                catch
                {
                    msg = "Invalid End Date !";
                    status = false;
                }
                if (resultData.StartDate != model.StartDate || resultData.EndDate != model.EndDate)
                {
                    if (model.StartDate >= model.EndDate)
                    {
                        msg = "Wrong start and end date !!";
                        status = false;
                    }
                    else
                    {
                        resultData.StartDate = model.StartDate;
                        resultData.EndDate = model.EndDate;
                        status = _Entities.SaveChanges() > 0;
                        if (status)
                            msg = "Successful";
                    }
                }
                else
                {
                    status = true;
                    msg = "Successful";
                }
            }

            return Json(new { status = status, msg = msg });
        }
        public object DeleteDeclaredExamSubjects(string id)
        {
            bool status = false;
            string msg = "Failed";
            string[] splitData = id.Split('~');
            long ExamId = Convert.ToInt64(splitData[0]);
            long SubjectId = Convert.ToInt64(splitData[1]);
            long Id = Convert.ToInt64(splitData[2]);
            var result = _Entities.TbExamResults.Where(x => x.ExamId == ExamId && x.IsActive && x.SubjectId == SubjectId).ToList();
            if (result.Count > 0 && result != null)
            {
                msg = "The result of this exam was given to children. Therefore it can not be avoided";
            }
            else
            {
                var data = _Entities.TbDeclaredExamSubjects.Where(x => x.Id == Id && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, msg = msg });
        }

        public object DeleteResultEnteredDeclaredExamsSubjects(string id)
        {
            bool status = false;
            string msg = "Failed";
            string[] splitData = id.Split('~');
            long ExamId = Convert.ToInt64(splitData[0]);
            long SubjectId = Convert.ToInt64(splitData[1]);
            long Id = Convert.ToInt64(splitData[2]);
            var result = _Entities.TbExamResults.Where(x => x.ExamId == ExamId && x.IsActive && x.SubjectId == SubjectId).ToList();
            if (result.Count > 0 && result != null)
            {
                foreach (var item in result)
                {
                    item.IsActive = false;
                    _Entities.SaveChanges();
                }
            }
            var data = _Entities.TbDeclaredExamSubjects.Where(x => x.Id == Id && x.IsActive).FirstOrDefault();
            data.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            if (status)
                msg = "Success";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult DeclaredExamSubjectEditPartial(string id)
        {
            DeclaredExamSubjectModel model = new DeclaredExamSubjectModel();
            long Id = Convert.ToInt64(id);
            var data = _Entities.TbDeclaredExamSubjects.Where(x => x.Id == Id && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.ExamId = data.DeclaredExamId;
                model.SubjectId = data.SubjectId;
                model.ExamTimeString = data.ExamDate.ToString("dd-MM-yyyy");
                model.ExamTime = data.ExamDate;
                model.TotalScore = data.TotalScore;
                model.Remark = data.Remark;
                model.SubjectName = data.Subject.SubjectName;
                model.Id = data.Id;
            }
            return PartialView("~/Views/Progress/_pv_DeclaredExamSubjectEdit.cshtml", model);
        }
        public object EditDeclaredSubjectExam(DeclaredExamSubjectModel model)
        {
            bool status = false;
            string msg = "Failed";
            bool flag = false;
            var resultData = _Entities.TbDeclaredExamSubjects.Where(x => x.Id == model.Id && x.IsActive).FirstOrDefault();
            if (resultData != null)
            {
                try
                {
                    if (model.ExamTimeString != string.Empty && model.ExamTimeString != null)
                    {
                        string[] splitData = model.ExamTimeString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        model.ExamTime = DateTime.ParseExact(date, "MM-dd-yyyy", new CultureInfo("en-US"), System.Globalization.DateTimeStyles.None);
                        resultData.ExamDate = model.ExamTime;
                    }
                }
                catch
                {
                    msg = "Invalid Start Date !";
                    flag = true;
                }
                var exam = _Entities.TbDeclaredExams.Where(x => x.Id == resultData.DeclaredExamId && x.IsActive).FirstOrDefault();
                if (exam != null)
                {
                    if (exam.StartDate > model.ExamTime)
                    {
                        msg = "The exam only started with " + exam.StartDate.ToShortDateString() + " ,so the date is invalid";
                        flag = true;
                    }
                    else if (exam.EndDate < model.ExamTime)
                    {
                        msg = "The exam ended with " + exam.EndDate.ToShortDateString() + " ,so the date is invalid";
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    resultData.Remark = model.Remark == null ? " " : model.Remark;
                    resultData.TotalScore = model.TotalScore;
                    try
                    {
                        status = _Entities.SaveChanges() > 0;
                    }
                    catch (Exception ex)
                    {

                    }
                    if (status)
                        msg = "Successful";
                }
            }

            return Json(new { status = status, msg = msg });
        }

        public ActionResult ExamTimeTableViewPrint(string id)
        {
            long ExamId = Convert.ToInt64(id);
            ExamTimetableModel model = new ExamTimetableModel();
            var data = _Entities.TbDeclaredExams.Where(x => x.Id == ExamId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.SchoolName = _user.School.SchoolName;
                model.SchoolAdddress = _user.School.Address;
                model.SchoolLogo = _user.School.FilePath;
                model.ExamName = data.TbExamBook.ExamName + " - " + data.TbExamTerm.DefaultExam;
                model.ClassName = data.Class.Class;
                model.StartDate = data.StartDate;
                model.EndDate = data.EndDate;
                model.List = new List<Satluj_Latest.Models.TimeTable>();
                var subjectList = data.TbDeclaredExamSubjects.Where(x => x.IsActive).ToList();
                if (subjectList.Count > 0 && subjectList != null)
                {
                    var dates = subjectList.Select(x => x.ExamDate.Date).Distinct().OrderBy(x => x.Date).ToList();
                    foreach (var item in dates)
                    {
                        Satluj_Latest.Models.TimeTable one = new Satluj_Latest.Models.TimeTable();
                        one.ExamDate = item;
                        var subjects = subjectList.Where(x => x.ExamDate.Date == item).OrderBy(x => x.TimeStamp).ToList();
                        if (subjects.Count == 0)
                        {
                            one.Subject1 = "";
                            one.Subject2 = "";
                        }
                        else if (subjects.Count == 1)
                        {
                            one.Subject1 = subjects[0].Subject.SubjectName;
                            one.Subject2 = "";
                        }
                        else if (subjects.Count == 2)
                        {
                            one.Subject1 = subjects[0].Subject.SubjectName;
                            one.Subject2 = subjects[1].Subject.SubjectName;
                        }
                        model.List.Add(one);
                    }
                }
            }
            return View(model);
        }

        public object DeleteOptionalSubjects(string id)
        {
            string[] splitData = id.Split('~');
            bool status = false;
            string msg = "Failed";
            long studentId = Convert.ToInt64(splitData[0]);
            long subjectId = Convert.ToInt64(splitData[1]);
            var examAnswerResult = _Entities.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectId && x.IsActive).FirstOrDefault();
            if (examAnswerResult != null)
            {
                msg = "This child has marks in the exam held on this subject";
            }
            else
            {
                var data = _Entities.TbOptionalSubjectStudents.Where(x => x.StudentId == studentId && x.SubjectId == subjectId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, msg = msg });
        }
        public object DeleteResultEnteredOptionalSubjects(string id)
        {
            string[] splitData = id.Split('~');
            bool status = false;
            string msg = "Failed";
            long studentId = Convert.ToInt64(splitData[0]);
            long subjectId = Convert.ToInt64(splitData[1]);
            var examAnswerResult = _Entities.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectId && x.IsActive).ToList();
            if (examAnswerResult != null && examAnswerResult.Count > 0)
            {
                foreach (var item in examAnswerResult)
                {
                    var one = _Entities.TbExamResults.Where(x => x.Id == item.Id).FirstOrDefault();
                    one.IsActive = false;
                    _Entities.SaveChanges();
                }
            }

            var data = _Entities.TbOptionalSubjectStudents.Where(x => x.StudentId == studentId && x.SubjectId == subjectId && x.IsActive).FirstOrDefault();
            data.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            if (status)
                msg = "Success";

            return Json(new { status = status, msg = msg });
        }
        public object DeleteRegion(string id)
        {
            bool status = false;
            string msg = "Failed";
            long RegionId = Convert.ToInt64(id);
            var resultCoScho = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionId && x.IsActive).ToList();
            if (resultCoScho.Count > 0 && resultCoScho != null)
            {
                msg = "Many co-scholastic areas have been added in this region";
            }
            else if (_Entities.TbScholasticAreas.Any(x => x.RegionId == RegionId && x.IsActive))
            {
                msg = "Many Scholastic areas have been added in this region";
            }
            else
            {
                var data = _Entities.TbRegionss.Where(x => x.Id == RegionId && x.IsActive).FirstOrDefault();
                data.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    msg = "Success";
                    var listClass = _Entities.TbRegionsClasses.Where(x => x.RegionId == RegionId && x.IsActive).ToList();
                    foreach (var item in listClass)
                    {
                        var one = _Entities.TbRegionsClasses.Where(x => x.Id == item.Id).FirstOrDefault();
                        one.IsActive = false;
                        _Entities.SaveChanges();
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public object DeleteResultEnteredRegions(string id)
        {
            bool status = false;
            string msg = "Failed";
            long RegionId = Convert.ToInt64(id);
            var resultCoScho = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionId && x.IsActive).ToList();
            foreach (var item in resultCoScho)
            {
                item.IsActive = false;
                _Entities.SaveChanges();
            }
            var resultScho = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionId && x.IsActive).ToList();
            foreach (var item in resultScho)
            {
                item.IsActive = false;
                _Entities.SaveChanges();
            }
            var data = _Entities.TbRegionss.Where(x => x.Id == RegionId && x.IsActive).FirstOrDefault();
            data.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            if (status)
            {
                msg = "Success";
                var listClass = _Entities.TbRegionsClasses.Where(x => x.RegionId == RegionId && x.IsActive).ToList();
                foreach (var item in listClass)
                {
                    var one = _Entities.TbRegionsClasses.Where(x => x.Id == item.Id).FirstOrDefault();
                    one.IsActive = false;
                    _Entities.SaveChanges();
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult RegionEditPartial(string id)
        {
            RegionModel model = new RegionModel();
            long regionId = Convert.ToInt64(id);
            var data = _Entities.TbRegionss.Where(x => x.Id == regionId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.RegionName = data.RegionName;
                model.Id = data.Id;
                model.SchoolId = _user.SchoolId;
                model._ClassList = new List<ClassList>();
                var list = _Entities.TbRegionsClasses.Where(x => x.IsActive && x.RegionId == data.Id).Include(x => x.Class).ToList();
                foreach (var item in list)
                {
                    ClassList one = new ClassList();
                    one.ClassId = item.ClassId;
                    one.IsRegion = true;
                    one.ClassName = item.Class.Class;
                    model._ClassList.Add(one);
                }
                //var dataFull = _Entities.sp_ClassWithRegion(model.SchoolId).ToList();
                var dataFull= _Entities.sp_ClassWithRegion
        .FromSqlRaw("EXEC sp_ClassWithRegion {0}",
                   model.SchoolId)
        .ToList();


                var classData = _Entities.TbClasses
                        .Where(x => x.SchoolId == _user.SchoolId &&
                                    x.IsActive &&
                                    x.PublishStatus == true)
                        .ToList();
                var others = classData.Where(x => !dataFull.Any(z => z.ClassId == x.ClassId)).ToList();
                foreach (var item in others)
                {
                    ClassList one = new ClassList();
                    one.ClassId = item.ClassId;
                    one.IsRegion = false;
                    one.ClassName = item.Class;
                    model._ClassList.Add(one);
                }
            }
            return PartialView("~/Views/Progress/_pv_RegionEdit.cshtml", model);
        }

        public object EditRegion(string id)
        {
            bool status = false;
            string msg = "Failed";
            string[] splitData = id.Split('~');
            string regionName = splitData[0];
            string ClassListString = splitData[1];
            long regionId = Convert.ToInt64(splitData[2]);
            List<long> ClassList = ClassListString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(long.Parse)
                        .ToList();

            if (_Entities.TbRegionss.Any(x => x.RegionName == regionName.Trim() && x.SchoolId == _user.SchoolId && x.IsActive && x.Id != regionId))
            {
                msg = "Region already exists!";
            }
            else
            {
                var region = _Entities.TbRegionss.Where(x => x.IsActive && x.Id == regionId).FirstOrDefault();
                if (region.RegionName.Trim().ToUpper() != regionName.Trim().ToUpper())
                {
                    region.RegionName = regionName.Trim().ToUpper();
                    status = _Entities.SaveChanges() > 0;
                }
                else
                {
                    status = true;
                }
                if (status)
                {
                    var otherRegionClasses = _Entities.TbRegionsClasses.Where(x => x.RegionId == regionId && x.IsActive).ToList();
                    var noDeletedRegionClassess = otherRegionClasses.Where(x => !ClassList.Any(y => y == x.ClassId)).ToList();
                    foreach (var item in noDeletedRegionClassess)
                    {
                        var regionDeletedClass = _Entities.TbRegionsClasses.Where(x => x.Id == item.Id).FirstOrDefault();
                        regionDeletedClass.IsActive = false;
                        _Entities.SaveChanges();
                    }
                    foreach (var item in ClassList)
                    {
                        var classList = _Entities.TbRegionsClasses.Where(x => x.RegionId == regionId && x.ClassId == item).FirstOrDefault();
                        if (classList == null)
                        {
                            var add = new TbRegionsClass();
                            add.RegionId = regionId;
                            add.ClassId = item;
                            add.TimeStamp = CurrentTime;
                            add.IsActive = true;
                            _Entities.TbRegionsClasses.Add(add);
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            classList.IsActive = true;
                            _Entities.SaveChanges();
                        }
                    }
                    msg = "Successful";
                }
            }
            return Json(new { status = status, msg = msg });
        }


        public ActionResult VtoIXProgressCard(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true).FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive && (x.SpecificTerm == null || x.SpecificTerm == DeclredExamDetails.TermId)).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).OrderBy(x => x.OrderNo).ThenBy(x => x.TimeStamp).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId == StudentDetails.ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                model.SchoolId = StudentDetails.SchoolId;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.ExamId == allExamTerms[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().ToList();
                    bool haveComputerScience = false;
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076)) // Information Technology Without Code for VI to VIII
                    {
                        haveComputerScience = true;
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD

                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }

                    model.TermDataList = new List<TermList>();
                    int tern = 0;

                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In  progress card , after  scholastic results need the total 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null)
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                z.TotalScore = Math.Round(totScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive && x.Main.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark, MidpointRounding.AwayFromZero).ToString();
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Convert.ToString(mainFull.Score);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            #region Scholastic Total Claculation 
                            if (findSecond == 3)
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var pt = (Convert.ToDouble(one.scholasticList[0].TotalScore) * 5) / 40;
                                var secondTot = (pt + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore));
                                z1.TotalScore = Math.Floor(secondTot).ToString();
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    //var x11 = (Convert.ToDouble(x1.Mark) / 40) * 5;
                                    var x11 = Convert.ToDouble(x1.Mark)/ 40 * 5;
                                    var pt_mark = Math.Round(x11, MidpointRounding.AwayFromZero).ToString();//Code added by Gayathri(10/10/2023)modify 2.5 into 3
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    za.Mark = ((Convert.ToDouble(pt_mark) + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark))).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion Scholastic Total Claculation 
                        }

                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        //var returnExamresult = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive == true).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive == true).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco, MidpointRounding.AwayFromZero).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive == true).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive == true).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion COMPUTER SCIENCE 
                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch (Exception x)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                model.SchoolId = StudentDetails.SchoolId;
                try
                {
                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    model.Remark2 = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId2).Select(x => x.Remark).FirstOrDefault();//razi to get correct remark
                    if (StudentDetails.Dob != null)
                    {
                        //    model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                        //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();

                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var term2Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().Union(term2Subjects.Select(x => x.Subject).ToList()).ToList(); // Wants to list as the order of they created 
                    bool haveComputerScience = false;
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076))
                    {
                        haveComputerScience = true;
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;

                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0; // Need a field for total scolastic
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null) // For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);
                                z.TotalScore = Math.Round(totScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    //var mainFull = scholastics.Where(x => x.tb_ScolasticAreaResultDetails.FirstOrDefault().SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive && x.Main.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark, MidpointRounding.AwayFromZero).ToString();
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Math.Round(mainFull.Score, MidpointRounding.AwayFromZero).ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);

                            #region Scholastic Total Claculation 
                            if (findSecond == 3)
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                //Code added by gayathri:02/03/2023for term 2 progresscard
                                var pt = (Convert.ToDouble(one.scholasticList[0].TotalScore) * 5) / 40;
                                var secondTot = pt + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore);
                                z1.TotalScore = Math.Floor(secondTot).ToString();
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    //Code added by gayathri:02/03/2023for term 2 progresscard
                                    //var x11 = Math.Round((Convert.ToDouble(x1.Mark) / 40) * 5);
                                    var x11 = Convert.ToDouble(x1.Mark) / 40 * 5;
                                    var pt_mark = Math.Round(x11, MidpointRounding.AwayFromZero).ToString();//Code added by Gayathri(10/10/2023)modify 2.5 into 3
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    za.Mark = (Convert.ToDouble(pt_mark) + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark)).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion Scholastic Total Claculation 
                        }


                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        //var returnExamresult = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco, MidpointRounding.AwayFromZero).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId && x.DeclaredExamId == Item.Id).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();//10050
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 || x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                            else
                            {
                                var a = term2Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (result != null)
                                    {
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                        xx.TheoryTotal = a.TotalScore == 0 ? "0" : Convert.ToString(a.TotalScore);
                                        xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    }
                                    else
                                    {
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                        xx.TheoryTotal = "0";
                                        xx.PracticalTotal = "0";
                                    }

                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                        }
                        #endregion COMPUTER SCIENCE 
                    }
                    
                    #endregion Term II
                    model.IsOneTerm = false;
                    
                }
                
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                //Anual 
                return View();
            }
        }

        public ActionResult RemarkHome()
        {
            RemarkModel model = new RemarkModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }

        public object SubmitAddRemark(RemarkModel model)
        {
            string msg = "Failed";
            bool status = false;
            if (_Entities.TbRemarks.Any(x => x.SchoolId == _user.SchoolId && x.Remark.ToUpper().Trim() == model.Remark.ToUpper().Trim() && x.IsActive))
            {
                msg = "Already exists";
            }
            else
            {
                var remark = new TbRemark();
                remark.Remark = model.Remark;
                remark.SchoolId = _user.SchoolId;
                remark.IsActive = true;
                remark.Timestamp = CurrentTime;
                _Entities.TbRemarks.Add(remark);
                status = _Entities.SaveChanges() > 0;
            }
            if (status)
                msg = "Successfull";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult GetRemarkList()
        {
            var model = new RemarkModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Progress/_pv_RemarkList.cshtml", model);
        }
        public object DeleteRemark(string id)
        {
            bool status = false;
            string msg = "Faailed";
            long remarkId = Convert.ToInt64(id);
            var remark = _Entities.TbRemarks.Where(x => x.Id == remarkId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (remark != null)
            {
                remark.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            if (status)
                msg = "Deeted!";
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult RemarkEditPartial(string id)
        {
            RemarkModel model = new RemarkModel();
            model.SchoolId = _user.SchoolId;
            model.RemarkId = Convert.ToInt64(id);
            var data = _Entities.TbRemarks.Where(x => x.SchoolId == model.SchoolId && x.Id == model.RemarkId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Remark = data.Remark;
            }
            return PartialView("~/Views/Progress/_pv_RemarkEdit.cshtml", model);
        }

        public object EditRemark(RemarkModel model)
        {
            bool status = false;
            string msg = "Failed";
            var old = _Entities.TbRemarks.Where(x => x.Remark.ToUpper().Trim() == model.Remark.ToUpper().Trim() && x.Id != model.RemarkId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (old != null)
            {
                msg = "Already we have the same Remark!";
            }
            else
            {
                var data = _Entities.TbRemarks.Where(x => x.Id == model.RemarkId).FirstOrDefault();
                if (data.Remark.ToUpper().Trim() != model.Remark.ToUpper().Trim())
                {
                    data.Remark = model.Remark.Trim();
                    status = _Entities.SaveChanges() > 0;
                }
                else
                {
                    status = true;
                }
            }
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }

        [HttpPost]
        public object GetAllRemarks()
        {
            long schoolId = _user.SchoolId;
            var result = new Satluj_Latest.Data.WebsiteService().gtGetAllRemarks(schoolId);
            return Json(new { Status = true, Message = "", result = result });
        }

        public ActionResult IXProgressCard(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true).FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive && (x.SpecificTerm == null || x.SpecificTerm == DeclredExamDetails.TermId)).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).OrderBy(x => x.OrderNo).ThenBy(x => x.TimeStamp).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId == StudentDetails.ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.ExamId == allExamTerms[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList();
                    bool haveComputerScience = false;
                    bool havemarketing = false;
                    
                    //if (allSubjects.Any(x => x.SubId == 20084)) // Information Technology with Code for IX
                    //{
                    //    haveComputerScience = true;              //10050
                    //}

                    if (allSubjects.Any(x => x.SubId == 20076 || x.SubId == 20084 || x.SubId == 20092)) // Information Technology with Code for IX
                    {
                        if (StudentDetails.DivisionId == 20650 || StudentDetails.DivisionId == 20914) //Check TS ELIOT have only one optional subject(Marketing and sales)--20914 added by dhanu on Oct 7 2025
                        {
                            haveComputerScience = false;

                        }
                        else
                        {
                            haveComputerScience = true;

                        }
                    }

                    if (allSubjects.Any(x => x.SubId == 20127)) // Marketing & sales with Code for IX
                    {
                        if (StudentDetails.DivisionId == 20650 || StudentDetails.DivisionId ==20914)
                        {
                            havemarketing = true;           //only TS ELIOT have Marketting and sales  

                        }
                        else
                        {
                            havemarketing = false;

                        }
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20084).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In IX progress card , after two scholastic results need scholastic total 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null)
                            {
                                if (data.ItemName == "PT")
                                {
                                    var totScore = Convert.ToDecimal(((data.TotalScore / data.DividedBy) / 40) * 5);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                    z.TotalScore = Math.Round(totScore).ToString();
                                }
                                else
                                {
                                    var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                    z.TotalScore = Math.Round(totScore).ToString();
                                }

                            }

                            else
                            {
                                if (data.ItemName == "PT")
                                {

                                    z.TotalScore = Math.Round((data.TotalScore / 40) * 5).ToString();//Razi
                                }
                                else
                                    z.TotalScore = Math.Round(data.TotalScore).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive && x.Main.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark).ToString("F0");
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Convert.ToString(mainFull.Score);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            #region Scholastic Total
                            if (findSecond == 3)// For Scholastic Total
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var secondTot = Convert.ToDouble(one.scholasticList[0].TotalScore) + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore);
                                z1.TotalScore = Math.Round(secondTot, 1).ToString("F0");
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x5 = Math.Round((Convert.ToDouble(x1.Mark) * 5) / 40).ToString(); //Razi changed 3 to 5
                                    za.Mark = Math.Floor(((Convert.ToDouble(x5) + Math.Round(Convert.ToDouble(x2.Mark)) + Math.Round(Convert.ToDouble(x3.Mark)) + Math.Round(Convert.ToDouble(x4.Mark))))).ToString();//Razi changed x5 yo x4 and x1 to x5
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                            sco = 80; //Add 80 mark for theory by Gayathri A(07/10/2023)
                        }
                        else
                        {
                            sco = 80; //Add 80 mark for theory by Gayathri A(07/10/2023)

                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                        #region MARKETING & SALES Created by Gayathri A(4/10/2023)
                        if (havemarketing == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20127 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//Razi on 28/03/22
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//Razi on 28/03/22
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion COMPUTER SCIENCE 


                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.TermId == 2).ToList().OrderByDescending(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();

                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().OrderBy(x => x.TmeStamp).ToList();// Order should be created
                    bool haveComputerScience = false;
                    bool havemarketing = false;
                    if (StudentDetails.StudentId == 40849 || StudentDetails.StudentId == 40850)
                    {
                        if (allSubjects.Any(x => x.SubId == 20076)) // Information Technology with Code 
                        {

                            haveComputerScience = true;


                        }
                    }
                    if (allSubjects.Any(x => x.SubId == 20127)) // Marketing & sales with Code for IX
                    {
                        if (StudentDetails.DivisionId == 20650 || StudentDetails.DivisionId == 20769 || StudentDetails.DivisionId == 20914)
                        {
                            havemarketing = true;           //only TS ELIOT have Marketting and sales  

                        }
                        else
                        {
                            havemarketing = false;

                        }
                    }
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076 || x.SubId == 20084 || x.SubId == 20092)) // Information Technology with Code 
                    {
                        if (StudentDetails.DivisionId == 20650 || StudentDetails.DivisionId == 20769|| StudentDetails.DivisionId ==20914) //Check TS ELIOT have only one optional subject(Marketing and sales)
                        {
                            haveComputerScience = false;

                        }
                        else
                        {
                            haveComputerScience = true;

                        }
                    }
                    //added by dhanu only these students are IT in tennyson division 28/03/2025
                    if (StudentDetails.StudentId == 40849 && StudentDetails.DivisionId == 20769 || StudentDetails.StudentId == 40850 && StudentDetails.DivisionId == 20769)
                    {
                        haveComputerScience = true;
                        havemarketing = false;

                    }



                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        // Error identifidde....19-Feb-2021.....

                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In IX progress card , after two scholastic results need the average ,For that its used to check the after second section 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null) // For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);
                                z.TotalScore = Math.Round(totScore).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    //var mainFull = scholastics.Where(x => x.tb_ScolasticAreaResultDetails.FirstOrDefault().SubjectId == sub.SubId && x.IsActive).FirstOrDefault();

                                    ///////////This line to test...........05-03-2020

                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive && x.Main.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark).ToString("F0");
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Math.Round(mainFull.Score).ToString("F0");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            //#region PT Average
                            //if (findSecond == 1)
                            //{
                            //    ScolasticAreaList z1 = new ScolasticAreaList();
                            //    z1.ScolasticArea = "PT.Avg";
                            //    var secondTot = (Convert.ToDouble(one.scholasticList[0].TotalScore) + Convert.ToDouble(one.scholasticList[1].TotalScore)) / 2;
                            //    z1.TotalScore = Math.Floor(secondTot).ToString();
                            //    z1.subjectList = new List<SubjectDetails>();
                            //    foreach (var sub in allSubjects)
                            //    {
                            //        SubjectDetails za = new SubjectDetails();
                            //        if (sub.Code != null && sub.Code != string.Empty)
                            //            za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            //        else
                            //            za.SubjectName = sub.SubjectName;
                            //        var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                            //        var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                            //        za.Mark = Math.Floor(((Convert.ToDouble(x1.Mark) + Convert.ToDouble(x2.Mark)) / 2)).ToString();
                            //        z1.subjectList.Add(za);
                            //    }
                            //    one.scholasticList.Add(z1);
                            //    findSecond = findSecond + 1;
                            //}
                            if (findSecond == 5)// For Scholastic Total
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var secondTot = (Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore) + Convert.ToDouble(one.scholasticList[4].TotalScore) + Convert.ToDouble(one.scholasticList[5].TotalScore));
                                z1.TotalScore = Math.Round(secondTot, 1).ToString("F0");
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x2 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[4].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[5].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();


                                    za.Mark = Math.Floor(((Convert.ToDouble(x1.Mark) + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark)))).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            //#endregion PT Average
                            findSecond = findSecond + 1;
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                            sco = 80; //Add 80 mark for theory by Gayathri A(07/10/2023)
                        }
                        else
                        {
                            sco = 80; //Add 80 mark for theory by Gayathri A(07/10/2023)

                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId && x.DeclaredExamId == Item.Id).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                        #region MARKETING & SALES Created by Gayathri A(4/10/2023)
                        if (havemarketing == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20127 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//Razi on 28/03/22
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion
                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();//10050
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    //xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//This is changed to convert PT marks out of 50 - Razi 28/03/2022
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                            else
                            {
                            }
                        }
                        #endregion COMPUTER SCIENCE 


                    }
                    #endregion Term II
                    model.IsOneTerm = false;
                }
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                //Anual 
                return View();
            }
        }

        public IActionResult HigherSecondaryProgressCard(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive && x.TbExamBook.IsActive && x.TbExamBook.Term.IsActive && x.Class.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.Exam.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true).FirstOrDefault();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive && (x.ClassId == StudentDetails.ClassId || x.ClassId == null)).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1 && x.IsActive).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId == StudentDetails.ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolId = _user.SchoolId;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                try
                {

                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph: " + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId && x.IsActive).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId && x.IsActive).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;
                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();
                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().ToList();// The Subjects wants to list in the order of them created 
                    bool havedance = false;
                    bool havelegal = false;
                    if (allSubjects.Any(x => x.SubId == 20133)) // dance for XI added by dhanu 06/10/2025
                    {
                        if (StudentDetails.DivisionId == 20889 || StudentDetails.DivisionId == 20893 || StudentDetails.DivisionId == 20887 || StudentDetails.DivisionId == 20888 || StudentDetails.DivisionId == 20892 || StudentDetails.DivisionId == 20886 || StudentDetails.DivisionId == 20890 || StudentDetails.DivisionId == 20891)
                        {
                            havedance = true;

                        }
                        else
                        {
                            havedance = false;

                        }
                    }
                    if (allSubjects.Any(x => x.SubId == 20131)) // LegalStudies for XI added by dhanu 06/10/2025
                    {
                        if (StudentDetails.DivisionId == 20889 || StudentDetails.DivisionId == 20893 || StudentDetails.DivisionId == 20887 || StudentDetails.DivisionId == 20888 || StudentDetails.DivisionId == 20892 || StudentDetails.DivisionId == 20886 || StudentDetails.DivisionId == 20890 || StudentDetails.DivisionId == 20891)
                        {
                            havelegal = true;

                        }
                        else
                        {
                            havelegal = false;

                        }
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty )
                            {
                                if (sub.SubId == 20128)//Marketing (812)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if(sub.SubId == 20129) //Food Nutrition and Dietetics (834)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if(sub.SubId == 20130) //Mass Media Studies (835)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20133) //Dance(Kathak)(056)  added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20132) //Psychology(037)   added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20131) //Legal Studies(074)    added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else
                                {
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";


                                }

                            }
                            else
                            {
                                a.SubjectName = sub.SubjectName;

                            }
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                {
                                    a.Mark = xxx.StudentScore.ToString() == null ? "0" : xxx.StudentScore.ToString();
                                    a.PracticalScore = xxx.PracticalScore.ToString() == null ? "0" : xxx.PracticalScore.ToString();
                                }
                                else
                                {
                                    a.Mark = "0";
                                    a.PracticalScore = "0";
                                }
                                a.MarkTotal = subjectIdFromDeclaredExam.TotalScore.ToString();
                                a.PracticalScoreTotal = (100 - subjectIdFromDeclaredExam.TotalScore).ToString();//changed by razi from 100 to 50 on 16/03/2022
                            }
                            else
                            {
                                a.Mark = "0";
                                a.PracticalScore = "0";
                                a.MarkTotal = "0";
                                a.PracticalScoreTotal = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                        #region DANCE Created by Dhanu(6/10/2025)
                        if (havedance == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20133 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion
                        #region LEGAL STUDIES Created by Dhanu (6/10/2025)
                        if (havelegal == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20131 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch (Exception x)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolId = _user.SchoolId;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                try
                {
                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == StudentDetails.ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    model.Remark2 = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId2).Select(x => x.Remark).FirstOrDefault();//razi 29/03
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + "-" + StudentDetails.Division.Division;
                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();
                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();
                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var term2Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var allSubjects = term1Subjects.Select(x => x.TbSubjectss).ToList().Union(term2Subjects.Select(x => x.TbSubjectss).ToList());
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().OrderBy(x => x.TmeStamp).ToList();// Order should be created
                    bool havedance = false;
                    bool havelegal = false;
                    if (allSubjects.Any(x => x.SubId == 20133)) // dance for XI added by dhanu 06/10/2025
                    {
                        if (StudentDetails.DivisionId == 20889 || StudentDetails.DivisionId == 20893 || StudentDetails.DivisionId == 20887 || StudentDetails.DivisionId == 20888 || StudentDetails.DivisionId == 20892 || StudentDetails.DivisionId == 20886 || StudentDetails.DivisionId == 20890 || StudentDetails.DivisionId == 20891)
                        {
                            havedance = true;

                        }
                        else
                        {
                            havedance = false;

                        }
                    }
                    if (allSubjects.Any(x => x.SubId == 20131)) // LegalStudies for XI added by dhanu 06/10/2025
                    {
                        if (StudentDetails.DivisionId == 20889 || StudentDetails.DivisionId == 20893 || StudentDetails.DivisionId == 20887 || StudentDetails.DivisionId == 20888 || StudentDetails.DivisionId == 20892 || StudentDetails.DivisionId == 20886 || StudentDetails.DivisionId == 20890 || StudentDetails.DivisionId == 20891)
                        {
                            havelegal = true;

                        }
                        else
                        {
                            havelegal = false;

                        }
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            //if (sub.Code != null && sub.Code != string.Empty)
                            //    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            //else
                            //    a.SubjectName = sub.SubjectName;
                            if (sub.Code != null && sub.Code != string.Empty)
                            {
                                if (sub.SubId == 20128)//Marketing (812)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20129) //Food Nutrition and Dietetics (834)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20130) //Mass Media Studies (835)
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20133) //Dance(Kathak)(056)  added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20132) //Psychology(037)   added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else if (sub.SubId == 20131) //Legal Studies(074)    added by dhanu 28/09/2025
                                {
                                    a.SubjectName = sub.SubjectName;

                                }
                                else
                                {
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";


                                }
                            }
                            else
                            {
                                a.SubjectName = sub.SubjectName;

                            }
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                {
                                    a.Mark = xxx.StudentScore.ToString() == null ? "0" : xxx.StudentScore.ToString();
                                    a.PracticalScore = xxx.PracticalScore.ToString() == null ? "0" : xxx.PracticalScore.ToString();
                                }
                                else
                                {
                                    a.Mark = "0";
                                    a.PracticalScore = "0";
                                }
                                a.MarkTotal = subjectIdFromDeclaredExam.TotalScore.ToString();
                                a.PracticalScoreTotal = (100 - subjectIdFromDeclaredExam.TotalScore).ToString();//changed by Gayathri from 50 to 100 on 02/03/2023
                            }
                            else
                            {
                                a.Mark = "0";
                                a.PracticalScore = "0";
                                a.MarkTotal = "0";
                                a.PracticalScoreTotal = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                        #region DANCE Created by Dhanu(6/10/2025)
                        if (havedance == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20133 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion
                        #region LEGAL STUDIES Created by Dhanu (6/10/2025)
                        if (havelegal == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20131 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion Term II
                    model.IsOneTerm = false;
                }
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                return View();
            }
        }
        public PartialViewResult ViewStudentsTotalScoreViewPage(string id)
        {
            StudentExamTotalSccore model = new StudentExamTotalSccore();
            string[] splitData = id.Split('~');
            long examId = Convert.ToInt64(splitData[0]);
            long classId = Convert.ToInt64(splitData[1]);
            long divisionId = Convert.ToInt64(splitData[2]);
            model.SchoolIdModel = _user.SchoolId;
            model.ExamIdModel = examId;
            model.ClassIdModel = classId;
            model.DivisionIdModel = divisionId;
            model.list = new List<StudentListForTotalSccore>();
            var students = _Entities.TbStudents.Where(x => x.ClassId == model.ClassIdModel && x.DivisionId == model.DivisionIdModel && x.SchoolId == model.SchoolIdModel && x.IsActive).ToList();
            var data = _Entities.TbStudentTotalScores.Where(x => x.SchoolId == model.SchoolIdModel && x.ClassId == model.ClassIdModel && x.DivisionId == model.DivisionIdModel && x.ExamId == model.ExamIdModel && x.IsActive && x.Student.IsActive == true).ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var item in students)
                {
                    StudentListForTotalSccore one = new StudentListForTotalSccore();
                    var havedata = data.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                    if (havedata != null)
                    {
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        one.MarkObtained = havedata.TotalScore;
                        one.MarkPercentage = havedata.Percentage;
                        model.list.Add(one);
                    }
                    else
                    {
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        one.MarkObtained = 0;
                        one.MarkPercentage = 0;
                        model.list.Add(one);
                    }
                }
            }
            else
            {
                foreach (var item in students)
                {
                    StudentListForTotalSccore one = new StudentListForTotalSccore();
                    one.StudentId = item.StudentId;
                    one.StudentName = item.StundentName;
                    //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                    one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                    one.MarkObtained = 0;
                    one.MarkPercentage = 0;
                    model.list.Add(one);
                }
            }
            return PartialView("~/Views/Progress/_pv_FullStudentTotalScore.cshtml", model);
        }

        public object SubmitStudentsTotalScoreAndPercentage(SchoolValue.StudentExamTotalSccore model)
        {
            string msg = "Failed";
            bool status = false;
            var olddata = _Entities.TbStudentTotalScores.Where(x => x.ClassId == model.ClassIdModel && x.DivisionId == model.DivisionIdModel && x.ExamId == model.ExamIdModel && x.SchoolId == model.SchoolIdModel && x.IsActive && x.Student.IsActive).ToList();
            if (olddata != null && olddata.Count > 0)
            {
                foreach (var item in model.list)
                {
                    var one = olddata.Where(x => x.StudentId == item.StudentId).FirstOrDefault();
                    if (one != null)
                    {
                        one.TotalScore = item.MarkObtained;
                        one.Percentage = item.MarkPercentage;
                        status = _Entities.SaveChanges() > 0;
                    }
                    else
                    {
                        if (item.MarkObtained != 0 && item.MarkPercentage != 0)
                        {
                            var newData = new TbStudentTotalScore();
                            newData.StudentId = item.StudentId;
                            newData.ClassId = model.ClassIdModel;
                            newData.DivisionId = model.DivisionIdModel;
                            newData.SchoolId = model.SchoolIdModel;
                            newData.ExamId = model.ExamIdModel;
                            newData.TotalScore = item.MarkObtained;
                            newData.Percentage = item.MarkPercentage;
                            newData.IsActive = true;
                            newData.TimeStamp = CurrentTime;
                            _Entities.TbStudentTotalScores.Add(newData);
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in model.list)
                {
                    var newData = new TbStudentTotalScore();
                    newData.StudentId = item.StudentId;
                    newData.ClassId = model.ClassIdModel;
                    newData.DivisionId = model.DivisionIdModel;
                    newData.SchoolId = model.SchoolIdModel;
                    newData.ExamId = model.ExamIdModel;
                    newData.TotalScore = item.MarkObtained;
                    newData.Percentage = item.MarkPercentage;
                    newData.IsActive = true;
                    newData.TimeStamp = CurrentTime;
                    _Entities.TbStudentTotalScores.Add(newData);
                    status = _Entities.SaveChanges() > 0;
                }
            }
            status = true;
            msg = "Successful";
            return Json(new { status = status, msg = msg });
        }

        public ActionResult PreviousyearCertificate()
        {
            ProgressCardModel model = new ProgressCardModel();
            model.SchoolId = _user.SchoolId;
            model.ProgressCardName = _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            //DropdownData dropdown = new DropdownData();
            model.AcademicYearList = _dropdown.GetAllAcademicYear();
            return View(model);
        }



        public PartialViewResult GetPrevious_StudentList(string id)
        {
            string[] splitData = id.Split('~');
            long classId = Convert.ToInt64(splitData[0]);
            long examDeclaredId = Convert.ToInt64(splitData[1]);
            long DivisionId = Convert.ToInt64(splitData[2]); ;
            StudentListForProgressCardModel model = new StudentListForProgressCardModel();
            model.Class = new Satluj_Latest.Data.Class(classId).ClassName;
            model.ClassId = classId;
            model.DivisionId= DivisionId;
            var studentLis = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == classId && x.DivisionId == DivisionId && x.IsActive).ToList();
            if ((model.Class == "I" )|| (model.Class == "II") || (model.Class == "III") || (model.Class == "IV") || (model.Class=="VI")
              ||  (model.Class=="VII") || (model.Class =="VIII")|| (model.Class =="IX"))
            {
                //var StudentLst = _Entities.SP_PreviousYear_VtoXII(_user.SchoolId, examDeclaredId).ToList();

               var StudentLst= _Entities.SP_PreviousYear_VtoXII_Results
         .FromSqlRaw("EXEC SP_PreviousYear_VtoXII {0}, {1}",
                     _user.SchoolId, examDeclaredId)
         .ToList();


                if (StudentLst != null && StudentLst.Count > 0)
                {
                    model.ExamId = examDeclaredId;
                    model.SchoolId = _user.SchoolId;
                    model._list = new List<StudentListWithExam>();
                    foreach (var item in StudentLst)
                    {
                        StudentListWithExam one = new StudentListWithExam();
                        one.StudnetId = item.StudentId;
                        one.StudnetName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        model._list.Add(one);
                    }
                }
            }
            else if(model.Class == "V")
            {
                //var StudentLst = _Entities.SP_PreviousYear_V(_user.SchoolId, DivisionId).ToList();
                var StudentLst= _Entities.SP_PreviousYear_V_Result
        .FromSqlRaw("EXEC SP_PreviousYear_V {0}, {1}",
                    _user.SchoolId, DivisionId)
        .ToList();


                if (StudentLst != null && StudentLst.Count > 0)
                {
                    model.ExamId = examDeclaredId;
                    model.SchoolId = _user.SchoolId;
                    model._list = new List<StudentListWithExam>();
                    foreach (var item in StudentLst)
                    {
                        StudentListWithExam one = new StudentListWithExam();
                        one.StudnetId = item.StudentId;
                        one.StudnetName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        model._list.Add(one);
                    }
                }

            }
            else if((model.Class == "XI SCIENCE") ||( model.Class == "XI COMMERCE") || (model.Class == "XI HUMANITIES") ||
                (model.Class == "XII SCIENCE") || model.Class == ("XII COMMERCE" )|| model.Class == ("XII HUMANITIES"))
            {
                //var StudentLst = _Entities.SP_PreviousYear_XItoXII(_user.SchoolId, examDeclaredId).ToList();
                var StudentLst= _Entities.SP_PreviousYear_XItoXII_Result
        .FromSqlRaw("EXEC SP_PreviousYear_XItoXII {0}, {1}, {2}",
                    _user.SchoolId, examDeclaredId)
        .ToList();


                if (StudentLst != null && StudentLst.Count > 0)
                {
                    model.ExamId = examDeclaredId;
                    model.SchoolId = _user.SchoolId;
                    model._list = new List<StudentListWithExam>();
                    foreach (var item in StudentLst)
                    {
                        StudentListWithExam one = new StudentListWithExam();
                        one.StudnetId = item.StudentId;
                        one.StudnetName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        model._list.Add(one);
                    }
                }
            }
            else if(model.Class == "X")
            {
                //var StudentLst = _Entities.SP_PreviousYear_X(_user.SchoolId, examDeclaredId).ToList();
                var StudentLst= _Entities.SP_PreviousYear_X_Result
        .FromSqlRaw("EXEC SP_PreviousYear_X {0}, {1}, {2}",
                    _user.SchoolId, examDeclaredId)
        .ToList();


                if (StudentLst != null && StudentLst.Count > 0)
                {
                    model.ExamId = examDeclaredId;
                    model.SchoolId = _user.SchoolId;
                    model._list = new List<StudentListWithExam>();
                    foreach (var item in StudentLst)
                    {
                        StudentListWithExam one = new StudentListWithExam();
                        one.StudnetId = item.StudentId;
                        one.StudnetName = item.StundentName;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        model._list.Add(one);
                    }
                }

            }
           
            model._list = model._list.OrderBy(x => x.RollNo).ThenBy(x => x.StudnetName).ToList();
            return PartialView("~/Views/Progress/_pv_PreviousYear_VtoX11.cshtml", model);
        }

        public ActionResult VtoIXProgressCard_Previous(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var Classid = Convert.ToInt64(splitData[2]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true && x.ClassId== Classid).FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && (x.SpecificTerm == null || x.SpecificTerm == DeclredExamDetails.TermId) && x.TotalScore !=0).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive ==true).OrderBy(x => x.OrderNo).ThenBy(x => x.TimeStamp).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId == Classid && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                model.SchoolId = StudentDetails.SchoolId;
                model.ClassId = Classid;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    //model.ClassDivision = StudentDetails.tb_Class.Class + "-" + StudentDetails.tb_Division.Division;
                    model.ClassDivision = _Entities.TbClasses.Where(x => x.ClassId == Classid && x.IsActive).Select(x => x.Class).FirstOrDefault();

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == Classid && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.ExamId == allExamTerms[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().ToList();
                    bool haveComputerScience = false;
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076)) // Information Technology Without Code for VI to VIII
                    {
                        haveComputerScience = true;
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD

                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }

                    model.TermDataList = new List<TermList>();
                    int tern = 0;

                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In  progress card , after  scholastic results need the total 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x =>x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null)
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                z.TotalScore = Math.Round(totScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark, MidpointRounding.AwayFromZero).ToString();
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Convert.ToString(mainFull.Score);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            #region Scholastic Total Claculation 
                            if (findSecond == 3)
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var pt = (Convert.ToDouble(one.scholasticList[0].TotalScore) * 5) / 40;
                                var secondTot = (pt + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore));
                                z1.TotalScore = Math.Floor(secondTot).ToString();
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    //var x11 = (Convert.ToDouble(x1.Mark) / 40) * 5;
                                    var x11 = Math.Round((Convert.ToDouble(x1.Mark) / 40) * 5);
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    za.Mark = ((x11 + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark))).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion Scholastic Total Claculation 
                        }

                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        //var returnExamresult = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive == true).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive == true).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco, MidpointRounding.AwayFromZero).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive == true).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive == true).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion COMPUTER SCIENCE 
                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch (Exception x)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                model.SchoolId = StudentDetails.SchoolId;
                model.ClassId = Classid;
                try
                {
                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == Classid && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == Classid && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    model.Remark2 = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId2).Select(x => x.Remark).FirstOrDefault();//razi to get correct remark
                    if (StudentDetails.Dob != null)
                    {
                        //    model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                        //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = _Entities.TbClasses.Where(x => x.ClassId == Classid && x.IsActive).Select(x => x.Class).FirstOrDefault();

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == Classid && x.IsActive && x.SchoolId == StudentDetails.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();

                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var term2Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().Union(term2Subjects.Select(x => x.Subject).ToList()).OrderBy(x => x.TmeStamp).ToList(); // Wants to list as the order of they created 
                    bool haveComputerScience = false;
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076))
                    {
                        haveComputerScience = true;
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;

                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0; // Need a field for total scolastic
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null) // For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);
                                z.TotalScore = Math.Round(totScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore, MidpointRounding.AwayFromZero).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    //var mainFull = scholastics.Where(x => x.tb_ScolasticAreaResultDetails.FirstOrDefault().SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark, MidpointRounding.AwayFromZero).ToString();
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Math.Round(mainFull.Score, MidpointRounding.AwayFromZero).ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);

                            #region Scholastic Total Claculation 
                            if (findSecond == 3)
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                //Code added by gayathri:02/03/2023for term 2 progresscard
                                var pt = (Convert.ToDouble(one.scholasticList[0].TotalScore) * 5) / 40;
                                //var secondTot = pt + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore);
                                var secondTot = Convert.ToDouble(pt) + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore);
                                z1.TotalScore = Math.Floor(secondTot).ToString();
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    //Code added by gayathri:02/03/2023for term 2 progresscard
                                    var x11 = Math.Round((Convert.ToDouble(x1.Mark) / 40) * 5, MidpointRounding.AwayFromZero);
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    za.Mark = (Convert.ToDouble(x11) + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark)).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion Scholastic Total Claculation 
                        }


                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        //var returnExamresult = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.IsActive).ToList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco, MidpointRounding.AwayFromZero).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId && x.DeclaredExamId == Item.Id).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();//10050
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 || x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                            else
                            {
                                var a = term2Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (result != null)
                                    {
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                        xx.TheoryTotal = a.TotalScore == 0 ? "0" : Convert.ToString(a.TotalScore);
                                        xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    }
                                    else
                                    {
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                        xx.TheoryTotal = "0";
                                        xx.PracticalTotal = "0";
                                    }

                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                        }
                        #endregion COMPUTER SCIENCE 
                    }

                    #endregion Term II
                    model.IsOneTerm = false;

                }

                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                //Anual 
                return View();
            }
        }

        public ActionResult IXProgressCard_Previous(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var ClassId = Convert.ToInt64(splitData[2]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true && x.ClassId==ClassId).FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && (x.SpecificTerm == null || x.SpecificTerm == DeclredExamDetails.TermId)).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId).OrderBy(x => x.OrderNo).ThenBy(x => x.TimeStamp).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId ==ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    //model.ClassDivision = StudentDetails.tb_Class.Class + "-" + StudentDetails.tb_Division.Division;
                    model.ClassDivision = _Entities.TbClasses.Where(x => x.ClassId == ClassId && x.IsActive).Select(x => x.Class).FirstOrDefault();

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.ExamId == allExamTerms[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList();
                    bool haveComputerScience = false;
                    //if (allSubjects.Any(x => x.SubId == 20084)) // Information Technology with Code for IX
                    //{
                    //    haveComputerScience = true;              //10050
                    //}

                    if (allSubjects.Any(x => x.SubId == 20076)) // Information Technology with Code for IX
                    {
                        haveComputerScience = true;              //10050,20084
                    }

                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20084).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In IX progress card , after two scholastic results need scholastic total 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.IsActive && x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null)
                            {
                                if (data.ItemName == "PT")
                                {
                                    var totScore = Convert.ToDecimal(((data.TotalScore / data.DividedBy) / 40) * 5);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                    z.TotalScore = Math.Round(totScore).ToString();
                                }
                                else
                                {
                                    var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);// For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                                    z.TotalScore = Math.Round(totScore).ToString();
                                }

                            }

                            else
                            {
                                if (data.ItemName == "PT")
                                {

                                    z.TotalScore = Math.Round((data.TotalScore / 40) * 5).ToString();//Razi
                                }
                                else
                                    z.TotalScore = Math.Round(data.TotalScore).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark).ToString("F0");
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Convert.ToString(mainFull.Score);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            #region Scholastic Total
                            if (findSecond == 3)// For Scholastic Total
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var secondTot = Convert.ToDouble(one.scholasticList[0].TotalScore) + Convert.ToDouble(one.scholasticList[1].TotalScore) + Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore);
                                z1.TotalScore = Math.Round(secondTot, 1).ToString("F0");
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x5 = Math.Round((Convert.ToDouble(x1.Mark) * 5) / 40).ToString(); //Razi changed 3 to 5
                                    za.Mark = Math.Floor(((Convert.ToDouble(x5) + Math.Round(Convert.ToDouble(x2.Mark)) + Math.Round(Convert.ToDouble(x3.Mark)) + Math.Round(Convert.ToDouble(x4.Mark))))).ToString();//Razi changed x5 yo x4 and x1 to x5
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            findSecond = findSecond + 1;
                            #endregion
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//Razi on 28/03/22
                                    model.csList.Add(xx);
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                    model.csList.Add(xx);
                                }
                            }
                        }
                        #endregion COMPUTER SCIENCE 

                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                model.RegionId = RegionDetails.Region.Id;
                try
                {
                    model.StudentId = studentId;
                    model.ExamId = declaredExamId;
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = _Entities.TbClasses.Where(x=>x.ClassId==ClassId && x.IsActive==true).Select(x=>x.Class).FirstOrDefault();

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.TermId == 2).ToList().OrderByDescending(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();

                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    //var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();

                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var term2Subjects = _Entities.TbDeclaredExamsubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().OrderBy(x => x.TmeStamp).ToList();// Order should be created
                    bool haveComputerScience = false;
                    if (allSubjects.Any(x => x.SubId == 10050 || x.SubId == 20076)) // Information Technology with Code 
                    {
                        haveComputerScience = true;
                    }
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        // Error identifidde....19-Feb-2021.....

                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {

                        }
                        else
                        {
                            allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        int findSecond = 0;// In IX progress card , after two scholastic results need the average ,For that its used to check the after second section 
                        foreach (var data in ScholasticData)
                        {
                            var scholastics = Item.TbScholasticResultMains.Where(x => x.ExamId == Item.Id && x.StudentId == studentId && x.ScholasticId == data.Id).ToList();
                            ScolasticAreaList z = new ScolasticAreaList();
                            z.ScolasticArea = data.ItemName;
                            if (data.DividedBy != 0 && data.DividedBy != null) // For the Periodic Test , the mark total is out of40 and the report will shown with divided by 4
                            {
                                var totScore = Convert.ToDecimal(data.TotalScore / data.DividedBy);
                                z.TotalScore = Math.Round(totScore).ToString();
                            }
                            else
                            {
                                z.TotalScore = Math.Round(data.TotalScore).ToString();
                            }
                            z.subjectList = new List<SubjectDetails>();
                            foreach (var sub in allSubjects)
                            {
                                SubjectDetails a = new SubjectDetails();
                                if (sub.Code != null && sub.Code != string.Empty)
                                    a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                else
                                    a.SubjectName = sub.SubjectName;
                                if (scholastics != null)
                                {
                                    //var main = scholastics.tb_ScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId).Select(x => x.Score).FirstOrDefault();
                                    //var mainFull = scholastics.Where(x => x.tb_ScolasticAreaResultDetails.FirstOrDefault().SubjectId == sub.SubId && x.IsActive).FirstOrDefault();

                                    ///////////This line to test...........05-03-2020

                                    var mainFull = _Entities.TbScolasticAreaResultDetails.Where(x => x.SubjectId == sub.SubId && x.Main.ExamId == Item.Id && x.Main.StudentId == studentId && x.Main.ScholasticId == data.Id && x.IsActive).FirstOrDefault();
                                    if (mainFull != null)
                                    {
                                        if (mainFull.Score == 0)
                                        {
                                            a.Mark = "0";
                                        }
                                        else
                                        {
                                            if (mainFull.Main.Scholastic.DividedBy != null && mainFull.Main.Scholastic.DividedBy != 0) // For Periodic Test , Mark enter in 40 and the marks wants to divided by 4 , For that we adjusting the table with 4
                                            {
                                                decimal originalmark = Convert.ToDecimal(mainFull.Score / mainFull.Main.Scholastic.DividedBy);
                                                //a.Mark = Convert.ToString(originalmark);
                                                a.Mark = Math.Round(originalmark).ToString("F0");
                                            }
                                            else
                                            {
                                                //a.Mark = Convert.ToString(mainFull.Score);
                                                a.Mark = Math.Round(mainFull.Score).ToString("F0");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        a.Mark = "0";
                                    }
                                }
                                else
                                {
                                    a.Mark = "0";
                                }
                                //a.Mark = _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString() == null ? "0" : _Entities.tb_ScolasticAreaResultDetails.Where(x => x.MainId == data.Id && x.SubjectId == sub.SubId).Select(x => (x.Score)).FirstOrDefault().ToString();
                                z.subjectList.Add(a);
                            }
                            one.scholasticList.Add(z);
                            //#region PT Average
                            //if (findSecond == 1)
                            //{
                            //    ScolasticAreaList z1 = new ScolasticAreaList();
                            //    z1.ScolasticArea = "PT.Avg";
                            //    var secondTot = (Convert.ToDouble(one.scholasticList[0].TotalScore) + Convert.ToDouble(one.scholasticList[1].TotalScore)) / 2;
                            //    z1.TotalScore = Math.Floor(secondTot).ToString();
                            //    z1.subjectList = new List<SubjectDetails>();
                            //    foreach (var sub in allSubjects)
                            //    {
                            //        SubjectDetails za = new SubjectDetails();
                            //        if (sub.Code != null && sub.Code != string.Empty)
                            //            za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            //        else
                            //            za.SubjectName = sub.SubjectName;
                            //        var x1 = one.scholasticList[0].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                            //        var x2 = one.scholasticList[1].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                            //        za.Mark = Math.Floor(((Convert.ToDouble(x1.Mark) + Convert.ToDouble(x2.Mark)) / 2)).ToString();
                            //        z1.subjectList.Add(za);
                            //    }
                            //    one.scholasticList.Add(z1);
                            //    findSecond = findSecond + 1;
                            //}
                            if (findSecond == 3)// For Scholastic Total
                            {
                                ScolasticAreaList z1 = new ScolasticAreaList();
                                z1.ScolasticArea = "Total";
                                var secondTot = (Convert.ToDouble(one.scholasticList[2].TotalScore) + Convert.ToDouble(one.scholasticList[3].TotalScore) + Convert.ToDouble(one.scholasticList[4].TotalScore) + Convert.ToDouble(one.scholasticList[5].TotalScore));
                                z1.TotalScore = Math.Round(secondTot, 1).ToString("F0");
                                z1.subjectList = new List<SubjectDetails>();
                                foreach (var sub in allSubjects)
                                {
                                    SubjectDetails za = new SubjectDetails();
                                    if (sub.Code != null && sub.Code != string.Empty)
                                        za.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                                    else
                                        za.SubjectName = sub.SubjectName;
                                    var x1 = one.scholasticList[2].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x2 = one.scholasticList[3].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x3 = one.scholasticList[4].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();
                                    var x4 = one.scholasticList[5].subjectList.Where(x => x.SubjectName == za.SubjectName).FirstOrDefault();


                                    za.Mark = Math.Floor(((Convert.ToDouble(x1.Mark) + Convert.ToDouble(x2.Mark) + Convert.ToDouble(x3.Mark) + Convert.ToDouble(x4.Mark)))).ToString();
                                    z1.subjectList.Add(za);
                                }
                                one.scholasticList.Add(z1);
                            }
                            //#endregion PT Average
                            findSecond = findSecond + 1;
                        }
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId).FirstOrDefault();
                            //var subjectIdFromDeclaredExam = sub.TbDeclaredExamsubjects.Where(x => x.SubjectId == sub.SubId && x.DeclaredExamId == Item.Id).FirstOrDefault();
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            //a.Mark = Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString() == null ? "0" : Item.tb_ExamResult.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).Select(x => (x.StudentScore)).FirstOrDefault().ToString();
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id).FirstOrDefault();
                                if (xxx != null)
                                    a.Mark = xxx.StudentScore.ToString();
                                else
                                    a.Mark = "0";
                            }
                            else
                            {
                                a.Mark = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            //c.CoScholasticResult = co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId && x.ExamId==Item.Id).Select(x => x.Score).FirstOrDefault() == null ? "0" : co.tb_Co_ScholasticResultmain.Where(x => x.StudentId == studentId).Select(x => x.Score).FirstOrDefault();
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);

                        #region  COMPUTER SCIENCE 
                        if (haveComputerScience == true)
                        {
                            ComputerScienceResultModel xx = new ComputerScienceResultModel();//10050
                            var thisSchoolSubject = _Entities.TbSubjects.Where(x => x.SubId == 20076 && x.SchoolI == schoolData.SchoolId && x.IsActive).FirstOrDefault();
                            if (tern == 1 && thisSchoolSubject != null)
                            {
                                var a = term1Subjects.Where(x => x.SubjectId == thisSchoolSubject.SubId).FirstOrDefault();
                                if (a != null)
                                {
                                    var result = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive && x.SubjectId == a.Id).FirstOrDefault();
                                    if (thisSchoolSubject.Code != null && thisSchoolSubject.Code != string.Empty)
                                        xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    else
                                        xx.SubjectName = thisSchoolSubject.SubjectName;
                                    if (result != null)
                                    {
                                        xx.TheoryScore = result.StudentScore == 0 ? "0" : Convert.ToString(result.StudentScore);
                                        xx.PracticalScore = result.PracticalScore == 0 ? "0" : Convert.ToString(result.PracticalScore);
                                    }
                                    else
                                    {
                                        xx.TheoryScore = "0";
                                        xx.PracticalScore = "0";
                                    }
                                    xx.TheoryTotal = a.TotalScore == 0 ? "" : Convert.ToString(a.TotalScore);
                                    //xx.PracticalTotal = Convert.ToString(100 - Convert.ToDecimal(xx.TheoryTotal));
                                    xx.PracticalTotal = Convert.ToString(50 - Convert.ToDecimal(xx.TheoryTotal));//This is changed to convert PT marks out of 50 - Razi 28/03/2022
                                }
                                else
                                {
                                    xx.SubjectName = thisSchoolSubject.SubjectName + "(" + (thisSchoolSubject.Code == null ? "" : thisSchoolSubject.Code) + ")";
                                    xx.TheoryScore = "0";
                                    xx.PracticalScore = "0";
                                    xx.TheoryTotal = "0";
                                    xx.PracticalTotal = "0";
                                }
                                model.csList.Add(xx);
                            }
                            else
                            {
                            }
                        }
                        #endregion COMPUTER SCIENCE 
                    }
                    #endregion Term II
                    model.IsOneTerm = false;
                }
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                //Anual 
                return View();
            }
        }


        public ActionResult HigherSecondaryProgressCard_Previous(string id)
        {
            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var ClassId = Convert.ToInt64(splitData[2]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive && x.TbExamBook.IsActive && x.TbExamBook.Term.IsActive && x.Class.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.TbExamBook.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive==true).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.Where(x => x.IsActive == true && x.ClassId==ClassId).FirstOrDefault();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive && (x.ClassId == ClassId || x.ClassId == null)).ToList();
            var schoolData = _Entities.TbLogins.Where(x => x.SchoolId == StudentDetails.SchoolId && x.RoleId == 1 && x.IsActive).FirstOrDefault();
            var periods = _Entities.TbAcademicPeriods.Where(x => x.SchoolId == StudentDetails.SchoolId && x.ClassId == ClassId && x.IsActive).OrderBy(x => x.StartDate).ToList();
            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.SchoolEmail = schoolData.Username;
                model.SchoolId = _user.SchoolId;
                model.ClassId = ClassId;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                try
                {

                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId ==ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-----------------New-------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 0)
                        model.Periods = periods.FirstOrDefault().StartDate.Month + " " + periods.FirstOrDefault().StartDate.Year;
                    else
                        model.Periods = "";
                    //---------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph: " + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId && x.IsActive).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId && x.IsActive).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToShortDateString();
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = _Entities.TbClasses.Where(x=>x.ClassId==ClassId && x.IsActive==true).Select(x=>x.Class).FirstOrDefault();
                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();
                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().ToList();// The Subjects wants to list in the order of them created 
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // HERE AVOIDING THE COMPUTER SCIENCE SUBJECT RESULT FROM LIST, THIS IS FOR CHANGE THE COMPUTER SCIENCE RESULT FROM THE PROGRESS CARD
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            //allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                {
                                    a.Mark = xxx.StudentScore.ToString() == null ? "0" : xxx.StudentScore.ToString();
                                    a.PracticalScore = xxx.PracticalScore.ToString() == null ? "0" : xxx.PracticalScore.ToString();
                                }
                                else
                                {
                                    a.Mark = "0";
                                    a.PracticalScore = "0";
                                }
                                a.MarkTotal = subjectIdFromDeclaredExam.TotalScore.ToString();
                                a.PracticalScoreTotal = (100 - subjectIdFromDeclaredExam.TotalScore).ToString();//changed by razi from 100 to 50 on 16/03/2022
                            }
                            else
                            {
                                a.Mark = "0";
                                a.PracticalScore = "0";
                                a.MarkTotal = "0";
                                a.PracticalScoreTotal = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                    }
                    #endregion Term II
                    model.IsOneTerm = true;
                }
                catch (Exception x)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.csList = new List<ComputerScienceResultModel>();// Computer Science
                model.SchoolLogo = StudentDetails.School.FilePath;
                model.ClassId = ClassId;
                model.SchoolEmail = schoolData.Username;
                model.SchoolId = _user.SchoolId;
                model.SchoolWebsite = schoolData.School.Website == null ? "" : schoolData.School.Website;
                try
                {
                    model.StudentId = studentId;
                    //model.ExamId = declaredExamId;
                    model.ExamId = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == ClassId && x.TermId == 1).Select(x => x.ExamId).FirstOrDefault();
                    model.ExamId2 = _Entities.TbDeclaredExams.Where(x => x.IsActive && x.ClassId == ClassId && x.TermId == 2).Select(x => x.ExamId).FirstOrDefault();
                    model.SchoolName = StudentDetails.School.SchoolName;
                    model.AccademicSession = ClassDetails.AcademicYear.AcademicYear;
                    model.ReportName = _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault() == null ? " REPORT " : _Entities.TbCertificateNames.Where(x => x.IsActive && x.SchoolId == StudentDetails.SchoolId).Select(x => x.CertificateName).FirstOrDefault();
                    model.RollNo = StudentDetails.ClasssNumber == null ? " " : StudentDetails.ClasssNumber;
                    model.StudentName = StudentDetails.StundentName;
                    model.FathersName = StudentDetails.ParentName;
                    model.MothersName = StudentDetails.MotherName == null ? " " : StudentDetails.MotherName;
                    model.RegionName = RegionDetails.Region.RegionName;
                    //-------------new-----------------------------
                    model.AdmissionNo = StudentDetails.StudentSpecialId;
                    model.StudentImage = StudentDetails.FilePath;
                    if (periods != null && periods.Count() > 1)
                        model.Periods = periods[1].StartDate.Month + "-" + periods[1].StartDate.Year;
                    else
                        model.Periods = "";
                    //------------------------------------------
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address + "   Ph:" + StudentDetails.School.Contact;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    model.Remark2 = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId2).Select(x => x.Remark).FirstOrDefault();//razi 29/03
                    if (StudentDetails.Dob != null)
                    //model.DateOfBirth = Convert.ToDateTime(StudentDetails.DOB).ToString("dd/MM/yyyy");
                    {
                        try
                        {
                            model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToString("dd/MM/yyyy");
                        }
                        catch
                        {
                            var az = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                            string[] xx = az.Split('/');
                            if (xx != null && xx.Count() > 2)
                            {
                                model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                            }
                            else
                            {
                                xx = az.Split('-');
                                if (xx != null && xx.Count() > 2)
                                {
                                    model.DateOfBirth = xx[1] + "/" + xx[0] + "/" + xx[2];
                                }
                                else
                                {
                                    model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                                }
                            }
                        }
                    }
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = _Entities.TbClasses.Where(x => x.ClassId == ClassId && x.IsActive == true).Select(x=>x.Class).FirstOrDefault();
                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();
                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
                    var term1DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[0].Id && x.IsActive).FirstOrDefault();
                    var term2DeclaredExams = twoDeclaredExams.Where(x => x.Id == twoDeclaredExams[1].Id && x.IsActive).FirstOrDefault();
                    var term1Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term1DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    var term2Subjects = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == term2DeclaredExams.Id && x.IsActive).OrderBy(x => x.TimeStamp).ToList();
                    //var allSubjects = term1Subjects.Select(x => x.TbSubjectss).ToList().Union(term2Subjects.Select(x => x.TbSubjectss).ToList());
                    var allSubjects = term1Subjects.Select(x => x.Subject).ToList().OrderBy(x => x.TmeStamp).ToList();// Order should be created
                    allSubjects = allSubjects.Where(x => x.SubId != 10050 && x.SubId != 20076).ToList(); // AVOIDING THE COMPUTER SCIENCE FROM THE SUBJECT LIST
                    var optionalSubjects = allSubjects.Where(x => x.IsOptonal == true).ToList();
                    foreach (var op in optionalSubjects)
                    {
                        if (_Entities.TbOptionalSubjectStudents.Any(x => x.SubjectId == op.SubId && x.StudentId == StudentDetails.StudentId && x.IsActive == true))
                        {
                        }
                        else
                        {
                            //allSubjects.Remove(op);
                        }
                    }
                    model.TermDataList = new List<TermList>();
                    int tern = 0;
                    foreach (var Item in twoDeclaredExams)
                    {
                        tern = tern + 1;
                        TermList one = new TermList();
                        one.TermName = "TERM " + tern;
                        one.scholasticList = new List<ScolasticAreaList>();
                        #region Exam
                        ScolasticAreaList zz = new ScolasticAreaList();
                        var returnExamresult = Item.TbExamResults.Where(x => x.StudentId == studentId && x.IsActive).OrderByDescending(x => x.Subject.TotalScore).ToList();
                        zz.subjectList = new List<SubjectDetails>();
                        zz.ScolasticArea = Item.TbExamBook.ExamName;
                        decimal sco = returnExamresult.FirstOrDefault() == null ? 0 : returnExamresult.FirstOrDefault().Subject.TotalScore;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault() == null ? 0 : allSubjects.FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).OrderByDescending(x => x.TotalScore).FirstOrDefault().TotalScore;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            var subjectIdFromDeclaredExam = _Entities.TbDeclaredExamSubjects.Where(x => x.DeclaredExamId == Item.Id && x.SubjectId == sub.SubId && x.IsActive).FirstOrDefault();
                            SubjectDetails a = new SubjectDetails();
                            if (sub.Code != null && sub.Code != string.Empty)
                                a.SubjectName = sub.SubjectName + "(" + sub.Code + ")";
                            else
                                a.SubjectName = sub.SubjectName;
                            if (subjectIdFromDeclaredExam != null)
                            {
                                var xxx = Item.TbExamResults.Where(x => x.StudentId == studentId && x.SubjectId == subjectIdFromDeclaredExam.Id && x.IsActive).FirstOrDefault();
                                if (xxx != null)
                                {
                                    a.Mark = xxx.StudentScore.ToString() == null ? "0" : xxx.StudentScore.ToString();
                                    a.PracticalScore = xxx.PracticalScore.ToString() == null ? "0" : xxx.PracticalScore.ToString();
                                }
                                else
                                {
                                    a.Mark = "0";
                                    a.PracticalScore = "0";
                                }
                                a.MarkTotal = subjectIdFromDeclaredExam.TotalScore.ToString();
                                a.PracticalScoreTotal = (100 - subjectIdFromDeclaredExam.TotalScore).ToString();//changed by Gayathri from 50 to 100 on 02/03/2023
                            }
                            else
                            {
                                a.Mark = "0";
                                a.PracticalScore = "0";
                                a.MarkTotal = "0";
                                a.PracticalScoreTotal = "0";
                            }
                            zz.subjectList.Add(a);
                        }
                        one.scholasticList.Add(zz);
                        #endregion Exam
                        var coScholastic = Item.TbCoScholasticResultmains.Where(x => x.IsActive && x.StudentId == studentId).ToList();
                        one.ColasticAreaResult = new List<CoscholasticAreaList>();
                        foreach (var co in CoScholasticData)
                        {
                            CoscholasticAreaList c = new CoscholasticAreaList();
                            c.CoScholasticData = co.ItemName;
                            c.CoScholasticResult = coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault() == null ? "-" : coScholastic.Where(x => x.CoScholasticId == co.Id).FirstOrDefault().Score;
                            one.ColasticAreaResult.Add(c);
                        }
                        model.TermDataList.Add(one);
                    }
                    #endregion Term II
                    model.IsOneTerm = false;
                }
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else
            {
                return View();
            }
        }







    }
}

