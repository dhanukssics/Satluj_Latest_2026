
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using Satluj_Latest.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class School : BaseReference
    {
        private TbSchool school;
        public School(TbSchool obj) 
        { school = obj; }
        public School(long id)
        {
            school = _Entities.TbSchools.FirstOrDefault(z => z.SchoolId == id);

            // Load classes
            _Entities.Entry(school).Collection(s => s.TbClasses).Load();

            // Load AcademicYear + Divisions
            foreach (var cls in school.TbClasses)
            {
                _Entities.Entry(cls).Reference(c => c.AcademicYear).Load();   // FIX
                _Entities.Entry(cls).Collection(c => c.TbDivisions).Load();
            }

            _Entities.Entry(school).Collection(s => s.TbStudents).Load();
            _Entities.Entry(school).Collection(s => s.TbCalenderEvents).Load();
            _Entities.Entry(school).Collection(s => s.TbBookCategories).Load();

            foreach (var cat in school.TbBookCategories)
            {
                _Entities.Entry(cat).Collection(c => c.TbLibraryBooks).Load();
            }
        }



        public long SchoolId { get { return school.SchoolId; } }
        public string SchoolName { get { return school.SchoolName; } }
        public string Address { get { return school.Address; } }
        public string City { get { return school.City; } }
        public string Website { get { return school.Website; } }
        public string Contact { get { return school.Contact; } }
        public System.DateTime TimeStamp { get { return school.TimeStamp; } }
        public System.Guid SchoolGuidId { get { return school.SchoolGuidId; } }
        public bool IsActive { get { return school.IsActive; } }
        public string FilePath { get { return school?.FilePath; } }
        public string State { get { return school?.State; } }
        public string Latitude { get { return school?.Latitude; } }
        public string Longitude { get { return school?.Longitude; } }
        public string FooterMessage { get { return school.BillingFooterMessage; } }
        public bool? PaymentOption { get { return school.PaymentOption == null ? false : school.PaymentOption; } }
        public List<Teacher> Teachers { get { return school.TbTeachers.Where(z => z.IsActive).ToList().Select(z => new Teacher(z)).ToList(); } }
        public List<Class> Class { get { return school.TbClasses.Where(z => z.IsActive).ToList().Select(z => new Class(z)).ToList(); } }

        public List<Division> Divisions { get { return school.TbClasses.SelectMany(z => z.TbDivisions).Where(z => z.IsActive && z.Class.IsActive).ToList().Select(z => new Division(z)).ToList(); } }

        public Login Login { get { return school.TbLogins.Where(z => z.SchoolId == school.SchoolId && z.IsActive).ToList().Select(z => new Login(z)).FirstOrDefault(); } }


        public List<Student> GetStudentDetails()
        {
            return school.TbStudents.Where(z => z.IsActive).ToList().Select(q => new Student(q)).ToList();
        }
        public List<TbStudent> GetPublishedClassStudentDetails()
        {
            return school.TbStudents.Where(z => z.IsActive && z.Class.PublishStatus == true).ToList().Select(q => new TbStudent(q)).ToList();
        }
        public List<TbStudent> GetStudentDetailsByFeeDiscount(long feeId)
        {
            var list1 = school.TbStudents.Where(z => z.IsActive).ToList().Select(q => new TbStudent(q)).ToList();
            var list2 = school.TbStudents.SelectMany(z => z.TbFeeDiscounts).Where(z => z.IsActive && z.FeeId == feeId).ToList().Select(q => new TbStudent(q.Student)).ToList();
            var list3 = list1.Except(list2).ToList();
            return list3;
        }
        public List<TbFeeDiscount> GetStudentDiscountList()
        {
            return school.TbStudents.SelectMany(z => z.TbFeeDiscounts).Where(z => z.IsActive).ToList().Select(q => new TbFeeDiscount(q)).ToList();
        }
        public List<FeeStudent> GetSpecialFeeStudentList(long feeId)
        {
            return school.TbStudents.SelectMany(z => z.TbFeeStudents).Where(z => z.IsActive && z.FeeId == feeId).ToList().Select(q => new FeeStudent(q)).ToList();
        }
        public List<Fee> GetAllFees()
        {
            return school.TbFees.Where(z => z.IsActive).ToList().Select(z => new Fee(z)).ToList();
        }

        public async Task<List<SPGetDailyReports>> GetReportDailyByDate(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            var result = await _Entities.DailyReports
                .FromSqlRaw("EXEC SP_GetDaily_Report @SchoolId, @StartDate, @EndDate",
                    new SqlParameter("@SchoolId", school.SchoolId),
                    new SqlParameter("@StartDate", start),
                    new SqlParameter("@EndDate", end))
                .OrderBy(x => x.Date)
                .ToListAsync();

            return result;
        }

        public List<TbStudent> GetStudentForParent(string admissionNo)
        {
            return school.TbStudents.Where(z => z.StudentSpecialId.ToUpper() == admissionNo.ToUpper()).ToList().Select(q => new TbStudent(q)).ToList();
        }
        public List<TbLogin> GetStaffDetails()
        {
            return school.TbLogins.Where(z => z.IsActive && z.RoleId == (int)UserRole.Staff).ToList().Select(q => new TbLogin(q)).ToList();
        }
        public List<TbSmsHistory> GetAllSmsOnSchool()
        {
            var result = _Entities.TbSmsHistories.Where(x => x.ScholId == school.SchoolId).ToList().Select(x => new TbSmsHistory(x)).ToList();
            return result;

        }
        public List<TbBookCategory> GetBookCategory()
        {
            var result = school.TbBookCategories.Where(x => x.IsActive).ToList().Select(x => new TbBookCategory(x)).ToList();
            return result;

        }
        public List<TbLaboratoryCategory> GetLaboratoryCategory()
        {
            var result = school.TbLaboratoryCategories.Where(x => x.IsActive).ToList().Select(x => new TbLaboratoryCategory(x)).ToList();
            return result;

        }
        public List<TbLibraryBook> GetAllBook()
        {
            var result = school.TbBookCategories.Where(x => x.IsActive).ToList().SelectMany(z => z.TbLibraryBooks.Where(m => m.IsActive).ToList()).ToList().Select(x => new TbLibraryBook(x)).ToList();
            return result;

        }
        public List<TbCircular> AllCircularList()
        {
            return _Entities.TbCirculars.Where(z => z.SchoolId == school.SchoolId && z.IsActive).ToList().Select(x => new TbCircular(x)).ToList();
        }

        public List<TbCalenderEvent> GetCalendarUpcomingEvent()
        {
            string StartDate = CurrentTime.ToString("yyyy-MM-dd") + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);
            return school.TbCalenderEvents.Where(z => z.EventDate >= minDate).ToList().Select(z => new TbCalenderEvent(z)).ToList();
        }
        public List<TbCalenderEvent> GetCalendarEventByDate(DateTime startDate, DateTime endDate)
        {
            string StartDate = startDate.Date.ToString("yyyy-MM-dd") + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);
            string EndDate = endDate.Date.ToString("yyyy-MM-dd") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(EndDate);
            return school.TbCalenderEvents.Where(z => z.EventDate >= minDate && z.EventDate <= maxDate).ToList().Select(z => new TbCalenderEvent(z)).ToList();
        }

        public List<IncomeExp> GetTrialBalance()
        {
            List<IncomeExp> list1 = new List<IncomeExp>();
            var income = school.TbIncomes.Where(x => x.IsActive).ToList();
            var groupedAccountHeadIncome = income.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHeadIncome.Count > 0)
            {
                foreach (var item in groupedAccountHeadIncome)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 1;
                    list1.Add(one);
                }
            }

            List<IncomeExp> list2 = new List<IncomeExp>();
            var expense = school.TbExpenses.Where(x => x.IsActive).ToList();
            var groupedAccountHeadExp = expense.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHeadExp.Count > 0)
            {
                foreach (var item in groupedAccountHeadExp)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 2;
                    list2.Add(one);
                }
            }
            var result = list1.Concat(list2).OrderBy(x => x.head).ToList();

            return result;
        }

        public List<BalanceSheetData> GetBalanceSheet()
        {
            List<IncomeExp> list = new List<IncomeExp>();
            List<BalanceSheetData> mainDataList = new List<BalanceSheetData>();
            var exp = school.TbExpenses.Where(x => x.IsActive && x.Particular == "Asset").ToList();
            var groupedAccountHead = exp.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHead.Count > 0)
            {
                foreach (var item in groupedAccountHead)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 4;
                    list.Add(one);
                }
            }

            List<IncomeExp> list2 = new List<IncomeExp>();
            var income = school.TbIncomes.Where(x => x.IsActive && x.Particular == "Liabilities").ToList();
            var groupedAccountHeadInc = income.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHeadInc.Count > 0)
            {
                foreach (var item in groupedAccountHeadInc)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 3;
                    list2.Add(one);
                }
            }
            //---------------Archana -----------------------------------
            int count = 0;
            if (list.Count > list2.Count)
            {
                foreach (var item in list)
                {
                    BalanceSheetData one = new BalanceSheetData();
                    one.Liabilities = item.head;
                    one.LiabilitiesAmount = Convert.ToString(item.amount);
                    if (count + 1 < list2.Count)
                    {
                        one.Asset = list2[count].head;
                        one.AssetAmount = Convert.ToString(list2[count].amount);
                    }
                    else
                    {
                        one.Asset = "";
                        one.AssetAmount = "";
                    }
                    mainDataList.Add(one);
                    count = count + 1;
                }
            }
            else
            {
                foreach (var item in list2)
                {
                    BalanceSheetData one = new BalanceSheetData();
                    one.Asset = item.head;
                    one.AssetAmount = Convert.ToString(item.amount);
                    if (count + 1 < list.Count)
                    {
                        one.Liabilities = list[count].head;
                        one.LiabilitiesAmount = Convert.ToString(list[count].amount);
                    }
                    else
                    {
                        one.Liabilities = "";
                        one.LiabilitiesAmount = "";
                    }
                    mainDataList.Add(one);
                    count = count + 1;
                }
            }
            //  var result = list.Concat(list2).OrderBy(x => x.head).ToList();
            return mainDataList;
        }

        public List<IncomeAndExpenditureData> GetIncomeAndExpenditure()
        {
            List<IncomeAndExpenditureData> mainDataList = new List<IncomeAndExpenditureData>();
            List<IncomeExp> list = new List<IncomeExp>();
            var exp = school.TbExpenses.Where(x => x.IsActive && x.Particular == "Indirect Expense" || x.Particular == "Direct Expense").ToList();
            var groupedAccountHead = exp.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHead.Count > 0)
            {
                foreach (var item in groupedAccountHead)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 5;
                    list.Add(one);
                }
            }

            List<IncomeExp> list2 = new List<IncomeExp>();
            var income = school.TbIncomes.Where(x => x.IsActive && x.Particular == "Indirect Income" || x.Particular == "Direct Income").ToList();
            var groupedAccountHeadInc = income.Select(x =>
               new
               {
                   head = x.AccountHead.ToUpper(),
                   amount = x.Amount
               }).GroupBy(s => new { s.head })
                         .Select(g =>
               new
               {
                   head = g.Key.head,
                   amount = g.Sum(x => (Convert.ToDecimal(x.amount))),
               }
              ).ToList();
            if (groupedAccountHeadInc.Count > 0)
            {
                foreach (var item in groupedAccountHeadInc)
                {
                    IncomeExp one = new IncomeExp();
                    one.head = item.head;
                    one.amount = item.amount;
                    one.accountType = 6;
                    list2.Add(one);
                }
            }

            //-------------------------------------------------------------
            int count = 0;
            if (list.Count > list2.Count)
            {
                foreach (var item in list)
                {
                    IncomeAndExpenditureData one = new IncomeAndExpenditureData();
                    one.Expenditure = item.head;
                    one.ExpenditureAmount = Convert.ToString(item.amount);
                    if (count + 1 < list2.Count)
                    {
                        one.Income = list2[count].head;
                        one.IncomeAmount = Convert.ToString(list2[count].amount);
                    }
                    else
                    {
                        one.Income = "";
                        one.IncomeAmount = "";
                    }
                    mainDataList.Add(one);
                    count = count + 1;
                }
            }
            else
            {
                foreach (var item in list2)
                {
                    IncomeAndExpenditureData one = new IncomeAndExpenditureData();
                    one.Income = item.head;
                    one.IncomeAmount = Convert.ToString(item.amount);
                    if (count + 1 < list.Count)
                    {
                        one.Expenditure = list[count].head;
                        one.ExpenditureAmount = Convert.ToString(list[count].amount);
                    }
                    else
                    {
                        one.Expenditure = "";
                        one.ExpenditureAmount = "";
                    }
                    mainDataList.Add(one);
                    count = count + 1;
                }
            }
            // return list.Concat(list2).OrderBy(x => x.head).ToList(); 
            return mainDataList;
        }
        public List<TbExam> AllExamList(string Classname)
        {
            List<TbExam> data = new List<TbExam>();
            if (Classname != string.Empty)
            {
                if (Classname != null)
                {
                    var classId = _Entities.TbClasses.Where(x => x.Class == Classname && x.IsActive && x.SchoolId == SchoolId && x.PublishStatus).FirstOrDefault();
                    data = _Entities.TbExams.Where(x => x.SchoolId == school.SchoolId && x.IsActive && x.ClassId == classId.ClassId).ToList().Select(x => new TbExam(x)).ToList();
                }
                else
                {
                    data = _Entities.TbExams.Where(x => x.SchoolId == school.SchoolId && x.IsActive).ToList().Select(x => new TbExam(x)).ToList();
                }
            }
            else
            {
                data = _Entities.TbExams.Where(x => x.SchoolId == school.SchoolId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus).ToList().Select(x => new TbExam(x)).ToList();
            }
            return data;
        }
        public List<TbTrip> GetTripDetailById(DateTime selDate, long busId)
        {
            string StartDate = selDate.Date.ToString("yyyy-MM-dd") + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);
            string EndDate = minDate.Date.ToString("yyyy-MM-dd") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(EndDate);
            return school.TbTrips.Where(z => z.TripDate >= minDate && z.TripDate <= maxDate && z.BusId == busId).ToList().Select(z => new TbTrip(z)).ToList();
        }
        public TbFile GetSchoolDiaryLink()
        {
            return school.TbFiles.Where(z => z.IsActive && z.FileModule == 1).FirstOrDefault();
        }
        //----------------- For Account Head Data------------- 06-Apr-2018
        public List<TbSubLedgerDatum> GetAccountDetails()
        {
            var data = school.TbAccountHeads.Where(x => x.IsActive).SelectMany(z => z.TbSubLedgerData).Where(x => x.IsActive).ToList().Select(z => new TbSubLedgerDatum(z)).ToList();
            return data;
        }
        public string CurrentStatus()
        {
            decimal expense = school.TbDayBookData.Where(x => x.IsActive && x.TypeId == 0).Sum(x => x.Amount);
            decimal income = school.TbDayBookData.Where(x => x.IsActive && x.TypeId == 1).Sum(x => x.Amount);
            string amout = "";
            if (expense > income)
            {
                decimal diff = expense - income;
                amout = "Loss : " + diff + " /-";
            }
            else
            {
                decimal diff = income - expense;
                amout = "Profit : " + diff + " /-";
            }
            return amout;
        }
        //------------------For get cash book report 
        public async Task<List<SPCashBookReportData>> GetCashBookDate(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            return await _Entities.SPCashBookReportData
                .FromSqlRaw(
                    "EXEC sp_CashBookReportData @StartDate, @EndDate, @SchoolId",
                    new SqlParameter("@StartDate", start),
                    new SqlParameter("@EndDate", end),
                    new SqlParameter("@SchoolId", school.SchoolId)
                )
                .OrderBy(x => x.EntryDate)
                .ToListAsync();
        }

        public async Task<List<sp_CashBookReportSummary_Result>> GetCashBookDateSummary(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            var result = await _Entities.SPCashBookReportSummary
                .FromSqlRaw("EXEC sp_CashBookReportSummary @StartDate, @EndDate, @SchoolId",
                    new SqlParameter("@StartDate", start),
                    new SqlParameter("@EndDate", end),
                    new SqlParameter("@SchoolId", school.SchoolId))
                .ToListAsync();

            return result;
        }

        public async Task<List<sp_CashBookDailyReport_Result>> GetCashBookDailyReportDate(DateTime startDate, DateTime endDate)
        {
            var result = await _Entities.CashBookDailyReportResult
                .FromSqlRaw("EXEC sp_CashBookDailyReport @startDate1, @endDate1, @schoolId1",
                    new SqlParameter("@startDate1", startDate),
                    new SqlParameter("@endDate1", endDate),
                    new SqlParameter("@schoolId1", school.SchoolId))
                .ToListAsync();

            return result.OrderBy(x => x.EntryDate).ThenBy(x => x.VoucherNo).ToList();
        }

        public async Task<List<LedgerDataModel>> GetLedgerData(DateTime startDate, DateTime endDate, long headId, int typeId)
        {
            var p1 = new SqlParameter("@startDate1", startDate);
            var p2 = new SqlParameter("@endDate1", endDate);
            var p3 = new SqlParameter("@schoolId1", school.SchoolId);

            // Get stored procedure result
            var data = await _Entities.LedgerReportResult
                .FromSqlRaw("EXEC SP_LEDGERDATAREPORT @startDate1, @endDate1, @schoolId1", p1, p2, p3)
                .ToListAsync();

            // Apply filter if specific ledger is selected  
            if (headId != 0)
            {
                data = data.Where(x => x.HeadId == headId && x.FromData == typeId).ToList();
            }

            var dataList = new List<LedgerDataModel>();

            if (data.Any())
            {
                foreach (var head in data.Select(x => x.HeadId).Distinct())
                {
                    var group = data.Where(x => x.HeadId == head).ToList();

                    LedgerDataModel ledger = new LedgerDataModel
                    {
                        HeadId = head,
                        HeadName = group.First().AccHeadName,
                        DebitTotal = group.Sum(x => x.Debit ?? 0),
                        CreditTotal = group.Sum(x => x.Credit ?? 0),
                        list = group.Select(g => new SubLedgerDetails
                        {
                            EntryDate = g.EntryDate ?? DateTime.Now,
                            VoucherNumber = g.VoucherNo,
                            Narration = g.Narration,
                            DebitAmount = g.Debit ?? 0,
                            CreditAmount = g.Credit ?? 0,
                            Symbol = (g.Debit ?? 0) == 0 ? "Cr" : "Dr"
                        }).ToList()
                    };

                    dataList.Add(ledger);
                }

                // Add total row
                dataList.Add(new LedgerDataModel
                {
                    HeadId = 0,
                    HeadName = "Total",
                    CreditTotal = data.Sum(x => x.Credit ?? 0),
                    DebitTotal = data.Sum(x => x.Debit ?? 0)
                });
            }

            return dataList;
        }

        public List<SPDayBookStatus> GetCashBookStatus(DateTime startDate, DateTime endDate)
        {
            var p1 = new SqlParameter("@startDate", startDate);
            var p2 = new SqlParameter("@endDate", endDate);
            var p3 = new SqlParameter("@schoolId", school.SchoolId);

            return _Entities.SPDayBookStatuses
                .FromSqlRaw("EXEC SP_DayBookStatus @startDate, @endDate, @schoolId", p1, p2, p3)
                .ToList()
                .Select(x => new SPDayBookStatus(x))
                .ToList();
        }
        //-------------------For get Bank List
        public List<TbBank> GetBanksDetails()
        {
            var data = school.TbBanks.Where(x => x.IsActive).ToList().Select(z => new TbBank(z)).ToList();
            return data;
        }
        public decimal GetCurrentBalance()
        {
            decimal amount;
            var deposit = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 0).Sum(x => x.Amount);
            var withdraw = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 1).Sum(x => x.Amount);
            //if (deposit > withdraw)
            amount = deposit - withdraw;
            //else
            //    amount = 0;
            return amount;
        }


        public async Task<List<sp_TrialBalance_Result>> GetTrialBalanceList(DateTime StartDate, DateTime Today)
        {
            var schoolId = school.SchoolId;

            var p1 = new SqlParameter("@schoolId1", schoolId);
            var p2 = new SqlParameter("@startDate1", StartDate);
            var p3 = new SqlParameter("@endDate1", Today);

            // Stored Proc Execution
            var data = await _Entities.SpTrialBalanceResults
                .FromSqlRaw("EXEC sp_TrialBalance @schoolId1, @startDate1, @endDate1", p1, p2, p3)
                .ToListAsync();

            // Calculates totals
            decimal totalDebit = data.Sum(x => x.Expense ?? 0);
            decimal totalCredit = data.Sum(x => x.Income ?? 0);

            // GET closing balance from SP_DayBookStatus
            var cashData = await _Entities.SPDayBookStatuses
                .FromSqlRaw("EXEC SP_DayBookStatus @startDate1, @endDate1, @schoolId1", p2, p3, p1)
                .FirstOrDefaultAsync();

            if (cashData != null)
            {
                var cashRow = new sp_TrialBalance_Result();
                cashRow.AccHeadName = "Cash in Hand";

                if (cashData.ClosingBalance >= 0)
                    cashRow.Expense = cashData.ClosingBalance; // Debit
                else
                    cashRow.Income = Math.Abs(cashData.ClosingBalance.Value); // Credit if negative

                data.Add(cashRow);

                totalDebit += cashRow.Expense ?? 0;
                totalCredit += cashRow.Income ?? 0;
            }

            // Append final row
            data.Add(new sp_TrialBalance_Result
            {
                AccHeadName = "TOTAL",
                Expense = totalDebit,
                Income = totalCredit
            });

            return data;
        }
        public async Task<List<SPBalanceSheet>> GetBalanceSheetList()
        {
            var schoolId = school.SchoolId;

            var p1 = new SqlParameter("@schoolId", schoolId);

            var result = await _Entities.SPBalanceSheetResults
                .FromSqlRaw("EXEC sp_BalanceSheet @schoolId", p1)
                .ToListAsync();

            List<SPBalanceSheet> data = new List<SPBalanceSheet>();

            var assetsData = result.Where(x => x.TypeId == 0).ToList();
            var liabilityData = result.Where(x => x.TypeId == 1).ToList();

            int i = 0;

            // Merge Assets & Liability Side by Side
            int max = Math.Max(assetsData.Count, liabilityData.Count);

            while (i < max)
            {
                var row = new SPBalanceSheet();

                if (i < assetsData.Count)
                {
                    row.Head = assetsData[i].Head;
                    row.Assets = assetsData[i].Assets;
                    row.AssetsAmount = assetsData[i].AssetsAmount;
                }

                if (i < liabilityData.Count)
                {
                    row.Liability = liabilityData[i].Liability;
                    row.LiabilityAmount = liabilityData[i].LiabilityAmount;
                }

                data.Add(row);
                i++;
            }

           
            var cash = await _Entities.SPDayBookStatuses
                .FromSqlRaw("EXEC SP_DayBookStatus @startDate, @endDate, @schoolId",
                    new SqlParameter("@startDate", DateTime.Now),
                    new SqlParameter("@endDate", DateTime.Now),
                    new SqlParameter("@schoolId", schoolId))
                .FirstOrDefaultAsync();

            if (cash != null)
            {
                data.Add(new SPBalanceSheet
                {
                    Head = "Cash Book",
                    Assets = cash.ClosingBalance > 0 ? "Cash In Hand" : "",
                    AssetsAmount = cash.ClosingBalance > 0 ? cash.ClosingBalance : null,
                    Liability = cash.ClosingBalance < 0 ? "Cash In Hand" : "",
                    LiabilityAmount = cash.ClosingBalance < 0 ? Math.Abs(cash.ClosingBalance.Value) : null
                });
            }

            
            data.Add(new SPBalanceSheet
            {
                Head = "Total",
                Assets = "Total",
                AssetsAmount = data.Sum(x => x.AssetsAmount ?? 0),
                Liability = "Total",
                LiabilityAmount = data.Sum(x => x.LiabilityAmount ?? 0)
            });

            return data;
        }
        public async Task<List<sp_BankStatement_Result>> GetBankStatementList(DateTime startDate, DateTime endDate, long BankId)
        {
            var p1 = new SqlParameter("@schoolId", school.SchoolId);
            var p2 = new SqlParameter("@startDate", startDate);
            var p3 = new SqlParameter("@endDate", endDate);
            var p4 = new SqlParameter("@bankId", BankId);

            var result = await _Entities.SPBankStatementResults
                .FromSqlRaw("EXEC sp_BankStatement @schoolId, @startDate, @endDate, @bankId", p1, p2, p3, p4)
                .ToListAsync();

            return result;
        }
        public decimal CurrentBankBalance(DateTime startDate, DateTime enddate)
        {
            string StartDate = startDate.ToString("yyyy-MM-dd") + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);
            string EndDate = enddate.ToString("yyyy-MM-dd") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(EndDate);
            var Withdraw = school.TbBankBookData.Where(x => x.EntryDate > minDate && x.EntryDate <= maxDate && x.IsActive && x.TypeId == 1).Sum(x => x.Amount);
            var Deposit = school.TbBankBookData.Where(x => x.EntryDate > minDate && x.EntryDate <= maxDate && x.IsActive && x.TypeId == 0).Sum(x => x.Amount);
            var balance = Deposit - Withdraw;
            return balance;
        }
        public async Task<Tuple<decimal, int, int, decimal, decimal>> Statements(DateTime startDate, DateTime enddate, long BankId)
        {
            var p1 = new SqlParameter("@startDate", startDate);
            var p2 = new SqlParameter("@endDate", enddate);
            var p3 = new SqlParameter("@schoolId", school.SchoolId);
            var p4 = new SqlParameter("@bankId", BankId);

            var openingBalance = await _Entities.SPBankYesterdayBalances
                .FromSqlRaw("EXEC sp_BankYesterdayBalance @startDate, @endDate, @schoolId, @bankId", p1, p2, p3, p4)
                .FirstOrDefaultAsync();

            decimal balance = openingBalance?.Balance ?? 0;

            
            var data = await GetBankStatementList(startDate, enddate, BankId);

           
            int creditCount = data.Count(x => x.Deposit != 0);
            int debitCount = data.Count(x => x.Withdraw != 0);
            decimal totalCredits = (decimal)data.Sum(x => x.Deposit);
            decimal totalDebits = (decimal)data.Sum(x => x.Withdraw);

            return new Tuple<decimal, int, int, decimal, decimal>(balance, debitCount, creditCount, totalDebits, totalCredits);
        }
        public List<ReceiptPaymentDataModel> GetReceiptPayment(DateTime startdate, DateTime enddate)
        {

            var receiptPayment = new Satluj_Latest.Data.School(school.SchoolId).GetReceiptPaymentBankData();
            var openingBankBalance = new Satluj_Latest.Data.School(school.SchoolId).GetReceiptPaymentBankDataByDate(startdate);
            var cashInHand = new Satluj_Latest.Data.School(school.SchoolId).GetCashBookStatus(startdate, enddate);
            decimal cashInHandRP = cashInHand.FirstOrDefault().OpeningBalance ?? 0;
            decimal closingBalanceRP = cashInHand.FirstOrDefault().ClosingBalance ?? 0;

            List<ReceiptPaymentDataModel> list = new List<ReceiptPaymentDataModel>();
            //ReceiptPaymentDataModel openingBankBalanceRP = new ReceiptPaymentDataModel();
            //openingBankBalanceRP.Type = 0;
            //openingBankBalanceRP.Receipt = "Opening Balance";
            //openingBankBalanceRP.Payment = "";
            //openingBankBalanceRP.ReceiptNarration = "";
            //openingBankBalanceRP.PaymentNarration = "";
            //openingBankBalanceRP.ReceiptAmount = cashInHand.FirstOrDefault().OpeningBalance ?? 0;
            //openingBankBalanceRP.PaymentAmount = 0;
            //list.Add(openingBankBalanceRP);
            //foreach (var item in receiptPayment)
            //{
            //    ReceiptPaymentDataModel receiptPaymentRP = new ReceiptPaymentDataModel();
            //    receiptPaymentRP.Type = 0;
            //    receiptPaymentRP.Receipt = item.BankName;
            //    receiptPaymentRP.ReceiptAmount = item.amount;
            //    receiptPaymentRP.ReceiptNarration = "";
            //    receiptPaymentRP.Payment = "";
            //    receiptPaymentRP.PaymentAmount = 0;
            //    receiptPaymentRP.PaymentNarration = "";
            //    list.Add(receiptPaymentRP);
            //}

            var p1 = new SqlParameter("@startDate1", startdate);
            var p2 = new SqlParameter("@endDate1", enddate);
            var p3 = new SqlParameter("@schoolId1", school.SchoolId);

            var data = _Entities.SPReceiptAndPaymentData
                .FromSqlRaw("EXEC sp_ReceiptAndPaymentData @startDate1, @endDate1, @schoolId1", p1, p2, p3)
                .ToList();
            var IncomeData = data.Where(x => x.TypeId == 1).GroupBy(x => new { x.TypeId, x.AccHeadName }).Select(y => new sp_ReceiptAndPaymentData_Result
            {
                TypeId = y.Key.TypeId,
                AccHeadName = y.Key.AccHeadName,
                Expense = y.Sum(x => x.Expense),
                Income = y.Sum(x => x.Income)

            }).ToList();
            var ExpenseData = data.Where(x => x.TypeId == 0).GroupBy(x => new { x.TypeId, x.AccHeadName }).Select(y => new sp_ReceiptAndPaymentData_Result
            {
                TypeId = y.Key.TypeId,
                AccHeadName = y.Key.AccHeadName,
                Expense = y.Sum(x => x.Expense),
                Income = y.Sum(x => x.Income)
            }).ToList();
            int count = 0;
            if (data.Count > 0)
            {
                if (IncomeData.Count >= ExpenseData.Count)
                {
                    foreach (var item in IncomeData)
                    {
                        ReceiptPaymentDataModel one = new ReceiptPaymentDataModel();
                        one.Type = item.TypeId;
                        one.Receipt = item.AccHeadName;
                        one.ReceiptAmount = item.Income ?? 0;
                        one.ReceiptNarration = item.Narration;
                        try
                        {
                            one.Payment = ExpenseData[count].AccHeadName;
                            one.PaymentAmount = ExpenseData[count].Expense ?? 0;
                            one.PaymentNarration = ExpenseData[count].Narration;
                        }
                        catch
                        {
                            one.Payment = "";
                            one.PaymentAmount = 0;
                            one.PaymentNarration = "";
                        }

                        list.Add(one);
                        count = count + 1;
                    }

                }
                else
                {
                    foreach (var item in ExpenseData)
                    {
                        ReceiptPaymentDataModel one = new ReceiptPaymentDataModel();
                        one.Type = item.TypeId;
                        one.Payment = item.AccHeadName;
                        one.PaymentAmount = item.Expense ?? 0;
                        one.PaymentNarration = item.Narration;
                        try
                        {
                            one.Receipt = IncomeData[count].AccHeadName;
                            one.ReceiptAmount = IncomeData[count].Income ?? 0;
                            one.ReceiptNarration = IncomeData[count].Narration;
                        }
                        catch
                        {
                            one.Receipt = "";
                            one.ReceiptAmount = 0;
                            one.ReceiptNarration = "";
                        }
                        list.Add(one);
                        count = count + 1;
                    }
                }
                ReceiptPaymentDataModel total = new ReceiptPaymentDataModel();
                total.Type = 0;
                total.Receipt = "Total";
                total.Payment = "Total";
                total.ReceiptNarration = "";
                total.PaymentNarration = "";
                total.ReceiptAmount = list.Sum(x => x.ReceiptAmount) + cashInHandRP;
                total.PaymentAmount = list.Sum(x => x.PaymentAmount);
                list.Add(total);
            }
            return list;
        }
        public List<BankBalanceAmountModel> GetReceiptPaymentBankData()
        {
            List<BankBalanceAmountModel> list = new List<BankBalanceAmountModel>();
            var data = school.TbBankBookData.Where(x => x.IsActive).ToList().Select(x => new BankBookData(x)).ToList();
            var banks = school.TbBanks.Where(x => x.IsActive).ToList();
            foreach (var item in banks)
            {
                BankBalanceAmountModel one = new BankBalanceAmountModel();
                one.BankName = item.BankName;
                var deposit = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 0 && x.BankId == item.BankId).Sum(x => x.Amount);
                var withdraw = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 1 && x.BankId == item.BankId).Sum(x => x.Amount);
                one.amount = deposit - withdraw;
                list.Add(one);
            }
            return list;
        }
        public List<BankBalanceAmountModel> GetReceiptPaymentBankDataByDate(DateTime eDate)
        {
            List<BankBalanceAmountModel> list = new List<BankBalanceAmountModel>();
            var data = school.TbBankBookData.Where(x => x.IsActive).ToList().Select(x => new BankBookData(x)).ToList();
            var banks = school.TbBanks.Where(x => x.IsActive).ToList();
            foreach (var item in banks)
            {
                BankBalanceAmountModel one = new BankBalanceAmountModel();
                one.BankName = item.BankName;
                var deposit = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 0 && x.BankId == item.BankId && x.EntryDate < eDate).Sum(x => x.Amount);
                var withdraw = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 1 && x.BankId == item.BankId && x.EntryDate < eDate).Sum(x => x.Amount);
                one.amount = deposit - withdraw;
                list.Add(one);
            }
            return list;
        }
        public decimal GetBankCurrentBalance(long BankId)
        {
            decimal amount;
            var deposit = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 0 && x.BankId == BankId).Sum(x => x.Amount);
            var withdraw = school.TbBankBookData.Where(x => x.IsActive && x.TypeId == 1 && x.BankId == BankId).Sum(x => x.Amount);
            amount = deposit - withdraw;
            return amount;
        }
        public List<Subjects> GetAllSubjects()
        {
            var data = school.TbSubjects.Where(x => x.IsActive).ToList().Select(z => new Subjects(z)).ToList();
            return data;
        }

        public List<TimetableListingModel> GetTimetable(long ClassId, long DivisionId)
        {
            var data = _Entities.TbTimeTables.Where(x => x.ClassId == ClassId && x.DivisionId == DivisionId && x.IsActive).ToList().OrderBy(x => x.Periods).Select(x => new TimeTable(x)).ToList();
            List<TimetableListingModel> list = new List<TimetableListingModel>();
            for (int i = 0; i < 6; i++)
            {
                var newData = data.Where(x => x.DayId == i).ToList();
                TimetableListingModel one = new TimetableListingModel();
                if (newData.Count > 0)
                {
                    one.DayName = newData[0].DayName;
                    one.One = newData.Where(x => x.Periods == 1).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 1).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 1).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.two = newData.Where(x => x.Periods == 2).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 2).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 2).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.three = newData.Where(x => x.Periods == 3).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 3).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 3).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.four = newData.Where(x => x.Periods == 4).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 4).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 4).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.five = newData.Where(x => x.Periods == 5).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 5).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 5).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.six = newData.Where(x => x.Periods == 6).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 6).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 6).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.seven = newData.Where(x => x.Periods == 7).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 7).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 7).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                    one.eight = newData.Where(x => x.Periods == 8).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 8).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 8).Select(x => x.SubjectAbbreviation).FirstOrDefault().ToUpper();
                }
                else
                {
                    if (i == 0)
                        one.DayName = "Monday";
                    else if (i == 1)
                        one.DayName = "Tuesday";
                    else if (i == 2)
                        one.DayName = "Wednesday";
                    else if (i == 3)
                        one.DayName = "Thursday";
                    else if (i == 4)
                        one.DayName = "Friday";
                    if (i == 5)
                        one.DayName = "Saturday";
                    one.One = "";
                    one.two = "";
                    one.three = "";
                    one.four = "";
                    one.five = "";
                    one.six = "";
                    one.seven = "";
                    one.eight = "";
                }
                list.Add(one);
            }
            return list;
        }
        public List<Payment> GetDetailedCollectionReportDate(DateTime StartDate, DateTime endDate)
        {
            var p1 = new SqlParameter("@SchoolId", school.SchoolId);
            var p2 = new SqlParameter("@StartDate", StartDate);
            var p3 = new SqlParameter("@EndDate", endDate);

            var result = _Entities.SPDetailedCollectionReportResults
                .FromSqlRaw("EXEC sp_DetailedCollectionReport @SchoolId, @StartDate, @EndDate", p1, p2, p3)
                .ToList();
            return result.Select(x => new Payment(x.PaymentId)).ToList();
        }
        public List<Student> GetAllStudentdata(long classId, long divisionId)
        {
            if (divisionId == 0)
            {
                return school.TbStudents.Where(x => x.ClassId == classId && x.IsActive).ToList().OrderBy(x => x.ClassId).ThenBy(x => x.DivisionId).ThenBy(x => x.StundentName).Select(x => new Student(x)).ToList();
            }
            else
            {
                return school.TbStudents.Where(x => x.ClassId == classId && x.DivisionId == divisionId && x.IsActive).ToList().OrderBy(x => x.ClassId).ThenBy(x => x.DivisionId).ThenBy(x => x.StundentName).Select(x => new Student(x)).ToList();
            }
        }


        public List<SP_GetLibraryDueBook_Result> GetBookDueList()
        {
            DateTime today = DateTime.Today;

            var schoolIdParam = new SqlParameter("@schoolId", school.SchoolId);
            var dateParam = new SqlParameter("@startDate", today);

            var data = _Entities.SPGetLibraryDueBookResult
                .FromSqlRaw("EXEC SP_GetLibraryDueBook @schoolId, @startDate", schoolIdParam, dateParam)
                .ToList();

            return data;
        }

        public List<SP_GetDailyFeeCollection_Home_Result> GetDailyFeeCollectionHome()
        {
            DateTime minDate = DateTime.Today.Date; 
            DateTime maxDate = minDate.AddDays(1).AddSeconds(-1); 

            var schoolIdParam = new SqlParameter("@schoolId", school.SchoolId);
            var startParam = new SqlParameter("@startDate", minDate);
            var endParam = new SqlParameter("@endDate", maxDate);

            var data = _Entities.SPGetDailyFeeCollectionHomeResult
                .FromSqlRaw("EXEC SP_GetDailyFeeCollection_Home @schoolId, @startDate, @endDate",
                    schoolIdParam, startParam, endParam)
                .ToList();

            return data;
        }

        public List<SP_GetLibraryBookCount_Home_Result> GetLibraryBookountHome()
        {
            var p1 = new SqlParameter("@schoolId", school.SchoolId);

            var data = _Entities.SPGetLibraryBookCountHomeResults
                .FromSqlRaw("EXEC SP_GetLibraryBookCount_Home @schoolId", p1)
                .ToList();

            return data;
        }


        public List<SPMonthlyAttendance> GetMonthlyAttendancereportReportDate(DateTime StartDate, long ClassId, long DivisionId, int ShiftId)
        {
            var p1 = new SqlParameter("@startDate", StartDate);
            var p2 = new SqlParameter("@schoolId", school.SchoolId);
            var p3 = new SqlParameter("@session", ShiftId == 1);
            var p4 = new SqlParameter("@classId", ClassId);
            var p5 = new SqlParameter("@divisionId", DivisionId);

            var result = _Entities.SPMonthlyAttendanceResults
                .FromSqlRaw("EXEC sp_MonthlyAttendance @startDate, @schoolId, @session, @classId, @divisionId",
                p1, p2, p3, p4, p5)
                .ToList();

            return result.Select(x => new SPMonthlyAttendance(x)).ToList();
        }

        public List<Fee> GetSpecialFeeList()
        {
            return school.TbFees.Where(z => z.IsActive && z.FeeType == (int)FeeType.SpecialFee).OrderBy(z => z.FeesName).ToList().Select(z => new Fee(z)).ToList();
        }

        public List<TimetableListingModel> GetMyTimetable(long TeacheruserId)
        {
            long TeacherId = school.TbTeachers.Where(x => x.UserId == TeacheruserId && x.IsActive).Select(x => x.TeacherId).FirstOrDefault();
            var data = school.TbTimeTables.Where(x => x.TeacherId == TeacherId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus == true).ToList().OrderBy(x => x.Periods).Select(x => new TimeTable(x)).ToList();
            var subList = school.TbSubjects.Where(x => x.IsActive).ToList();
            List<TimetableListingModel> list = new List<TimetableListingModel>();
            for (int i = 0; i < 6; i++)
            {
                var newData = data.Where(x => x.DayId == i).ToList().Select(x => new TimeTable(x.Id)).ToList();
                TimetableListingModel one = new TimetableListingModel();
                if (newData.Count > 0)
                {
                    one.DayName = newData[0].DayName;
                    //one.One = newData.Where(x => x.Periods == 1).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 1).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 1).Select(x => x.Subject).FirstOrDefault();
                    //one.two = newData.Where(x => x.Periods == 2).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 2).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 2).Select(x => x.Subject).FirstOrDefault();
                    //one.three = newData.Where(x => x.Periods == 3).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 3).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 3).Select(x => x.Subject).FirstOrDefault();
                    //one.four = newData.Where(x => x.Periods == 4).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 4).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 4).Select(x => x.Subject).FirstOrDefault();
                    //one.five = newData.Where(x => x.Periods == 5).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 5).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 5).Select(x => x.Subject).FirstOrDefault();
                    //one.six = newData.Where(x => x.Periods == 6).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 6).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 6).Select(x => x.Subject).FirstOrDefault(); 
                    //one.seven = newData.Where(x => x.Periods == 7).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 7).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 7).Select(x => x.Subject).FirstOrDefault();
                    //one.eight = newData.Where(x => x.Periods == 8).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 8).Select(x => x.DivisionName).FirstOrDefault() + " - " + newData.Where(x => x.Periods == 8).Select(x => x.Subject).FirstOrDefault();

                    one.One = newData.Where(x => x.Periods == 1).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 1).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 1).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 1).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 1).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.two = newData.Where(x => x.Periods == 2).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 2).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 2).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 2).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 2).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.three = newData.Where(x => x.Periods == 3).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 3).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 3).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 3).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 3).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.four = newData.Where(x => x.Periods == 4).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 4).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 4).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 4).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 4).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.five = newData.Where(x => x.Periods == 5).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 5).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 5).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 5).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 5).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.six = newData.Where(x => x.Periods == 6).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 6).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 6).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 6).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 6).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.seven = newData.Where(x => x.Periods == 7).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 7).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 7).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 7).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 7).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                    one.eight = newData.Where(x => x.Periods == 8).Select(x => x.ClassName).FirstOrDefault() + "-" + newData.Where(x => x.Periods == 8).Select(x => x.DivisionName).FirstOrDefault() + " - " + Convert.ToString(newData.Where(x => x.Periods == 8).Select(x => x.SubjectAbbreviation).FirstOrDefault() == null ? newData.Where(x => x.Periods == 8).Select(x => x.Subject).FirstOrDefault() : newData.Where(x => x.Periods == 8).Select(x => x.SubjectAbbreviation).FirstOrDefault());
                }
                else
                {
                    if (i == 0)
                        one.DayName = "Monday";
                    else if (i == 1)
                        one.DayName = "Tuesday";
                    else if (i == 2)
                        one.DayName = "Wednesday";
                    else if (i == 3)
                        one.DayName = "Thursday";
                    else if (i == 4)
                        one.DayName = "Friday";
                    if (i == 5)
                        one.DayName = "Saturday";
                    one.One = "";
                    one.two = "";
                    one.three = "";
                    one.four = "";
                    one.five = "";
                    one.six = "";
                    one.seven = "";
                    one.eight = "";
                }
                list.Add(one);
            }
            return list;
        }

        public List<OutstandingReportMainModel> GetOutStandingData(long ClassId, long DivisionId, long FeeId)
        {

            var result = new List<OutstandingReportMainModel>();

            
            var data = _Entities.SPOutstandingReportNew
                .FromSqlRaw("EXEC SP_OutstandingReportNew @SchoolId, @ClassId, @DivisionId, @FeeId",
                    new SqlParameter("@SchoolId", school.SchoolId),
                    new SqlParameter("@ClassId", ClassId),
                    new SqlParameter("@DivisionId", DivisionId),
                    new SqlParameter("@FeeId", FeeId))
                .ToList()
                .Select(x => new SP_OutstandingReportNew_Result(x))
                .ToList();

            if (data.Count > 0)
            {
                var students = data.Select(o => o.StudentId).Distinct().ToList().Select(o => new Satluj_Latest.Data.Student(o)).ToList().OrderBy(x => x.StundentName);
                foreach (var item in students)
                {
                    decimal totalAmount = 00;
                    var discountFee = item.Discount.ToList();
                    var list = data.Where(x => x.StudentId == item.StudentId).ToList();
                    OutstandingReportMainModel one = new OutstandingReportMainModel();
                    one.SubList = new List<SubList>();
                    one.StudentId = item.StudentId;
                    one.StudentName = item.StundentName;
                    one.ClassDetails = item.ClassName + " / " + item.DivisionName;
                    one.ContactNumber = item.ContactNumber;
                    one.DivisionId = item.DivisionId;
                    one.ClassOrder = item.ClassOrder;
                    foreach (var sub in list)
                    {
                        SubList subOne = new SubList();
                        var disc = discountFee.Where(x => x.feeId == sub.FeeId && sub.DiscountAllowed == 0).FirstOrDefault();
                        if (disc != null)
                        {
                            decimal amount = (sub.Amount ?? 0) - disc.discountAmount;
                            subOne.Amount = amount;
                        }
                        else
                        {
                            subOne.Amount = sub.Amount ?? 0;
                        }
                        subOne.FeeId = sub.FeeId;
                        subOne.FeeName = sub.Feename;
                        one.SubList.Add(subOne);
                        totalAmount = totalAmount + subOne.Amount;
                    }
                    one.Total = totalAmount;
                    result.Add(one);
                }
                List<OutstandingReportMainModel> newModel = new List<OutstandingReportMainModel>();
                newModel = result.OrderBy(x => x.ClassOrder).ThenBy(x => x.DivisionId).ThenBy(x => x.StudentName).ToList();
                return newModel;
            }
            else
            {
                return result;
            }

        }

        public List<OutstandingReportMainModel> GetBilledData(DateTime startDate, DateTime endDate)
        {

            List<OutstandingReportMainModel> model = new List<OutstandingReportMainModel>();

            var data = _Entities.SPBilledReport
                .FromSqlRaw("EXEC Sp_BilledReport @SchoolId, @StartDate, @EndDate",
                    new SqlParameter("@SchoolId", school.SchoolId),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate))
                .ToList();
            if (data.Count > 0)
            {
                var students = data.Select(o => o.StudentId).Distinct().ToList().Select(o => new Satluj_Latest.Data.Student(o)).ToList().OrderBy(x => x.StundentName);
                foreach (var item in students)
                {
                    decimal totalAmount = 00;
                    var discountFee = item.Discount.ToList();
                    var list = data.Where(x => x.StudentId == item.StudentId).ToList();
                    OutstandingReportMainModel one = new OutstandingReportMainModel();
                    one.SubList = new List<SubList>();
                    one.StudentId = item.StudentId;
                    one.StudentName = item.StundentName;
                    one.ClassDetails = item.ClassName + " / " + item.DivisionName;
                    one.ContactNumber = item.ContactNumber;
                    one.DivisionId = item.DivisionId;
                    one.ClassOrder = item.ClassOrder;
                    foreach (var sub in list)
                    {
                        SubList subOne = new SubList();
                        var disc = discountFee.Where(x => x.feeId == sub.FeeId && sub.DiscountAllowed == 0).FirstOrDefault();
                        if (disc != null)
                        {
                            decimal amount = (sub.Amount ?? 0) - disc.discountAmount;
                            subOne.Amount = amount;
                        }
                        else
                        {
                            subOne.Amount = sub.Amount ?? 0;
                        }
                        subOne.FeeId = sub.FeeId;
                        subOne.FeeName = sub.Feename;
                        one.SubList.Add(subOne);
                        totalAmount = totalAmount + subOne.Amount;
                    }
                    one.Total = totalAmount;
                    model.Add(one);
                }
                List<OutstandingReportMainModel> newModel = new List<OutstandingReportMainModel>();
                newModel = model.OrderBy(x => x.ClassOrder).ThenBy(x => x.DivisionId).ThenBy(x => x.StudentName).ToList();
                return newModel;
            }
            else
            {
                return model;
            }

        }


        public SchoolSenderId GetSenderDetails()
        {
            var data = school.TbSchoolSenderIds.Where(x => x.IsActive == true).ToList().Select(x => new SchoolSenderId(x)).FirstOrDefault();
            return data;
        }
        public List<Exams> GetProgressData(long studentId, long classId)
        {
            var data = _Entities.TbExams.Where(x => x.ClassId == classId && x.IsActive).ToList().OrderBy(x => x.StartDate).Select(x => new Exams(x)).ToList();
            return data;
        }
        public List<StockUpdate> GetCurrentStock()
        {
            var data = school.TbStockUpdates.Where(x => x.IsActive).ToList().Select(x => new StockUpdate(x)).OrderByDescending(x => x.TimeStamp).ToList();
            return data;
        }
        public List<StockUpdate> GetCurrentStockReport(long catId)
        {
            if (catId != 0)
            {
                var data = school.TbStockUpdates.Where(x => x.IsActive && x.CategoryId == catId).ToList().Select(x => new StockUpdate(x)).OrderByDescending(x => x.TimeStamp).ToList();
                return data;
            }
            else
            {
                var data = school.TbStockUpdates.Where(x => x.IsActive).ToList().Select(x => new StockUpdate(x)).OrderByDescending(x => x.TimeStamp).ToList();
                return data;
            }
        }
        public AccountHead GetFeeIncomeHead()//**
        {
            var data = school.TbAccountHeads.Where(x => x.IsActive == true && x.ForBill == true).ToList().Select(x => new AccountHead(x)).FirstOrDefault();
            return data;
        }
        public string GetOpeningBalance(DateTime fromDate, long bankId, int sourceId)// sourceId:0=Cash ,1=Bank
        {
            try
            {
                var pSchoolId = new SqlParameter("@schoolId", school.SchoolId);
                var pFromDate = new SqlParameter("@fromDate", fromDate);
                var pBankId = new SqlParameter("@bankId", bankId);
                var pSourceId = new SqlParameter("@souceId", sourceId);

                
                var data =  _Entities.SPOpeningBalanceAmountInCashBook
                    .FromSqlRaw("EXEC sp_OpeningBalanceAmountInCashBook @schoolId, @fromDate, @bankId, @souceId",
                        pSchoolId, pFromDate, pBankId, pSourceId)
                    .ToList();
                if (sourceId == 2 && bankId == 0)
                {
                    decimal close = data.Sum(x => x.Opening ?? 0m);
                    return close.ToString();
                }
                else
                {
                    return (data.FirstOrDefault()?.Opening ?? 0m).ToString();
                }
                }
            catch (Exception)
            {
                return "0";
            }
        }
        public List<sp_CashEntryReport_Result> GetCashBookReport(DateTime fromdate, DateTime toDate, long schoolId)
        {
            var p1 = new SqlParameter("@schoolId", schoolId);
            var p2 = new SqlParameter("@fromDate", fromdate);
            var p3 = new SqlParameter("@toDate", toDate);

            var result = _Entities.SPCashEntryReport
                .FromSqlRaw("EXEC sp_CashEntryReport @schoolId, @fromDate, @toDate", p1, p2, p3)
                .ToList();

            return result.Select(x => new sp_CashEntryReport_Result(x)).ToList();
        }
        public string GetClosingBalance(DateTime fromDate, long bankId, int sourceId)// sourceId:0=Cash ,1=Bank
        {
            try
            {
                var p1 = new SqlParameter("@schoolId", school.SchoolId);
                var p2 = new SqlParameter("@toDate", fromDate);
                var p3 = new SqlParameter("@bankId", bankId);
                var p4 = new SqlParameter("@souceId", sourceId);

                if (bankId == 0 && sourceId == 2)
                {
                    var list = _Entities.SPClosingBalanceAmountResult
                        .FromSqlRaw("EXEC sp_ClosingBalanceAmount @schoolId, @toDate, @bankId, @souceId",
                            p1, p2, p3, p4)
                        .ToList();

                    decimal total = 0;
                    long lastBank = -1;

                    foreach (var row in list)
                    {
                        if (row.BankId != lastBank)
                        {
                            total += (decimal)row.Opening;
                        }
                        lastBank = row.BankId ?? 0;
                    }

                    return total.ToString();
                }
                else
                {
                   
                    var data = _Entities.SPClosingBalanceAmountResult
                        .FromSqlRaw("EXEC sp_ClosingBalanceAmount @schoolId, @toDate, @bankId, @souceId",
                            p1, p2, p3, p4)
                        .FirstOrDefault();

                    return data?.Opening.ToString() ?? "0";
                }
            }
            catch
            {
                return "0";
            }
        }
        public List<sp_BankEntryReport_Result> GetBankBookReport(DateTime fromdate, DateTime toDate, long schoolId, long bankId)
        {
            var p1 = new SqlParameter("@schoolId", schoolId);
            var p2 = new SqlParameter("@fromDate", fromdate);
            var p3 = new SqlParameter("@toDate", toDate);
            var p4 = new SqlParameter("@bankId", bankId);

            var list = _Entities.SPBankEntryReport
                .FromSqlRaw("EXEC sp_BankEntryReport @schoolId, @fromDate, @toDate, @bankId",
                p1, p2, p3, p4)
                .ToList();

            return list;
        }
        public List<ReceiptPaymentNewModel> GetRPReport(DateTime fromdate, DateTime toDate, long schoolId)
        {
            var bankList = school.TbBanks.Where(x => x.IsActive).ToList();
            List<ReceiptPaymentNewModel> list = new List<ReceiptPaymentNewModel>();
            var p1 = new SqlParameter("@schoolId", schoolId);
            var p2 = new SqlParameter("@fromDate", fromdate);
            var p3 = new SqlParameter("@toDate", toDate);

            var data= _Entities.SPReceiptAndPaymentResult
                .FromSqlRaw("EXEC sp_ReceiptPayment @schoolId, @fromDate, @toDate", p1, p2, p3)
                .ToList();
            if (data != null && data.Count > 0)
            {
                var receiptList = data.Where(x => x.Payment == 0 && x.Receipt != 0).OrderBy(x => x.FromData).OrderBy(x => x.AccHeadName).ToList();
                var paymentList = data.Where(x => x.Payment != 0 && x.Receipt == 0).OrderBy(x => x.FromData).OrderBy(x => x.AccHeadName).ToList();
                foreach (var item in receiptList)
                {
                    var head = "";
                    if (item.FromData == 3 || item.FromData == 4)
                    {
                        if (Convert.ToInt32(item.BillNo) == 0)
                            head = item.AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == item.Id).Select(x => x.SubLedgerName).FirstOrDefault();
                        else
                            head = item.AccHeadName + " " + school.TbFees.Where(x => x.FeeId == item.Id).Select(x => x.FeesName).FirstOrDefault();
                    }
                    else
                    {
                        head = item.AccHeadName;
                    }
                    receiptList.Where(w => w == item).ToList().ForEach(s => s.AccHeadName = head);
                }
                receiptList = receiptList.GroupBy(x => x.AccHeadName).Select(m => new Models.sp_ReceiptPayment_Result
                {
                    AccHeadName = m.First().AccHeadName,
                    FromData = m.First().FromData,
                    Id = 0,
                    BillNo = 0,
                    Receipt = m.Sum(p => p.Receipt),
                    Payment = m.Sum(p => p.Payment)
                }).ToList();
                //-------------------------------------------------------------
                foreach (var item in paymentList)
                {
                    var head = "";
                    if (item.FromData == 3 || item.FromData == 4)
                    {
                        if (Convert.ToInt32(item.BillNo) == 0)
                            head = item.AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == item.Id).Select(x => x.SubLedgerName).FirstOrDefault();
                        else
                            head = item.AccHeadName + " " + school.TbFees.Where(x => x.FeeId == item.Id).Select(x => x.FeesName).FirstOrDefault();
                    }
                    else
                    {
                        head = item.AccHeadName;
                    }
                    paymentList.Where(w => w == item).ToList().ForEach(s => s.AccHeadName = head);
                }
                paymentList = paymentList.GroupBy(x => x.AccHeadName).Select(m => new Models.sp_ReceiptPayment_Result
                {
                    AccHeadName = m.First().AccHeadName,
                    FromData = m.First().FromData,
                    Id = 0,
                    BillNo = 0,
                    Receipt = m.Sum(p => p.Receipt),
                    Payment = m.Sum(p => p.Payment)
                }).ToList();
                //-------------------------------------------------------------
                int i = 0;
                #region Data Model Creating
                if (receiptList.Count > paymentList.Count)
                {
                    foreach (var item in receiptList)
                    {
                        ReceiptPaymentNewModel one = new ReceiptPaymentNewModel();
                        //if (item.FromData == 3 || item.FromData == 4)
                        //{
                        //    if (Convert.ToInt32(item.BillNo) == 0)
                        //        one.AccountHeadReceipt = item.AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == item.Id).Select(x => x.SubLedgerName).FirstOrDefault();
                        //    else
                        //        one.AccountHeadReceipt = item.AccHeadName + " " + school.tb_Fee.Where(x => x.FeeId == item.Id).Select(x => x.FeesName).FirstOrDefault();
                        //}
                        //else
                        //{
                        one.AccountHeadReceipt = item.AccHeadName;
                        //}
                        one.receipt = item.Receipt ?? 0;
                        try
                        {
                            long subId = paymentList[i].Id;
                            //if (paymentList[i].FromData == 3 || paymentList[i].FromData == 4)
                            //{
                            //    if (Convert.ToInt32(paymentList[i].BillNo) == 0)
                            //        one.AccountHeadPayment = paymentList[i].AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == subId).Select(x => x.SubLedgerName).FirstOrDefault();
                            //    else
                            //        one.AccountHeadPayment = paymentList[i].AccHeadName + " " + school.tb_Fee.Where(x => x.FeeId == subId).Select(x => x.FeesName).FirstOrDefault();
                            //}
                            //{
                            one.AccountHeadPayment = paymentList[i].AccHeadName;
                            //}
                            one.payment = paymentList[i].Payment ?? 0;
                        }
                        catch
                        {
                            one.AccountHeadPayment = "";
                            one.payment = 0;
                        }
                        one.SLNo = 2;
                        list.Add(one);
                        i = i + 1;
                    }
                    //}
                }
                else
                {
                    foreach (var item in paymentList)
                    {
                        ReceiptPaymentNewModel one = new ReceiptPaymentNewModel();
                        //if (item.FromData == 3 || item.FromData == 4)
                        //{
                        //    if (Convert.ToInt32(item.BillNo) == 0)
                        //        one.AccountHeadPayment = item.AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == item.Id).Select(x => x.SubLedgerName).FirstOrDefault();
                        //    else
                        //        one.AccountHeadPayment = item.AccHeadName + " " + _Entities.tb_Fee.Where(x => x.FeeId == item.Id).Select(x => x.FeesName).FirstOrDefault();
                        //}
                        //else
                        //{
                        one.AccountHeadPayment = item.AccHeadName;
                        //}
                        one.payment = item.Payment ?? 0;
                        try
                        {
                            long subId = receiptList[i].Id;
                            //if (receiptList[i].FromData == 3 || receiptList[i].FromData == 4)
                            //{
                            //    if (Convert.ToInt32(receiptList[i].BillNo) == 0)
                            //        one.AccountHeadReceipt = receiptList[i].AccHeadName + " " + _Entities.TbSubLedgerData.Where(x => x.LedgerId == subId).Select(x => x.SubLedgerName).FirstOrDefault();
                            //    else
                            //        one.AccountHeadReceipt = receiptList[i].AccHeadName + " " + _Entities.tb_Fee.Where(x => x.FeeId == subId).Select(x => x.FeesName).FirstOrDefault();
                            //}
                            //{
                            one.AccountHeadReceipt = receiptList[i].AccHeadName;
                            //}
                            one.receipt = receiptList[i].Receipt ?? 0;
                        }
                        catch (Exception ex)
                        {
                            one.AccountHeadReceipt = "";
                            one.receipt = 0;
                        }
                        one.SLNo = 2;
                        list.Add(one);
                        i = i + 1;
                    }
                }
                #endregion Data Model Creating
            }

            //------------------------------Receipt---------------------------
            #region Open Cash 
            ReceiptPaymentNewModel open = new ReceiptPaymentNewModel();
            open.receipt = school.TbBalances.Where(x => x.SchoolId == school.SchoolId && x.BankId == 0 && x.SourceId == 0 && x.CurrentDate.Date < fromdate && x.IsActive).OrderByDescending(x => x.CurrentDate).Select(x => x.Closing).FirstOrDefault();
            open.AccountHeadReceipt = "CASH IN HAND";
            open.payment = 0;
            open.AccountHeadPayment = "";
            open.Id = 0;
            open.FromData = 0;
            open.SLNo = 0;
            list.Add(open);
            #endregion Open Cash
            #region Bank Balance
            var balance = school.TbBalances.Where(x => x.SchoolId == school.SchoolId && x.SourceId == 2 && x.BankId != 0 && x.IsActive && x.CurrentDate.Date < fromdate).OrderByDescending(x => x.CurrentDate).ToList();
            foreach (var x in balance)
            {
                var xxxx = school.TbBanks.Where(y => y.BankId == x.BankId).Select(y => y.BankName).FirstOrDefault().ToUpper();
                if (list.Any(m => m.AccountHeadReceipt == xxxx))
                {

                }
                else
                {
                    ReceiptPaymentNewModel b = new ReceiptPaymentNewModel();
                    b.receipt = x.Closing;
                    b.AccountHeadReceipt = school.TbBanks.Where(y => y.BankId == x.BankId).Select(y => y.BankName).FirstOrDefault().ToUpper();
                    b.payment = 0;
                    b.AccountHeadPayment = "";
                    b.Id = 0;
                    b.FromData = 0;
                    b.SLNo = 1;
                    list.Add(b);
                }
            }
            var baalanceBank = bankList.Where(x => !list.Any(h => h.AccountHeadReceipt == x.BankName.ToUpper())).ToList();
            foreach (var bb in baalanceBank)
            {
                ReceiptPaymentNewModel b = new ReceiptPaymentNewModel();
                b.receipt = 0;
                b.AccountHeadReceipt = bb.BankName.ToUpper();
                b.payment = 0;
                b.AccountHeadPayment = "";
                b.Id = 0;
                b.FromData = 0;
                b.SLNo = 1;
                list.Add(b);
            }
            #endregion Bank Balance
            //---------------------------------------------------
            //------------------------------Payment---------------------------
            #region Closing Cash 
            ReceiptPaymentNewModel close = new ReceiptPaymentNewModel();
            close.payment = school.TbBalances.Where(x => x.SchoolId == school.SchoolId && x.BankId == 0 && x.SourceId == 0 && x.CurrentDate.Date <= toDate && x.IsActive).OrderByDescending(x => x.CurrentDate).Select(x => x.Closing).FirstOrDefault();
            close.AccountHeadPayment = "CASH IN HAND";
            close.receipt = 0;
            close.AccountHeadReceipt = "";
            close.Id = 0;
            close.FromData = 0;
            close.SLNo = 3;
            list.Add(close);
            #endregion close Cash
            #region Bank Balance
            var balanceClose = school.TbBalances.Where(x => x.SchoolId == school.SchoolId && x.SourceId == 2 && x.BankId != 0 && x.IsActive && x.CurrentDate.Date <= toDate).OrderByDescending(x => x.CurrentDate).ToList();
            foreach (var x in balanceClose)
            {
                var xxxxx = school.TbBanks.Where(y => y.BankId == x.BankId).Select(y => y.BankName).FirstOrDefault().ToUpper();
                if (list.Any(m => m.AccountHeadPayment == xxxxx))
                {

                }
                else
                {
                    ReceiptPaymentNewModel b = new ReceiptPaymentNewModel();
                    b.payment = x.Closing;
                    b.AccountHeadPayment = school.TbBanks.Where(y => y.BankId == x.BankId).Select(y => y.BankName).FirstOrDefault().ToUpper();
                    b.receipt = 0;
                    b.AccountHeadReceipt = "";
                    b.Id = 0;
                    b.FromData = 0;
                    b.SLNo = 4;
                    list.Add(b);
                }
            }
            var baalanceBankb = bankList.Where(x => !list.Any(h => h.AccountHeadPayment == x.BankName.ToUpper())).ToList();
            foreach (var bb in baalanceBankb)
            {
                ReceiptPaymentNewModel b = new ReceiptPaymentNewModel();
                b.receipt = 0;
                b.AccountHeadPayment = bb.BankName.ToUpper();
                b.payment = 0;
                b.AccountHeadReceipt = "";
                b.Id = 0;
                b.FromData = 0;
                b.SLNo = 4;
                list.Add(b);
            }
            #endregion Bank Balance

            //---------------------------------------------------
            return list.OrderBy(x => x.SLNo).ToList();
        }
        public decimal CashAmount(DateTime startDate, DateTime endDate, int type)//type=0 = Opening 1 = Closing 
        {
            if (type == 0)
            {
                var data = school.TbBalances.Where(x => x.SourceId == 0 && x.BankId == 0 && x.IsActive == true && x.CurrentDate.Date <= endDate.Date).OrderByDescending(x => x.CurrentDate).FirstOrDefault();
                if (data != null)
                    return data.Closing;
                else
                    return 0;
            }
            else
            {
                var data = school.TbBalances.Where(x => x.SourceId == 0 && x.BankId == 0 && x.IsActive == true && x.CurrentDate.Date >= startDate.Date).OrderByDescending(x => x.CurrentDate).FirstOrDefault();
                if (data != null)
                    return data.Closing;
                else
                    return 0;
            }
        }
        public decimal PaymentClosingBank(DateTime startDate, DateTime endDate, int type)//type=0 = Receipt 1 = Payment
        {
            var data = school.TbBalances.Where(x => x.SourceId == 2 && x.BankId != 0 && x.IsActive == true && x.CurrentDate.Date <= endDate.Date).OrderByDescending(x => x.CurrentDate).FirstOrDefault();
            if (data != null)
                return data.Closing;
            else
                return 0;
        }
        public List<sp_DayBookReport_Result> GetDayBookReport(DateTime fromDate, DateTime toDate, long schoolId) // Not use this Stored Procedure in live 
        {
            var p1 = new SqlParameter("@schoolId", schoolId);
            var p2 = new SqlParameter("@fromDate", fromDate);
            var p3 = new SqlParameter("@toDate", toDate);

            return _Entities.spDayBookReportResults
                .FromSqlRaw("EXEC sp_DayBookReport @schoolId, @fromDate, @toDate", p1, p2, p3)
                .ToList();
        }
        public List<DayBookReportModel> DayBookReportDetailsCollection(DateTime fromDate, DateTime toDate)
        {
            var banks = school.TbBanks.ToList();
            List<DayBookReportModel> data = new List<DayBookReportModel>();
            var balanceDetails = school.TbBalances.Where(x => (x.CurrentDate.Date) <= toDate.Date && (x.CurrentDate.Date) >= fromDate.Date && x.IsActive).OrderBy(x => x.CurrentDate).ToList();
            var bankDetails = school.TbBankEntries.Where(x => x.CancelStatus == false && x.IsActive == true && (x.EnterDate.Date) <= toDate.Date && (x.EnterDate.Date) >= fromDate.Date).ToList();
            var cashDetails = school.TbCashEntries.Where(x => x.CancelStatus == false && x.IsActive == true && (x.EnterDate.Date) <= toDate.Date && (x.EnterDate.Date) >= fromDate.Date).ToList();
            if (balanceDetails != null && balanceDetails.Count > 0)
            {
                var dateList = balanceDetails.GroupBy(x => x.CurrentDate.Date).ToList();
                foreach (var item in dateList)
                {
                    DayBookReportModel one = new DayBookReportModel();
                    one.EnterDate = item.Key;
                    //one.Opening = school.tb_Balance.Where(x => (x.CurrentDate.Date) < item.Key).Sum(x => x.Closing);
                    decimal cc = school.TbBalances.Where(x => x.CurrentDate.Date < item.Key && x.SourceId == 0 && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.Closing).FirstOrDefault();
                    decimal bnk = 0;
                    foreach (var bk in banks)
                    {
                        bnk = bnk + school.TbBalances.Where(x => x.CurrentDate.Date < item.Key && x.SourceId == 2 && x.BankId == bk.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.Closing).FirstOrDefault();
                    }
                    one.Opening = bnk + cc;

                    decimal close = one.Opening;
                    one._list = new List<DayBookReportDetails>();
                    if (cashDetails != null && cashDetails.Count > 0)
                    {
                        var cashList = cashDetails.Where(x => x.EnterDate.Date == item.Key).ToList();
                        foreach (var cash in cashList)
                        {
                            DayBookReportDetails subOne = new DayBookReportDetails();
                            if (cash.BillNo != null && cash.BillNo != "")
                                subOne.VoucherNo = cash.VoucherNumber + " / " + cash.BillNo;
                            else
                                subOne.VoucherNo = cash.VoucherNumber;

                            subOne.TransactionType = cash.TransactionType;
                            subOne.AccountHeadName = cash.Head.AccHeadName;
                            if (cash.BillNo == null || cash.BillNo == string.Empty)
                                subOne.SubLedger = _Entities.TbSubLedgerData.Where(x => x.LedgerId == cash.SubId).Select(x => x.SubLedgerName).FirstOrDefault() ?? "";
                            else
                                subOne.SubLedger = _Entities.TbFees.Where(x => x.FeeId == cash.SubId).Select(x => x.FeesName).FirstOrDefault() ?? "";
                            if (cash.TransactionType == "R")
                            {
                                subOne.IncomeAmount = cash.Amount ?? 0;
                                subOne.ExpenseAmount = 0;
                            }
                            else
                            {
                                subOne.IncomeAmount = 0;
                                subOne.ExpenseAmount = cash.Amount ?? 0;
                            }
                            subOne.Narration = cash.Narration;
                            subOne.FromStatus = "C";
                            one._list.Add(subOne);

                            close = close + subOne.IncomeAmount - subOne.ExpenseAmount;
                        }
                    }
                    if (bankDetails != null && bankDetails.Count > 0)
                    {
                        var bankList = bankDetails.Where(x => x.EnterDate.Date == item.Key).ToList();
                        foreach (var bak in bankList)
                        {
                            DayBookReportDetails subOne = new DayBookReportDetails();
                            if (bak.BillNo != null && bak.BillNo != "")
                                subOne.VoucherNo = bak.VoucherNumber + " / " + bak.BillNo;
                            else
                                subOne.VoucherNo = bak.VoucherNumber;
                            subOne.TransactionType = bak.TransactionType;
                            subOne.AccountHeadName = bak.Head.AccHeadName;
                            if (bak.BillNo == null || bak.BillNo == string.Empty)
                                subOne.SubLedger = _Entities.TbSubLedgerData.Where(x => x.LedgerId == bak.SubId).Select(x => x.SubLedgerName).FirstOrDefault() ?? "";
                            else
                                subOne.SubLedger = _Entities.TbFees.Where(x => x.FeeId == bak.SubId).Select(x => x.FeesName).FirstOrDefault() ?? "";
                            if (bak.TransactionType == "R")
                            {
                                subOne.IncomeAmount = bak.Amount ?? 0;
                                subOne.ExpenseAmount = 0;
                            }
                            else
                            {
                                subOne.IncomeAmount = 0;
                                subOne.ExpenseAmount = bak.Amount ?? 0;
                            }
                            subOne.Narration = bak.Narration;
                            subOne.FromStatus = "B";
                            one._list.Add(subOne);
                            close = close + subOne.IncomeAmount - subOne.ExpenseAmount;
                        }
                    }
                    one.Closing = close;
                    data.Add(one);
                }
            }
            return data.OrderBy(x => x.EnterDate).ToList();
        }

        public List<LedgerReportDataModel> GetLedgerReport(DateTime startDate, DateTime toDate, long schoolId, long headId, long subId)
        {
            List<LedgerReportDataModel> list = new List<LedgerReportDataModel>();
            #region HeadId=0: Means wants to show only the all account head, avoid the subledgers
            if (headId == 0)
            {
                var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date).ToList();
                var bankData = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date).ToList();
                var accountHeadIdCash = cashData.Select(x => x.HeadId).Distinct().ToList();
                var accountHeadIdBank = bankData.Select(x => x.HeadId).Distinct().ToList();

                var accountHead = accountHeadIdCash.Union(accountHeadIdBank).ToList();
                if (accountHead.Count > 0 && accountHead != null)
                {
                    foreach (var item in accountHead)
                    {
                        LedgerReportDataModel one = new LedgerReportDataModel();
                        var headdata = school.TbAccountHeads.Where(x => x.AccountId == item).FirstOrDefault();
                        one.AccountHead = headdata.AccHeadName;
                        one._LedgerDetailsList = new List<LedgerDetailsList>();
                        var cashDate = cashData.Where(x => x.HeadId == item).Select(x => x.EnterDate.Date).Distinct().ToList();
                        var bankDate = bankData.Where(x => x.HeadId == item).Select(x => x.EnterDate.Date).Distinct().ToList();
                        var allDates = cashDate.Union(bankDate).OrderBy(x => x.Date).ToList();
                        if (headdata.ForBill == true)
                        {
                            var datesCancel = school.TbBillCancelAccounts.Where(x => x.IsActive == true && !allDates.Any(y => y.Date == x.CancelDate.Date) && x.CancelDate.Date >= startDate.Date && x.CancelDate.Date <= toDate.Date).Select(x => x.CancelDate.Date).ToList();
                            allDates = allDates.Union(datesCancel).OrderBy(x => x.Date).ToList();
                        }

                        foreach (var date in allDates)
                        {
                            LedgerDetailsList subOne = new LedgerDetailsList();
                            subOne.AccountDate = date;
                            subOne.SubLedger = "";
                            var incomeCash = cashData.Where(x => x.HeadId == item && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                            var incomeBank = bankData.Where(x => x.HeadId == item && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                            var expenseCash = cashData.Where(x => x.HeadId == item && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                            var expenseBank = bankData.Where(x => x.HeadId == item && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                            var income = incomeCash + incomeBank;
                            var expense = expenseCash + expenseBank;
                            if (headdata.ForBill == true)
                            {
                                var cancelExpense = school.TbBillCancelAccounts.Where(x => x.CancelDate.Date == date.Date).Sum(x => x.Amount);
                                expense = cancelExpense;
                            }
                            subOne.Income = income ?? 0;
                            subOne.Expence = expense ?? 0;
                            if (income > expense)
                            {
                                //subOne.Income = (income - expense) ?? 0;
                                //subOne.Expence = 0;
                                subOne.Status = "Cr";
                            }
                            else if (income == expense)
                            {
                                //subOne.Expence = 0;
                                //subOne.Income = 0;
                                subOne.Status = "";
                            }
                            else
                            {
                                //subOne.Expence = (expense - income) ?? 0;
                                //subOne.Income = 0;
                                subOne.Status = "Dt";
                            }
                            one._LedgerDetailsList.Add(subOne);
                        }
                        list.Add(one);
                    }
                }
            }
            #endregion Means wants to show only the all account head, avoid the subledgers
            #region SubId=0 and Head!=0: Means wants to show only the particular head with no sub
            else if (subId == 0)
            {
                var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();
                var bankData = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();
                var accountHeadIdCash = cashData.Select(x => x.HeadId).Distinct().ToList();
                var accountHeadIdBank = bankData.Select(x => x.HeadId).Distinct().ToList();
                var accountHead = accountHeadIdCash.Union(accountHeadIdBank).ToList();
                if (accountHead.Count > 0 && accountHead != null)
                {
                    LedgerReportDataModel one = new LedgerReportDataModel();
                    var headHead = school.TbAccountHeads.Where(x => x.AccountId == headId).FirstOrDefault();
                    one.AccountHead = headHead.AccHeadName;
                    one._LedgerDetailsList = new List<LedgerDetailsList>();
                    var cashDate = cashData.Where(x => x.HeadId == headId).Select(x => x.EnterDate.Date).Distinct().ToList();
                    var bankDate = bankData.Where(x => x.HeadId == headId).Select(x => x.EnterDate.Date).Distinct().ToList();
                    var allDates = cashDate.Union(bankDate).OrderBy(x => x.Date).ToList();
                    if (headHead.ForBill == true)
                    {
                        var datesCancel = school.TbBillCancelAccounts.Where(x => x.IsActive == true && !allDates.Any(y => y.Date == x.CancelDate.Date) && x.CancelDate.Date >= startDate.Date && x.CancelDate.Date <= toDate.Date).Select(x => x.CancelDate.Date).ToList();
                        allDates = allDates.Union(datesCancel).OrderBy(x => x.Date).ToList();
                    }
                    foreach (var date in allDates)
                    {
                        LedgerDetailsList subOne = new LedgerDetailsList();
                        subOne.AccountDate = date;
                        subOne.SubLedger = "";
                        var incomeCash = cashData.Where(x => x.HeadId == headId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                        var incomeBank = bankData.Where(x => x.HeadId == headId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                        var expenseCash = cashData.Where(x => x.HeadId == headId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                        var expenseBank = bankData.Where(x => x.HeadId == headId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                        var income = incomeCash + incomeBank;
                        var expense = expenseCash + expenseBank;
                        if (headHead.ForBill == true)
                        {
                            var cancelExpense = school.TbBillCancelAccounts.Where(x => x.CancelDate.Date == date.Date).Sum(x => x.Amount);
                            expense = cancelExpense;
                        }
                        subOne.Income = income ?? 0;
                        subOne.Expence = expense ?? 0;
                        if (income > expense)
                        {
                            subOne.Status = "Cr";
                        }
                        else if (income == expense)
                        {
                            subOne.Status = "";
                        }
                        else
                        {
                            subOne.Status = "Dt";
                        }
                        one._LedgerDetailsList.Add(subOne);
                    }
                    list.Add(one);
                }
            }
            #endregion SubId=0 and Head!=0: Means wants to show only the particular head with no sub

            //****************** Subledger select the all condition ***********************
            #region Select all subledger data
            else if (subId == 1)//Subledger ALL
            {
                var checkFee = school.TbAccountHeads.Where(x => x.AccountId == headId && x.ForBill == true).FirstOrDefault();
                #region Select all subledger data from the fees
                if (checkFee != null)
                {
                    var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();
                    var bankData = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();
                    LedgerReportDataModel one = new LedgerReportDataModel();
                    one.AccountHead = school.TbAccountHeads.Where(x => x.AccountId == headId).Select(x => x.AccHeadName).FirstOrDefault();
                    one._LedgerDetailsList = new List<LedgerDetailsList>();
                    var cashDate = cashData.Select(x => x.EnterDate.Date).Distinct().OrderBy(x => x.Date).ToList();
                    var bankDate = bankData.Select(x => x.EnterDate.Date).Distinct().OrderBy(x => x.Date).ToList();
                    var allDates = cashDate.Union(bankDate).ToList().OrderBy(x => x.Date).ToList();
                    var cancellCashBillsNotIncludes = school.TbBillCancelAccounts.Where(x => x.IsActive && x.CancelDate.Date >= startDate.Date && x.CancelDate.Date <= toDate.Date && !allDates.Any(y => y.Date == x.CancelDate.Date)).Select(x => x.CancelDate.Date).ToList();
                    allDates = allDates.Union(cancellCashBillsNotIncludes).ToList().OrderBy(x => x.Date).ToList();
                    if (allDates != null && allDates.Count > 0)
                    {
                        foreach (var day in allDates)
                        {
                            var cashSub = cashData.Where(x => x.EnterDate.Date == day.Date).Select(x => x.SubId).Distinct().ToList();
                            var bankSub = bankData.Where(x => x.EnterDate.Date == day.Date).Select(x => x.SubId).Distinct().ToList();
                            var allSub = cashSub.Union(bankSub).OrderBy(x => x).ToList();
                            var cancelSub = school.TbBillCancelAccounts.Where(x => x.IsActive && x.CancelDate.Date >= startDate.Date && x.CancelDate.Date <= toDate.Date && x.CancelDate.Date == day.Date && !allSub.Any(y => y == x.ItemId)).Select(x => x.ItemId).ToList();
                            var sub = allSub.Union(cancelSub).ToList();
                            foreach (var item in sub)
                            {
                                LedgerDetailsList oneSub = new LedgerDetailsList();
                                oneSub.AccountDate = day;
                                oneSub.SubLedger = school.TbFees.Where(x => x.FeeId == item && x.IsActive).Select(x => x.FeesName).FirstOrDefault();
                                if (oneSub.SubLedger != null)
                                {
                                    var cashIncome = cashData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "R").Sum(x => x.Amount);
                                    var bankIncome = bankData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "R").Sum(x => x.Amount);

                                    var cashExpense = cashData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "P").Sum(x => x.Amount);
                                    var bankExpense = bankData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "P").Sum(x => x.Amount);

                                    var cancelExpense = school.TbBillCancelAccounts.Where(x => x.IsActive && x.CancelDate.Date == day.Date && x.ItemId == item).Sum(x => x.Amount);
                                    var income = cashIncome + bankIncome;
                                    var expense = bankExpense + cashExpense + cancelExpense;

                                    oneSub.Income = income ?? 0;
                                    oneSub.Expence = expense ?? 0;
                                    if (income > expense)
                                        oneSub.Status = "Cr";
                                    else if (income == expense)
                                        oneSub.Status = "";
                                    else
                                        oneSub.Status = "Dt";

                                    one._LedgerDetailsList.Add(oneSub);
                                }
                            }
                        }
                        list.Add(one);
                    }
                }
                #endregion Select all subledger data from the fees
                else
                {
                    var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();
                    var bankData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId).ToList();

                    LedgerReportDataModel one = new LedgerReportDataModel();
                    one.AccountHead = school.TbAccountHeads.Where(x => x.AccountId == headId).Select(x => x.AccHeadName).FirstOrDefault();
                    one._LedgerDetailsList = new List<LedgerDetailsList>();
                    var cashDate = cashData.Select(x => x.EnterDate.Date).Distinct().ToList();
                    var bankDate = bankData.Select(x => x.EnterDate.Date).Distinct().ToList();
                    var allDates = cashDate.Union(bankDate).OrderBy(x => x.Date).ToList();
                    if (allDates != null && allDates.Count > 0)
                    {
                        foreach (var day in allDates)
                        {
                            var cashSub = cashData.Where(x => x.EnterDate.Date == day.Date).Select(x => x.SubId).Distinct().ToList();
                            var bankSub = bankData.Where(x => x.EnterDate.Date == day.Date).Select(x => x.SubId).Distinct().ToList();
                            var allSub = cashSub.Union(bankSub).OrderBy(x => x).ToList();
                            foreach (var item in allSub)
                            {
                                LedgerDetailsList oneSub = new LedgerDetailsList();
                                oneSub.AccountDate = day;
                                oneSub.SubLedger = _Entities.TbSubLedgerData.Where(x => x.LedgerId == item && x.IsActive).Select(x => x.SubLedgerName).FirstOrDefault();
                                if (oneSub.SubLedger != null)
                                {
                                    var cashIncome = cashData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "R").Sum(x => x.Amount);
                                    var bankIncome = bankData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "R").Sum(x => x.Amount);
                                    var cashExpense = cashData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "P").Sum(x => x.Amount);
                                    var bankExpense = bankData.Where(x => x.HeadId == headId && x.SubId == item && x.EnterDate.Date == day.Date && x.TransactionType == "P").Sum(x => x.Amount);
                                    var income = cashIncome + bankIncome;
                                    var expense = bankExpense + cashExpense;

                                    oneSub.Income = income ?? 0;
                                    oneSub.Expence = expense ?? 0;
                                    if (income > expense)
                                        oneSub.Status = "Cr";
                                    else if (income == expense)
                                        oneSub.Status = "";
                                    else
                                        oneSub.Status = "Dt";

                                    one._LedgerDetailsList.Add(oneSub);
                                }
                            }
                            list.Add(one);
                        }
                    }

                }
            }
            #endregion Select all subledger data
            //****************** Subledger select the all condition ***********************
            #region SubId!=0 and Head!=0: Means wants to show only the particular head with particular sub
            else
            {
                var checkFee = school.TbAccountHeads.Where(x => x.AccountId == headId && x.ForBill == true).FirstOrDefault();
                #region Means the filter head is From the fees
                if (checkFee != null)
                {
                    var cancelSubLedgerId = _Entities.TbSubLedgerData.Where(x => x.IsActive && x.AccHeadId == checkFee.AccountId).OrderByDescending(x => x.LedgerId).FirstOrDefault();
                    var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId && x.SubId == subId).ToList();
                    var bankData = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId && x.SubId == subId).ToList();
                    var accountHeadIdCash = cashData.Select(x => x.HeadId).Distinct().ToList();
                    var accountHeadIdBank = bankData.Select(x => x.HeadId).Distinct().ToList();
                    var accountHead = accountHeadIdCash.Union(accountHeadIdBank).ToList();
                    if (accountHead.Count > 0 && accountHead != null)
                    {
                        LedgerReportDataModel one = new LedgerReportDataModel();
                        one.AccountHead = school.TbAccountHeads.Where(x => x.AccountId == headId).Select(x => x.AccHeadName).FirstOrDefault();
                        one._LedgerDetailsList = new List<LedgerDetailsList>();
                        var cashDate = cashData.Select(x => x.EnterDate.Date).Distinct().ToList();
                        var bankDate = bankData.Select(x => x.EnterDate.Date).Distinct().ToList();
                        var allDates = cashDate.Union(bankDate).OrderBy(x => x.Date).ToList();
                        if (allDates.Count > 0 && allDates != null)
                        {
                            foreach (var date in allDates)
                            {
                                LedgerDetailsList subOne = new LedgerDetailsList();
                                subOne.AccountDate = date;
                                subOne.SubLedger = _Entities.TbFees.Where(x => x.FeeId == subId).Select(x => x.FeesName).FirstOrDefault();
                                var incomeCash = cashData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                                var incomeBank = bankData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                                var expenseCash = cashData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                                var expenseBank = bankData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                                if (cancelSubLedgerId != null && cancelSubLedgerId.SubLedgerName == "Bill Cancel")
                                {
                                    var cancelCash = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date == date && x.HeadId == headId && x.SubId == cancelSubLedgerId.LedgerId).ToList();
                                    var cancelBank = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date == date && x.HeadId == headId && x.SubId == cancelSubLedgerId.LedgerId).ToList();
                                    if (cancelCash.Count > 0 && cancelCash != null)
                                    {
                                        foreach (var xx in cancelCash)
                                        {
                                            var cancelAmount = school.TbBillCancelAccounts.Where(x => x.CashBankType == false && x.CashBankId == xx.Id && x.ItemId == subId).ToList();
                                            if (cancelAmount != null && cancelAmount.Count > 0)
                                            {
                                                expenseCash = expenseCash + cancelAmount.Sum(x => x.Amount);
                                            }
                                        }
                                    }
                                    if (cancelBank.Count > 0 && cancelBank != null)
                                    {
                                        foreach (var xx in cancelBank)
                                        {
                                            var cancelAmount = school.TbBillCancelAccounts.Where(x => x.CashBankType == true && x.CashBankId == xx.Id && x.ItemId == subId).ToList();
                                            if (cancelAmount != null && cancelAmount.Count > 0)
                                            {
                                                expenseBank = expenseBank + cancelAmount.Sum(x => x.Amount);
                                            }
                                        }
                                    }
                                }
                                var income = incomeCash + incomeBank;
                                var expense = expenseCash + expenseBank;
                                subOne.Income = income ?? 0;
                                subOne.Expence = expense ?? 0;
                                if (income > expense)
                                {
                                    //subOne.Income = (income - expense) ?? 0;
                                    //subOne.Expence = 0;
                                    subOne.Status = "Cr";
                                }
                                else if (income == expense)
                                {
                                    //subOne.Expence = 0;
                                    //subOne.Income = 0;
                                    subOne.Status = "";
                                }
                                else
                                {
                                    //subOne.Expence = (expense - income) ?? 0;
                                    //subOne.Income = 0;
                                    subOne.Status = "Dt";
                                }
                                one._LedgerDetailsList.Add(subOne);
                            }

                            #region Checking that the dates of cancelling while it we not taken
                            var cancellCashBillsNotIncludes = school.TbBillCancelAccounts.Where(x => x.IsActive && x.ItemId == subId && x.CancelDate.Date >= startDate.Date && x.CancelDate.Date <= toDate.Date && !allDates.Any(y => y.Date == x.CancelDate.Date)).ToList();
                            if (cancellCashBillsNotIncludes != null && cancellCashBillsNotIncludes.Count > 0)
                            {
                                var cancelDates = cancellCashBillsNotIncludes.Select(x => x.CancelDate).Distinct().OrderBy(x => x.Date).ToList();
                                foreach (var yy in cancelDates)
                                {
                                    LedgerDetailsList subOne = new LedgerDetailsList();
                                    subOne.AccountDate = yy;
                                    subOne.SubLedger = _Entities.TbFees.Where(x => x.FeeId == subId).Select(x => x.FeesName).FirstOrDefault();
                                    subOne.Income = 0;
                                    subOne.Expence = cancellCashBillsNotIncludes.Where(x => x.CancelDate.Date == yy.Date).Sum(x => x.Amount);
                                    subOne.Status = "Dt";
                                    one._LedgerDetailsList.Add(subOne);
                                }
                            }
                            #endregion Checking that the dates of cancelling while it we not taken
                        }
                        list.Add(one);

                    }
                }
                #endregion Means the filter head is From the fees
                #region Means the filter head is general account section 
                else
                {
                    var cashData = school.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId && x.SubId == subId).ToList();
                    var bankData = school.TbBankEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.EnterDate.Date >= startDate.Date && x.EnterDate.Date <= toDate.Date && x.HeadId == headId && x.SubId == subId).ToList();
                    var accountHeadIdCash = cashData.Select(x => x.HeadId).Distinct().ToList();
                    var accountHeadIdBank = bankData.Select(x => x.HeadId).Distinct().ToList();
                    var accountHead = accountHeadIdCash.Union(accountHeadIdBank).ToList();
                    if (accountHead.Count > 0 && accountHead != null)
                    {
                        LedgerReportDataModel one = new LedgerReportDataModel();
                        one.AccountHead = school.TbCashEntries.Where(x => x.HeadId == headId).Select(x => x.Head.AccHeadName).FirstOrDefault();
                        one._LedgerDetailsList = new List<LedgerDetailsList>();
                        var cashDate = cashData.Select(x => x.EnterDate.Date).Distinct().ToList();
                        var bankDate = bankData.Select(x => x.EnterDate.Date).Distinct().ToList();
                        var allDates = cashDate.Union(bankDate).OrderBy(x => x.Date).ToList();
                        if (allDates.Count > 0 && allDates != null)
                        {
                            foreach (var date in allDates)
                            {
                                LedgerDetailsList subOne = new LedgerDetailsList();
                                subOne.AccountDate = date;
                                subOne.SubLedger = _Entities.TbSubLedgerData.Where(x => x.LedgerId == subId).Select(x => x.SubLedgerName).FirstOrDefault();
                                var incomeCash = cashData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                                var incomeBank = bankData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "R").Sum(x => x.Amount);
                                var expenseCash = cashData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                                var expenseBank = bankData.Where(x => x.HeadId == headId && x.SubId == subId && x.EnterDate.Date == date && x.TransactionType == "P").Sum(x => x.Amount);
                                var income = incomeCash + incomeBank;
                                var expense = expenseCash + expenseBank;
                                subOne.Income = income ?? 0;
                                subOne.Expence = expense ?? 0;
                                if (income > expense)
                                {
                                    //subOne.Income = (income - expense) ?? 0;
                                    //subOne.Expence = 0;
                                    subOne.Status = "Cr";
                                }
                                else if (income == expense)
                                {
                                    //subOne.Expence = 0;
                                    //subOne.Income = 0;
                                    subOne.Status = "";
                                }
                                else
                                {
                                    //subOne.Expence = (expense - income) ?? 0;
                                    //subOne.Income = 0;
                                    subOne.Status = "Dt";
                                }
                                one._LedgerDetailsList.Add(subOne);
                            }
                        }
                        list.Add(one);
                    }
                }
                #endregion Means the filter head is general account section 
            }
            #endregion SubId!=0 and Head!=0: Means wants to show only the particular head with particular sub
            return list.OrderBy(x => x.AccountHead).ToList();
        }

        public List<Class> NotRegionAssignedClass()
        {
            var assignedClassIds = _Entities.SPClassWithRegion
       .FromSqlRaw("EXEC sp_ClassWithRegion {0}", school.SchoolId)
       .AsEnumerable()
       .Select(x => x.ClassId)
       .ToList();
          
            var allClasses = school.TbClasses
                            .Where(x => x.IsActive && x.PublishStatus == true)
                            .ToList()
                            .Select(x => new Class(x))
                            .ToList();
            var result = allClasses
                            .Where(c => !assignedClassIds.Contains(c.ClassId))
                            .ToList();

            return result;
        }
        public List<RegionDataWithClass> GetAllRegionsWithClass()
        {
            List<RegionDataWithClass> list = new List<RegionDataWithClass>();
            int slNo = 0;
            var data = _Entities.SPClassWithRegion
                    .FromSqlRaw("EXEC sp_ClassWithRegion {0}", school.SchoolId)
                    .ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    if (list.Any(x => x.RegionId == item.Region))
                    { }
                    else
                    {
                        slNo = slNo + 1;
                        RegionDataWithClass one = new RegionDataWithClass();
                        one.RegionId = item.Region;
                        one.RegionName = item.RegionName;
                        var classList = data.Where(x => x.Region == item.Region).Select(x => x.Class).ToList();
                        one.ClaassList = string.Join(" , ", classList);
                        one.Slno = slNo;
                        list.Add(one);
                    }
                }
            }
            return list;
        }

        public List<ScholasticArea> GetAllScholasticArea()
        {
            var data = school.TbScholasticAreas.Where(x => x.IsActive).ToList().Select(x => new ScholasticArea(x)).ToList();
            return data;
        }
        public List<Co_ScholasticArea> GetAllCoScholasticArea()
        {
            var data = school.TbCoScholasticAreas.Where(x => x.IsActive).ToList().Select(x => new Co_ScholasticArea(x)).ToList();
            return data;
        }
        public List<DeclaredExams> GetAllDeclaredExamDetails()
        {
            var data = school.TbDeclaredExams.Where(x => x.IsActive && x.Class.IsActive && x.Class.PublishStatus).ToList().Select(x => new DeclaredExams(x)).ToList();
            return data;
        }
        public List<DeclaredExamSubjects> GetAllDeclaredExamSubjectList(long ExamId)
        {
            var data = school.TbDeclaredExams.Where(x => x.ExamId == ExamId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus).FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).ToList().Select(x => new DeclaredExamSubjects(x)).ToList();
            return data;
        }
        //Added by Gayathri A (01/06/2023)--Previous year progresscard
        public List<DeclaredExamSubjects> GetAllDeclaredExamSubjectList_Previous(long ExamId)
        {
            var data = school.TbDeclaredExams.Where(x => x.ExamId == ExamId && x.IsActive && x.Class.IsActive).FirstOrDefault().TbDeclaredExamSubjects.Where(x => x.IsActive).ToList().Select(x => new DeclaredExamSubjects(x)).ToList();
            return data;
        }


        public ExamTimetableModel_V_VIII GetAllDeclaredExamSubjectList_V_VIII(long id)
        {
            long ExamId = Convert.ToInt64(id);
            ExamTimetableModel_V_VIII model = new ExamTimetableModel_V_VIII();
            var data = _Entities.TbDeclaredExams.Where(x => x.Id == ExamId && x.IsActive).FirstOrDefault();
            if (data != null)
            {
                //model.SchoolName = _user.tb_School.SchoolName;
                //model.SchoolAdddress = _user.tb_School.Address;
                //model.SchoolLogo = _user.tb_School.FilePath;
                model.ExamName = data.TbExamBook.ExamName + " - " + data.TbExamTerm.DefaultExam;
                model.ClassName = data.Class.Class;
                model.StartDate = data.StartDate;
                model.EndDate = data.EndDate;
                model.List = new List<TimeTable_V_VIII>();
                var subjectList = data.TbDeclaredExamSubjects.Where(x => x.IsActive).ToList();
                if (subjectList.Count > 0 && subjectList != null)
                {
                    var dates = subjectList.Select(x => x.ExamDate.Date).Distinct().OrderBy(x => x.Date).ToList();
                    foreach (var item in dates)
                    {
                        TimeTable_V_VIII one = new TimeTable_V_VIII();
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
            return model;
        }


        public string ExamName(long ExamId)
        {
            var ExamName = school.TbDeclaredExams.Where(x => x.ExamId == ExamId && x.IsActive && x.Class.IsActive && x.Class.PublishStatus).FirstOrDefault();
            return ExamName.TbExamBook.ExamName + " - " + ExamName.TbExamTerm.DefaultExam + " ( " + ExamName.Class.Class + " ) ";
        }
        public List<Department> GetAllDepartment()
        {
            var data = school.TbDepartments.Where(x => x.IsActive).ToList().Select(x => new Department(x)).OrderByDescending(x => x.DepartmentName).ToList();
            return data;
        }
        public List<Designation> GetAllDesignation()
        {
            var data = school.TbDesignations.Where(x => x.IsActive).ToList().Select(x => new Designation(x)).OrderByDescending(x => x.DesignationName).ToList();
            return data;
        }
        public List<RoleDetails> GetAllRoleDetails()
        {
            var data = school.TbRoleDetails.Where(x => x.IsActive).ToList().Select(x => new RoleDetails(x)).OrderByDescending(x => x.RoleName).ToList();
            return data;
        }
        public List<Staff> Staff
        {
            get
            {
                return
                 _Entities.TbStaffs.Where(x => x.IsActive && x.User.SchoolId == school.SchoolId).ToList().Select(x => new Staff(x)).ToList();
            }
        }
        public List<AcademicPeriods> GetAllAcademicPeriods()
        {
            var data = school.TbAcademicPeriods.Where(x => x.IsActive && x.Class.IsActive == true && x.Class.PublishStatus == true).ToList().Select(x => new AcademicPeriods(x)).ToList();
            return data;
        }
        public List<RemarkClass> GetAllRemarks()
        {
            var data = school.TbRemarks.Where(x => x.IsActive).ToList().Select(z => new RemarkClass(z)).ToList();
            return data;
        }
        public List<V_Assesment> GetAllAssesmentDetails()
        {
            var data = school.TbVAssesments.Where(x => x.IsActive).ToList().Select(x => new V_Assesment(x)).ToList();
            return data;
        }
        public List<Student> Listing_V_ClassStudents(long DivisionId)
        {
            var data = school.TbStudents.Where(x => x.DivisionId == DivisionId && x.IsActive).OrderBy(x=>x.ClasssNumber).ThenBy(x=>x.StundentName).ToList().Select(x => new Student(x)).ToList();
            return data;
        }
        public List<V_TotalScoreList> Get_V_TotalScores(int enumType)
        {
            var data = school.TbVTotalScoreLists.Where(x => x.EnumTypeId == enumType && x.IsActive).ToList().Select(x => new V_TotalScoreList(x)).ToList();
            return data;
        }

    }
}
