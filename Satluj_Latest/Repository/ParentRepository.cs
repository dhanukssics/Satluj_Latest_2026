using Microsoft.EntityFrameworkCore;
using Satluj_Latest.MapModel;
using Satluj_Latest.PostModel;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.PostModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Satluj_Latest.Repository
{
    public class ParentRepository
    {
        private readonly SchoolDbContext _Entity;
        private readonly IWebHostEnvironment _env;
        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        public DateTime currentTime = DateTime.UtcNow;
        public DateTime currentIndianTime = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        public ParentRepository(SchoolDbContext Entities)
        {
            _Entity = Entities;
        }


        public Tuple<bool, string, TbParent> AddParent(ParentRegistrationPostModel model)
        {
            var status = false;
            var msg = "Failed";

            var parent = new TbParent();
            parent.ParentGuid = Guid.NewGuid();
            parent.ParentName = model.ParentName;
            parent.Address = model.Address;
            parent.ContactNumber = model.ContactNumber;
            parent.Email = model.Email;
            parent.Password = model.Password;
            parent.IsActive = true;
            parent.City = model.City;
            parent.State = model.State;
            parent.TimeStamp = currentTime;
            parent.FilePath = model.FilePath;
            _Entity.TbParents.Add(parent);
            status = _Entity.SaveChanges() > 0;
            msg = status ? "Success" : "Failed";
            return new Tuple<bool, string, TbParent>(status, msg, parent);
        }

        public Tuple<bool, string, TbParent> ParentLogin(ParentLoginPostModel model)
        {
            var status = false;
            var msg = "Failed";
            var parentData = _Entity.TbParents.Where(x => x.Email.ToLower() == model.email.ToLower() && x.Password == model.password && x.IsActive).FirstOrDefault();
            if (parentData != null)
            {
                msg = "Success";
                status = true;

                var insertToken =new TbDeviceToken();
                insertToken.RoleId = 1;
                insertToken.UserId = parentData.ParentId;
                insertToken.Token = model.deviceToken;
                insertToken.TimeStamp = currentTime;
                insertToken.IsActive = true;
                insertToken.LoginStatus = 1;
                _Entity.TbDeviceTokens.Add(insertToken);
                status = _Entity.SaveChanges() > 0;

                return new Tuple<bool, string, TbParent>(status, msg, parentData);
            }
            else
            {
                msg = "Invalid Username or Password";
                return new Tuple<bool, string, TbParent>(status, msg, null);
            }
        }

        public object getUserById(long parentId)
        {
            return _Entity.TbParents.Where(z => z.ParentId == parentId).FirstOrDefault();
        }

        public Tuple<bool, string, List<Data.Student>> AddKid(ParentAddKidPostModel model)
        {
            var status = false;
            string msg = "Wrong Kid Id";
            long parentId = Convert.ToInt64(model.parentId);
            var studentData = _Entity.TbStudents.Where(x => x.StudentSpecialId == model.kidSpecialId && x.IsActive).FirstOrDefault();
            if (studentData != null)
            {
                studentData.ParentId = parentId;
                status = _Entity.SaveChanges() > 0;
                msg = "Success";
                var fullKidList = _Entity.TbStudents.Where(x => x.ParentId == parentId && x.IsActive).ToList().Select(z => new Data.Student(z)).ToList();
                return new Tuple<bool, string, List<Data.Student>>(status, msg, fullKidList);
            }
            else
            {
                var fullKidList = _Entity.TbStudents.Where(x => x.ParentId == parentId && x.IsActive).ToList().Select(z => new Data.Student(z)).ToList();
                return new Tuple<bool, string, List<Data.Student>>(status, msg, fullKidList);
            }

        }

        public Tuple<bool, string, List<Data.Student>> liststudent(ParentIdPostModel model)
        {
            var status = true;
            string msg = "success";
            long parentId = Convert.ToInt64(model.parentId);
            var studentData = _Entity.TbStudents.Where(x => x.ParentId == parentId && x.IsActive).ToList().Select(z => new Data.Student(z)).ToList();
            return new Tuple<bool, string, List<Data.Student>>(status, msg, studentData);
        }

        public Tuple<bool, string, List<AllMessages>> MessagesList(ParentMessagePostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long studentId = Convert.ToInt64(model.kidId);
            long schoolId = Convert.ToInt64(model.SchoolId);
            long classId = Convert.ToInt64(model.ClassId);
            int indexVal = Convert.ToInt32(model.index);
            int lengthVal = Convert.ToInt32(model.length);
            //var messageData = _Entity.tb_AllMessage.Where(x => (x.ToMsgSentId == studentId || x.ToMsgSentId == schoolId || x.ToMsgSentId == classId) && x.IsActive).ToList().OrderByDescending(x => x.Timestamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new AllMessages(x)).ToList();// 22-Jan -2018 Commended this line 
            var messageData = _Entity.TbAllMessages.Where(x => x.ToMsgSentId == studentId && x.MessageType == 1 && x.IsActive).ToList().OrderByDescending(x => x.Timestamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new AllMessages(x)).ToList();
            var classMessageData = _Entity.TbAllMessages.Where(x => x.ToMsgSentId == classId && x.MessageType == 2 && x.IsActive).ToList().OrderByDescending(x => x.Timestamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new AllMessages(x)).ToList();
            var schoolMessageData = _Entity.TbAllMessages.Where(x => x.ToMsgSentId == schoolId && x.MessageType == 3 && x.IsActive).ToList().OrderByDescending(x => x.Timestamp).ToList().Skip(indexVal * lengthVal).Take(lengthVal).ToList().Select(x => new AllMessages(x)).ToList();
            if (classMessageData.Count > 0)
                messageData.AddRange(classMessageData);
            if (schoolMessageData.Count > 0)
                messageData.AddRange(schoolMessageData);
            if (messageData.Count > 0)
            {
                status = true;
                msg = "Success";
            }
            else
            {
                status = true;
                msg = "No messages";
            }
            return new Tuple<bool, string, List<AllMessages>>(status, msg, messageData);
        }

        public Tuple<bool, string, List<AttendanceDetails>> AttendanceList(ParentKidAttendanceDataPostModel model)
        {
            List<AttendanceDetails> attendanceDataList = new List<AttendanceDetails>();
            bool status = false;
            string msg = "Failed";
            long studentId = Convert.ToInt64(model.kidId);
            int year = Convert.ToInt32(model.year);
            int month = Convert.ToInt32(model.month);
            var attendanceData = _Entity.TbAttendances.Where(x => x.AttendanceDate.Year == year && x.AttendanceDate.Month == month && x.StudentId == studentId && x.IsActive).ToList().Select(x => new TbAttendance(x)).ToList();
            List<DateTime> attendanceTime = attendanceData.Select(z => z.AttendanceDate.Date).Distinct().ToList();
            if (attendanceData.Count > 0)
            {
                status = true;
                msg = "Success";
                string oneDate = "";
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
                return new Tuple<bool, string, List<AttendanceDetails>>(status, msg, attendanceDataList);
            }
            else
            {
                return new Tuple<bool, string, List<AttendanceDetails>>(status, msg, attendanceDataList);
            }
        }

        public async Task <Tuple<bool, string, Driver>> KidTravellingData(ParentKidTravellingStatusPostModel model)
        {
            string msg = "Failed";
            bool status = false;
            long kidId = Convert.ToInt64(model.kidId);
            //var data = _Entity.SP_StudentCurrentTravellingData(kidId).ToList().Select(x => new SPStudentCurrentTravellingData(x)).ToList();
              var data = await _Entity.TbTrips
        .FromSqlRaw("EXEC SP_StudentCurrentTravellingData @studentId={0}", kidId)
        .FirstOrDefaultAsync();
            if (data !=null)
            {
                var newData = data;
                if (newData.TravellingStatus != 2)
                {
                    status = true;
                    if (newData.TravellingStatus == 0)
                        msg = "Trip Start";
                    else
                        msg = "Running";

                    var driverData = _Entity.TbDrivers.Where(x => x.DriverId == newData.DriverId).ToList().Select(x => new Driver(x)).FirstOrDefault();
                    return new Tuple<bool, string, Driver>(status, msg, driverData);
                }
                else
                {
                    msg = "Trip Completed";
                    return new Tuple<bool, string, Driver>(status, msg, null);
                }
            }
            else
            {
                msg = "No Travelling";
                return new Tuple<bool, string, Driver>(status, msg, null);
            }
        }

        public Tuple<bool, string> ParentLogout(ParentLogoutPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long parentId = Convert.ToInt64(model.parentId);
            var parentData = _Entity.TbDeviceTokens.Where(x => x.UserId == parentId && x.Token == model.deviceToken && x.LoginStatus == 1 && x.IsActive).ToList();
            if (parentData.Count > 0)
            {
                foreach (var item in parentData)
                {
                    item.LoginStatus = 0;
                    _Entity.SaveChanges();
                }
            }
            msg = "Successfully Logout";
            status = true;
            return new Tuple<bool, string>(status, msg);
        }

        public async Task<Tuple<bool, string, StudentBillHistoryMapModel>> GetStudentBillHistoryAsync(StudentBillHistoryPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            StudentBillHistoryMapModel mainData = new StudentBillHistoryMapModel();
            List<PaidHistory> _NewPaidHistory = new List<PaidHistory>();
            List<DueBills> _NewDueHistory = new List<DueBills>();
            long studentId = Convert.ToInt64(model.studentId);
            long classsId = Convert.ToInt64(model.classId);

            var data = _Entity.TbPayments.Where(z => z.StudentId == studentId && z.ClassId == classsId && z.BillNo != null && z.IsActive == true).ToList();
            if (data.Count > 0)
            {
                long billNo = 0;
                foreach (var item in data)
                {
                    if (billNo != item.BillNo)
                    {
                        billNo = item.BillNo ?? 0;
                        PaidHistory one = new PaidHistory();
                        one.BillNo = billNo;
                        one.Billdate = item.TimeStamp;
                        List<PaidBills> oneBills = new List<PaidBills>();
                        var oneList = data.Where(x => x.BillNo == billNo).ToList();
                        foreach (var item2 in oneList)
                        {
                            string ext = "";
                            try
                            {
                                var duedate = item2.Fee.TbFeeClasses.Where(z => z.FeeId == item2.Fee.FeeId).FirstOrDefault().DueDate;
                                ext = String.Format("{0:y}", duedate);
                            }
                            catch (Exception ex)
                            {
                                ext = "Additional";
                            }
                            PaidBills newOne = new PaidBills();
                            newOne.Particulars = item2.Fee.FeesName.ToString() + ' ' + ext;
                            newOne.Discount = item2.Discount ?? 0;
                            newOne.Amount = item2.Amount;
                            oneBills.Add(newOne);
                        }
                        one.PaidBillsData = oneBills;
                        _NewPaidHistory.Add(one);
                    }
                }
            }
          
                var dueData = await _Entity.SPFullFees
    .FromSqlRaw("EXEC SP_FullFee @classId, @studentId", classsId, studentId)
    .ToListAsync();           
            if (dueData.Count > 0)
            {
                foreach (var item2 in dueData)
                {
                    DueBills newOne = new DueBills();
                    newOne.Particulars = item2.Feename;
                    newOne.Details = item2.DueDate.ToString("MMMM");
                    newOne.Amount = item2.Amount;
                    _NewDueHistory.Add(newOne);
                }

            }


            mainData.PaidHistoryData = _NewPaidHistory;
            mainData.DueHistoryData = _NewDueHistory;
            msg = "Successful";
            status = true;
            return new Tuple<bool, string, StudentBillHistoryMapModel>(status, msg, mainData);

        }
        public async Task<Tuple<bool, string>> SendMessageAsync(ParentMessageSendPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long parentId = Convert.ToInt64(model.ParentId);
            long studentId = Convert.ToInt64(model.StudentId);
            string filePath = string.Empty;
            if (model.PostFile != null && model.PostFile.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "Media", "Parent", "Message");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileExtension = Path.GetExtension(model.PostFile.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;

                string fileSavePath = Path.Combine(folderPath, fileName);

                // Save file in ASP.NET Core
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await model.PostFile.CopyToAsync(stream);
                }

                // Relative path to store in DB
                 filePath = Path.Combine("/Media/Parent/Message/", fileName);

            }
            var message = new TbParentMessage();
            message.SenderId = parentId;
            message.StudentId = studentId;
            message.Subject = model.Subject;
            message.Description = model.Description;
            message.FilePath = filePath;
            message.IsActive = true;
            //message.TimeStamp =  currentTime;
            message.TimeStamp = currentIndianTime;
            _Entity.TbParentMessages.Add(message);
            status = _Entity.SaveChanges() > 0;
            if (status)
            {
                msg = "Success";
                return new Tuple<bool, string>(status, msg);
            }
            else
            {
                msg = "Sorry, Can't send the message!";
                return new Tuple<bool, string>(status, msg);
            }
        }

        public Tuple<bool, string, List<NewAttendanceDetails>> NewAttendanceList(ParentKidAttendanceDataPostModel model)
        {
            List<NewAttendanceDetails> attendanceDataList = new List<NewAttendanceDetails>();
            bool status = false;
            string msg = "Failed";
            long studentId = Convert.ToInt64(model.kidId);
            int year = Convert.ToInt32(model.year);
            int month = Convert.ToInt32(model.month);
            var attendanceData = _Entity.TbAttendances.Where(x => x.AttendanceDate.Year == year && x.AttendanceDate.Month == month && x.StudentId == studentId && x.IsActive).ToList().Select(x => new TbAttendance(x)).ToList();
            List<DateTime> attendanceTime = attendanceData.Select(z => z.AttendanceDate.Date).Distinct().ToList();
            if (attendanceData.Count > 0)
            {
                status = true;
                msg = "Success";
                foreach (var item in attendanceTime)
                {
                    NewAttendanceDetails obj = new NewAttendanceDetails();
                    obj.attendanceDate = item.ToShortDateString();
                    obj.mornignShift = (int)AttendanceStatus.NotTaken;
                    obj.eveningShift = (int)AttendanceStatus.NotTaken;
                    var eachDay = attendanceData.Where(z => z.AttendanceDate.Date == item.Date).ToList();
                    if (eachDay.Count > 0)
                    {
                        var morningStatus = eachDay.FirstOrDefault(z => z.ShiftStatus == 0);
                        var eveningStatus = eachDay.FirstOrDefault(z => z.ShiftStatus == 1);
                        if (morningStatus != null)
                        {
                            if (morningStatus.AttendanceData == true)
                                obj.mornignShift = (int)AttendanceStatus.Present;
                            else
                                obj.mornignShift = (int)AttendanceStatus.Absent;
                        }
                        if (eveningStatus != null)
                        {
                            if (eveningStatus.AttendanceData)
                                obj.eveningShift = (int)AttendanceStatus.Present;
                            else
                                obj.eveningShift = (int)AttendanceStatus.Absent;
                        }
                    }

                    attendanceDataList.Add(obj);
                }
                return new Tuple<bool, string, List<NewAttendanceDetails>>(status, msg, attendanceDataList);
            }
            else
            {
                return new Tuple<bool, string, List<NewAttendanceDetails>>(status, msg, attendanceDataList);
            }
        }
        public Tuple<bool, string, List<EventsList>> GetKidsSchoolEvents(long ParentId)
        {
            bool status = false;
            string msg = "Failed";
            List<EventsList> list = new Satluj_Latest.Data.Parent(ParentId).ParentEventList();
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
        public Tuple<bool, string, List<CircularsList>> GetKidsSchoolCirculars(long ParentId)
        {
            bool status = false;
            string msg = "Failed";
            List<CircularsList> list = new Satluj_Latest.Data.Parent(ParentId).ParentCircularList();
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

        public Tuple<bool, string> ChangePassword(ParentChangePasswordPostModel model)
        {
            string msg = "Failed";
            bool status = false;
            long ParentId = Convert.ToInt64(model.ParentId);
            var parentDetails = _Entity.TbParents.Where(x => x.ParentId == ParentId && x.IsActive).FirstOrDefault();
            if (parentDetails != null)
            {
                if (parentDetails.Password.Trim() != model.OldPassword)
                {
                    msg = "The wrong password ";
                }
                else if (parentDetails.Password == model.CurrentPassword)
                {
                    msg = "No changes occured!";
                }
                else
                {
                    parentDetails.Password = model.CurrentPassword;
                    status = _Entity.SaveChanges() > 0;
                    if (status)
                        msg = "Successful";
                }
            }
            return new Tuple<bool, string>(status, msg);
        }

        public async Task<Tuple<bool, string, string>> ParentProfileEditAsync(ParentProfileEditPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            string returnPath = "";
            long ParentId = Convert.ToInt64(model.ParentId);
            if (model.ProfilePath != null && model.ProfilePath != string.Empty)
            {
                string profileFolderPath = Path.Combine(_env.WebRootPath, "Media", "Parent", "Profile");

                if (!Directory.Exists(profileFolderPath))
                {
                    Directory.CreateDirectory(profileFolderPath);
                }

                byte[] imageBytes = Convert.FromBase64String(model.ProfilePath);

                string profileImageName = Guid.NewGuid().ToString() + ".jpeg";

                string filePath = Path.Combine(profileFolderPath, profileImageName);

                // relative path for saving in database
                string fileSave = Path.Combine("/Media/Parent/Profile/", profileImageName);

                // write file
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.WriteAsync(imageBytes, 0, imageBytes.Length);
                
                  }

                returnPath = fileSave;

                var parent = _Entity.TbParents.Where(x => x.ParentId == ParentId).FirstOrDefault();
                parent.FilePath = returnPath;
                status = _Entity.SaveChanges() > 0;
                if (status)
                    msg = "Successful";
            }
            return new Tuple<bool, string, string>(status, msg, returnPath);
        }
    }
}


