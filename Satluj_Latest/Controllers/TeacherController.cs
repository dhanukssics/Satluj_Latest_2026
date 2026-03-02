
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Service.Helper;
using Satluj_Latest.Utility;
using System.Net;
using System.Net.Mail;
using System.Text;
using static Satluj_Latest.Models.SchoolValue;
using Student = Satluj_Latest.Models.Student;



namespace Satluj_Latest.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private DropdownData _dropdown ;
        public TeacherController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, IWebHostEnvironment env,DropdownData dropdown) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _env = env;
            _dropdown = dropdown;
        }

        // GET: Teacher
        public IActionResult Attendance()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.userId = _user.UserId;
            model.Selecteddate = CurrentTime;
            DropdownData dropdown = new DropdownData();
            ViewBag.TeacherClassList = dropdown.GetTeacherClass(model.userId);
            ViewBag.Classlist = dropdown.GetClasses(model.schoolId);

            return View(model);
        }

        public PartialViewResult AttendanceMarking(string id)
        {
            string[] splitData = id.Split('~');

            DateTime minDate = DateTime.Parse(splitData[0]);
            int shift = int.Parse(splitData[1]);
            long classId = long.Parse(splitData[2]);
            long divisionId = long.Parse(splitData[3]);

            
            DateTime maxDate = minDate.Date.AddDays(1).AddSeconds(-1);

            var attendance = _Entities.TbAttendances
                .Where(z =>
                    z.ClassId == classId &&
                    z.DivisionId == divisionId &&
                    z.AttendanceDate >= minDate &&
                    z.AttendanceDate <= maxDate &&
                    z.ShiftStatus == shift)
                .OrderBy(x => x.Student.StundentName)
                .ToList();

            var model = new AttendanceModels
            {
                classId = classId,
                divisionId = divisionId,
                shift = shift,
                minDate = minDate,
                maxDate = maxDate,
                IsAdminUser = HttpContext.Session.GetString("IsAdmin") == "true"
            };

            if (!attendance.Any())
            {
                return PartialView("~/Views/Teacher/_pv_AttendanceMark_Grid.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Teacher/_pv_AttendanceView_Grid.cshtml", model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Attendance(SchoolModel model)
        {
            bool status = false;
            string msg = "Failed";

            long teacherId = model.teacherId;
            long classId = model.classId;
            long divisionId = model.divisionId;
            int shiftStatus = model.shiftStatus;
            DateTime attendanceDate = Convert.ToDateTime(model.attendanceDate);

            try
            {
                
                try
                {
                    await _Entities.Database.ExecuteSqlRawAsync(
                        "EXEC SP_DeleteOldAttendance @p0, @p1, @p2, @p3, @p4",
                        attendanceDate, _user.UserId, classId, divisionId, shiftStatus
                    );
                }
                catch
                {
                }

                
                foreach (var item in model.studentList)
                {
                    var attendance = new TbAttendance
                    {
                        AttendanceGuid = Guid.NewGuid(),
                        StaffId = _user.UserId,
                        ClassId = classId,
                        DivisionId = divisionId,
                        AttendanceDate = attendanceDate,
                        AttendanceData = item.attendaneStatus != "False",
                        IsActive = true,
                        TimeStamp = DateTime.UtcNow,
                        StudentId = item.StudentId,
                        ShiftStatus = shiftStatus
                    };

                    await _Entities.TbAttendances.AddAsync(attendance);
                }

                status = await _Entities.SaveChangesAsync() > 0;

                
                if (model.IsSendSMS)
                {
                    var absentStudents = model.studentList
                        .Where(z => z.attendaneStatus == "False")
                        .ToList();

                    if (absentStudents.Count > 0)
                    {
                        
                        Task.Run(() => SendSMSForAttendance(absentStudents, attendanceDate, divisionId));
                    }
                }

                msg = status ? "Success" : "Failed";
                return Json(new { status, msg });
            }
            catch
            {
                return Json(new { status, msg });
            }
        }

        public bool SendSMSForAttendance(List<Student> list, DateTime attendanceDate, long divisionId)
        {
            try
            {
                var classDetails = new Division(divisionId);
                long headId = 0;
                bool status = false;
                string msg = "Failed";
                var smsHead = new TbSmsHead();
                smsHead.Head = "Attendance " + CurrentTime.ToString("dd-MM-yyyy") + " of " + classDetails.Class.ClassName + " - " + classDetails.DivisionName;
                smsHead.SchoolId = _user.SchoolId;
                smsHead.TimeStamp = CurrentTime;
                smsHead.IsActive = true;
                smsHead.SenderType = 0;
                _Entities.TbSmsHeads.Add(smsHead);
                status = _Entities.SaveChanges() > 0;
                headId = smsHead.HeadId;
                foreach (var item in list)
                {
                    long studentId = Convert.ToInt64(item.studentId);
                    var studentList = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
                    if (studentList.ContactNumber != null)
                    {
                        if (studentList.ContactNumber != string.Empty)
                        {

                            SendSMS(studentList.ContactNumber, studentList.StundentName, false, attendanceDate, studentList.StudentId, headId);
                            string message = "Dear Parent,  " + studentList.StundentName + " is absent today (" + attendanceDate.ToString("dd/MM/yyyy") + ") - " + _user.Name;
                            //----------------Send Email
                            SendStudentEmail(message, studentList.ParentEmail, "Attendance Details", studentList.School.SchoolName);// Send EMAIL for student attendance
                            SendStudentPush(studentList, message);

                        }
                    }
                }
            }
            catch
            {

            }
            return true;

        }
        private void SendStudentPush(TbStudent studentDetails, string message)
        {
            try
            {
                string schoolName = "Message from " + studentDetails.School.SchoolName;
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
                            body = message,
                            title = schoolName
                        },
                        priority = "high",
                        data = new
                        {
                            Role = "Teacher",
                            Function = "AttendanceData"
                        },
                        from = "School"
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
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

        private void SendStudentEmail(string message, string parentEmail, string subject, string schoolName)
        {
            var filePath = Path.Combine( _env.ContentRootPath,"Content","template","BirthdayWishesEmail.html");
            var emailTemplate = System.IO.File.ReadAllText(filePath);
            var mBody = emailTemplate.Replace("{{resetLink}}", message).Replace("{{resetLink1}}", schoolName);
            bool sendMail = Send(subject, mBody, parentEmail);
        }
        private bool Send(string subject, string mailbody, string email)
        {
            try
            {
                //MailMessage msg = new MailMessage();
                //msg.Subject = subject;
                //msg.Body = mailbody;
                //msg.From = new MailAddress("info.schoolman@gmail.com");
                //msg.To.Add(new MailAddress(email));
                //msg.IsBodyHtml = true;
                //SmtpClient client = new SmtpClient();
                //client.Host = "k2smtp.gmail.com";
                //NetworkCredential basicauthenticationinfo = new NetworkCredential("info.schoolman@gmail.com", "Info@123");
                //client.Port = int.Parse("587");//25//465
                //client.EnableSsl = true;
                //client.UseDefaultCredentials = false;
                //client.Credentials = basicauthenticationinfo;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                //        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                //        System.Security.Cryptography.X509Certificates.X509Chain chain,
                //        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                //{
                //    return true;
                //};
                //try
                //{
                //    client.Send(msg);
                //    client.Dispose();
                //}
                //catch (Exception ex)
                //{
                //}
                try
                {
                    MailMessage msg = new MailMessage();
                    msg.Subject = subject;
                    msg.Body = mailbody;
                    msg.From = new MailAddress("schoolman@srishtis.com");
                    msg.To.Add(new MailAddress(email, "Dear"));
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.Host = "k2smtpout.secureserver.net";
                    System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("schoolman@srishtis.com", "ca@12345");
                    client.Port = int.Parse("25");
                    client.EnableSsl = false;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicauthenticationinfo;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                }
                catch (Exception ex) { }
                return true;

            }
            catch (Exception ex)
            {

            }
            return true;
        }

        public bool SendSMS(string phone, string student, bool status, DateTime attendanceDate, long studentId, long headId)
        {
            var school = _Entities.TbSchools.Where(z => z.SchoolId == _user.SchoolId).FirstOrDefault();
            if (school.SmsActive)
            {
                var package = _Entities.TbSmsPackages.Where(z => z.SchoolId == _user.SchoolId && z.IsActive && z.IsDisabled == false).FirstOrDefault();
                if (package != null)
                {
                    if (package.ToDate >= CurrentTime)
                    {
                        //     ---------------------------------

                        string message = "";
                        if (status)
                        {
                            //  message = "Dear Parent,  " + student + " is present today (" + attendanceDate.ToString("dd/MM/yyyy") + ") - " + _user.Name;
                        }
                        else
                        {
                            message = "Dear Parent,  " + student + " is absent today (" + attendanceDate.ToString("dd/MM/yyyy") + ") - " + _user.Name;
                        }
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
                        var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + message + "&sender=" + senderName;

                        // var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + message + "&priority=ndnd&stype=normal";
                        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                        HttpWebRequest request = this.GetRequest(url);
                        WebResponse webResponse = request.GetResponse();
                        var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                        var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                        alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
                        if (responseText != null)
                        {
                            try
                            {
                                if (headId != 0)
                                {
                                    TbSmsHistory sms = new TbSmsHistory();
                                    sms.IsActive = true;
                                    sms.MessageContent = message;
                                    sms.SendStatus = Convert.ToString(respList.success);
                                    sms.MessageDate = CurrentTime;
                                    sms.ScholId = _user.SchoolId;
                                    var xx = message.Length;
                                    sms.IsActive = true;
                                    sms.MobileNumber = phone;
                                    decimal count = message.Length / 160; // 22-11-2018 Shinu
                                    if ((count % 1) > 0)
                                    {
                                        sms.SmsSentPerStudent = Convert.ToInt32(count) + 1;
                                    }
                                    else
                                    {
                                        sms.SmsSentPerStudent = Convert.ToInt32(count);
                                    }
                                    sms.StuentId = studentId;
                                    sms.HeadId = headId;
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
                            return true;
                        }
                        else
                            return false;


                        //       --------------------------------------

                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;



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
        public IActionResult MyTimeTable()
        {
            SchoolModel model = new SchoolModel();
            model.schoolId = _user.SchoolId;
            model.userId = _user.UserId;
            return View(model);
        }
        public IActionResult Departments()
        {
            DepartmentsModel model = new DepartmentsModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public async Task<IActionResult> AddDepartment(DepartmentsModel model)
        {
            bool status = false;
            string msg = "Failed";
            var schoolId = _user.SchoolId;
            if (await _Entities.TbDepartments.AnyAsync(x => x.SchoolId == schoolId && x.IsActive && x.DepartmentName.Trim().ToLower() == model.DepartmentName.Trim().ToLower()))
            {
                msg = "The Department name is already exists";
            }
            //else if (_Entities.tb_Department.Any(x => x.SchoolId == schoolId & x.Abbreviation.Trim().ToLower() == model.Abbreviation.Trim().ToLower()))
            //{
            //    msg = "The Abbreviation is already exists";
            //}
            else
            {
                var dep =new TbDepartment();
                dep.SchoolId = schoolId;
                dep.DepartmentName = model.DepartmentName;
                //dep.Abbreviation = model.Abbreviation;
                dep.Abbreviation = "";
                dep.IsActive = true;
                dep.TimeStamp = CurrentTime;
                await _Entities.TbDepartments.AddAsync(dep);
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }

        public IActionResult Designation()
        {
            DesignationModel model = new DesignationModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public async Task<IActionResult> AddDesignation(DesignationModel model)
        {
            bool status = false;
            string msg = "Failed";
            var schoolId = _user.SchoolId;
            if (await _Entities.TbDesignations.AnyAsync(x => x.SchoolId == schoolId && x.IsActive && x.DesignationName.Trim().ToLower() == model.DesignationName.Trim().ToLower()))
            {
                msg = "The Designation name is already exists";
            }
            //else if (_Entities.tb_Designation.Any(x => x.SchoolId == schoolId & x.DesignationCode.Trim().ToLower() == model.DesignationCode.Trim().ToLower()))
            //{
            //    msg = "The DesignationCode is already exists";
            //}
            else
            {
                Random rnd = new Random();
                var code = "CD" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
                var dep = new TbDesignation();
                dep.SchoolId = schoolId;
                dep.DesignationName = model.DesignationName;
                //dep.DesignationCode = model.DesignationCode;
                dep.DesignationCode = code;
                dep.IsActive = true;
                dep.TimeStamp = CurrentTime;
                await _Entities.TbDesignations.AddAsync(dep);
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }
        public IActionResult RoleDetails()
        {
            RoleDetailsModel model = new RoleDetailsModel();
            model.SchoolId = _user.SchoolId;
            model.IsAdmin = true;
            model.RolesList = _dropdown.GetAllRoles(_user.SchoolId);
            return View(model);
        }
        public IActionResult LoadRoleList()
        {
            long schoolId = _user.SchoolId;

            RoleDetailsModel model = new RoleDetailsModel();
            model.RolesList = _dropdown.GetAllRoles(_user.SchoolId);
            model.IsAdmin = true;

            return PartialView("_pv_RoleList", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddRoleDetails(RoleDetailsModel model)
        {
            bool status = false;
            string msg = "Failed";
            var schoolId = _user.SchoolId;
            if (await _Entities.TbRoleDetails.AnyAsync(x => x.SchoolId == schoolId & x.RoleName.Trim().ToLower() == model.RoleName.Trim().ToLower()))
            {
                msg = "The Role is already exists";
            }
            else
            {
                var dep = new TbRoleDetail();
                dep.SchoolId = schoolId;
                dep.RoleName = model.RoleName;
                dep.IsActive = true;
                dep.TimeStamp = CurrentTime;
                await _Entities.TbRoleDetails.AddAsync(dep);
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }

        public async Task<IActionResult> DeleteDesignation(string id)
        {
            bool status = false;
            string msg = "Failed";
            long desId = Convert.ToInt64(id);
            if (await _Entities.TbTeachers.AnyAsync(x => x.SchoolId == _user.SchoolId && x.IsActive && x.DesignationId == desId))
            {
                msg = "Some teachers have given this designation";
            }
            else if (await _Entities.TbStaffs.AnyAsync(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.DesignationId == desId))
            {
                msg = "Some Staffs have given this designation";
            }
            else
            {
                var data = await _Entities.TbDesignations.Where(x => x.Id == desId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefaultAsync();
                data.IsActive = false;
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }

        public async Task<IActionResult> DeleteDesignationWithTeacherOrStaff(string id)
        {
            bool status = false;
            string msg = "Failed";
            long desId = Convert.ToInt64(id);
            var teachers = await _Entities.TbTeachers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.DesignationId == desId).ToListAsync();
            if (teachers.Count > 0 && teachers != null)
            {
                foreach (var item in teachers)
                {
                    var teach =await _Entities.TbTeachers.Where(x => x.TeacherId == item.TeacherId).FirstOrDefaultAsync();
                    teach.DesignationId = null;
                    await _Entities.SaveChangesAsync();
                }
            }
            var staffs = await _Entities.TbStaffs.Where(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.DesignationId == desId).ToListAsync();
            if (staffs.Count > 0 && staffs != null)
            {
                foreach (var item in staffs)
                {
                    var staff = await _Entities.TbStaffs.Where(x => x.StaffId == item.StaffId).FirstOrDefaultAsync();
                    staff.DesignationId = null;
                    await _Entities.SaveChangesAsync();
                }
            }
            var data = await _Entities.TbDesignations.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.Id == desId).FirstOrDefaultAsync();
            data.IsActive = false;
            status = await _Entities.SaveChangesAsync() > 0;
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult DesignationEditPartial(string id)
        {
            DesignationModel model = new DesignationModel();
            long desId = Convert.ToInt64(id);
            var data = _Entities.TbDesignations.Where(x => x.Id == desId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.DesignationName = data.DesignationName;
            }
            return PartialView("~/Views/Teacher/_pv_DesignationEdit.cshtml", model);
        }
        public async Task<IActionResult> EditDesignation(DesignationModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (await _Entities.TbDesignations.AnyAsync(x => x.SchoolId == _user.SchoolId && x.DesignationName.Trim().ToUpper() == model.DesignationName.Trim().ToUpper() && x.Id != model.Id))
            {
                msg = "Already we have the same Designation !";
            }
            else
            {
                var data = await _Entities.TbDesignations.Where(x => x.Id == model.Id && x.IsActive).FirstOrDefaultAsync();
                if (data.DesignationName.ToUpper().Trim() != model.DesignationName.Trim().ToUpper())
                {
                    data.DesignationName = model.DesignationName.Trim().ToUpper();
                    status = await _Entities.SaveChangesAsync() > 0;
                    msg = "Successful";
                }
                else
                {
                    msg = "Successful";
                    status = true;
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            bool status = false;
            string msg = "Failed";
            long depId = Convert.ToInt64(id);
            if (await _Entities.TbTeachers.AnyAsync(x => x.SchoolId == _user.SchoolId && x.IsActive && x.DepartmentId == depId))
            {
                msg = "Some teachers have given this department";
            }
            else if (await _Entities.TbStaffs.AnyAsync(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.DepartmentId == depId))
            {
                msg = "Some Staffs have given this department";
            }
            else
            {
                var data = await _Entities.TbDepartments.Where(x => x.Id == depId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefaultAsync();
                data.IsActive = false;
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }
        public async Task<IActionResult> DeleteDepartmentWithTeacherOrStaff(string id)
        {
            bool status = false;
            string msg = "Failed";
            long depId = Convert.ToInt64(id);
            var teachers = await _Entities.TbTeachers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.DepartmentId == depId).ToListAsync();
            if (teachers.Count > 0 && teachers != null)
            {
                foreach (var item in teachers)
                {
                    var teach = await _Entities.TbTeachers.Where(x => x.TeacherId == item.TeacherId).FirstOrDefaultAsync();
                    teach.DepartmentId = null;
                    await _Entities.SaveChangesAsync();
                }
            }
            var staffs =await _Entities.TbStaffs.Where(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.DepartmentId == depId).ToListAsync();
            if (staffs.Count > 0 && staffs != null)
            {
                foreach (var item in staffs)
                {
                    var staff =await  _Entities.TbStaffs.Where(x => x.StaffId == item.StaffId).FirstOrDefaultAsync();
                    staff.DepartmentId = null;
                    await _Entities.SaveChangesAsync();
                }
            }
            var data = await _Entities.TbDepartments.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.Id == depId).FirstOrDefaultAsync();
            data.IsActive = false;
            status = await _Entities.SaveChangesAsync() > 0;
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult DepartmentEditPartial(string id)
        {
            DepartmentsModel model = new DepartmentsModel();
            long desId = Convert.ToInt64(id);
            var data = _Entities.TbDepartments.Where(x => x.Id == desId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                model.DepartmentId = data.Id;
                model.DepartmentName = data.DepartmentName;
            }
            return PartialView("~/Views/Teacher/_pv_DepartmentEdit.cshtml", model);
        }

        public async Task<IActionResult> EditDepartment(DepartmentsModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (await _Entities.TbDepartments.AnyAsync(x => x.SchoolId == _user.SchoolId && x.DepartmentName.Trim().ToUpper() == model.DepartmentName.Trim().ToUpper() && x.Id != model.DepartmentId))
            {
                msg = "Already we have the same Department !";
            }
            else
            {
                var data =await  _Entities.TbDepartments.Where(x => x.Id == model.DepartmentId && x.IsActive).FirstOrDefaultAsync();
                if (data.DepartmentName.ToUpper().Trim() != model.DepartmentName.Trim().ToUpper())
                {
                    data.DepartmentName = model.DepartmentName.Trim().ToUpper();
                    status = await _Entities.SaveChangesAsync() > 0;
                    msg = "Successful";
                }
                else
                {
                    msg = "Successful";
                    status = true;
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public async Task<IActionResult> DeleteRoleDetails(string id)
        {
            bool status = false;
            string msg = "Failed";
            long roleId = Convert.ToInt64(id);
            if (await _Entities.TbUserRoles.AnyAsync(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.RoleId == roleId))
            {
                msg = "Some teachers or staffs have  this roles";
            }
            else
            {
                var data = await _Entities.TbRoleDetails.Where(x => x.Id == roleId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefaultAsync();
                data.IsActive = false;
                status = await _Entities.SaveChangesAsync() > 0;
                if (status)
                    msg = "Successful";
            }
            return Json(new { status = status, msg = msg });
        }

        public async Task<IActionResult> DeleteRoleWithTeacherOrStaff(string id)
        {
            bool status = false;
            string msg = "Failed";
            long roleId = Convert.ToInt64(id);
            var details =await _Entities.TbUserRoles.Where(x => x.User.SchoolId == _user.SchoolId && x.IsActive && x.RoleId == roleId).ToListAsync();
            if (details.Count > 0 && details != null)
            {
                foreach (var item in details)
                {
                    var one =await  _Entities.TbUserRoles.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                    one.IsActive = false;
                    await _Entities.SaveChangesAsync();
                }
            }
            var data = await _Entities.TbRoleDetails.Where(x => x.IsActive && x.SchoolId == _user.SchoolId && x.Id == roleId).FirstOrDefaultAsync();
            data.IsActive = false;
            status = await _Entities.SaveChangesAsync() > 0;
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult RoleDetailsEditPartial(string id)
        {
            RoleDetailsModel model = new RoleDetailsModel();
            long roleId = Convert.ToInt64(id);
            var data = _Entities.TbRoleDetails.Where(x => x.Id == roleId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (data != null)
            {
                model.Id = data.Id;
                model.RoleName = data.RoleName;
            }
            return PartialView("~/Views/Teacher/_pv_RoleDetailsEdit.cshtml", model);
        }

        public async Task<IActionResult> EditRoleDetails(RoleDetailsModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (await _Entities.TbRoleDetails.AnyAsync(x => x.SchoolId == _user.SchoolId && x.RoleName.Trim().ToUpper() == model.RoleName.Trim().ToUpper() && x.Id != model.Id))
            {
                msg = "Already we have the same Role !";
            }
            else
            {
                var data = await _Entities.TbRoleDetails.Where(x => x.Id == model.Id && x.IsActive).FirstOrDefaultAsync();
                if (data.RoleName.ToUpper().Trim() != model.RoleName.Trim().ToUpper())
                {
                    data.RoleName = model.RoleName.Trim().ToUpper();
                    status = _Entities.SaveChanges() > 0;
                    msg = "Successful";
                }
                else
                {
                    msg = "Successful";
                    status = true;
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public IActionResult AttandanceSummaryReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime date = Convert.ToDateTime(splitDate[0]);
            long section = Convert.ToInt64(splitDate[1]);
            long classId = Convert.ToInt64(splitDate[2]);
            long div = Convert.ToInt64(splitDate[3]);

            string Maxdate = date.Date.ToString("MM-dd-yyyy") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(Maxdate);
            AttendanceSummaryReportModel model = new AttendanceSummaryReportModel();
            var atndance = _Entities.TbAttendances.Where(z => z.ClassId == classId && z.DivisionId == div && z.AttendanceDate >= date && z.AttendanceDate <= date && z.ShiftStatus == section).OrderBy(x => x.Student.StundentName).ToList();
            model.SchoolName = _user.School.SchoolName;
            model.SchoolAddress = _user.School.Address;
            model.SchoolLogo = _user.School.FilePath;
            model.AttendanceDate = date;
            model.ClassName = _user.School.TbClasses.Where(x => x.ClassId == classId && x.IsActive).Select(x => x.Class).FirstOrDefault();
            model.DivisionName = _Entities.TbDivisions.Where(x => x.DivisionId == div && x.IsActive).Select(x => x.Division).FirstOrDefault();
            var students = _Entities.TbStudents.Where(x => x.SchoolId == _user.SchoolId && x.IsActive && x.ClassId == classId && x.DivisionId == div).ToList();
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
            var teacher = _Entities.TbTeacherClasses.Where(x => x.ClassId == classId && x.DivisionId == div && x.Teacher.IsActive).FirstOrDefault();
            if (teacher != null)
                model.InCharge = teacher.Teacher.TeacherName;
            else
                model.InCharge = "";
            return View(model);
        }
        public IActionResult Periods()
        {
            AcademicPeriodsModelClass model = new AcademicPeriodsModelClass();
            model.SchoolId = _user.SchoolId;
            ViewBag.Regionlist = _dropdown.GetRegion(model.SchoolId);
            return View(model);
        }
        public async Task<IActionResult> AddAcademicPeriods(AcademicPeriodsModelClass model)
        {
            bool status = false;
            string msg = "Failed";
            if (model.PeriodsName.Trim() == string.Empty)
            {
                msg = "Please enter the Periods Name";
            }
            else
            {
                if (model.ClassId == 0)
                {
                    var thisRegionClass = await _Entities.TbAcademicPeriods.Where(x => x.IsActive == true && x.SchoolId == model.SchoolId && x.Class.IsActive == true && x.Class.PublishStatus == true && x.RegionId == model.RegionId && x.PeriodsName.ToUpper().Trim() == model.PeriodsName.ToUpper().Trim()).ToListAsync();
                    if (thisRegionClass != null && thisRegionClass.Count > 0)
                    {
                        msg = "Already exists this Period!";
                    }
                    else
                    {
                        var classList =await  _Entities.TbRegionsClasses.Where(x => x.RegionId == model.RegionId && x.IsActive == true && x.Class.IsActive == true && x.Class.PublishStatus == true).ToListAsync();
                        foreach (var item in classList)
                        {
                            var newData = new TbAcademicPeriod();
                           
                            newData.RegionId = model.RegionId;
                            newData.ClassId = item.ClassId;
                            newData.PeriodsName = model.PeriodsName;
                            newData.SchoolId = _user.SchoolId;
                            if (model.StartDatestring != string.Empty && model.StartDatestring != null)
                            {
                                string[] splitData = model.StartDatestring.Split('-');
                                var dd = splitData[0];
                                var mm = splitData[1];
                                var yyyy = splitData[2];
                                var date = mm + '-' + dd + '-' + yyyy;
                                newData.StartDate = Convert.ToDateTime(date);
                            }
                            if (model.EndDatestring != string.Empty && model.EndDatestring != null)
                            {
                                string[] splitData = model.EndDatestring.Split('-');
                                var dd = splitData[0];
                                var mm = splitData[1];
                                var yyyy = splitData[2];
                                var date = mm + '-' + dd + '-' + yyyy;
                                newData.EndDate = Convert.ToDateTime(date);
                            }
                            newData.IsActive = true;
                            newData.TimeStamp = CurrentTime;
                            await _Entities.TbAcademicPeriods.AddAsync(newData);
                            try
                            {
                                status = await _Entities.SaveChangesAsync() > 0;
                            }
                            catch (Exception ex)
                            {

                            }
                            if (status)
                                msg = "Successful";
                        }
                    }
                }
                else
                {
                    var oldData =await _Entities.TbAcademicPeriods.Where(x => x.ClassId == model.ClassId && x.IsActive == true && x.PeriodsName.ToUpper().Trim() == model.PeriodsName.ToUpper().Trim()).FirstOrDefaultAsync();
                    if (oldData != null)
                    {
                        msg = "Already exists this Period!";
                    }
                    else
                    {
                        var newData =new TbAcademicPeriod();
                        newData.RegionId = model.RegionId;
                        newData.ClassId = model.ClassId;
                        newData.PeriodsName = model.PeriodsName;
                        newData.SchoolId = _user.SchoolId;
                        if (model.StartDatestring != string.Empty && model.StartDatestring != null)
                        {
                            string[] splitData = model.StartDatestring.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            newData.StartDate = Convert.ToDateTime(date);
                        }
                        if (model.EndDatestring != string.Empty && model.EndDatestring != null)
                        {
                            string[] splitData = model.EndDatestring.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            newData.EndDate = Convert.ToDateTime(date);
                        }
                        newData.IsActive = true;
                        newData.TimeStamp = CurrentTime;
                        await _Entities.TbAcademicPeriods.AddAsync(newData);
                        status = await _Entities.SaveChangesAsync() > 0;
                        if (status)
                            msg = "Successful";
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult ListAcademicPeriodsData()
        {
            AcademicPeriodsModelClass model = new AcademicPeriodsModelClass();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Teacher/_pv_AcademicPeriodsList.cshtml", model);
        }
        public IActionResult Health()
        {
            HealthModel model = new HealthModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.classlist = _dropdown.GetClasses(model.SchoolId);
            return View(model);
        }

        public PartialViewResult SearchStudentsForHealth(string id)
        {
            string[] split = id.Split('~');
            HealthResultModel model = new HealthResultModel();
            model.SchoolId = _user.SchoolId;
            model.ClassId = Convert.ToInt64(split[0]);
            model.DivisionId = Convert.ToInt64(split[1]);
            model.PeriodId = Convert.ToInt64(split[2]);
            model._StudentListForHealth = new List<StudentListForHealth>();
            var data = _Entities.TbHealths.Where(x => x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.PeriodsId == model.PeriodId && x.IsActive).ToList();
            var attendanceData = _Entities.TbAttendancePeriods.Where(x => x.DivisionId == model.DivisionId && x.PeriodId == model.PeriodId && x.IsActive).ToList();
            var students = _Entities.TbStudents.Where(x => x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.IsActive).ToList();
            if (data != null && data.Count > 0)// Already healthe data inserted
            {
                if (data.Count == students.Count)// Full students data uploaded 
                {
                    foreach (var item in data)
                    {
                        StudentListForHealth one = new StudentListForHealth();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.Student.StundentName;
                        one.Height = item.Height;
                        one.Weight = item.Weight;
                        one.RollNo = int.TryParse(item.Student.ClasssNumber, out var rn) ? rn : 0;
                        //one.RollNo = item.tb_Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.tb_Student.ClasssNumber);
                        if (attendanceData != null && attendanceData.Count > 0)
                        {
                            var att = attendanceData.Where(x => x.StudentId == item.StudentId && x.DivisionId == model.DivisionId).FirstOrDefault();
                            if (att != null)
                            {
                                one.WorkingDays = att.TotalDays == null ? 0 : att.TotalDays;
                                one.PresentDays = att.PresentDays == null ? 0 : att.PresentDays;
                            }
                            else
                            {
                                one.WorkingDays = 0;
                                one.PresentDays = 0;
                            }
                        }
                        else
                        {
                            one.WorkingDays = 0;
                            one.PresentDays = 0;
                        }
                        model._StudentListForHealth.Add(one);
                    }
                }
                else// Some students data missing 
                {
                    foreach (var item in data)
                    {
                        StudentListForHealth one = new StudentListForHealth();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.Student.StundentName;
                        one.Height = item.Height;
                        one.Weight = item.Weight;
                        one.RollNo = int.TryParse(item.Student.ClasssNumber, out var rn) ? rn : 0;
                        //one.RollNo = item.tb_Student.ClasssNumber == null ? 0 : Convert.ToInt32(item.tb_Student.ClasssNumber);
                        if (attendanceData != null && attendanceData.Count > 0)
                        {
                            var att = attendanceData.Where(x => x.StudentId == item.StudentId && x.DivisionId == model.DivisionId).FirstOrDefault();
                            if (att != null)
                            {
                                one.WorkingDays = att.TotalDays == null ? 0 : att.TotalDays;
                                one.PresentDays = att.PresentDays == null ? 0 : att.PresentDays;
                            }
                            else
                            {
                                one.WorkingDays = 0;
                                one.PresentDays = 0;
                            }
                        }
                        else
                        {
                            one.WorkingDays = 0;
                            one.PresentDays = 0;
                        }
                        model._StudentListForHealth.Add(one);
                    }
                    var balanceStudents = students.Where(x => !data.Any(y => y.StudentId == x.StudentId)).ToList();
                    foreach (var item in balanceStudents)
                    {
                        StudentListForHealth one = new StudentListForHealth();
                        one.StudentId = item.StudentId;
                        one.StudentName = item.StundentName;
                        one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;
                        //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                        if (attendanceData != null && attendanceData.Count > 0)
                        {
                            var att = attendanceData.Where(x => x.StudentId == item.StudentId && x.DivisionId == model.DivisionId).FirstOrDefault();
                            if (att != null)
                            {
                                one.WorkingDays = att.TotalDays == null ? 0 : att.TotalDays;
                                one.PresentDays = att.PresentDays == null ? 0 : att.PresentDays;
                            }
                            else
                            {
                                one.WorkingDays = 0;
                                one.PresentDays = 0;
                            }
                        }
                        else
                        {
                            one.WorkingDays = 0;
                            one.PresentDays = 0;
                        }
                        model._StudentListForHealth.Add(one);
                    }
                }
            }
            else // No data inserted  
            {
                foreach (var item in students)
                {
                    StudentListForHealth one = new StudentListForHealth();
                    one.StudentId = item.StudentId;
                    one.StudentName = item.StundentName;
                    one.RollNo = int.TryParse(item.ClasssNumber, out var rn) ? rn : 0;

                    //one.RollNo = item.ClasssNumber == null ? 0 : Convert.ToInt32(item.ClasssNumber);
                    if (attendanceData != null && attendanceData.Count > 0)
                    {
                        var att = attendanceData.Where(x => x.StudentId == item.StudentId && x.DivisionId == model.DivisionId).FirstOrDefault();
                        if (att != null)
                        {
                            one.WorkingDays = att.TotalDays == null ? 0 : att.TotalDays;
                            one.PresentDays = att.PresentDays == null ? 0 : att.PresentDays;
                        }
                        else
                        {
                            one.WorkingDays = 0;
                            one.PresentDays = 0;
                        }
                    }
                    else
                    {
                        one.WorkingDays = 0;
                        one.PresentDays = 0;
                    }
                    model._StudentListForHealth.Add(one);
                }
            }
            model._StudentListForHealth = model._StudentListForHealth.OrderBy(x => x.RollNo).ThenBy(x => x.StudentName).ToList();
            return PartialView("~/Views/Teacher/_pv_StudentsHealthResult.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStudentsHealthDetails(HealthResultModel model)
        {
            bool status = false;
            string msg = "Failed";

            foreach (var item in model._StudentListForHealth)
            {
                if (item.Height != 0 || item.Weight != 0)
                {
                    var oldData = await _Entities.TbHealths.Where(x => x.ClassId == model.ClassId && x.DivisionId == model.DivisionId && x.PeriodsId == model.PeriodId && x.IsActive && x.StudentId == item.StudentId).FirstOrDefaultAsync();
                    if (oldData != null)
                    {
                        oldData.Height = item.Height;
                        oldData.Weight = item.Weight;
                        oldData.TimeStamp = CurrentTime;
                        status = await _Entities.SaveChangesAsync() > 0;
                    }
                    else
                    {
                        var newHealth = new TbHealth()
                        {
                            ClassId = model.ClassId,
                            DivisionId = model.DivisionId,
                            PeriodsId = model.PeriodId,
                            StudentId = item.StudentId,
                            Height = item.Height,
                            Weight = item.Weight,
                            IsActive = true,
                            TimeStamp = CurrentTime,
                        };
                       
                        await _Entities.TbHealths.AddAsync(newHealth);
                        status = await _Entities.SaveChangesAsync() > 0;
                    }
                }
                //if (item.WorkingDays != 0 || item.PresentDays != 0)
                {
                    var oldData = await _Entities.TbAttendancePeriods.Where(x => x.DivisionId == model.DivisionId && x.PeriodId == model.PeriodId && x.IsActive && x.StudentId == item.StudentId).FirstOrDefaultAsync();
                    if (oldData != null)
                    {
                        oldData.TotalDays = item.WorkingDays;
                        oldData.PresentDays = item.PresentDays;
                        oldData.TimeStampp = CurrentTime;
                        status = await _Entities.SaveChangesAsync() > 0;
                    }
                    else
                    {
                        if (item.WorkingDays != 0 || item.PresentDays != 0)
                        {
                            var newAttendance = new TbAttendancePeriod()
                            {
                                SchoolId = _user.SchoolId,
                                StudentId = item.StudentId,
                                DivisionId = model.DivisionId,
                                PeriodId = model.PeriodId,
                                UserId = _user.UserId,
                                TotalDays = item.WorkingDays,
                                PresentDays = item.PresentDays,
                                IsActive = true,
                                TimeStampp = CurrentTime
                            };
                            
                            await _Entities.TbAttendancePeriods.AddAsync(newAttendance);
                            status = await _Entities.SaveChangesAsync() > 0;
                        }
                    }
                }
            }
            if (status)
                msg = "Successful";
            return Json(new { status = status, msg = msg });
        }








    }
}