
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest;
using Satluj_Latest.Data;
using Satluj_Latest.DataLibrary.Repository;
using Satluj_Latest.Helper;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Utility;
using System.Collections.Specialized;
using System.Globalization;

using System.Net;
using System.Text.Json;


namespace Satluj_Latest.Controllers
{
    public class ParentController : BaseController
    {
        private readonly IHttpContextAccessor _http;
        private readonly SchoolDbContext _Entities;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _env;
        private readonly DropdownData _dropdown;
        public ParentController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, IHttpContextAccessor httpAccessor, HttpClient httpClient, IWebHostEnvironment env, DropdownData dropdown) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _Entities = Entities;
            _http = httpAccessor;
            _httpClient = httpClient;
            _env = env;
            _dropdown = dropdown;
        }

        //
        // GET: /Parent/

        public IActionResult Home()
        {
            ParentRegisterModel model = new ParentRegisterModel();
            model.parentId = _parentUser.ParentId;
            return View(model);
        }

        public PartialViewResult AddChildView()
        {
            var model = new SchoolModel();

            //var dropdown = new DropdownData();
            ViewBag.SchoolList = _dropdown
                .GetSchoolList()
                .OrderBy(x => x.Text)
                .ToList();
            return PartialView("~/Views/Parent/_pv_AddChildModel.cshtml", model);
        }

        public PartialViewResult SearchAdmission(string id)
        {
            StudentModel model = new StudentModel();
            string[] splitData = id.Split('~');
            model.admissionNo = splitData[0];
            model.schoolId = Convert.ToInt64(splitData[1]);
            return PartialView("~/Views/Parent/_pv_StudentDetailsParent.cshtml", model);
        }

        public IActionResult Attendance(string id)
        {
            AttendanceModels model = new AttendanceModels();
            model.studentId = Convert.ToInt64(id);
            model.StudentName = new Satluj_Latest.Data.Student(model.studentId).StundentName;
            return View(model);
        }

        public PartialViewResult AttendancePartial(string id)
        {
            string[] splitData = id.Split('~');
            int monthInDigit = DateTime.ParseExact(splitData[1], "MMMM", CultureInfo.InvariantCulture).Month;
            AttendanceModels model = new AttendanceModels();
            model.studentId = Convert.ToInt64(splitData[0]);
            model.month = Convert.ToInt16(monthInDigit);
            model.year = Convert.ToInt16(splitData[2]);
            return PartialView("~/Views/Parent/_pv_Attendance_Grid.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> OtpSubmit(string id)
        {
            bool status = false;
            string message = "Wrong OTP";

            var split = id.Split('~');
            long studentId = Convert.ToInt64(split[0]);
            string otp = split[1];

            var otpDetail = await _Entities.TbOtpmessages
                .Where(z => z.StudentId == studentId && z.ExpTimeStamp >= CurrentTime && z.Otp == otp)
                .OrderByDescending(z => z.OtpId)
                .FirstOrDefaultAsync();

            if (otpDetail != null)
            {
                var student = await _Entities.TbStudents.FirstOrDefaultAsync(z => z.StudentId == studentId);
                if (student != null)
                {
                    student.ParentId = _parentUser.ParentId;
                    status = await _Entities.SaveChangesAsync() > 0;
                    message = status ? "Student Added" : "Failed";

                    if (status)
                    {
                        otpDetail.IsActive = false;
                        await _Entities.SaveChangesAsync();
                    }
                }
            }

            return Json(new { status, msg = message });
        }

        [HttpPost]
        public async Task<IActionResult> AddChildToParent(string id)
        {
            long studentId = Convert.ToInt64(id);

            var student = await _Entities.TbStudents.FirstOrDefaultAsync(z => z.StudentId == studentId);

            if (student == null)
                return Json(new { status = false, msg = "Student not found" });

            student.ParentId = _parentUser.ParentId;

            bool status = await _Entities.SaveChangesAsync() > 0;

            return Json(new { status, msg = status ? "Student Added" : "Failed" });
        }

        public IActionResult BillingDetails(string id)
        {
            var split = id.Split('~');
            long studentId = Convert.ToInt64(split[0]);
            string admissionNo = split[1];

            var student = new Satluj_Latest.Data.Student(studentId);

            var model = new FeeModel();

            if (student.StudentSpecialId == admissionNo)
            {
                model.SchoolModel = new SchoolModel
                {
                    studentName = student.StundentName,
                    classNumber = student.ClasssNumber,
                    className = student.ClassName,
                    division = student.DivisionName,
                    classInCharge = student.Teacher?.TeacherName ?? "Not Assigned",
                    classId = student.ClassId,
                    studentId = studentId
                };
            }

            return View(model);
        }
        public async Task<PartialViewResult> OtpModelView(string id)
        {
            var model = new StudentModel
            {
                studentId = Convert.ToInt64(id)
            };

            string otp = new Random().Next(0, 999999).ToString("D6");
            DateTime expiryTime = CurrentTime.AddMinutes(5);

            var student = _Entities.TbStudents.FirstOrDefault(x => x.StudentId == model.studentId);

            if (student != null)
            {
                string senderName = _Entities.TbSchoolSenderIds
                                .Where(x => x.SchoolId == student.SchoolId && x.IsActive == true)
                                .Select(x => x.SenderId)
                                .FirstOrDefault() ?? "MYSCHO";

                string message = $"OTP for SchoolMan - {otp}";

                string url = $"http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers={student.ContactNumber}&route=2&message={message}&sender={senderName}";

                try
                {
                    var response = await _httpClient.GetStringAsync(url);

                    
                    var respList = JsonSerializer.Deserialize<alvosmsResp>(response, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                   
                }
            }

            
            var otpEntity = new TbOtpmessage
            {
                StudentId = model.studentId,
                Otp = otp,
                ExpTimeStamp = expiryTime,
                Otptype = 1,
                IsActive = true,
                TimeStamp = CurrentTime
            };

            _Entities.TbOtpmessages.Add(otpEntity);
            await _Entities.SaveChangesAsync();

            return PartialView("~/Views/Parent/_pv_AddChild_OTP.cshtml", model);
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

        public PartialViewResult StudentHistoryBillPartialView(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            model.SchoolModel.studentId = Convert.ToInt64(id);
            return PartialView("~/Views/Parent/_pv_History_Billing_StudentFee_Model.cshtml", model);
        }

        public PartialViewResult LoadTableForBilling(string id)
        {
            FeeModel model = new FeeModel();
            model.SchoolModel = new SchoolModel();
            string[] splitData = id.Split('~');
            model.SchoolModel.studentId = Convert.ToInt64(splitData[1]);
            model.BillNumber = Convert.ToInt64(splitData[0]);
            return PartialView("~/Views/Parent/_pv_History_PopupGrid.cshtml", model);
        }

        public IActionResult ChildAcademyPayment()
        {
            return View();
        }

        public IActionResult CoursesOffered()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentMainBillPay(FeeModel model)
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
                    studentPaidsAmount.AddAccountStatus = false;
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
                //Session["REFERENCE"] = reference;
                //var payment = Entities.tb_Payment.Where(x => x.UserId == _user.UserId && x.PaymentType == 1).FirstOrDefault();
                PaymentModels pay = new PaymentModels();
                pay.ReferenceNo = reference;
                var student = _Entities.TbParents.Where(z => z.ParentId == _parentUser.ParentId).FirstOrDefault();
                pay.Description = "Payment";
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                pay.ReturnUrl = $"{baseUrl}//Parent/ccavResponseHandler";
                //pay.ReturnUrl = "http://localhost:16138/Parent/ccavResponseHandler";
                pay.Name = student.ParentName;
                pay.Address = student.Address;
                pay.City = student.City;
                pay.State = student.State;
                pay.PostalCode = student.PostalCode;
                pay.PhoneNo = student.ContactNumber;
                pay.Email = student.Email;
                pay.Amount = Convert.ToDouble(amountTopay);
                pay.CourseName = "Bill";
                pay.BillNo = BillNo;
                pay.SchoolId = SchoolId;
                pay.StudentId = StudentId;
                HttpContext.Session.SetString("PaymentPostData", JsonSerializer.Serialize(pay));
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
            return PartialView("~/Views/Parent/_pv_PrintAccountBillData.cshtml", model);
        }

        #region PaymentGateway
        public IActionResult CoursePayment(string id)
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
            pay.ReferenceNo = reference;
            var student = _Entities.TbParents.Where(z => z.ParentId == _parentUser.ParentId).FirstOrDefault();
            pay.Description = "Payment";
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            pay.ReturnUrl = baseUrl + "/Parent/ccavResponseHandler";
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
            HttpContext.Session.SetString("PaymentPostData", JsonSerializer.Serialize(pay));
            //return View("CCAVRequestHandler",pay);
            return Json(new { status = status });
        }
        public IActionResult PaymentPost()
        {
            var reference = HttpContext.Session.GetString("REFERENCE");

            var json = HttpContext.Session.GetString("PaymentPostData");
            var pay = JsonSerializer.Deserialize<PaymentModels>(json);

            var model = new PaymentModels
            {
                ReferenceNo = reference,
                ReturnUrl = pay.ReturnUrl,
                Name = pay.Name,
                Address = pay.Address,
                City = pay.City,
                State = pay.State,
                PostalCode = pay.PostalCode,
                PhoneNo = pay.PhoneNo,
                Email = pay.Email,
                Amount = pay.Amount,
                Description = "Payment"
            };

            return View(model);
        }
        public IActionResult CCAVRequestHandler()
        {

            var json = HttpContext.Session.GetString("PaymentPostData");
            var pay = JsonSerializer.Deserialize<PaymentModels>(json);

            CCACrypto crypto = new CCACrypto();
            string workingKey = "3891AA5249F6E3DBA928422EB4BA18DD";
            string accessCode = "AVTM75FA75BW58MTWB";

            var form = Request.HasFormContentType ? Request.Form : null;

            var sorted = form?.OrderBy(x => x.Key)
                .Where(x => !x.Key.StartsWith("_"))
                .Select(x => $"{x.Key}={x.Value}")
                .ToList();

            string requestData = string.Join("&", sorted);

            string encryptedRequest = crypto.Encrypt(requestData, workingKey);

            pay.iframeSrc = $"https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest={encryptedRequest}&access_code={accessCode}";

            return View(pay);
        }
        public async Task<IActionResult> ccavResponseHandler()
        {
            bool status = false;
            PaymentModels model = new PaymentModels();
            //PaymentModels pay = new PaymentModels();
            var json = HttpContext.Session.GetString("PaymentPostData");
            var pay = JsonSerializer.Deserialize<PaymentModels>(json);
            //pay = (PaymentModels)Session["PaymentPostData"];

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
            ccavenueTable.ParentId = _parentUser.ParentId;
            ccavenueTable.Course = pay.CourseName;
            ccavenueTable.BillNo = pay.BillNo;
            ccavenueTable.SchoolId = pay.SchoolId;

            // ccavenueTable.Amount = Convert.ToDecimal(pay.Amount);
            _Entities.TbCcavenueCourseResponses.Add(ccavenueTable);
            _Entities.SaveChanges();
            if (isSuccess)
            {

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
                    smsHead.Head = "BillDate Payment " + paymentList.FirstOrDefault().Student.StundentName;
                    smsHead.SchoolId = _user.SchoolId;
                    smsHead.TimeStamp = CurrentTime;
                    smsHead.IsActive = true;
                    smsHead.SenderType = (int)SMSSendType.Student;
                    _Entities.TbSmsHeads.Add(smsHead);
                    status = _Entities.SaveChanges() > 0;


                    messagepre = "Dear Parent of " + paymentList.FirstOrDefault().Student.StundentName + ", you have paid Rs." + string.Format("{0:0.00}", amt) + " on " + CurrentTime;

                    var phone = paymentList.FirstOrDefault().Student.ContactNumber.ToString();
                    int length = messagepre.Length;
                    int que = length / 160;
                    int rem = length % 160;
                    if (rem > 0)
                        que++;
                    int smsCount = que;
                    //var url = "http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers=" + phone + "&route=2&message=" + messagepre + "&sender=" + senderName;
                    ////  var url = "http://bhashsms.com//api/sendmsg.php?user=srishtitrans&pass=123456&sender=MCHILD&phone=" + phone + "&text=" + item.Description + "&priority=ndnd&stype=normal";

                    //ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    //HttpWebRequest request = this.GetRequest(url);
                    //WebResponse webResponse = request.GetResponse();
                    //var responseText = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    //var newresponse = responseText.Remove(responseText.Length - 2).TrimEnd();
                    string url = $"http://alvosms.in/api/v1/send?token=ivku4o2r6gjdq98bm3aesl50pyz7h1&numbers={phone}&route=2&message={messagepre}&sender={senderName}";

                    string responseText = await _httpClient.GetStringAsync(url);

                    var respList = JsonSerializer.Deserialize<alvosmsResp>(responseText,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    //alvosmsResp respList = new JavaScriptSerializer().Deserialize<alvosmsResp>(responseText);
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


        #endregion

        #region Message
        public async Task<ActionResult> Messages()
        {
            ParentRegisterModel model = new ParentRegisterModel();
            model.parentId = _parentUser.ParentId;
            return View(model);
        }
        public PartialViewResult MessageSectionView(string id)
        {
            StudentModel model = new StudentModel();
            model.studentId = Convert.ToInt64(id);
            return PartialView("~/Views/Parent/_pv_Message_View.cshtml", model);
        }

        public PartialViewResult MessageSectionInnerView(string id)
        {
            Satluj_Latest.MapModel.ParentTeacherConversationMapModel model = new Satluj_Latest.MapModel.ParentTeacherConversationMapModel();
            string[] splitData = id.Split('~');
            model.length = Convert.ToInt32(splitData[0]);
            model.StudentId = splitData[1];
            model.RequirestType = 0;
            return PartialView("~/Views/Parent/_pv_Message_Inner_View.cshtml", model);
        }

        #endregion

        public async Task<ActionResult> Dummy()
        {
            return View();
        }

        public async Task<ActionResult> ParentHome()
        {
            ParentRegisterModel model = new ParentRegisterModel();
            model.parentId = _parentUser.ParentId;
            return View(model);
        }

        public async Task<ActionResult> ProgressCard(string id)
        {
            ProgressCardModel model = new ProgressCardModel();
            long StudnetId = Convert.ToInt64(id);
            var studentDetails = new Satluj_Latest.Data.Student(StudnetId);
            model.SchoolId = studentDetails.SchoolId;
            model.ClassId = studentDetails.ClassId;
            model.DivisionId = studentDetails.DivisionId;
            var region = _Entities.TbRegionsClasses.Where(x => x.ClassId == model.ClassId).FirstOrDefault();
            if (region != null)
            {
                model.RegionId = region.RegionId;
                model.RegionName = region.Region.RegionName;
            }
            else
            {
                model.RegionId = 0;
                model.RegionName = "";
            }
            model.ProgressCardName = _Entities.TbCertificateNames.Where(x => x.SchoolId == model.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault() == null ? " " : _Entities.TbCertificateNames.Where(x => x.SchoolId == model.SchoolId && x.IsActive).Select(x => x.CertificateName).FirstOrDefault();
            model.StudnetName = studentDetails.StundentName;
            model.ClassName = studentDetails.ClassName;
            model.DivisionName = studentDetails.DivisionName;
            model.StudentId = studentDetails.StudentId;
            return View(model);
        }

        public async Task<ActionResult> ParentProgressCardGeneration(string id)
        {

            string[] splitData = id.Split('~');
            var studentId = Convert.ToInt64(splitData[0]);
            var declaredExamId = Convert.ToInt64(splitData[1]);
            var DeclredExamDetails = _Entities.TbDeclaredExams.Where(x => x.Id == declaredExamId && x.IsActive).FirstOrDefault();
            var ExamTermDetails = DeclredExamDetails.Exam.Term;
            var StudentDetails = _Entities.TbStudents.Where(x => x.StudentId == studentId && x.IsActive).FirstOrDefault();
            var ClassDetails = DeclredExamDetails.Class;
            var RegionDetails = ClassDetails.TbRegionsClasses.FirstOrDefault();
            var ScholasticData = _Entities.TbScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).ToList();
            var CoScholasticData = _Entities.TbCoScholasticAreas.Where(x => x.RegionId == RegionDetails.RegionId && x.IsActive).ToList();

            if (ExamTermDetails.Id == 1)
            {
                // Term one report
                #region Term 1
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.SchoolLogo = StudentDetails.School.FilePath;
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
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).FirstOrDefault() == null ? " " : StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                        model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + " - " + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId && x.Id == model.ExamId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();
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
                        zz.ScolasticArea = Item.Exam.ExamName;
                        var sco = returnExamresult.FirstOrDefault()?.Subject?.TotalScore ?? 0;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault()?
                                    .TbDeclaredExamSubjects?
                                    .Where(x => x.IsActive)
                                    .FirstOrDefault()?
                                    .TotalScore ?? 0;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            SubjectDetails a = new SubjectDetails();
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
                catch (Exception ex)
                {
                    model.IsIssued = true;
                }
                return View(model);
            }
            else if (ExamTermDetails.Id == 2)
            {
                #region Term II
                StudentTerm2ProgressCardModel model = new StudentTerm2ProgressCardModel();
                model.SchoolLogo = StudentDetails.School.FilePath;
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
                    model.Today = DateTime.Now.ToShortDateString();
                    model.SchoolAdddress = StudentDetails.School.Address;
                    model.Remark = StudentDetails.TbStudentRemarks.Where(x => x.ExamId == model.ExamId).Select(x => x.Remark).FirstOrDefault();
                    if (StudentDetails.Dob != null)
                        model.DateOfBirth = Convert.ToDateTime(StudentDetails.Dob).ToShortDateString();
                    else
                        model.DateOfBirth = " ";
                    model.ClassDivision = StudentDetails.Class.Class + " - " + StudentDetails.Division.Division;

                    var twoDeclaredExams = _Entities.TbDeclaredExams.Where(x => x.ClassId == StudentDetails.ClassId && x.IsActive && x.SchoolId == StudentDetails.SchoolId).ToList().OrderBy(x => x.ExamId).ToList();

                    var allExamTerms = _Entities.TbExamBooks.Where(x => x.SchoolId == StudentDetails.SchoolId && x.IsActive).ToList();

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
                        zz.ScolasticArea = Item.Exam.ExamName;
                        var sco = returnExamresult.FirstOrDefault()?.Subject?.TotalScore ?? 0;
                        if (sco == 0)
                        {
                            sco = allSubjects.FirstOrDefault()?
                                .TbDeclaredExamSubjects?
                                .Where(x => x.IsActive)
                                .FirstOrDefault()?
                                .TotalScore ?? 0;
                        }
                        zz.TotalScore = Math.Round(sco).ToString();
                        foreach (var sub in allSubjects)
                        {
                            SubjectDetails a = new SubjectDetails();
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

        public async Task<ActionResult> Notifications()
        {
            ParentStudentClassModel model = new ParentStudentClassModel();
            model.ParentId = _parentUser.ParentId;
            return View(model);
        }

        public PartialViewResult GetParentCircularList(string id)
        {
            ParentStudentClassModel model = new ParentStudentClassModel();
            model.ParentId = Convert.ToInt64(id);
            return PartialView("~/Views/Parent/_pv_ParentCirculars.cshtml", model);
        }

        //18-sep-2020 Jibin Start..........................................

        #region 18-sep-2020 Jibin Start


        //jibin 29/08/2020


        public async Task<ActionResult> Uploaded_Assignments(int id)
        {
            AssementUpload model = new AssementUpload();
            //  model.ClassId = id;
            var stud = _Entities.TbStudents.Where(x => x.StudentId == id).FirstOrDefault();
            model.ClassId = Convert.ToInt32(stud.ClassId);
            model.StudentId = Convert.ToInt32(stud.StudentId);

            model.Division = Convert.ToInt32(stud.DivisionId);

            return View(model);

        }

        public PartialViewResult AssessmentPartial(string id)
        {
            string[] splitData = id.Split('~');
            int monthInDigit = DateTime.ParseExact(splitData[1], "MMMM", CultureInfo.InvariantCulture).Month;
            AssessmentModel model = new AssessmentModel();
            model.classId = Convert.ToInt32(splitData[0]);
            model.month = Convert.ToInt32(monthInDigit);
            model.year = Convert.ToInt32(splitData[2]);
            model.divisionId = Convert.ToInt32(splitData[3]);//newly added
            model.studentId = Convert.ToInt32(splitData[4]);//newly added on 9/20/2020



            return PartialView("~/Views/Parent/_pv_AssessmentGrid.cshtml", model);
        }



        public FileResult DownloadFiles(string Filename)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "/Media/Uploads/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + Filename);
            string fileName = Filename;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        //jibin 29/08/2020
        public async Task<ActionResult> Uploaded_Answer(int id)
        {
            ExamResultMainModel model = new ExamResultMainModel();

            // model.ClassId = id;

            model.StudentId = id;
            var stud = _Entities.TbStudents.Where(x => x.StudentId == id).FirstOrDefault();

            if (stud != null)
            {
                model.SchoolId = stud.SchoolId;
                model.ClassId = stud.ClassId;


            }

            return View(model);

        }




        public async Task <ActionResult> Answer_Uploaded(List<IFormFile> TrackFile, TbStudentAssesmentUpload user)
        {

            try
            {


                foreach (var file in TrackFile)
                {
                    if (file == null || file.Length == 0)
                        continue;

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string uploadFolder = Path.Combine(_env.WebRootPath, "Media", "Uploads");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var clas = _Entities.TbClasses.Where(x => x.ClassId == user.ClassId).FirstOrDefault();



                    var stud = _Entities.TbStudents.Where(x => x.StudentId == user.StudentId).FirstOrDefault();
                    var data = new TbStudentAssesmentUpload();

                    data.FileName = fileName;


                    data.Class = clas.Class;
                    data.ClassId = user.ClassId; ;


                    int divisionId = Convert.ToInt32(stud.DivisionId);
                    var div = _Entities.TbDivisions.Where(x => x.DivisionId == divisionId).FirstOrDefault();


                    data.Division = div.Division;
                    data.DivisionId = Convert.ToInt32(div.DivisionId);


                    data.StudentName = stud.StundentName;
                    data.StudentId = Convert.ToInt32(stud.StudentId);

                    data.Description = user.Description;
                    data.SubjectId = user.SubjectId;
                    data.UploadedDate = CurrentTime.Date; 
                    var teacherUpload = _Entities.TbAssesmentUploads.Where(a => a.ClassId == user.ClassId &&
                            a.SubjectId == user.SubjectId &&
                            a.Division == user.DivisionId)
                            .OrderByDescending(a => a.FileUploadedDate)  
                            .FirstOrDefault();

                    if (teacherUpload != null)
                    {
                        data.TeacherFileId = teacherUpload.FileId;  
                    }
                    _Entities.TbStudentAssesmentUploads.Add(data);
                    _Entities.SaveChanges();

                }

                ViewBag.Message = user.ClassId;
                ExamResultMainModel model = new ExamResultMainModel();
                model.ClassId = Convert.ToInt32(user.ClassId);
                var clasnew = _Entities.TbStudents.Where(x => x.StudentId == user.StudentId).FirstOrDefault();
                model.StudentId = Convert.ToInt32(user.StudentId);
                model.SchoolId = clasnew.SchoolId;

                TempData["alertMessage"] = "Successfully Saved..!";


                return View("~/Views/Parent/Uploaded_Answer.cshtml", model);
            }
            catch
            {

                ExamResultMainModel model = new ExamResultMainModel();
                model.ClassId = Convert.ToInt32(user.ClassId);
                var clasGet = _Entities.TbStudents.Where(x => x.StudentId == user.StudentId).FirstOrDefault();
                model.StudentId = Convert.ToInt32(clasGet.StudentId);

                model.SchoolId = clasGet.SchoolId;
                return View("~/Views/Parent/Uploaded_Answer.cshtml", model);
            }
        }


        #endregion
        //18-sep-2020 Jibin end..........................................


    }
}
