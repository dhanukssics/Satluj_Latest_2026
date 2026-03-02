using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using Satluj_Latest.Controllers;

using System.Data;

using System.Globalization;
using Satluj_Latest.Data;


namespace TrackTap.Controllers
{
    public class SuperAdminController : AdminBaseController
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly DropdownData _dropdownData;
        public SuperAdminController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities, IWebHostEnvironment env, IConfiguration config) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {

            _env = env;
            _config = config;
        }

        // GET: SuperAdmin
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SchoolList()
        {
            return View();
        }
        public IActionResult PaymentList()
        {
            ViewBag.getschool = _dropdownData.GetSchoolList();
            return View();
        }
        public PartialViewResult GetPaymentGatewayListBySchool(string id)
        {
            string[] data = id.Split('~');
            long schoolId = Convert.ToInt64(data[0]);
            int year = Convert.ToInt16(data[2]);
            int opr = 0;
            int monthInDigit = 0;
            string month = data[1];
            if (month != "0")
            {
                monthInDigit = DateTime.ParseExact(data[1], "MMMM", CultureInfo.InvariantCulture).Month;
            }
            SchoolModel model = new SchoolModel();
            model.schoolId = schoolId;
            model.month = monthInDigit;
            model.year = year;

            if ((schoolId == 0) && (monthInDigit == 0))
                opr = 1;
            else if ((schoolId != 0) && (monthInDigit == 0))
                opr = 2;
            else if ((schoolId == 0) && (monthInDigit != 0))
                opr = 3;
            else if ((schoolId != 0) && (monthInDigit != 0))
                opr = 4;

            model.opr = opr;
            return PartialView("~/Views/SuperAdmin/_pv_PaymentList_Grid.cshtml", model);
        }
        public async Task<IActionResult> StatusPaymentGateway(string id)
        {
            string msg = "Failed";
            bool status = false;

            string[] data = id.Split('~');

            long schoolId = Convert.ToInt64(data[0]);
            int paymentStatus = Convert.ToInt16(data[1]);

            var school = await _Entities.TbSchools.Where(z => z.IsActive && z.SchoolId == schoolId).FirstOrDefaultAsync();
            if (school != null)
            {
                if (paymentStatus == 1)
                    school.PaymentOption = false;
                else
                    school.PaymentOption = true;

                status = _Entities.SaveChanges() > 0;
                msg = status ? " Activated" : "No changes made";
            }

            return Json(new { msg = msg, status = status });
        }
        public PartialViewResult RefreshSchoolGrid()
        {
            return PartialView("~/Views/SuperAdmin/_pv_School_list.cshtml");
        }

        public IActionResult StudentExcelUpload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadStudentExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return Json(new { status = false, msg = "No file selected" });

            string ext = Path.GetExtension(excelFile.FileName).ToLower();
            if (ext != ".xls" && ext != ".xlsx")
                return Json(new { status = false, msg = "Invalid file type" });

            string uploadFolder = Path.Combine(_env.WebRootPath, "Media/Excel");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string filePath = Path.Combine(uploadFolder, excelFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await excelFile.CopyToAsync(stream);
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                DataTable dt = new DataTable();

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var ws = package.Workbook.Worksheets[0];
                    if (ws == null)
                        return Json(new { status = false, msg = "Excel worksheet is empty" });

                    bool hasHeader = true;

                   
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                        dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");

                   
                    int startRow = hasHeader ? 2 : 1;

                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var row = dt.NewRow();
                        for (int col = 1; col <= ws.Dimension.End.Column; col++)
                        {
                            row[col - 1] = ws.Cells[rowNum, col].Text;
                        }
                        dt.Rows.Add(row);
                    }
                }

                
                string conString = _config.GetConnectionString("SatlujCon");

                using (SqlConnection con = new SqlConnection(conString))
                using (SqlBulkCopy bulk = new SqlBulkCopy(con))
                {
                    bulk.DestinationTableName = "tb_Student";

                    
                    bulk.ColumnMappings.Add("SchoolId", "SchoolId");
                    bulk.ColumnMappings.Add("StudentSpecialId", "StudentSpecialId");
                    bulk.ColumnMappings.Add("StundentName", "StundentName");
                    bulk.ColumnMappings.Add("ParentName", "ParentName");
                    bulk.ColumnMappings.Add("Address", "Address");
                    bulk.ColumnMappings.Add("City", "City");
                    bulk.ColumnMappings.Add("ContactNumber", "ContactNumber");
                    bulk.ColumnMappings.Add("ClasssNumber", "ClasssNumber");
                    bulk.ColumnMappings.Add("ClassId", "ClassId");
                    bulk.ColumnMappings.Add("DivisionId", "DivisionId");
                    bulk.ColumnMappings.Add("BusId", "BusId");
                    bulk.ColumnMappings.Add("TripNo", "TripNo");
                    bulk.ColumnMappings.Add("FilePath", "FilePath");
                    bulk.ColumnMappings.Add("TimeStamp", "TimeStamp");
                    bulk.ColumnMappings.Add("StudentGuid", "StudentGuid");
                    bulk.ColumnMappings.Add("IsActive", "IsActive");
                    bulk.ColumnMappings.Add("ParentId", "ParentId");
                    bulk.ColumnMappings.Add("State", "State");
                    bulk.ColumnMappings.Add("Gender", "Gender");
                    bulk.ColumnMappings.Add("BloodGroup", "BloodGroup");
                    bulk.ColumnMappings.Add("ParentEmail", "ParentEmail");
                    bulk.ColumnMappings.Add("PostalCode", "PostalCode");
                    bulk.ColumnMappings.Add("DOB", "DOB");
                    bulk.ColumnMappings.Add("Aadhaar", "Aadhaar");
                    bulk.ColumnMappings.Add("BioNumber", "BioNumber");

                    await con.OpenAsync();
                    await bulk.WriteToServerAsync(dt);
                }

                return Json(new { status = true, msg = "Upload successful" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message });
            }
        }




        public IActionResult SmsAllHistory()
        {
            FeeModel model = new FeeModel();
            model.StartDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult SmsAllHistoryByDate(string id)
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
            return PartialView("~/Views/SuperAdmin/_SmsAllHistory_Grid.cshtml", model);
        }

        public IActionResult PackageList(string id)
        {

            SmsPackageModels model = new SmsPackageModels();
            model.SchoolId = Convert.ToInt64(id);
            model.SmsStatus = _Entities.TbSchools.Where(z => z.SchoolId == model.SchoolId).FirstOrDefault().SmsActive;
            model.FromDate = CurrentTime.ToString(); ;
            return View(model);
        }

        public PartialViewResult AddPackageView(string id)
        {
            SmsPackageModels model = new SmsPackageModels();
            model.SchoolId = Convert.ToInt64(id);
            return PartialView("~/Views/SuperAdmin/_pv_AddPackage.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddSmsPackage(SmsPackageModels model)
        {
            var message = "failed";
            var status = false;
            var smsPackage = new TbSmsPackage();
            smsPackage.SchoolId = model.SchoolId;

            if (model.ToDate != null)
            {
                string[] splitData1 = model.ToDate.Split('-');
                var dd1 = splitData1[0];
                var mm1 = splitData1[1];
                var yyyy1 = splitData1[2];
                var dat1 = mm1 + '-' + dd1 + '-' + yyyy1;
                smsPackage.ToDate = Convert.ToDateTime(dat1);
            }
            if (model.FromDate != null)
            {
                string[] splitData12 = model.FromDate.Split('-');
                var dd12 = splitData12[0];
                var mm12 = splitData12[1];
                var yyyy12 = splitData12[2];
                var dat12 = mm12 + '-' + dd12 + '-' + yyyy12;
                smsPackage.FromDate = Convert.ToDateTime(dat12);
            }
            smsPackage.SmsRate = model.SmsRate;
            smsPackage.AllowedSms = model.AllowedSms;
            smsPackage.IsActive = true;
            smsPackage.IsDisabled = true;
            smsPackage.TimeStamp = CurrentTime;
            await _Entities.TbSmsPackages.AddAsync(smsPackage);
            if (_Entities.SaveChanges() > 0)
            {
                message = "success";
                status = true;
            }
            return Json(new { Status = status, Message = message, });
        }
        public PartialViewResult RefreshSmsPackageGrid(string id)
        {
            SmsPackageModels model = new SmsPackageModels();
            model.SchoolId = Convert.ToInt64(id);
            return PartialView("~/Views/SuperAdmin/_pv_Package_list.cshtml", model);
        }

        public async Task<IActionResult> SmsActivate(string id)
        {
            string msg = "Failed";
            bool status = false;

            string[] data = id.Split('~');

            long schoolId = Convert.ToInt64(data[0]);
            int smsStatus = Convert.ToInt16(data[1]);

            var school = await _Entities.TbSchools.Where(z => z.IsActive && z.SchoolId == schoolId).FirstOrDefaultAsync();
            if (school != null)
            {
                if (smsStatus == 1)
                {
                    school.SmsActive = true;
                    msg = " Activated";
                }
                else
                {
                    school.SmsActive = false;
                    msg = " Deactivated";

                }

                status = await _Entities.SaveChangesAsync() > 0;
                msg = status ? msg : "No changes made";
            }

            return Json(new { msg = msg, status = status });
        }

        public async Task<IActionResult> SmsPackageActivate(string id)
        {
            string msg = "Failed";
            bool status = false;

            string[] data = id.Split('~');

            long schoolId = Convert.ToInt64(data[0]);
            int packageId = Convert.ToInt16(data[1]);
            int smsStatus = Convert.ToInt16(data[2]);

            var package =await _Entities.TbSmsPackages.Where(z => z.IsActive && z.SchoolId == schoolId).ToListAsync();
            if (package.Count > 0)
            {
                if (smsStatus == 0)
                {
                    foreach (var item in package)
                    {
                        if (item.PackageId == packageId)
                        {
                            item.IsDisabled = false;
                            await _Entities.SaveChangesAsync();
                        }
                        else
                        {
                            item.IsDisabled = true;
                            await _Entities.SaveChangesAsync();
                        }
                    }
                    msg = " Activated";
                    status = true;
                }
                else
                {
                    foreach (var item in package)
                    {
                        item.IsDisabled = true;
                        await _Entities.SaveChangesAsync();
                    }
                    msg = " Deactivated";
                    status = true;
                }

                msg = status ? msg : "No changes made";
            }

            return Json(new { msg = msg, status = status });
        }


    }
}