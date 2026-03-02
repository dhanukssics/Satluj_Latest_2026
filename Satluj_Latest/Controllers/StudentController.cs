using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Satluj_Latest;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Utility;
using Satluj_Latest.Controllers;
using System.Linq;


namespace TrackTap.Controllers
{
    public class StudentController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly DropdownData _dropdown;
        public StudentController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, IWebHostEnvironment env) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _env = env;
        }

        // GET: Student
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Subjects()
        {
            SubjectsModel model = new SubjectsModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public async Task<IActionResult> SubmitAddSubject(SubjectsModel model)
        {
            bool status = false;
            string message = "Failed";
            try
            {
                var old = await _Entities.TbSubjects.Where(x => x.SubjectName.ToUpper() == model.SubjectName.Trim().ToUpper() && x.SchoolI == _user.SchoolId && x.IsActive).FirstOrDefaultAsync();
                if (old == null)
                {
                    var sub = new TbSubject();
                    sub.SchoolI = _user.SchoolId;
                    sub.SubjectName = model.SubjectName;
                    sub.IsActive = true;
                    sub.TmeStamp = CurrentTime;
                    sub.IsOptonal = model.IsOptional;
                    sub.Abbreviation = model.Abbreviation;
                    sub.Code = model.Code;
                    await _Entities.TbSubjects.AddAsync(sub);
                    status =await _Entities.SaveChangesAsync() > 0;
                    message = status ? " Successful" : "Failed !";
                }
                else
                {
                    message = "Already Exists!";
                }
            }
            catch (Exception ex)
            {
                message = status ? " Successful" : "Failed !";
            }

            return Json(new { status = status, msg = message });
        }
        public async Task<IActionResult> DeleteSubject(string id)
        {
            bool status = false;
            string msg = "False";
            long subId = Convert.ToInt64(id);
            var sub = await _Entities.TbSubjects.FirstOrDefaultAsync(x => x.SubId == subId && x.IsActive);
            if (sub != null)
            {
                sub.IsActive = false;
                status = await _Entities.SaveChangesAsync() > 0;
            }
            var optional = await _Entities.TbOptionalSubjectStudents.Where(x => x.SchoolId == _user.SchoolId && x.SubjectId == subId).ToListAsync();
            foreach (var item in optional)
            {
                var one =await _Entities.TbOptionalSubjectStudents.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                one.IsActive = false;
                await _Entities.SaveChangesAsync();
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult GetSubjectDataList()
        {
            var model = new SubjectsModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Student/_pv_SubjectList.cshtml", model);
        }
        public IActionResult TimeTable()
        {
            TimetableModel model = new TimetableModel();
            model.SchoolId = _user.SchoolId;
            model.Period = Periods.One;
            model.SchoolName = _user.School.SchoolName;
            return View(model);
        }
        public PartialViewResult GetTimeTableList(string id)
        {
            string[] splitdata = id.Split('~');
            var model = new TimetableModel();
            model.SchoolId = _user.SchoolId;
            model.ClassId = Convert.ToInt64(splitdata[0]);
            model.DivisonId = Convert.ToInt64(splitdata[1]);
            model.ClassName = new Division(model.DivisonId).ClassName;
            model.DivisionName = new Division(model.DivisonId).DivisionName;
            return PartialView("~/Views/Student/_TimetableList.cshtml", model);
        }
        public async Task<IActionResult> SubmitTimetable(TimetableModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                int day = Convert.ToInt32((Days)model.DayId);
                int period = (int)model.Period + 1;
                if (await _Entities.TbTimeTables.AnyAsync(x => x.SchoolId == _user.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisonId && x.Periods == ((int)model.Period + 1) && x.DayId == (int)model.DayId && x.IsActive))
                {
                    msg = "Already assigned this period, please edit the data!";
                }

                else if (await _Entities.TbTimeTables.AnyAsync(x => x.SchoolId == _user.SchoolId && x.TeacherId == model.TeacherId && x.Periods == period && x.DayId == day && x.IsActive && x.TimeStamp.Year==DateTime.Now.Year))
                {
                    msg = " Already assigned this teacher to an another class !";
                }
                else
                {
                    var data = new TbTimeTable();
                    data.SchoolId = _user.SchoolId;
                    data.ClassId = model.ClassId;
                    data.DivisionId = model.DivisonId;
                    data.TeacherId = model.TeacherId;
                    data.SubjectId = model.SubjectId;
                    data.DayId = (int)model.DayId;
                    data.Periods = (int)model.Period + 1;
                    data.IsActive = true;
                    data.TimeStamp = CurrentTime;
                    await _Entities.TbTimeTables.AddAsync(data);
                    status =await  _Entities.SaveChangesAsync() > 0;
                    msg = " Timetable added !";
                }
            }
            catch (Exception ex)
            {
                msg = "Please select all data";
            }
            //msg = status ? " Timetable added" : "Failed!";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult EditTimeTable(string id)
        {
            string[] splitdata = id.Split('~');
            var model = new TimetableModel();
            model.SchoolId = _user.SchoolId;
            model.ClassId = Convert.ToInt64(splitdata[2]);
            model.DivisonId = Convert.ToInt64(splitdata[3]);
            var dayData = splitdata[1].ToString();
            int period = Convert.ToInt32(splitdata[0]);
            int day = 0;
            if (dayData == "Monday")
                day = 0;
            else if (dayData == "Tuesday")
                day = 1;
            else if (dayData == "Wednesday")
                day = 2;
            else if (dayData == "Thursday")
                day = 3;
            else if (dayData == "Friday")
                day = 4;
            else if (dayData == "Saturday")
                day = 5;
            var data = _Entities.TbTimeTables.Where(x => x.SchoolId == _user.SchoolId && x.ClassId == model.ClassId && x.DivisionId == model.DivisonId && x.Periods == period && x.DayId == day && x.IsActive).FirstOrDefault();
            model.TableId = data.Id;
            model.TeacherId = data.TeacherId;
            model.SubjectId = data.SubjectId;
            model.Period = (Periods)data.Periods - 1;
            model.DayId = (Days)data.DayId;
            return PartialView("~/Views/Student/_pv_EditTimeTable.cshtml", model);
        }
        public async Task<IActionResult> EditTimeTableData(TimetableModel model)
        {
            bool status = false;
            string msg = "Success";
            try
            {
                var data = await _Entities.TbTimeTables.Where(x => x.SchoolId == _user.SchoolId && x.Id == model.TableId && x.IsActive).FirstOrDefaultAsync();
                if (data != null)
                {
                    if (data.TeacherId != model.TeacherId)
                    {
                        if (await _Entities.TbTimeTables.AnyAsync(x => x.SchoolId == _user.SchoolId && x.TeacherId == model.TeacherId && x.Periods == data.Periods && x.DayId == data.DayId && x.IsActive))
                        {
                            msg = " Already assigned this teacher to an another class !";
                        }
                        else
                        {
                            data.TeacherId = model.TeacherId;
                            data.SubjectId = model.SubjectId;
                            _Entities.SaveChanges();
                            status = true;
                            msg = "Successful";
                        }
                    }
                    else
                    {
                        data.TeacherId = model.TeacherId;
                        data.SubjectId = model.SubjectId;
                        _Entities.SaveChanges();
                        status = true;
                        msg = "Successful";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { status = status, msg = msg });
        }
        public IActionResult InstallationInstruction()
        {
            var model = new SchoolModelForId();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SendInstallationInstruction(SchoolModelForId model)
        {
            bool status = false;
            string message = "Failed";

            var schoolId = _user.SchoolId;

            var school = await _Entities.TbSchools
                .FirstOrDefaultAsync(x => x.SchoolId == schoolId);

            if (school == null || !school.SmsActive)
                return Json(new { status, message });

            var package = await _Entities.TbSmsPackages
                .FirstOrDefaultAsync(x => x.SchoolId == schoolId && x.IsActive && !x.IsDisabled);

            if (package == null || package.ToDate < CurrentTime)
                return Json(new { status, message = "SMS package expired" });

            var appLink = await _Entities.TbPushData
                .Where(x => x.SchoolId == schoolId)
                .Select(x => x.PlayStore)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(appLink))
                return Json(new { status = false, message = "Sorry, you don't have an app!" });

            var senderId = await _Entities.TbSchoolSenderIds
                .Where(x => x.SchoolId == schoolId && x.IsActive==true)
                .Select(x => x.SenderId)
                .FirstOrDefaultAsync() ?? "MYSCHO";

            var users = JsonConvert.DeserializeObject<List<SendMessage>>(model.Data);

            if (users == null || !users.Any())
                return Json(new { status = false, message = "No recipients" });

            var smsHead = new TbSmsHead
            {
                Head = "Installation Instruction",
                SchoolId = schoolId,
                SenderType = (int)SMSSendType.Student,
                IsActive = true,
                TimeStamp = CurrentTime
            };

            await _Entities.TbSmsHeads.AddAsync(smsHead);
            await _Entities.SaveChangesAsync();

            using HttpClient httpClient = new HttpClient();

            foreach (var item in users[0].list)
            {
                long studentId = Convert.ToInt64(item.StudentId);

                var student = await _Entities.TbStudents
                    .FirstOrDefaultAsync(x => x.StudentId == studentId);

                if (student == null) continue;

                string smsText =
                    $"Dear parent of {student.StundentName} (Admission No: {student.StudentSpecialId}), " +
                    $"Kindly install the school app using the link: {appLink}";

                int smsCount = (int)Math.Ceiling(smsText.Length / 160.0);

                string url =
                    $"http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1" +
                    $"&numbers={item.Number}&route=2&message={Uri.EscapeDataString(smsText)}&sender={senderId}";

                var response = await httpClient.GetStringAsync(url);
                var smsResponse = JsonConvert.DeserializeObject<alvosmsResp>(response);

                var history = new TbSmsHistory
                {
                    IsActive = true,
                    MessageContent = smsText,
                    MessageDate = CurrentTime,
                    ScholId = schoolId,
                    StuentId = studentId,
                    MobileNumber = item.Number.ToString(),
                    HeadId = smsHead.HeadId,
                    SendStatus = smsResponse?.success.ToString(),
                    MessageReturnId = smsResponse?.data?.FirstOrDefault()?.messageId,
                    DelivaryStatus = "Pending",
                    SmsSentPerStudent = smsCount
                };

                await _Entities.TbSmsHistories.AddAsync(history);
                await _Entities.SaveChangesAsync();
            }

            return Json(new { status = true, message = "Success" });
        }

        private HttpClient CreateHttpClient(bool allowAutoRedirect = true)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = allowAutoRedirect,

                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
            );

            return client;
        }

        public PartialViewResult ProgressCardView(string id)
        {
            string[] splitdata = id.Split('~');
            StudentModel model = new StudentModel();
            model.studentId = Convert.ToInt64(splitdata[0]);
            model.classId = Convert.ToInt64(splitdata[1]);
            model.divisionId = Convert.ToInt64(splitdata[2]);
            model.schoolId = _user.SchoolId;
            return PartialView("~/Views/Student/_p_v_ProgressCard.cshtml", model);
        }
        public PartialViewResult ProgressCardButton(string id)
        {
            string[] splitdata = id.Split('~');
            StudentModel model = new StudentModel();
            model.studentId = Convert.ToInt64(splitdata[0]);
            model.Data = Convert.ToString(splitdata[1]);
            return PartialView("~/Views/Student/_pv_ProgressCardButton.cshtml", model);
        }
        public IActionResult ProgressCardGeneration(string id)
        {
            string[] splitdata = id.Split('~');
            long studnetId = Convert.ToInt64(splitdata[0]);
            string[] exams = splitdata[1].Split(',');
            int examcount = exams.Length; //basheer on 28/01/2019 to get the count of exams
            var student = _Entities.TbStudents.Where(x => x.StudentId == studnetId && x.IsActive).FirstOrDefault();
            ProgressCardReportModel model = new ProgressCardReportModel();
            model.SchoolName = student.School.SchoolName;
            model.SchoolLogo = student.School.FilePath;
            model.SchoolAddress = student.School.Address;
            model.ClassName = student.Class.Class + " " + student.Division.Division;
            model.AccademicSession = student.Class.AcademicYear.AcademicYear;
            model.StudentName = student.StundentName;
            model.StudentAddress = student.Address;
            model.Parent = student.ParentName;
            model.DateOfBirth = student.Dob.ToString();
            model.AdmissionNumber = student.StudentSpecialId;
            model.ClassNumber = student.ClasssNumber;
            if (examcount > 1)
            {
                long examOne = Convert.ToInt64(exams[0]);
                long examTwo = Convert.ToInt64(exams[1]);
                var one = _Entities.TbExams.Where(x => x.ExamId == examOne && x.IsActive).ToList().Select(x => new Exams(x)).FirstOrDefault();
                var two = _Entities.TbExams.Where(x => x.ExamId == examTwo && x.IsActive).ToList().Select(x => new Exams(x)).FirstOrDefault();
                model.ExamOne = one.ExamName;
                model.ExamTwo = two.ExamName;
                List<ExamSubjects> oneSub = one.ExamSubjectsList.Union(two.ExamSubjectsList).ToList();
                var subjects = oneSub.Select(x => x.SubjectId).Distinct().ToList().Select(x => new Subjects(x)).ToList();
                List<StudentMarks> markOne = student.TbStudentMarks.Where(x => x.ExamId == one.ExamId && x.IsActive).ToList().Select(x => new StudentMarks(x)).ToList();
                List<StudentMarks> markTwo = student.TbStudentMarks.Where(x => x.ExamId == two.ExamId && x.IsActive).ToList().Select(x => new StudentMarks(x)).ToList();

                model.Marks = new List<StudentProgressCardMarks>();
                foreach (var item in subjects)
                {
                    StudentProgressCardMarks progress = new StudentProgressCardMarks();
                    progress.Subject = item.SubjectName;
                    var x1 = one.ExamSubjectsList.Where(x => x.SubjectId == item.SubId && x.IsActive).FirstOrDefault();
                    var x2 = two.ExamSubjectsList.Where(x => x.SubjectId == item.SubId && x.IsActive).FirstOrDefault();
                    if (x1 != null)
                    {
                        progress.InternalOne = markOne.Where(x => x.SubjectId == x1.SubId).Select(x => x.InternalMark).FirstOrDefault() ?? 0;
                        progress.ExternalOne = markOne.Where(x => x.SubjectId == x1.SubId).Select(x => x.ExternalMark).FirstOrDefault() ?? 0;
                        progress.TotalOne = progress.InternalOne + progress.ExternalOne;
                        progress.InternalTotalOne = x1.InternalMarks;
                        progress.ExternalTotalOne = x1.ExternalMark;
                        progress.GrandTotalOne = x1.InternalMarks + x1.ExternalMark;
                    }
                    else
                    {
                        progress.InternalOne = 0;
                        progress.ExternalOne = 0;
                        progress.TotalOne = progress.InternalOne + progress.ExternalOne;
                        progress.InternalTotalOne = 0;
                        progress.ExternalTotalOne = 0;
                        progress.GrandTotalOne = 0;
                    }

                    if (x2 != null)
                    {
                        progress.InternalTwo = markTwo.Where(x => x.SubjectId == x2.SubId).Select(x => x.InternalMark).FirstOrDefault() ?? 0;
                        progress.ExternalTwo = markTwo.Where(x => x.SubjectId == x2.SubId).Select(x => x.ExternalMark).FirstOrDefault() ?? 0;
                        progress.TotalTwo = progress.InternalTwo + progress.ExternalTwo;
                        progress.InternalTotalTwo = x2.InternalMarks;
                        progress.ExternalTotalTwo = x2.ExternalMark;
                        progress.GrandTotalTwo = x2.InternalMarks + x2.ExternalMark;
                    }
                    else
                    {
                        progress.InternalTwo = 0;
                        progress.ExternalTwo = 0;
                        progress.TotalTwo = progress.InternalTwo + progress.ExternalTwo;
                        progress.InternalTotalTwo = 0;
                        progress.ExternalTotalTwo = 0;
                        progress.GrandTotalTwo = 0;
                    }

                    progress.GrandTotal = progress.TotalOne + progress.TotalTwo;
                    progress.GrandGrandTotal = progress.GrandTotalOne + progress.GrandTotalTwo;

                    var totalOutOff = oneSub.Where(x => x.ExamId == one.ExamId && x.SubjectId == item.SubId).Select(x => x.InternalMarks).FirstOrDefault() + oneSub.Where(x => x.ExamId == one.ExamId && x.SubjectId == item.SubId).Select(x => x.ExternalMark).FirstOrDefault();
                    totalOutOff = totalOutOff + oneSub.Where(x => x.ExamId == two.ExamId && x.SubjectId == item.SubId).Select(x => x.InternalMarks).FirstOrDefault() + oneSub.Where(x => x.ExamId == two.ExamId && x.SubjectId == item.SubId).Select(x => x.ExternalMark).FirstOrDefault();
                    if (totalOutOff == 0)
                        totalOutOff = 1;
                    var percentage = Math.Round((progress.GrandTotal / totalOutOff) * 100, 2);
                    if (percentage > 90)
                        progress.Grade = "A1";
                    else if (percentage > 80)
                        progress.Grade = "A2";
                    else if (percentage > 70)
                        progress.Grade = "B1";
                    else if (percentage > 60)
                        progress.Grade = "B2";
                    else if (percentage > 50)
                        progress.Grade = "C1";
                    else if (percentage > 40)
                        progress.Grade = "C2";
                    else if (percentage > 32)
                        progress.Grade = "D";
                    else
                        progress.Grade = "E";
                    progress.Rank = "1";
                    model.Marks.Add(progress);
                }
                var OutOff = oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.InternalMarks) + oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.ExternalMark);
                OutOff = OutOff + oneSub.Where(x => x.ExamId == two.ExamId && x.IsActive).Sum(x => x.InternalMarks) + oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.ExternalMark);
                if (OutOff == 0)
                    OutOff = 1;
                var studentTotal = model.Marks.Sum(x => x.GrandTotal);
                model.Overall = studentTotal + "/" + OutOff;
                model.Percentage = (studentTotal / OutOff) * 100;
                if (model.Percentage > 90)
                    model.Grade = "A1";
                else if (model.Percentage > 80)
                    model.Grade = "A2";
                else if (model.Percentage > 70)
                    model.Grade = "B1";
                else if (model.Percentage > 60)
                    model.Grade = "B2";
                else if (model.Percentage > 50)
                    model.Grade = "C1";
                else if (model.Percentage > 40)
                    model.Grade = "C2";
                else if (model.Percentage > 32)
                    model.Grade = "D";
                else
                    model.Grade = "E";
                model.Rank = "1";
                if (model.Percentage < 32)
                {
                    model.Status = "";
                }
                else
                {
                    model.Status = "**Congratulations!";
                }
            }
            else
            {
                long examOne = Convert.ToInt64(exams[0]);
                // long examTwo = 0;
                var one = _Entities.TbExams.Where(x => x.ExamId == examOne && x.IsActive).ToList().Select(x => new Exams(x)).FirstOrDefault();
                //var two = _Entities.tb_Exams.Where(x => x.ExamId == examTwo && x.IsActive).ToList().Select(x => new Exams(x)).FirstOrDefault();
                model.ExamOne = one.ExamName;
                //model.ExamTwo = two.ExamName;
                List<ExamSubjects> oneSub = one.ExamSubjectsList.ToList();
                var subjects = oneSub.Select(x => x.SubjectId).Distinct().ToList().Select(x => new Subjects(x)).ToList();
                List<StudentMarks> markOne = student.TbStudentMarks.Where(x => x.ExamId == one.ExamId && x.IsActive).ToList().Select(x => new StudentMarks(x)).ToList();
                //List<StudentMarks> markTwo = student.tb_StudentMarks.Where(x => x.ExamId == two.ExamId && x.IsActive).ToList().Select(x => new StudentMarks(x)).ToList();

                model.Marks = new List<StudentProgressCardMarks>();
                foreach (var item in subjects)
                {
                    StudentProgressCardMarks progress = new StudentProgressCardMarks();
                    progress.Subject = item.SubjectName;
                    var x1 = one.ExamSubjectsList.Where(x => x.SubjectId == item.SubId && x.IsActive).FirstOrDefault();
                    //var x2 = two.ExamSubjectsList.Where(x => x.SubjectId == item.SubId && x.IsActive).FirstOrDefault();
                    if (x1 != null)
                    {
                        progress.InternalOne = markOne.Where(x => x.SubjectId == x1.SubId).Select(x => x.InternalMark).FirstOrDefault() ?? 0;
                        progress.ExternalOne = markOne.Where(x => x.SubjectId == x1.SubId).Select(x => x.ExternalMark).FirstOrDefault() ?? 0;
                        progress.TotalOne = progress.InternalOne + progress.ExternalOne;
                        progress.InternalTotalOne = x1.InternalMarks;
                        progress.ExternalTotalOne = x1.ExternalMark;
                        progress.GrandTotalOne = x1.InternalMarks + x1.ExternalMark;
                    }
                    else
                    {
                        progress.InternalOne = 0;
                        progress.ExternalOne = 0;
                        progress.TotalOne = progress.InternalOne + progress.ExternalOne;
                        progress.InternalTotalOne = 0;
                        progress.ExternalTotalOne = 0;
                        progress.GrandTotalOne = 0;
                    }

                    //if (x2 != null)
                    //{
                    //    progress.InternalTwo = markTwo.Where(x => x.SubjectId == x2.SubId).Select(x => x.InternalMark).FirstOrDefault() ?? 0;
                    //    progress.ExternalTwo = markTwo.Where(x => x.SubjectId == x2.SubId).Select(x => x.ExternalMark).FirstOrDefault() ?? 0;
                    //    progress.TotalTwo = progress.InternalTwo + progress.ExternalTwo;
                    //    progress.InternalTotalTwo = x2.InternalMarks;
                    //    progress.ExternalTotalTwo = x2.ExternalMark;
                    //    progress.GrandTotalTwo = x2.InternalMarks + x2.ExternalMark;
                    //}
                    //else
                    //{
                    //    progress.InternalTwo = 0;
                    //    progress.ExternalTwo = 0;
                    //    progress.TotalTwo = progress.InternalTwo + progress.ExternalTwo;
                    //    progress.InternalTotalTwo = 0;
                    //    progress.ExternalTotalTwo = 0;
                    //    progress.GrandTotalTwo = 0;
                    //}

                    progress.GrandTotal = progress.TotalOne;
                    progress.GrandGrandTotal = progress.GrandTotalOne;

                    var totalOutOff = oneSub.Where(x => x.ExamId == one.ExamId && x.SubjectId == item.SubId).Select(x => x.InternalMarks).FirstOrDefault() + oneSub.Where(x => x.ExamId == one.ExamId && x.SubjectId == item.SubId).Select(x => x.ExternalMark).FirstOrDefault();
                    //totalOutOff = totalOutOff + oneSub.Where(x => x.ExamId == two.ExamId && x.SubjectId == item.SubId).Select(x => x.InternalMarks).FirstOrDefault() + oneSub.Where(x => x.ExamId == two.ExamId && x.SubjectId == item.SubId).Select(x => x.ExternalMark).FirstOrDefault();
                    if (totalOutOff == 0)
                        totalOutOff = 1;
                    var percentage = Math.Round((progress.GrandTotal / totalOutOff) * 100, 1); //changed 2 into 1,because only one test 
                    if (percentage > 90)
                        progress.Grade = "A1";
                    else if (percentage > 80)
                        progress.Grade = "A2";
                    else if (percentage > 70)
                        progress.Grade = "B1";
                    else if (percentage > 60)
                        progress.Grade = "B2";
                    else if (percentage > 50)
                        progress.Grade = "C1";
                    else if (percentage > 40)
                        progress.Grade = "C2";
                    else if (percentage > 32)
                        progress.Grade = "D";
                    else
                        progress.Grade = "E";
                    progress.Rank = "1";
                    model.Marks.Add(progress);
                }
                var OutOff = oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.InternalMarks) + oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.ExternalMark);
                //OutOff = OutOff + oneSub.Where(x => x.ExamId == two.ExamId && x.IsActive).Sum(x => x.InternalMarks) + oneSub.Where(x => x.ExamId == one.ExamId && x.IsActive).Sum(x => x.ExternalMark);
                if (OutOff == 0)
                    OutOff = 1;
                var studentTotal = model.Marks.Sum(x => x.GrandTotal);
                model.Overall = studentTotal + "/" + OutOff;
                model.Percentage = (studentTotal / OutOff) * 100;
                if (model.Percentage > 90)
                    model.Grade = "A1";
                else if (model.Percentage > 80)
                    model.Grade = "A2";
                else if (model.Percentage > 70)
                    model.Grade = "B1";
                else if (model.Percentage > 60)
                    model.Grade = "B2";
                else if (model.Percentage > 50)
                    model.Grade = "C1";
                else if (model.Percentage > 40)
                    model.Grade = "C2";
                else if (model.Percentage > 32)
                    model.Grade = "D";
                else
                    model.Grade = "E";
                model.Rank = "1";
                if (model.Percentage < 32)
                {
                    model.Status = "";
                }
                else
                {
                    model.Status = "**Congratulations!";
                }

            }

            var present = _Entities.TbAttendances.Where(x => x.StudentId == student.StudentId && x.IsActive && x.AttendanceDate.Year == CurrentTime.Year && x.AttendanceData == true).Count();
            var total = _Entities.TbAttendances.Where(x => x.StudentId == student.StudentId && x.IsActive && x.AttendanceDate.Year == CurrentTime.Year).Count();
            model.Attendance = present.ToString() + " : " + total.ToString();
            model.CurrentDate = CurrentTime.ToShortDateString();
            return View(model);
        }
        public PartialViewResult GetTeacherTimeTableList()
        {
            var model = new TimetableModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Student/_pv_TeacherTimeList.cshtml", model);
        }
        public PartialViewResult GetTeacherTimeTableListReport(string id)
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.teacherId = Convert.ToInt64(id);
            var data = _Entities.TbTeachers.Where(x => x.TeacherId == model.teacherId && x.IsActive).FirstOrDefault();
            model.userId = data.UserId;
            model.TeacherName = data.TeacherName;
            return PartialView("~/Views/Student/_pv_TeacherTimeTable.cshtml", model);
        }
        public PartialViewResult SubjectsEditPartial(string id)
        {
            SubjectsModel model = new SubjectsModel();
            long subId = Convert.ToInt64(id);
            var data = _Entities.TbSubjects.Where(x => x.SubId == subId && x.IsActive && x.SchoolI == _user.SchoolId).FirstOrDefault();
            if (data != null)
            {
                model.SubjectId = data.SubId;
                model.SubjectName = data.SubjectName;
                model.IsOptional = data.IsOptonal;
                model.Abbreviation = data.Abbreviation;
                model.Code = data.Code;
            }
            return PartialView("~/Views/Student/_pv_SubjectEdit.cshtml", model);
        }
        public async Task<IActionResult> EditSubjects(SubjectsModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (await _Entities.TbSubjects.AnyAsync(x => x.SchoolI == _user.SchoolId && x.SubjectName.Trim().ToUpper() == model.SubjectName.Trim().ToUpper() && x.SubId != model.SubjectId && x.IsOptonal == model.IsOptional && x.IsActive))
            {
                msg = "Already we have the same Subject !";
            }
            else
            {
                var data = await _Entities.TbSubjects.Where(x => x.SubId == model.SubjectId && x.IsActive).FirstOrDefaultAsync();
                if (data.SubjectName == model.SubjectName)
                {
                    if (data.IsOptonal == model.IsOptional)
                    {
                        data.Abbreviation = model.Abbreviation;
                        data.Code = model.Code;
                        await _Entities.SaveChangesAsync();
                        msg = "Success";
                        status = true;
                    }
                    else
                    {
                        if (data.IsOptonal == true && model.IsOptional == false)
                        {
                            var students = await _Entities.TbOptionalSubjectStudents.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.SubjectId == model.SubjectId).ToListAsync();
                            foreach (var item in students)
                            {
                                var one = await _Entities.TbOptionalSubjectStudents.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                                one.IsActive = false;
                                await _Entities.SaveChangesAsync();
                            }
                        }
                        data.SubjectName = model.SubjectName;
                        data.IsOptonal = model.IsOptional;
                        data.Abbreviation = model.Abbreviation;
                        data.Code = model.Code;
                        status = await _Entities.SaveChangesAsync() > 0;
                        if (status)
                            msg = "Success";
                    }
                }
                else
                {
                    if (data.IsOptonal == true && model.IsOptional == false)
                    {
                        var students = await _Entities.TbOptionalSubjectStudents.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.SubjectId == model.SubjectId).ToListAsync();
                        foreach (var item in students)
                        {
                            var one =await  _Entities.TbOptionalSubjectStudents.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                            one.IsActive = false;
                            await _Entities.SaveChangesAsync();
                        }
                    }
                    data.SubjectName = model.SubjectName;
                    data.IsOptonal = model.IsOptional;
                    data.Abbreviation = model.Abbreviation;
                    data.Code = model.Code;
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Success";
                }
            }
            return Json(new { status = status, msg = msg });
        }


        //18-sep-2020 Jibin Start..........................................

        #region 18-sep-2020 Jibin Start


        //added by jibin 28/08/2020

        public IActionResult Upload_Assignmentsadm()
        {

            // var students = _Entities.tb_AssesmentUpload.ToList();
            ExamResultMainModel model = new ExamResultMainModel();
            model.Data = new List<FileListData>();
            model.SchoolId = _user.SchoolId;
            ViewBag.Teacherlist = _dropdown.GetTeachers(model.SchoolId);
            return View(model);
        }


        public IActionResult Upload_Assignments()
        {


            ExamResultMainModel model = new ExamResultMainModel();
            model.Data = new List<FileListData>();
            model.SchoolId = _user.SchoolId;

            var teacher_model = _Entities.TbTeachers.Where(x => x.UserId == _user.UserId).FirstOrDefault();
            var teachersub_model = _Entities.TbTeacherClassSubjects.Where(x => x.TeacherId == teacher_model.TeacherId).FirstOrDefault();

            if (teachersub_model == null)//jibin 9/16/2020
            {


                var teachersub = _Entities.TbTeacherClasses.Where(x => x.TeacherId == teacher_model.TeacherId).FirstOrDefault();
                model.ClassId = teachersub.ClassId;
                model.divisionId = teachersub.DivisionId;
                model.UserId = _user.UserId;
                model.TeacherId = teacher_model.TeacherId;

                return View(model);

            }
            model.ClassId = teachersub_model.ClassId;
            model.divisionId = teachersub_model.DivisionId;
            model.UserId = _user.UserId;
            model.TeacherId = teacher_model.TeacherId;


            ViewBag.Subjectlist = _dropdown.GetSubject(model.TeacherId);
            ViewBag.classlist = _dropdown.GetAllClassesTeacherWise(model.TeacherId, model.SchoolId);


            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> trackFiles,TbAssesmentUpload user)
        {
            try
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "Media", "Uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (trackFiles != null && trackFiles.Count > 0)
                {
                    foreach (var file in trackFiles)
                    {
                        if (file.Length == 0) continue;

                        string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);

                        string finalFileName = originalName + extension;
                        string fullPath = Path.Combine(uploadFolder, finalFileName);

                        int counter = 1;
                        while (System.IO.File.Exists(fullPath))
                        {
                            finalFileName = $"{originalName}_{counter}{extension}";
                            fullPath = Path.Combine(uploadFolder, finalFileName);
                            counter++;
                        }

                        // Save file
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var teacher = await _Entities.TbTeachers
                            .FirstOrDefaultAsync(x => x.Email == _user.Username);

                        var subject = await _Entities.TbSubjects
                            .FirstOrDefaultAsync(x => x.SubId == user.SubjectId);

                        var division = await _Entities.TbDivisions
                            .FirstOrDefaultAsync(x => x.DivisionId == user.Division);

                        var data = new TbAssesmentUpload
                        {
                            ClassId = user.ClassId,
                            Division = user.Division,
                            DivisionName = division?.Division,
                            Description = user.Description,
                            Filename = finalFileName,
                            SubjectId = user.SubjectId,
                            SubjectName = subject?.SubjectName,
                            FileUploadedDate = CurrentTime,
                            TeacherName = teacher?.TeacherName
                        };

                        await _Entities.TbAssesmentUploads.AddAsync(data);
                        await _Entities.SaveChangesAsync();
                    }
                }
                else
                {
                    // No file uploaded – metadata only
                    var teacher = await _Entities.TbTeachers
                        .FirstOrDefaultAsync(x => x.Email == _user.Username);

                    var subject = await _Entities.TbSubjects
                        .FirstOrDefaultAsync(x => x.SubId == user.SubjectId);

                    var division = await _Entities.TbDivisions
                        .FirstOrDefaultAsync(x => x.DivisionId == user.Division);

                    var data = new TbAssesmentUpload
                    {
                        ClassId = user.ClassId,
                        Division = user.Division,
                        DivisionName = division?.Division,
                        Description = user.Description,
                        SubjectId = user.SubjectId,
                        SubjectName = subject?.SubjectName,
                        FileUploadedDate = CurrentTime,
                        TeacherName = teacher?.TeacherName,
                        Filename = string.Empty
                    };

                    await _Entities.TbAssesmentUploads.AddAsync(data);
                    await _Entities.SaveChangesAsync();
                }

                TempData["ID"] = user.ClassId;
                TempData["alertMessage"] = "Successfully Uploaded..!";

                return Json(new { status = true, message = "Upload successful" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> UploadFileadm(List<IFormFile> trackFiles,TbAssesmentUpload user)
        {
            try
            {
                bool status = false;
                string uploadFolder = Path.Combine(_env.WebRootPath, "Media", "Uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (trackFiles != null && trackFiles.Count > 0)
                {
                    foreach (var file in trackFiles)
                    {
                        if (file.Length == 0) continue;

                        string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);

                        string finalFileName = originalName + extension;
                        string fullPath = Path.Combine(uploadFolder, finalFileName);
                        int counter = 1;

                        while (System.IO.File.Exists(fullPath))
                        {
                            finalFileName = $"{originalName}_{counter}{extension}";
                            fullPath = Path.Combine(uploadFolder, finalFileName);
                            counter++;
                        }

                        // Save file
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var subject = await _Entities.TbSubjects
                            .FirstOrDefaultAsync(x => x.SubId == user.SubjectId);

                        var division = await _Entities.TbDivisions
                            .FirstOrDefaultAsync(x => x.DivisionId == user.Division);

                        var data = new TbAssesmentUpload
                        {
                            ClassId = user.ClassId,
                            Division = user.Division,
                            DivisionName = division?.Division,
                            Description = user.Description,
                            Filename = finalFileName,
                            SubjectId = user.SubjectId,
                            SubjectName = subject?.SubjectName,
                            FileUploadedDate = CurrentTime,
                            TeacherName = user.TeacherName
                        };

                        await _Entities.TbAssesmentUploads.AddAsync(data);
                        status = await _Entities.SaveChangesAsync() > 0;

                        if (status)
                        {
                            var notification = new TbNotification
                            {
                                SchoolId = _user.SchoolId,
                                ClassId = user.ClassId,
                                DivisionId = user.Division,
                                NotificationMessage = user.Description,
                                CreatedAt = CurrentTime,
                                IsRead = 0,
                                Source = "Assignment Upload",
                                SourceId = data.FileId
                            };

                            await _Entities.TbNotifications.AddAsync(notification);
                            await _Entities.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    // No file uploaded – metadata only
                    var subject = await _Entities.TbSubjects
                        .FirstOrDefaultAsync(x => x.SubId == user.SubjectId);

                    var division = await _Entities.TbDivisions
                        .FirstOrDefaultAsync(x => x.DivisionId == user.Division);

                    var data = new TbAssesmentUpload
                    {
                        ClassId = user.ClassId,
                        Division = user.Division,
                        DivisionName = division?.Division,
                        Description = user.Description,
                        SubjectId = user.SubjectId,
                        SubjectName = subject?.SubjectName,
                        FileUploadedDate = CurrentTime,
                        TeacherName = user.TeacherName,
                        Filename = string.Empty
                    };

                    await _Entities.TbAssesmentUploads.AddAsync(data);
                    status = await _Entities.SaveChangesAsync() > 0;

                    if (status)
                    {
                        var notification = new TbNotification
                        {
                            SchoolId = _user.SchoolId,
                            ClassId = user.ClassId,
                            DivisionId = user.Division,
                            NotificationMessage = $"New Assignment Uploaded: {user.Description}",
                            CreatedAt = CurrentTime,
                            IsRead = 0,
                            Source = "Assignment",
                            SourceId = data.FileId
                        };

                        await _Entities.TbNotifications.AddAsync(notification);
                        await _Entities.SaveChangesAsync();
                    }
                }

                TempData["ID"] = user.ClassId;
                TempData["alertMessage"] = "Successfully Uploaded..!";

                return Json(new { status = true, message = "Files uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public PartialViewResult ListAssessmentadm(int id)
        {


            var students = _Entities.TbAssesmentUploads.Where(x => x.ClassId == id).ToList();
            AssementUpload model = new AssementUpload();
            model.Data = new List<FileListDatanew>();
            foreach (var item in students)
            {
                FileListDatanew one = new FileListDatanew();
                one.FileName = item.Filename;
                one.ClassId = item.ClassId;
                one.DivisionName = item.DivisionName;
                one.FileId = item.FileId;
                one.DivisionName = item.DivisionName;
                one.SubjectId = item.SubjectId;
                one.File_Uploaded_Date = item.FileUploadedDate;
                model.Data.Add(one);

            }


            return PartialView("~/Views/Student/_pv_ListAssessmentadm.cshtml", model);
        }



        [HttpPost]
        public PartialViewResult ListAssessment(int id)
        {


            var students = _Entities.TbAssesmentUploads.Where(x => x.ClassId == id).ToList();
            AssementUpload model = new AssementUpload();
            model.Data = new List<FileListDatanew>();
            foreach (var item in students)
            {
                FileListDatanew one = new FileListDatanew();
                one.FileName = item.Filename;
                one.ClassId = item.ClassId;
                one.DivisionName = item.DivisionName;
                one.FileId = item.FileId;
                one.DivisionName = item.DivisionName;
                one.SubjectId = item.SubjectId;
                one.File_Uploaded_Date = item.FileUploadedDate;
                model.Data.Add(one);

            }


            return PartialView("~/Views/Student/_pv_ListAssessment.cshtml", model);
        }

        public async Task<IActionResult> Delete_Assignments(int id)
        {
            await _Entities.Database.ExecuteSqlRawAsync("EXEC SP_DeleteOldAssessment {0}", id);

            await _Entities.SaveChangesAsync();
            var notifications =await _Entities.TbNotifications
                   .Where(n => n.Source == "Assignment Upload" && n.SourceId == id)
                   .ToListAsync();

            if (notifications.Any())
            {
                _Entities.TbNotifications.RemoveRange(notifications);
                await _Entities.SaveChangesAsync();
            }
            TempData["alertMessage"] = "File is Deleted";
            return RedirectToAction("Upload_Assignments");
        }

        public async Task<IActionResult> Delete_Assignmentsadm(int id)
        {
            await _Entities.Database.ExecuteSqlRawAsync("EXEC SP_DeleteOldAssessment {0}", id);
            await _Entities.SaveChangesAsync();
            var notifications = await _Entities.TbNotifications
                  .Where(n => n.Source == "Assignment Upload" && n.SourceId == id)
                  .ToListAsync();

            if (notifications.Any())
            {
                _Entities.TbNotifications.RemoveRange(notifications);
                await _Entities.SaveChangesAsync();
            }
            TempData["alertMessage"] = "File is Deleted";
            return RedirectToAction("Upload_Assignmentsadm");
        }


        public IActionResult Update_AssessmentFile(int id)
        {
            var students = _Entities.TbAssesmentUploads.Where(x => x.FileId == id).FirstOrDefault();
            AssementUpload model = new AssementUpload();

            model.FileId = students.FileId;


            return View(model);
        }

        public IActionResult Update_AssessmentFileadm(int id)
        {
            var students = _Entities.TbAssesmentUploads.Where(x => x.FileId == id).FirstOrDefault();
            AssementUpload model = new AssementUpload();

            model.FileId = students.FileId;


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update_Assignments(AssementUpload user, List<IFormFile> TrackFile)
        {
            try
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "Media/Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var file in TrackFile)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);

                    string finalFileName = originalName + extension;
                    string fullPath = Path.Combine(uploadFolder, finalFileName);

                    int counter = 1;
                    while (System.IO.File.Exists(fullPath))
                    {
                        finalFileName = $"{originalName}_{counter}{extension}";
                        fullPath = Path.Combine(uploadFolder, finalFileName);
                        counter++;
                    }

                   
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    
                    await _Entities.Database.ExecuteSqlRawAsync(
                        "EXEC SP_UpdateOldAssessment {0}, {1}",
                        user.FileId,
                        finalFileName
                    );
                }

                TempData["alertMessage"] = "File updated successfully";
                return RedirectToAction("Upload_Assignments");
            }
            catch (Exception)
            {
                TempData["alertMessage"] = "Update failed!";
                return RedirectToAction("Upload_Assignments");
            }
        }



        [HttpPost]
        public async Task<IActionResult> Update_Assignmentsadm(AssementUpload user, List<IFormFile> TrackFile)
        {
            try
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "Media/Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var file in TrackFile)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);

                    string finalFileName = originalName + extension;
                    string fullPath = Path.Combine(uploadFolder, finalFileName);

                    int counter = 1;
                    while (System.IO.File.Exists(fullPath))
                    {
                        finalFileName = $"{originalName}_{counter}{extension}";
                        fullPath = Path.Combine(uploadFolder, finalFileName);
                        counter++;
                    }

                   
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                   
                    await _Entities.Database.ExecuteSqlRawAsync(
                        "EXEC SP_UpdateOldAssessment {0}, {1}",
                        user.FileId,
                        finalFileName
                    );
                }

                TempData["alertMessage"] = "File is Updated";
                return RedirectToAction("Upload_Assignmentsadm");
            }
            catch
            {
                TempData["alertMessage"] = "Update Failed!";
                return RedirectToAction("Upload_Assignmentsadm");
            }
        }



        public FileResult DownloadFiles(string Filename)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "/Media/Uploads/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + Filename);
            string fileName = Filename;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public IActionResult View_Answer(int id, string divid, int subid)
        {


            var students = _Entities.TbStudentAssesmentUploads.Where(x => x.ClassId == id && x.SubjectId == subid && x.Division == divid).ToList();//add conditions
            AssementUpload model = new AssementUpload();
            model.Data = new List<FileListDatanew>();
            model.SchoolId = Convert.ToInt32(_user.SchoolId);
            foreach (var item in students)
            {
                FileListDatanew one = new FileListDatanew();

                one.Description = item.Description;
                one.FileName = item.FileName;
                one.File_Uploaded_Date = item.UploadedDate;
                one.DivisionName = item.Division;
                var studname = _Entities.TbStudents.Where(x => x.StudentId == item.StudentId).ToList();
                foreach (var std in studname)
                    one.StudentName = std.StundentName;

                model.Data.Add(one);


            }
            return View(model);
        }


        #endregion

        //18-sep-2020 Jibin End..........................................
    }
}

