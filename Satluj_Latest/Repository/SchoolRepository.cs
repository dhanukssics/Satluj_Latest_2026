using Microsoft.EntityFrameworkCore;
using Satluj_Latest.PostModel;
using Satluj_Latest.Data;
using Satluj_Latest.MapModel;
using Satluj_Latest.Models;
using Satluj_Latest.Service.Helper;
using Satluj_Latest.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;
using static Satluj_Latest.PostModel.TeacherAttendancePostModel;
using static System.Net.Mime.MediaTypeNames;

namespace Satluj_Latest.Repository
{
    public class SchoolRepository
    {
        Random rnd = new Random();


        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        private readonly SchoolDbContext _Entity;

        public SchoolRepository(SchoolDbContext Entity)
        {
            _Entity = Entity;
        }
        public SchoolRepository()
        {
            _Entity = new SchoolDbContext();
        }
        public DateTime currentTime = DateTime.UtcNow;

        public Tuple<bool, string, School> AddNewSchool(SchoolRegistrationPostModel model)
        {
            var status = false;
            var msg = "Failed";
            Tuple<string, string> locationData = GetdtLatLong(model.Address);//Find current lat and long
            if (locationData.Item1 != "")
            {
                //var school = _Entity.tb_School.Create();
                var school = new TbSchool();
                school.SchoolGuidId = Guid.NewGuid();
                school.SchoolName = model.SchoolName;
                school.Address = model.Address;
                school.Contact = model.Contact;
                school.IsActive = true;
                school.City = model.City;
                school.State = model.State;
                school.TimeStamp = currentTime;
                school.FilePath = model.FilePath;
                school.Website = model.Website;
                school.Latitude = locationData.Item1;
                school.Longitude = locationData.Item2;
                _Entity.TbSchools.Add(school);
                status = _Entity.SaveChanges() > 0;
                if (status)
                {
                    //var login = _Entity.TbLogins.Create();
                    var login = new TbLogin();
                    login.SchoolId = school.SchoolId;
                    login.RoleId = 1;
                    login.Name = school.SchoolName;
                    login.Username = model.Email;
                    login.Password = model.Password;
                    login.IsActive = true;
                    login.TimeStamp = currentTime;
                    login.DisableStatus = false;
                    login.LoginGuid = Guid.NewGuid();
                    _Entity.TbLogins.Add(login);
                    status = _Entity.SaveChanges() > 0;
                    msg = "Success";
                }
                var schoolData = _Entity.TbSchools.Where(x => x.SchoolId == school.SchoolId && x.IsActive).ToList().Select(x => new School(x)).FirstOrDefault();
                return new Tuple<bool, string, School>(status, msg, schoolData);
            }
            else
            {
                msg = "Invalid Address";
                return new Tuple<bool, string, School>(status, msg, null);
            }
        }

        private Tuple<string, string> GetdtLatLong(string address)
        {
            try
            {
                var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = xdoc.Element("GeocodeResponse").Element("result");
                var locationElement = result.Element("geometry").Element("location");
                var lat = locationElement.Element("lat");
                var lon = locationElement.Element("lng");
                string latData = lat.Value.ToString();
                string longData = lon.Value.ToString();
                return Tuple.Create(latData, longData);
            }
            catch (Exception ex)
            {

                return Tuple.Create("", "");
            }
        }


        public Tuple<bool, string, School> SchoolLogin(SchoolLoginPostModel model)
        {
            var status = false;
            var msg = "Failed";
            var schoolData = _Entity.TbLogins.Where(x => x.Username.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive).FirstOrDefault();
            if (schoolData != null)
            {
                var schoolFullData = _Entity.TbSchools.Where(x => x.SchoolId == schoolData.SchoolId).ToList().Select(x => new School(x)).FirstOrDefault();
                msg = "Success";
                status = true;
                return new Tuple<bool, string, School>(status, msg, schoolFullData);
            }
            else
            {
                msg = "Invalid Username or Password!";
                return new Tuple<bool, string, School>(status, msg, null);
            }
        }

        public Tuple<bool, string, TbSchool> AddNewClassAndDivision(SchoolAddClassAndDivisionPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.SchoolId);
            var academicYear = _Entity.TbAcademicYears.Where(x => x.IsActive == true && x.CurrentYear == true).FirstOrDefault();
            int order = Convert.ToInt32(model.ClassNameOrdr);
            var classData = _Entity.TbClasses.Where(x => x.ClassOrder == order && x.SchoolId == schoolId && x.IsActive == true && x.AcademicYearId == academicYear.YearId).FirstOrDefault();
            if (classData != null)
            {
                var divisionData = _Entity.TbDivisions.Where(x => x.Division == model.Division && x.ClassId == classData.ClassId && x.IsActive == true).FirstOrDefault();
                if (divisionData != null)
                {
                    msg = "Division Already Exists";
                    status = true;
                    var schoolDivisionData = _Entity.TbSchools.Where(x => x.SchoolId == schoolId && x.IsActive).FirstOrDefault();
                    return new Tuple<bool, string, TbSchool>(status, msg, schoolDivisionData);
                }
                else
                {
                    //var division = _Entity.tb_Division.Create();
                    var division = new TbDivision();
                    division.DivisionGuid = Guid.NewGuid();
                    division.Division = model.Division;
                    division.ClassId = classData.ClassId;
                    division.IsActive = true;
                    division.TimeStamp = currentTime;
                    _Entity.TbDivisions.Add(division);
                    status = _Entity.SaveChanges() > 0;
                    msg = status ? "Success" : "Failed";
                    var schoolDivisionData = _Entity.TbSchools.Where(x => x.SchoolId == schoolId && x.IsActive).FirstOrDefault();
                    return new Tuple<bool, string, TbSchool>(status, msg, schoolDivisionData);
                }
            }
            else
            {
                var classOrder = _Entity.TbClassLists.Where(x => x.OrderValue == order && x.IsActive).FirstOrDefault();
                //var addClass = _Entity.tb_Class.Create();
                var addClass = new TbClass();
                addClass.ClassGuild = Guid.NewGuid();
                addClass.SchoolId = Convert.ToInt64(model.SchoolId);
                addClass.Class = classOrder.ClassName;
                addClass.Timestamp = currentTime;
                addClass.IsActive = true;
                addClass.AcademicYearId = academicYear.YearId;
                addClass.ClassOrder = order;
                //if (Convert.ToInt32(model.ClassName) == 1)
                //    addClass.ClassOrder = 1;
                //if (Convert.ToInt32(model.ClassName) == 2)
                //    addClass.ClassOrder = 2;
                //if (Convert.ToInt32(model.ClassName) == 3)
                //    addClass.ClassOrder = 3;
                //if (Convert.ToInt32(model.ClassName) == 4)
                //    addClass.ClassOrder = 4;
                //if (Convert.ToInt32(model.ClassName) == 5)
                //    addClass.ClassOrder = 5;
                //if (Convert.ToInt32(model.ClassName) == 6)
                //    addClass.ClassOrder = 6;
                //if (Convert.ToInt32(model.ClassName) == 7)
                //    addClass.ClassOrder = 7;
                //if (Convert.ToInt32(model.ClassName) == 8)
                //    addClass.ClassOrder = 8;
                //if (Convert.ToInt32(model.ClassName) == 9)
                //    addClass.ClassOrder = 9;
                //if (Convert.ToInt32(model.ClassName) == 10)
                //    addClass.ClassOrder = 10;
                //if (Convert.ToInt32(model.ClassName) == 11)
                //    addClass.ClassOrder = 11;
                //if (Convert.ToInt32(model.ClassName) == 12)
                //    addClass.ClassOrder = 12;
                _Entity.TbClasses.Add(addClass);
                status = _Entity.SaveChanges() > 0;
                {
                    //var division = _Entity.tb_Division.Create();
                    var division = new TbDivision();
                    division.DivisionGuid = Guid.NewGuid();
                    division.Division = model.Division;
                    division.ClassId = addClass.ClassId;
                    division.IsActive = true;
                    division.TimeStamp = currentTime;
                    _Entity.TbDivisions.Add(division);
                    status = _Entity.SaveChanges() > 0;
                    {
                        msg = status ? "Success" : "Failed";
                        var schoolDivisionData = _Entity.TbSchools.Where(x => x.SchoolId == schoolId && x.IsActive).FirstOrDefault();
                        return new Tuple<bool, string, TbSchool>(status, msg, schoolDivisionData);
                    }
                }
            }
        }

        public Tuple<bool, string, List<TbClass>> FullClassWithDivision(SchoolClassListWithDivisionPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.SchoolId);
            var classData = _Entity.TbClasses.Where(x => x.SchoolId == schoolId && x.IsActive && x.PublishStatus==true).OrderBy(x => x.ClassOrder).ToList();
            if (classData != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<TbClass>>(status, msg, classData);
            }
            else
            {
                return new Tuple<bool, string, List<TbClass>>(status, msg, null);
            }
        }

        //public Tuple<bool, string, string> AddNewTeacher(SchoolAddTeacherPostModel model)
        //{
        //    var status = false;
        //    var msg = "Failed";

        //    long schoolId = Convert.ToInt64(model.schoolId);

        //    long classId = 0;
        //    if (model.classId != null && model.classId != string.Empty)
        //        classId = Convert.ToInt64(model.classId);

        //    long divisionId = 0;
        //    if (model.divisionId != null && model.divisionId != string.Empty)
        //        divisionId = Convert.ToInt64(model.divisionId);
        //    //if (_Entity.TbTeachersClass.Any(x => x.DivisionId == divisionId))
        //    //{
        //    //    msg = "Class Already Assigned";
        //    //    return new Tuple<bool, string, string>(status, msg, "");
        //    //}
        //    //else
        //    //{

        //    var addTeacher = _Entity.TbTeachers.Create();
        //    addTeacher.TeacherGuid = Guid.NewGuid();
        //    addTeacher.TeacherName = model.teacherName;
        //    addTeacher.TeacherSpecialId = "TR" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
        //    addTeacher.SchoolId = schoolId;
        //    addTeacher.Email = model.emailId;
        //    addTeacher.ContactNumber = model.contactNumber;
        //    addTeacher.TimeStamp = currentTime;
        //    addTeacher.IsActive = true;
        //    addTeacher.FilePath = model.filePath;
        //    _Entity.TbTeachers.Add(addTeacher);
        //    status = _Entity.SaveChanges() > 0;
        //    {
        //        var teacherLogin = _Entity.TbLogins.Create();
        //        teacherLogin.SchoolId = schoolId;
        //        teacherLogin.RoleId = Convert.ToInt32(ClassLibrary.UserRole.Teacher);
        //        teacherLogin.Name = model.teacherName;
        //        teacherLogin.Username = model.emailId;
        //        teacherLogin.Password = addTeacher.TeacherSpecialId;
        //        teacherLogin.IsActive = true;
        //        teacherLogin.DisableStatus = false;
        //        teacherLogin.LoginGuid = Guid.NewGuid();
        //        _Entity.TbLogins.Add(teacherLogin);
        //        status = _Entity.SaveChanges() > 0;
        //        if (classId != 0 && divisionId != 0)
        //        {

        //            var addTeacherClass = _Entity.TbTeachersClass.Create();
        //            addTeacherClass.TeacherId = addTeacher.TeacherId;
        //            addTeacherClass.ClassId = classId;
        //            addTeacherClass.DivisionId = divisionId;
        //            _Entity.TbTeachersClass.Add(addTeacherClass);
        //            status = _Entity.SaveChanges() > 0;
        //        }
        //        msg = "Success";
        //        status = true;
        //    }
        //    return new Tuple<bool, string, string>(status, msg, teacherLogin.Password);
        //}
        public Tuple<bool, string, string> AddNewTeacher(SchoolAddTeacherPostModel model)
        {

            var status = false;
            var msg = "Failed";

            long schoolId = Convert.ToInt64(model.schoolId);

            long classId = 0;
            if (model.classId != null && model.classId != string.Empty)
                classId = Convert.ToInt64(model.classId);

            long divisionId = 0;
            if (model.divisionId != null )
                divisionId = Convert.ToInt64(model.divisionId);
            //if (_Entity.TbTeachersClass.Any(x => x.DivisionId == divisionId))
            //{
            //    msg = "Class Already Assigned";
            //    return new Tuple<bool, string, string>(status, msg, "");
            //}
            //else
            //{
            //var teacherLogin = _Entity.TbLogins.Create();
            var teacherLogin = new TbLogin();
            teacherLogin.SchoolId = schoolId;
            teacherLogin.RoleId = Convert.ToInt32(UserRole.Teacher);
            teacherLogin.Name = model.teacherName;
            teacherLogin.Username = model.emailId;
            teacherLogin.Password = "TR" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            teacherLogin.IsActive = true;
            teacherLogin.TimeStamp = currentTime;
            teacherLogin.DisableStatus = false;
            teacherLogin.LoginGuid = Guid.NewGuid();
            _Entity.TbLogins.Add(teacherLogin);
            status = _Entity.SaveChanges() > 0;
            if (status)
            {
                //var userType = _Entity.tb_UserModuleMain.Where(x => x.SchoolId == schoolId && x.IsTeacher == true).FirstOrDefault();
                //var addTeacher = _Entity.TbTeachers.Create();
                var addTeacher = new TbTeacher();
                addTeacher.TeacherGuid = Guid.NewGuid();
                addTeacher.TeacherName = model.teacherName;
                addTeacher.TeacherSpecialId = teacherLogin.Password;
                addTeacher.SchoolId = schoolId;
                addTeacher.Email = model.emailId;
                addTeacher.ContactNumber = model.contactNumber;
                addTeacher.TimeStamp = currentTime;
                addTeacher.IsActive = true;
                addTeacher.FilePath = model.filePath;
                addTeacher.UserId = teacherLogin.UserId;
                addTeacher.DepartmentId = model.DepartmentId == null ? 0 : Convert.ToInt64(model.DepartmentId);
                addTeacher.DesignationId = model.DesignationId == null ? 0 : Convert.ToInt64(model.DesignationId);
                if (model.UserTypeId != null && model.UserTypeId != string.Empty)
                    addTeacher.UserType = Convert.ToInt64(model.UserTypeId);
                _Entity.TbTeachers.Add(addTeacher);
                status = _Entity.SaveChanges() > 0;
                {
                    if (!string.IsNullOrEmpty(model.RolesData))
                    {
                        var roles = model.RolesData.Split(',');

                        foreach (var item in roles)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var role = new TbUserRole();
                                role.UserId = addTeacher.UserId;
                                role.RoleId = Convert.ToInt64(item);
                                role.IsActive = true;
                                role.TimeStamp = currentTime;

                                _Entity.TbUserRoles.Add(role);
                            }
                        }

                        _Entity.SaveChanges();
                    }



                    if (classId != 0 && divisionId != 0)
                    {

                        var addTeacherClass = new TbTeacherClass();
                        addTeacherClass.TeacherId = addTeacher.TeacherId;
                        addTeacherClass.ClassId = classId;
                        addTeacherClass.DivisionId = divisionId;
                        _Entity.TbTeacherClasses.Add(addTeacherClass);
                        status = _Entity.SaveChanges() > 0;
                    }
                    msg = "Success";
                    status = true;
                }
            }
            return new Tuple<bool, string, string>(status, msg, teacherLogin.Password);
            //}
        }

        public Tuple<bool, string, List<TbTeacher>> FullTeacherList(SchoolTeacherListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var teacherList = _Entity.TbTeachers.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(z => z.TeacherName).ToList();
            if (teacherList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<TbTeacher>>(status, msg, teacherList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<TbTeacher>>(status, msg, null);
            }
        }

        public Tuple<bool, string, string> AddNewBus(SchoolAddBusPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var schoolPlace = _Entity.TbSchools.Where(x => x.SchoolId == schoolId && x.IsActive).FirstOrDefault();
            var addBus = new TbBu() ;
            addBus.SchoolId = schoolId;
            addBus.BusName = model.busName;
            addBus.TripNumber = Convert.ToInt32(model.tripNumber);
            addBus.IsActive = true;
            addBus.TimeStamp = currentTime;
            addBus.LocationEnd = schoolPlace.City;
            addBus.BusType = model.busType;
            addBus.BusGuid = Guid.NewGuid();
            addBus.LocationStart = model.endLocation;
            addBus.BusSpecialId = "BS" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            _Entity.TbBus.Add(addBus);
            status = _Entity.SaveChanges() > 0;
            {
                msg = "Success";
                return new Tuple<bool, string, string>(status, msg, addBus.BusSpecialId);
            }
        }

        public Tuple<bool, string, List<TbBu>> FullBusList(SchoolBusListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var busList = _Entity.TbBus.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
            if (busList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<TbBu>>(status, msg, busList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<TbBu>>(status, msg, null);
            }
        }

        public Tuple<bool, string, string> AddStudent(SchoolAddStudentPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long busId = Convert.ToInt64(model.busId);
            long classId = Convert.ToInt64(model.classId);
            long divisionId = Convert.ToInt64(model.divisionId);
            var addStudent = new TbStudent();
            //addStudent.StudentSpecialId = "ST" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            addStudent.StudentSpecialId = model.admissionId;
            addStudent.SchoolId = schoolId;
            addStudent.StundentName = model.studentName;
            addStudent.ParentName = model.parentName;
            addStudent.Address = model.address;
            addStudent.City = model.city;
            addStudent.ContactNumber = model.contact;
            addStudent.ClasssNumber = model.classNo;
            addStudent.ClassId = classId;
            addStudent.DivisionId = divisionId;
            addStudent.BusId = busId;
            addStudent.TripNo = model.tripNo;
            addStudent.FilePath = model.filePath;
            addStudent.TimeStamp = currentTime;
            addStudent.StudentGuid = Guid.NewGuid();
            addStudent.IsActive = true;
            addStudent.State = model.state;
            addStudent.Gender = model.gender;
            addStudent.BloodGroup = model.bloodGroup;
            addStudent.ParentEmail = "";
            if (model.DOB != null && model.DOB.Year != 1)
            {
                addStudent.Dob = Convert.ToDateTime(model.DOB);
            }
            _Entity.TbStudents.Add(addStudent);
            status = _Entity.SaveChanges() > 0;
            {
                msg = "Success";
                return new Tuple<bool, string, string>(status, msg, addStudent.StudentSpecialId);
            }
        }

        public Tuple<bool, string, List<TbStudent>> FullStudentList(SchoolFullStudentListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long classId = Convert.ToInt64(model.classId);
            long divisionId = Convert.ToInt64(model.divisionId);
            var studentList = _Entity.TbStudents.Where(x => x.SchoolId == schoolId && x.IsActive && x.ClassId == classId && x.DivisionId == divisionId).OrderBy(z => z.StundentName).ToList();
            if (studentList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<TbStudent>>(status, msg, studentList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<TbStudent>>(status, msg, null);
            }
        }

        public Tuple<bool, string, string> AddDriver(SchoolAddDriverPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.SchoolId);
            var addDriver = new TbDriver();
            addDriver.DriverSpecialId = "DR" + RandomStringGenerator.RandomString() + rnd.Next(1, 100);
            addDriver.SchoolId = schoolId;
            addDriver.DriverName = model.DriverName;
            addDriver.LicenseNumber = model.LicenseNumber;
            addDriver.ContactNumber = model.ContactNumber;
            addDriver.Address = model.Address;
            addDriver.FilePath = model.FilePath;
            addDriver.TimeStamp = currentTime;
            addDriver.DriverGuid = Guid.NewGuid();
            addDriver.IsActive = true;
            addDriver.City = model.City;
            addDriver.State = model.State;
            _Entity.TbDrivers.Add(addDriver);
            status = _Entity.SaveChanges() > 0;
            {
                msg = "Success";
                return new Tuple<bool, string, string>(status, msg, addDriver.DriverSpecialId);
            }
        }

        public Tuple<bool, string, List<TbDriver>> FullDriverList(SchoolDriverListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var driverList = _Entity.TbDrivers.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.DriverName).ToList();
            if (driverList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<TbDriver>>(status, msg, driverList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<TbDriver>>(status, msg, null);
            }
        }

        public Tuple<bool, string, List<TbClass>> DeleteDivision(SchoolDeleteDivisionPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long divisionId = Convert.ToInt64(model.divisionId);
            var divisionData = _Entity.TbDivisions.Where(x => x.DivisionId == divisionId && x.Class.SchoolId == schoolId && x.IsActive == true).FirstOrDefault();
            divisionData.IsActive = false;
            status = _Entity.SaveChanges() > 0;
            {
                var division = _Entity.TbDivisions.Where(z => z.ClassId == divisionData.ClassId && z.IsActive).ToList();
                if (division.Count <= 0)
                {
                    var classRow = _Entity.TbClasses.FirstOrDefault(z => z.ClassId == divisionData.ClassId && z.IsActive);
                    if (classRow != null)
                    {
                        classRow.IsActive = false;
                        _Entity.SaveChanges();
                    }
                }

                var teacherData = _Entity.TbTeacherClasses.Where(x => x.DivisionId == divisionId).FirstOrDefault();
                if (teacherData != null)
                {
                    _Entity.TbTeacherClasses.Remove(teacherData);
                    _Entity.SaveChanges();
                }
                var classData = _Entity.TbClasses.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (classData != null)
                {
                    msg = "Success";
                    status = true;
                    return new Tuple<bool, string, List<TbClass>>(status, msg, classData);
                }
                else
                {
                    return new Tuple<bool, string, List<TbClass>>(status, msg, null);
                }

            }
        }

        public Tuple<bool, string, List<TbTeacher>> DeleteTeacher(SchoolDeleteTeacherPostModel model)
        {
            long schoolId = Convert.ToInt64(model.schoolId);
            long teacherId = Convert.ToInt64(model.teacherId);
            
            var teacherData = _Entity.TbTeachers
                .FirstOrDefault(x => x.TeacherId == teacherId &&
                                     x.SchoolId == schoolId 
                                     );

            if (teacherData == null)
                return new Tuple<bool, string, List<TbTeacher>>(false, "Teacher not found", null);

            // Soft Delete Teacher
            teacherData.IsActive = false;
            _Entity.SaveChanges();

            // Remove class assignment (optional)
            var teacherClass = _Entity.TbTeacherClasses
                .Where(x => x.TeacherId == teacherId)
                .ToList();

            if (teacherClass.Any())
            {
                _Entity.TbTeacherClasses.RemoveRange(teacherClass);
                _Entity.SaveChanges();
            }

            // Get updated list
            var list = _Entity.TbTeachers
                .Where(x => x.SchoolId == schoolId && x.IsActive==true)
                .ToList();

            return new Tuple<bool, string, List<TbTeacher>>(true, "Deleted Successfully", list);
        }
        public Tuple<bool, string, List<TbBu>> DeleteBus(SchoolDeleteBusPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long busId = Convert.ToInt64(model.busId);
            var busData = _Entity.TbBus.Where(x => x.SchoolId == schoolId && x.BusId == busId).FirstOrDefault();
            busData.IsActive = false;
            status = _Entity.SaveChanges() > 0;
            {
                var studentList = _Entity.TbStudents.Where(x => x.SchoolId == schoolId && x.BusId == busId && x.IsActive).ToList();
                if (studentList.Count > 0)
                {
                    foreach (var item in studentList)
                    {
                        item.BusId = 1;
                        item.TripNo = null;
                        status = _Entity.SaveChanges() > 0;
                    }
                }
                var busList = _Entity.TbBus.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (busList != null)
                {
                    msg = "Success";
                    status = true;
                    return new Tuple<bool, string, List<TbBu>>(status, msg, busList);
                }
                else
                {
                    msg = "No data found";
                    status = true;
                    return new Tuple<bool, string, List<TbBu>>(status, msg, null);
                }
            }

        }

        public Tuple<bool, string, List<TbStudent>> DeleteStudent(SchoolDeleteStudentPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long studentId = Convert.ToInt64(model.studentId);
            var studentData = _Entity.TbStudents.Where(x => x.SchoolId == schoolId && x.StudentId == studentId).FirstOrDefault();
            studentData.IsActive = false;
            status = _Entity.SaveChanges() > 0;
            {
                var studentList = _Entity.TbStudents.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (studentList != null)
                {
                    msg = "Success";
                    status = true;
                    return new Tuple<bool, string, List<TbStudent>>(status, msg, studentList);
                }
                else
                {
                    msg = "No data found";
                    status = true;
                    return new Tuple<bool, string, List<TbStudent>>(status, msg, null);
                }
            }
        }

        public Tuple<bool, string, List<TbDriver>> DeleteDriver(SchoolDeleteDriverPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long driverId = Convert.ToInt64(model.driverId);
            var driverData = _Entity.TbDrivers.Where(x => x.SchoolId == schoolId && x.DriverId == driverId).FirstOrDefault();
            driverData.IsActive = false;
            status = _Entity.SaveChanges() > 0;
            {
                var driverList = _Entity.TbDrivers.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (driverList != null)
                {
                    msg = "Success";
                    status = true;
                    return new Tuple<bool, string, List<TbDriver>>(status, msg, driverList);
                }
                else
                {
                    msg = "No data found";
                    status = true;
                    return new Tuple<bool, string, List<TbDriver>>(status, msg, null);
                }
            }
        }

        public Tuple<bool, string, TbSchool> EditSchoolData(SchoolEditPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long scholId = Convert.ToInt64(model.schoolId);
            var school = _Entity.TbSchools.Where(x => x.SchoolId == scholId && x.IsActive).FirstOrDefault();

            if (model.Address != string.Empty && model.Address != null)
                school.Address = model.Address;

            if (model.Contact != string.Empty && model.Contact != null)
                school.Contact = model.Contact;

            if (model.City != string.Empty && model.City != null)
                school.City = model.City;

            if (model.State != string.Empty && model.State != null)
                school.State = model.State;

            if (model.FilePath != string.Empty && model.FilePath != null)
                school.FilePath = model.FilePath;

            if (model.Website != string.Empty && model.Website != null)
                school.Website = model.Website;

            if (model.SchoolName != string.Empty && model.SchoolName != null)
                school.SchoolName = model.SchoolName;

            status = _Entity.SaveChanges() > 0;
            msg = status ? "Success" : "Failed";
            return new Tuple<bool, string, TbSchool>(status, msg, school);
        }

        public Tuple<bool, string, string> ChangeUsername(SchoolChangeUserNamePostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var schoolData = _Entity.TbLogins.Where(x => x.SchoolId == schoolId && x.Username.ToLower() == model.oldEmailId.ToLower()).FirstOrDefault();
            schoolData.Username = model.newEmailId;
            status = _Entity.SaveChanges() > 0;
            msg = status ? "Success" : "Failed";
            return new Tuple<bool, string, string>(status, msg, schoolData.Username);
        }

        public Tuple<bool, string> ChangePassword(SchoolChangePasswordPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var schoolData = _Entity.TbLogins.Where(x => x.SchoolId == schoolId && x.Password == model.oldPassword).FirstOrDefault();
            schoolData.Password = model.newPassword;
            status = _Entity.SaveChanges() > 0;
            msg = status ? "Success" : "Failed";
            return new Tuple<bool, string>(status, msg);
        }

        public Tuple<bool, string, List<Class>> FullClassList(SchoolClassListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var classList = _Entity.TbClasses.Where(x => x.SchoolId == schoolId && x.IsActive && x.PublishStatus).OrderBy(x => x.ClassOrder).ToList().Select(z => new Class(z)).ToList();
            if (classList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<Class>>(status, msg, classList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<Class>>(status, msg, null);
            }
        }

        public Tuple<bool, string, List<Division>> FullDivisionList(SchoolDivisionListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long classId = Convert.ToInt64(model.classId);
            var classList = _Entity.TbDivisions.Where(x => x.Class.SchoolId == schoolId && x.ClassId == classId && x.IsActive).OrderBy(x => x.Division).ToList().Select(z => new Division(z)).ToList();
            if (classList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<Division>>(status, msg, classList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<Division>>(status, msg, null);
            }
        }

        public object getUserById(long schoolId)
        {
            return _Entity.TbSchools.Where(z => z.SchoolId == schoolId).FirstOrDefault();
        }

        public Tuple<bool, string, Trip> TravelHistory(SchoolTravelHistoryPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long busId = Convert.ToInt64(model.busId);
            DateTime historyDate = Convert.ToDateTime(model.dateTime);
            int shiftStatus = Convert.ToInt32(model.timeStatus);
            //var data = _Entity.tb_Trip.Where(x => x.BusId == busId && EntityFunctions.TruncateTime(x.TripDate) == historyDate.Date && x.TripNo == model.tripNumber && x.IsActive && x.ShiftStatus == shiftStatus).ToList().Select(z => new Trip(z)).ToList();
            //if (data.Count > 0)
            //{
            //    //msg = "Success";
            //    //status = true;
            //    //if (model.timeStatus == "0")
            //    //{
            //    //    return new Tuple<bool, string, Trip>(status, msg, data.FirstOrDefault());
            //    //}
            //    //else
            //    //{
            //    //    if (data.Count > 1)
            //    //        return new Tuple<bool, string, Trip>(status, msg, data.LastOrDefault());
            //    //    else
            //    //        return new Tuple<bool, string, Trip>(status, msg, null);
            //    //}
            //    msg = "Success";
            //    status = true;
            //    return new Tuple<bool, string, Trip>(status, msg, data.FirstOrDefault());
            //}
            //var data = _Entity.SP_TravelHistory(busId, model.tripNumber, historyDate, shiftStatus).FirstOrDefault();
            var data = _Entity.sptravelhistyoryresult
         .FromSqlRaw("EXEC SP_TravelHistory {0}, {1}, {2}",
                     busId, model.tripNumber, historyDate, shiftStatus).FirstOrDefault();
        


            if (data != null)
            {
                long tripId = Convert.ToInt64(data);
                Trip tripData = _Entity.TbTrips.Where(x => x.TripId == tripId).ToList().Select(x => new Trip(x)).FirstOrDefault();
                msg = "Success";
                status = true;
                return new Tuple<bool, string, Trip>(status, msg, tripData);
            }

            else
            {
                msg = "No History";
                return new Tuple<bool, string, Trip>(status, msg, null);
            }

        }

        public Tuple<bool, string, List<Bus>> FullRunningBusList(SchoolBusListPostModel model)
        {
            var status = false;
            var msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            var currentDateTime = DateTime.UtcNow.Date;
            //var busList = _Entity.TbBus.Where(x => x.SchoolId == schoolId && x.IsActive).SelectMany(y => y.tb_Trip).Where(z => EntityFunctions.TruncateTime(z.StartTime) == currentDateTime && z.TravellingStatus == 1 && z.IsActive).ToList().Select(c => new Bus(c.TbBus)).ToList();
            var busList = _Entity.TbBus.Where(x => x.SchoolId == schoolId && x.IsActive).SelectMany(y => y.TbTrips).Where(z => z.TravellingStatus == 1 && z.IsActive).ToList().Select(c => new Bus(c.Bus)).ToList();
            //var data = _Entity.SP_CurrentRunningBus(schoolId, currentDateTime).ToList();
            //var busList = data.Select(x => new Bus(x.BusId)).ToList();
            if (busList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<Bus>>(status, msg, busList);
            }
            else
            {
                msg = "No data found";
                status = true;
                return new Tuple<bool, string, List<Bus>>(status, msg, null);
            }
        }

        public Tuple<string, bool, List<SPUnassignedTeachers>> GetFreeClass(UnassignedClassPostModel model)
        {
            string msg = "Failed";
            bool status = false;
            long schoolId = Convert.ToInt64(model.schoolId);
            //var freeClassList = _Entity.SP_UnassignedTeachers(schoolId).ToList().Select(x => new SPUnassignedTeachers(x)).ToList();

            var freeClassList=  _Entity.UnassignedTeachersResults
        .FromSqlRaw("EXEC SP_UnassignedTeachers {0}",
                    schoolId)
        .ToList();

            if (freeClassList != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<string, bool, List<SPUnassignedTeachers>>(msg, status, freeClassList);
            }
            else
            {
                msg = "No data found";
                status = false;
                return new Tuple<string, bool, List<SPUnassignedTeachers>>(msg, status, null);
            }
        }

        public Tuple<string, bool, List<SP_UnassignedDivisionResult>> GetUnassignedDivision(UnassignedDivisionPostModel model)
        {
            string msg = "Failed";
            bool status = false;
            long schoolId = Convert.ToInt64(model.schoolId);
            long classId = Convert.ToInt64(model.classId);
            //var freeDivisionList = _Entity.SP_UnassignedDivisions(schoolId, classId).ToList().Select(x => new SPUnassignedDivisions(x)).ToList();
            var freeDivisionList = _Entity.UnassignedDivisionResults
         .FromSqlRaw("EXEC SP_UnassignedDivisions {0}, {1}, {2}",
                     schoolId, classId)
         .ToList();

            if (freeDivisionList.Count > 0)
            {
                status = true;
                msg = "Success";
                return new Tuple<string, bool, List<SP_UnassignedDivisionResult>>(msg, status, freeDivisionList);
            }
            else
            {
                msg = "No such divisions";
                return new Tuple<string, bool, List<SP_UnassignedDivisionResult>>(msg, status, null);
            }
        }

        public Tuple<string, bool, Driver> GetCurrentTravellingBusDriverData(TravellingBusDriverDataPostModel model)
        {
            long busId = Convert.ToInt64(model.busId);
            string msg = "Failed";
            bool status = false;
            var busData = _Entity.TbTrips.Where(x => x.BusId == busId && x.IsActive && x.TravellingStatus != 2).FirstOrDefault();
            if (busData != null)
            {
                var driverData = _Entity.TbDrivers.Where(x => x.DriverId == busData.DriverId && x.IsActive).ToList().Select(x => new Driver(x)).FirstOrDefault();
                if (driverData != null)
                {
                    status = true;
                    msg = "Success";
                    return new Tuple<string, bool, Driver>(msg, status, driverData);
                }
                else
                {
                    msg = "No SDriver Details";
                    return new Tuple<string, bool, Driver>(msg, status, null);
                }
            }
            else
            {
                msg = "Current bus is not travelling";
                return new Tuple<string, bool, Driver>(msg, status, null);
            }
        }

        public Tuple<bool, string, List<CircularNotificationListMapModel>> GetAllCircularList(CircularNotificationListPostModel model)
        {
            bool status = false;
            string msg = "Faile";
            long schoolId = Convert.ToInt64(model.schoolId);
            List<CircularNotificationListMapModel> data = new List<CircularNotificationListMapModel>();
            //var dataFull = _Entity.tb_Circular.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.CircularId).ToList();
            //if (dataFull.Count > 0)
            //{
            //    foreach (var item in dataFull)
            //    {
            //        CircularNotificationListMapModel one = new CircularNotificationListMapModel();
            //        one.Head = "Circular ";
            //        one.CircularId = item.CircularId;
            //        one.SchoolId = item.SchoolId;
            //        one.CircularDate = item.CircularDate;
            //        one.Description = item.Description;
            //        one.FilePath = item.FilePath ?? "";
            //        one.TimeStamp = item.TimeStamp;
            //        one.FromStatus = 0;
            //        data.Add(one);
            //    }
            //}
            var eventData = _Entity.TbCalenderEvents.Where(x => x.SchoolId == schoolId && x.IsActive).OrderBy(x => x.EventId).ToList();
            if (eventData.Count > 0)
            {
                foreach (var item in eventData)
                {
                    CircularNotificationListMapModel one = new CircularNotificationListMapModel();
                    one.Head = item.EventHead;
                    one.CircularId = item.EventId;
                    one.SchoolId = item.SchoolId;
                    one.CircularDate = item.EventDate;
                    one.Description = item.EventDetails;
                    one.FilePath = "";
                    one.TimeStamp = item.TimeStamp;
                    one.FromStatus = 1;
                    data.Add(one);
                }
            }
            if (data.Count > 0)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<CircularNotificationListMapModel>>(status, msg, data);
            }
            else
            {
                msg = "No Circulars";
                status = true;
                return new Tuple<bool, string, List<CircularNotificationListMapModel>>(status, msg, data);
            }
        }

        public Tuple<bool, string, List<CircularNotificationListMapModel>> GetCircularEventWithDate(CircularEventWithDatePostModel model)
        {
            bool status = false;
            string msg = "Faile";
            long schoolId = Convert.ToInt64(model.ScholId);
            List<CircularNotificationListMapModel> data = new List<CircularNotificationListMapModel>();
            //var dataFull = _Entity.sp_CircularNotificationPerDate(model.EventDate, schoolId).ToList();
           var dataFull= _Entity.Circularresult.
                FromSqlRaw("EXEC sp_CircularNotificationPerDate {0}, {1}",
                   model.EventDate, schoolId)
        .ToList();


            if (dataFull.Count > 0)
            {
                foreach (var item in dataFull)
                {
                    CircularNotificationListMapModel one = new CircularNotificationListMapModel();
                    one.Head = item.EventHead;
                    one.CircularId = item.CircularId;
                    one.SchoolId = schoolId;
                    one.CircularDate = item.CircularDate;
                    one.Description = item.Description;
                    one.FilePath = item.FilePath ?? "";
                    one.TimeStamp = currentTime;
                    one.FromStatus = 0;
                    data.Add(one);
                }
            }
            if (dataFull.Count > 0)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, List<CircularNotificationListMapModel>>(status, msg, data);
            }
            else
            {
                msg = "No Circulars";
                status = true;
                return new Tuple<bool, string, List<CircularNotificationListMapModel>>(status, msg, data);
            }
        }


        public Tuple<bool, string, List<StaffSMSMapModel>> GetAllStaffData(SchoolClassListPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            List<StaffSMSMapModel> data = new List<StaffSMSMapModel>();
            try
            {
                #region Teacher
                StaffSMSMapModel oneData = new StaffSMSMapModel();
                oneData.MemberList = new List<MemberList>();
                var teacher = _Entity.TbTeachers.Where(x => x.SchoolId == schoolId && x.IsActive).ToList();
                if (teacher.Count > 0)
                {
                    oneData.MemberType = 1;//For Teacher
                    oneData.TypeName = "Teaching Staff ";
                    foreach (var item in teacher)
                    {
                        MemberList mem = new MemberList();
                        mem.MemmberName = item.TeacherName;
                        mem.MemberId = item.UserId;
                        mem.ContactNumber = item.ContactNumber;
                        oneData.MemberList.Add(mem);
                    }
                    data.Add(oneData);
                }
                #endregion Teacher
                #region Staff
                oneData = new StaffSMSMapModel();
                oneData.MemberList = new List<MemberList>();
                //var nonteacher = _Entity.TbStaffs.Where(x => x.TbLogins.SchoolId == schoolId && x.IsActive).ToList();
                var nonteacher = (from staff in _Entity.TbStaffs
                                  join login in _Entity.TbLogins
                                      on staff.UserId equals login.UserId
                                  where login.SchoolId == schoolId
                                        && staff.IsActive == true
                                  select staff)
                   .ToList();
                if (nonteacher.Count > 0)
                {
                    oneData.MemberType = 2;//For Staff
                    oneData.TypeName = "Non Teaching Staff";
                    foreach (var item in nonteacher)
                    {
                        MemberList mem = new MemberList();
                        mem.MemmberName = item.StaffName;
                        mem.MemberId = item.UserId;
                        mem.ContactNumber = item.Contact;
                        oneData.MemberList.Add(mem);
                    }
                    data.Add(oneData);
                }
                #endregion Staff
                status = true;
                msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new Tuple<bool, string, List<StaffSMSMapModel>>(status, msg, data);
        }

        public Tuple<bool, string, List<sp_ParentTeacherConversation_Result>> GetParentTeacherConversation(ParentTeacherConversationMapModel model)
        {
            bool status = false;
            string msg = "Failed";
            List<sp_ParentTeacherConversation_Result> data = new List<sp_ParentTeacherConversation_Result>();
            try
            {
                long studentId = Convert.ToInt64(model.StudentId);
                //data = _Entity.sp_ParentTeacherConversation(studentId, model.RequirestType, model.length).ToList();
             data= _Entity.sp_ParentTeacherConversation_Results
         .FromSqlRaw("EXEC sp_ParentTeacherConversation {0}, {1}, {2}",
                     studentId, model.RequirestType, model.length)
         .ToList();


                status = true;
                msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new Tuple<bool, string, List<sp_ParentTeacherConversation_Result>>(status, msg, data);
        }

        public Tuple<bool, string> MessageFromSchool(MessageFromSchoolPostModel.MessageDetailsFromSchoolPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                long schoolid = Convert.ToInt64(model.SchoolId);
                HttpClient client = new HttpClient();
                var history = new TbSmsHistory();
                var numbers = new List<string>();
                var MsgId = new List<string>();
                var numb = "";
                var senderName = "MYSCHO";
                var senderData = _Entity.TbSchoolSenderIds.Where(x => x.SchoolId == schoolid && x.IsActive == true).FirstOrDefault();
                if (senderData != null)
                    senderName = senderData.SenderId;
                var smsHead = new TbSmsHead();

                if (model.TypeId == (int)SchoolMsgFromApp.FullSchool)
                {
                    smsHead.Head = "Message From SchoolMan App(All Classes)";
                    smsHead.SchoolId = schoolid;
                    smsHead.TimeStamp = currentTime;
                    smsHead.IsActive = true;
                    smsHead.SenderType = (int)SMSSendType.Student;
                    _Entity.TbSmsHeads.Add(smsHead);
                    status = _Entity.SaveChanges() > 0;
                    msg = "Successful";
                    var list = _Entity.TbStudents.Where(x => x.IsActive && x.SchoolId == schoolid).ToList();
                    foreach (var item in list)
                    {
                        string messagepre = "Dear Parent of " + item.StundentName;
                        messagepre = messagepre + ", " + model.Message;
                        //-----SPECIAL CHARACTER SENDING -------------------
                        messagepre = messagepre.Replace("#", "%23");
                        messagepre = messagepre.Replace("&", "%26");
                        //--------------------------------------------------
                        var phone = item.ContactNumber.ToString();
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
                        alvosmsResp respList= JsonSerializer.Deserialize<alvosmsResp>(responseText);
                        if (status)
                        {
                            TbSmsHistory sms = new TbSmsHistory();
                            sms.IsActive = true;
                            sms.MessageContent = model.Message;
                            sms.MessageDate = currentTime;
                            sms.ScholId = schoolid;
                            sms.StuentId = item.StudentId;
                            sms.MobileNumber = phone;
                            sms.HeadId = smsHead.HeadId;
                            sms.SendStatus = Convert.ToString(respList.success);
                            if (respList.data != null)
                            {
                                sms.MessageReturnId = respList.data[0].messageId;
                                sms.DelivaryStatus = "Pending";
                            }
                            sms.SmsSentPerStudent = smsCount;
                            _Entity.TbSmsHistories.Add(sms);
                            _Entity.SaveChanges();
                        }
                    }

                }
                else
                {
                    smsHead.Head = "Message From SchoolMan App ";
                    smsHead.SchoolId = schoolid;
                    smsHead.TimeStamp = currentTime;
                    smsHead.IsActive = true;
                    smsHead.SenderType = (int)SMSSendType.Student;
                    _Entity.TbSmsHeads.Add(smsHead);
                    status = _Entity.SaveChanges() > 0;
                    msg = "Successful";
                    foreach (var item in model.MultipleClass)
                    {
                        long classid = Convert.ToInt64(item.ClassId);
                        long divisionid = Convert.ToInt64(item.DivisionId);
                        var list = _Entity.TbStudents.Where(x => x.IsActive && x.SchoolId == schoolid && x.ClassId == classid && x.DivisionId == divisionid).ToList();
                        foreach (var student in list)
                        {
                            string messagepre = "Dear Parent of " + student.StundentName;
                            messagepre = messagepre + ", " + model.Message;
                            //-----SPECIAL CHARACTER SENDING -------------------
                            messagepre = messagepre.Replace("#", "%23");
                            messagepre = messagepre.Replace("&", "%26");
                            //--------------------------------------------------
                            var phone = student.ContactNumber.ToString();
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
                            alvosmsResp respList=  JsonSerializer.Deserialize<alvosmsResp>(responseText);


                            if (status)
                            {
                                TbSmsHistory sms = new TbSmsHistory();
                                sms.IsActive = true;
                                sms.MessageContent = model.Message;
                                sms.MessageDate = currentTime;
                                sms.ScholId = schoolid;
                                sms.StuentId = student.StudentId;
                                sms.MobileNumber = phone;
                                sms.HeadId = smsHead.HeadId;
                                sms.SendStatus = Convert.ToString(respList.success);
                                if (respList.data != null)
                                {
                                    sms.MessageReturnId = respList.data[0].messageId;
                                    sms.DelivaryStatus = "Pending";
                                }
                                sms.SmsSentPerStudent = smsCount;
                                _Entity.TbSmsHistories.Add(sms);
                                _Entity.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new Tuple<bool, string>(status, msg);
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

        public Tuple<bool, string, List<sp_ParentTeacherConversationFull_Result>> GetParentTeacherConversationFull(ParentTeacherMapModel model)
        {
            bool status = false;
            string msg = "Failed";
            List<sp_ParentTeacherConversationFull_Result> data = new List<sp_ParentTeacherConversationFull_Result>();
            try
            {
                long studentId = Convert.ToInt64(model.StudentId);
                //data = _Entity.sp_ParentTeacherConversationFull(studentId,0, model.length).ToList();
               data= _Entity.sp_ParentTeacherConversationFull_Results
        .FromSqlRaw("EXEC sp_ParentTeacherConversationFull {0}, {1}, {2}",
                    studentId, 0, model.length)
        .ToList();


                status = true;
                msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return new Tuple<bool, string, List<sp_ParentTeacherConversationFull_Result>>(status, msg, data);
        }

        public Tuple<bool, string, List<sp_TeacherConversation_Result>> GetTeacherConversation(TeacherConversationMapModel model)
        {
            bool status = false;
            string msg = "Failed";
            List<sp_TeacherConversation_Result> data = new List<sp_TeacherConversation_Result>();
            try
            {
                long studentId = Convert.ToInt64(model.StudentId);
                long teacherId = Convert.ToInt64(model.TeacherId);
                //data = _Entity.sp_TeacherConversation(studentId, teacherId, model.Length).ToList();
               data=_Entity.sp_TeacherConversation_Results
                            .FromSqlRaw("EXEC sp_TeacherConversation {0}, {1}, {2}",
                    studentId, teacherId, model.Length)
        .ToList();

                status = true;
                msg = "Successful";
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }
            return new Tuple<bool, string, List<sp_TeacherConversation_Result>>(status, msg, data);
        }
    }
}

