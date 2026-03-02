
//using ChildrenScholarship.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satluj_Latest;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Helper;
using Satluj_Latest.MapModel;
using Satluj_Latest.Models;
using Satluj_Latest.Models.ViewModels;
using Satluj_Latest.PostModel;
using Satluj_Latest.Repository;
using Satluj_Latest.Service.Helper;
using Satluj_Latest.Utility;
//using CS.EntityLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using PagedList;
//using PagedList.Mvc;
//using TrackTap.ClassLibrary.Utility;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Satluj_Latest.Models.SchoolValue;


namespace Satluj_latestversion.Controllers
{
    public class SchoolController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public DateTime CurrentTime =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        private readonly DropdownData _dropdown;
        
        public SchoolController(DropdownData dropdown,SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, IWebHostEnvironment env) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _env = env;
            _dropdown = dropdown;

        }

        //public SchoolController(IWebHostEnvironment env)
        //{
        //}

        //
        // GET: /School/
        public object CheckEmail(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbLogins.Any(x => x.Username.ToLower() == text.ToLower() && x.IsActive && x.SchoolId == _user.SchoolId))
            {
                Status = true;
                Message = "Username already in use";
            }
            return Json(new { Status = Status, Message = Message });
        }
        public IActionResult Class()
        {
            var model = new SchoolValue();
            model.schoolId = _user.SchoolId;
            ViewBag.classlist = _dropdown.GetClasses(model.schoolId);
            return View(model);
        }
        public PartialViewResult AddClassParialView()
        {
            AddClassModel model = new AddClassModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.ClassList = _dropdown.GetClassList();
            return PartialView("_pv_AddClass", model);

        }

        public PartialViewResult DatatableClassList(Satluj_Latest.Models.FilterModel model)
        {
            model.schoolId = _user.SchoolId;

            if (model.classId == 0)
                model.classId = null;

            return PartialView("~/Views/School/_pv_ClassList.cshtml", model);
        }


        [HttpPost]
        public IActionResult AddClass(AddClassModel model)

        {
            bool status = false;
            string msg = "failed";
            try
            {
                var academicYear = _Entities.TbAcademicYears.Where(z => z.IsActive && z.CurrentYear == true).FirstOrDefault();//currentyear need to change
                var getClass = _Entities.TbClasses.Where(z => z.SchoolId == model.SchoolId && z.PublishStatus==true && z.School.IsActive && z.AcademicYearId == academicYear.YearId && z.IsActive && z.Class.ToLower() == model.ClassName.ToLower()).FirstOrDefault();
                if (getClass != null)
                {
                    if (_Entities.TbDivisions.Any(z => z.Division.ToLower() == model.Division.ToLower() && z.ClassId == getClass.ClassId && z.IsActive))
                    {
                        msg = "Division already added";
                    }
                    else
                    {
                        var newDiv = new TbDivision();
                        newDiv.ClassId = getClass.ClassId;
                        newDiv.Division = model.Division.ToUpper();
                        newDiv.DivisionGuid = Guid.NewGuid();
                        newDiv.IsActive = true;
                        newDiv.TimeStamp = CurrentTime;
                        _Entities.TbDivisions.Add(newDiv);

                        status = _Entities.SaveChanges() > 0;
                        msg = status ? "success" : "failed";
                    }
                    return Json(new { status = status, msg = msg, list = new Satluj_Latest.Data.DropdownData().RefreshClasses(model.SchoolId) });
                }
                else
                {
                    var newClass = new TbClass();
                    newClass.Class = model.ClassName;
                    newClass.ClassGuild = Guid.NewGuid();
                    newClass.Timestamp = CurrentTime;
                    newClass.SchoolId = model.SchoolId;
                    newClass.IsActive = true;
                    newClass.ClassOrder = model.OrderValue;
                    newClass.AcademicYearId = academicYear.YearId;
                    newClass.PublishStatus = true;
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
                    return Json(new { status = status, msg = msg, list = new Satluj_Latest.Data.DropdownData().RefreshClasses(model.SchoolId) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = status, msg = msg });
            }
        }

        public ActionResult ClassNotPublished()
        {
            var model = new SchoolValue();
            model.schoolId = _user.SchoolId;
 
            ViewBag.UnPublishedClasses = _dropdown.GetUnPublishedClasses(model.schoolId);

            return View(model);
        }
        public PartialViewResult AddClassUnPublishedParialView()
        {
            AddClassModel model = new AddClassModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Academicyear = _dropdown.GetOtherAcademicYear();
            return PartialView("~/Views/School/_pv_AddClassUnPublished.cshtml", model);
        }
        public object AddClassUnPublished(AddClassModel model)
        {
            bool status = false;
            string msg = "failed";
            try
            {
                var academicYear = _Entities.TbAcademicYears.FirstOrDefault();
                var getClass = _Entities.TbClasses.Where(z => z.SchoolId == model.SchoolId && z.School.IsActive && z.IsActive && z.AcademicYearId == model.AcademicYearId && z.Class.ToLower() == model.ClassName.ToLower()).FirstOrDefault();
                if (getClass != null)
                {
                    if (_Entities.TbDivisions.Any(z => z.Division.ToLower() == model.Division.ToLower() && z.ClassId == getClass.ClassId && z.IsActive))
                    {
                        msg = "Division already added";
                    }
                    else
                    {
                        var newDiv = new TbDivision();
                        newDiv.ClassId = getClass.ClassId;
                        newDiv.Division = model.Division.ToUpper();
                        newDiv.DivisionGuid = Guid.NewGuid();
                        newDiv.IsActive = true;
                        newDiv.TimeStamp = CurrentTime;
                        _Entities.TbDivisions.Add(newDiv);

                        status = _Entities.SaveChanges() > 0;
                        msg = status ? "success" : "failed";
                    }
                    return Json(new { status = status, msg = msg, list = new Satluj_Latest.Data.DropdownData().RefreshClasses(model.SchoolId) });
                }
                else
                {
                    var newClass = new TbClass();
                    newClass.Class = model.ClassName;
                    newClass.ClassGuild = Guid.NewGuid();
                    newClass.Timestamp = CurrentTime;
                    newClass.SchoolId = model.SchoolId;
                    newClass.IsActive = true;
                    newClass.ClassOrder = model.OrderValue;
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
                    return Json(new { status = status, msg = msg, list = new Satluj_Latest.Data.DropdownData().RefreshClasses(model.SchoolId) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = status, msg = msg });
            }
        }

        public PartialViewResult DatatableClassListUnPublished(Satluj_Latest.Models.FilterModel model)
        {
            return PartialView("~/Views/School/_pv_ClassListUnPublished.cshtml", model);
        }

        public object DeleteClassUnPublished(string id)
        {
            bool status = false;
            string message = "Failed";
            long DivisionId = Convert.ToInt64(id);
            var Division = _Entities.TbDivisions.FirstOrDefault(z => z.DivisionId == DivisionId);
            if (Division != null)
            {
                Division.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    if (!_Entities.TbDivisions.Any(z => z.ClassId == Division.ClassId && z.IsActive))
                    {
                        var classData = _Entities.TbClasses.FirstOrDefault(z => z.ClassId == Division.ClassId);
                        if (classData != null)
                        {
                            classData.IsActive = false;
                            _Entities.SaveChanges();
                        }
                    }
                }
            }
            message = status ? "Deleted" : "failed";
            return Json(new { status = status, msg = message, list = new Satluj_Latest.Data.DropdownData().RefreshClassesUnPublished(_user.SchoolId) });
        }

        public object PublishClassUnPublished(string id)
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
                    var prevClass = _Entities.TbClasses.Where(z => z.Class == Class.Class && z.AcademicYearId != Class.AcademicYearId && z.SchoolId == _user.SchoolId).ToList();
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
            return Json(new { status = status, msg = message, list = new Satluj_Latest.Data.DropdownData().RefreshClassesUnPublished(_user.SchoolId) });
        }

        public PartialViewResult GetToPromotionStudentsByDivGrid(string id)
        {
            StudentModel model = new StudentModel();
            model.divisionId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_PromoteStudent_Student_Grid.cshtml", model);

        }

        [HttpPost]
        public object PremoteStudentClass(StudentModel model)
        {
            bool status = false;
            string message = "Failed";
            List<string> studentsUserId = model.stringStudentId.Split(',').ToList();
            var studentList = _Entities.TbStudents.Where(z => z.DivisionId == model.divisionId && z.IsActive).ToList();

            var premotion = new TbStudentPremotion();
            foreach (var userId in studentsUserId)
            {
                long userIdLong = Convert.ToInt64(userId);
                var isStudent = studentList.Where(z => z.StudentId == userIdLong && z.IsActive).FirstOrDefault();
                if (isStudent != null)
                {
                    premotion.StudentId = userIdLong;
                    premotion.FromDivision = model.divisionId;
                    premotion.ToDivision = model.toDivisionId;
                    premotion.TimeStamp = CurrentTime;
                    _Entities.TbStudentPremotions.Add(premotion);
                    status = _Entities.SaveChanges() > 0;
                }

            }
            foreach (var userId in studentsUserId)
            {
                long userIdLong = Convert.ToInt32(userId);
                var isStudent = _Entities.TbStudents.Where(z => z.StudentId == userIdLong && z.IsActive).FirstOrDefault();
                if (isStudent != null)
                {
                    isStudent.DivisionId = model.toDivisionId;
                    isStudent.ClassId = model.classId;
                    status = _Entities.SaveChanges() > 0;
                }

            }
            message = status ? " Student promoted" : "Failed to promote student";
            return Json(new { status = status, msg = message });
        }
        public ActionResult Home()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.Currenttime = CurrentTime;
            return View(model);
        }
        public ActionResult Billing()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            ViewBag.ClassList = _dropdown.GetClasses(model.schoolId);

            return View(model);
        }
        public PartialViewResult GetUserListByDivision(string id)
        {
            SchoolModel model = new SchoolModel();
            model.divisionId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_Billing_UserByDivision_Grid.cshtml", model);

        }
        public ActionResult BillingDetails(string id)
        {
            long studentId = Convert.ToInt32(id);
            var student = new Satluj_Latest.Data.Student(studentId);
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentName = student.StundentName;
            model.SchoolModel.classNumber = student.ClasssNumber; //Archana
            model.SchoolModel.className = student.ClassName;
            model.SchoolModel.division = student.DivisionName;
            model.SchoolModel.classInCharge = student.Teacher == null ? "Not Assigned" : student.Teacher.TeacherName;
            model.SchoolModel.classId = student.ClassId;
            model.SchoolModel.studentId = studentId;
            model.SchoolModel.curredntDateTime = CurrentTime;
            model.SchoolId = _user.SchoolId;
            ViewBag.BankList = _dropdown.GetBankLists(model.SchoolId);
            return View(model);
        }
        public ActionResult ClassDetails(long classId, long divId)
        {
            var model = new SchoolValue();
            model.classId = classId;
            model.divId = divId;
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public ActionResult Student()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            ViewBag.Classes = _dropdown.GetClasses(model.schoolId);
            return View(model);
        }
        public ActionResult Teacher()
        {
            return View();
        }
        public PartialViewResult AddStudentView()
        {
            StudentModel model = new StudentModel();
            model.schoolId = _user.SchoolId;
            model.state = _user.School.State;
            model.city = _user.School.City;
            ViewBag.Classlist = _dropdown.GetClasses(model.schoolId);
            ViewBag.Buslist = _dropdown.GetBusName(model.schoolId);
            //model.busId = 1;
            return PartialView("~/Views/School/_pv_AddStudent_Model.cshtml", model);
        }
        #region Library
        public ActionResult BookCategory()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public PartialViewResult EditCategoryView(string id)
        {
            LibraryModels model = new LibraryModels();
            long catId = Convert.ToInt64(id);
            var category = _Entities.TbBookCategories.Where(z => z.CategoryId == catId).FirstOrDefault();
            model.schoolId = _user.SchoolId;
            model.categoryId = category.CategoryId;
            model.categoryName = category.Category;
            return PartialView("~/Views/School/_pv_BookCategory_Edit.cshtml", model);
        }


        public PartialViewResult AddCategoryView()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_BookCategory_Add.cshtml", model);
        }
        [HttpPost]
        public object AddBookCategory(LibraryModels model)
        {
            bool status = false;
            string message = "Failed";
            var category = new TbBookCategory();
            category.Category = model.categoryName;
            category.SchoolId = model.schoolId;
            category.IsActive = true;
            category.TimeStamp = CurrentTime;
            _Entities.TbBookCategories.Add(category);
            status = _Entities.SaveChanges() > 0;
            message = status ? " Category Added" : "Failed";
            return Json(new { status = status, msg = message });
        }
        [HttpPost]
        public object EditBookCategory(LibraryModels model)
        {
            bool status = false;
            string message = "Failed";
            var category = _Entities.TbBookCategories.Where(z => z.CategoryId == model.categoryId).FirstOrDefault();
            category.Category = model.categoryName;
            status = _Entities.SaveChanges() > 0;
            message = status ? " Category Edited" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public object DeleteBookCategory(string id)
        {
            bool status = false;
            string message = "Failed";
            long catId = Convert.ToInt64(id);
            var category = _Entities.TbBookCategories.Where(z => z.CategoryId == catId).FirstOrDefault();

            if (_Entities.TbLibraryBooks.Any(x => x.CategoryId == catId))
            {
                category.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                message = status ? " Category deleted successfully" : "Failed to delete Category";
            }
            else
            {
                _Entities.TbBookCategories.Remove(category);
                status = _Entities.SaveChanges() > 0 ? true : false;
                message = status ? "Category deleted successfully!" : "Failed to delete Category!";
            }

            return Json(new { status = status, msg = message });
        }
        public PartialViewResult BookCategoryListPartial()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_BookCategory_list.cshtml", model);
        }

        public ActionResult LibraryBook()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public PartialViewResult AddLibraryBookView()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            model.bookCount = 1;
            return PartialView("~/Views/School/_pv_LibraryBook_Add.cshtml", model);
        }


        [HttpPost]
        public object AddLibraryBook(LibraryModels model)
        {
            bool status = false;
            string message = "Failed";
            long SlNo = 0;
            var slNo = _Entities.TbLibraryBookSerialNumbers.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            if (slNo != null)
            {
                SlNo = slNo.SerialNo;
            }

            for (int i = 0; i < model.bookCount; i++)
            {
                var book = new TbLibraryBook();
                SlNo = SlNo + 1;
                book.CategoryId = model.categoryId;
                book.Title = model.title;
                book.Author = model.author;
                book.Status = 0; //Available
                book.IsActive = true;
                book.TimeStamp = CurrentTime;
                book.SerialNumber = SlNo;
                book.ReferenceNumber = model.ReferenceNumber == null ? " " : model.ReferenceNumber;
                _Entities.TbLibraryBooks.Add(book);
                status = _Entities.SaveChanges() > 0;
            }

            if (status)
            {
                var srlNo1 = _Entities.TbLibraryBookSerialNumbers.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
                if (srlNo1 != null)
                {
                    srlNo1.SerialNo = SlNo;
                    status = _Entities.SaveChanges() > 0 ? true : false;

                }
                else
                {
                    var slNoTable = new TbLibraryBookSerialNumber();
                    slNoTable.SchoolId = _user.SchoolId;
                    slNoTable.SerialNo = SlNo;
                    _Entities.TbLibraryBookSerialNumbers.Add(slNoTable);
                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
            }
            message = status ? " Book Added" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public PartialViewResult IssueLibraryBookView(string id)
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            model.bookId = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_LibraryBook_Issue_Add.cshtml", model);
        }
        public object AcceptLibraryBook(string id)
        {
            bool status = false;
            string message = "Failed";
            long bookId = Convert.ToInt64(id);
            var book = _Entities.TbLibraryBooks.Where(z => z.BookId == bookId).FirstOrDefault();
            book.Status = 0;//Available

            var studentBook = _Entities.TbLibraryBookStudents.Where(z => z.BookId == bookId && z.Status == true && z.IsActive).ToList().OrderByDescending(z => z.StudentBookId).FirstOrDefault();
            studentBook.Status = false; //accept
            studentBook.AcceptDateTime = CurrentTime;
            DateTime TodayDate = CurrentTime.Date;
            DateTime IssuedDate = studentBook.IssueDateTime.Date;

            status = _Entities.SaveChanges() > 0;
            if (status)
            {
                var dueDays = book.Category.School.LibraryDueDays;
                if (dueDays != null || dueDays != 0)
                {
                    var fineLib = book.Category.School.TbLibraryFines.FirstOrDefault();
                    if (fineLib != null)
                    {
                        double diffDate = (TodayDate - IssuedDate).TotalDays;
                        if (dueDays <= diffDate)
                        {
                            var feeStudent = new TbFeeStudent();
                            feeStudent.Amount = fineLib.FineAmount;
                            feeStudent.StudentId = Convert.ToInt32(studentBook.StudentId);
                            feeStudent.FeeId = fineLib.FeeId;
                            feeStudent.FeeStudentGuid = Guid.NewGuid();
                            feeStudent.IsActive = true;
                            feeStudent.TimeStamp = CurrentTime;
                            feeStudent.DueDate = CurrentTime;
                            feeStudent.Instalment = 1;
                            _Entities.TbFeeStudents.Add(feeStudent);
                            status = _Entities.SaveChanges() > 0;
                        }
                    }

                }
            }
            message = status ? "Book Accepted successfully" : "Failed to Accept Book";

            return Json(new { status = status, msg = message });
        }
        [HttpPost]
        public object IssueLibraryBook(LibraryModels model)
        {
            bool status = false;
            string message = "Failed";
            var isStudent = _Entities.TbStudents.Where(z => z.StudentSpecialId.ToUpper() == model.admissionNumber.ToUpper() && z.SchoolId == _user.SchoolId).OrderByDescending(z => z.StudentId).FirstOrDefault();
            if (isStudent != null)
            {
                var student = new TbLibraryBookStudent();
                student.StudentId = isStudent.StudentId;
                student.BookId = model.bookId;
                student.Status = true; //issue
                student.IsActive = true;
                student.IssueDateTime = CurrentTime;
                _Entities.TbLibraryBookStudents.Add(student);
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    var book = _Entities.TbLibraryBooks.Where(z => z.BookId == model.bookId && z.IsActive).FirstOrDefault();
                    book.Status = 1; //Issued
                    _Entities.SaveChanges();
                }
                message = status ? " Book Issued" : "Failed";
                return Json(new { status = status, msg = message });
            }
            else
            {
                message = "Admission number does not exist";
                return Json(new { status = status, msg = message });
            }
        }
        [HttpPost]
        public object EditLibraryBook(LibraryModels model)
        {
            bool status = false;
            string message = "Failed";
            var book = _Entities.TbLibraryBooks.Where(z => z.BookId == model.bookId).FirstOrDefault();
            book.CategoryId = model.categoryId;
            book.Title = model.title;
            book.Author = model.author;
            book.ReferenceNumber = model.ReferenceNumber;
            status = _Entities.SaveChanges() > 0;
            message = status ? " Book Edited" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public object DeleteLibraryBook(string id)
        {
            bool status = false;
            string message = "Failed";
            long bookId = Convert.ToInt64(id);
            var book = _Entities.TbLibraryBooks.Where(z => z.BookId == bookId).FirstOrDefault();

            //if (_Entities.tb_LibraryBook.Any(x => x.CategoryId == catId))
            //{
            book.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            message = status ? " Book deleted successfully" : "Failed to delete Book";
            //}
            //else
            //{
            //    _Entities.tb_BookCategory.Remove(category);
            //    status = _Entities.SaveChanges() > 0 ? true : false;
            //    message = status ? "Category deleted successfully!" : "Failed to delete Category!";
            //}

            return Json(new { status = status, msg = message });
        }
        public PartialViewResult BookListPartial()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_LibraryBook_Grid.cshtml", model);

        }


        public PartialViewResult EditLibraryBookView(string id)
        {
            LibraryModels model = new LibraryModels();
            long bookId = Convert.ToInt64(id);
            var book = _Entities.TbLibraryBooks.Where(z => z.BookId == bookId).FirstOrDefault();
            model.schoolId = _user.SchoolId;
            model.categoryId = book.CategoryId;
            model.title = book.Title;
            model.author = book.Author;
            model.status = book.Status;
            model.bookId = bookId;
            model.ReferenceNumber = book.ReferenceNumber == null ? "" : book.ReferenceNumber;
            return PartialView("~/Views/School/_pv_LibraryBook_Edit.cshtml", model);
        }
        public ActionResult LibraryBookStudent(string id)
        {
            long bookid = Convert.ToInt64(id);
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            model.bookId = bookid;
            return View(model);
        }


        public ActionResult LibraryBookDue()
        {
            LibraryModels model = new LibraryModels();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        #endregion

        #region Calendar

        public ActionResult CalendarEvent()
        {
            CalendarEventModels model = new CalendarEventModels();
            model.schoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult AddCalendarEventView()
        {
            return PartialView("~/Views/School/_pv_CalendarEvent_Add.cshtml");
        }
        [HttpPost]
        public object AddCalendarEvent(CalendarEventModels model)
        {
            bool status = false;
            string message = "Failed";
            var calender = new TbCalenderEvent();
            calender.EventHead = model.eventHead;
            calender.EventDetails = model.eventDetails;

            string[] splitData = model.eventDate.Split('-');
            var zdd = splitData[0];
            var zmm = splitData[1];
            var zyyyy = splitData[2];
            var Date = zmm + '-' + zdd + '-' + zyyyy;
            calender.EventDate = Convert.ToDateTime(Date);
            calender.SchoolId = _user.SchoolId;
            calender.IsActive = true;
            calender.TimeStamp = CurrentTime;
            _Entities.TbCalenderEvents.Add(calender);
            status = _Entities.SaveChanges() > 0;
            if (status)
            {
                // Create a notification entry added by dhanu for api 6 Nov 2025
                var notification = new TbNotification();
                notification.UserName = null; 
                notification.NotificationMessage = $"{model.eventHead}";
                notification.CreatedAt = CurrentTime;
                notification.IsRead = 0;
                notification.Source = "Event";
                notification.SchoolId = 3;
                notification.ClassId = null;
                notification.DivisionId = null;
                notification.SourceId = calender.EventId;
                _Entities.TbNotifications.Add(notification);
                _Entities.SaveChanges();

                message = "Event Added and Notification Created";
            }
            else
            {
                message = "Failed to Add Event";
            }
            message = status ? " Event Added" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public PartialViewResult EditCalendarEventView(string id)
        {
            CalendarEventModels model = new CalendarEventModels();
            long eventId = Convert.ToInt64(id);
            var calender = _Entities.TbCalenderEvents.Where(z => z.EventId == eventId).FirstOrDefault();
            model.eventId = calender.EventId;
            model.eventDate = calender.EventDate.ToString("dd-MM-yyyy");
            model.eventHead = calender.EventHead;
            model.eventDetails = calender.EventDetails;
            return PartialView("~/Views/School/_pv_CalendarEvent_Edit.cshtml", model);
        }
        [HttpPost]
        public object EditCalendarEvent(CalendarEventModels model)
        {
            bool status = false;
            string message = "Failed";
            var calender = _Entities.TbCalenderEvents.Where(z => z.EventId == model.eventId).FirstOrDefault();
            string[] splitData = model.eventDate.Split('-');
            var zdd = splitData[0];
            var zmm = splitData[1];
            var zyyyy = splitData[2];
            var Date = zmm + '-' + zdd + '-' + zyyyy;
            calender.EventDate = Convert.ToDateTime(Date);
            calender.EventHead = model.eventHead;
            calender.EventDetails = model.eventDetails;
            status = _Entities.SaveChanges() > 0;
            message = status ? " Event Edited" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public object DeleteCalendarEvent(string id)
        {
            bool status = false;
            string message = "Failed";
            long eventId = Convert.ToInt64(id);
            var calender = _Entities.TbCalenderEvents.Where(z => z.EventId == eventId).FirstOrDefault();
            var notifications = _Entities.TbNotifications
            .Where(n => n.Source == "Event" && n.SourceId == eventId)
            .ToList();

            if (notifications.Any())
            {
                _Entities.TbNotifications.RemoveRange(notifications);
            }

            _Entities.TbCalenderEvents.Remove(calender);
            status = _Entities.SaveChanges() > 0 ? true : false;
            message = status ? "Event deleted successfully!" : "Failed to delete Event!";

            return Json(new { status = status, msg = message });
        }
        public PartialViewResult CalendarEventListPartial()
        {
            CalendarEventModels model = new CalendarEventModels();
            model.schoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            return PartialView("~/Views/School/_pv_CalendarEvent_list.cshtml", model);

        }
        public PartialViewResult SearchCalendarEvent(string id)
        {
            string[] splitData = id.Split('~');
            DateTime startTime = Convert.ToDateTime(splitData[0]);
            DateTime endTime = Convert.ToDateTime(splitData[1]);
            CalendarEventModels model = new CalendarEventModels();
            model.schoolId = _user.SchoolId;
            model.startDate = startTime;
            model.endDate = endTime;
            return PartialView("~/Views/School/_pv_CalendarEvent_list.cshtml", model);

        }

        #endregion

        #region PromoteStudent 
        public ActionResult PromoteStudent()
        {
            StudentModel model = new StudentModel();
            model.schoolId = _user.SchoolId;
            ViewBag.Classlist = _dropdown.GetClasses(model.schoolId);
            ViewBag.unpublishedclass = _dropdown.GetUnPublishedClasses(model.schoolId);
            return View(model);
        }
        #endregion

        public async Task<object> AddStudent(StudentModel model)
        {
            Random rnd = new Random();
            bool status = false;
            string msg = string.Empty;
            // var school = Entities.tb_School.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            TbStudent stud = new TbStudent();
            stud.SchoolId = _user.SchoolId;
            stud.StudentSpecialId = model.admissionNo; //"ST" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            stud.StundentName = model.studentName;
            stud.ParentName = model.parentName;
            stud.ParentEmail = model.parentEmail;
            stud.Address = model.address;
            stud.City = model.city;
            stud.ContactNumber = model.contactNo;
            stud.Gender = model.gender;
            stud.BloodGroup = model.bloodGroup;
            stud.BioNumber = model.biometricId == null ? "  xxxx   " : model.biometricId;
            stud.MotherName = model.MotherName == null ? " " : model.MotherName;
            try
            {
                if (model.DOBstring != string.Empty && model.DOBstring != null)
                {
                    string[] splitData = model.DOBstring.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var dob = mm + '-' + dd + '-' + yyyy;
                    stud.Dob = Convert.ToDateTime(dob);
                }
            }
            catch
            {

            }
            // stud.ClasssNumber = _user.tb_School.School;
            stud.ClassId = model.classId;
            stud.DivisionId = model.divisionId;
            string division = model.divisionId.ToString();
            string classId = model.classId.ToString();
            stud.BusId = model.BusId == null ? Convert.ToInt64(1) : Convert.ToInt64(model.BusId);
            stud.TripNo = model.tripNumber;
            stud.TimeStamp = CurrentTime;
            stud.StudentGuid = Guid.NewGuid();
            stud.IsActive = true;
            // stud.ParentId = false;
            stud.State = model.state;
            //Profile Pic
            if (model.profilePic != null)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "Media/Student/Profile");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var imageString = model.profilePic.Substring(model.profilePic.IndexOf(',') + 1);

                // Convert Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(imageString);

                // Generate unique filename
                string imageName = Guid.NewGuid().ToString() + ".jpeg";

                // Full physical path
                string imgFilePath = Path.Combine(folderPath, imageName);

                // Relative path (to store in DB)
                string fileSave = "/Media/Student/Profile/" + imageName;

                // Save the file
                using (var stream = new FileStream(imgFilePath, FileMode.Create))
                {
                    await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    //imageFile.Write(imageByte, 0, imageByte.Length);
                    //   imageFile.Flush();
                    stud.FilePath = fileSave;
                }
            }
            //Profile Pic
            try
            {
                _Entities.TbStudents.Add(stud);
                status = _Entities.SaveChanges() > 0 ? true : false;
            }
            catch (Exception ex)
            {

            }
            msg = status ? "Student added successfully!" : "Failed to add Student!";
            return Json(new { status = status, msg = msg, division = division, classId = classId });
        }
        public PartialViewResult GetUserListByDivGrid(string id)
        {
            SchoolModel model = new SchoolModel();
            model.divisionId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_Student_ByDivision_Grid.cshtml", model);

        }
        public PartialViewResult GetUserListByClassDiv(string id)
        {
            string[] splitData = id.Split('~');
            SchoolModel model = new SchoolModel();
            model.classId = Convert.ToInt64(splitData[0]);
            model.divisionId = Convert.ToInt64(splitData[1]);
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_Student_ByDivision_Grid.cshtml", model);

        }
        public PartialViewResult GetUserListByClassBilling(string id)
        {
            string[] splitData = id.Split('~');
            SchoolModel model = new SchoolModel();
            model.classId = Convert.ToInt64(splitData[0]);
            model.divisionId = Convert.ToInt64(splitData[1]);
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_Billing_UserByDivision_Grid.cshtml", model);

        }
        [HttpPost]
        public async Task<object> StudentMainBillPay(FeeModel model)
        {
            //using (DbContextTransaction trans = _Entities.Database.BeginTransaction())
            //{
            //    try
            //    {
            decimal sumAmt = 0;
            bool status = false;
            string message = "Failed";
            List<string> feeDetails = model.FeeDetails.Split(',').ToList();
            long SchoolId = model.SchoolId;
            long ClassId = model.ClassId;
            long StudentId = model.StudentId;
            DateTime BillDate = model.TimeStamp;
            if (model.TimeStamp.ToString("MM-dd-YYYY") == CurrentTime.ToString("MM-dd-YYYY"))
            {
                BillDate = CurrentTime;
            }
            decimal TotalAmountPaid = 0;
            if (model.PaidAmount != 0)
            {
                TotalAmountPaid = Convert.ToDecimal(model.PaidAmount);
            }
            Guid PaymentGuid = Guid.NewGuid(); // To find same bill elements
            long BillNo = 1;
            var payment = new TbPayment();
            var billNo = _Entities.TbPaymentBillNos.Where(z => z.SchoolId == SchoolId).FirstOrDefault();
            if (billNo != null)
            {
                BillNo = billNo.BillNo + 1;
            }
            else
            {
                var slNoTable = new TbPaymentBillNo();
                slNoTable.SchoolId = SchoolId;
                slNoTable.BillNo = 1;
                _Entities.TbPaymentBillNos.Add(slNoTable);
                status = _Entities.SaveChanges() > 0 ? true : false;
            }
            var studDetails = _Entities.TbStudents.Where(z => z.StudentId == StudentId && z.IsActive == true).FirstOrDefault();
            long thisBillVoucherNumber = 0;
            var vouchrTbl = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (vouchrTbl == null)
            {
                var voucherNumber = new TbVoucherNumber();
                voucherNumber.SchoolId = _user.SchoolId;
                voucherNumber.PaymentVoucher = 1;
                voucherNumber.ReceiptVoucher = 1;
                voucherNumber.ContraVoucher = 1;
                voucherNumber.IsActive = true;
                voucherNumber.TimeStamp = CurrentTime;
                _Entities.TbVoucherNumbers.Add(voucherNumber);
                _Entities.SaveChanges();
                thisBillVoucherNumber = voucherNumber.ReceiptVoucher;
            }
            else
            {
                thisBillVoucherNumber = vouchrTbl.ReceiptVoucher;
            }
            long headIdBill = 0;
            var accountHead = _Entities.TbAccountHeads.Where(x => x.SchoolId == _user.SchoolId && x.ForBill == true).FirstOrDefault();
            if (accountHead == null)
            {
                var AccountHead = new TbAccountHead();
                AccountHead.AccHeadName = "Fee Income";
                AccountHead.ForBill = true;
                AccountHead.SchoolId = _user.SchoolId;
                AccountHead.IsActive = true;
                AccountHead.TimeStamp = CurrentTime;
                _Entities.TbAccountHeads.Add(AccountHead);
                _Entities.SaveChanges();
                headIdBill = AccountHead.AccountId;
            }
            else
                headIdBill = accountHead.AccountId;

            var previousBalanceAmount = _Entities.TbStudentBalances.Where(z => z.StudentId == StudentId && z.IsActive).FirstOrDefault();//Archana 30-11-2018
            decimal prevAdv = 0;
            if (previousBalanceAmount != null)
                prevAdv = previousBalanceAmount.Amount;
            bool UpdateVoucher = false;
            foreach (var fee in feeDetails)
            {
                string[] splitData = fee.Split('^');
                decimal paymentAmount = Convert.ToDecimal(splitData[0]);
                long feeId = Convert.ToInt32(splitData[1]);
                payment.FeeId = feeId;
                payment.FeeGuid = new Guid(splitData[2]);
                decimal maxAmount = Convert.ToDecimal(splitData[3]);
                decimal discount = Convert.ToDecimal(splitData[4]);
                payment.MaxAmount = maxAmount;
                payment.Discount = discount;
                payment.BillType = Convert.ToInt32(splitData[6]);
                int isAmountEdit = Convert.ToInt16(splitData[5]);

                if (isAmountEdit != 0)
                {
                    //var paymentList = new Satluj_Latest.Data.Student(StudentId).GetStudentPaymentFees(StudentId, ClassId).OrderBy(z => z.DueDate).ToList();
                    var studentObj = new Satluj_Latest.Data.Student(StudentId);

                    var fees = await studentObj.GetStudentPaymentFees(StudentId, ClassId);

                    var paymentList = fees
                        .OrderBy(z => z.DueDate)
                        .ToList();
                    var dueFee = paymentList.Where(z => z.FeeGuid == payment.FeeGuid).FirstOrDefault();
                    if (dueFee != null)
                    {
                        if (dueFee.Amount != paymentAmount)
                        {
                            var due = new TbFeeDue();
                            due.FeeId = payment.FeeId;
                            decimal amtAfterDiscount = maxAmount - discount;
                            due.Amount = amtAfterDiscount - paymentAmount;
                            due.FeeDuesGuid = Guid.NewGuid();
                            due.StudentId = StudentId;
                            due.IsActive = true;
                            due.DueDate = dueFee.DueDate;
                            due.TimeStamp = BillDate;
                            due.ParentGuid = payment.FeeGuid;
                            due.BillNo = BillNo;
                            _Entities.TbFeeDues.Add(due);
                            // status = _Entities.SaveChanges() > 0 ? true : false;
                        }
                    }
                }

                payment.Amount = paymentAmount;
                sumAmt = sumAmt + payment.Amount;
                payment.PaymentMode = model.PaymentType;
                if (model.PaymentType == 2)
                {
                    payment.ChequeDate = Convert.ToDateTime(model.ChequeDate);
                    payment.BankId = model.BankId;
                    payment.ChequeNumber = model.ChequeNumber;
                }
                else
                {
                    payment.BankId = 0;
                }
                payment.BillNo = BillNo;
                payment.IsPaid = true;
                payment.PaymentType = 1;
                payment.PaymentGuid = PaymentGuid;
                payment.StudentId = StudentId;
                payment.ClassId = ClassId;
                payment.SchoolId = SchoolId;
                payment.TimeStamp = BillDate;
                payment.IsActive = true;
                // Archana (12-12-2018)
                // Checking the bill have a partial paid balance payment, then the bill wants to save the first bill no 
                try
                {
                    long previousBillno = Convert.ToInt64(splitData[8]);
                    payment.PartialPaidParentBillNo = previousBillno;
                }
                catch
                {
                    payment.PartialPaidParentBillNo = 0;
                }
                _Entities.TbPayments.Add(payment);
                status = _Entities.SaveChanges() > 0 ? true : false;

                try
                {
                    var d = BillDate.ToString("MM-dd-yyyy");
                    DateTime todayDate = Convert.ToDateTime(d);
                    var incDetail = _Entities.TbIncomes.Where(z => z.AccountHead == "Fee Collected" && z.Date == todayDate && z.SchoolId == _user.SchoolId && z.IsActive).FirstOrDefault();
                    if (incDetail != null)
                    {
                        double? payAmt = Convert.ToDouble(paymentAmount);
                        double? amt = incDetail.Amount;
                        payAmt = payAmt + amt;
                        incDetail.Amount = Convert.ToDouble(payAmt);
                        status = _Entities.SaveChanges() > 0 ? true : false;
                    }
                    else
                    {
                        var income = new TbIncome();
                        income.AccountHead = "Fee Collected";
                        income.Amount = Convert.ToDouble(paymentAmount);
                        income.Particular = "Fee Income";
                        income.SchoolId = _user.SchoolId;
                        income.IsActive = true;
                        income.Date = todayDate;
                        _Entities.TbIncomes.Add(income);
                        status = _Entities.SaveChanges() > 0 ? true : false;
                    }
                }
                catch (Exception ex)
                {

                }

                #region Account Sction

                if (prevAdv < Convert.ToDecimal(paymentAmount))
                {
                    decimal currentPaiedAmountPerItem = Convert.ToDecimal(paymentAmount) - prevAdv;
                    #region The Payment mode is Cash
                    if (model.PaymentType == 1)// Cash
                    {
                        UpdateVoucher = true;
                        var cashEntry = new TbCashEntry();
                        if (vouchrTbl != null)
                            cashEntry.VoucherNumber = thisBillVoucherNumber.ToString();
                        else
                            cashEntry.VoucherNumber = "1";
                        cashEntry.BillNo = BillNo.ToString();
                        cashEntry.VoucherType = "RV";
                        cashEntry.TransactionType = "R";
                        cashEntry.Amount = currentPaiedAmountPerItem;
                        cashEntry.HeadId = headIdBill;
                        cashEntry.SubId = feeId;
                        cashEntry.Narration = "Fee Paid " + studDetails.StundentName;
                        cashEntry.EnterDate = BillDate;
                        cashEntry.UserId = _user.UserId;
                        cashEntry.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                        cashEntry.CancelStatus = false;
                        cashEntry.SchoolId = _user.SchoolId;
                        cashEntry.Migration = false;
                        cashEntry.IsActive = true;
                        cashEntry.TimeStamp = CurrentTime;
                        if (cashEntry.EnterDate.Date == CurrentTime.Date)
                            cashEntry.EditStatus = "N";
                        else if (cashEntry.EnterDate.Date < CurrentTime.Date)
                            cashEntry.EditStatus = "P";
                        else
                            cashEntry.EditStatus = "F";
                        cashEntry.ReverseStatus = false;
                        cashEntry.AdvanceStatus = false;
                        _Entities.TbCashEntries.Add(cashEntry);
                        _Entities.SaveChanges();

                        #region Data added to Balance table for Account
                        int sourceId = Convert.ToInt32(DataFromStatus.Cash);
                        var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == 0).FirstOrDefault();
                        if (balance != null)
                        {
                            balance.Closing = balance.Closing + currentPaiedAmountPerItem;
                            balance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            try
                            {
                                var balanceEntry = new TbBalance();
                                balanceEntry.SchoolId = _user.SchoolId;
                                balanceEntry.CurrentDate = BillDate;
                                balanceEntry.SourceId = sourceId;
                                DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == 0 && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                if (yesterday.Year != 0001)
                                    balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                else
                                    balanceEntry.Opening = 0;
                                balanceEntry.Closing = balanceEntry.Opening + currentPaiedAmountPerItem;
                                balanceEntry.IsActive = true;
                                balanceEntry.BankId = 0;
                                balanceEntry.TimeStamp = CurrentTime;
                                _Entities.TbBalances.Add(balanceEntry);
                                _Entities.SaveChanges();

                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        AllUpdatesInBalance(BillDate, sourceId, 0, currentPaiedAmountPerItem);

                        #endregion Data added to Balance table for Account
                    }
                    #endregion The Payment mode is Cash
                    #region The Payment mode is Bank
                    else// Bank
                    {
                        UpdateVoucher = true;
                        var bankEntry = new TbBankEntry();
                        if (vouchrTbl != null)
                            bankEntry.VoucherNumber = thisBillVoucherNumber.ToString();
                        else
                            bankEntry.VoucherNumber = "1";
                        bankEntry.VoucherType = "RV";
                        bankEntry.BillNo = BillNo.ToString();
                        bankEntry.TransactionType = "R";
                        bankEntry.Amount = currentPaiedAmountPerItem;
                        bankEntry.ModeType = model.PaymentType;
                        if (model.PaymentType == 2)
                        {
                            bankEntry.ChequeDate = Convert.ToDateTime(model.ChequeDate);
                            bankEntry.ChequeNumber = model.ChequeNumber;
                        }

                        bankEntry.HeadId = headIdBill;
                        bankEntry.SubId = feeId;
                        bankEntry.BankId = model.BankId;
                        bankEntry.Narration = "Fee Paid " + studDetails.StundentName;
                        bankEntry.EnterDate = BillDate;
                        bankEntry.UserId = _user.UserId;
                        bankEntry.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                        bankEntry.CancelStatus = false;
                        bankEntry.SchoolId = _user.SchoolId;
                        bankEntry.Migration = false;
                        bankEntry.IsActive = true;
                        bankEntry.TimeStamp = CurrentTime;
                        if (bankEntry.EnterDate.Date == CurrentTime.Date)
                            bankEntry.EditStatus = "N";
                        else if (bankEntry.EnterDate.Date < CurrentTime.Date)
                            bankEntry.EditStatus = "P";
                        else
                            bankEntry.EditStatus = "F";
                        _Entities.TbBankEntries.Add(bankEntry);
                        _Entities.SaveChanges();


                        #region Data added to Balance table for Account
                        int sourceId = Convert.ToInt32(DataFromStatus.Bank);
                        var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == model.BankId).FirstOrDefault();
                        if (balance != null)
                        {
                            balance.Closing = balance.Closing + currentPaiedAmountPerItem;
                            balance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            var balanceEntry = new TbBalance();
                            balanceEntry.SchoolId = _user.SchoolId;
                            balanceEntry.CurrentDate = BillDate;
                            balanceEntry.SourceId = sourceId;
                            DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == model.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == model.BankId).ToList().Sum(x => x.Closing);
                            else
                                balanceEntry.Opening = 0;
                            balanceEntry.Closing = balanceEntry.Opening + currentPaiedAmountPerItem;
                            balanceEntry.IsActive = true;
                            balanceEntry.BankId = model.BankId;
                            balanceEntry.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(balanceEntry);
                            _Entities.SaveChanges();
                        }

                        AllUpdatesInBalance(BillDate, sourceId, model.BankId, currentPaiedAmountPerItem);
                        #endregion Data added to Balance table for Account
                    }
                    #endregion The Payment mode is Bank
                    prevAdv = 0;//Here clear the all previous amount ,becuse it will reduced the current fee amount
                }
                else
                {
                    prevAdv = prevAdv - Convert.ToDecimal(paymentAmount);
                }
                #endregion Account Sction

            }
            if (UpdateVoucher == true)
            {
                var vouchrTbl2 = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                vouchrTbl2.ReceiptVoucher = vouchrTbl2.ReceiptVoucher + 1;
                _Entities.SaveChanges();
            }

            var billNo1 = _Entities.TbPaymentBillNos.Where(z => z.SchoolId == SchoolId).FirstOrDefault();
            if (billNo1 != null)
            {
                billNo.BillNo = BillNo;
                status = _Entities.SaveChanges() > 0 ? true : false;
            }
            try

            {
                decimal payableAmount = 0;
                decimal bal = 0;
                decimal prevBal = 0;
                //bool balAndCash = false;
                bool ispayable = false;

                decimal tempSumTotal = 0;
                tempSumTotal = sumAmt;
                if (sumAmt == 0)
                {
                    sumAmt = payment.Amount;
                }
                var balance = _Entities.TbStudentBalances.Where(z => z.StudentId == StudentId && z.IsActive).FirstOrDefault();
                if (balance != null)
                {
                    prevBal = balance.Amount;
                    bal = balance.Amount;
                    //sumAmt = (bal - sumAmt);
                    if ((prevBal < tempSumTotal) && (prevBal != 0))
                    {
                        ispayable = true;
                        payableAmount = tempSumTotal - prevBal;

                    }
                    //if (sumAmt > tempSumTotal)//check negetive or not 
                    //{
                    //    sumAmt = tempSumTotal;
                    //}
                    //else
                    //{
                    //    if (!ispayable)
                    //    {

                    //    balAndCash = true;
                    //    }
                    //    sumAmt = Math.Abs(sumAmt);
                    //}
                    if (TotalAmountPaid != 0)
                    {
                        var tempBal = TotalAmountPaid - sumAmt;
                        bal = tempBal + prevBal;
                    }
                    else
                    {
                        if (ispayable)
                        {
                            bal = 0;
                        }
                        else
                        {
                            bal = balance.Amount - sumAmt;
                        }
                    }
                    if (bal < 0) //if no balance available (balance.Amount - sumAmt) gets -ve
                    {
                        bal = 0;
                    }
                }
                else
                {
                    if (TotalAmountPaid != 0)
                    {
                        bal = TotalAmountPaid - sumAmt;
                    }
                    else
                    {
                        //bal = Math.Abs(bal - sumAmt);
                        bal = bal - sumAmt;
                    }
                    if (bal < 0)
                    {
                        bal = 0;
                    }
                }
                if (balance != null)
                {
                    try
                    {
                        balance.Amount = bal;
                        _Entities.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var messageex = ex.Message;
                    }
                }
                else
                {
                    if (bal != 0)
                    {
                        var studentBalance = new TbStudentBalance();
                        studentBalance.StudentId = StudentId;
                        studentBalance.Amount = bal;
                        studentBalance.IsActive = true;
                        _Entities.TbStudentBalances.Add(studentBalance);
                        status = _Entities.SaveChanges() > 0 ? true : false;
                    }
                }
                if ((TotalAmountPaid != 0) || (bal != 0))
                {
                    var studentPaidsAmount = new TbStudentPaidAmount();
                    studentPaidsAmount.StudentId = StudentId;
                    studentPaidsAmount.PaidAmount = TotalAmountPaid;
                    studentPaidsAmount.PreviousBalance = prevBal;
                    studentPaidsAmount.BalanceAmount = bal;
                    studentPaidsAmount.BillNo = BillNo;
                    studentPaidsAmount.AddAccountStatus = false;
                    studentPaidsAmount.IsActive = true;
                    _Entities.TbStudentPaidAmounts.Add(studentPaidsAmount);
                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
                //else if (bal != 0)
                //{
                //    var studentPaidsAmount = new tb_StudentPaidAmount();
                //    studentPaidsAmount.StudentId = StudentId;
                //    studentPaidsAmount.PaidAmount = TotalAmountPaid;
                //    studentPaidsAmount.PreviousBalance = prevBal;
                //    studentPaidsAmount.BalanceAmount = bal;
                //    studentPaidsAmount.BillNo = BillNo;
                //    studentPaidsAmount.IsActive = true;
                //    _Entities.TbStudentsPaidAmount.Add(studentPaidsAmount);
                //    status = _Entities.SaveChanges() > 0 ? true : false;
                //}
                if (ispayable)
                {
                    var studentPaidsAmount = new TbStudentPaidAmount();
                    studentPaidsAmount.StudentId = StudentId;
                    studentPaidsAmount.PaidAmount = payableAmount;
                    studentPaidsAmount.PreviousBalance = prevBal;
                    studentPaidsAmount.BalanceAmount = bal;
                    studentPaidsAmount.BillNo = BillNo;
                    studentPaidsAmount.IsActive = true;
                    studentPaidsAmount.AddAccountStatus = false;
                    _Entities.TbStudentPaidAmounts.Add(studentPaidsAmount);
                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
                #region Archana : Is the student pay more amount than he payable ?
                decimal currentPayableAmt = tempSumTotal - prevBal; // Fee Item Total - Previous Amount
                if (currentPayableAmt > 0)// The Student wnats to pay now
                {
                    decimal advance = TotalAmountPaid - currentPayableAmt;// The actual bill amount after the previous advance
                    long subId = 0;
                    if (advance > 0)
                    {
                        var head = _Entities.TbAccountHeads.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ForBill == true).FirstOrDefault();
                        if (head != null)
                        {
                            var sub = _Entities.TbSubLedgerData.Where(x => x.AccHeadId == head.AccountId && x.IsActive).FirstOrDefault();
                            if (sub == null)
                            {
                                var subAdd = new TbSubLedgerDatum();
                                subAdd.SubLedgerName = "Advance Amount";
                                subAdd.AccHeadId = head.AccountId;
                                subAdd.IsActive = true;
                                subAdd.TimeStamp = CurrentTime;
                                _Entities.TbSubLedgerData.Add(subAdd);
                                _Entities.SaveChanges();
                                subId = subAdd.LedgerId;
                            }
                            else
                            {
                                subId = sub.LedgerId;
                            }

                            if (model.PaymentType == 1)// Cash
                            {
                                var advCash = new TbCashEntry();
                                advCash.VoucherNumber = thisBillVoucherNumber.ToString();
                                advCash.VoucherType = "RV";
                                advCash.BillNo = "";
                                advCash.TransactionType = "R";
                                advCash.Amount = advance;
                                advCash.HeadId = head.AccountId;
                                advCash.SubId = subId;
                                advCash.Narration = "Advance Paid " + studDetails.StundentName;
                                advCash.EnterDate = BillDate;
                                advCash.UserId = _user.UserId;
                                advCash.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                                advCash.CancelStatus = false;
                                advCash.SchoolId = _user.SchoolId;
                                advCash.Migration = false;
                                advCash.IsActive = true;
                                advCash.TimeStamp = CurrentTime;
                                if (BillDate.Date > CurrentTime.Date)
                                    advCash.EditStatus = "P";
                                else
                                    advCash.EditStatus = "N";
                                advCash.ReverseStatus = false;
                                advCash.AdvanceStatus = false;
                                _Entities.TbCashEntries.Add(advCash);
                                _Entities.SaveChanges();




                                int sourceId = Convert.ToInt32(DataFromStatus.Cash);
                                var balanceNow = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == 0).FirstOrDefault();
                                if (balance != null)
                                {
                                    balanceNow.Closing = balanceNow.Closing + advance;
                                    balanceNow.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = BillDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == model.BankId).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening + advance;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = model.BankId;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                AllUpdatesInBalance(BillDate, sourceId, 0, advance);

                            }
                            else
                            {
                                var advBank = new TbBankEntry();
                                advBank.VoucherNumber = thisBillVoucherNumber.ToString();
                                advBank.VoucherType = "RV";
                                advBank.BillNo = "";
                                advBank.TransactionType = "R";
                                advBank.Amount = advance;
                                advBank.ModeType = model.PaymentType;
                                if (model.PaymentType == 2)
                                {
                                    advBank.ChequeDate = Convert.ToDateTime(model.ChequeDate);
                                    advBank.ChequeNumber = model.ChequeNumber;
                                }
                                advBank.HeadId = head.AccountId;
                                advBank.SubId = subId;
                                advBank.BankId = model.BankId;
                                advBank.Narration = "Advance Paid " + studDetails.StundentName;
                                advBank.EnterDate = BillDate;
                                advBank.UserId = _user.UserId;
                                advBank.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                                advBank.CancelStatus = false;
                                advBank.SchoolId = _user.SchoolId;
                                advBank.Migration = false;
                                advBank.IsActive = true;
                                advBank.TimeStamp = CurrentTime;
                                if (BillDate.Date > CurrentTime.Date)
                                    advBank.EditStatus = "P";
                                else
                                    advBank.EditStatus = "N";
                                _Entities.TbBankEntries.Add(advBank);
                                _Entities.SaveChanges();


                                int sourceId = Convert.ToInt32(DataFromStatus.Bank);
                                var balanceNow = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == model.BankId).FirstOrDefault();
                                if (balance != null)
                                {
                                    balanceNow.Closing = balanceNow.Closing + advance;
                                    balanceNow.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = BillDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == model.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == model.BankId).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening + advance;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = model.BankId;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                AllUpdatesInBalance(BillDate, sourceId, 0, advance);

                            }
                            if (UpdateVoucher == false)
                            {
                                var vouchrTbl2 = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                                vouchrTbl2.ReceiptVoucher = vouchrTbl2.ReceiptVoucher + 1;
                                _Entities.SaveChanges();
                            }
                        }
                    }
                }
                else // The student does not want to pay , the all bill amount will satisfies the previous advance amount
                {
                    if (TotalAmountPaid > 0)//The student paid amount ,when their is no need to paid
                    {
                        long subId = 0;
                        var head = _Entities.TbAccountHeads.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ForBill == true).FirstOrDefault();
                        if (head != null)
                        {
                            var sub = _Entities.TbSubLedgerData.Where(x => x.AccHeadId == head.AccountId && x.IsActive).FirstOrDefault();
                            if (sub == null)
                            {
                                var subAdd = new TbSubLedgerDatum();
                                subAdd.SubLedgerName = "Advance Amount";
                                subAdd.AccHeadId = head.AccountId;
                                subAdd.IsActive = true;
                                subAdd.TimeStamp = CurrentTime;
                                _Entities.TbSubLedgerData.Add(subAdd);
                                _Entities.SaveChanges();
                                subId = subAdd.LedgerId;
                            }
                            else
                            {
                                subId = sub.LedgerId;
                            }
                            if (model.PaymentType == 1)// Cash
                            {
                                var advCash = new TbCashEntry();
                                advCash.VoucherNumber = thisBillVoucherNumber.ToString();
                                advCash.VoucherType = "RV";
                                advCash.BillNo = "";
                                advCash.TransactionType = "R";
                                advCash.Amount = TotalAmountPaid;
                                advCash.HeadId = head.AccountId;
                                advCash.SubId = subId;
                                advCash.Narration = "Advance Paid " + studDetails.StundentName;
                                advCash.EnterDate = BillDate;
                                advCash.UserId = _user.UserId;
                                advCash.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                                advCash.CancelStatus = false;
                                advCash.SchoolId = _user.SchoolId;
                                advCash.Migration = false;
                                advCash.IsActive = true;
                                advCash.TimeStamp = CurrentTime;
                                if (BillDate.Date > CurrentTime.Date)
                                    advCash.EditStatus = "P";
                                else
                                    advCash.EditStatus = "N";
                                advCash.ReverseStatus = false;
                                advCash.AdvanceStatus = false;
                                _Entities.TbCashEntries.Add(advCash);
                                _Entities.SaveChanges();


                                int sourceId = Convert.ToInt32(DataFromStatus.Cash);
                                var balanceNow = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == 0).FirstOrDefault();
                                if (balance != null)
                                {
                                    balanceNow.Closing = balanceNow.Closing + TotalAmountPaid;
                                    balanceNow.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = BillDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x =>x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == model.BankId).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening + TotalAmountPaid;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = model.BankId;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                AllUpdatesInBalance(BillDate, sourceId, 0, TotalAmountPaid);
                            }
                            else
                            {
                                var advBank = new TbBankEntry();
                                advBank.VoucherNumber = thisBillVoucherNumber.ToString();
                                advBank.VoucherType = "RV";
                                advBank.BillNo = " ";
                                advBank.TransactionType = "R";
                                advBank.Amount = TotalAmountPaid;
                                advBank.ModeType = model.PaymentType;
                                if (model.PaymentType == 2)
                                {
                                    advBank.ChequeDate = Convert.ToDateTime(model.ChequeDate);
                                    advBank.ChequeNumber = model.ChequeNumber;
                                }
                                advBank.HeadId = head.AccountId;
                                advBank.SubId = subId;
                                advBank.BankId = model.BankId;
                                advBank.Narration = "Advance Paid " + studDetails.StundentName;
                                advBank.EnterDate = BillDate;
                                advBank.UserId = _user.UserId;
                                advBank.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bill);
                                advBank.CancelStatus = false;
                                advBank.SchoolId = _user.SchoolId;
                                advBank.Migration = false;
                                advBank.IsActive = true;
                                advBank.TimeStamp = CurrentTime;
                                if (BillDate.Date > CurrentTime.Date)
                                    advBank.EditStatus = "P";
                                else
                                    advBank.EditStatus = "N";
                                _Entities.TbBankEntries.Add(advBank);
                                _Entities.SaveChanges();


                                int sourceId = Convert.ToInt32(DataFromStatus.Bank);
                                var balanceNow = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == BillDate.Date && x.SourceId == sourceId && x.BankId == model.BankId).FirstOrDefault();
                                if (balance != null)
                                {
                                    balanceNow.Closing = balanceNow.Closing + TotalAmountPaid;
                                    balanceNow.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = BillDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x =>x.CurrentDate.Date < BillDate && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == model.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == model.BankId).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening + TotalAmountPaid;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = model.BankId;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                AllUpdatesInBalance(BillDate, sourceId, 0, TotalAmountPaid);

                            }
                            if (UpdateVoucher == false)
                            {
                                var vouchrTbl2 = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                                vouchrTbl2.ReceiptVoucher = vouchrTbl2.ReceiptVoucher + 1;
                                _Entities.SaveChanges();
                            }
                        }
                    }

                }

                #endregion
            }
            catch (Exception ex)
            {


            }

            var dateTime = BillDate.ToString("dd-MMM-yyyy");

            var description = "failed";
            #region Email
            try
            {
                var smtpDetails = _Entities.TbSmtpdetails.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
                var paidAmount = Convert.ToInt32(payment.Amount);
                //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/email/FeePayment.html");
                var filePath = Path.Combine(_env.WebRootPath, "Content", "email", "FeePayment.html");

                var emailTemplate = System.IO.File.ReadAllText(filePath);
                var mailBody = emailTemplate.Replace("{{schoolname}}", studDetails.School.SchoolName)
                   .Replace("{{parent}}", studDetails.ParentName)
                .Replace("{{student}}", studDetails.StundentName)
                .Replace("{{amount}}", string.Format("{0:0.00}", sumAmt))
                .Replace("{{date}}", dateTime);
                Mail.Send("School Fee Payment", mailBody, studDetails.ParentName, smtpDetails.EmailId, smtpDetails.Password, new System.Collections.ArrayList { studDetails.ParentEmail });
                description = "success";
            }
            catch
            {
                description = "Something went wrong";
            }
            #endregion Email
            var package = _Entities.TbSmsPackages.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.IsDisabled == false).FirstOrDefault();
            if (package != null)
            {
                try
                {
                    if (model.TimeStamp.ToString("MM-dd-YYYY") == CurrentTime.ToString("MM-dd-YYYY"))
                    {

                        #region  SMS 
                        HttpClient client = new HttpClient();
                        var history = new TbSmsHistory();
                        var numbers = new List<string>();
                        var MsgId = new List<string>();
                        var numb = "";
                        string messagepre = "";
                        var senderName = "MYSCHO";

                        var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                        if (senderData != null)
                            senderName = senderData.SenderId;
                        message = "success";
                        status = true;
                        var smsHead = new TbSmsHead();
                        smsHead.Head = "BillDate Payment " + studDetails.StundentName;
                        smsHead.SchoolId = _user.SchoolId;
                        smsHead.TimeStamp = CurrentTime;
                        smsHead.IsActive = true;
                        smsHead.SenderType = (int)SMSSendType.Student;
                        _Entities.TbSmsHeads.Add(smsHead);
                        status = _Entities.SaveChanges() > 0;


                        messagepre = "Dear Parent of " + studDetails.StundentName + ", you have paid Rs." + string.Format("{0:0.00}", sumAmt) + " on " + dateTime;

                        var phone = studDetails.ContactNumber.ToString();
                        int length = messagepre.Length;
                        int que = length / 160;
                        int rem = length % 160;
                        if (rem > 0)
                            que++;
                        int smsCount = que;
                        var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + messagepre + "&sender=" + senderName;
                        //  var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + item.Description + "&priority=ndnd&stype=normal";

                        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                        HttpWebRequest request = this.GetRequest(url);
                        WebResponse webResponse = request.GetResponse();
                        var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                        var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                        alvosmsResp respList = JsonConvert.DeserializeObject<alvosmsResp>(responseText);
                        if (status)
                        {
                            TbSmsHistory sms = new TbSmsHistory();
                            sms.IsActive = true;
                            sms.MessageContent = messagepre;
                            sms.MessageDate = CurrentTime;
                            sms.ScholId = _user.SchoolId;
                            sms.StuentId = StudentId;
                            sms.MobileNumber = phone;
                            sms.HeadId = smsHead.HeadId;
                            sms.SendStatus = Convert.ToString(respList.success);
                            if (respList.data != null)
                            {
                                sms.MessageReturnId = respList.data[0].messageId;
                                sms.DelivaryStatus = "Pending";
                            }
                            sms.SmsSentPerStudent = smsCount;
                            _Entities.TbSmsHistories.Add(sms);
                            _Entities.SaveChanges();
                        }
                        #endregion  SMS 
                    }
                }
                catch (Exception ex)
                {
                    var x = ex.InnerException;
                }
            }
            return Json(new { status = status, serialNo = BillNo, msg = status ? "Bill Paid Sucessfully" : "Failed To Pay Bill" });
        }

        private void AllUpdatesInBalance(DateTime billDate, int sourceId, long BankId, decimal amount)
        {
            if (sourceId == Convert.ToInt32(DataFromStatus.Cash))
            {
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.IsActive == true && x.BankId == 0 &&x.CurrentDate.Date > billDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        item.Opening = item.Opening + amount;
                        item.Closing = item.Closing + amount;
                        _Entities.SaveChanges();
                    }
                }
            }
            else
            {
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceId && x.BankId == BankId &&x.CurrentDate.Date > billDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        item.Opening = item.Opening + amount;
                        item.Closing = item.Closing + amount;
                        _Entities.SaveChanges();
                    }
                }
            }
        }

        private void AllUpdatesInBalanceCancel(DateTime billDate, int sourceId, long BankId, decimal amount)
        {
            if (sourceId == Convert.ToInt32(DataFromStatus.Cash))
            {
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.IsActive == true && x.BankId == 0 &&x.CurrentDate.Date >= billDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        if (item.CurrentDate.Date == billDate.Date)
                        {
                            item.Closing = item.Closing + amount;
                        }
                        else
                        {
                            item.Opening = item.Opening + amount;
                            item.Closing = item.Closing + amount;
                        }
                        _Entities.SaveChanges();
                    }
                }
            }
            else
            {
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceId && x.BankId == BankId &&x.CurrentDate.Date >= billDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        if (item.CurrentDate.Date == billDate.Date)
                        {
                            item.Closing = item.Closing + amount;
                        }
                        else
                        {
                            item.Opening = item.Opening + amount;
                            item.Closing = item.Closing + amount;
                        }
                        _Entities.SaveChanges();
                    }
                }
            }
        }
        public ActionResult Fee()
        {
            var model = new Satluj_Latest.Models.AddClassFees();
            model.SchoolId = _user.SchoolId;
            ViewBag.CommonFees = _dropdown.RefreshSchoolFees(model.SchoolId);
            return View(model);
        }
        public PartialViewResult AddFeePartialView()
        {
            var model = new Satluj_Latest.Models.AddFee();
            model.SchoolId = _user.SchoolId;
            int year = CurrentTime.Year;   //Archana
            DateTime firstDay = new DateTime(year, 1, 1);
            model.DueDateString = firstDay.ToString("dd-MM-yyyy");
            model.EndDateString = firstDay.ToString("dd-MM-yyyy");
            model.HaveFineDate = CurrentTime.AddDays(1);
            model.HaveFineDateString = model.HaveFineDate.ToShortDateString();
            return PartialView("~/Views/School/_pv_AddFee.cshtml", model);
        }

        [HttpPost]
        public object AddFee(SchoolValue.AddFee model)
        {
            DateTime dueDate = ConvertDateToServer(model.DueDateString);
            DateTime endDate = ConvertDateToServer(model.EndDateString);

            bool status = false;
            string message = "Failed";
            if (_Entities.TbFees.Any(z => z.FeesName.ToLower() == model.FeeName.ToLower() && z.IsActive && z.SchoolId == _user.SchoolId))
            {
                message = "Fee Already Added";
            }
            else
            {
                var fee = new TbFee();
                fee.FeesName = model.FeeName;
                fee.FeeType = model.FeeType;
                fee.IsActive = true;
                fee.Interval = model.Interval;
                fee.SchoolId = model.SchoolId;
                fee.TimeStamp = CurrentTime;
                if (model.IsReccuring == 1)
                {
                    if (model.Interval > 1)
                    {
                        fee.FeeStartDate = dueDate;
                    }
                    else
                        fee.FeeStartDate = CurrentTime;
                }
                else if (model.IsDueDate == 1)
                {
                    fee.FeeStartDate = endDate;
                }
                else
                {
                    fee.FeeStartDate = CurrentTime;
                }

                //--------------Archana new fine calculation-----------------
                try
                {
                    if (model.HaveFineDateString != string.Empty && model.HaveFineDateString != null)
                    {
                        string[] splitData = model.HaveFineDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        fee.DueDate = Convert.ToDateTime(date);
                    }
                }
                catch
                {

                }
                fee.FineAmount = model.FineAmount;
                fee.NoOfDays = model.FineDays;
                //--------------Archana new fine calculation-----------------

                _Entities.TbFees.Add(fee);
                status = _Entities.SaveChanges() > 0;
                message = status ? " Fee Added" : "failed";
            }
            return Json(new { status = status, msg = message, list = new Satluj_Latest.Data.DropdownData().RefreshSchoolFees(model.SchoolId) });
        }

        [HttpPost]
        public object AddFeeClass(Satluj_Latest.Models.AddClassFees model)
        {
            bool status = false;
            string message = "Failed";

            //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            //List<Datalist> routes_list =
            //         (List<Datalist>)json_serializer.DeserializeObject(model.DataList);
            List<Datalist> routes_list = JsonConvert.DeserializeObject<List<Datalist>>(model.DataList);

            //List<Datalist> routes_list = new JavaScriptSerializer().Deserialize<List<Datalist>>(model.DataList);
            long FeeId = Convert.ToInt64(model.FeeId);
            var FeeDetail = _Entities.TbFees.Where(z => z.FeeId == FeeId && z.IsActive).FirstOrDefault();
            foreach (var value in routes_list)
            {
                DateTime dueDt = FeeDetail.FeeStartDate;
                var feeclass = new TbFeeClass();
                if (FeeDetail.Interval > 1)
                {
                    var addMonth = 12 / FeeDetail.Interval;
                    for (int i = 1; i <= FeeDetail.Interval; i++)
                    {
                        feeclass.Amount = Convert.ToDecimal(value.amount);
                        feeclass.ClassId = value.classId;
                        feeclass.FeeClassGuid = Guid.NewGuid();
                        feeclass.FeeId = FeeId;
                        feeclass.IsActive = true;
                        feeclass.PublishStatus = true;
                        feeclass.TimeStamp = CurrentTime;
                        feeclass.DueDate = dueDt;
                        feeclass.Instalment = i;
                        dueDt = dueDt.AddMonths(addMonth);
                        _Entities.TbFeeClasses.Add(feeclass);
                        status = _Entities.SaveChanges() > 0;
                    }
                }
                else
                {
                    feeclass.Amount = Convert.ToDecimal(value.amount);
                    feeclass.ClassId = value.classId;
                    feeclass.FeeClassGuid = Guid.NewGuid();
                    feeclass.FeeId = Convert.ToInt64(model.FeeId);
                    feeclass.IsActive = true;
                    feeclass.PublishStatus = true;
                    feeclass.TimeStamp = CurrentTime;
                    feeclass.DueDate = dueDt;
                    feeclass.Instalment = FeeDetail.Interval;
                    _Entities.TbFeeClasses.Add(feeclass);
                    status = _Entities.SaveChanges() > 0;
                }
            }
            message = status ? " Fee Added" : "failed";
            return Json(new { status = status, msg = message });
        }

        public PartialViewResult ViewFeeClass(string id)
        {
            var model = new Satluj_Latest.Models.ViewFeeClass();
            model.SchoolId = (int)_user.SchoolId;
            model.FeeId = (int)Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_ViewFeeClass.cshtml", model);
        }
        public ActionResult FeeList()
        {
            var model = new Satluj_Latest.Models.ListFee();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public PartialViewResult RefreshFeelistPartial()
        {
            var model = new Satluj_Latest.Models.ListFee();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_Feelist.cshtml", model);
        }

        public object DeleteFee(string id)
        {
            bool status = false;
            string message = "Failed";
            long feeId = Convert.ToInt64(id);
            var fee = _Entities.TbFees.FirstOrDefault(z => z.FeeId == feeId);
            if (fee != null)
                fee.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            message = status ? "Fee Deleted" : "failed";
            return Json(new { status = status, msg = message });
        }

        public object EditFee(SchoolValue.EditFee model)
        {
            bool status = false;
            string message = "Failed";
            long feeId = Convert.ToInt64(model.feeId);
            if (_Entities.TbFees.Any(z => z.FeesName.ToLower() == model.feename.ToLower() && z.SchoolId == _user.SchoolId && z.FeeId != feeId && z.IsActive))
            {
                message = "Duplicate Feename";
            }
            else
            {
                var fee = _Entities.TbFees.FirstOrDefault(z => z.FeeId == feeId);
                if (fee != null)
                {
                    fee.FeesName = model.feename;
                    fee.FeeType = model.feeType;
                    _Entities.SaveChanges();
                    status = true;
                }
                message = status ? "Fee Edited" : "failed";
            }
            return Json(new { status = status, msg = message });
        }

        public PartialViewResult EditFeePartial(string id)
        {
            var model = new EditFee();
            model.feeId = Convert.ToInt64(id);
            var fee = _Entities.TbFees.FirstOrDefault(z => z.FeeId == model.feeId);
            if (fee != null)
            {
                model.feename = fee.FeesName;
                model.feeType = fee.FeeType;
            }
            else
            {
                model.feename = string.Empty;
                model.feeType = Convert.ToInt32(1);
            }
            return PartialView("~/Views/School/_pv_EditFee.cshtml", model);
        }
        public ActionResult FeeClassList(string id)
        {
            var model = new Satluj_Latest.Models.FeeclassList();
            model.schoolId = _user.SchoolId;
            model.feeId = Convert.ToInt64(id);
            return View(model);
        }
        public PartialViewResult RefreshFeeClasslistPartial(string id)
        {
            var model = new Satluj_Latest.Models.FeeclassList();
            model.schoolId = _user.SchoolId;
            model.feeId = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_FeeClassList.cshtml", model);
        }

        public object DeleteFeeClass(string id)
        {
            bool status = false;
            string message = "Failed";
            long feeClassId = Convert.ToInt64(id);
            var feeClass = _Entities.TbFeeClasses.FirstOrDefault(z => z.FeeClassId == feeClassId);
            if (feeClass != null)
                feeClass.IsActive = false;
            status = _Entities.SaveChanges() > 0;
            message = status ? "Deleted" : "failed";
            return Json(new { status = status, msg = message });
        }


        public PartialViewResult FeeClassEditPartial(string id)
        {
            var model = new Satluj_Latest.Models.EditFeeClass();
            model.feeClassId = Convert.ToInt64(id);
            var feeclass = _Entities.TbFeeClasses.FirstOrDefault(z => z.FeeClassId == model.feeClassId);
            if (feeclass != null)
            {
                model.amount = Convert.ToDecimal(feeclass.Amount);
                model.classname = feeclass.Class.Class;
                model.feeId = feeclass.FeeId;
                model.DueDate = feeclass.DueDate;
            }
            else
            {
                model.amount = Convert.ToDecimal(0.00);
                model.classname = string.Empty;
                model.feeId = feeclass.FeeId;

            }
            return PartialView("~/Views/School/_pv_FeeClassEdit.cshtml", model);
        }

        [HttpPost]
        public object EditFeeClass(SchoolValue.EditFeeClass model)
        {
            bool status = false;
            string message = "Failed";
            long feeClassId = Convert.ToInt64(model.feeClassId);
            var feeClass = _Entities.TbFeeClasses.FirstOrDefault(z => z.FeeClassId == feeClassId);
            if (feeClass != null)
            {
                feeClass.Amount = Convert.ToDecimal(model.amount);
                feeClass.DueDate = Convert.ToDateTime(model.DueDate);
                _Entities.SaveChanges();
                status = true;
            }
            message = status ? "Edited" : "failed";
            return Json(new { status = status, msg = message, feeId = model.feeId });
        }

        public object DeleteClass(string id)
        {
            bool status = false;
            string message = "Failed";
            long DivisionId = Convert.ToInt64(id);
            var Division = _Entities.TbDivisions.FirstOrDefault(z => z.DivisionId == DivisionId);
            if (Division != null)
            {
                Division.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    if (!_Entities.TbDivisions.Any(z => z.ClassId == Division.ClassId && z.IsActive))
                    {
                        var classData = _Entities.TbClasses.FirstOrDefault(z => z.ClassId == Division.ClassId);
                        if (classData != null)
                        {
                            classData.IsActive = false;
                            _Entities.SaveChanges();
                        }
                    }
                }
            }
            message = status ? "Deleted" : "failed";
            return Json(new { status = status, msg = message, list = new Satluj_Latest.Data.DropdownData().RefreshClasses(_user.SchoolId) });
        }

        public PartialViewResult EditStudentModel(string id)
        {
            var model = new EditStudentModel();
            model.studentId = Convert.ToInt64(id);
            model.schoolId = _user.SchoolId;
            var student = new Satluj_Latest.Data.Student(model.studentId);
            if (student != null)
            {
                model.address = student.Address;
                model.BusId = student.BusId.ToString();
                model.city = student.City;
                model.classId = student.ClassId;
                model.parentEmail = student.ParentEmail;
                model.className = student.ClassName;
                model.bloodGroup = student.BloodGroup;
                model.gender = student.Gender;
                model.contactNo = student.ContactNumber;
                model.division = student.DivisionName;
                model.divisionId = student.DivisionId;
                model.filePath = student.FilePath;
                model.parentName = student.ParentName;
                model.admissionNo = student.StudentSpecialId;
                model.state = student.State;
                model.tripNumber = student.TripNo;
                model.studentName = student.StundentName;
                //model.DOBstring = student.DOB?.ToString("dd-MM-yyyy"); // 12-Jun-2018 Archana changed these line from Kathus system , becouse of an error
                try
                {
                    model.DOBstring = student.DOB?.ToString("dd-MM-yyyy");
                }
                catch
                {
                    model.DOBstring = student.DOB.ToString();
                }
                model.MobileNo = student.MobileNo;
                model.PlaceOfBirth = student.PlaceOfBirth;
                model.MotherTongue = student.MotherTongue;
                //model.DateOfJoining = student.DateOfJoining ?? CurrentTime;
                try
                {
                    model.DateOfJoiningString = student.DateOfJoining?.ToString("dd-MM-yyyy");
                }
                catch
                {
                    model.DateOfJoiningString = student.DateOfJoining.ToString();
                }
                if (student.NationalityId != null)
                    model.NationalityId = (Nationality)student.NationalityId;
                if (student.CategoryId != null)
                    model.CategoryId = (StudentCategory)student.CategoryId;
                if (student.CountryId != null)
                    model.CountryId = (Country)student.CountryId;
                model.RollNo1 = student.ClasssNumber ?? "";
                model.Pincode = student.PostalCode;
            }


            //return PartialView("~/Views/School/_pv_EditStudentModel.cshtml", model);
            return PartialView("~/Views/School/_pv_Edit_Student_Model.cshtml", model);
        }

        [HttpPost]
        public object EditStudent(EditStudentModel model)
        {
            bool status = false;
            string msg = string.Empty;
            var stud = _Entities.TbStudents.FirstOrDefault(z => z.StudentId == model.studentId);
            if (stud != null)
            {
                stud.StundentName = model.studentName;
                //stud.ParentName = model.parentName;
                stud.Address = model.address;
                stud.City = model.city;
                stud.ParentEmail = model.parentEmail;
                stud.ContactNumber = model.contactNo;
                stud.Gender = model.gender;
                stud.BloodGroup = model.bloodGroup;
                //stud.BusId = model.BusId == null ? Convert.ToInt64(9) : Convert.ToInt64(model.BusId);
                //stud.TripNo = model.tripNumber;
                //stud.BioNumber = model.biometricId;
                stud.TimeStamp = CurrentTime;
                stud.StudentSpecialId = model.admissionNo;
                stud.State = model.state;
                //Profile Pic
                if (model.profilePic != null)
                {
                    //string folderPath = Server.MapPath("~/Media/Student/Profile/");
                    string folderPath = Path.Combine(_env.WebRootPath, "Media", "Student", "Profile");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    var imageString = model.profilePic.Substring(model.profilePic.IndexOf(',') + 1);
                    byte[] imageByte = Convert.FromBase64String(imageString);
                    string imageName = Guid.NewGuid().ToString() + ".jpeg";
                    var imgFilePath = Path.Combine(folderPath, imageName);
                    var fileSave = "/Media/Student/Profile/" + imageName;

                    using (var imageFile = new FileStream(imgFilePath, FileMode.Create))
                    {
                        imageFile.Write(imageByte, 0, imageByte.Length);
                        imageFile.Flush();
                        stud.FilePath = fileSave;
                    }
                }
                //Profile Pic
                try
                {
                    if (model.DOBstring != string.Empty && model.DOBstring != null)
                    {
                        string[] splitData = model.DOBstring.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var dob = mm + '-' + dd + '-' + yyyy;
                        stud.Dob = Convert.ToDateTime(dob);
                    }
                }
                catch
                {

                }
                try
                {
                    if (model.DateOfJoiningString != string.Empty && model.DateOfJoiningString != null)
                    {
                        string[] splitData = model.DateOfJoiningString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var doj = mm + '-' + dd + '-' + yyyy;
                        stud.DateOfJoining = Convert.ToDateTime(doj);
                    }
                }
                catch
                {

                }
                stud.MobileNo = model.MobileNo;
                stud.ClasssNumber = model.RollNo1;
                stud.NationalityId = Convert.ToInt32(model.NationalityId);
                stud.CountryId = Convert.ToInt32(model.CountryId);
                stud.CategoryId = Convert.ToInt32(model.CategoryId);
                stud.PlaceOfBirth = model.PlaceOfBirth;
                stud.MotherTongue = model.MotherTongue;
                stud.PostalCode = model.Pincode;
                try
                {
                    //_Entities.Entry(stud).State = EntityState.Modified;
                    _Entities.Entry(stud).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {

                }
                msg = status ? "Student edited successfully!" : "Failed to add Student!";
            }
            return Json(new { status = status, msg = msg, division = stud == null ? model.divisionId.ToString() : stud.DivisionId.ToString() });
        }

        public object DeleteStudent(string id)
        {
            bool status = false;
            string message = "Failed";
            long studentId = Convert.ToInt64(id);
            var student = _Entities.TbStudents.FirstOrDefault(z => z.StudentId == studentId);
            if (student != null)
            {
                student.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? "Deleted" : "failed";
            return Json(new { status = status, msg = message, division = student == null ? Convert.ToString(0) : student.DivisionId.ToString() });
        }

        public ActionResult DiscountAssignClassList()
        {
            var model = new Satluj_Latest.Models.FilterModel();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public ActionResult FeeDiscount()
        {
            var model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            ViewBag.Classlist = _dropdown.GetClasses(model.schoolId);
            return View(model);
        }
        public ActionResult DiscountDetails(string id)
        {
            var model = new Satluj_Latest.Models.DiscountDetails();
            model.StudentId = Convert.ToInt64(id);
            return View(model);
        }
        public object DeleteDiscount(string id)
        {
            bool status = false;
            string message = "Failed";
            string[] splitData = id.Split('~');
            long studentId = Convert.ToInt32(splitData[0]);
            long feeId = Convert.ToInt32(splitData[1]);


            var discount = _Entities.TbFeeDiscounts.Where(z => z.StudentId == studentId && z.IsActive && z.FeeId == feeId).FirstOrDefault();
            if (discount != null)
            {
                discount.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? " Discount deleted" : "Failed to delete discount";

            return Json(new { status = status, msg = message });
        }

        public PartialViewResult RefreshFeeDiscountGrid()
        {
            var model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_DiscountStudent_Grid.cshtml", model);

        }
        public PartialViewResult EditFeeDiscountStudView(string id)
        {
            var model = new FeeModel();
            string[] splitData = id.Split('~');
            model.StudentId = Convert.ToInt32(splitData[0]);
            model.FeeId = Convert.ToInt32(splitData[1]);
            var discount = _Entities.TbFeeDiscounts.Where(z => z.StudentId == model.StudentId && z.IsActive && z.FeeId == model.FeeId).FirstOrDefault();
            model.FeeName = discount.Fee.FeesName;
            model.StudentName = discount.Student.StundentName;
            model.DiscountAmount = Convert.ToDecimal(String.Format("{0:0.00}", discount.DiscountAmount));
            return PartialView("~/Views/School/_pv_EditFeeDiscountStudent_model.cshtml", model);

        }
        [HttpPost]
        public object EditFeeStudentDiscount(FeeModel model)
        {
            bool status = false;
            string message = "Failed";
            long studentId = model.StudentId;
            long feeId = model.FeeId;
            var discount = _Entities.TbFeeDiscounts.Where(z => z.StudentId == studentId && z.IsActive && z.FeeId == feeId).FirstOrDefault();
            if (discount != null)
            {
                discount.DiscountAmount = model.DiscountAmount;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? " Discount edit" : "Failed to edit discount";
            return Json(new { status = status, msg = message });
        }
        public PartialViewResult GetDiscountSyudentsByDivGrid(string id)
        {
            SchoolModel model = new SchoolModel();
            model.divisionId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_DiscountStudent_ByDivision_Grid.cshtml", model);

        }
        public ActionResult AssignDiscount(int id)
        {
            long divisionId = Convert.ToInt32(id);
            var model = new FeeModel();
            model.DivisionId = divisionId;
            model.SchoolId = _user.SchoolId;
            return View(model);
        }


        [HttpPost]
        public object AssignDiscount(FeeModel model)
        {
            bool status = false;
            string message = "Failed";
            List<string> studentsUserId = model.FeeStudentId.Split(',').ToList();

            var discount = new TbFeeDiscount();
            foreach (var userId in studentsUserId)
            {
                long userIdLong = Convert.ToInt32(userId);
                var isDiscount = _Entities.TbFeeDiscounts.Where(z => z.StudentId == userIdLong && z.FeeId == model.FeeId && z.IsActive).FirstOrDefault();
                if (isDiscount == null)
                {
                    discount.StudentId = userIdLong;
                    discount.FeeId = model.FeeId;
                    discount.DiscountAmount = model.Amount;
                    discount.TimeStamp = CurrentTime;
                    discount.IsActive = true;
                    _Entities.TbFeeDiscounts.Add(discount);
                    status = _Entities.SaveChanges() > 0;
                    message = status ? " Discount added" : "Failed to add discount";
                }

            }
            return Json(new { status = status, msg = message });
        }

        public PartialViewResult GetDiscountSyudentsByFeeId(string id)
        {
            //  string[] splitData = id.Split('~');

            FeeModel model = new FeeModel();
            model.SchoolId = _user.SchoolId;
            model.FeeId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_DiscountStudent_ByFee_Grid.cshtml", model);

        }

        public IActionResult TeacherList()
        {
            var model = new TeachervViewModel();
            model.SchoolId = _user.SchoolId;
            model.IsAdmin = true;

            model.Teachers = _Entities.TbTeachers
                .Where(x => x.SchoolId == _user.SchoolId && x.IsActive==true)
                .ToList()
                .Select(x => new Teacher(x))
                .ToList();

            return View(model);
        }


        //var model = new Satluj_Latest.Models.SchoolId();
        // model.schoolId = _user.SchoolId;
        // return PartialView("~/Views/School/_pv_teacher_list.cshtml", model);
        public PartialViewResult TeacherListPartial()
        {
            var model = new TeachervViewModel();
            model.SchoolId = _user.SchoolId;
            model.IsAdmin = true;
            return PartialView("~/Views/School/_pv_teacher_list.cshtml", model);
        }

        public PartialViewResult TeacherAddModel()
        {
            var model = new SchoolValue.TeacherAddModel();
            model.schoolId = _user.SchoolId;

            ViewBag.Designations = _dropdown.GetAllDesignation(_user.SchoolId);
            ViewBag.Departments = _dropdown.GetAllDepartment(_user.SchoolId);
            ViewBag.UserTypes = _dropdown.GetAllTeacherUserTypeDataAdd(_user.SchoolId);
            ViewBag.Classes = _dropdown.GetFreeClasses(_user.SchoolId);
            ViewBag.Roles = _dropdown.GetAllRoles(_user.SchoolId);
            return PartialView("~/Views/School/_pv_AddTeacher_Model.cshtml", model);
        }
        public JsonResult GetFreeDivision(long classId)
        {
            var divisions = _dropdown.GetFreeDivision(classId);
            return Json(divisions);
        }


        public object AddTeacher(SchoolValue.TeacherAddModel obj)
        {
            bool status = false;
            string message = "Failed";
            if (_Entities.TbTeachers.Any(x => x.ContactNumber.Trim() == obj.contactNumber.Trim() && x.IsActive && x.SchoolId==_user.SchoolId))
            {
                message = "Contact Number already in use";
            }
            else if (_Entities.TbLogins.Any(x => x.Username.ToLower() == obj.emailId.ToLower() && x.IsActive))
            {
                message = "Email already in use";
            }
            else
            {
                var model = new SchoolAddTeacherPostModel();
                model.classId = obj.classId;
                model.divisionId = obj.divisionId;
                model.emailId = obj.emailId;
                model.filePath = obj.filePath;
                model.image = obj.image;
                model.schoolId = _user.SchoolId.ToString();
                model.teacherName = obj.teacherName;
                model.contactNumber = obj.contactNumber;
                if (obj.DepartmentId != 0)
                    model.DepartmentId = obj.DepartmentId.ToString();
                if (obj.DesignationId != 0)
                    model.DesignationId = obj.DesignationId.ToString();
                model.RolesData = obj.RolesData;
                if (obj.UserTypeId != null)
                    model.UserTypeId = Convert.ToString(obj.UserTypeId);
                Tuple<bool, string, string> data = _schoolRepository.AddNewTeacher(model);
                status = data.Item1;
                if (status)
                    message = "Added Successfully";
            }
            return Json(new { status = status, msg = message });
        }

        public ActionResult TeacherDetails(string id)
        {
            var model = new Satluj_Latest.Models.TeacherId();
            model.teacherId = Convert.ToInt64(id);
            return View(model);
        }
        public object DeleteTeacher(string id, string schoolId)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { status = false, msg = "Invalid teacher id" });

            if (string.IsNullOrEmpty(schoolId))
                return Json(new { status = false, msg = "Invalid school id" });

            var model = new SchoolDeleteTeacherPostModel
            {
                teacherId = id,
                schoolId = schoolId
            };

            var result = _schoolRepository.DeleteTeacher(model);

            return Json(new { status = result.Item1, msg = result.Item2 });
        }

        public PartialViewResult TeacherEditPartial(long id)
        {
            var teacher = _Entities.TbTeachers
                .FirstOrDefault(t => t.TeacherId == id && t.IsActive);

            if (teacher == null)
                return PartialView("_Error", "Teacher not found");

            // LOAD USER ROLES
            var userRoles = _Entities.TbUserRoles
                .Where(r => r.UserId == teacher.UserId && r.IsActive)
                .Select(r => r.RoleId.ToString())
                .ToList();

            var model = new Satluj_Latest.Models.TeacherEditModel
            {
                teacherId = teacher.TeacherId,
                schoolId = teacher.SchoolId,
                teacherName = teacher.TeacherName,
                emailId = teacher.Email,
                contactNumber = teacher.ContactNumber,
                DepartmentId = teacher.DepartmentId ?? 0,
                DesignationId = teacher.DesignationId ?? 0,
                RolesData = string.Join("~", userRoles),

                // DROPDOWNS
                DesignationList = _Entities.TbDesignations
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.DesignationName })
                    .ToList(),

                DepartmentList = _Entities.TbDepartments
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.DepartmentName })
                    .ToList(),
                UserTypes = _Entities.TbUserModuleMains
                    .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.UserTypeName }),
                // ROLE LIST
                RolesList = _Entities.TbRoleDetails
                    .Select(r => new RoleVM { Id = r.Id, RoleName = r.RoleName })
                    .ToList()
            };

            return PartialView("~/Views/School/_pv_Teacher_EditModel.cshtml", model);
        }

        public object EditTeacher(SchoolValue.TeacherEditModel model)
        {
            bool status = false;
            string msg = "Failed";
            var teacher = _Entities.TbTeachers.FirstOrDefault(z => z.TeacherId == model.teacherId);
            if (teacher != null)
            {
                teacher.ContactNumber = model.contactNumber;
                teacher.TeacherName = model.teacherName;
                teacher.Email = model.emailId;
                teacher.DepartmentId = model.DepartmentId;
                teacher.DesignationId = model.DesignationId;
                if (model.UserTypeId != null && model.UserTypeId != 0)
                    teacher.UserType = model.UserTypeId;
                //----------------------------------
                var loginData = _Entities.TbLogins.Where(x => x.UserId == teacher.UserId && x.IsActive).FirstOrDefault();
                if (loginData.Username != model.emailId)
                {
                    loginData.Username = model.emailId;
                }
                
                //----------------------------------
            }

            foreach (var item in _Entities.TbTeacherClasses.Where(z => z.TeacherId == model.teacherId))
            {
                _Entities.TbTeacherClasses.Remove(item);
            }
            if (Convert.ToInt32(model.classId) != 0 && Convert.ToInt32(model.divisionId) != 0)
            {

                var addTeacherClass = new TbTeacherClass();
                addTeacherClass.TeacherId = teacher.TeacherId;
                addTeacherClass.ClassId = Convert.ToInt64(model.classId);
                addTeacherClass.DivisionId = Convert.ToInt64(model.divisionId); ;
                _Entities.TbTeacherClasses.Add(addTeacherClass);
            }
            status = _Entities.SaveChanges() > 0;
            msg = status ? "Updated Successfully" : "Failed";
            if (status)
            {
                var oldRole = _Entities.TbUserRoles.Where(x => x.UserId == teacher.UserId && x.IsActive).ToList();
                if (oldRole.Count > 0 && oldRole != null)
                {
                    foreach (var item in oldRole)
                    {
                        _Entities.TbUserRoles.Remove(item);
                    }
                }
                if (model.RolesData!=null && model.RolesData != "" && teacher != null)
                {
                    string[] splitData = model.RolesData.Split('~');
                    foreach (var item in splitData)
                    {
                        var role = new TbUserRole();
                        role.UserId = teacher.UserId;
                        role.RoleId = Convert.ToInt64(item);
                        role.IsActive = true;
                        role.TimeStamp = CurrentTime;
                        _Entities.TbUserRoles.Add(role);
                        _Entities.SaveChanges();
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public IActionResult StaffList()
        {
            var model = new StaffListViewModel();

            model.SchoolId = _user.SchoolId;

            var staffEntities = _Entities.TbStaffs
                                          .Include(s => s.User)
                                          .Include(s => s.Department)
                                          .Include(s => s.Designation)
                                          .Where(s => s.User.SchoolId == _user.SchoolId && s.IsActive==true)
                                          .ToList();

            model.Staffs = staffEntities
                                .Select(s => new Satluj_Latest.Data.Staff(s))
                                .ToList();

            model.IsAdmin = true;

            return View(model);
        }


        public PartialViewResult StaffAddModel()
        {
            StaffModels model = new StaffModels();
            Random rnd = new Random();

            model.Password = "SF" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            model.SchoolId = _user.SchoolId;
            model.Designations = _dropdown.GetAllDesignation(_user.SchoolId);
            model.Departments = _dropdown.GetAllDepartment(_user.SchoolId);
            model.UserTypes =  _dropdown.GetAllTeacherUserTypeDataAdd(_user.SchoolId);

            model.RolesList = _dropdown.GetAllRoles(_user.SchoolId);
            
            return PartialView("~/Views/School/_pv_AddStaff_Model.cshtml", model);
        }

        public object AddStaff(StaffModels model)
        {
            bool status = false;
            string msg = "Failed";
            if (_Entities.TbLogins.Any(x => x.Username.ToLower() == model.emailId.ToLower() && x.IsActive && x.SchoolId == _user.SchoolId))
            {
                msg = "Email Id already exist! ";
            }
            else
            {
                var login = new TbLogin();
                login.SchoolId = _user.SchoolId;
                login.RoleId = (int)UserRole.Staff;
                login.Name = model.Name;
                login.Username = model.emailId;
                login.Password = model.Password;
                login.IsActive = true;
                login.TimeStamp = CurrentTime;
                login.DisableStatus = false;
                login.LoginGuid = Guid.NewGuid();
                _Entities.TbLogins.Add(login);
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    var staff = new TbStaff();
                    staff.UserId = login.UserId;
                    staff.StaffName = model.Name;
                    staff.Contact = model.Contact;
                    staff.Address = model.Address;
                    staff.IsActive = true;
                    staff.TimeStamp = CurrentTime;
                    try
                    {
                        if (model.DOBstring != "")
                        {
                            string[] splitData = model.DOBstring.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var dob = mm + '-' + dd + '-' + yyyy;
                            staff.Dob = Convert.ToDateTime(dob);
                        }
                    }
                    catch
                    {

                    }
                    if (model.DepartmentId != 0)
                        staff.DepartmentId = model.DepartmentId;
                    if (model.DesignationId != 0)
                        staff.DesignationId = model.DesignationId;
                    if (model.UserTypeId != null)
                        staff.UserType = model.UserTypeId;

                    _Entities.TbStaffs.Add(staff);
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                    {
                        if (model.RoleData != "")
                        {
                            string[] splitData = model.RoleData.Split(',');
                            foreach (var item in splitData)
                            {
                                var role = new TbUserRole();
                                role.UserId = staff.UserId;
                                role.RoleId = Convert.ToInt64(item);
                                role.IsActive = true;
                                role.TimeStamp = CurrentTime;
                                _Entities.TbUserRoles.Add(role);
                                _Entities.SaveChanges();
                            }
                        }
                    }
                }
                msg = status ? " Staff added" : "Failed to add Staff";
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult StaffListPartial()
        {
            var model = new StaffListViewModel();
            model.SchoolId = _user.SchoolId;

            model.Staffs = _Entities.TbStaffs
                            .Where(s => s.User.SchoolId == _user.SchoolId && s.IsActive)
                            .Select(s => new Satluj_Latest.Data.Staff(s))
                            .ToList();

            model.IsAdmin = true;

            return PartialView("~/Views/School/_pv_Staff_list.cshtml", model);
        }
        public PartialViewResult StaffEditModel(string id)
        {
            long userId = Convert.ToInt64(id);

            var user = _Entities.TbLogins
                .FirstOrDefault(z => z.UserId == userId && z.IsActive);

            var staffEntity = _Entities.TbStaffs
                .FirstOrDefault(z => z.UserId == userId && z.IsActive);

            if (user == null || staffEntity == null)
                return PartialView("_Error", "Staff not found");

            var staff = new Satluj_Latest.Data.Staff(staffEntity.StaffId);

            // Create model
            StaffModels model = new StaffModels
            {
                UserId = user.UserId,
                Name = staff.StaffName,
                Email = user.Username,
                Password = user.Password,
                Address = staff.Address,
                Contact = staff.Contact,
                DepartmentId = staff.DepartmentId ?? 0,
                DesignationId = staff.DesignationId ?? 0,
                RoleData = staff.RoleIdString(),
                DOBstring = staff.DOB.ToString("dd-MM-yyyy"),
                SchoolId = _user.SchoolId,
                UserTypeId = staff.UserType ?? 0
            };

            
            model.Designations = _dropdown.GetAllDesignation(_user.SchoolId);
            model.Departments = _dropdown.GetAllDepartment(_user.SchoolId);
            model.UserTypes = _dropdown.GetAllTeacherUserTypeDataAdd(_user.SchoolId);
            model.RolesList = _dropdown.GetAllRoles(_user.SchoolId);

            return PartialView("~/Views/School/_pv_EditStaff_Model.cshtml", model);
        }
        public object EditStaff(StaffModels model)
        {
            bool status = false;
            string msg = "Failed";

            var login = _Entities.TbLogins
                .FirstOrDefault(z => z.UserId == model.UserId && z.IsActive);

            var staff = _Entities.TbStaffs
                .FirstOrDefault(z => z.UserId == model.UserId && z.IsActive);

            if (login == null || staff == null)
                return Json(new { status = false, msg = "Staff not found!" });

            // Update login table
            login.Name = model.Name;
            login.Username = model.Email;

            // Update main staff table
            staff.StaffName = model.Name;
            staff.Address = model.Address;
            staff.Contact = model.Contact;

            try
            {
                if (!string.IsNullOrEmpty(model.DOBstring))
                {
                    var parts = model.DOBstring.Split('-');
                    staff.Dob = new DateTime(
                        Convert.ToInt32(parts[2]),
                        Convert.ToInt32(parts[1]),
                        Convert.ToInt32(parts[0])
                    );
                }
            }
            catch { }

            staff.DepartmentId = model.DepartmentId;
            staff.DesignationId = model.DesignationId;
            staff.UserType = model.UserTypeId;

            // Remove old roles
            var oldRoles = _Entities.TbUserRoles
                            .Where(x => x.UserId == staff.UserId)
                            .ToList();

            _Entities.TbUserRoles.RemoveRange(oldRoles);

            // Add new roles
            if (!string.IsNullOrEmpty(model.RoleData))
            {
                var roles = model.RoleData.Split(',');

                foreach (var r in roles)
                {
                    if (string.IsNullOrWhiteSpace(r)) continue;

                    _Entities.TbUserRoles.Add(new TbUserRole
                    {
                        UserId = staff.UserId,
                        RoleId = Convert.ToInt64(r),
                        IsActive = true,
                        TimeStamp = CurrentTime
                    });
                }
            }

            
            status = _Entities.SaveChanges() > 0;

            msg = status ? "Staff updated successfully" : "No changes made";

            return Json(new { status, msg });
        }
        public object DeleteStaff(string id)
        {
            bool status = false;
            string msg = "Failed";

            if (string.IsNullOrEmpty(id))
                return Json(new { status = false, msg = "Invalid Staff Id" });

            long userId = Convert.ToInt64(id);

            // ---- 1) Get Login (Staff Login Entry) ----
            var login = _Entities.TbLogins
                                .FirstOrDefault(z => z.UserId == userId && z.IsActive);

            if (login == null)
                return Json(new { status = false, msg = "Staff not found" });

            // ---- 2) Get Staff table record ----
            var staff = _Entities.TbStaffs
                                 .FirstOrDefault(s => s.UserId == userId && s.IsActive);

            // ---- 3) Deactivate both ----
            login.IsActive = false;

            if (staff != null)
                staff.IsActive = false;

            status = _Entities.SaveChanges() > 0;
            msg = status ? "Staff Deleted" : "Failed to delete Staff";

            return Json(new { status = status, msg = msg });
        }
        public object GetFeeSuggestByText(string id)
        {
            string[] splitData = id.Split('~');
            bool status = false;
            string message = "Failed";
            string name = id;
            //var school = Entities.tb_School.Where(z => z.IsActive == true && z.ViewStatus == true && z.StateId == stateId && (z.School.StartsWith(name) || z.School.Contains(name))).Select(z => new
            var school = _Entities.TbFees.Where(z => z.IsActive && z.SchoolId == _user.SchoolId && (z.FeesName.StartsWith(name))).Select(z => new
            {
                feeId = z.FeeId,
                feeName = z.FeesName,
                schoolId = z.SchoolId,
            }).ToList();
            status = school.Count > 0;
            message = status ? "Success" : "Failed";
            return Json(new { status = status, message = message, list = school });
        }
        [HttpPost]
        public object AddAdditionalFeeStudent(FeeModel model)
        {
            bool status = false;
            string msg = "Failed";

            var PaidAmount = model.PaidAmount;
            var FeeName = model.FeeName;
            if (model.FeeId == 0)
            {
                if (PaidAmount > 0 && FeeName != null)
                {
                    var isExist1 = _Entities.TbFees.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && (z.FeesName.ToLower() == FeeName.ToLower())).ToList();
                    if (isExist1.Count > 0)
                    {
                        var feeStudent1 = new TbFeeStudent();
                        feeStudent1.Amount = PaidAmount;
                        feeStudent1.StudentId = model.StudentId;
                        feeStudent1.FeeId = isExist1.FirstOrDefault().FeeId;
                        feeStudent1.FeeStudentGuid = Guid.NewGuid();
                        feeStudent1.IsActive = true;
                        feeStudent1.TimeStamp = CurrentTime;
                        feeStudent1.DueDate = CurrentTime;
                        feeStudent1.Instalment = 1;
                        _Entities.TbFeeStudents.Add(feeStudent1);
                        status = _Entities.SaveChanges() > 0;
                        msg = status ? " Fee added" : "Failed to add Fee";
                    }
                    else
                    {
                        var fee = new TbFee();
                        var feeStudent = new TbFeeStudent();
                        fee.FeesName = FeeName;
                        fee.SchoolId = _user.SchoolId;
                        fee.TimeStamp = CurrentTime;
                        fee.IsActive = true;
                        fee.FeeType = 2;
                        fee.Interval = 1;
                        fee.FeeStartDate = CurrentTime;
                        _Entities.TbFees.Add(fee);
                        status = _Entities.SaveChanges() > 0;
                        var feeId = fee.FeeId;

                        feeStudent.Amount = PaidAmount;
                        feeStudent.StudentId = model.StudentId;
                        feeStudent.FeeId = feeId;
                        feeStudent.FeeStudentGuid = Guid.NewGuid();
                        feeStudent.IsActive = true;
                        feeStudent.TimeStamp = CurrentTime;
                        feeStudent.DueDate = CurrentTime;
                        feeStudent.Instalment = 1;
                        _Entities.TbFeeStudents.Add(feeStudent);
                        status = _Entities.SaveChanges() > 0;
                        msg = status ? " Fee added" : "Failed to add fee";
                    }
                }
                if (model.FeeDetails != null)
                {
                    List<string> feeDetails = model.FeeDetails.Split(',').ToList();

                    foreach (var data in feeDetails)
                    {
                        string[] splitDataAdd = data.Split('~');
                        long addFeeid = Convert.ToInt64(splitDataAdd[0]);
                        decimal addAmount = Convert.ToDecimal(splitDataAdd[1]);

                        var isExist = _Entities.TbFees.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.FeeId == addFeeid).ToList();
                        if (isExist.Count > 0)
                        {
                            var feeStudent1 = new TbFeeStudent();
                            feeStudent1.Amount = addAmount;
                            feeStudent1.StudentId = model.StudentId;
                            feeStudent1.FeeId = isExist.FirstOrDefault().FeeId;
                            feeStudent1.FeeStudentGuid = Guid.NewGuid();
                            feeStudent1.IsActive = true;
                            feeStudent1.TimeStamp = CurrentTime;
                            feeStudent1.DueDate = CurrentTime;
                            feeStudent1.Instalment = 1;
                            _Entities.TbFeeStudents.Add(feeStudent1);
                            status = _Entities.SaveChanges() > 0;
                            msg = status ? " Fee added" : "Failed to add Fee";
                        }
                    }
                }

            }

            else
            {
                var feeStudent = new TbFeeStudent();
                feeStudent.Amount = model.Amount;
                feeStudent.StudentId = model.StudentId;
                feeStudent.FeeId = model.FeeId;
                feeStudent.FeeStudentGuid = Guid.NewGuid();
                feeStudent.IsActive = true;
                feeStudent.TimeStamp = CurrentTime;
                feeStudent.DueDate = CurrentTime;
                feeStudent.Instalment = 1;
                _Entities.TbFeeStudents.Add(feeStudent);
                status = _Entities.SaveChanges() > 0;
                msg = status ? " Fee added" : "Failed to add Fee";
            }
            var nowDate = String.Format("{0:y}", CurrentTime);
            return Json(new { status = status, msg = msg, feeName = model.FeeName, date = nowDate, amount = model.Amount });
        }

        public PartialViewResult AddPaymentPartialView(string id)
        {
            var model = new FeeModel();
            model.StudentId = Convert.ToInt64(id);
            model.SchoolId = _user.SchoolId;

            return PartialView("~/Views/School/_pv_AdditionalBilling.cshtml", model);
        }
        public PartialViewResult AddAdvancePaymentPartialView(string id)
        {
            var model = new FeeModel();
            model.StudentId = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_AdvancePayment_Add.cshtml", model);
        }
        public PartialViewResult BillingPaymentDeteils(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentId = Convert.ToInt64(id); ;
            return PartialView("~/Views/School/_pv_Billing_PaymentDeteils_Grid.cshtml", model);
        }
        [HttpPost]
        public object EditStudentFee(FeeModel model)
        {
            bool status = true;
            string message = "Failed";

            //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            //List<Datalist> routes_list =
            //         (List<Datalist>)json_serializer.DeserializeObject(model.DataList);

            //List<Datalist> routes_list = new JavaScriptSerializer().Deserialize<List<Datalist>>(model.FeeDetails);
            //long FeeId = Convert.ToInt64(model.FeeId);
            List<Datalist> routes_list = JsonConvert.DeserializeObject<List<Datalist>>(model.FeeDetails);
            foreach (var value in routes_list)
            {
                long feeStudentId = Convert.ToInt32(value.feeStudentId);
                var FeeDetail = _Entities.TbFeeStudents.Where(z => z.FeeStudentId == feeStudentId && z.IsActive).FirstOrDefault();
                if (FeeDetail != null)
                {
                    FeeDetail.Amount = Convert.ToDecimal(value.amount);
                    status = _Entities.SaveChanges() > 0;
                }

            }

            message = status ? " Fee Added" : "Fee Added";
            return Json(new { status = true, msg = message });
        }
        public PartialViewResult EditBillingPayment(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentId = Convert.ToInt64(id); ;
            return PartialView("~/Views/School/_pv_Edit_BillingDetails.cshtml", model);
        }

        public object DeleteStudentEditableFee(string id)
        {
            bool status = false;
            string message = "Failed";
            string[] splitData = id.Split('~');
            long studentId = Convert.ToInt32(splitData[0]);
            Guid feeGuid = new Guid(splitData[1]);
            var fee = _Entities.TbFeeStudents.Where(z => z.FeeStudentGuid == feeGuid && z.StudentId == studentId).FirstOrDefault();
            if (fee != null)
            {
                _Entities.TbFeeStudents.Remove(fee);
                status = _Entities.SaveChanges() > 0;
            }
            //fee.IsActive = false;
            message = status ? "Fee Deleted" : "failed";
            return Json(new { status = status, msg = message });
        }
        public object DeleteStudentCommonFee(string id)
        {
            bool status = false;
            string message = "Failed";
            string[] splitData = id.Split('~');
            long studentId = Convert.ToInt32(splitData[0]);
            Guid feeGuid = new Guid(splitData[1]);
            long classId = Convert.ToInt64(splitData[2]);
            var fee = _Entities.TbFeeClasses.Where(z => z.FeeClassGuid == feeGuid && z.ClassId == classId).FirstOrDefault();
            if (fee != null)
            {
                var delStudentFee = _Entities.TbDeletedFeeStudents.Where(z => z.StudentId == studentId && z.FeeClassId == fee.FeeClassId).FirstOrDefault();
                if (delStudentFee == null)
                {
                    var DeleteFeeStudent = new TbDeletedFeeStudent();
                    DeleteFeeStudent.StudentId = studentId;
                    DeleteFeeStudent.FeeClassId = fee.FeeClassId;
                    DeleteFeeStudent.ParentGuid = feeGuid;
                    DeleteFeeStudent.IsActive = true;
                    DeleteFeeStudent.TimeStamp = CurrentTime;
                    _Entities.TbDeletedFeeStudents.Add(DeleteFeeStudent);
                    status = _Entities.SaveChanges() > 0;
                }
            }
            //fee.IsActive = false;
            message = status ? "Fee Deleted" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public ActionResult SpecialFeeClassList(string id)
        {
            long feeId = Convert.ToInt32(id);
            var model = new Satluj_Latest.Models.FilterModel();
            model.schoolId = _user.SchoolId;
            model.FeeId = feeId;
            return View(model);
        }
        public ActionResult AssignSpecialFee(string id)
        {
            string[] splitData = id.Split('~');
            long divisionId = Convert.ToInt32(splitData[0]);
            long feeId = Convert.ToInt32(splitData[1]);
            var model = new FeeModel();
            model.DivisionId = divisionId;
            model.SchoolId = _user.SchoolId;
            model.FeeId = feeId;
            return View(model);
        }
        [HttpPost]
        public object AssignSpecialFee(FeeModel model)
        {
            bool status = false;
            string msg = "Failed";
            List<string> studentsUserId = model.FeeStudentId.Split(',').ToList();

            var FeeDetail = _Entities.TbFees.Where(z => z.FeeId == model.FeeId && z.IsActive).FirstOrDefault();
            var discount = new TbFeeDiscount();
            foreach (var userId in studentsUserId)
            {


                if (FeeDetail.Interval > 1)
                {
                    DateTime dueDt = FeeDetail.FeeStartDate;
                    var addMonth = 12 / FeeDetail.Interval;
                    for (int i = 1; i <= FeeDetail.Interval; i++)
                    {
                        var feeStudent = new TbFeeStudent();
                        feeStudent.Amount = model.Amount;
                        feeStudent.StudentId = Convert.ToInt32(userId);
                        feeStudent.FeeId = model.FeeId;
                        feeStudent.FeeStudentGuid = Guid.NewGuid();
                        feeStudent.IsActive = true;
                        feeStudent.TimeStamp = CurrentTime;
                        feeStudent.DueDate = dueDt;
                        dueDt = dueDt.AddMonths(addMonth);
                        feeStudent.Instalment = i;
                        _Entities.TbFeeStudents.Add(feeStudent);
                        status = _Entities.SaveChanges() > 0;
                        msg = status ? " Fee added" : "Failed to add Fee";

                    }
                }
                else
                {
                    var feeStudent = new TbFeeStudent();
                    feeStudent.Amount = model.Amount;
                    feeStudent.StudentId = Convert.ToInt32(userId);
                    feeStudent.FeeId = model.FeeId;
                    feeStudent.FeeStudentGuid = Guid.NewGuid();
                    feeStudent.IsActive = true;
                    feeStudent.TimeStamp = CurrentTime;
                    feeStudent.DueDate = CurrentTime;
                    feeStudent.Instalment = 1;
                    _Entities.TbFeeStudents.Add(feeStudent);
                    status = _Entities.SaveChanges() > 0;
                    msg = status ? " Fee added" : "Failed to add Fee";
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult GetStudentFeeByDivGrid(string id)
        {
            string[] splitData = id.Split('~');
            FeeModel model = new FeeModel();
            long divisionId = Convert.ToInt32(splitData[0]);
            long feeId = Convert.ToInt32(splitData[1]);

            model.DivisionId = divisionId;
            model.FeeId = feeId;
            return PartialView("~/Views/School/_pv_StudentFee_ByDivision_Grid.cshtml", model);

        }
        public ActionResult SpecialFeeStudent(string id)
        {
            var model = new FeeModel();
            long feeId = Convert.ToInt32(id);
            model.FeeId = feeId;
            model.SchoolId = _user.SchoolId;
            ViewBag.Classes = _dropdown.GetClasses(model.SchoolId);

            return View(model);
        }
        public object DeleteStudentFee(string id)
        {
            bool status = false;
            string message = "Failed";
            string[] splitData = id.Split('~');
            long studentId = Convert.ToInt32(splitData[0]);
            long studentFeeId = Convert.ToInt32(splitData[1]);


            var feeStudent = _Entities.TbFeeStudents.Where(z => z.StudentId == studentId && z.IsActive && z.FeeStudentId == studentFeeId).FirstOrDefault();
            if (feeStudent != null)
            {
                feeStudent.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? " Fee deleted" : "Failed to delete fee";

            return Json(new { status = status, msg = message });
        }
        public PartialViewResult RefreshFeeStudentGrid(string id)
        {
            var model = new FeeModel();
            model.SchoolId = _user.SchoolId;
            model.FeeId = Convert.ToInt32(id);
            return PartialView("~/Views/School/_pv_StudentFee_Student_Grid.cshtml", model);
        }
        public PartialViewResult EditFeeStudAmountView(string id)
        {
            var model = new FeeModel();
            string[] splitData = id.Split('~');
            model.StudentId = Convert.ToInt32(splitData[0]);
            model.SpecialFeeId = Convert.ToInt32(splitData[1]);
            var feeDetail = _Entities.TbFeeStudents.Where(z => z.StudentId == model.StudentId && z.IsActive && z.FeeStudentId == model.SpecialFeeId).FirstOrDefault();
            model.FeeName = feeDetail.Fee.FeesName;
            model.StudentName = feeDetail.Student.StundentName;
            model.Amount = Convert.ToDecimal(String.Format("{0:0.00}", feeDetail.Amount));
            return PartialView("~/Views/School/_pv_Edit_SpecialFee_StudentAmount_Model.cshtml", model);

        }
        [HttpPost]
        public object EditFeeStudentAmount(FeeModel model)
        {
            bool status = false;
            string message = "Failed";
            long studentId = model.StudentId;
            long specialFeeId = model.SpecialFeeId;
            var feeDetails = _Entities.TbFeeStudents.Where(z => z.StudentId == studentId && z.IsActive && z.FeeStudentId == specialFeeId).FirstOrDefault();
            if (feeDetails != null)
            {
                feeDetails.Amount = model.Amount;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? " Fee edited" : "Failed to edit fee";
            return Json(new { status = status, msg = message });
        }
        public PartialViewResult PrintAccountBillData(string id)
        {
            string[] splitData = id.Split('~');
            var model = new Satluj_Latest.Models.PrintBill();
            model.StudentId = Convert.ToInt64(splitData[0]);
            model.BillNumber = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/School/_pv_PrintAccountBillData.cshtml", model);
        }
        public object CheckAdmissionNo(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbStudents.Any(x => x.StudentSpecialId.ToLower() == text.ToLower() && x.IsActive))
            {
                Status = true;
                Message = "Admission Number already in use";
            }
            return Json(new { Status = Status, Message = Message });
        }
        public object CheckTeacherContactNumberNo(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbTeachers.Any(x => x.ContactNumber.Trim() == text.Trim() && x.IsActive))
            {
                Status = true;
                Message = "Contact Number already in use";
            }
            return Json(new { Status = Status, Message = Message });
        }

        #region SMTP
        public ActionResult SMTPDetails()
        {
            SMTPDetailsModel model = new SMTPDetailsModel();
            long schoolId = _user.SchoolId;
            var smtpDetail = _Entities.TbSmtpdetails.Where(z => z.SchoolId == schoolId).FirstOrDefault();
            if (smtpDetail != null)
            {
                model.email = smtpDetail.EmailId;
                model.password = smtpDetail.Password;
            }
            return View(model);
        }

        [HttpPost]
        public object AddEmailerSetup(SMTPDetailsModel model)
        {
            bool status = false;
            string message = "Failed";
            long schoolId = _user.SchoolId;
            var smtp = new TbSmtpdetail();
            var smtpDetail = _Entities.TbSmtpdetails.Where(z => z.SchoolId == schoolId).FirstOrDefault();
            if (smtpDetail == null)
            {
                try
                {
                    smtp.EmailId = model.email;
                    smtp.Password = model.password;
                    smtp.SchoolId = schoolId;
                    _Entities.TbSmtpdetails.Add(smtp);
                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
                catch (Exception ex)
                {

                }
            }
            message = status ? " SMTP Added" : "Failed to add SMTP";
            return Json(new { status = status, msg = message });
        }


        [HttpPost]
        public object EditEmailerSetup(SMTPDetailsModel model)
        {
            bool status = false;
            string message = "Failed";
            long schoolId = _user.SchoolId;
            var smtpDetail = _Entities.TbSmtpdetails.Where(z => z.SchoolId == schoolId).FirstOrDefault();
            if (smtpDetail != null)
            {
                smtpDetail.EmailId = model.email;
                smtpDetail.Password = model.password;
                status = _Entities.SaveChanges() > 0;
            }
            message = status ? " SMTP Edit Successfully" : "Failed to edit SMTP";
            return Json(new { status = status, msg = message });
        }

        #endregion

        public PartialViewResult AddStudentBillPartialView(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentId = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_History_Billing_StudentFee_Model.cshtml", model);
        }

        public PartialViewResult LoadTableForBilling(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            string[] splitData = id.Split('~');
            model.SchoolModel.studentId = Convert.ToInt64(splitData[1]);
            model.BillNumber = Convert.ToInt64(splitData[0]);
            return PartialView("~/Views/School/_pv_History_PopupGrid.cshtml", model);
        }

        public ActionResult CollectionReport()
        {
            FeeModel model = new FeeModel();
            model.StartDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult CollectionReportByDate(string id)
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
            return PartialView("~/Views/School/_pv_CollectionReport_Grid.cshtml", model);
        }

        public ActionResult CollectionReportDetail(string id)
        {
            string[] split = id.Split('~');

            long billno = Convert.ToInt64(split[0]);
            long studentId = Convert.ToInt64(split[1]);

            FeeModel model = new FeeModel();
            model.BillNumber = billno;
            model.StudentId = studentId;
            return View(model);
        }
        public object CancelBill(string id)
        {
            bool status = false;
            string msg = string.Empty;
            string[] splitData = id.Split('~');

            long billNo = Convert.ToInt64(splitData[0]);
            long studentId = Convert.ToInt64(splitData[1]);
            decimal preiousCr = 0;
            decimal currentCr = 0;
            var Bills_Which_paid_Balance_Of_This_Bill = _Entities.TbPayments.Where(x => x.IsActive == true && x.IsPaid == true && x.PartialPaidParentBillNo == billNo).ToList();
            var studentPaidAmt = _Entities.TbStudentPaidAmounts.Where(z => z.StudentId == studentId && z.BillNo == billNo && z.StudentId == studentId && z.IsActive).FirstOrDefault();
            if (studentPaidAmt != null)
            {
                preiousCr = studentPaidAmt.PreviousBalance ?? 0;
                currentCr = studentPaidAmt.BalanceAmount;
                var studentBalance = _Entities.TbStudentBalances.Where(z => z.StudentId == studentId).FirstOrDefault();
                if (studentPaidAmt.PaidAmount == 0)
                {
                    var paymentBill = _Entities.TbPayments.Where(z => z.SchoolId == _user.SchoolId && z.BillNo == billNo && z.StudentId == studentId).ToList();
                    foreach (var payment in paymentBill)
                    {
                        var billDue = _Entities.TbFeeDues.Where(z => z.StudentId == payment.StudentId && z.IsActive && z.BillNo == payment.BillNo).FirstOrDefault();
                        if (billDue != null)
                        {
                            #region Partial Paid after this bill calculation 
                            // Here we checking that the cancelling bill have a partial payment . If yes, then checking that the balance payment payed in the other bill.
                            //if yes, then we wants to regenreate the feedues of the cancelling bill which is minusing from the payed bill amount .
                            if (Bills_Which_paid_Balance_Of_This_Bill.Count > 0 && Bills_Which_paid_Balance_Of_This_Bill != null)
                            {
                                var afterPaidPartial = Bills_Which_paid_Balance_Of_This_Bill.Where(x => x.FeeGuid == billDue.FeeDuesGuid).FirstOrDefault();
                                if (afterPaidPartial != null)
                                {
                                    var thisBillPartial = _Entities.TbFeeDues.Where(x => x.StudentId == studentId && x.BillNo == afterPaidPartial.BillNo && x.ParentGuid == afterPaidPartial.FeeGuid).FirstOrDefault();
                                    if (thisBillPartial != null)
                                    {
                                        thisBillPartial.ParentGuid = payment.FeeGuid;
                                    }
                                    afterPaidPartial.FeeGuid = payment.FeeGuid;
                                    afterPaidPartial.MaxAmount = afterPaidPartial.MaxAmount + payment.Amount;
                                    var feeDueNow = new TbFeeDue();
                                    feeDueNow.Amount = payment.Amount;
                                    feeDueNow.FeeId = payment.FeeId;
                                    feeDueNow.StudentId = payment.StudentId;
                                    feeDueNow.FeeDuesGuid = Guid.NewGuid();
                                    feeDueNow.IsActive = true;
                                    feeDueNow.DueDate = billDue.DueDate;
                                    feeDueNow.TimeStamp = CurrentTime;
                                    feeDueNow.ParentGuid = payment.FeeGuid;
                                    feeDueNow.BillNo = afterPaidPartial.BillNo;
                                    _Entities.TbFeeDues.Add(feeDueNow);
                                    _Entities.SaveChanges();
                                }
                            }
                            #endregion Partial Paid after this bill calculation 

                            _Entities.TbFeeDues.Remove(billDue);
                        }
                        var current = _Entities.TbPayments.Where(x => x.PaymentId == payment.PaymentId).FirstOrDefault();
                        current.IsActive = false;
                        status = _Entities.SaveChanges() > 0 ? true : false;

                    }
                    if (status)
                    {
                        studentPaidAmt.AddAccountStatus = false;
                        studentPaidAmt.IsActive = false;
                        status = _Entities.SaveChanges() > 0 ? true : false;
                    }
                    if (status)
                    {
                        decimal diffBal = preiousCr - currentCr;
                        decimal bal = studentBalance.Amount;
                        if (studentBalance.Amount != (bal - diffBal))
                        {
                            studentBalance.Amount = bal - diffBal;
                            status = _Entities.SaveChanges() > 0 ? true : false;
                        }
                    }
                    msg = status ? "Bill cancelled!" : "Failed to cancel bill!";
                }
                else
                {
                    var listtodelete = _Entities.TbStudentPaidAmounts.Where(z => z.PaidId >= studentPaidAmt.PaidId && z.StudentId == studentId && z.IsActive).ToList();
                    foreach (var data in listtodelete)
                    {
                        data.IsActive = false;
                        data.AddAccountStatus = false;
                        var paymentBill = _Entities.TbPayments.Where(z => z.SchoolId == _user.SchoolId && z.BillNo == data.BillNo && z.StudentId == studentId).ToList();
                        foreach (var payment in paymentBill)
                        {
                            var pay = _Entities.TbPayments.Where(x => x.PaymentId == payment.PaymentId).FirstOrDefault();
                            pay.IsActive = false;
                            status = _Entities.SaveChanges() > 0 ? true : false;
                        }
                    }
                    if (status)
                    {
                        if (studentBalance.Amount != preiousCr)
                        {
                            studentBalance.Amount = preiousCr;
                            status = _Entities.SaveChanges() > 0 ? true : false;
                        }
                    }

                    msg = status ? "Bill cancelled!" : "Failed to cancel bill!";
                }
            }
            else
            {
                var paymentBill = _Entities.TbPayments.Where(z => z.SchoolId == _user.SchoolId && z.BillNo == billNo && z.StudentId == studentId).ToList();
                foreach (var payment in paymentBill)
                {
                    var billDue = _Entities.TbFeeDues.Where(z => z.StudentId == payment.StudentId && z.IsActive && z.BillNo == payment.BillNo).FirstOrDefault();
                    if (billDue != null)
                    {
                        #region Partial Paid after this bill calculation 
                        // Here we checking that the cancelling bill have a partial payment . If yes, then checking that the balance payment payed in the other bill.
                        //if yes, then we wants to regenreate the feedues of the cancelling bill which is minusing from the payed bill amount .
                        if (Bills_Which_paid_Balance_Of_This_Bill.Count > 0 && Bills_Which_paid_Balance_Of_This_Bill != null)
                        {
                            var afterPaidPartial = Bills_Which_paid_Balance_Of_This_Bill.Where(x => x.FeeGuid == billDue.FeeDuesGuid).FirstOrDefault();
                            if (afterPaidPartial != null)
                            {
                                var thisBillPartial = _Entities.TbFeeDues.Where(x => x.StudentId == studentId && x.BillNo == afterPaidPartial.BillNo && x.ParentGuid == afterPaidPartial.FeeGuid).FirstOrDefault();
                                if (thisBillPartial != null)
                                {
                                    thisBillPartial.ParentGuid = payment.FeeGuid;
                                }
                                afterPaidPartial.FeeGuid = payment.FeeGuid;
                                afterPaidPartial.MaxAmount = afterPaidPartial.MaxAmount + payment.Amount;
                                var feeDueNow = new TbFeeDue();
                                feeDueNow.Amount = payment.Amount;
                                feeDueNow.FeeId = payment.FeeId;
                                feeDueNow.StudentId = payment.StudentId;
                                feeDueNow.FeeDuesGuid = Guid.NewGuid();
                                feeDueNow.IsActive = true;
                                feeDueNow.DueDate = billDue.DueDate;
                                feeDueNow.TimeStamp = CurrentTime;
                                feeDueNow.ParentGuid = payment.FeeGuid;
                                feeDueNow.BillNo = afterPaidPartial.BillNo;
                                _Entities.TbFeeDues.Add(feeDueNow);
                                _Entities.SaveChanges();
                            }
                        }
                        #endregion Partial Paid after this bill calculation 
                        _Entities.TbFeeDues.Remove(billDue);
                    }
                    var currentPaymnet = _Entities.TbPayments.Where(x => x.PaymentId == payment.PaymentId).FirstOrDefault();
                    currentPaymnet.IsActive = false;
                    status = _Entities.SaveChanges() > 0 ? true : false;
                }
                msg = status ? "Bill cancelled!" : "Failed to cancel bill!";
            }

            #region Account from Cancel
            try// Archana 29-11-2018 New bill cancel section for Account section
            {
                var student = new Satluj_Latest.Data.Student(studentId);
                var studentPaidFee = student.StudentPaidAmountByBillNo(billNo);
                decimal refundAmount = 0;
                if (studentPaidFee == null)
                {
                    refundAmount = _Entities.TbPayments.Where(x => x.StudentId == studentId && x.SchoolId == _user.SchoolId && x.BillNo == billNo && x.IsActive == false).ToList().Sum(x => x.Amount);
                }
                else
                {
                    refundAmount = studentPaidFee.PaidAmount;
                }
                if (status == true && refundAmount != 0)
                {
                    int mode = Convert.ToInt32(splitData[2]);
                    long headId = 0;
                    long subId = 0;
                    var vouchr = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
                    if (vouchr == null)
                    {
                        var vou = new TbVoucherNumber();
                        vou.IsActive = true;
                        vou.PaymentVoucher = 1;
                        vou.ReceiptVoucher = 1;
                        vou.SchoolId = _user.SchoolId;
                        vou.ContraVoucher = 1;
                        _Entities.TbVoucherNumbers.Add(vou);
                        _Entities.SaveChanges();
                        vouchr = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
                    }
                    var head = _Entities.TbAccountHeads.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ForBill == true).FirstOrDefault();
                    if (head != null)
                    {
                        headId = head.AccountId;
                        var sub = _Entities.TbSubLedgerData.Where(x => x.AccHeadId == head.AccountId && x.IsActive).ToList();
                        if (sub.Count > 0 && sub != null)
                        {
                            if (sub.Count == 1)
                            {
                                var subAdd = new TbSubLedgerDatum();
                                subAdd.SubLedgerName = "Bill Cancel";
                                subAdd.AccHeadId = head.AccountId;
                                subAdd.IsActive = true;
                                subAdd.TimeStamp = CurrentTime;
                                _Entities.TbSubLedgerData.Add(subAdd);
                                _Entities.SaveChanges();
                                subId = subAdd.LedgerId;
                            }
                            else
                            {
                                subId = sub[1].LedgerId;
                            }
                        }
                        else
                        {
                            var subAdd = new TbSubLedgerDatum();
                            subAdd.SubLedgerName = "Advance Amount";
                            subAdd.AccHeadId = head.AccountId;
                            subAdd.IsActive = true;
                            subAdd.TimeStamp = CurrentTime;
                            _Entities.TbSubLedgerData.Add(subAdd);
                            _Entities.SaveChanges();

                            var subAdd2 = new TbSubLedgerDatum();
                            subAdd2.SubLedgerName = "Bill Cancel";
                            subAdd2.AccHeadId = head.AccountId;
                            subAdd2.IsActive = true;
                            subAdd2.TimeStamp = CurrentTime;
                            _Entities.TbSubLedgerData.Add(subAdd2);
                            _Entities.SaveChanges();
                            subId = subAdd2.LedgerId;
                        }
                    }
                    else
                    {
                        var AccountHead = new TbAccountHead();
                        AccountHead.AccHeadName = "Fee Income";
                        AccountHead.ForBill = true;
                        AccountHead.SchoolId = _user.SchoolId;
                        AccountHead.IsActive = true;
                        AccountHead.TimeStamp = CurrentTime;
                        _Entities.TbAccountHeads.Add(AccountHead);
                        _Entities.SaveChanges();
                        headId = AccountHead.AccountId;

                        var subAdd = new TbSubLedgerDatum();
                        subAdd.SubLedgerName = "Advance Amount";
                        subAdd.AccHeadId = AccountHead.AccountId;
                        subAdd.IsActive = true;
                        subAdd.TimeStamp = CurrentTime;
                        _Entities.TbSubLedgerData.Add(subAdd);
                        _Entities.SaveChanges();

                        var subAdd2 = new TbSubLedgerDatum();
                        subAdd2.SubLedgerName = "Bill Cancel";
                        subAdd2.AccHeadId = AccountHead.AccountId;
                        subAdd2.IsActive = true;
                        subAdd2.TimeStamp = CurrentTime;
                        _Entities.TbSubLedgerData.Add(subAdd2);
                        _Entities.SaveChanges();
                        subId = subAdd2.LedgerId;
                    }
                    if (mode == 1)//Cash
                    {
                        var cash = new TbCashEntry();
                        cash.AdvanceStatus = false;
                        cash.Amount = refundAmount;
                        cash.VoucherNumber = Convert.ToString(vouchr.PaymentVoucher);
                        cash.BillNo = "";
                        cash.CancelStatus = false;
                        cash.DataFromStatus = false;
                        cash.EditStatus = "P";
                        cash.EnterDate = CurrentTime;
                        cash.HeadId = headId;
                        cash.IsActive = true;
                        cash.Migration = false;
                        cash.Narration = "Bill Cancel " + billNo;
                        cash.ReverseStatus = false;
                        cash.SchoolId = _user.SchoolId;
                        cash.SubId = subId;
                        cash.TimeStamp = CurrentTime;
                        cash.TransactionType = "P";
                        cash.UserId = _user.UserId;
                        cash.VoucherNumber = Convert.ToString(vouchr.PaymentVoucher);
                        cash.VoucherType = "PV";
                        _Entities.TbCashEntries.Add(cash);
                        _Entities.SaveChanges();

                        vouchr.PaymentVoucher = vouchr.PaymentVoucher + 1;
                        _Entities.SaveChanges();

                        var cancelledBills = _Entities.TbPayments.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == false && x.BillNo == billNo && x.IsPaid == true).ToList();
                        if (cancelledBills.Count > 0 && cancelledBills != null)
                        {
                            foreach (var item in cancelledBills)
                            {
                                var cancelAccount = new TbBillCancelAccount();
                                cancelAccount.SchoolId = _user.SchoolId;
                                cancelAccount.CashBankType = false;//Cash
                                cancelAccount.CashBankId = cash.Id;
                                cancelAccount.ItemId = item.FeeId;
                                cancelAccount.Amount = item.Amount;
                                cancelAccount.CancelDate = CurrentTime;
                                cancelAccount.IsActive = true;
                                _Entities.TbBillCancelAccounts.Add(cancelAccount);
                                _Entities.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        var bank = new TbBankEntry();
                        bank.Amount = refundAmount;
                        bank.BankId = Convert.ToInt64(splitData[3]);
                        bank.BillNo = "";
                        bank.CancelStatus = false;
                        if (mode == 2)//Cheque
                        {
                            bank.ChequeDate = Convert.ToDateTime(splitData[5]);
                            bank.ChequeNumber = Convert.ToString(splitData[4]);
                        }
                        bank.DataFromStatus = false;
                        bank.EditStatus = "P";
                        bank.EnterDate = CurrentTime;
                        bank.HeadId = headId;
                        bank.SubId = subId;
                        bank.IsActive = true;
                        bank.Migration = false;
                        bank.ModeType = mode;
                        bank.Narration = "Bill Cancel " + billNo;
                        bank.SchoolId = _user.SchoolId;
                        bank.TimeStamp = CurrentTime;
                        bank.TransactionType = "P";
                        bank.UserId = _user.UserId;
                        bank.VoucherNumber = Convert.ToString(vouchr.PaymentVoucher);
                        bank.VoucherType = "PV";
                        _Entities.TbBankEntries.Add(bank);
                        _Entities.SaveChanges();

                        vouchr.PaymentVoucher = vouchr.PaymentVoucher + 1;
                        _Entities.SaveChanges();

                        var cancelledBills = _Entities.TbPayments.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == false && x.BillNo == billNo && x.IsPaid == true).ToList();
                        if (cancelledBills.Count > 0 && cancelledBills != null)
                        {
                            foreach (var item in cancelledBills)
                            {
                                var cancelAccount = new TbBillCancelAccount();
                                cancelAccount.SchoolId = _user.SchoolId;
                                cancelAccount.CashBankType = true;//Bank
                                cancelAccount.CashBankId = bank.Id;
                                cancelAccount.ItemId = item.FeeId;
                                cancelAccount.Amount = item.Amount;
                                cancelAccount.CancelDate = CurrentTime;
                                cancelAccount.IsActive = true;
                                _Entities.TbBillCancelAccounts.Add(cancelAccount);
                                _Entities.SaveChanges();
                            }
                        }
                    }

                    #region Data added to Balance table for Account
                    int sourceId = 0;
                    if (mode == 1)//cash
                        sourceId = Convert.ToInt32(DataFromStatus.Cash);
                    else
                        sourceId = Convert.ToInt32(DataFromStatus.Bank);
                    long bankId = Convert.ToInt64(splitData[3]);
                    var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == CurrentTime.Date && x.SourceId == sourceId && x.BankId == bankId).FirstOrDefault();
                    if (balance != null)
                    {
                        balance.Closing = balance.Closing - refundAmount;
                        balance.TimeStamp = CurrentTime;
                        _Entities.SaveChanges();
                    }
                    else
                    {
                        try
                        {
                            var balanceEntry = new TbBalance();
                            balanceEntry.SchoolId = _user.SchoolId;
                            balanceEntry.CurrentDate = CurrentTime;
                            balanceEntry.SourceId = sourceId;
                            DateTime yesterday = _Entities.TbBalances.Where(x =>x.CurrentDate.Date < CurrentTime.Date && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == bankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true &&x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == bankId).ToList().Sum(x => x.Closing);
                            else
                                balanceEntry.Opening = 0;
                            balanceEntry.Closing = balanceEntry.Opening - refundAmount;
                            balanceEntry.IsActive = true;
                            balanceEntry.BankId = bankId;
                            balanceEntry.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(balanceEntry);
                            _Entities.SaveChanges();

                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion Data added to Balance table for Account
                }
            }
            catch (Exception ex)
            {

            }
            #endregion Account from Cancel
            return Json(new { status = status, msg = msg });
        }

        #region SMS
        public ActionResult Messages()
        {
            var model = new SchoolModelForId();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }

        public object SMSOLD(SchoolModelForId model)
        {
            HttpClient client = new HttpClient();
            string message = "Failed";
            var status = false;
            // List<string> numbers = model.Numbers.Split(',').ToList();
            //  List<string> studentsUserId = model.StudentId.Split(',').ToList();
            //  var phone = model.Numbers;
            var phone = "9961797049";
            var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + model.Description + "&priority=ndnd&stype=normal";
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest request = this.GetRequest(url);
            WebResponse webResponse = request.GetResponse();
            var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
            List<string> MessageIds = newresponse.Split(' ').ToList();

            if (newresponse != null)
            {
                message = "success";
                status = true;
                var history = new TbSmsHistory();


                //foreach (var Id in studentsUserId)
                //{
                //    history.StuentId = Convert.ToInt64(Id);
                //    history.MessageDate = DateTime.UtcNow;
                //    history.MessageContent = model.Description;
                //    history.IsActive = true;
                //    history.ScholId = _user.SchoolId;
                //    _Entities.tb_SmsHistory.Add(history);
                //    _Entities.SaveChanges();
                //    //foreach (var num in MessageIds)
                //    //{
                //    //    var s = _Entities.tb_SmsHistory.Where(x => x.Id == history.Id).FirstOrDefault();
                //    //    if (s != null)
                //    //    {
                //    //        s.SendStatus = num;
                //    //    }


                //    //}
                //}


            }
            return responseText;
        }

        [HttpPost]
        public object SMS(SchoolModelForId model)
        {
            HttpClient client = new HttpClient();
            var history = new TbSmsHistory();
            var numbers = new List<string>();
            var MsgId = new List<string>();
            var statusFail = "100";
            var numb = "";
            string message = "Failed";
            var status = false;
            string messagepre = "";
            long studentId = 0;
            List<SendMessage> Userdata = JsonConvert.DeserializeObject<List<SendMessage>>(model.Data).ToList();
            //foreach (var item in Userdata)

            var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            if (school.SmsActive)
            {
                var package = _Entities.TbSmsPackages.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.IsDisabled == false).FirstOrDefault();
                if (package != null)
                {
                    if (package.ToDate >= CurrentTime)
                    {
                        //     ---------------------------------

                        if (Userdata.Count > 0)
                        {
                            var senderName = "MYSCHO";
                            //if (_user.SchoolId == 10116)
                            //{
                            //    senderName = "PARDSE";
                            //}
                            //else if (_user.SchoolId == 10117)
                            //{
                            //    senderName = "HOLYIN";
                            //}
                            var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                            if (senderData != null)
                                senderName = senderData.SenderId;
                            message = "success";
                            status = true;
                            var smsHead = new TbSmsHead();
                            smsHead.Head = Userdata[0].Description;
                            smsHead.SchoolId = _user.SchoolId;
                            smsHead.TimeStamp = CurrentTime;
                            smsHead.IsActive = true;
                            smsHead.SenderType = (int)SMSSendType.Student;
                            _Entities.TbSmsHeads.Add(smsHead);
                            status = _Entities.SaveChanges() > 0;

                            if (Userdata[0].list.Count > 0 && Userdata[0].list != null)
                            {
                                foreach (var ms in Userdata[0].list)
                                {
                                    studentId = Convert.ToInt64(ms.StudentId);
                                    var studentDetails = _Entities.TbStudents.Where(z => z.StudentId == studentId).FirstOrDefault();
                                    messagepre = "Dear Parent of " + studentDetails.StundentName;
                                    messagepre = messagepre + ", " + Userdata[0].Description;
                                    //-----SPECIAL CHARACTER SENDING -------------------
                                    messagepre = messagepre.Replace("#", "%23");
                                    messagepre = messagepre.Replace("&", "%26");
                                    //--------------------------------------------------
                                    var phone = ms.Number.ToString();
                                    int length = messagepre.Length;
                                    int que = length / 160;
                                    int rem = length % 160;
                                    if (rem > 0)
                                        que++;
                                    int smsCount = que;
                                    var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + messagepre + "&sender=" + senderName;
                                    //  var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + item.Description + "&priority=ndnd&stype=normal";

                                    ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    HttpWebRequest request = this.GetRequest(url);
                                    WebResponse webResponse = request.GetResponse();
                                    var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                                    var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                                    //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
                                    alvosmsResp respList = JsonConvert.DeserializeObject<alvosmsResp>(responseText);

                                    if (status)
                                    {
                                        statusFail = "0";

                                        TbSmsHistory sms = new TbSmsHistory();
                                        sms.IsActive = true;
                                        sms.MessageContent = Userdata[0].Description;
                                        sms.MessageDate = CurrentTime;
                                        sms.ScholId = _user.SchoolId;
                                        sms.StuentId = studentId;
                                        sms.MobileNumber = phone;
                                        sms.HeadId = smsHead.HeadId;
                                        sms.SendStatus = Convert.ToString(respList.success);
                                        if (respList.data != null)
                                        {
                                            sms.MessageReturnId = respList.data[0].messageId;
                                            sms.DelivaryStatus = "Pending";
                                        }
                                        sms.SmsSentPerStudent = smsCount;
                                        try
                                        {
                                            _Entities.TbSmsHistories.Add(sms);
                                            _Entities.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        string emailId = studentDetails.ParentEmail;
                                        string subject = "Message from School";
                                        string schoolName = studentDetails.School.SchoolName;
                                        SendSMSEmail(messagepre, emailId, subject, schoolName);// Send EMAIL for student SMS
                                        SendSMSPush(studentDetails, messagepre);// Send PUSH for student SMS

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Package validity expired";
                        statusFail = "2";
                    }
                }
                else
                {
                    message = "Please update your package";
                    statusFail = "3";
                }
            }
            else
            {
                message = "SMS cannot be send please contact our support team";
                statusFail = "1";
            }
            return Json(new { Status = status, statusFail = statusFail, message = message });

        }

        [HttpPost]
        public object HomeworkSMS(SchoolModelForId model)
        {
            HttpClient client = new HttpClient();
            var history = new TbSmsHistory();
            var numbers = new List<string>();
            var MsgId = new List<string>();
            ViewBag.Subjects = _dropdown.GetSubjectss(model.SchoolId);
            var numb = "";
            string message = "Failed to send sms";
            var status = false;
            string messagepre = "";
            long studentId = 0;
            List<SendMessage> Userdata = JsonConvert.DeserializeObject<List<SendMessage>>(model.Data).ToList();
            //foreach (var item in Userdata)
            var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            if (school.SmsActive)
            {
                var package = _Entities.TbSmsPackages.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.IsDisabled == false).FirstOrDefault();
                if (package != null)
                {
                    if (package.ToDate >= CurrentTime)
                    {
                        //     ---------------------------------


                        if (Userdata.Count > 0)
                        {
                            var senderName = "MYSCHO";
                            //if (_user.SchoolId == 10116)
                            //{
                            //    senderName = "PARDSE";
                            //}
                            //else if (_user.SchoolId == 10117)
                            //{
                            //    senderName = "HOLYIN";
                            //}
                            var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                            if (senderData != null)
                                senderName = senderData.SenderId;
                            message = "success";
                            status = true;
                            var smsHead = new TbSmsHead();
                            smsHead.Head = Userdata[0].Description;
                            smsHead.SchoolId = _user.SchoolId;
                            smsHead.TimeStamp = CurrentTime;
                            smsHead.IsActive = true;
                            smsHead.SenderType = (int)SMSSendType.Student;
                            _Entities.TbSmsHeads.Add(smsHead);
                            status = _Entities.SaveChanges() > 0;


                            var homeworkSms = new TbHomeworkSm();
                            homeworkSms.HeadId = smsHead.HeadId;
                            homeworkSms.SchoolId = _user.SchoolId;
                            homeworkSms.TimeStamp = CurrentTime;
                            homeworkSms.IsActive = true;
                            _Entities.TbHomeworkSms.Add(homeworkSms);
                            status = _Entities.SaveChanges() > 0;
                            if (Userdata[0].list.Count > 0 && Userdata[0].list != null)
                            {
                                foreach (var ms in Userdata[0].list)
                                {
                                    studentId = Convert.ToInt64(ms.StudentId);
                                    var studentDetails = _Entities.TbStudents.Where(z => z.StudentId == studentId).FirstOrDefault();
                                    messagepre = "Dear Parent of " + studentDetails.StundentName;
                                    messagepre = messagepre + ", " + Userdata[0].Description;
                                    //-----SPECIAL CHARACTER SENDING -------------------
                                    messagepre = messagepre.Replace("#", "%23");
                                    messagepre = messagepre.Replace("&", "%26");
                                    //--------------------------------------------------
                                    var phone = ms.Number.ToString();
                                    int length = messagepre.Length;
                                    int que = length / 160;
                                    int rem = length % 160;
                                    if (rem > 0)
                                        que++;
                                    int smsCount = que;
                                    var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + messagepre + "&sender=" + senderName;
                                    //  var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + item.Description + "&priority=ndnd&stype=normal";

                                    ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    HttpWebRequest request = this.GetRequest(url);
                                    WebResponse webResponse = request.GetResponse();
                                    var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                                    var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                                    //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
                                    alvosmsResp respList = JsonConvert.DeserializeObject<alvosmsResp>(responseText);

                                    if (status)
                                    {
                                        TbSmsHistory sms = new TbSmsHistory();
                                        sms.IsActive = true;
                                        sms.MessageContent = Userdata[0].Description;
                                        sms.MessageDate = CurrentTime;
                                        sms.ScholId = _user.SchoolId;
                                        sms.StuentId = studentId;
                                        sms.MobileNumber = phone;
                                        sms.HeadId = smsHead.HeadId;
                                        sms.SendStatus = Convert.ToString(respList.success);
                                        if (respList.data != null)
                                        {
                                            sms.MessageReturnId = respList.data[0].messageId;
                                            sms.DelivaryStatus = "Pending";
                                        }
                                        sms.SmsSentPerStudent = smsCount;
                                        _Entities.TbSmsHistories.Add(sms);
                                        _Entities.SaveChanges();

                                        string emailId = studentDetails.ParentEmail;
                                        string subject = "Message from School";
                                        string schoolName = studentDetails.School.SchoolName;
                                        SendSMSEmail(messagepre, emailId, subject, schoolName);// Send EMAIL for student SMS
                                        SendSMSPush(studentDetails, messagepre);// Send PUSH for student SMS

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return message;
        }
        private void SendSMSPush(TbStudent studentDetails, string messagepre)
        {
            try
            {
                var tokenData = _Entities.TbDeviceTokens.Where(x => x.UserId == studentDetails.ParentId && x.IsActive == true && x.LoginStatus == 1).OrderBy(x => x.TokenId).FirstOrDefault();
                if (tokenData != null)
                {
                    var applicationID = "";
                    var senderId = "";
                    var pushData = _Entities.TbPushData.Where(x => x.SchoolId == studentDetails.SchoolId).FirstOrDefault();
                    if (pushData != null)
                    {
                        applicationID = pushData.LegacyNumber;
                        senderId = pushData.SenderId;
                    }
                    else
                    {
                        applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                        senderId = "47900857750";
                    }

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = tokenData.Token,
                        notification = new
                        {
                            body = messagepre,
                            title = "Message From School"
                        },
                        priority = "high",
                        data = new
                        {
                            Role = "School",
                            Function = "Message"
                        },
                        from = "School"
                    };
                    //var serializer = new JavaScriptSerializer();
                    //var json = serializer.Serialize(data);
                    var json=JsonConvert.SerializeObject(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private HttpWebRequest GetRequest(string url, string httpMethod = "GET", bool allowAutoRedirect = true)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

            request.Timeout = Convert.ToInt32(new TimeSpan(0, 5, 0).TotalMilliseconds);
            request.Method = httpMethod;
            return request;
        }

        public async Task <IActionResult> SmsHistory()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.Selecteddate = CurrentTime;
            model.Selecteddate_From = CurrentTime.ToString();
            model.Selecteddate_To = CurrentTime.ToString();
            Tuple<string, string, List<SmsHead>> tt = new Satluj_Latest.Data.WebsiteService().GetAllSmsHeadByDate(model.Selecteddate_From, model.Selecteddate_To, model.schoolId);
            ViewBag.msgCount = tt.Item1;
            ViewBag.TotalCount = tt.Item2;
            ViewBag.Result = tt.Item3;
            string allowedsms = "0";
            string extraSms = "0";
            string extraCost = "0";
            //var package = _Entities.SP_GetSmsPackage(_user.SchoolId).ToList().Select(y => new SpSmsPackage(y)).ToList
            var package1 = _Entities.GetSpSmsPackages
         .FromSqlRaw("EXEC SP_GetSmsPackage {0}",
                     _user.SchoolId).ToList();
            var package = package1
    .Select(y => new SpSmsPackage(y))
    .ToList();
            if (package.Count > 0)
            {
                var data = package.Where(z => z.IsDisabled == false && z.IsActive).FirstOrDefault();
                if (data != null)
                {
                    allowedsms = data.AllowedSms.ToString();
                    extraSms = data.ExtraSmsCount.ToString();
                    extraCost = data.ExtraAmount.ToString();
                }
            }
            ViewBag.Allowedsms = allowedsms;
            ViewBag.ExtraSms = extraSms;
            ViewBag.ExtraCost = extraCost;
            return View(model);

        }
        public object GetAllSmsHistoryOnDate()
        {
            var model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.Selecteddate = CurrentTime;
            return PartialView("~/Views/School/_pv_SmsHistory.cshtml", model);
        }
        public ActionResult SmsHistoryDetail(string id)
        {
            SMSHistoryModel model = new SMSHistoryModel();
            long headId = Convert.ToInt64(id);
            //ViewBag.Result = new Satluj_Latest.Data.SmsHead(headId).SmsHistory;
            var data = new Satluj_Latest.Data.SmsHead(headId);
            if (data != null)
            {
                model.headId = data.headId;
                model.head = data.head;
                model.SenderType = data.SenderType;
            }
            return View(model);
        }

        public ActionResult HomeworkSms()
        {
            var model = new HomeWorkSmsModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }

        #endregion


        public ActionResult Report()
        {
            var model = new DateClass();
            model.Selecteddate = CurrentTime;
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        [HttpPost]
        public object GetAllFeeHead()
        {
            long schoolId = _user.SchoolId;
            var result = new Satluj_Latest.Data.WebsiteService().gtGetAllFeeHead(schoolId);
            return Json(new { Status = true, Message = "", result = result });
        }
        [HttpPost]
        public object GetAllExpenceAccountHeads()
        {
            long schoolId = _user.SchoolId;
            var result = new Satluj_Latest.Data.WebsiteService().gtAllExpenceAccountHead(schoolId);
            return Json(new { Status = true, Message = "", result = result });
        }
        [HttpPost]
        public object GetAllIncomeAccountHeads()
        {
            long schoolId = _user.SchoolId;
            var result = new Satluj_Latest.Data.WebsiteService().gtAllIncomeAccountHead(schoolId);
            return Json(new { Status = true, Message = "", result = result });
        }

        public object GetIncomeAccountHeads(string id)
        {
            var result = new Satluj_Latest.Data.WebsiteService().GetIncomeAccountHeads(id);
            return Json(new { Status = true, Message = "", result = result });
        }
        [HttpPost]
        public object SubmitIncomeData(IncomeData model)
        {
            var message = "failed";
            var status = false;
            var income = new TbIncome();
            income.AccountHead = model.HeadValue;
            income.Amount = Convert.ToDouble(model.Amount);
            income.Particular = model.ParticularValue;
            income.SchoolId = _user.SchoolId;
            income.IsActive = true;
            var d = string.Format(model.SelectedDate, "MM/dd/yyyy");
            income.Date = Convert.ToDateTime(d);

            _Entities.TbIncomes.Add(income);
            if (_Entities.SaveChanges() > 0)
            {
                message = "success";
                status = true;
            }



            return Json(new { Status = status, Message = message, Id = income.Id });

        }
        [HttpPost]
        public object SubmitIncomeDataEdit(IncomeData model)
        {
            var message = "failed";
            var status = false;
            var income = _Entities.TbIncomes.Where(x => x.Id == model.Id).FirstOrDefault();
            var amount = income.Amount;
            if (income != null)
            {
                income.AccountHead = model.HeadValue;
                income.Amount = Convert.ToDouble(model.Amount);
                income.Particular = model.ParticularValue;
                income.SchoolId = _user.SchoolId;
                income.IsActive = true;
                var d = string.Format(model.SelectedDate, "MM/dd/yyyy");
                income.Date = Convert.ToDateTime(d);
            }

            if (_Entities.SaveChanges() > 0)
            {
                message = "success";
                status = true;
            }



            return Json(new { Status = status, Message = message, Amount = amount });

        }

        [HttpPost]
        public object SubmitExpenceDataEdit(IncomeData model)
        {
            var message = "failed";
            var status = false;
            var expence = _Entities.TbExpenses.Where(x => x.Id == model.Id).FirstOrDefault();
            var amount = expence.Amount;
            if (expence != null)
            {
                expence.AccountHead = model.HeadValue;
                expence.Amount = Convert.ToDouble(model.Amount);
                expence.Particular = model.ParticularValue;
                expence.SchoolId = _user.SchoolId;
                expence.IsActive = true;
                var d = string.Format(model.SelectedDate, "MM/dd/yyyy");
                expence.Date = Convert.ToDateTime(d);
            }

            if (_Entities.SaveChanges() > 0)
            {
                message = "success";
                status = true;
            }



            return Json(new { Status = status, Message = message, Amount = amount });

        }
        [HttpPost]
        public object SubmitExpenceData(IncomeData model)
        {
            var message = "failed";
            var status = false;
            var expence = new TbExpense();
            expence.AccountHead = model.HeadValue;
            expence.Amount = Convert.ToDouble(model.Amount);
            expence.Particular = model.ParticularValue;
            expence.SchoolId = _user.SchoolId;
            expence.IsActive = true;
            var d = string.Format(model.SelectedDate, "MM/dd/yyyy");
            expence.Date = Convert.ToDateTime(d);

            _Entities.TbExpenses.Add(expence);
            if (_Entities.SaveChanges() > 0)
            {
                message = "success";
                status = true;
            }

            return Json(new { Status = status, Message = message, Id = expence.Id });

        }
        [HttpGet]
        public PartialViewResult GetAllReportOnDate(string id)
        {

            var model = new DateClass();
            model.Selecteddate = Convert.ToDateTime(id);
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_ReportViewTable.cshtml", model);
        }
        [HttpGet]
        public PartialViewResult GetAllSmsHistoryReportOnDate(string Selecteddate_From, string Selecteddate_To)
        {

            var model = new SchoolModel();
            if (Selecteddate_From == null && Selecteddate_To == null)
            {

                model.Selecteddate = CurrentTime;
                model.schoolId = _user.SchoolId;
            }
            else
            {
                model.Selecteddate_From = Selecteddate_From;
                model.Selecteddate_To = Selecteddate_To;
                model.schoolId = _user.SchoolId;
            }
            Tuple<string, string, List<SmsHead>> tt = new Satluj_Latest.Data.WebsiteService().GetAllSmsHeadByDate(model.Selecteddate_From, model.Selecteddate_To, model.schoolId);
            ViewBag.msgCount = tt.Item1;
            ViewBag.TotalCount = tt.Item2;
            ViewBag.Result = tt.Item3;

            return PartialView("~/Views/School/_pv_SmsHistory.cshtml", model);
        }

        public PartialViewResult PrintIncomeExpenceData(string id)
        {

            var model = new DateClass();
            model.Selecteddate = Convert.ToDateTime(id);
            model.SchoolId = _user.SchoolId;
            //model.studentId = Convert.ToInt64(splitData[0]);
            //model.billNumber = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/School/_pv_PrintIncomeExpence.cshtml", model);
        }
        //---------------Archana 02-Feb-2018--------------
        public ActionResult CircularNotification()
        {
            var model = new Satluj_Latest.Models.CircularList();
            model.schoolId = _user.SchoolId;
            return View(model);
        }


        public PartialViewResult AddCircularNotifications()
        {
            var model = new AddCircularNotification();
            //model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_AddCircularNotifications.cshtml", model);
        }
        [HttpPost]
        public IActionResult DocumentUpload(IFormFile file)
        {
            bool status = false;
            string msg = "Failed";
            string filepath = string.Empty;

            string userId = Convert.ToString(_user.UserId);
            string schoolId = Convert.ToString(_user.SchoolId);

            if (file != null && file.Length > 0)
            {
                // wwwroot path
                string folderPath = Path.Combine(
                    _env.WebRootPath,
                    "Media",
                    schoolId,
                    "CircularData"
                );

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string finalFileName = originalFileName + extension;

                string fullPath = Path.Combine(folderPath, finalFileName);
                int counter = 1;

                while (System.IO.File.Exists(fullPath))
                {
                    finalFileName = $"{originalFileName}_{counter}{extension}";
                    fullPath = Path.Combine(folderPath, finalFileName);
                    counter++;
                }

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                filepath = $"/Media/{schoolId}/CircularData/{finalFileName}";
                msg = "Success";
                status = true;
            }

            return Json(new
            {
                Status = status,
                Message = msg,
                UserData = filepath
            });
        }

        [HttpPost]
        public object SubmitAddCircularNotifications(AddCircularNotification model)
        {
            long userId = _user.UserId;
            long schoolId = _user.SchoolId;
            string msg = "Failed";
            bool status = false;
            var circular = new TbCircular();
            try
            {
                circular.SchoolId = schoolId;
                circular.LoginType = _user.RoleId;
                circular.UserId = _user.UserId;
                string[] splitData = model.DocumentDateString.ToString().Split('-');
                var zdd = splitData[0];
                var zmm = splitData[1];
                var zyyyy = splitData[2];
                var Date = zmm + '-' + zdd + '-' + zyyyy;
                circular.CircularDate = Convert.ToDateTime(Date);

                circular.Description = model.DocumentDetails;
                circular.FilePath = model.FilePath;
                circular.IsActive = true;
                circular.TimeStamp = CurrentTime;
                circular.CircularHead = model.DocumentHead;
                _Entities.TbCirculars.Add(circular);
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    //  Create notification entry added by dhanu for api 06 Nov 2025
                    var notification = new TbNotification();
                    notification.SchoolId = schoolId;
                    notification.ClassId = null;
                    notification.DivisionId = null;
                    notification.NotificationMessage = $"{model.DocumentHead}";
                    notification.CreatedAt = CurrentTime;
                    notification.IsRead = 0;
                    notification.Source = "Circular";
                    notification.UserName = null;
                    notification.SourceId = circular.CircularId;
                    _Entities.TbNotifications.Add(notification);
                    _Entities.SaveChanges();

                    msg = "Circular Added and Notification Created";
                }
                else
                {
                    msg = "Failed to Add Circular";
                }
                //msg = "Success";
                ///---------------Send Push and Mail for the all student's parents 
                //var student = _Entities.TbStudents.Where(x => x.IsActive && x.ParentId != null && x.SchoolId == _user.SchoolId).ToList();
                //if (student.Count > 0)
                //{
                //    foreach (var item in student)
                //    {
                //        var parentDetails = _Entities.tb_DeviceToken.Where(x => x.UserId == item.ParentId && x.LoginStatus == 1).ToList().GroupBy(x => new { x.UserId, x.TokenId }).Select(x => x.FirstOrDefault()).ToList();
                //        foreach (var data in parentDetails)
                //        {
                //            School schoolData = new School(_user.SchoolId);
                //            string school = schoolData.SchoolName;
                //            long studentUserId = Convert.ToInt64(item.StudentId);
                //            var message = "Your Kid's  School have a circular, " + " , " + circular.CircularHead+" , "+ circular.Description + " , " + circular.CircularDate.ToShortDateString();
                //            onlyCircularPushandroid(data.Token, message, school, studentUserId, item.SchoolId, circular);
                //            SendMailsForOnlyCircularNotification(item, circular, school);
                //            status = true;
                //            msg = "Successful";
                //        }
                //    }
                //}//commented by dhanu 3rd Nov 2025
            }


            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = status ? "Circular notification added successfully" : "Failed to Circular notification documents", UserData = circular.SchoolId });
        }

        private bool SendMailsForOnlyCircularNotification(TbStudent item, TbCircular circular, string school)
        {
            //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/template/CircularNotification.html");
            var filePath = Path.Combine(_env.WebRootPath, "Content", "template", "CircularNotification.html");
            var emailTemplate = System.IO.File.ReadAllText(filePath);
            string Message = "Your kid " + item.StundentName + "'s School have a Circular";
            string message2 = circular.Description + " , " + circular.CircularDate.ToShortDateString();
            string filePathDoc = "http://sutluj.schoolman.in" + circular.FilePath;
            string downLoad = "Download";
            if (circular.FilePath == null || circular.FilePath == "")
            {
                downLoad = "No files for Download";
            }
            var mBody = emailTemplate.Replace("{{resetLink}}", message2).Replace("{{resetLink1}}", school).Replace("{{resetLink2}}", Message).Replace("{{resetLink3}}", filePathDoc).Replace("{{resetLink4}}", downLoad);
            bool sendMail = Send("Circular", mBody, item.ParentEmail);
            return sendMail;
        }

        [HttpGet]
        public PartialViewResult CircularNotificationDataList()
        {
            Satluj_Latest.Models.CircularList model = new Satluj_Latest.Models.CircularList();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_CircularNotification.cshtml", model);
        }
        public object DeleteCircularNotification(string id)
        {
            bool status = false;
            string msg = "False";
            long circularId = Convert.ToInt64(id);
            var note = _Entities.TbCirculars.FirstOrDefault(x => x.CircularId == circularId);
            if (note != null)
            {
                var notifications = _Entities.TbNotifications
                .Where(n => n.Source == "Circular" && n.SourceId == circularId)
                .ToList();

                if (notifications.Any())
                {
                    _Entities.TbNotifications.RemoveRange(notifications);
                }

                if (note.FilePath != null)
                {
                    string filePath = note.FilePath;
                    //string path = Server.MapPath("~" + filePath);
                    string path = Path.Combine(_env.WebRootPath, filePath.TrimStart('~', '/'));

                    FileInfo file = new FileInfo(path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                note.IsActive = false;
                note.FilePath = null;
                status = _Entities.SaveChanges() > 0;
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult EditDocument(string id)
        {
            long circularId = Convert.ToInt64(id);
            AddCircularNotification model = new AddCircularNotification();
            var data = _Entities.TbCirculars.Where(x => x.CircularId == circularId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.DocumentDateString = data.CircularDate.ToString("dd-MM-yyyy");
                model.DocumentDetails = data.Description;
                model.FilePath = data.FilePath;
                model.DocumentHead = data.CircularHead;
                model.CircularId = data.CircularId;
            }
            return PartialView("~/Views/School/_pv_EditCircularNotifications.cshtml", model);
        }
        public object SubmitEditCircularNotifications(AddCircularNotification model)
        {
            bool status = false;
            string msg = "Failed";
            var circular = _Entities.TbCirculars.FirstOrDefault(x => x.CircularId == model.CircularId && x.IsActive);
            if (circular != null)
            {
                if (circular.FilePath != null)
                {
                    if (circular.FilePath != model.FilePath)
                    {
                        string filePath = circular.FilePath;
                        //string path = Server.MapPath("~" + filePath);
                        string path = Path.Combine(_env.WebRootPath, filePath.TrimStart('~', '/'));
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string[] splitData = model.DocumentDateString.ToString().Split('-'); 
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        circular.CircularDate = Convert.ToDateTime(Date);

                        circular.FilePath = model.FilePath;
                        circular.Description = model.DocumentDetails;
                        circular.CircularHead = model.DocumentHead;
                        status = _Entities.SaveChanges() > 0;
                    }
                    else
                    {
                        string[] splitData = model.DocumentDateString.ToString().Split('-');
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        circular.CircularDate = Convert.ToDateTime(Date);

                        circular.Description = model.DocumentDetails;
                        circular.CircularHead = model.DocumentHead;
                        status = _Entities.SaveChanges() > 0;
                    }

                }
                else
                {
                    if (model.DocumentDate == DateTime.MinValue)
                    {
                        circular.CircularDate = new DateTime(1753, 1, 1); // or null
                    }
                    else
                    {
                        circular.CircularDate = model.DocumentDate;
                    }

                   // circular.CircularDate = model.DocumentDate;
                    circular.FilePath = model.FilePath;
                    circular.Description = model.DocumentDetails;
                    circular.CircularHead = model.DocumentHead;
                    status = _Entities.SaveChanges() > 0;
                }
            }
            if (status)
                msg = "Success";
            else
                msg = "Failed";
            return Json(new { status = status, msg = msg });
        }

        public ActionResult NavigationHistory()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.curredntDateTime = CurrentTime;
            return View(model);
        }
        public PartialViewResult GpsHistoryGrid(string id)
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.curredntDateTime = Convert.ToDateTime(id);

            return PartialView("~/Views/School/_pv_GPSHistory_Grid.cshtml", model);
        }
        public PartialViewResult NavigationHistoryDetails(string id)
        {
            string[] splitData = id.Split('~');
            long busId = Convert.ToInt64(splitData[0]);
            var selDate = splitData[1];
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.curredntDateTime = Convert.ToDateTime(selDate);
            model.busId = busId;
            return PartialView("~/Views/School/_pv_GPSHistoryDetail_Grid.cshtml", model);

        }
        public object GetLatestExpParticulr(string id)
        {
            var msg = "Failed";
            var status = false;
            long schoolId = _user.SchoolId;
            var res = new Satluj_Latest.Data.WebsiteService().GetLatestExpParticular(id, schoolId);
            if (res != null)
            {
                status = true;
                msg = "Success";

            }

            return Json(new { status = status, msg = msg, value = res });

        }
        public object GetLatestIncParticulr(string id)
        {
            var msg = "Failed";
            var status = false;
            long schoolId = _user.SchoolId;

            var res = new Satluj_Latest.Data.WebsiteService().GetLatestIncParticular(id, schoolId);
            if (res != null)
            {
                status = true;
                msg = "Success";

            }

            return Json(new { status = status, msg = msg, value = res });

        }

        #region Accounts
        public ActionResult TrialBalance()
        {
            SchoolModel model = new SchoolModel();
            model.curredntDateTime = CurrentTime;
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public ActionResult BalanceSheet()
        {
            SchoolModel model = new SchoolModel();
            model.curredntDateTime = CurrentTime;
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public ActionResult IncomeAndExpenditure()
        {
            SchoolModel model = new SchoolModel();
            model.curredntDateTime = CurrentTime;
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        #endregion
        //--------------------Archana 19-Feb-2018 -----------------
        [HttpGet]
        public ActionResult StudentDetailedView(string id)
        {
            long studentId = Convert.ToInt64(id);

            var student = _Entities.TbStudents.Where(x => x.IsActive && x.StudentId == studentId).FirstOrDefault();
            StudentModel model = new StudentModel();
            model.studentId = studentId;
            model.studentName = student.StundentName;
            model.className = student.Class.Class;
            model.division = student.Division.Division;
            model.admissionNo = student.StudentSpecialId;
            model.address = student.Address;
            model.filePath = student.FilePath;
            model.gender = student.Gender;
            model.classId = student.ClassId;
            model.divisionId = student.DivisionId;
            model.contactNo = student.ContactNumber;
            if (student.ParentId != null)
            {
                model.parentName = student.Parent.ParentName ?? "";
                model.parentEmail = student.Parent.Email;
            }
            else
            {
                var xx = student.ParentName.ToString();
                model.parentName = student.ParentName;
                model.parentEmail = student.ParentEmail;
            }
            model.rollNo = student.ClasssNumber;
            return View(model);
        }
        public object GetAttendanceCountData(string id)
        {
            string msg = "Faile";
            int totalCount = 0;
            int absentCount = 0;
            long studentId = 0;
            DateTime date = CurrentTime;
            string variable = Convert.ToString(id);
            if (id != null)
            {
                string[] data = Regex.Split(variable, "~");
                if (data.Count() == 2)
                {
                    date = Convert.ToDateTime("01-" + data[0] + "-" + CurrentTime.Year);
                    studentId = Convert.ToInt64(data[1]);
                    //var returnData = _Entities.sp_TotalAttendance(date, studentId).ToList();
                    //if (returnData.Count > 0)
                    //{
                    //    msg = "Success";
                    //    totalCount = returnData.Count();
                    //    absentCount = returnData.Where(x => x.AttendanceData == false).ToList().Count();
                    //}
                    List<AttendanceDetails> attendanceDataList = new List<AttendanceDetails>();
                    var attendanceData = _Entities.TbAttendances.Where(x => x.AttendanceDate.Year == CurrentTime.Year && x.AttendanceDate.Month == date.Month && x.StudentId == studentId && x.IsActive).ToList().Select(x => new TbAttendance(x)).ToList();
                    List<DateTime> attendanceTime = attendanceData.Select(z => z.AttendanceDate.Date).Distinct().ToList();
                    if (attendanceData.Count > 0)
                    {
                        foreach (var item in attendanceTime)
                        {
                            AttendanceDetails obj = new AttendanceDetails();
                            obj.attendanceDate = item.ToShortDateString();
                            obj.mornignShift = false;
                            obj.eveningShift = false;
                            var eachDay = attendanceData.Where(z => z.AttendanceDate.Date == item.Date).ToList();
                            if (eachDay.Count > 0)
                            {
                                var morningStatus = eachDay.FirstOrDefault(z => z.ShiftStatus == 0);
                                var eveningStatus = eachDay.FirstOrDefault(z => z.ShiftStatus == 1);
                                if (morningStatus != null)
                                {
                                    if (morningStatus.AttendanceData)
                                        obj.mornignShift = true;
                                }
                                if (eveningStatus != null)
                                {
                                    if (eveningStatus.AttendanceData)
                                        obj.eveningShift = true;
                                }
                            }

                            attendanceDataList.Add(obj);
                        }
                    }
                    if (attendanceDataList.Count > 0)
                    {
                        msg = "Success";
                        //totalCount = 2 * attendanceDataList.Count();
                        //absentCount = attendanceDataList.Where(x => x.mornignShift == false).ToList().Count();
                        //absentCount = absentCount + attendanceDataList.Where(x => x.eveningShift == false).ToList().Count();
                        totalCount = attendanceData.Count();
                        absentCount = attendanceData.Where(x => x.AttendanceData == false).ToList().Count();
                    }
                }
            }
            return Json(new { msg = msg, totalCount = totalCount, absentCount = absentCount });
        }

        public ActionResult CurriculumDetails()
        {
            var model = new SchoolValue();
            model.schoolId = _user.SchoolId;
            ViewBag.Classes = _dropdown.GetClasses(model.schoolId);

            return View(model);
        }
        public PartialViewResult DatatableExamList(Satluj_Latest.Models.FilterModel model)
        {
            //return PartialView("~/Views/School/_pv_SchoolExaminationList.cshtml", model);
            return PartialView("~/Views/School/_pv_SchoolExamList.cshtml", model);
        }

        public PartialViewResult AddNewExams()
        {
            var model = new ExamsModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_AddNewExams.cshtml", model);
        }
        public object SubmitAddExam(ExamsModel model)
        {
            long userId = _user.UserId;
            long schoolId = _user.SchoolId;
            string msg = "Failed";
            bool status = false;
            if (_Entities.TbExams.Any(x => x.SchoolId == schoolId && x.IsActive && x.ClassId == model.ClassId && x.ExamName == model.ExamName.ToUpper().Trim()))
            {
                msg = "Exam Already Exists";
            }
            else
            {
                var exam = new  TbExam();
                exam.SchoolId = schoolId;
                exam.ClassId = model.ClassId;
                exam.ExamName = model.ExamName.ToUpper().Trim();
                exam.IsActive = true;
                exam.TimeStamp = CurrentTime;
                exam.UserId = userId;
                try
                {
                    if (model.StartDateString != string.Empty)
                    {
                        string[] splitData = model.StartDateString.ToString().Split('-');
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.StartDate = Convert.ToDateTime(Date);
                    }
                    if (model.EndDateString != string.Empty)
                    {
                        string[] splitData = model.EndDateString.ToString().Split('-');
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.EndDate = Convert.ToDateTime(Date);
                    }
                }
                catch
                {
                    if (model.StartDateString != string.Empty)
                    {
                        string[] splitData = model.StartDateString.ToString().Split('-');
                        var zmm = splitData[0];
                        var zdd = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.StartDate = Convert.ToDateTime(Date);
                    }
                    if (model.EndDateString != string.Empty)
                    {
                        string[] splitData = model.EndDateString.ToString().Split('-');
                        var zmm = splitData[0];
                        var zdd = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.EndDate = Convert.ToDateTime(Date);
                    }
                }
                _Entities.TbExams.Add(exam);
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Success";
            }
            return Json(new { status = status, message = status ? "Exam added successfully" : "Failed to Exam documents", UserData = _user.SchoolId });
        }

        public ActionResult ExamSubjectDetails(string id)
        {
            var model = new ExamSubjectDetailsModel();
            model.ExamId = Convert.ToInt64(id);
            var data = _Entities.TbExams.Where(x => x.ExamId == model.ExamId).FirstOrDefault();
            model.ExamName = data.ExamName;
            return View(model);
        }

        public object DeleteSchoolexams(string id)
        {
            bool status = false;
            string msg = "False";
            long examId = Convert.ToInt64(id);
            var exam = _Entities.TbExams.FirstOrDefault(x => x.ExamId == examId && x.IsActive);
            if (exam != null)
            {
                exam.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult EditExam(string id)
        {
            long examId = Convert.ToInt64(id);
            ExamsModel model = new ExamsModel();
            var data = _Entities.TbExams.Where(x => x.ExamId == examId && x.IsActive).FirstOrDefault();
            ViewBag.Classlist = _dropdown.GetClasses(model.SchoolId);
            if (data != null)
            {
                model.ExamId = data.ExamId;
                model.SchoolId = data.SchoolId;
                model.UserId = data.UserId;
                model.ClassId = data.ClassId;
                model.ExamName = data.ExamName;
                if (data.StartDate != null)
                    model.StartDateString = data.StartDate.Value.ToString("MM-dd-yyyy");
                if (data.EndDate != null)
                    model.EndDateString = data.EndDate.Value.ToString("MM-dd-yyyy");
            }
            return PartialView("~/Views/School/_pv_EditExam.cshtml", model);
        }

        public object SubmitEditExam(ExamsModel model)
        {
            bool status = false;
            string msg = "Failed";
            var old = _Entities.TbExams.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ClassId == model.ClassId && x.ExamName == model.ExamName.ToUpper().Trim()).Count();
            if (old > 1)
            {
                msg = "Exam Already Exists";
            }
            else
            {
                var exam = _Entities.TbExams.Where(z => z.ExamId == model.ExamId && z.IsActive).FirstOrDefault();
                exam.ClassId = model.ClassId;
                exam.ExamName = model.ExamName.ToUpper().Trim();
                try
                {
                    if (model.StartDateString != string.Empty)
                    {
                        string[] splitData = model.StartDateString.ToString().Split('-');
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.StartDate = Convert.ToDateTime(Date);
                    }
                    if (model.EndDateString != string.Empty)
                    {
                        string[] splitData = model.EndDateString.ToString().Split('-');
                        var zdd = splitData[0];
                        var zmm = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.EndDate = Convert.ToDateTime(Date);
                    }
                }
                catch
                {
                    if (model.StartDateString != string.Empty)
                    {
                        string[] splitData = model.StartDateString.ToString().Split('-');
                        var zmm = splitData[0];
                        var zdd = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.StartDate = Convert.ToDateTime(Date);
                    }
                    if (model.EndDateString != string.Empty)
                    {
                        string[] splitData = model.EndDateString.ToString().Split('-');
                        var zmm = splitData[0];
                        var zdd = splitData[1];
                        var zyyyy = splitData[2];
                        var Date = zmm + '-' + zdd + '-' + zyyyy;
                        exam.EndDate = Convert.ToDateTime(Date);
                    }
                }

                status = _Entities.SaveChanges() > 0;
                msg = status ? " Successfull" : "No changes made";
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult AddNewSubjects(string id)
        {
            var model = new ExamSubjectDetailsModel();
            model.ExamId = Convert.ToInt64(id);
            model.SchoolId = _user.SchoolId;
            model.ExamDate = CurrentTime;
            var data = _Entities.TbExams.Where(x => x.ExamId == model.ExamId).FirstOrDefault();
            if (data != null)
            {
                model.StartDate = data.StartDate ?? CurrentTime;
                model.EndDate = data.EndDate ?? CurrentTime;
            }
            return PartialView("~/Views/School/_pv_AddNewSubjects.cshtml", model);
        }
        public object SubmitAddSubject(ExamSubjectDetailsModel model)
        {
            string msg = "Failed";
            bool status = false;
            //DateTime examDate = CurrentTime;
            //try
            //{
            //    examDate = Convert.ToDateTime(model.ExamDate.ToString("MM-dd-yyyy"));
            //}
            //catch
            //{
            //    string[] splitdate = model.ExamDate.ToString("dd-MM-yyyy").Split('-');
            //    var zdd = splitdate[0];
            //    var zmm = splitdate[1];
            //    var zyyyy = splitdate[2];
            //    var Date = zmm  + '-' + zdd + '-' + zyyyy;
            //    examDate = Convert.ToDateTime(Date);
            //}
            //if (Convert.ToDateTime(model.StartDate.ToString("MM-dd-yyyy")) <= examDate && Convert.ToDateTime(model.EndDate.ToString("MM-dd-yyyy")) >= examDate)
            //{
            try
            {
                var sub = new TbExamSubject();
                sub.ExamId = model.ExamId;
                sub.Subject = new Satluj_Latest.Data.Subjects(model.SubjectId).SubjectName;
                sub.InternalMarks = model.Internal;
                sub.ExternalMark = model.External;
                //sub.Mark = model.Total;
                sub.Mark = sub.InternalMarks + sub.ExternalMark;
                sub.IsActive = true;
                sub.TimeStamp = CurrentTime;
                sub.SubjectId = model.SubjectId;
                sub.ExamDate = model.ExamDate.Add(model.ExamTime);
                _Entities.TbExamSubjects.Add(sub);
                status = _Entities.SaveChanges() > 0;
                if (status)
                    msg = "Subject added successfully";
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }
            //}
            //else
            //{
            //    msg = "This exam date is not in between the exam start and end dates";
            //}
            return Json(new { status = status, message = msg, UserData = model.ExamId });
        }

        public PartialViewResult ExamSubjectList(string id)
        {
            ExamSubjectDetailsModel model = new ExamSubjectDetailsModel();
            model.ExamId = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_ExamSubjectList.cshtml", model);
        }

        public object DeleteSchoolSubject(string id)
        {
            bool status = false;
            string msg = "False";
            long subId = Convert.ToInt64(id);
            var sub = _Entities.TbExamSubjects.FirstOrDefault(x => x.SubId == subId && x.IsActive);
            if (sub != null)
            {
                sub.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg, UserData = subId });
        }

        public PartialViewResult EditSubjects(string id)
        {
            long subjectId = Convert.ToInt64(id);
            ExamSubjectDetailsModel model = new ExamSubjectDetailsModel();
            var data = _Entities.TbExamSubjects.Where(x => x.SubId == subjectId && x.IsActive).FirstOrDefault();
            ViewBag.Subjectlist = _dropdown.GetSubjectss(model.SchoolId);
            if (data != null)
            {
                model.ExamId = data.ExamId;
                model.Subject = data.Subject;
                model.Internal = data.InternalMarks;
                model.External = data.ExternalMark;
                model.Total = data.Mark;
                model.ExamSubjectId = data.SubId;
                model.SubjectId = data.SubjectId;
                model.SchoolId = _user.SchoolId;
                model.ExamDate = Convert.ToDateTime(data.ExamDate.ToShortDateString());
                //string[] data = Regex.Split(variable, "~");
                string timeSpan = "";
                string[] split = data.ExamDate.ToShortTimeString().Split(':');
                if (split.Count() > 1)
                {
                    if (split[0].Length == 2)
                    {
                        timeSpan = split[0];
                    }
                    else
                    {
                        timeSpan = "0" + split[0];
                    }
                    //timeSpan = timeSpan+":"+ split[1].Substring(0,2) + ":00";
                    timeSpan = timeSpan + ":" + split[1].Substring(0, 2);
                }
                model.ExamTime = TimeSpan.Parse(timeSpan);
            }
            return PartialView("~/Views/School/_pv_EditSubject.cshtml", model);
        }
        public object SubmitEditSubject(ExamSubjectDetailsModel model)
        {
            bool status = false;
            string msg = "Failed";
            var sub = _Entities.TbExamSubjects.Where(z => z.ExamId == model.ExamId && z.SubId == model.ExamSubjectId && z.IsActive).FirstOrDefault();
            sub.Subject = new Satluj_Latest.Data.Subjects(model.SubjectId).SubjectName;
            sub.InternalMarks = model.Internal;
            sub.ExternalMark = model.External;
            //sub.Mark = model.Total;
            sub.Mark = sub.InternalMarks + sub.ExternalMark;
            sub.SubjectId = model.SubjectId;
            sub.ExamDate = model.ExamDate.Add(model.ExamTime);
            status = _Entities.SaveChanges() > 0;
            msg = status ? " Successful" : "No changes made";
            return Json(new { status = status, msg = msg, UserData = sub.ExamId });
        }

        public PartialViewResult DatatableSubjectList(string id)
        {
            AddStudentMarkModel model = new AddStudentMarkModel();
            model.StudentList = new List<StudentList>();
            string[] splitdata = id.Split('~');
            model.SubjectId = Convert.ToInt64(splitdata[0]);
            model.SchoolId = _user.SchoolId;
            var data = new Satluj_Latest.Data.Exams(model.SubjectId, model.SchoolId);
            var realSubjectId = data.ExamSubjectsList.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefault();
            var subjectData = new Satluj_Latest.Data.Subjects(realSubjectId.SubjectId);
            model.DivisionId = Convert.ToInt64(splitdata[1]);
            model.ExamId = data.ExamId;
            model.ClassId = data.ClassId;
            if (subjectData.IsOptonal == false)
            {
                //-----------------------------------------------Normal--------------------------------------------------
                var totalCount = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();// Total Students
                //var dataList = _Entities.sp_StudentMarkList(model.ExamId, model.SubjectId, model.DivisionId).ToList(); //Students Mark Details
            var dataList =    _Entities.sp_StudentMarkListResult
         .FromSqlRaw("EXEC sp_StudentMarkList {0}, {1}, {2}",
                    model.ExamId, model.SubjectId, model.DivisionId)
         .ToList();

                if (dataList.Count > 0)
                {
                    var subject = data.ExamSubjectsList.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefault();
                    model.TotalInternalMark = subject.InternalMarks;
                    model.TotalExternalMark = subject.ExternalMark;

                    if (dataList.Count == totalCount.Count)// Full Students attend the exam
                    {
                        foreach (var item in dataList)
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId ?? 0;
                            one.StudentName = item.StundentName;
                            one.InternalMark = item.StudentInternalMark;
                            one.ExternalMark = item.StudentExternalMark;
                            one.Total = item.StudentTotalMark;
                            one.ExamId = item.ExamId;
                            one.SubjectId = item.SubId;
                            model.StudentList.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in dataList)// Full Students attend the exam
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId ?? 0;
                            one.StudentName = item.StundentName;
                            one.InternalMark = item.StudentInternalMark;
                            one.ExternalMark = item.StudentExternalMark;
                            one.Total = item.StudentTotalMark;
                            one.ExamId = item.ExamId;
                            one.SubjectId = item.SubId;
                            model.StudentList.Add(one);
                        }
                        var result = totalCount.Where(p => !dataList.Any(p1 => p1.StudentId == p.StudentId)).ToList();
                        foreach (var item in result)// Remainig Students whoes not attend the exam
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.StundentName;
                            one.InternalMark = 0;
                            one.ExternalMark = 0;
                            one.Total = 0;
                            one.ExamId = data.ExamId;
                            one.SubjectId = model.SubjectId;
                            model.StudentList.Add(one);
                        }
                    }
                }
                else
                {
                    //basheer on 25/01/2019 to view marks who did not write exam
                    var subject = data.ExamSubjectsList.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefault();
                    model.TotalInternalMark = subject.InternalMarks;
                    model.TotalExternalMark = subject.ExternalMark;
                    foreach (var item in totalCount) // No entry of result
                    {

                        StudentList one = new StudentList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one.InternalMark = 0;
                        one.ExternalMark = 0;
                        one.Total = 0;
                        one.ExamId = data.ExamId;
                        one.SubjectId = model.SubjectId;
                        model.StudentList.Add(one);
                    }
                }
            }
            //----------------------------------------------------------------------------------------
            //--------------------------------------OPTIONAL SUBJECTS--------------------------------------------------
            else
            {
                var totalCount = _Entities.TbOptionalSubjectStudents.Where(x => x.SchoolId == _user.SchoolId && x.Student.ClassId == model.ClassId && x.Student.DivisionId == model.DivisionId && x.IsActive && x.Student.IsActive==true && x.SubjectId==realSubjectId.SubjectId).ToList();// Total optional Students
                //var dataList = _Entities.sp_StudentMarkList(model.ExamId, model.SubjectId, model.DivisionId).ToList(); //Students Mark Details
                var dataList = _Entities.sp_StudentMarkListResult
         .FromSqlRaw("EXEC sp_StudentMarkList {0}, {1}, {2}",
                     model.ExamId, model.SubjectId, model.DivisionId)
         .ToList();

                if (dataList.Count > 0)
                {
                    var subject = data.ExamSubjectsList.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefault();
                    model.TotalInternalMark = subject.InternalMarks;
                    model.TotalExternalMark = subject.ExternalMark;

                    if (dataList.Count >= totalCount.Count)// Full Students attend the exam
                    {
                        foreach (var item in dataList)
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId ?? 0;
                            one.StudentName = item.StundentName;
                            one.InternalMark = item.StudentInternalMark;
                            one.ExternalMark = item.StudentExternalMark;
                            one.Total = item.StudentTotalMark;
                            one.ExamId = item.ExamId;
                            one.SubjectId = item.SubId;
                            model.StudentList.Add(one);
                        }
                    }
                    else
                    {
                        foreach (var item in dataList)// Full Students attend the exam
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId ?? 0;
                            one.StudentName = item.StundentName;
                            one.InternalMark = item.StudentInternalMark;
                            one.ExternalMark = item.StudentExternalMark;
                            one.Total = item.StudentTotalMark;
                            one.ExamId = item.ExamId;
                            one.SubjectId = item.SubId;
                            model.StudentList.Add(one);
                        }
                        var result = totalCount.Where(p => !dataList.Any(p1 => p1.StudentId == p.StudentId)).ToList();
                        foreach (var item in result)// Remainig Students whoes not attend the exam
                        {
                            StudentList one = new StudentList();
                            one.StudentId = item.StudentId;
                            one.StudentName = item.Student.StundentName;
                            one.InternalMark = 0;
                            one.ExternalMark = 0;
                            one.Total = 0;
                            one.ExamId = data.ExamId;
                            one.SubjectId = model.SubjectId;
                            model.StudentList.Add(one);
                        }
                    }
                }
                else
                {
                    var subject = data.ExamSubjectsList.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefault();
                    model.TotalInternalMark = subject.InternalMarks;
                    model.TotalExternalMark = subject.ExternalMark;
                    foreach (var item in totalCount) // No entry of result
                    {

                        StudentList one = new StudentList();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.Student.StundentName;
                        one.InternalMark = 0;
                        one.ExternalMark = 0;
                        one.Total = 0;
                        one.ExamId = data.ExamId;
                        one.SubjectId = model.SubjectId;
                        model.StudentList.Add(one);
                    }
                }
            }
            model.StudentList = model.StudentList.OrderBy(XmlSiteMapProvider => XmlSiteMapProvider.StudentName).ToList();
            return PartialView("~/Views/School/_pv_AddStudentsMark.cshtml", model);
        }
        [HttpPost]
        public object SubmitAddStudentsMarks(StudentsMarkListData data1)
        {
            List<StudentList> data = new List<StudentList>();
            data = data1._ListData;
            bool status = false;
            string msg = "Failed";
            if (data.Count > 0)
            {
                var studentData = new Satluj_Latest.Data.Student(data[0].StudentId);
                long examId = Convert.ToInt64(data1.ExamId);
                long subjectId = Convert.ToInt64(data1.SubjectId);
                var olddata = _Entities.TbStudentMarks.Where(x => x.ExamId == examId && x.SubjectId == subjectId && x.Student.DivisionId==studentData.DivisionId).ToList();
                if (olddata.Count > 0)
                {
                    foreach (var item in olddata)
                    {
                        _Entities.TbStudentMarks.Remove(item);
                        status = _Entities.SaveChanges() > 0;
                    }
                }
                foreach (var item in data)
                {
                    var mark = new TbStudentMark();
                    mark.StudentId = item.StudentId;
                    mark.ExamId = examId;
                    mark.SubjectId = subjectId;
                    mark.Mark = item.Total;
                    mark.IsActive = true;
                    mark.TimeStamp = CurrentTime;
                    mark.InternalMark = item.InternalMark;
                    mark.ExternalMark = item.ExternalMark;
                    _Entities.TbStudentMarks.Add(mark);
                    status = _Entities.SaveChanges() > 0;
                    msg = "Success";
                }
                //var dataList = _Entities.sp_StudentMarkList(examId, subjectId).ToList(); //Students Mark Details
                //AddStudentMarkModel model = new AddStudentMarkModel();
                //model.StudentList = new List<StudentList>();
                //model.SubjectId = subjectId;
                //model.SchoolId = _user.SchoolId;
                //model.ExamId = examId;
                //foreach (var item in dataList)
                //{
                //    StudentList one = new StudentList();
                //    one.StudentId = item.StudentId ?? 0;
                //    one.StudentName = item.StundentName;
                //    one.InternalMark = item.StudentInternalMark ?? 0;
                //    one.ExternalMark = item.StudentExternalMark ?? 0;
                //    one.Total = item.StudentTotalMark;
                //    one.ExamId = item.ExamId;
                //    one.SubjectId = item.SubId;
                //    model.StudentList.Add(one);
                //}
                //model.StudentList = model.StudentList.OrderBy(XmlSiteMapProvider => XmlSiteMapProvider.StudentName).ToList();
                return Json(new { status = status, msg = msg });
            }
            else
            {
                msg = "Success";
                status = true;
                return Json(new { status = status, msg = msg });
            }
        }
        public ActionResult StudentMarks()
        {
            StudentMarksEntry model = new StudentMarksEntry();
            model.SchoolId = _user.SchoolId;
            model.UserId = _user.UserId;
            ViewBag.classlist = _dropdown.GetClasses(model.SchoolId);
            ViewBag.UserClasses= _dropdown.GetClassesUserWise(model.SchoolId, model.UserId);
            return View(model);
        }
        public ActionResult DiaryUpload()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        [HttpPost]
        public IActionResult DiaryUploadPDF(IFormFile file)
        {
            bool status = false;
            string msg = "failed";

            if (file != null && file.Length > 0)
            {
                try
                {
                    // wwwroot/Media/School/Diary
                    string folderPath = Path.Combine(
                        _env.WebRootPath,
                        "Media",
                        "School",
                        "Diary"
                    );

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string pdfName = $"{_user.School.SchoolName}{Guid.NewGuid()}.pdf";
                    string pdfFilePath = Path.Combine(folderPath, pdfName);

                    // Save file
                    using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    string fileSave = $"/Media/School/Diary/{pdfName}";

                    // Save to DB
                    TbFile fileEntity = new TbFile
                    {
                        FilePath = fileSave,
                        FileModule = 1, // Diary
                        FileType = 3,   // PDF
                        SchoolId = _user.SchoolId,
                        IsActive = true,
                        TimeStamp = CurrentTime
                    };

                    _Entities.TbFiles.Add(fileEntity);
                    status = _Entities.SaveChanges() > 0;
                    msg = "Success";
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }

            return Json(new { status, msg });
        }

        //}
        //------------------------ARCHANA K V --------16-Mar-2018-----------------
        public ActionResult StaffMessages()
        {
            var model = new SchoolModelForId();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        [HttpPost]
        public object SMSStaff(SchoolModelForId model)
        {
            HttpClient client = new HttpClient();
            var history = new TbSmsHistory();
            var numbers = new List<string>();
            var MsgId = new List<string>();

            var numb = "";
            string message = "Failed";
            var statusFail = "100";
            var status = false;
            string messagepre = "";
            long staffId = 0;
            int userType = 0;
            List<SendStaffMessage> Userdata = JsonConvert.DeserializeObject<List<SendStaffMessage>>(model.Data);

            var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            if (school.SmsActive)
            {
                var package = _Entities.TbSmsPackages.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.IsDisabled == false).FirstOrDefault();
                if (package != null)
                {
                    if (package.ToDate >= CurrentTime)
                    {
                        //     ---------------------------------


                        if (Userdata.Count > 0 && Userdata != null)
                        {
                            //foreach (var Userdata1 in Userdata)
                            {
                                var senderName = "MYSCHO";
                                //if (_user.SchoolId == 10116)
                                //{
                                //    senderName = "PARDSE";
                                //}
                                //else if (_user.SchoolId == 10117)
                                //{
                                //    senderName = "HOLYIN";
                                //}
                                var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                                if (senderData != null)
                                    senderName = senderData.SenderId;
                                message = "success";
                                status = true;
                                var smsHead = new TbSmsHead();
                                smsHead.Head = Userdata[0].Description;
                                smsHead.SchoolId = _user.SchoolId;
                                smsHead.TimeStamp = CurrentTime;
                                smsHead.IsActive = true;
                                smsHead.SenderType = (int)SMSSendType.Staff;
                                _Entities.TbSmsHeads.Add(smsHead);
                                status = _Entities.SaveChanges() > 0;

                                foreach (var item in Userdata[0].list)
                                {
                                    staffId = Convert.ToInt64(item.StaffId);
                                    userType = Convert.ToInt32(item.Type);
                                    var staffDetails = _Entities.TbLogins.Where(z => z.UserId == staffId && z.IsActive).FirstOrDefault();
                                    messagepre = "Dear " + staffDetails.Name;
                                    messagepre = messagepre + " ,  " + Userdata[0].Description;
                                    //-----SPECIAL CHARACTER SENDING -------------------
                                    messagepre = messagepre.Replace("#", "%23");
                                    messagepre = messagepre.Replace("&", "%26");
                                    //--------------------------------------------------
                                    var phone = item.Number.ToString();
                                    int length = messagepre.Length;
                                    int que = length / 160;
                                    int rem = length % 160;
                                    if (rem > 0)
                                        que++;
                                    int smsCount = que;
                                    var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + messagepre + "&sender=" + senderName;
                                    ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    HttpWebRequest request = this.GetRequest(url);
                                    WebResponse webResponse = request.GetResponse();
                                    var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                                    var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                                    //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
                                    alvosmsResp respList = JsonConvert.DeserializeObject<alvosmsResp>(responseText);

                                    if (status)
                                    {
                                        statusFail = "0";
                                        TbStaffSmshistory sms = new TbStaffSmshistory();
                                        sms.StaffId = staffId;
                                        sms.MessageContent = Userdata[0].Description;
                                        sms.SendStatus = Convert.ToString(respList.success);
                                        sms.MessageDate = CurrentTime;
                                        sms.IsActive = true;
                                        sms.ScholId = _user.SchoolId;
                                        sms.MobileNumber = phone;
                                        sms.SmsSentPerStudent = smsCount;
                                        sms.HeadId = smsHead.HeadId;
                                        sms.UserType = userType;
                                        if (respList.data != null)
                                        {
                                            sms.MessageReturnId = respList.data[0].messageId;
                                            sms.DelivaryStatus = "Pending";
                                        }
                                        _Entities.TbStaffSmshistories.Add(sms);
                                        _Entities.SaveChanges();
                                    }
                                    string emailId = staffDetails.Username;
                                    string subject = "Message from School";
                                    string schoolName = staffDetails.School.SchoolName;
                                    SendSMSEmail(messagepre, emailId, subject, schoolName);
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Package validity expired";
                        statusFail = "2";
                    }
                }
                else
                {
                    message = "Please update your package";
                    statusFail = "3";
                }
            }
            else
            {
                //Basheer on 25/01/2019 changed spelling mistake
                message = "SMS cannot be send please contact our support team";
                statusFail = "1";
            }
            return Json(new { Status = status, statusFail = statusFail, message = message });

        }

        private void SendSMSEmail(string msg, string emailId, string subject, string schoolName)
        {
            try
            {
                //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/template/BirthdayWishesEmail.html");
                var filePath = Path.Combine(
    _env.WebRootPath,
    "Content",
    "template",
    "BirthdayWishesEmail.html"
);

                var emailTemplate = System.IO.File.ReadAllText(filePath);
                var mBody = emailTemplate.Replace("{{resetLink}}", msg).Replace("{{resetLink1}}", schoolName);
                bool sendMail = Send(subject, mBody, emailId);
            }
            catch (Exception ex)
            { }
        }

        //--------------------------ARCHANA K V ------------20-MAR-2018
        public ActionResult SettingsHome()
        {
            FeeAlertDataModel model = new FeeAlertDataModel();
            var data = _Entities.TbFeeAlertData.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.AlertDate = data.AlertDate;
            }

            var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            model.SchoolId = _user.SchoolId;
            model.FooterMessage = school.BillingFooterMessage;
            model.libraryDueDay = school.LibraryDueDays ?? 0;
            var libDueFine = school.TbLibraryFines.FirstOrDefault();
            if (libDueFine != null)
            {
                model.feeId = libDueFine.FeeId;
                model.libFineAmount = libDueFine.FineAmount;
            }
            else
            {
                model.feeId = 0;
            }
            model.SenderDetails = new SchoolSenderIdModel();
            var sender = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (sender != null)
            {
                model.SenderDetails.SenderData = sender.SenderId;
                model.SenderDetails.SenderId = sender.Id;
            }
            else
            {
                model.SenderDetails.SenderData = "MYSCHO";
            }
            return View(model);
        }
        [HttpPost]
        public object AddBillingFooter(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
                school.BillingFooterMessage = model.FooterMessage;
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }

        [HttpPost]
        public object AddLibraryDueDays(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
                school.LibraryDueDays = model.libraryDueDay;
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
        [HttpPost]
        public object AddLibraryDueFine(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var libFine = _Entities.TbLibraryFines.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
                if (libFine != null)
                {
                    libFine.FeeId = model.feeId;
                    libFine.FineAmount = model.libFineAmount;
                }
                else
                {
                    TbLibraryFine LibraryFine = new TbLibraryFine();
                    LibraryFine.FeeId = model.feeId;
                    LibraryFine.FineAmount = model.libFineAmount;
                    LibraryFine.SchoolId = _user.SchoolId;
                    LibraryFine.IsActive = true;
                    LibraryFine.TimeStamp = CurrentTime;
                    _Entities.TbLibraryFines.Add(LibraryFine);
                }
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
        [HttpPost]
        public object AddNewFeeAlertDate(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var date = new TbFeeAlertDatum();
                date.AlertDate = Convert.ToDateTime(model.AlertDate);
                date.SchoolId = _user.SchoolId;
                date.IsActive = true;
                date.TimeStamp = CurrentTime;
                _Entities.TbFeeAlertData.Add(date);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }

        public object CheckSMSDeliveryData(string id)
        {
            string msg = "Failed";
            bool status = false;
            long head = Convert.ToInt64(id);
            var data = new Satluj_Latest.Data.SmsHead(head).StaffSMSHistory;
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.MessageReturnId != null)
                    {
                        var url = "http://alvosms.in/api/v1/dlr?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&message-id=" + item.MessageReturnId;
                        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                        HttpWebRequest request = this.GetRequest(url);
                        WebResponse webResponse = request.GetResponse();
                        var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                        var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                        //DelivaryCheck respList = new JavaScriptSerializer().Deserialize<DelivaryCheck>(responseText);
                        DelivaryCheck respList =JsonConvert.DeserializeObject<DelivaryCheck>(responseText);

                        var sms = _Entities.TbStaffSmshistories.Where(x => x.Id == item.Id).FirstOrDefault();
                        sms.DelivaryStatus = respList.dlr;
                        _Entities.SaveChanges();
                        status = true;
                        msg = "Successful";
                    }
                }
            }
            return Json(new { status = status });
        }

        public PartialViewResult GetAllSmsHistoryWithStatus(string id)
        {
            long headId = Convert.ToInt64(id);
            var model = new SMSHistoryModel();
            model.headId = headId;
            model.SenderType = 1;
            return PartialView("~/Views/School/_pv_StaffSMSHistory.cshtml", model);
        }
        public object CheckSMSDeliveryDataStudent(string id)
        {
            string msg = "Failed";
            bool status = false;
            long head = Convert.ToInt64(id);
            var data = new Satluj_Latest.Data.SmsHead(head).SmsHistory;
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.MessageReturnId != null)
                    {
                        var url = "http://alvosms.in/api/v1/dlr?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&message-id=" + item.MessageReturnId;
                        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                        HttpWebRequest request = this.GetRequest(url);
                        WebResponse webResponse = request.GetResponse();
                        var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                        var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                        //DelivaryCheck respList = new JavaScriptSerializer().Deserialize<DelivaryCheck>(responseText);
                        DelivaryCheck respList =
    JsonConvert.DeserializeObject<DelivaryCheck>(responseText);

                        var sms = _Entities.TbSmsHistories.Where(x => x.Id == item.Id).FirstOrDefault();
                        sms.DelivaryStatus = respList.dlr;
                        _Entities.SaveChanges();
                        status = true;
                        msg = "Successful";
                    }
                }
            }
            return Json(new { status = status });
        }
        public PartialViewResult GetAllSmsHistoryWithStatusStudent(string id)
        {
            long headId = Convert.ToInt64(id);
            var model = new SMSHistoryModel();
            model.headId = headId;
            model.SenderType = 0;
            return PartialView("~/Views/School/_pv_StudentSMSHistory.cshtml", model);
        }
        [HttpGet]
        public object SendBirthdayWishes()
        {
            string msg = "Successful";
            bool status = false;
            long schoolId = _user.SchoolId;
            try
            {
                var studentList = _Entities.TbStudents.Where(x => x.SchoolId == schoolId && x.Dob.Value.Month == CurrentTime.Month && x.Dob.Value.Day == CurrentTime.Day).ToList().Select(x => new Satluj_Latest.Data.Student(x)).ToList();
                if (studentList.Count > 0)
                {
                    var smsHead = new TbSmsHead();
                    smsHead.Head = "Todays Birthday Wishes from your School";
                    smsHead.SchoolId = schoolId;
                    smsHead.TimeStamp = CurrentTime;
                    smsHead.IsActive = true;
                    smsHead.SenderType = 0;//For student
                    _Entities.TbSmsHeads.Add(smsHead);
                    status = _Entities.SaveChanges() > 0;
                    foreach (var item in studentList)
                    {
                        long parentId = item.ParentId ?? 0;
                        var parent = new Parent(parentId);
                        var schoolData = new School(item.SchoolId);

                        #region SMS
                        SendBirthDayWishSMS(item, schoolData, smsHead.HeadId);
                        #endregion SMS

                        #region PUSH
                        if (item.ParentId != null && item.ParentId != 0)
                        {
                            SendBirthDayWishPush(item, schoolData);
                        }
                        #endregion PUSH

                        #region EMAIL
                        SendBirthDayWishEMAIL(item, schoolData, parent);
                        #endregion EMAIL
                    }
                    status = true;
                    msg = "Successful";
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { status = status });
        }
        private void SendBirthDayWishEMAIL(Satluj_Latest.Data.Student item, School school, Parent parent)
        {
            //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/template/BirthdayWishesEmail.html");
            var filePath = Path.Combine(
     _env.ContentRootPath,
     "Content",
     "template",
     "BirthdayWishesEmail.html"
 );

            var emailTemplate = System.IO.File.ReadAllText(filePath);
            //string Message = "Happy Birthday to you " + item.StundentName + " from " + school.SchoolName;
            string Message = "Dear " + item.StundentName + ",  May the joy that you have spread in the past come back to you on this day. Wishes you a very happy birthday! - " + _user.School.SchoolName;
            var mBody = emailTemplate.Replace("{{resetLink}}", Message).Replace("{{resetLink1}}", school.SchoolName);
            bool sendMail = Send("Birthday Wishes", mBody, item.ParentEmail);
        }

        private void SendBirthDayWishPush(Satluj_Latest.Data.Student item, School school)
        {
            try
            {
                var tokenData = _Entities.TbDeviceTokens.Where(x => x.UserId == item.ParentId && x.IsActive == true && x.LoginStatus == 1).OrderByDescending(x => x.TokenId).FirstOrDefault();
                if (tokenData != null)
                {
                    string message = "Happy Birthday to you " + item.StundentName + " from " + school.SchoolName;
                    var applicationID = "";
                    var senderId = "";
                    var pushData = _Entities.TbPushData.Where(x => x.SchoolId == item.SchoolId).FirstOrDefault();
                    if (pushData != null)
                    {
                        applicationID = pushData.LegacyNumber;
                        senderId = pushData.SenderId;
                    }
                    else
                    {
                        applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                        senderId = "47900857750";
                    }

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = tokenData.Token,
                        notification = new
                        {
                            body = message,
                            title = "Birthday Wishes"
                        },
                        priority = "high",
                        data = new
                        {
                            Role = "School",
                            Function = "Birthday"
                        },
                        from = "School"
                    };
                    //var serializer = new JavaScriptSerializer();
                    //var json = serializer.Serialize(data);
                    var json = JsonConvert.SerializeObject(data);

                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SendBirthDayWishSMS(Satluj_Latest.Data.Student student, School school, long headId)
        {



            var phone = student.ContactNumber.ToString();
            var senderName = "MYSCHO";
            //if (student.SchoolId == 10116)
            //{
            //    senderName = "PARDSE";
            //}
            //else if (student.SchoolId == 10117)
            //{
            //    senderName = "HOLYIN";
            //}
            var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
            if (senderData != null)
                senderName = senderData.SenderId;
            //string message = "Happy Birthday to you " + student.StundentName + " from " + school.SchoolName;
            //string message = "Dear, May the joy that you have spread in the past come back to you on this day. Wishes you a very happy birthday!";
            string message = "Dear " + student.StundentName + ",  May the joy that you have spread in the past come back to you on this day. Wishes you a very happy birthday!- " + _user.School.SchoolName;
            var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + message + "&sender=" + senderName;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest request = this.GetRequest(url);
            WebResponse webResponse = request.GetResponse();
            var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
            //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
            alvosmsResp respList =
    JsonConvert.DeserializeObject<alvosmsResp>(responseText);
            //--------------For History --------------------
            TbSmsHistory sms = new TbSmsHistory();
            sms.IsActive = true;
            sms.MessageContent = message;
            sms.MessageDate = CurrentTime;
            sms.ScholId = school.SchoolId;
            sms.StuentId = student.StudentId;
            sms.MobileNumber = phone;
            sms.HeadId = headId;
            sms.SendStatus = Convert.ToString(respList.success);
            if (respList.data != null)
            {
                sms.MessageReturnId = respList.data[0].messageId;
                sms.DelivaryStatus = "Pending";
            }
            sms.SmsSentPerStudent = 1;
            _Entities.TbSmsHistories.Add(sms);
            _Entities.SaveChanges();
            //--------------------------------------------------
        }
        private bool Send(string subject, string mailbody, string email)
        {
            ////try
            ////{
            ////    MailMessage msg = new MailMessage();
            ////    //SmtpClient client = new SmtpClient();
            ////    msg.Subject = subject;
            ////    msg.Body = mailbody;
            ////    msg.From = new MailAddress("info.schoolman@gmail.com");
            ////    msg.To.Add(new MailAddress(email));
            ////    msg.IsBodyHtml = true;

            ////    SmtpClient client = new SmtpClient();
            ////    //client.Host = "k2smtp.gmail.com";
            ////    client.Host = "k2smtpout.secureserver.net";
            ////    //System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("info.schoolman@gmail.com", "Info@123");
            ////    NetworkCredential basicauthenticationinfo = new NetworkCredential("info.schoolman@gmail.com", "Info@123");
            ////    client.Port = int.Parse("587");//25//465
            ////    client.EnableSsl = true;
            ////    client.UseDefaultCredentials = false;
            ////    client.Credentials = basicauthenticationinfo;
            ////    client.DeliveryMethod = SmtpDeliveryMethod.Network;
            ////    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
            ////            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            ////            System.Security.Cryptography.X509Certificates.X509Chain chain,
            ////            System.Net.Security.SslPolicyErrors sslPolicyErrors)
            ////    {
            ////        return true;
            ////    };
            ////    try
            ////    {
            ////        client.Send(msg);
            ////        client.Dispose();
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////    }

            ////}
            ////catch (Exception ex)
            ////{

            ////}
            try
            {
                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                msg.Body = mailbody;
                msg.From = new MailAddress("schoolman@srishtis.com");
                msg.To.Add(new MailAddress(email, "Dear"));
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                //client.Host = "k2smtpout.secureserver.net";
                client.Host = "smtpout.secureserver.net";
                System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("schoolman@srishtis.com", "ca@12345");
                //client.Port = int.Parse("25");
                client.Port = int.Parse("80");
                client.EnableSsl = false;
                client.UseDefaultCredentials = false;
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(msg);
            }
            catch (Exception ex)
            {

            }
            return true;
        }
        public object SendFeeDueAlert()
        {
            bool status = false;
            string msg = "Failed";
            long InsertHead = 0;
            long SchoolId = _user.SchoolId;
            try
            {
                var alarmDate = _Entities.TbFees.Where(x => x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
                if (alarmDate != null)
                {
                    //var mainData = _Entities.sp_FeeAlertDetails().ToList();
                  var mainData=  _Entities.sp_FeeAlertDetailsResult.FromSqlRaw("EXEC sp_FeeAlertDetails")
        .ToList();

                    var data = mainData.Where(x => x.SchoolId == SchoolId).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var students = data.Select(o => o.StudentId).Distinct().ToList();

                        var smsHead = new TbSmsHead();
                        smsHead.Head = "Fee Due Alert From Site";
                        smsHead.SchoolId = SchoolId;
                        smsHead.TimeStamp = CurrentTime;
                        smsHead.IsActive = true;
                        smsHead.SenderType = 0;//For Student
                        _Entities.TbSmsHeads.Add(smsHead);
                        status = _Entities.SaveChanges() > 0;
                        var schoolAlertDate = _Entities.TbFeeAlertData.Where(x => x.SchoolId == SchoolId && x.IsActive).FirstOrDefault();
                        schoolAlertDate.IsActive = false;
                        _Entities.SaveChanges();
                        InsertHead = smsHead.HeadId;
                        foreach (var item in students)
                        {
                            var isNeedStatus = false;// If the student have bill amount, but it's less than the advance amount , then there is no need to send the SMS
                            var Amount = data.Where(z => z.StudentId == item).Sum(z => z.Amount);
                            if (Amount != null && Amount != 0)
                            {
                                var student = data.Where(x => x.StudentId == item).FirstOrDefault();
                                var advance = _Entities.TbStudentBalances.Where(z => z.IsActive == true && z.StudentId == student.StudentId).FirstOrDefault();
                                if (advance != null)
                                {
                                    if (advance.Amount > Amount)
                                    {
                                        isNeedStatus = true;
                                    }
                                    else
                                    {
                                        Amount = (int)(Amount - advance.Amount);
                                    }
                                }
                                if (isNeedStatus == false)
                                {
                                    if (student.ContactNumber != null && student.ContactNumber != string.Empty)
                                    {
                                        #region SMS
                                        SendFeeAlertSMS(Amount, student, InsertHead);
                                        #endregion

                                        var studentData = new Satluj_Latest.Data.Student(student.StudentId);
                                        #region Push
                                        if (studentData.ParentId != null)
                                        {
                                            SendFeeAlertPush(Amount, studentData);
                                        }
                                        #endregion

                                        #region Email
                                        try
                                        {
                                            SendFeeAlertMail(Amount, studentData.ParentEmail, studentData.SchoolName, student.StundentName);
                                        }
                                        catch { }
                                        #endregion
                                        msg = "Successful";
                                        status = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        msg = "No dues";
                    }
                }
                else
                {
                    msg = "Please save a date for due start !";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
        private void SendFeeAlertSMS(decimal? Amount, sp_FeeAlertDetails student, long InsertHead)
        {
            var dueAmount = Math.Round((double)Amount, 2);
            var school = new Satluj_Latest.Data.School(student.SchoolId);
            var phone = student.ContactNumber.ToString();
            var senderName = "MYSCHO";
            //if (student.SchoolId == 10116)
            //{
            //    senderName = "PARDSE";
            //}
            //else if (student.SchoolId == 10117)
            //{
            //    senderName = "HOLYIN";
            //}
            var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
            if (senderData != null)
                senderName = senderData.SenderId;
            //string message = "Your kid " + student.StundentName + " have RS : " + dueAmount + " /- fee due from " + school.SchoolName + " School";
            string message = "Dear parent of " + student.StundentName + " , Gentle reminder that there is a fees due of Rs." + dueAmount + " /- .Kindly remit the fee at the earliest.";

            var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + message + "&sender=" + senderName;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest request = this.GetRequest(url);
            WebResponse webResponse = request.GetResponse();
            var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
            //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
            alvosmsResp respList =JsonConvert.DeserializeObject<alvosmsResp>(responseText);
            TbSmsHistory sms = new TbSmsHistory();
            sms.IsActive = true;
            sms.MessageContent = message;
            sms.MessageDate = CurrentTime;
            sms.ScholId = student.SchoolId;
            sms.StuentId = student.StudentId;
            sms.MobileNumber = phone;
            sms.HeadId = InsertHead;
            sms.SendStatus = Convert.ToString(respList.success);
            sms.SmsSentPerStudent = 1;
            if (respList.data != null)
            {
                sms.MessageReturnId = respList.data[0].messageId;
                sms.DelivaryStatus = "Pending";
            }
            _Entities.TbSmsHistories.Add(sms);
            _Entities.SaveChanges();
        }
        private void SendFeeAlertPush(decimal? Amount, Satluj_Latest.Data.Student student)
        {
            try
            {
                var tokenData = _Entities.TbDeviceTokens.Where(x => x.UserId == student.ParentId && x.IsActive == true && x.LoginStatus == 1).OrderBy(x => x.TokenId).FirstOrDefault();
                if (tokenData != null)
                {
                    var dueAmount = Math.Round((double)Amount, 2);
                    //string message = "Happy Birthday to you " + student.StundentName + " from " + student.SchoolName;
                    string message = "Your kid " + student.StundentName + " have RS : " + dueAmount + " /- fee due from " + student.SchoolName + " School";
                    var applicationID = "";
                    var senderId = "";
                    var pushData = _Entities.TbPushData.Where(x => x.SchoolId == student.SchoolId).FirstOrDefault();
                    if (pushData != null)
                    {
                        applicationID = pushData.LegacyNumber;
                        senderId = pushData.SenderId;
                    }
                    else
                    {
                        applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                        senderId = "47900857750";
                    }

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = tokenData.Token,
                        notification = new
                        {
                            body = message,
                            title = "Birthday Wishes"
                        },
                        priority = "high",
                        data = new
                        {
                            Role = "School",
                            Function = "Birthday"
                        },
                        from = "School"
                    };
                    //var serializer = new JavaScriptSerializer();
                    //var json = serializer.Serialize(data);
                    var json = JsonConvert.SerializeObject(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private bool SendFeeAlertMail(decimal? amount, string emailId, string schoolName, string StundentName)
        {
            try
            {
                var dueAmount = Math.Round((double)amount, 2);
                //var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/template/BirthdayWishesEmail.html");
                var filePath = Path.Combine(
    _env.WebRootPath,
    "Content",
    "template",
    "BirthdayWishesEmail.html"
);

                var emailTemplate = System.IO.File.ReadAllText(filePath);
                //string Message = "Your kid " + StundentName + " have RS : " + dueAmount + " /- fee due from " + schoolName + " School";
                string Message = "Dear parent of " + StundentName + " , Gentle reminder that there is a fees due of Rs." + dueAmount + " /- .Kindly remit the fee at the earliest.";
                var mBody = emailTemplate.Replace("{{resetLink}}", Message).Replace("{{resetLink1}}", schoolName);
                bool sendMail = Send("Fee due alert", mBody, emailId);
            }
            catch (Exception ex) { }
            return true;
        }
        public object SendCircularNotificationMessages()
        {
            bool status = false;
            string msg = "Faile";
            bool sendData = false;
            long schoolId = _user.SchoolId;
            try
            {
                //var notificationMain = _Entities.SP_CircularNotification(CurrentTime).ToList();
              var notificationMain= _Entities.SP_CircularNotification
        .FromSqlRaw("EXEC SP_CircularNotification {0}",
                    CurrentTime)
        .ToList();

                var notification = notificationMain.Where(x => x.SchoolId == schoolId).ToList();
                if (notification.Count > 0)
                {
                    foreach (var item2 in notification)
                    {
                        var student = _Entities.TbStudents.Where(x => x.IsActive && x.ParentId != null && x.SchoolId == item2.SchoolId).ToList();
                        if (student.Count > 0)
                        {
                            foreach (var item in student)
                            {
                                string school = item.School.SchoolName;
                                string kidName = item.StundentName;
                                long parentId = item.ParentId ?? 0;
                                SendMailsForCircularNotification(item, item2, school, kidName);
                                if (item.ParentId != null)
                                {
                                    var parentDetails = _Entities.TbDeviceTokens.Where(x => x.UserId == item.ParentId && x.LoginStatus == 1).ToList().GroupBy(x => new { x.UserId, x.TokenId }).Select(x => x.FirstOrDefault()).ToList();
                                    foreach (var data in parentDetails)
                                    {
                                        long studentUserId = Convert.ToInt64(item.StudentId);
                                        var message = "Your Kid's  School have an event " + item2.Description + " on " + item2.CircularDate.ToShortDateString();
                                        circularPushandroid(data.Token, message, school, studentUserId, item.SchoolId, item2);
                                    }
                                    status = true;
                                    msg = "Successful";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { status = status });
        }
        private bool circularPushandroid(string deviceId, string message, string tittle, long kidId, long schoolId, SP_CircularNotification_Result note)
        {
            try
            {
                var applicationID = "";
                var senderId = "";
                var pushData = _Entities.TbPushData.Where(x => x.SchoolId == schoolId).FirstOrDefault();
                if (pushData != null)
                {
                    applicationID = pushData.LegacyNumber;
                    senderId = pushData.SenderId;
                }
                else
                {
                    applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                    senderId = "47900857750";
                }

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var dataMain = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message,
                        title = tittle,
                        sound = "Enabled"
                    },
                    priority = "high",
                    data = new
                    {
                        KidId = kidId,
                        SchoolId = schoolId,
                        Date = note.CircularDate,
                        Description = note.Description,
                        Filepath = note.FilePath,
                        Role = "Teacher",
                        Function = "Events"
                    },
                    from = "Teacher",
                    Type = "Events"
                };
                //var serializer = new JavaScriptSerializer();
                //var json = serializer.Serialize(dataMain);
                var json = JsonConvert.SerializeObject(dataMain);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //Response.Write(sResponseFromServer);
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }
            return true;
        }
        private bool SendMailsForCircularNotification(TbStudent data, SP_CircularNotification_Result eventData, string school, string kidName)
        {
            var filePath = Path.Combine(
                _env.WebRootPath,
                "Content",
                "template",
                "CircularNotification.html"
            );
            var emailTemplate = System.IO.File.ReadAllText(filePath);
            string Message = "Your kid " + kidName + "'s School have an event ";
            string message2 = eventData.Description + " on " + eventData.CircularDate.ToShortDateString();
            string filePathDoc = "http://sutluj.schoolman.in" + eventData.FilePath;
            string downLoad = "Download";
            if (eventData.FilePath == null || eventData.FilePath == "")
            {
                downLoad = "No files for Download";
            }
            var mBody = emailTemplate.Replace("{{resetLink}}", message2).Replace("{{resetLink1}}", school).Replace("{{resetLink2}}", Message).Replace("{{resetLink3}}", filePathDoc).Replace("{{resetLink4}}", downLoad);
            bool sendMail = Send("Events", mBody, data.ParentEmail);
            return sendMail;
        }
        [HttpPost]
        public IActionResult DiaryUploadPDFEdit(IFormFile file)
        {
            bool status = false;
            string msg = "failed";
            string fileSave = "";

            var isDiary = _Entities.TbFiles
                .Where(z => z.SchoolId == _user.SchoolId
                            && z.IsActive
                            && z.FileModule == 1
                            && z.FileType == 3)
                .FirstOrDefault();

            if (isDiary != null)
            {
                // Delete old file if exists
                string oldFilePath = Path.Combine(
                    _env.WebRootPath,
                    isDiary.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                FileInfo fileInfo = new FileInfo(oldFilePath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                if (file != null && file.Length > 0)
                {
                    // Upload folder
                    string folderPath = Path.Combine(_env.WebRootPath, "Media", "School", "Diary");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Generate new PDF name
                    string pdfName = $"{_user.School.SchoolName}{Guid.NewGuid()}.pdf";
                    string pdfFilePath = Path.Combine(folderPath, pdfName);

                    // Save uploaded file
                    using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    fileSave = $"/Media/School/Diary/{pdfName}";
                    msg = "Success";
                    status = true;

                    try
                    {
                        // Update DB
                        isDiary.FilePath = fileSave;
                        isDiary.TimeStamp = CurrentTime;
                        status = _Entities.SaveChanges() > 0;
                        msg = "Success";
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                }
            }

            return Json(new { status, msg, url = fileSave });
        }


        [HttpPost]
        public IActionResult LogoUploadEdit(IFormFile documentFile)
        {
            if (documentFile == null)
                return Json(new { status = false, msg = "FILE IS NULL" });

            var school = _Entities.TbSchools
                .FirstOrDefault(z => z.SchoolId == _user.SchoolId && z.IsActive);

            if (school == null)
                return Json(new { status = false, msg = "School Not Found" });

            // Delete old
            if (!string.IsNullOrEmpty(school.FilePath))
            {
                string oldPath = Path.Combine(_env.WebRootPath, school.FilePath.TrimStart('/'));

                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            // Create new folder
            string folder = Path.Combine(_env.WebRootPath, "Media/School/Logo");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Save new file
            string fileName = $"{Guid.NewGuid()}.jpeg";
            string savePath = Path.Combine(folder, fileName);

            using (var fs = new FileStream(savePath, FileMode.Create))
                documentFile.CopyTo(fs);

            string dbPath = "/Media/School/Logo/" + fileName;

            school.FilePath = dbPath;
            school.TimeStamp = DateTime.Now;

            _Entities.SaveChanges();

            return Json(new { status = true, url = dbPath });
        }

        [HttpPost]
        public IActionResult LogoUpload(IFormFile documentFile)
        {
            if (documentFile == null)
                return Json(new { status = false, msg = "FILE IS NULL" });

            var school = _Entities.TbSchools
                .FirstOrDefault(z => z.SchoolId == _user.SchoolId && z.IsActive);

            if (school == null)
                return Json(new { status = false, msg = "School Not Found" });

            string folder = Path.Combine(_env.WebRootPath, "Media/School/Logo");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = $"{Guid.NewGuid()}.jpeg";
            string savePath = Path.Combine(folder, fileName);

            using (var fs = new FileStream(savePath, FileMode.Create))
                documentFile.CopyTo(fs);

            string dbPath = "/Media/School/Logo/" + fileName;

            school.FilePath = dbPath;
            school.TimeStamp = DateTime.Now;
            _Entities.SaveChanges();

            return Json(new { status = true, url = dbPath });
        }


        #region Lab
        public ActionResult LaboratoryCategory()
        {
            LaboratoryInventoryModels model = new LaboratoryInventoryModels();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public PartialViewResult EditLaboratoryCategoryView(string id)
        {
            LaboratoryInventoryModels model = new LaboratoryInventoryModels();
            long catId = Convert.ToInt64(id);
            var category = _Entities.TbLaboratoryCategories.Where(z => z.CategoryId == catId).FirstOrDefault();
            model.schoolId = _user.SchoolId;
            model.categoryId = category.CategoryId;
            model.laboratoryName = category.LaboratoryName;
            return PartialView("~/Views/School/_pv_LaboratoryCategory_Edit.cshtml", model);
        }


        public PartialViewResult AddLaboratoryCategoryView()
        {
            LaboratoryInventoryModels model = new LaboratoryInventoryModels();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_LaboratoryCategory_Add.cshtml", model);
        }
        [HttpPost]
        public object AddLaboratoryCategory(LaboratoryInventoryModels model)
        {
            bool status = false;
            string message = "Failed";
            var category = new TbLaboratoryCategory();
            category.LaboratoryName = model.laboratoryName;
            category.SchoolId = model.schoolId;
            category.IsActive = true;
            _Entities.TbLaboratoryCategories.Add(category);
            status = _Entities.SaveChanges() > 0;
            message = status ? " Category Added" : "Failed";
            return Json(new { status = status, msg = message });
        }
        [HttpPost]
        public object EditLaboratoryCategory(LaboratoryInventoryModels model)
        {
            bool status = false;
            string message = "Failed";
            var category = _Entities.TbLaboratoryCategories.Where(z => z.CategoryId == model.categoryId).FirstOrDefault();
            category.LaboratoryName = model.laboratoryName;
            status = _Entities.SaveChanges() > 0;
            message = status ? " Category Edited" : "Failed";
            return Json(new { status = status, msg = message });
        }
        public object DeleteLaboratoryCategory(string id)
        {
            bool status = false;
            string message = "Failed";
            long catId = Convert.ToInt64(id);
            var category = _Entities.TbLaboratoryCategories.Where(z => z.CategoryId == catId).FirstOrDefault();

            if (_Entities.TbLaboratoryCategories.Any(x => x.CategoryId == catId))
            {
                category.IsActive = false;
                status = _Entities.SaveChanges() > 0;
                message = status ? " Category deleted successfully" : "Failed to delete Category";
            }
            else
            {
                _Entities.TbLaboratoryCategories.Remove(category);
                status = _Entities.SaveChanges() > 0 ? true : false;
                message = status ? "Category deleted successfully!" : "Failed to delete Category!";
            }

            return Json(new { status = status, msg = message });
        }
        public PartialViewResult LaboratoryCategoryListPartial()
        {
            LaboratoryInventoryModels model = new LaboratoryInventoryModels();
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_LaboratoryCategory_list.cshtml", model);

        }
        #endregion



        #region Forms
        public ActionResult DOCertificate(string id)
        {
            long studentId = Convert.ToInt64(id);
            var student = new Satluj_Latest.Data.Student(studentId);
            StudentModel model = new StudentModel();
            model.studentName = student.StundentName;
            model.studentId = student.StudentId;
            model.classId = student.ClassId;
            model.className = student.ClassName + " / " + student.DivisionName;
            model.division = student.DivisionName;
            model.divisionId = student.DivisionId;
            model.parentName = student.ParentName;
            if (student.DOB != null && student.DOB.Value.Year != 01)
            {
                model.DOBstring = Convert.ToString(Convert.ToDateTime(student.DOB).ToString("dd/MM/yyyy")) ?? "";
            }
            else
            {
                model.DOBstring = " ";
            }
            model.CurrentDate = CurrentTime.ToString("dd/MM/yyyy");
            if (CurrentTime.Month < 6)
            {
                model.AcademinYear = CurrentTime.AddYears(-1).Year + " - " + CurrentTime.Year;
            }
            else
            {
                model.AcademinYear = CurrentTime.Year + " - " + CurrentTime.AddYears(-1).Year;
            }
            return View(model);
        }
        public ActionResult AadharCardForm(string id)
        {
            long studentId = Convert.ToInt64(id);
            var student = new Satluj_Latest.Data.Student(studentId);
            StudentModel model = new StudentModel();
            model.studentName = student.StundentName;
            model.studentId = student.StudentId;
            model.classId = student.ClassId;
            model.className = student.ClassName + " / " + student.DivisionName;
            model.division = student.DivisionName;
            model.divisionId = student.DivisionId;
            model.parentName = student.ParentName;
            if (student.DOB != null && student.DOB.Value.Year != 01)
            {
                model.DOBstring = Convert.ToString(Convert.ToDateTime(student.DOB).ToString("dd/MM/yyyy")) ?? "";
            }
            else
            {
                model.DOBstring = " ";
            }
            model.CurrentDate = CurrentTime.ToString("dd/MM/yyyy");
            if (CurrentTime.Month < 6)
            {
                model.AcademinYear = CurrentTime.AddYears(-1).Year + " - " + CurrentTime.Year;
            }
            else
            {
                model.AcademinYear = CurrentTime.Year + " - " + CurrentTime.AddYears(-1).Year;
            }
            return View(model);
        }
        public ActionResult FeeRemittance(string id)
        {
            long studentId = Convert.ToInt64(id);
            var student = new Satluj_Latest.Data.Student(studentId);
            StudentModel model = new StudentModel();
            model.studentName = student.StundentName;
            model.studentId = student.StudentId;
            model.classId = student.ClassId;
            model.className = student.ClassName + " / " + student.DivisionName;
            model.division = student.DivisionName;
            model.divisionId = student.DivisionId;
            model.parentName = student.ParentName;
            if (student.DOB != null && student.DOB.Value.Year != 01)
            {
                model.DOBstring = Convert.ToString(Convert.ToDateTime(student.DOB).ToString("dd/MM/yyyy")) ?? "";
            }

            model.CurrentDate = CurrentTime.ToString("dd/MM/yyyy");
            if (CurrentTime.Month < 6)
            {
                model.AcademinYear = CurrentTime.AddYears(-1).Year + " - " + CurrentTime.Year;
            }
            else
            {
                model.AcademinYear = CurrentTime.Year + " - " + CurrentTime.AddYears(-1).Year;
            }
            return View(model);
        }
        #endregion Forms

        private DateTime ConvertDateToServer(string stringDate)
        {
            string[] splitData = stringDate.Split('-');
            var dd = splitData[0];
            var mm = splitData[1];
            var yyyy = splitData[2];
            var retDate = mm + '-' + dd + '-' + yyyy;
            return Convert.ToDateTime(retDate);
        }

        private bool onlyCircularPushandroid(string deviceId, string message, string tittle, long kidId, long schoolId, TbCircular note)
        {
            try
            {
                var applicationID = "";
                var senderId = "";
                var pushData = _Entities.TbPushData.Where(x => x.SchoolId == schoolId).FirstOrDefault();
                if (pushData != null)
                {
                    applicationID = pushData.LegacyNumber;
                    senderId = pushData.SenderId;
                }
                else
                {
                    applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                    senderId = "47900857750";
                }

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var dataMain = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message,
                        title = tittle,
                        sound = "Enabled"
                    },
                    priority = "high",
                    data = new
                    {
                        KidId = kidId,
                        SchoolId = schoolId,
                        Date = note.CircularDate,
                        Description = note.Description,
                        Filepath = note.FilePath,
                        Role = "Teacher",
                        Function = "Circular"
                    },
                    from = "Teacher",
                    Type = "Circular"
                };
                //var serializer = new JavaScriptSerializer();
                //var json = serializer.Serialize(dataMain);
                var json = JsonConvert.SerializeObject(dataMain);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //Response.Write(sResponseFromServer);
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }
            return true;
        }
        public object SendAppDetails()
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = _user.SchoolId;
            var appdet = _Entities.TbPushData.Where(x => x.SchoolId == schoolId).Select(x => x.PlayStore).FirstOrDefault();
            if (appdet != null)
            {
                var studentsList = _Entities.TbStudents.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (studentsList.Count > 0)
                {
                    status = true;
                    msg = "Successful";
                    Thread Thread = new Thread(() => SendAppDetailsSMS(studentsList, appdet));
                    Thread.Start();
                }
            }
            else
            {
                status = false;
                msg = "Sorry , you don't have an app !";
            }
            return Json(new { status = status, msg = msg });
        }

        private void SendAppDetailsSMS(List<TbStudent> studentsList, string appdet)
        {
            try
            {
                var smsHead = new TbSmsHead();
                smsHead.Head = "From School APP Details";
                smsHead.SchoolId = _user.SchoolId;
                smsHead.TimeStamp = CurrentTime;
                smsHead.IsActive = true;
                smsHead.SenderType = 0;
                _Entities.TbSmsHeads.Add(smsHead);
                _Entities.SaveChanges();
                var senderName = "MYSCHO";
                //if (_user.SchoolId == 10116)
                //{
                //    senderName = "PARDSE";
                //}
                //else if (_user.SchoolId == 10117)
                //{
                //    senderName = "HOLYIN";
                //}
                var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                if (senderData != null)
                    senderName = senderData.SenderId;
                foreach (var item in studentsList)
                {
                    string message = "Dear  parent of " + item.StundentName + " ( Admission No : " + item.StudentSpecialId + " ), Kindly follow the below link to install the school APP,  to get in touch with your child’s school and to know about the attendance and marks. " + appdet;
                    var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + item.ContactNumber + "&route=2&message=" + message + "&sender=" + senderName;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    HttpWebRequest request = this.GetRequest(url);
                    WebResponse webResponse = request.GetResponse();
                    var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                    //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
                    alvosmsResp respList =JsonConvert.DeserializeObject<alvosmsResp>(responseText);
                    TbSmsHistory sms = new TbSmsHistory();
                    sms.IsActive = true;
                    sms.MessageContent = message;
                    sms.MessageDate = CurrentTime;
                    sms.ScholId = item.SchoolId;
                    sms.StuentId = item.StudentId;
                    sms.MobileNumber = item.ContactNumber;
                    sms.HeadId = smsHead.HeadId;
                    sms.SendStatus = Convert.ToString(respList.success);
                    sms.SmsSentPerStudent = 1;
                    if (respList.data != null)
                    {
                        sms.MessageReturnId = respList.data[0].messageId;
                        sms.DelivaryStatus = "Pending";
                    }
                    _Entities.TbSmsHistories.Add(sms);
                    _Entities.SaveChanges();

                }
            }
            catch
            {

            }
        }
        public PartialViewResult UpcomingEventView(string id)
        {
            long eventId = Convert.ToInt64(id);
            var data = _Entities.TbCalenderEvents.Where(x => x.EventId == eventId).FirstOrDefault();
            CalendarEventModels model = new CalendarEventModels();
            model.eventId = data.EventId;
            model.schoolId = _user.SchoolId;
            model.eventHead = data.EventHead;
            model.eventDetails = data.EventDetails;
            model.eventDate = data.EventDate.ToShortDateString();
            return PartialView("~/Views/School/_pv_UpcomingEventView.cshtml", model);
        }
        public object CheckAdmissionNoEdit(string text)
        {
            bool Status = false;
            string Message = "Failed";
            string[] splitData = text.Split('~');
            try
            {
                long studentId = Convert.ToInt64(splitData[0]);
                string specialId = splitData[1];
                if (_Entities.TbStudents.Where(x => x.StudentId != studentId).Any(x => x.StudentSpecialId.ToLower() == specialId.ToLower() && x.IsActive))
                {
                    Status = true;
                    Message = "Admission Number already in use";
                }
            }
            catch
            {

            }
            return Json(new { Status = Status, Message = Message });
        }

        [HttpPost]
        public object AddSMSSenderIdData(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var date = new TbSchoolSenderId();
                date.SchoolId = _user.SchoolId;
                date.SenderId = model.SenderDetails.SenderData;
                date.IsActive = true;
                _Entities.TbSchoolSenderIds.Add(date);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }

        [HttpPost]
        public object EditSMSSenderIdData(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var date = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                date.SenderId = model.SenderDetails.SenderData;
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult LoadTableForBillingNew(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            string[] splitData = id.Split('~');
            model.SchoolModel.studentId = Convert.ToInt64(splitData[1]);
            model.BillNumber = Convert.ToInt64(splitData[0]);
            return PartialView("~/Views/School/_pv_CollectionDetaildView.cshtml", model);
        }
        public ActionResult AttendanceDivisionApp()
        {
            var model = new SchoolModelForId();
            model.SchoolId = _user.SchoolId;

            var schoolDataModel = new SchoolClassListPostModel();
            schoolDataModel.schoolId = _user.SchoolId.ToString();

            var dataResult = _schoolRepository.GetAllStaffData(schoolDataModel);

            ViewBag.DataResult = dataResult;

            return View(model);
        }


        [HttpPost]
        public object AttendanceDivisionApp(SchoolModel model)
        {
            bool status = false;
            string message = "Failed";
            long schoolId = _user.SchoolId;
            List<string> DivisionStringId = model.DivisionStringId.Split(',').ToList();

            var biometricDivision = new TbBiometricDivision();
            //_Entities.Database.ExecuteSqlCommand("Delete from [dbo].[tb_BiometricDivision] Where SchoolId = " + schoolId);
            _Entities.Database.ExecuteSqlRaw(
    "DELETE FROM [dbo].[tb_BiometricDivision] WHERE SchoolId = {0}",
    schoolId
);
            foreach (var divisionId in DivisionStringId)
            {
                long DivisionId = Convert.ToInt32(divisionId);
                biometricDivision.DivisionId = DivisionId;
                biometricDivision.SchoolId = schoolId;
                _Entities.TbBiometricDivisions.Add(biometricDivision);
                status = _Entities.SaveChanges() > 0;
                message = status ? " Updated" : "Failed to update";

            }
            return Json(new { status = status, msg = message });
        }
        [HttpPost]
        public object AddFeeAccountHeadData(FeeAlertDataModel model) //***
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var date = new TbAccountHead();
                date.SchoolId = _user.SchoolId;
                date.AccHeadName = model.FeeIncomeHead.AccountHead;
                date.IsActive = true;
                date.TimeStamp = CurrentTime;
                date.ForBill = true;
                _Entities.TbAccountHeads.Add(date);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }
        [HttpPost]
        public object EditFeeAccountHeadData(FeeAlertDataModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var date = _Entities.TbAccountHeads.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.ForBill == true).FirstOrDefault();
                date.AccHeadName = model.FeeIncomeHead.AccountHead;
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult LoadReturnPaymentMode(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            string[] splitData = id.Split('~');
            model.SchoolModel.studentId = Convert.ToInt64(splitData[1]);
            model.BillNumber = Convert.ToInt64(splitData[0]);
            model.SchoolId = _user.SchoolId;
            model.SchoolModel.curredntDateTime = CurrentTime;
            model.ChequeDate = CurrentTime.ToShortDateString();
            return PartialView("~/Views/School/_pv_ReturnPaymnetModeView.cshtml", model);
        }
        public PartialViewResult LoadBankEntrySearchView()
        {
            BankEntryModel model = new BankEntryModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/SchoolAccount/_pv_BankEntrySearch.cshtml", model);
        }

        //basheer on 25/01/2019
        public ActionResult ChangePassword()
        {
            PasswordChangeModel model = new PasswordChangeModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object PasswordChange(PasswordChangeModel model)
        {
            string msg = "Failed";
            bool status = false;
            var checkpass = _Entities.TbLogins.Where(x => x.SchoolId == model.SchoolId && x.Password == model.OldPassword && x.UserId==_user.UserId).FirstOrDefault();
            if (checkpass != null)
            {
                try
                {
                    checkpass.Password = model.ConfirmPassword.Trim();
                    _Entities.SaveChanges();
                    status = true;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
                if (status == true)
                {
                    msg = "Password Changed Successfully !";
                    Thread Thread = new Thread(() => SendPasswordChangePush(model));
                    Thread.Start();
                }
                else
                    msg = "Failed to Change Password !";
            }
            else
            {
                msg = "Incorrect old password";
            }

            return Json(new { status = status, msg = msg });
        }
        //Basheer on 29/01/2019 
        private void SendPasswordChangePush(PasswordChangeModel model)
        {
            try
            {
                string schoolName = "Message from " + _user.School.SchoolName;
                var tokenData = _Entities.TbDeviceTokens.Where(x => x.UserId == model.SchoolId && x.IsActive == true && x.LoginStatus == 1).OrderBy(x => x.TokenId).FirstOrDefault();
                if (tokenData != null)
                {
                    var applicationID = "";
                    var senderId = "";
                    var pushData = _Entities.TbPushData.Where(x => x.SchoolId == model.SchoolId).FirstOrDefault();
                    if (pushData != null)
                    {
                        applicationID = pushData.LegacyNumber;
                        senderId = pushData.SenderId;
                    }
                    else
                    {
                        applicationID = "AIzaSyAGcW_XdoA-bwVtUQ4IcnncTM2Toso3sv4";
                        senderId = "47900857750";
                    }

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = tokenData.Token,
                        notification = new
                        {
                            body = "Password is changed.Please Login again",
                            title = schoolName
                        },
                        priority = "high",
                        data = new
                        {
                            Role = "School",
                            Function = "PasswordChange"
                        },
                        from = "School"
                    };
                    //var serializer = new JavaScriptSerializer();
                    //var json = serializer.Serialize(data);
                    var json = JsonConvert.SerializeObject(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public ActionResult AddNewStudentsFullDetails()
        {
            StudentModel model = new StudentModel();
            model.schoolId = _user.SchoolId;
            model.state = _user.School.State;
            model.city = _user.School.City;
            ViewBag.Classlist = _dropdown.GetClasses(model.schoolId);
            return View(model);
        }

        [HttpPost]
        public IActionResult AddNewStudent(StudentModel model)
        {
            bool status = false;
            string msg = string.Empty;
            long StudentId = 0;

            var stud = new TbStudent
            {
                SchoolId = _user.SchoolId,
                StudentSpecialId = model.admissionNo,
                StundentName = model.studentName,
                ParentName = " ",
                Address = model.address,
                City = model.city,
                ContactNumber = model.contactNo,
                ClasssNumber = model.RollNo1,
                ClassId = model.classId,
                DivisionId = model.divisionId,
                BusId = model.BusId == null ? 1 : Convert.ToInt64(model.BusId),
                TripNo = model.tripNumber,
                MotherName = string.IsNullOrEmpty(model.MotherName) ? "  " : model.MotherName,
                TimeStamp = CurrentTime,
                StudentGuid = Guid.NewGuid(),
                IsActive = true,
                State = model.state,
                Gender = model.gender,
                BloodGroup = model.bloodGroup,
                ParentEmail = string.IsNullOrEmpty(model.parentEmail) ? " " : model.parentEmail,
                PostalCode = model.Pincode,
                Aadhaar = " xxx  ",
                BioNumber = string.IsNullOrEmpty(model.biometricId) ? "  xxxx   " : model.biometricId,
                MobileNo = string.IsNullOrEmpty(model.MobileNo) ? " " : model.MobileNo,
                PlaceOfBirth = model.PlaceOfBirth,
                MotherTongue = model.MotherTongue,
                NationalityId = Convert.ToInt32(model.NationalityId),
                CountryId = Convert.ToInt32(model.CountryId),
                CategoryId = Convert.ToInt32(model.CategoryId)
            };

            // Parse DOB
            if (!string.IsNullOrEmpty(model.DOBstring))
            {
                try
                {
                    var splitData = model.DOBstring.Split('-');
                    var dob = $"{splitData[1]}-{splitData[0]}-{splitData[2]}";
                    stud.Dob = Convert.ToDateTime(dob);
                }
                catch { }
            }

            // Parse DateOfJoining
            if (!string.IsNullOrEmpty(model.DateOfJoiningString))
            {
                try
                {
                    var splitData = model.DateOfJoiningString.Split('-');
                    var doj = $"{splitData[1]}-{splitData[0]}-{splitData[2]}";
                    stud.DateOfJoining = Convert.ToDateTime(doj);
                }
                catch { }
            }

            // Save profile picture if present
            if (!string.IsNullOrEmpty(model.profilePic))
            {
                string folderPath = Path.Combine(_env.WebRootPath, "Media", "Student", "Profile");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var imageString = model.profilePic.Substring(model.profilePic.IndexOf(',') + 1);
                byte[] imageBytes = Convert.FromBase64String(imageString);

                string imageName = $"{Guid.NewGuid()}.jpeg";
                string imgFilePath = Path.Combine(folderPath, imageName);
                string fileSave = $"/Media/Student/Profile/{imageName}";

                using (var imageFile = new FileStream(imgFilePath, FileMode.Create))
                {
                    imageFile.Write(imageBytes, 0, imageBytes.Length);
                    imageFile.Flush();
                }

                stud.FilePath = fileSave;
            }

            try
            {
                _Entities.TbStudents.Add(stud);
                status = _Entities.SaveChanges() > 0;
                StudentId = stud.StudentId;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            string division = model.divisionId.ToString();
            string classId = model.classId.ToString();
            msg = status ? "Student added successfully, Now you can add the Parents Details !" : "Failed to add Student!";

            return Json(new { status, msg, division, classId, studentId = StudentId });
        }

        public PartialViewResult StudentsParentsInfoPage(string id)
        {
            string[] splitData = id.Split('~');
            StudentModel model = new StudentModel();
            model.studentId = Convert.ToInt64(splitData[0]);
            model.classId = Convert.ToInt64(splitData[1]);
            model.divisionId = Convert.ToInt64(splitData[2]);
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_AddStudentPerentsInfo.cshtml", model);
        }
        public object AddNewStudentParentInfo(StudentModel model)
        {
            string msg = "Failed";
            bool status = false;
            var student = _Entities.TbStudents.Where(x => x.StudentId == model.studentId && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            #region Updates
            if (student.ParentId != null)
            {
                var parent = _Entities.TbParents.Where(x => x.ParentId == student.ParentId).FirstOrDefault();
                parent.ParentName = model.FatherName;
                //parent.Address = model.FatherAddress;
                //parent.City = model.FatherCity;
                parent.Email = model.FatherEmail;
                parent.ContactNumber = model.FatherContact;
                //parent.Password = model.Password;
                parent.TimeStamp = CurrentTime;
                //parent.ParentGuid = new Guid();
                parent.IsActive = true;
                //parent.State = model.FatherState;
                //parent.PostalCode = model.FatherPincode;
                parent.FatherEducation = model.FatherEducation;
                parent.FatherOccupation = model.FatherOccupation;
                parent.FatherCountryId = Convert.ToInt32(model.FatherCountryId);
                parent.MotherName = model.MotherName;
                parent.MotherEducation = model.MotherEducaton;
                parent.MotherOccupation = model.MotherOccupation;
                parent.MotherContactNo = model.MotherContact;
                parent.MotherEmail = model.MotherEmail;
                //parent.MotherAddress = model.MotherAddress;
                //parent.MotherCountryId = Convert.ToInt32(model.MotherCountryId);
                //parent.MotherState = model.MotherState;
                //parent.MotherCity = model.MotherCity;
                //parent.MotherPincode = model.MotherPincode;
                parent.GuardianName = model.GuardianName;
                parent.GuardianEducation = model.GuardianEduation;
                parent.GuardianOccupation = model.GuardianOccupation;
                parent.GuardianContactNo = model.GuardianContact;
                parent.GuardianEmail = model.GuardianEmail;
                //parent.GuardianAddress = model.GuardianAddress;
                //parent.GuardianCountryId = Convert.ToInt32(model.GuardianCountryId);
                //parent.GuardianState = model.GuardianState;
                //parent.GuardianCity = model.GuardianCity;
                //parent.GuardianPincode = model.GuardianPincode;
                status = _Entities.SaveChanges() > 0;

                student.ParentEmail = model.FatherEmail;
                student.ParentName = model.FatherName;
                student.MotherName = model.MotherName;
                _Entities.SaveChanges();
                if (status)
                    msg = "Successfully Updated!";
                return Json(new { status = status, msg = msg, division = student == null ? model.divisionId.ToString() : student.DivisionId.ToString() });
            }
            #endregion Updates
            #region Add
            else
            {
                var parent = new TbParent();
                parent.ParentName = model.FatherName == null ? "" : model.FatherName;
                //parent.Address = model.FatherAddress;
                //parent.City = model.FatherCity;
                parent.Address = "";
                parent.City = "";
                parent.Email = model.FatherEmail == null ? "" : student.ParentEmail;
                parent.ContactNumber = model.FatherContact == null ? " " : model.FatherContact;
                parent.Password = model.Password == null ? " 123 " : model.Password;
                parent.TimeStamp = CurrentTime;
                parent.ParentGuid = new Guid();
                parent.IsActive = true;
                //parent.State = model.FatherState;
                //parent.PostalCode = model.FatherPincode;
                parent.State = "";
                parent.PostalCode = "";
                parent.FatherEducation = model.FatherEducation;
                parent.FatherOccupation = model.FatherOccupation;
                parent.FatherCountryId = Convert.ToInt32(model.FatherCountryId);
                parent.MotherName = model.MotherName;
                parent.MotherEducation = model.MotherEducaton;
                parent.MotherOccupation = model.MotherOccupation;
                parent.MotherContactNo = model.MotherContact;
                parent.MotherEmail = model.MotherEmail;
                //parent.MotherAddress = model.MotherAddress;
                //parent.MotherCountryId = Convert.ToInt32(model.MotherCountryId);
                //parent.MotherState = model.MotherState;
                //parent.MotherCity = model.MotherCity;
                //parent.MotherPincode = model.MotherPincode;
                parent.MotherAddress = "";
                parent.MotherCountryId = 0;
                parent.MotherState = "";
                parent.MotherCity = "";
                parent.MotherPincode = "";
                parent.GuardianName = model.GuardianName;
                parent.GuardianEducation = model.GuardianEduation;
                parent.GuardianOccupation = model.GuardianOccupation;
                parent.GuardianContactNo = model.GuardianContact;
                parent.GuardianEmail = model.GuardianEmail;
                //parent.GuardianAddress = model.GuardianAddress;
                //parent.GuardianCountryId = Convert.ToInt32(model.GuardianCountryId);
                //parent.GuardianState = model.GuardianState;
                //parent.GuardianCity = model.GuardianCity;
                //parent.GuardianPincode = model.GuardianPincode;
                parent.GuardianAddress = "";
                parent.GuardianCountryId = 0;
                parent.GuardianState = "";
                parent.GuardianCity = "";
                parent.GuardianPincode = "";
                try
                {
                    _Entities.TbParents.Add(parent);
                    status = _Entities.SaveChanges() > 0;
                }
                catch (Exception ex)
                {

                }

                student.ParentEmail = model.FatherEmail == null ? " " : student.ParentEmail;
                student.ParentName = model.FatherName == null ? " " : model.FatherName;
                student.MotherName = model.MotherName == null ? " " : model.MotherName;
                student.ParentId = parent.ParentId;
                _Entities.SaveChanges();

                if (status)
                    msg = "Successful!";
                string division = Convert.ToString(model.divisionId);
                string classId = Convert.ToString(model.classId);
                return Json(new { status = status, msg = msg, division = division, classId = classId });
            }
            #endregion Add     
        }
        public PartialViewResult EditStudentParentsInfo(string id)
        {
            var model = new StudentModel();
            model.studentId = Convert.ToInt64(id);
            model.schoolId = _user.SchoolId;
            var student = _Entities.TbStudents.Where(x => x.StudentId == model.studentId && x.IsActive && x.SchoolId == model.schoolId).FirstOrDefault();
            if (student.ParentId != null)
            {
                var parent = _Entities.TbParents.Where(x => x.ParentId == student.ParentId && x.IsActive).FirstOrDefault();
                model.FatherName = parent.ParentName;
                model.FatherAddress = parent.Address;
                model.FatherCity = parent.City;
                model.FatherEmail = parent.Email;
                model.FatherContact = parent.ContactNumber;
                model.FatherState = parent.State;
                model.FatherPincode = parent.PostalCode;
                model.FatherEducation = parent.FatherEducation;
                model.FatherOccupation = parent.FatherOccupation;
                if (parent.FatherCountryId != null)
                    model.FatherCountryId = (Country)parent.FatherCountryId;
                model.MotherName = parent.MotherName;
                model.MotherEducaton = parent.MotherEducation;
                model.MotherOccupation = parent.MotherOccupation;
                model.MotherContact = parent.MotherContactNo;
                model.MotherEmail = parent.MotherEmail;
                model.MotherAddress = parent.MotherAddress;
                if (parent.MotherCountryId != null)
                    model.MotherCountryId = (Country)parent.MotherCountryId;
                model.MotherState = parent.MotherState;
                model.MotherCity = parent.MotherCity;
                model.MotherPincode = parent.MotherPincode;
                model.GuardianName = parent.GuardianName;
                model.GuardianEduation = parent.GuardianEducation;
                model.GuardianOccupation = parent.GuardianOccupation;
                model.GuardianContact = parent.GuardianContactNo;
                model.GuardianEmail = parent.GuardianEmail;
                model.GuardianAddress = parent.GuardianAddress;
                if (parent.GuardianCountryId != null)
                    model.GuardianCountryId = (Country)parent.GuardianCountryId;
                model.GuardianState = parent.GuardianState;
                model.GuardianCity = parent.GuardianCity;
                model.GuardianPincode = parent.GuardianPincode;
                model.ParentId = parent.ParentId;
            }
            return PartialView("~/Views/School/_pv_EditStudentParentsDetails.cshtml", model);
        }
        public ActionResult StudentDetailedViewPrint(string id)
        {
            long studentId = Convert.ToInt64(id);
            StudentModel model = new StudentModel();
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
            model.stringStudentId = studentDetails.StudentSpecialId;
            model.classId = studentDetails.ClassId;
            model.admissionNo = studentDetails.StudentSpecialId;
            model.studentName = studentDetails.StundentName;
            model.division = studentDetails.Division.Division;
            model.RollNo1 = studentDetails.ClasssNumber == null ? "" : studentDetails.ClasssNumber;
            model.className = studentDetails.Class.Class;
            model.contactNo = studentDetails.ContactNumber;
            model.parentName = studentDetails.ParentName == null ? "" : studentDetails.ParentName;
            model.parentEmail = studentDetails.ParentEmail;
            model.address = studentDetails.Address;
            model.state = studentDetails.State;
            model.city = studentDetails.City;
            model.profilePic = studentDetails.FilePath == null ? "" : studentDetails.FilePath;
            model.gender = studentDetails.Gender;
            model.bloodGroup = studentDetails.BloodGroup;
            if (studentDetails.Dob != null)
                model.DOBstring = Convert.ToDateTime(studentDetails.Dob).ToShortDateString();
            model.MobileNo = studentDetails.MobileNo;
            if (studentDetails.Dob != studentDetails.DateOfJoining)
                model.DateOfJoiningString = Convert.ToDateTime(studentDetails.DateOfJoining).ToShortDateString();
            model.NationalityId = (Nationality)studentDetails.NationalityId;
            model.CountryId = (Country)studentDetails.CountryId;
            model.CategoryId = (StudentCategory)studentDetails.CategoryId;
            model.PlaceOfBirth = studentDetails.PlaceOfBirth;
            model.MotherTongue = studentDetails.MotherTongue;
            model.Pincode = studentDetails.PostalCode;
            if (model.NationalityId != null)
                model.Nationality = Convert.ToString((Nationality)model.NationalityId);
            if (model.CategoryId != null)
                model.Category = Convert.ToString((StudentCategory)model.CategoryId);
            if (model.CountryId != null)
                model.Country = Convert.ToString((Country)model.CountryId);
            if (studentDetails.Parent != null)
            {
                model.FatherName = studentDetails.Parent.ParentName == null ? "" :  studentDetails.Parent.ParentName;
                model.MotherName = studentDetails.Parent.MotherName == null ? "" :  studentDetails.Parent.MotherName;
                model.GuardianName = studentDetails.Parent.GuardianName == null ? "" :  studentDetails.Parent.GuardianName;
                model.FatherOccupation = studentDetails.Parent.FatherOccupation == null ? "" :  studentDetails.Parent.FatherOccupation;
                model.MotherOccupation = studentDetails.Parent.MotherOccupation == null ? "" :  studentDetails.Parent.MotherOccupation;
                model.GuardianOccupation = studentDetails.Parent.GuardianOccupation == null ? "" :  studentDetails.Parent.GuardianOccupation;
                model.FatherContact =  studentDetails.Parent.ContactNumber == null ? "" :  studentDetails.Parent.ContactNumber;
                model.MotherContact =  studentDetails.Parent.MotherContactNo == null ? "" :  studentDetails.Parent.MotherContactNo;
                model.GuardianContact =  studentDetails.Parent.GuardianContactNo == null ? "" :  studentDetails.Parent.GuardianContactNo;
                model.FatherEmail =  studentDetails.Parent.Email == null ? "" :  studentDetails.Parent.Email;
                model.MotherEmail =  studentDetails.Parent.MotherEmail == null ? "" :  studentDetails.Parent.MotherEmail;
                model.GuardianEmail =  studentDetails.Parent.GuardianEmail == null ? "" :  studentDetails.Parent.GuardianEmail;
                model.FatherEducation =  studentDetails.Parent.FatherEducation == null ? "" :  studentDetails.Parent.FatherEducation;
                model.MotherEducaton =  studentDetails.Parent.MotherEducation == null ? "" :  studentDetails.Parent.MotherEducation;
                model.GuardianEduation =  studentDetails.Parent.GuardianEducation == null ? "" :  studentDetails.Parent.GuardianEducation;
            }
            else
            {
                model.FatherName = "";
                model.MotherName = "";
                model.GuardianName = "";
                model.FatherOccupation = "";
                model.MotherOccupation = "";
                model.GuardianOccupation = "";
                model.FatherContact = "";
                model.MotherContact = "";
                model.GuardianContact = "";
                model.FatherEmail = "";
                model.MotherEmail = "";
                model.GuardianEmail = "";
                model.FatherEducation = "";
                model.MotherEducaton = "";
                model.GuardianEduation = "";
            }
            model.SchoolName = _user.School.SchoolName;
            model.SchoolAddress = _user.School.Address;
            model.CurrentDate = CurrentTime.ToShortDateString();
            model.SchoolLogo = _user.School.FilePath;
            return View(model);

        }

        public PartialViewResult ClassDivisionEditPartial(string id)
        {
            SchoolValue model = new SchoolValue();
            string[] splitData = id.Split('~');
            model.classId = Convert.ToInt64(splitData[0]);
            model.divId = Convert.ToInt64(splitData[1]);
            var data = new Satluj_Latest.Data.Division(model.divId);
            model.className = data.ClassName;
            model.DivisionName = data.DivisionName;

            return PartialView("~/Views/School/_pv_ClassDivisionEdit.cshtml", model);
        }
        public object EditClassDivision(SchoolValue model)
        {
            bool status = false;
            string msg = "Failed";
            var data = _Entities.TbDivisions.Where(x => x.ClassId == model.classId && x.Division.ToUpper().Trim() == model.DivisionName.ToUpper().Trim() && x.DivisionId != model.divId).FirstOrDefault();
            if (data != null)
            {
                msg = "This division is already exists in the same class!";
            }
            else
            {
                var old = _Entities.TbDivisions.Where(x => x.DivisionId == model.divId).FirstOrDefault();
                if (old.Division != model.DivisionName)
                {
                    old.Division = model.DivisionName.ToUpper().Trim();
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Success";
                }
                else
                {
                    msg = "No changes !";
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public ActionResult SchoolClassDivisionPrint(string id)
        {
            long Id = Convert.ToInt64(id);
            var model = new Satluj_Latest.Models.ClassDivisionDetailsForPrint();
                        
            var school = _Entities.TbSchools.FirstOrDefault(s => s.SchoolId == _user.SchoolId);

            model.SchoolName = school?.SchoolName;
            model.SchoolAddress = school?.Address;
            model.SchoolLogo = school?.FilePath;
            model.Details = new List<SchoolValue>();
            if (Id == 0)
            {
                model.ReportHead = "All Classes";
                var data = _Entities.TbDivisions.Where(x => x.Class.SchoolId == _user.SchoolId && x.IsActive && x.Class.IsActive).Include(x => x.Class).ToList();
                foreach (var item in data)
                {
                    SchoolValue one = new SchoolValue();
                    one.className = item.Class.Class;
                    one.DivisionName = item.Division;
                    model.Details.Add(one);
                }
            }
            else
            {
                var data = _Entities.TbDivisions.Where(x => x.Class.SchoolId == _user.SchoolId && x.IsActive && x.Class.IsActive && x.ClassId == Id).Include(x => x.Class).ToList();
                model.ReportHead = data[0].Class.Class;
                foreach (var item in data)
                {
                    SchoolValue one = new SchoolValue();
                    one.className = item.Class.Class;
                    one.DivisionName = item.Division;
                    model.Details.Add(one);
                }
            }
            return View(model);
        }
        public ActionResult SchoolSettingsHome()
        {
            SchoolValue model = new SchoolValue();
            model.schoolId = _user.SchoolId;
            return View(model);
        }
        public PartialViewResult ChangePasswordPartialView()
        {
            PasswordChangeModel model = new PasswordChangeModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/School/_pv_ChangePassword.cshtml", model);
        }
        public PartialViewResult SMTPDetailsPartialView()
        {
            SMTPDetailsModel model = new SMTPDetailsModel();
            long schoolId = _user.SchoolId;
            var smtpDetail = _Entities.TbSmtpdetails.Where(z => z.SchoolId == schoolId).FirstOrDefault();
            if (smtpDetail != null)
            {
                model.email = smtpDetail.EmailId;
                model.password = smtpDetail.Password;
                return PartialView("~/Views/School/_pv_SMTP_Edit.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/School/_pv_SMTP_Add.cshtml", model);
            }
        }
        public PartialViewResult LibrarySettingsPartialView()
        {
            FeeAlertDataModel model = new FeeAlertDataModel();

            // Load school & model fields
            var school = _Entities.TbSchools
                .Include(s => s.TbLibraryFines)
                .FirstOrDefault(z => z.SchoolId == _user.SchoolId);

            model.SchoolId = _user.SchoolId;
            model.libraryDueDay = school?.LibraryDueDays ?? 0;

            var libFine = school?.TbLibraryFines?.FirstOrDefault();
            model.feeId = libFine?.FeeId ?? 0;
            model.libFineAmount = libFine?.FineAmount ?? 0;

            // GET DROPDOWN FROM DATABASE
            ViewBag.FeeList = _dropdown.SchoolSpecialFeesList(model.SchoolId);

            return PartialView("_pv_LibrarySettings", model);
        }
        public PartialViewResult SelectSiblings(string id)
        {
            SchoolModel model = new SchoolModel();
            model.CurrentAddedStudent = Convert.ToInt64(id);
            return PartialView("~/Views/School/_pv_SelectSiblings.cshtml", model);
        }
        public PartialViewResult SearchAdmission(string id)
        {
            StudentModel model = new StudentModel();
            string[] splitData = id.Split('~');
            model.admissionNo = splitData[0];
            model.schoolId = Convert.ToInt64(splitData[1]);
            model.CurrentStudentId = Convert.ToInt64(splitData[2]);
            return PartialView("~/Views/School/_pv_StudentDetailsInParent.cshtml", model);
        }
        public object AddParentToKid(string id)
        {
            bool status = false;
            string msg = "Failed";
            string[] splitData = id.Split('~');
            long siblings = Convert.ToInt64(splitData[0]);
            long currentStudent = Convert.ToInt64(splitData[1]);
            var parent = _Entities.TbStudents.Where(x => x.StudentId == siblings && x.IsActive).FirstOrDefault();
            if (parent != null)
            {
                if (parent.ParentId == null)
                {
                    msg = "This students information is not available !";
                }
                else
                {
                    var newStudent = _Entities.TbStudents.Where(x => x.StudentId == currentStudent && x.IsActive).FirstOrDefault();
                    newStudent.ParentId = parent.ParentId;
                    newStudent.ParentName = parent.ParentName;
                    newStudent.MotherName = parent.MotherName;
                    status = _Entities.SaveChanges() > 0;
                }
            }
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg, studentId = currentStudent });
        }
        public PartialViewResult StudentsParentsInfoPageView(string id)
        {
            StudentModel model = new StudentModel();
            model.studentId = Convert.ToInt64(id);
            model.schoolId = _user.SchoolId;
            var studentDetails = _Entities.TbStudents.Where(x => x.StudentId == model.studentId).FirstOrDefault();
            model.classId = studentDetails.ClassId;
            model.divisionId = studentDetails.DivisionId;
            model.FatherName = studentDetails.ParentName;
            model.MotherName = studentDetails.MotherName;
            model.GuardianName =  studentDetails.Parent.GuardianName;
            model.FatherEducation =  studentDetails.Parent.FatherEducation;
            model.MotherEducaton =  studentDetails.Parent.MotherEducation;
            model.GuardianEduation =  studentDetails.Parent.GuardianEducation;
            model.FatherOccupation =  studentDetails.Parent.FatherOccupation;
            model.MotherOccupation =  studentDetails.Parent.MotherOccupation;
            model.GuardianOccupation =  studentDetails.Parent.GuardianOccupation;
            model.FatherContact =  studentDetails.Parent.ContactNumber;
            model.MotherContact =  studentDetails.Parent.MotherContactNo;
            model.GuardianContact =  studentDetails.Parent.GuardianContactNo;
            model.FatherEmail = studentDetails.ParentEmail;
            model.MotherEmail =  studentDetails.Parent.MotherEmail;
            model.GuardianEmail =  studentDetails.Parent.GuardianEmail;
            return PartialView("~/Views/School/_pv_AddStudentPerentsInfoView.cshtml", model);
        }

        public ActionResult SummaryHome()
        {
            SummaryModel model = new SummaryModel();
            model.SchoolId = _user.SchoolId;
            model.DatetimeString = CurrentTime;
            model._list = new List<ClassListForSummary>();
            var classList = _Entities.TbClasses.Where(x => x.SchoolId == model.SchoolId && x.IsActive && x.PublishStatus).OrderBy(x=>x.ClassOrder).ToList();
            foreach(var item in classList)
            {
                ClassListForSummary one = new ClassListForSummary();
                one.ClassId = item.ClassId;
                one.ClassName = item.Class;
                model._list.Add(one);
            }
            return View(model);
        }
        public ActionResult GenerateAttendanceSummary(string id)
        {
            string[] split = id.Split('~');
            var classList = split.Where(x => x != split[0]).ToList();
            AttendanceSummaryFull modelMain = new AttendanceSummaryFull();
            modelMain.SchoolName = _user.School.SchoolName;
            modelMain.SchoolAddress = _user.School.Address;
            modelMain.SchoolLogo = _user.School.FilePath;
            modelMain.AttendanceDate = Convert.ToDateTime(split[0]);
            modelMain._list = new List<AttendanceSummaryReportModel>();
            foreach (var item in classList)
            {
                long ClassId = Convert.ToInt64(item);
                var divisionList = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.IsActive).ToList();
                foreach(var item1 in divisionList)
                {
                    string Maxdate = modelMain.AttendanceDate.Date.ToString("MM-dd-yyyy") + ' ' + "11:59:00 PM";
                    DateTime maxDate = Convert.ToDateTime(Maxdate);
                    AttendanceSummaryReportModel model = new AttendanceSummaryReportModel();
                    var atndance = _Entities.TbAttendances.Where(z => z.ClassId == ClassId && z.DivisionId == item1.DivisionId && z.AttendanceDate >= modelMain.AttendanceDate && z.AttendanceDate <= modelMain.AttendanceDate && z.ShiftStatus == 0).OrderBy(x => x.Student.StundentName).ToList();
                    model.SchoolName = _user.School.SchoolName;
                    model.SchoolAddress = _user.School.Address;
                    model.SchoolLogo = _user.School.FilePath;
                    model.AttendanceDate = modelMain.AttendanceDate;
                    model.ClassName = _user.School.TbClasses.Where(x => x.ClassId == ClassId && x.IsActive).Select(x => x.Class).FirstOrDefault();
                    model.DivisionName = _Entities.TbDivisions.Where(x => x.DivisionId == item1.DivisionId && x.IsActive).Select(x => x.Division).FirstOrDefault();
                    var students = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ClassId == ClassId && x.DivisionId == item1.DivisionId).ToList();
                    if (atndance != null && atndance.Count > 0)
                    {
                        model.Present = atndance.Where(x => x.AttendanceData == true).Count();
                        model.Absent = atndance.Where(x => x.AttendanceData == false).Count();
                    }
                    else
                    {
                        model.Present = 0;
                        model.Absent = 0;
                    }
                    model.TotalStudents = students.Count;
                    if (model.TotalStudents > atndance.Count)
                    {
                        int balance = model.TotalStudents - atndance.Count;
                        model.Absent = model.Absent + balance;
                    }
                    var teacher = _Entities.TbTeacherClasses.Where(x => x.ClassId == ClassId && x.DivisionId == item1.DivisionId && x.Teacher.IsActive).FirstOrDefault();
                    if (teacher != null)
                        model.InCharge = teacher.Teacher.TeacherName;
                    else
                        model.InCharge = "";
                    modelMain._list.Add(model);
                }
            }
            return View(modelMain);
        }

        public ActionResult GenerateTimetableSummary(string id)
        {
            string[] split = id.Split('~');
            TimetableSummaryFull model = new TimetableSummaryFull();
            model.SchoolName = _user.School.SchoolName;
            model.SchoolAddress = _user.School.Address;
            model.SchoolLogo = _user.School.FilePath;
            model._list = new List<ListOfTimetable>();
            foreach (var item in split)
            {
                long ClassId = Convert.ToInt64(item);
                var divisionList = _Entities.TbDivisions.Where(x => x.ClassId == ClassId && x.IsActive).ToList();
                foreach(var item1 in divisionList)
                {
                    ListOfTimetable one = new ListOfTimetable();
                    one.ClassName = item1.Class.Class;
                    one.DivisionName = item1.Division;
                    one.list = new List<TimetableListingModel>();
                    one.list = new Satluj_Latest.Data.School(_user.SchoolId).GetTimetable(ClassId, item1.DivisionId);
                    model._list.Add(one);
                }
            }
            return View(model);
        }












































































































        //chnaged by gayathri a



    }
}



