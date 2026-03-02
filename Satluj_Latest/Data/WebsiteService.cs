using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using Satluj_Latest.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class WebsiteService : BaseReference
    {
        public List<Class> getAllClassByFeeId(long schoolId, long feeId)
        {
            if (feeId <= 0)
                return new List<Class>();

            var result =
                from cls in _Entities.TbClasses
                where cls.SchoolId == schoolId && cls.IsActive
                where !_Entities.TbFeeClasses
                        .Any(f => f.Fee.SchoolId == schoolId &&
                                  f.FeeId == feeId &&
                                  f.ClassId == cls.ClassId &&
                                  f.IsActive)
                select new Class(cls);

            return result.ToList();
        }

        public List<FeeClass> getAllFeeClassByFeeId(long schoolId, long feeId)
        {
            return _Entities.TbFeeClasses.Where(z => z.FeeId == feeId && z.IsActive && z.Fee.SchoolId == schoolId).ToList().
                Select(z => new FeeClass(z)).ToList();
        }
        public List<string> gtGetAllFeeHead(long schoolId)
        {
            var school = _Entities.TbFees.Where(z => z.IsActive && z.SchoolId == schoolId).Select(x => x.FeesName).ToList();
            return school;
        }
        public List<string> gtAllIncomeAccountHead(long schoolId)
        {
            return _Entities.TbIncomes.Where(x => x.IsActive && x.SchoolId == schoolId).ToList().Select(z => new Income(z)).ToList().Select(x => x.AccountHead).Distinct().ToList();
        }
        public List<string> gtAllExpenceAccountHead(long schoolId)
        {

            return _Entities.TbExpenses.Where(x => x.IsActive && x.SchoolId== schoolId).ToList().Select(z => new Expence(z)).ToList().Select(x => x.AccountHead).Distinct().ToList();
        }
        public List<string> GetIncomeAccountHeads(string str)
        {
            return _Entities.TbIncomes.Where(x => x.IsActive && (x.AccountHead.StartsWith(str))).ToList().Select(z => new Income(z)).ToList().Select(x => x.AccountHead).Distinct().ToList();
        }
        public List<Income> IncomeList()
        {
            return _Entities.TbIncomes.Where(x => x.IsActive).ToList().Select(z => new Income(z)).ToList();
        }
        public List<Expence> ExpenceList()
        {
            return _Entities.TbExpenses.Where(x => x.IsActive).ToList().Select(z => new Expence(z)).ToList();
        }
        public List<Income> IncomelistOnDate(DateTime cdate, long SchoolId)
        {
            return _Entities.TbIncomes.Where(x => (x.IsActive && x.Date.Year == cdate.Year && x.Date.Month == cdate.Month && x.Date.Day == cdate.Day) && x.SchoolId == SchoolId).ToList().Select(z => new Income(z)).ToList();

        }
        public List<Expence> ExpencelistOnDate(DateTime cdate, long SchoolId)
        {
            return _Entities.TbExpenses.Where(x => (x.IsActive && x.Date.Year == cdate.Year && x.Date.Month == cdate.Month && x.Date.Day == cdate.Day) && x.SchoolId == SchoolId).ToList().Select(z => new Expence(z)).ToList();

        }
        public double BalancebdDate(DateTime cdate, long SchoolId)
        {
            string endDate = cdate.ToString("MM-dd-yyyy");
            DateTime date = Convert.ToDateTime(endDate);
            //var balance = _Entities.Sp_BalanceBDIncoExp(date, SchoolId, null, null).FirstOrDefault();
           var balance= _Entities.Sp_BalanceBDIncoExp
        .FromSqlRaw("EXEC Sp_BalanceBDIncoExp {0}, {1}, {2},{3}",
                    date, SchoolId, null, null)
        .FirstOrDefault();

            return Convert.ToDouble(balance);

        }
        public Tuple<string, string, List<SmsHistory>> GetAllSmsOnDate(DateTime cdate, long schoolId)
        {
            var result = _Entities.TbSmsHistories.Where(x => (x.MessageDate.Value.Year == cdate.Year && x.MessageDate.Value.Month == cdate.Month && x.MessageDate.Value.Day == cdate.Day) && x.ScholId == schoolId).ToList().Select(z => new SmsHistory(z)).ToList();
            var sum = result.Sum(x => x.SmsPerStudent).ToString();
            var totalsum = _Entities.TbSmsHistories.Where(x => x.ScholId == schoolId).ToList().Sum(z => z.SmsSentPerStudent).ToString();
            return new Tuple<string, string, List<SmsHistory>>(sum, totalsum, result);

        }
        public Tuple<string, string, List<SPSmsDataOnDatecs>> GetAllSmsOnTwoDate(string cdate, string edate, long SchoolId)
        {
            // long SchoolId = 19;
            // return _Entities.tb_SmsHistory.ToList().Select(z => new SmsHistory(z)).ToList();

            // var result = _Entities.tb_SmsHistory.Where(x => x.MessageDate.Value.Year == cdate && x.MessageDat1e.Value.Month == cdate.Month && x.MessageDate.Value.Day == cdate.Day).ToList().Select(z => new SmsHistory(z)).ToList();
            var ScId = Convert.ToString(SchoolId);
            //var sdate = Convert.ToDateTime(cdate);
            //var esdate = Convert.ToDateTime(edate);
            //var result = _Entities.SP_GetAllSmsOnTwoDates(cdate, edate, ScId).ToList().Select(x => new SPSmsDataOnDatecs(x)).ToList();
          var result=  _Entities.SP_GetAllSmsOnTwoDates
        .FromSqlRaw("EXEC SP_GetAllSmsOnTwoDates {0}, {1}, {2}",
                    cdate, edate, ScId)
        .ToList().Select(x => new SPSmsDataOnDatecs(x)).ToList(); 

            var sum = result.Sum(x => x.SmsSentPerStudent).ToString();
            var totalsum = _Entities.TbSmsHistories.Where(x => x.ScholId == SchoolId).ToList().Sum(z => z.SmsSentPerStudent).ToString();

            return new Tuple<string, string, List<SPSmsDataOnDatecs>>(sum, totalsum, result);

        }
        public Tuple<string, string, List<SmsHead>> GetAllSmsHeadByDate(string cdate, string edate, long SchoolId)
        {

            var ScId = Convert.ToString(SchoolId);
            var smsHist = _Entities.TbSmsHistories.Where(x => x.ScholId == SchoolId && x.SendStatus == "1" && x.IsActive == true).ToList();
            var totalsum = smsHist.Sum(z => z.SmsSentPerStudent).ToString();

            var smsStaffHist = _Entities.TbStaffSmshistories.Where(x => x.ScholId == SchoolId && x.SendStatus == "1" && x.IsActive == true).ToList();
            var totalsum2 =  smsStaffHist.Sum(z => z.SmsSentPerStudent).ToString();

            DateTime cddate = Convert.ToDateTime(cdate);
            string StartDate = cddate.Date.ToShortDateString() + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);

            DateTime eddate = Convert.ToDateTime(edate);
            string EndDate = eddate.Date.ToShortDateString() + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(EndDate);

            var result = _Entities.TbSmsHeads.Where(z => z.TimeStamp >= minDate && z.TimeStamp <= maxDate && z.SchoolId == SchoolId && z.IsActive).ToList().Select(z => new SmsHead(z)).ToList();
            var xcx = smsHist.Where(z => z.MessageDate >= minDate && z.MessageDate <= maxDate && z.ScholId == SchoolId).ToList();

            var xcxStaff = smsStaffHist.Where(z => z.MessageDate >= minDate && z.MessageDate <= maxDate && z.ScholId == SchoolId).ToList();
            var sum = xcx.Sum(x => x.SmsSentPerStudent).ToString();
            //var sum = xcx.Count();
            var sum2 = xcxStaff.Sum(x => x.SmsSentPerStudent).ToString();
            //var sum2 = xcxStaff.Count();

            var total = Convert.ToString(Convert.ToInt32(totalsum) + Convert.ToInt32(totalsum2));
            //var total = Convert.ToString(smsHist.Count() + smsStaffHist.Count());
            var daySum = Convert.ToString(Convert.ToInt32(sum)+Convert.ToInt32(sum2));
            //return new Tuple<string, string, List<SmsHead>>(sum, totalsum, result);
            return new Tuple<string, string, List<SmsHead>>(daySum, total, result);

        }
        public string GetLatestExpParticular(string AccountHead, long SchoolId)
        {
            var result = _Entities.TbExpenses.Where(x => x.AccountHead == AccountHead && x.SchoolId == SchoolId).OrderByDescending(x => x.Id).Select(x => x.Particular).FirstOrDefault();
            return result;
        }


        public string GetLatestIncParticular(string AccountHead, long SchoolId)
        {
            var result = _Entities.TbIncomes.Where(x => x.AccountHead == AccountHead && x.SchoolId == SchoolId).OrderByDescending(x => x.Id).Select(x => x.Particular).FirstOrDefault();
            return result;
        }
        public List<SmsHistory> GetAllSms(long SchoolId)
        {
            return _Entities.TbSmsHistories.Where(x => x.IsActive == true && x.ScholId == SchoolId).ToList().Select(z => new SmsHistory(z)).ToList();

        }
        public List<Exams> GetFullMarkData(long classId)
        {
            var exam = _Entities.TbExams.Where(z => z.ClassId == classId && z.IsActive).ToList().OrderByDescending(x => x.TimeStamp).Select(z => new Exams(z)).ToList();
            return exam;
        }
        //public List<Exams> GetFullMarkData(long classId, long divisionId)
        //{
        //    var exam = _Entities.tb_Exams.Where(z => z.ClassId == classId && z.DivisionId == divisionId && z.IsActive).ToList().OrderByDescending(x => x.TimeStamp).Select(z => new Exams(z)).ToList();
        //    return exam;
        //}
        public List<StudentMarkListData> GetStudentMarks(long ExamId, long StudentId)
        {
            List<StudentMarkListData> list = new List<StudentMarkListData>();
            var data = _Entities.TbStudentMarks.Where(x => x.StudentId == StudentId && x.ExamId == ExamId).ToList();
            foreach (var item in data)
            {
                StudentMarkListData one = new StudentMarkListData();
                one.SubjectName = item.Subject.Subject;
                one.Internal = Convert.ToString(item.InternalMark + " / " + item.Subject.InternalMarks);
                one.External = Convert.ToString(item.ExternalMark + " / " + item.Subject.ExternalMark);
                one.Total = Convert.ToString(item.Mark + " / " + item.Subject.Mark);
                one.SubjectId = item.Subject.SubId;
                list.Add(one);
            }
            return list;
        }
        public List<Sp_BusTripHistoryHead_Result> GetTripByDate(DateTime startDate, long schoolId)
        {
            string StartDate = startDate.Date.ToString("yyyy-MM-dd") + ' ' + "12:00:00 AM";
            DateTime minDate = Convert.ToDateTime(StartDate);

            string EndDate = startDate.Date.ToString("yyyy-MM-dd") + ' ' + "11:59:00 PM";
            DateTime maxDate = Convert.ToDateTime(EndDate);

            return _Entities.Sp_BusTripHistoryHead_Result
    .FromSqlInterpolated(
        $"EXEC Sp_BusTripHistoryHead {schoolId}, {minDate}, {maxDate}"
    )
    .AsEnumerable()
    .OrderBy(z => z.BusName)
    .ToList();

            //_Entities.Sp_BusTripHistoryHead(schoolId, minDate, maxDate).ToList().OrderBy(z => z.BusName).ToList();
        }
        public List<string>GetAllSupplierList(long schoolId)
        {
            return _Entities.TbStockUpdates.Where(x => x.IsActive && x.SchoolId == schoolId).ToList().Select(z => new StockUpdate(z)).ToList().Select(x => x.SupplierName).Distinct().ToList();
        }
        public List<string> gtGetAllRemarks(long schoolId)
        {
            var school = _Entities.TbRemarks.Where(z => z.IsActive && z.SchoolId == schoolId).Select(x => x.Remark).ToList();
            return school;
        }
    }
}
