using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Satluj_Latest;

using Satluj_Latest.Helper;
using Satluj_Latest.Models;
using Satluj_Latest.PostModel;
using Satluj_Latest.Repository;
using Satluj_Latest.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;



//using CCA.Util;
//using System.Web.Helpers;

namespace Satluj_Latest.Controllers
{
    
    public class AccountController : PreLoginController
    {
        private readonly SchoolDbContext _Entities;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //
        // GET: /Account/
        public DateTime currentTime = DateTime.UtcNow;

        public AccountController(SchoolDbContext Entities, IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor, SchoolRepository schoolRepository,
                    ParentRepository parentRepository) : base(schoolRepository, parentRepository)
        {
            _Entities = Entities;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public IActionResult LoginPage()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult SchoolLogin()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Disclaimer()
        {
            return View();
        }
        public IActionResult Privacypolicy()
        {
            return View();
        }
        public IActionResult TermsConditions()
        {
            return View("Terms&Conditions");
        }
        public IActionResult SuperAdminLogin()
        {
            return View();
        }
        public IActionResult LoginParent()
        {
            return View();
        }
        public IActionResult ParentLogin()
        {
            return View();
        }
        public IActionResult ParentRegistration()
        {
            return View();
        }
        //Added by Gayathri 
        public async Task<IActionResult> ChkSuperAdminLogin(LoginModel model)
        {
            try
            {

                var user = _Entities.TbLoginAdmins.Where(x => x.UserName.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive).ToList().Where(a => a.UserName.ToLower() == model.Email.ToLower() && a.Password == model.Password).FirstOrDefault();
                if (user != null)
                {
                    // 1. Create claims
                    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.AdminId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName)
    };

                    // 2. Create identity & principal
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // 3. Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // 4. Store session (optional)
                    HttpContext.Session.SetString("UserName", user.UserName);

                    return Json(new { status = true, msg = "Success" });
                }
                else
                {
                    return Json(new { status = false, msg = "Username/Password incorrect" });
                }

            }

            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.InnerException.InnerException });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ParentRegister(ParentRegisterModel model)
        {
            bool Status = false;
            string Message = "Failed";
            try
            {
                if (_Entities.TbParents.Any(x => x.Email.ToLower() == model.email.ToLower() && x.IsActive))
                {
                    Message = "Email Already Exists";
                    return Json(new { Status = Status, Message = Message });
                }
                else
                {
                    var newModel = new ParentRegistrationPostModel();
                    newModel.ParentName = model.parentName;
                    newModel.Address = model.address;
                    newModel.City = model.city;
                    newModel.Email = model.email;
                    newModel.ContactNumber = model.contactNo;
                    newModel.Password = model.password;
                    newModel.FilePath = model.FilePath;
                    newModel.image = model.image;
                    newModel.Password = model.password;
                    newModel.State = model.state;

                    Tuple<bool, string, TbParent> data = _parentRepository.AddParent(newModel);
                    Status = data.Item1;
                    Message = data.Item2;
                    if (Status)
                    {
                        // 1. Set Authentication Cookie using ASP.NET Core Identity
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, data.Item3.ParentId.ToString()),
                                new Claim("UserType", ((int)UserRole.Parent).ToString())
                            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // 2. Set Cookie
                        Response.Cookies.Append("UserType",
                            ((int)UserRole.Parent).ToString(),
                            new CookieOptions { HttpOnly = true });

                        // 3. Session Values
                        HttpContext.Session.SetString("UserType", ((int)UserRole.Parent).ToString());
                        HttpContext.Session.SetString("Parent", Newtonsoft.Json.JsonConvert.SerializeObject(data.Item3));

                        // 4. Redirect
                        return RedirectToAction("ParentHome", "Parent");
                    }

                    else
                    {
                        return Json(new { Status = Status, Message = Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = Status, Message = Message });
            }
        }
        public ActionResult SchoolRegistration()
        {
            return View();
        }
        public ActionResult StudentDetails()
        {
            return View();
        }
        public ActionResult StaffLogin()
        {
            return View();
        }
        public ActionResult MasterLogin()
        {
            return View();
        }
        public async Task<IActionResult> CheckLogin(LoginModel model)
        {
            try
            {
                if (model.userType == (int)UserRole.School)
                {
                    var user = _Entities.TbLogins.Where(x => x.Username.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive == true && x.RoleId == (int)UserRole.School).ToList().Where(a => a.Username.ToLower() == model.Email.ToLower() && a.Password == model.Password).FirstOrDefault();
                    if (user != null)
                    {
                        // 1. Create auth cookie
                        var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                                    new Claim("UserType", user.RoleId.ToString()),
                                    new Claim("IsAdmin", "true")
                                };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // 2. Cookies
                        Response.Cookies.Append("UserType", user.RoleId.ToString(),
                            new CookieOptions { HttpOnly = true });

                        // 3. Session
                        HttpContext.Session.SetString("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserType", user.RoleId.ToString());
                        HttpContext.Session.SetString("IsAdmin", "true");

                        if (user.RoleId == (int)UserRole.School)
                        {
                            //Session.Timeout = 180;
                            return Json(new { status = true, msg = "Success", userType = 1 });
                        }
                        else if (user.RoleId == (int)UserRole.Staff)
                        {
                            //ModelState.AddModelError("CustomError", "Username/Password not matching.");                      
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else if (user.RoleId == (int)UserRole.Teacher)
                        {
                            //ModelState.AddModelError("CustomError", "Username/Password not matching.");                      
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else if (user.RoleId == (int)UserRole.Master)
                        {
                            //ModelState.AddModelError("CustomError", "Username/Password not matching.");                      
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Username/Password incorrect" });

                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username/Password incorrect" });

                    }
                }
                else if ((model.userType == (int)UserRole.Staff) || (model.userType == (int)UserRole.Teacher))
                {
                    var user = _Entities.TbLogins.Where(x => x.Username.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive == true && (x.RoleId == (int)UserRole.Staff) || (x.RoleId == (int)UserRole.Teacher)).ToList().Where(a => a.Username.ToLower() == model.Email.ToLower() && a.Password == model.Password).FirstOrDefault();
                    if (user != null)
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                                new Claim("UserType", user.RoleId.ToString())
                            };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Persistent = true (remember me)
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties { IsPersistent = true });

                        // Write cookie
                        Response.Cookies.Append("UserType",
                            user.RoleId.ToString(),
                            new CookieOptions { HttpOnly = true });

                        // Session
                        HttpContext.Session.SetString("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserType", user.RoleId.ToString());


                        if (user.RoleId == (int)UserRole.Teacher)
                        {
                            var isAdmin = _Entities.TbTeachers.Where(x => x.UserId == user.UserId).FirstOrDefault();
                            if(isAdmin.UserType!=null)
                            {
                                bool IsAdminCheck= _Entities.TbUserModuleMains.Where(x => x.Id == isAdmin.UserType && x.IsActive).Select(x => x.IsAdmin).FirstOrDefault()??false;
                                HttpContext.Session.SetString("IsAdmin", IsAdminCheck.ToString());
                            }
                            else
                            {
                                HttpContext.Session.SetString("IsAdmin", false.ToString());
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetString("IsAdmin", false.ToString());
                        }
                        if (user.RoleId == (int)UserRole.School)
                        {
                            //Session.Timeout = 180;
                            return Json(new { status = true, msg = "Success", userType = 1 });
                        }
                        else if (user.RoleId == (int)UserRole.Staff)
                        {
                            //ModelState.AddModelError("CustomError", "Username/Password not matching.");                      
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else if (user.RoleId == (int)UserRole.Teacher)
                        {
                            //ModelState.AddModelError("CustomError", "Username/Password not matching.");                      
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else if (user.RoleId == (int)UserRole.Master)
                        {
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Username/Password incorrect" });

                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username/Password incorrect" });

                    }
                }
                else if (model.userType == (int)UserRole.Parent)
                {
                    var parent = _Entities.TbParents.Where(x => x.Email.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive == true).ToList().Where(a => a.Email.ToLower() == model.Email.ToLower() && a.Password == model.Password).FirstOrDefault();
                    _parentUser = parent;
                    if (parent != null)
                    {
                        // 1. Create Authentication Cookie using Claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, parent.ParentId.ToString()),
                            new Claim("UserType", model.userType.ToString()),
                            new Claim("IsAdmin", "false")
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // true = persistent cookie (same as FormsAuthentication "remember me")
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties { IsPersistent = true }
                        );


                        // 2. Write normal cookie (UserType)
                        Response.Cookies.Append(
                            "UserType",
                            model.userType.ToString(),
                            new CookieOptions { HttpOnly = true }
                        );


                        // 3. Store session values (must be strings / JSON)
                        HttpContext.Session.SetString("Parent", Newtonsoft.Json.JsonConvert.SerializeObject(parent));
                        HttpContext.Session.SetString("UserType", model.userType.ToString());
                        HttpContext.Session.SetString("IsAdmin", "false");

                        //Session.Timeout = 180;
                        return Json(new { status = true, msg = "Success", userType = (int)UserRole.Parent });
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username/Password incorrect" });

                    }
                }
                if (model.userType == (int)UserRole.Master)// Archana created new Role as Master for tthe satluj school , they have the same functionality of School 
                {
                    var user = _Entities.TbLogins.Where(x => x.Username.ToLower() == model.Email.ToLower() && x.Password == model.Password && x.IsActive == true && x.RoleId == (int)UserRole.Master).ToList().Where(a => a.Username.ToLower() == model.Email.ToLower() && a.Password == model.Password).FirstOrDefault();
                    if (user != null)
                    {
                        // 1. Authentication Cookie (replaces FormsAuthentication)
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim("UserType", user.RoleId.ToString()),
                            new Claim("IsAdmin", "true")
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties { IsPersistent = false }   // false = don’t remember me
                        );


                        // 2. Write normal cookie for UserType
                        Response.Cookies.Append(
                            "UserType",
                            user.RoleId.ToString(),
                            new CookieOptions { HttpOnly = true }
                        );


                        // 3. Store Session values (must be string or JSON)

                        HttpContext.Session.SetString("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserType", user.RoleId.ToString());
                        HttpContext.Session.SetString("IsAdmin", "true");

                        if (user.RoleId == (int)UserRole.Master)
                        {
                            return Json(new { status = true, msg = "Success", userType = 5 });
                        }
                        else if (user.RoleId == (int)UserRole.Staff)
                        {
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else if (user.RoleId == (int)UserRole.Teacher)
                        {
                            return Json(new { status = true, msg = "Success", userType = user.RoleId });
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Username/Password incorrect" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username/Password incorrect" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Username/Password incorrect" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.InnerException?.Message ?? ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> SchoolRegistrationSubmit(SchoolRegisterModel model)
        {
            bool Status = false;
            string Message = "Failed";
            long schoolId = 0;
            string latData = "";
            string longData = "";
            try
            {
                if (_Entities.TbLogins.Any(x => x.Username.ToLower() == model.emailaddress.ToLower() && x.IsActive))
                {
                    Message = "Email Already Exists";
                    return Json(new { Status = Status, Message = Message });
                }
                else
                {
                    try
                    {
                        var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(model.address));
                        var request = WebRequest.Create(requestUri);
                        var response = request.GetResponse();
                        var xdoc = XDocument.Load(response.GetResponseStream());
                        var result = xdoc.Element("GeocodeResponse").Element("result");
                        var locationElement = result.Element("geometry").Element("location");
                        var lat = locationElement.Element("lat");
                        var lon = locationElement.Element("lng");
                        latData = lat.Value.ToString();
                        longData = lon.Value.ToString();
                    }
                    catch (Exception ex)
                    {

                        latData = "9.387137";
                        longData = "76.547018";
                    }
                    if (latData != "")
                    {

                        var school = new TbSchool();
                        school.SchoolGuidId = Guid.NewGuid();
                        school.SchoolName = model.schoolName;
                        school.Address = model.address;
                        school.Contact = model.contactNumber;
                        school.IsActive = true;
                        school.City = model.city;
                        school.State = model.state;
                        school.TimeStamp = CurrentTime;
                        school.FilePath = model.FilePath;
                        school.Website = model.website;
                        school.Latitude = latData;
                        school.Longitude = longData;
                        _Entities.TbSchools.Add(school);
                        Status = _Entities.SaveChanges() > 0;
                        if (Status)
                        {
                            var login = new TbLogin();
                            login.SchoolId = school.SchoolId;
                            login.RoleId = 1;
                            login.Name = school.SchoolName;
                            login.Username = model.emailaddress;
                            login.Password = model.password;
                            login.IsActive = true;
                            login.TimeStamp = CurrentTime;
                            login.DisableStatus = false;
                            login.LoginGuid = Guid.NewGuid();
                            _Entities.TbLogins.Add(login);
                            Status = _Entities.SaveChanges() > 0;
                            Message = "Success";
                        }
                        schoolId = school.SchoolId;
                        //return new Tuple<bool, string, School>(status, msg, schoolData);

                    }
                    else
                    {
                        Message = "Invalid Address";
                        return Json(new { Status = Status, Message = Message });
                    }

                }
                if (Status)
                {
                    var schoolData = _Entities.TbSchools.Where(x => x.SchoolId == schoolId && x.IsActive)
                                                         .ToList()
                                                         .Select(x => new TbSchool // Assuming 'School' is aliased or needs the full path
                                                         {
                                                             // Map necessary properties here, e.g.:
                                                             SchoolId = x.SchoolId,
                                                             SchoolName = x.SchoolName,
                                                             IsActive = x.IsActive
                                                             // ... continue mapping all required fields
                                                         })
                                                         .FirstOrDefault();
                    // 1. Create claims
                    var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, schoolData.TbLogins.First().UserId.ToString()),
    new Claim("UserType", ((int)UserRole.School).ToString())
};

                    // 2. Sign in (replaces FormsAuthentication.SetAuthCookie)
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties { IsPersistent = true }
                    );

                    // 3. Store cookie
                    Response.Cookies.Append("UserType", ((int)UserRole.School).ToString());

                    // 4. Store session values
                    var user = _Entities.TbLogins.FirstOrDefault(x => x.UserId == schoolData.TbLogins.FirstOrDefault().UserId);

                    HttpContext.Session.SetString("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                    HttpContext.Session.SetString("UserType", ((int)UserRole.School).ToString());


                    return Json(new { Status = Status, Message = Message });
                }
                else
                {
                    return Json(new { Status = Status, Message = Message });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = Status, Message = Message });
            }
        }

        [HttpPost]
        public async Task<object> Contact(ContactUsModel model)
        {
            bool Status = false;
            string Message = "Failed";
            var email = model.email;
            var contactNo = model.contactNo;
            var schoolName = model.schoolName;
            var message = model.message;
            //var contact = new tb_ContactUs();
            //contact.Name = model.name;
            //contact.Email = model.email;
            //contact.ContactNo = model.contactNo;
            //contact.SchoolName = model.schoolName;
            //contact.Message = model.message;
            //contact.ReplyStatus = false;
            //contact.IsActive = true;
            //contact.ContactDate = CurrentTime;
            //Entities.tb_ContactUs.Add(contact);
            //Status = Entities.SaveChanges() > 0;

            //var state = false;
            var description = "failed";
            //try
            //{

            //    var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/email/ContactUs.html");
            //    var emailTemplate = System.IO.File.ReadAllText(filePath);
            //    var mailBody = emailTemplate.Replace("{{user}}", model.name);


            //    Mail.Send("Child Academy - Contact", mailBody, model.name, new System.Collections.ArrayList { model.email });

            //    state = true;
            //    description = "success";

            //}
            //catch (Exception exx)
            //{
            //    state = false;
            //    description = "Something went wrong";
            //}

            try
            {

                var filePath = Path.Combine(_env.WebRootPath, "Content", "email", "ContactAdmin.html");
                var emailTemplate = System.IO.File.ReadAllText(filePath);

                var mailBody = emailTemplate
                    .Replace("{{user}}", model.name)
                    .Replace("{{messgae}}", model.message)
                    .Replace("{{email}}", model.email)
                    .Replace("{{contactNo}}", model.contactNo)
                    .Replace("{{schoolName}}", model.schoolName);

                 Send("School Man - ContactUs", mailBody, "info.schoolman@gmail.com");

                Status = true;
                description = "success";


            }
            catch
            {
                Status = false;
                description = "Something went wrong";
            }

            Message = Status ? description : "Something went wrong!";
            return Json(new { Status = Status, Message = Message });
        }
        public PartialViewResult SearchAdmission(string id)
        {
            StudentModel model = new StudentModel();
            string[] splitData = id.Split('~');
            model.admissionNo = splitData[0];
            model.schoolId = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/Account/_pv_StudentDetailsNonLoginParent.cshtml", model);
        }

        public ActionResult BillingDetails(string id)
        {
            string[] splitData = id.Split('~');
            long studentId = Convert.ToInt64(splitData[0]);
            string admissionNo = splitData[1];

            var student = new Satluj_Latest.Data.Student(studentId);
            FeeModel model = new FeeModel();
            if (student.StudentSpecialId == admissionNo)
            {
                model.SchoolModel = new SchoolModel();
                model.SchoolModel.studentName = student.StundentName;
                model.SchoolModel.classNumber = student.ClasssNumber; //Archana
                model.SchoolModel.className = student.ClassName;
                model.SchoolModel.division = student.DivisionName;
                model.SchoolModel.classInCharge = student.Teacher == null ? "Not Assigned" : student.Teacher.TeacherName;
                model.SchoolModel.classId = student.ClassId;
                model.SchoolModel.studentId = studentId;
            }


            return View(model);
        }

        public PartialViewResult StudentHistoryBillPartialView(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentId = Convert.ToInt64(id);
            return PartialView("~/Views/Account/_pv_History_Billing_StudentFee_Model.cshtml", model);
        }
        public PartialViewResult LoadTableForBilling(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            string[] splitData = id.Split('~');
            model.SchoolModel.studentId = Convert.ToInt64(splitData[1]);
            model.BillNumber = Convert.ToInt64(splitData[0]);
            return PartialView("~/Views/Account/_pv_History_PopupGrid.cshtml", model);
        }


        [HttpPost]
        public object StudentMainBillPay(FeeModel model)
        {
            decimal sumAmt = 0;
            bool status = false;
            string message = "Failed";
            List<string> feeDetails = model.FeeDetails.Split(',').ToList();
            long SchoolId = model.SchoolId;
            long ClassId = model.ClassId;
            long StudentId = model.StudentId;
            DateTime BillDate = CurrentTime;
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
                int isAmountEdit = Convert.ToInt16(splitData[5]);
                if (isAmountEdit != 0)
                {
                    var paymentList = new Satluj_Latest.Data.Student(StudentId).GetStudentPaymentFees(ClassId,StudentId).Result.OrderBy(z => z.DueDate).ToList();
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
                            _Entities.TbFeeDues.Add(due);
                            // status = _Entities.SaveChanges() > 0 ? true : false;
                        }
                    }
                }

                payment.Amount = paymentAmount;
                sumAmt = sumAmt + payment.Amount;

                payment.BillNo = BillNo;
                payment.IsPaid = false;
                payment.PaymentType = 2;
                payment.PaymentGuid = PaymentGuid;
                payment.StudentId = StudentId;
                payment.ClassId = ClassId;
                payment.SchoolId = SchoolId;
                payment.TimeStamp = BillDate;
                payment.IsActive = true;
                _Entities.TbPayments.Add(payment);
                status = _Entities.SaveChanges() > 0 ? true : false;

            }
            var billNo1 = _Entities.TbPaymentBillNos.Where(z => z.SchoolId == SchoolId).FirstOrDefault();
            if (billNo1 != null)
            {
                billNo.BillNo = BillNo;
                status = _Entities.SaveChanges() > 0 ? true : false;

            }
            bool ispayable = false;
            decimal payableAmount = 0;
            decimal prevBal = 0;
            try
            {
                decimal bal = 0;
                //bool balAndCash = false;

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
                    studentPaidsAmount.IsActive = true;
                    _Entities.TbStudentPaidAmounts.Add(studentPaidsAmount);
                    status = _Entities.SaveChanges() > 0 ? true : false;



                }
                //else if (bal != 0)
                //{
                //    var studentPaidsAmount = new TbStudentsPaidAmount();
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

            }
            catch (Exception ex)
            {


            }

            var studDetails = _Entities.TbStudents.Where(z => z.StudentId == StudentId && z.IsActive == true).FirstOrDefault();
            var dateTime = BillDate.ToString("dd-MMM-yyyy");

            var description = "failed";
            bool isGateway = false;
            if (ispayable || (prevBal == 0))
            {
                var amountTopay = payableAmount == 0 ? sumAmt : payableAmount;
                isGateway = true;
                Guid guid = Guid.NewGuid();
                string reference = guid.ToString().Replace("-", string.Empty).Substring(0, 10).ToUpper();
                HttpContext.Session.SetString("REFERENCE", reference);
                //var payment = Entities.tb_Payment.Where(x => x.UserId == _user.UserId && x.PaymentType == 1).FirstOrDefault();
                PaymentModels pay = new PaymentModels();
                pay.ReferenceNo = HttpContext.Session.GetString("REFERENCE");
                var student = studDetails;
                pay.Description = "Payment";
                //string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                pay.ReturnUrl = baseUrl + "/Account/ccavResponseHandler";
                //pay.ReturnUrl = "http://localhost:16138/Parent/ccavResponseHandler";
                pay.Name = student.ParentName;
                pay.Address = student.Address;
                pay.City = student.City;
                pay.State = student.State;
                pay.PostalCode = student.PostalCode;
                pay.PhoneNo = student.ContactNumber;
                pay.Email = student.ParentEmail;
                pay.Amount = Convert.ToDouble(amountTopay);
                pay.CourseName = "Bill";
                pay.BillNo = BillNo;
                pay.SchoolId = SchoolId;
                pay.StudentId = StudentId;
                // Session["PaymentPostData"] = pay;
                HttpContext.Session.SetString("PaymentPostData", JsonConvert.SerializeObject(pay));
            }
            //try
            //{
            //    var smtpDetails = _Entities.tb_SMTPDetail.Where(z => z.SchoolId == SchoolId).FirstOrDefault();

            //    var paidAmount = Convert.ToInt32(payment.Amount);
            //    var filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/email/FeePayment.html");
            //    var emailTemplate = System.IO.File.ReadAllText(filePath);
            //    var mailBody = emailTemplate.Replace("{{schoolname}}", studDetails.tb_School.SchoolName)
            //       .Replace("{{parent}}", studDetails.ParentName)
            //    .Replace("{{student}}", studDetails.StundentName)
            //    .Replace("{{amount}}", string.Format("{0:0.00}", sumAmt))
            //    .Replace("{{date}}", dateTime);
            //    Mail.Send("School Fee Payment", mailBody, studDetails.ParentName, smtpDetails.EmailId, smtpDetails.Password, new System.Collections.ArrayList { studDetails.ParentEmail });


            //    description = "success";

            //}
            //catch
            //{

            //    description = "Something went wrong";
            //}
            return Json(new { status = status, serialNo = BillNo, payment = isGateway, msg = status ? "Bill Paid Sucessfully" : "Failed To Pay Bill" });
        }
        public PartialViewResult PrintAccountBillData(string id)
        {
            string[] splitData = id.Split('~');

            var model = new PrintBill();
            model.StudentId = Convert.ToInt64(splitData[0]);
            model.BillNumber = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/Account/_pv_PrintAccountBillData.cshtml", model);
        }
        #region PaymentGateway
        public ActionResult CoursePayment(string id)
        {

            bool status = true;
            int caseSwitch = Convert.ToInt16(id);

            float amount = 1;
            string course = "";
            switch (caseSwitch)
            {
                case 1:
                    amount = 24000;
                    course = "Core Php with responsive web";
                    break;
                case 2:
                    amount = 22000;
                    course = "Dotnet";
                    break;
                case 3:
                    amount = 10000;
                    course = "Java";
                    break;
                case 4:
                    amount = 24000;
                    course = "Android";
                    break;
                case 5:
                    amount = 23000;
                    course = "Ios";
                    break;
                case 6:
                    amount = 230;
                    course = "ionic";
                    break;
                default:
                    status = false;
                    break;
            }
            Guid guid = Guid.NewGuid();
            string reference = guid.ToString().Replace("-", string.Empty).Substring(0, 10).ToUpper();
            HttpContext.Session.SetString("REFERENCE", reference);
            //var payment = Entities.tb_Payment.Where(x => x.UserId == _user.UserId && x.PaymentType == 1).FirstOrDefault();
            PaymentModels pay = new PaymentModels();
            pay.ReferenceNo = HttpContext.Session.GetString("REFERENCE");
            var student = _Entities.TbParents
                .FirstOrDefault(z => z.ParentId == _parentUser.ParentId); pay.Description = "Payment";
            //string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            pay.ReturnUrl = baseUrl + "/Account/ccavResponseHandler";
            //pay.ReturnUrl = "http://localhost:16138/Parent/ccavResponseHandler";
            pay.Name = student.ParentName;
            pay.Address = student.Address;
            pay.City = student.City;
            pay.State = student.State;
            pay.PostalCode = student.PostalCode;
            pay.PhoneNo = student.ContactNumber;
            pay.Email = student.Email;
            pay.Amount = amount;
            pay.CourseName = course;
            pay.BillNo = 11;
            pay.SchoolId = 1;
            pay.StudentId = 1;
            HttpContext.Session.SetString("PaymentPostData", JsonConvert.SerializeObject(pay));
            //return View("CCAVRequestHandler",pay);
            return Json(new { status = status });
        }
        public ActionResult PaymentPost()
        {
            PaymentModels model = new PaymentModels();
            PaymentModels pay = new PaymentModels();
            if (HttpContext.Session.GetString("REFERENCE") != null)
            {
                model.ReferenceNo = HttpContext.Session.GetString("REFERENCE").ToString();
            }
            model.Amount = pay.Amount;
            model.Description = "Payment";
            //string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            //model.ReturnUrl = "http://localhost:16138/Student/PaymentResponse";
            string payJson = HttpContext.Session.GetString("PaymentPostData");
              pay = JsonConvert.DeserializeObject<PaymentModels>(payJson);
            
            //pay = (PaymentModels)Session["PaymentPostData"];
            model.ReturnUrl = pay.ReturnUrl; //http://localhost:16138/Parent/PaymentResponse";//"http://localhost:16138/Parent/ccavResponseHandler";
            model.Name = pay.Name;
            model.Address = pay.Address;
            model.City = pay.City;
            model.State = pay.State;
            model.PostalCode = pay.PostalCode;
            model.PhoneNo = pay.PhoneNo;
            model.Email = pay.Email;
            model.Amount = pay.Amount;
            return View(model);
        }
        public ActionResult CCAVRequestHandler()
        {

            PaymentModels pay = new PaymentModels();
            CCACrypto ccaCrypto = new CCACrypto();
            string workingKey = "3891AA5249F6E3DBA928422EB4BA18DD";//put in the 32bit alpha numeric key in the quotes provided here 	
            string ccaRequest = "";
            string strEncRequest = "";
            string iframeSrc = "";
            string strAccessCode = "AVTM75FA75BW58MTWB";// put the access key in the quotes provided here.


            //NameValueCollection nameValue = (Request.Form.Count > 0) ? Request.Form : Request.QueryString;
            //SortedDictionary<string, string> sortedDict = NameValueCreator.SortNameValueCollection(nameValue);

            Dictionary<string, string> nameValue;

            if (Request.HasFormContentType)
            {
                nameValue = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
            }
            else
            {
                nameValue = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
            }

            SortedDictionary<string, string> sortedDict =
                new SortedDictionary<string, string>(nameValue, StringComparer.Ordinal);

            foreach (KeyValuePair<string, string> p in sortedDict)
            {
                if (p.Key != null)
                {
                    if (!p.Key.StartsWith("_"))
                    {
                        ccaRequest = ccaRequest + p.Key + "=" + p.Value + "&";
                        /* Response.Write(name + "=" + Request.Form[name]);
                          Response.Write("</br>");*/
                    }
                }
            }
            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            iframeSrc = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + strEncRequest + "&access_code=" + strAccessCode;
            pay.iframeSrc = iframeSrc;
            //pay.strEncRequest = strEncRequest;
            //pay.strAccessCode = strAccessCode;
            return View(pay);
        }
        public ActionResult ccavResponseHandler()
        {
            bool status = false;
            PaymentModels model = new PaymentModels();
            PaymentModels pay = new PaymentModels();
            var payJson = HttpContext.Session.GetString("PaymentPostData");
                pay = JsonConvert.DeserializeObject<PaymentModels>(payJson);
            string workingKey = "3891AA5249F6E3DBA928422EB4BA18DD";//put in the 32bit alpha numeric key in the quotes provided here
            CCACrypto ccaCrypto = new CCACrypto();
            string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"], workingKey);
            NameValueCollection Params = new NameValueCollection();
            string[] segments = encResponse.Split('&');
            foreach (string seg in segments)
            {
                string[] parts = seg.Split('=');
                if (parts.Length > 0)
                {
                    string Key = parts[0].Trim();
                    string Value = parts[1].Trim();
                    Params.Add(Key, Value);
                }
            }
            var ccavenueTable = new TbCcavenueCourseResponse();
            var amt = "";
            bool isSuccess = false;
            for (int i = 0; i < Params.Count; i++)
            {
                if (Params.Keys[i] == "order_id")
                {
                    ccavenueTable.OrderId = Params[i];
                }
                else if (Params.Keys[i] == "order_status")
                {
                    ccavenueTable.OrderStatus = Params[i] == "Failure" ? false : true;
                    model.PaymentStatus = Params[i];
                    isSuccess = Params[i] == "Failure" ? false : true;
                }
                else if (Params.Keys[i] == "payment_mode")
                {
                    ccavenueTable.PaymentMode = Params[i];
                }
                else if (Params.Keys[i] == "tracking_id ")
                {
                    ccavenueTable.TrackingId = Params[i];
                }
                else if (Params.Keys[i] == "amount")
                {
                    ccavenueTable.Amount = Convert.ToDecimal(Params[i]);
                    amt = Params[i];
                }
                // Response.Write(Params.Keys[i] + " = " + Params[i] + "<br>");
            }
            ccavenueTable.ParentId = 1;
            ccavenueTable.Course = pay.CourseName;
            ccavenueTable.BillNo = pay.BillNo;
            ccavenueTable.SchoolId = pay.SchoolId;

            // ccavenueTable.Amount = Convert.ToDecimal(pay.Amount);
            _Entities.TbCcavenueCourseResponses.Add(ccavenueTable);
            _Entities.SaveChanges();
            if (isSuccess)
            {
                //var studDetails = _Entities.TbStudents.Where(z => z.StudentId == pay.StudentId).FirstOrDefault();
                var paymentList = _Entities.TbPayments.Where(z => z.StudentId == pay.StudentId && z.BillNo == pay.BillNo).ToList();
                foreach (var item in paymentList)
                {
                    item.IsPaid = true;
                    _Entities.SaveChanges();
                }
                try
                {
                    var history = new TbSmsHistory();
                    var numbers = new List<string>();
                    var MsgId = new List<string>();

                    var numb = "";
                    string messagepre = "";


                    var senderName = "MYSCHO";

                    var senderData = _Entities.TbSchoolSenderIds.Where(x => x.SchoolId == pay.SchoolId && x.IsActive == true).FirstOrDefault();
                    if (senderData != null)
                        senderName = senderData.SenderId;
                    status = true;

                    var smsHead = new TbSmsHead();
                    var firstPayment = paymentList.FirstOrDefault();
                    smsHead.Head = "BillDate Payment " + paymentList.FirstOrDefault().Student.StundentName;

                    smsHead.SchoolId = _user.SchoolId;
                    smsHead.TimeStamp = CurrentTime;
                    smsHead.IsActive = true;
                    smsHead.SenderType = (int)SMSSendType.Student;
                    _Entities.TbSmsHeads.Add(smsHead);
                    status = _Entities.SaveChanges() > 0;
                    var second = paymentList.FirstOrDefault();
                    messagepre = "Dear Parent of " + paymentList.FirstOrDefault().Student.StundentName + ", you have paid Rs." + string.Format("{0:0.00}", amt) + " on " + CurrentTime;

                    var phone = paymentList.FirstOrDefault().Student.ContactNumber.ToString();
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
                    alvosmsResp respList =  JsonConvert.DeserializeObject<alvosmsResp>(responseText);

                    if (status)
                    {
                        TbSmsHistory sms = new TbSmsHistory();
                        sms.IsActive = true;
                        sms.MessageContent = messagepre;
                        sms.MessageDate = CurrentTime;
                        sms.ScholId = _user.SchoolId;
                        sms.StuentId = pay.StudentId;
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


                }
                catch (Exception ex)
                {
                    var x = ex.InnerException;
                }
            }
            else
            {
                var studentPaidAmount = _Entities.TbStudentPaidAmounts.Where(z => z.StudentId == pay.StudentId && z.BillNo == pay.BillNo).FirstOrDefault();
                if (studentPaidAmount != null)
                {
                    studentPaidAmount.IsActive = false;
                    studentPaidAmount.AddAccountStatus = false;
                    var studentBalance = _Entities.TbStudentBalances.Where(z => z.StudentId == pay.StudentId).FirstOrDefault();
                    if (studentBalance != null)
                    {
                        studentBalance.Amount = studentPaidAmount.PreviousBalance ?? 0;
                    }
                    status = _Entities.SaveChanges() > 0 ? true : false;

                }

            }
            double amnt = Convert.ToDouble(amt);
            if (amnt != pay.Amount)
            {
                model.PaymentStatus = "Fraud";
            }

            model.Amount = Convert.ToDouble(amt);
            model.CourseName = pay.CourseName;
            model.StudentId = pay.StudentId;
            model.BillNo = pay.BillNo;
            return View(model);
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
        #endregion

        //public object SearchAdmission(string id)
        //{
        //    bool Status = false;
        //    string Message = "Failed";
        //    try
        //    {
        //        string[] splitData = id.Split('~');
        //        string admission = splitData[0];
        //        long schoolId = Convert.ToInt64(splitData[1]);
        //        var IsAdmission = _Entities.TbStudents.Where(z => z.StudentSpecialId == admission && z.SchoolId == schoolId).FirstOrDefault();
        //        if (IsAdmission != null)
        //        {
        //            Status = true;
        //            return Json(new { Status = Status, Message = Message }, );
        //        }
        //        else
        //        {
        //            Status = false;
        //            return Json(new { Status = Status, Message = Message }, );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Status = Status, Message = Message }, );
        //    }
        //}
        private object UpdateSession(long schoolId)
        {
            var user = _schoolRepository.getUserById(schoolId);
            if (user != null)
            {
                HttpContext.Session.SetString("School", user.ToString());
            }
            return true;
        }

        public ActionResult DummyHome()
        {
            return View();
        }
        public object IsEmailExist(string Email)
        {
            bool status = _Entities.TbLogins.Any(z => z.Username.ToUpper() == Email.ToUpper() && z.IsActive == true);
            return Json(new { status = status });
        }
        public object CheckEmail(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbLogins.Any(x => x.Username.ToLower() == text.ToLower() && x.IsActive))
            {
                Status = true;
                Message = "Username already in use";
            }
            return Json(new { Status = Status, Message = Message });
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult ForgotPasswordParent()
        {
            return View();
        }
        public object CheckExistEmail(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbLogins.Any(x => x.Username.ToLower().Trim() == text.ToLower().Trim() && x.RoleId == 1 && x.IsActive))
            {

            }
            else
            {
                Status = true;
                Message = "Email not exists";
            }
            return Json(new { Status = Status, Message = Message });
        }
        public object CheckParentExistEmail(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (_Entities.TbParents.Any(x => x.Email.ToLower().Trim() == text.ToLower().Trim() && x.IsActive))
            {

            }
            else
            {
                Status = true;
                Message = "Email not exists";
            }
            return Json(new { Status = Status, Message = Message });
        }
        public object SendMailForPasswordParent(ForgotPasswordModel model)
        {
            bool sendData = false;
            bool status = false;
            string Message = "Failed";
            try
            {
                var parent = _Entities.TbParents.Where(x => x.Email.Trim().ToLower() == model.email.Trim().ToLower() && x.IsActive).FirstOrDefault();
                if (parent != null)
                {
                    var resetPasswordData = new TbResetPassword();
                    resetPasswordData.LinkExpireStatus = true;
                    resetPasswordData.UserId = parent.ParentId;
                    resetPasswordData.UserGuid = parent.ParentGuid;
                    resetPasswordData.IsActive = true;
                    resetPasswordData.TimeStamp = CurrentTime;
                    _Entities.TbResetPasswords.Add(resetPasswordData);
                    status = _Entities.SaveChanges() > 0;
                    Message = "Success ,Please Check Your Email";
                    sendData = SendMailDataParent(parent.Email, parent.ParentGuid);
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { Status = status, Message = Message });
        }
        public object SendMailForPassword(ForgotPasswordModel model)
        {
            bool sendData = false;
            bool status = false;
            string Message = "Failed";
            try
            {
                var schoolPassword = _Entities.TbLogins.Where(x => x.Username.Trim().ToLower() == model.email.Trim().ToLower() && x.IsActive).FirstOrDefault();
                if (schoolPassword != null)
                {
                    var resetPasswordData = new TbResetPassword();
                    resetPasswordData.LinkExpireStatus = true;
                    resetPasswordData.UserId = schoolPassword.SchoolId;
                    resetPasswordData.UserGuid = schoolPassword.LoginGuid;
                    resetPasswordData.IsActive = true;
                    resetPasswordData.TimeStamp = CurrentTime;
                    _Entities.TbResetPasswords.Add(resetPasswordData);
                    status = _Entities.SaveChanges() > 0;
                    Message = "Success ,Please Check Your Email";
                    sendData = SendMailData(schoolPassword.Username, schoolPassword.LoginGuid);
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { Status = status, Message = Message });
        }
        private bool SendMailData(string email, Guid loginGuid)
        {
            string sendGuid = Convert.ToString(loginGuid);
            var filePath = Path.Combine(_env.WebRootPath, "Content", "template", "WebResetPassword.html");
            var emailTemplate = System.IO.File.ReadAllText(filePath);
            var request = _httpContextAccessor.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}/Account/CheckResetSchoolPassword/{sendGuid}";
            var mBody = emailTemplate.Replace("{{resetLink}}", url);
            bool sendMail = Send("Reset Password", mBody, email);
            return sendMail;
        }
        private bool SendMailDataParent(string email, Guid loginGuid)
        {
            string sendGuid = Convert.ToString(loginGuid);
            var filePath = Path.Combine(_env.WebRootPath, "Content", "template", "WebResetPassword.html");
            var emailTemplate = System.IO.File.ReadAllText(filePath);
            var request = _httpContextAccessor.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}/Account/CheckResetParentPassword/{sendGuid}";
            var mBody = emailTemplate.Replace("{{resetLink}}", url);
            bool sendMail = Send("Reset Password", mBody, email);
            return sendMail;
        }
        private bool Send(string subject, string mailbody, string email)
        {
            MailMessage msg = new MailMessage();
            SmtpClient client = new System.Net.Mail.SmtpClient();
            msg.Subject = subject;
            msg.Body = mailbody;
            msg.From = new MailAddress("info.schoolman@gmail.com");
            msg.To.Add(new MailAddress(email));
            msg.Bcc.Add(new MailAddress("archanakv.srishti@gmail.com"));
            msg.IsBodyHtml = true;
            client.Host = "k2smtp.gmail.com";
            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("info.schoolman@gmail.com", "Info@123");
            client.Port = int.Parse("587");//25//465
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicauthenticationinfo;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                    System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                    System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
            return true;
        }
        public object CheckResetSchoolPassword(string id)//Check the link is expired
        {
            var userGuid = new Guid(id);
            var resetPasswordData = _Entities.TbResetPasswords.Where(x => x.UserGuid == userGuid && x.LinkExpireStatus == true && x.IsActive).FirstOrDefault();
            if (resetPasswordData != null)
                return RedirectToAction("ResetSchoolPassword", new { id = id });//Can change password
            else
                return RedirectToAction("ExpiredResetPassword");// expired link 
        }
        public object CheckResetParentPassword(string id)//Check the link is expired
        {
            var userGuid = new Guid(id);
            var resetPasswordData = _Entities.TbResetPasswords.Where(x => x.UserGuid == userGuid && x.LinkExpireStatus == true && x.IsActive).FirstOrDefault();
            if (resetPasswordData != null)
                return RedirectToAction("ResetParentPassword", new { id = id });//Can change password
            else
                return RedirectToAction("ExpiredResetPassword");// expired link 
        }
        public ActionResult ResetSchoolPassword(string id)
        {
            Guid schoolGuid = new Guid(id);
            var model = new ChangePasswordModel();
            model.LoginGuid = schoolGuid;
            return View(model);
        }
        public ActionResult ResetParentPassword(string id)
        {
            Guid schoolGuid = new Guid(id);
            var model = new ChangePasswordModel();
            model.LoginGuid = schoolGuid;
            return View(model);
        }
        public ActionResult ExpiredResetPassword()
        {
            return View();
        }
        public object ChangePasswordWithNew(ChangePasswordModel model)
        {
            bool status = false;
            string message = "failed";
            try
            {
                var schoolData = _Entities.TbLogins.Where(x => x.LoginGuid == model.LoginGuid && x.IsActive).FirstOrDefault();
                if (schoolData != null)
                {
                    var resetData = _Entities.TbResetPasswords.Where(x => x.UserGuid == model.LoginGuid && x.IsActive).FirstOrDefault();
                    if (resetData != null)
                    {
                        resetData.LinkExpireStatus = false;
                        resetData.IsActive = false;
                        _Entities.SaveChanges();
                    }
                    schoolData.Password = model.password;
                    status = _Entities.SaveChanges() > 0;
                    {
                        status = true;
                        message = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { Status = status, Message = message });
        }
        public object ChangePasswordWithNewParent(ChangePasswordModel model)
        {
            bool status = false;
            string message = "failed";
            try
            {
                var schoolData = _Entities.TbParents.Where(x => x.ParentGuid == model.LoginGuid && x.IsActive).FirstOrDefault();
                if (schoolData != null)
                {
                    var resetData = _Entities.TbResetPasswords.Where(x => x.UserGuid == model.LoginGuid && x.IsActive).FirstOrDefault();
                    if (resetData != null)
                    {
                        resetData.LinkExpireStatus = false;
                        resetData.IsActive = false;
                        _Entities.SaveChanges();
                    }
                    schoolData.Password = model.password;
                    status = _Entities.SaveChanges() > 0;
                    {
                        status = true;
                        message = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { Status = status, Message = message });
        }

        



    }
}
