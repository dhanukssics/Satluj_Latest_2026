using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Utility;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.PostModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Satluj_Latest.PostModel.TeacherAttendancePostModel;
using static System.Net.Mime.MediaTypeNames;

namespace Satluj_Latest.Repository
{
    public class TeacherRepository
    {
        private readonly SchoolDbContext _Entity;
        private readonly IWebHostEnvironment _env;
        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        public DateTime currentTime = DateTime.UtcNow;
        public DateTime CurrentTime = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public TeacherRepository(IWebHostEnvironment env)
        {
            _env = env;
        }
        public Tuple<bool, string, TbTeacher> TeacherLogin(TeacherLoginPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            var teacherData = _Entity.TbTeachers.Where(x => x.TeacherSpecialId == model.teacherId && x.IsActive).FirstOrDefault();
            if (teacherData != null)
            {
                if (_Entity.TbTeacherClasses.Any(x => x.TeacherId == teacherData.TeacherId))
                {
                    msg = "Success";
                    status = true;
                    return new Tuple<bool, string, TbTeacher>(status, msg, teacherData);
                }
                else
                {
                    msg = "No Classes Assigned for This teacher!";
                    return new Tuple<bool, string, TbTeacher>(status, msg, null);
                }
            }
            else
            {
                msg = "Invalid Special Id";
                return new Tuple<bool, string, TbTeacher>(status, msg, null);
            }

        }

        public Tuple<bool, string> AttendanceWork(TeacherAttendancePostModel.TeacherAttendanceDataListPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long teacherId = Convert.ToInt64(model.teacherId);
            var user = _Entity.TbTeachers.Where(z => z.TeacherId == teacherId).FirstOrDefault();
            long classId = Convert.ToInt64(model.classId);
            long divisionId = Convert.ToInt64(model.divisionId);
            int shiftStatus = Convert.ToInt32(model.shiftstatus);
            DateTime attendancedate = Convert.ToDateTime(model.attendanceDateTime);
            //bool attendanceStatus = false;
            //if (Convert.ToInt32(model.shiftstatus) == 1)
            //    attendanceStatus = true;
            //List<tb_Attendance> oldData = _Entity.tb_Attendance.Where(x => EntityFunctions.TruncateTime(x.AttendanceDate) == attendancedate.Date && x.TeacherId == teacherId && x.ClassId == classId && x.DivisionId == divisionId && x.ShiftStatus == shiftStatus).ToList();
            //if (oldData.Count > 0)
            //{
            //    foreach (var item in oldData)
            //    {
            //        _Entity.tb_Attendance.Remove(item);
            //    }
            //    _Entity.SaveChanges();
            //}
            try
            {
                //_Entity.SP_DeleteOldAttendance(attendancedate, user.UserId, classId, divisionId, shiftStatus);

                _Entity.MonthlyAttendanceResult
        .FromSqlRaw("EXEC SP_DeleteOldAttendance {0}, {1}, {2},{3},{4}",
                    attendancedate, user.UserId, classId, divisionId, shiftStatus);
        

            }
            catch
            {

            }
            var fullStudentList = _Entity.TbStudents.Where(x => x.ClassId == classId && x.DivisionId == divisionId && x.IsActive).ToList();
            ////////var smsHead = new tb_SmsHead();
            ////////smsHead.Head = "Attendance " + DateTime.UtcNow.ToString("dd-MM-yyyy");
            ////////smsHead.SchoolId = fullStudentList[0].SchoolId;
            ////////smsHead.TimeStamp = DateTime.UtcNow;
            ////////smsHead.IsActive = true;
            ////////smsHead.SenderType = 0;
            ////////_Entity.tb_SmsHead.Add(smsHead);
            ////////status = _Entity.SaveChanges() > 0;
            //---------------------------------------------
            foreach (var item in fullStudentList)
            {
                long SchoolId = 0;
                try
                {
                     SchoolId = fullStudentList[0].SchoolId;
                }
                catch
                {
                    SchoolId = 0;
                }
                var attendance = new TbAttendance();
                //var attendance = _Entity.tb_Attendance.Create();
                attendance.AttendanceGuid = Guid.NewGuid();
                attendance.StaffId = user.UserId;
                attendance.ClassId = classId;
                attendance.DivisionId = divisionId;
                attendance.AttendanceDate = attendancedate;
                if (model.studentList.Any(x => long.Parse(x.studentId) == item.StudentId))
                    attendance.AttendanceData = true;
                else
                    attendance.AttendanceData = false;
                //attendance.AttendanceData = Convert.ToInt32(item.attendaneStatus) == 0 ? false : true;
                attendance.IsActive = true;
                attendance.TimeStamp = currentTime.Date; 
                //attendance.StudentId = Convert.ToInt64(item.studentId);
                attendance.StudentId = Convert.ToInt64(item.StudentId);
                attendance.ShiftStatus = shiftStatus;
                _Entity.TbAttendances.Add(attendance);
                status = _Entity.SaveChanges() > 0;
                ////////////if (attendance.AttendanceData != true)
                ////////////{
                ////////////    Thread Thread = new Thread(() => SendSMS(item.ContactNumber, item.StundentName, attendance.AttendanceData, attendance.AttendanceDate, item.StudentId, smsHead.HeadId, SchoolId, user));
                ////////////    Thread.Start();
                ////////////}
            }
            long schoolId = fullStudentList[0].SchoolId;
            //var student_list = fullStudentList.Where(z => z.attendaneStatus == "False").ToList();
            var student_list = fullStudentList.Where(z => !model.studentList.Any(x => x.studentId == z.StudentId.ToString())).ToList().Select(y => new Data.Student(y)).ToList();
            if (student_list != null)
            {
                Thread Thread = new Thread(() => SendSMSForAttendance(student_list, attendancedate, divisionId, schoolId, user));
                Thread.Start();
            }

            msg = status ? "Success" : "Failed";
            return new Tuple<bool, string>(status, msg);
        }

        private void SendSMSForAttendance(List<Satluj_Latest.Data.Student> list, DateTime attendancedate, long divisionId,long schoolId, TbTeacher user)
        {
            try
            {
                var classDetails = new Satluj_Latest.Data.Division(divisionId);
                long headId = 0;
                bool status = false;
                string msg = "Failed";
                var smsHead = new TbSmsHead();
                smsHead.Head = "Attendance " + CurrentTime.ToString("dd-MM-yyyy") + " of " + classDetails.Class.ClassName + " - " + classDetails.DivisionName;
                smsHead.SchoolId = schoolId;
                smsHead.TimeStamp = CurrentTime;
                smsHead.IsActive = true;
                smsHead.SenderType = 0;
                _Entity.TbSmsHeads.Add(smsHead);
                status = _Entity.SaveChanges() > 0;
                headId = smsHead.HeadId;
                foreach (var item in list)
                {
                    long studentId = Convert.ToInt64(item.StudentId);
                    var studentList = _Entity.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
                    if (studentList.ContactNumber != null)
                    {
                        if (studentList.ContactNumber != string.Empty)
                        {
                            SendSMS(studentList.ContactNumber, studentList.StundentName, false, attendancedate, studentList.StudentId, headId, schoolId,  user);
                            string message = "Dear Parent,  " + studentList.StundentName + " is absent today (" + attendancedate.ToString("dd/MM/yyyy") + ") - " + user.TeacherName;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public Tuple<bool, string, List<TbAttendance>> AttendanceDataList(TeacherAttendanceDataPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            List<TbAttendance> xxx = new List<TbAttendance>();
            long teacherId = Convert.ToInt64(model.teacherId);
            var user = _Entity.TbTeachers.Where(z => z.TeacherId == teacherId).FirstOrDefault();

            long classId = Convert.ToInt64(model.classId);
            long divisionId = Convert.ToInt64(model.divisionId);
            DateTime attendancedate = Convert.ToDateTime(model.date);
            var x1 = attendancedate.Date;
            var x11 = System.DateTime.UtcNow;
            int shiftStatus = Convert.ToInt32(model.shiftStatus);
            //List<Attendance> data = _Entity.SP_AttendanceData(user.UserId, classId, divisionId, shiftStatus, attendancedate).ToList().Select(x => new Attendance(x.AttendanceId)).ToList();
            List<TbAttendance> data = _Entity.spAttendances
        .FromSqlRaw("EXEC SP_AttendanceData {0}, {1}, {2},{3},{4}",
                    user.UserId, classId, divisionId, shiftStatus, attendancedate)
        .ToList();

            if (data.Count > 0)
            {
                status = true;
                msg = "Success";
            }
            else
            {
                status = false;
                msg = "No Attendance Data";
            }
            //List<tb_Attendance> asd = data.Convertto
            return new Tuple<bool, string, List<TbAttendance>>(status, msg, data);
        }

        public Tuple<bool, string, List<Data.Student>> StudentList(TeacherStudentListPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long teacherId = Convert.ToInt64(model.teacherId);
            long classId = Convert.ToInt64(model.classId);
            long divisionId = Convert.ToInt64(model.divisionId);

            var dataDetails = _Entity.TbTeacherClasses.Where(x => x.TeacherId == teacherId && x.ClassId == classId && x.DivisionId == divisionId).FirstOrDefault();
            var data = _Entity.TbStudents.Where(x => x.ClassId == classId && x.DivisionId == divisionId && x.IsActive).ToList().Select(z => new Data.Student(z)).ToList();
            if (data.Count > 0)
            {
                status = true;
                msg = "Success";
            }
            else
            {
                msg = " No students are currently assigned";
            }
            return new Tuple<bool, string, List<Data.Student>>(status, msg, data);
        }

        public Tuple<bool, string> SingleMessaging(TeacherSingleMessagingPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long teacherId = Convert.ToInt64(model.teacherId);
            long studentid = Convert.ToInt64(model.studentid);
            var singleMessage = new TbMessage(); 
            singleMessage.TeacherId = teacherId;
            singleMessage.StudentId = studentid;
            singleMessage.Subject = model.subject;
            singleMessage.Descrption = model.description;
            singleMessage.MessageType = true;
            singleMessage.IsActive = true;
            singleMessage.TimeStamp = CurrentTime;
            _Entity.TbMessages.Add(singleMessage);
            status = _Entity.SaveChanges() > 0;
            if (status)
            {
                msg = "Success";
                return new Tuple<bool, string>(status, msg);
            }
            else
                return new Tuple<bool, string>(status, msg);
        }

        public Tuple<bool, string> MultipleMessaging(TeacherMultipleMessagingPostModel.MultipleMessage model)
        {
            bool status = false;
            string msg = "Failed";
            long teacherId = Convert.ToInt64(model.teacherId);
            foreach (var item in model.studentList)
            {
                long studentId = Convert.ToInt64(item.studentId);
                var singleMessage = new TbMessage();
                singleMessage.TeacherId = teacherId;
                singleMessage.StudentId = studentId;
                singleMessage.Subject = model.subject;
                singleMessage.Descrption = model.description;
                singleMessage.MessageType = false;
                singleMessage.IsActive = true;
                singleMessage.TimeStamp = CurrentTime;
                _Entity.TbMessages.Add(singleMessage);
                status = _Entity.SaveChanges() > 0;
            }
            if (status)
            {
                msg = "Success";
                return new Tuple<bool, string>(status, msg);
            }
            else
                return new Tuple<bool, string>(status, msg);
        }

        public async Task<Tuple<bool, string>> sentmessageAsync(TeacherSentMessageModel model)
        {
            bool status = false;
            string msg = "Failed";
            long teacherId = Convert.ToInt64(model.TeacherId);
            long toSentId = Convert.ToInt64(model.ToSentId);

            string filepath = string.Empty;
            if (model.PostFile != null && model.PostFile.Length > 0)
            {
                //string folderPath = System.Web.HttpContext.Current.Server.MapPath("/Media/School/Message/");
                string folderPath = Path.Combine(_env.WebRootPath, "Media", "School", "Message");

                string fileExtension = Path.GetExtension(model.PostFile.FileName);
                string fileName = Guid.NewGuid().ToString();
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var fileSavePath = Path.Combine(folderPath, model.PostFile.FileName);
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await model.PostFile.CopyToAsync(stream);
                }

                //model.PostFile.CopyToAsync(fileSavePath);
                filepath = Path.Combine("/Media/School/Message/", model.PostFile.FileName);
            }

            var message = new TbAllMessage();
            message.TeacherId = teacherId;
            message.ToMsgSentId = toSentId;
            message.Subject = model.Subject;
            message.Description = model.Descritpion;
            message.Filepath = filepath;
            message.MessageType = (int)model.MessageType;
            message.IsActive = true;
            message.Timestamp = currentTime;
            _Entity.TbAllMessages.Add(message);
            status = _Entity.SaveChanges() > 0;
            if (status)
            {
                msg = "Success";
                return new Tuple<bool, string>(status, msg);
            }
            else
                return new Tuple<bool, string>(status, msg);
        }
        public Tuple<bool, string, List<ParentMessage>> GetMessage(TeacherMessagePostModel model)
        {
            bool status = false;
            string msg = "No Messages";
            long studentId = 0;
            long teacherId = Convert.ToInt64(model.teacherId);
            int indexVal = Convert.ToInt32(model.index);
            int lengthVal = Convert.ToInt32(model.length);
            if (model.studentId != "0")
                studentId = Convert.ToInt64(model.studentId);
            long divisionId = Convert.ToInt64(model.divisionId);
            var data = new List<ParentMessage>();
            if (studentId == 0)
            {
                data = _Entity.TbParentMessages.Where(x => x.Student.DivisionId == divisionId && x.IsActive == true).ToList().OrderByDescending(x => x.TimeStamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new ParentMessage(x)).ToList();
                if (data.Count > 0)
                {
                    status = true;
                    msg = "Success";
                }
                else
                {
                    status = true;
                    msg = "No messages";
                }
            }
            else
            {
                data = _Entity.TbParentMessages.Where(x => x.Student.DivisionId == divisionId && x.IsActive == true && x.StudentId == studentId).ToList().OrderByDescending(x => x.TimeStamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new ParentMessage(x)).ToList();
                if (data.Count > 0)
                {
                    status = true;
                    msg = "Success";
                }
                else
                {
                    status = true;
                    msg = "No messages";
                }
            }
            return new Tuple<bool, string, List<ParentMessage>>(status, msg, data);
        }

        public Tuple<bool, string, List<ParentMessage>> MessageRead(TeacherMessageReadPostModel model)
        {
            bool status = false;
            string msg = "Faile";
            long messageId = Convert.ToInt64(model.messageId);
            long teacherId = Convert.ToInt64(model.teacherId);
            int indexVal = Convert.ToInt32(model.index);
            int lengthVal = Convert.ToInt32(model.length);
            long studentId = Convert.ToInt64(model.studentId);
            long divisionId = Convert.ToInt64(model.divisionId);
            var dataa = _Entity.TbParentMessages.Where(x => x.MessageId == messageId && x.IsActive).FirstOrDefault();
            if (dataa != null)
            {
                dataa.ReadStatus = false;
                status = _Entity.SaveChanges() > 0;
            }
            var data = new List<ParentMessage>();
            if (studentId == 0)
            {
                data = _Entity.TbParentMessages.Where(x => x.Student.DivisionId == divisionId && x.IsActive == true).ToList().OrderByDescending(x => x.TimeStamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new ParentMessage(x)).ToList();
                if (data.Count > 0)
                {
                    status = true;
                    msg = "Success";
                }
                else
                {
                    status = true;
                    msg = "No messages";
                }
            }
            else
            {
                data = _Entity.TbParentMessages.Where(x => x.Student.DivisionId == divisionId && x.IsActive == true && x.StudentId == studentId).ToList().OrderByDescending(x => x.TimeStamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new ParentMessage(x)).ToList();
                if (data.Count > 0)
                {
                    status = true;
                    msg = "Success";
                }
                else
                {
                    status = true;
                    msg = "No messages";
                }
            }
            return new Tuple<bool, string, List<ParentMessage>>(status, msg, data);
        }
        public bool SendSMS(string phone, string student, bool status, DateTime attendanceDate, long studentId, long headId,long SchoolId,TbTeacher user)
        {
            string message = "";
            if (status)
            {
            }
            else
            {
                message = "Dear Parent,  " + student + " is absent today (" + attendanceDate.ToString("dd/MM/yyyy") + ") - " + user.TeacherName;
            }

            var senderName = "MYSCHO";
            //if (SchoolId == 10116)
            //{
            //    senderName = "PARDSE";
            //}
            //else if (SchoolId == 10117)
            //{
            //    senderName = "HOLYIN";
            //}
            var senderData = _Entity.TbSchoolSenderIds.Where(x => x.SchoolId == SchoolId && x.IsActive == true).FirstOrDefault();
            if (senderData != null)
                senderName = senderData.SenderId;
            var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + message + "&sender=" + senderName;

            // var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + message + "&priority=ndnd&stype=normal";
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            HttpWebRequest request = this.GetRequest(url);
            WebResponse webResponse = request.GetResponse();
            var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
            //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
            alvosmsResp respList = JsonSerializer.Deserialize<alvosmsResp>(responseText);
            if (responseText != null)
            {
                try
                {
                    if (headId != null && headId != 0)
                    {
                        TbSmsHistory sms = new TbSmsHistory();
                        sms.IsActive = true;
                        sms.MessageContent = message;
                        sms.SendStatus = Convert.ToString(respList.success);
                        sms.MessageDate = DateTime.UtcNow;
                        sms.ScholId = SchoolId;
                        sms.IsActive = true;
                        sms.MobileNumber = phone;
                        sms.SmsSentPerStudent = 1;
                        sms.StuentId = studentId;
                        sms.HeadId = headId;
                        if (respList.data != null)
                        {
                            sms.MessageReturnId = respList.data[0].messageId;
                            sms.DelivaryStatus = "Pending";
                        }
                        _Entity.TbSmsHistories.Add(sms);
                        _Entity.SaveChanges();
                    }
                }
                catch
                {

                }
                return true;
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

        public Tuple<bool, string, List<EventsList>> GetSchoolEvents(long TeacherId)
        {
            bool status = false;
            string msg = "Failed";
            List<EventsList> list = new Satluj_Latest.Data.Teacher(TeacherId).EventList();
            if (list != null && list.Count > 0)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<EventsList>>(status, msg, list);
            }
            else
            {
                msg = "Success";
                return new Tuple<bool, string, List<EventsList>>(status, msg, list);
            }
        }

        public Tuple<bool, string, List<CircularsList>> GetSchoolCirculars(long teacherId)
        {
            bool status = false;
            string msg = "Failed";
            List<CircularsList> list = new Satluj_Latest.Data.Teacher(teacherId).CircularList();
            if (list != null && list.Count > 0)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<CircularsList>>(status, msg, list);
            }
            else
            {
                msg = "Success";
                return new Tuple<bool, string, List<CircularsList>>(status, msg, list);
            }
        }
    }
}

