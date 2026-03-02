using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Utility
{
    class Common
    {

    }
    public class BillFeeDateHistory
    {
        public long studentId { get; set; }
        public long ClassId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

    }
    public class paymentTb
    {
        public long PaymentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public long FeeId { get; set; }
        public long StudentId { get; set; }
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public System.DateTime TimeStamp { get; set; }

    }
    public class billNumberList
    {
        public long BillNo { get; set; }
        public System.DateTime TimeStamp { get; set; }

    }

    public class alvosmsResp
    {
        public int success { get; set; }
        //public string code { get; set; }
        //public string text { get; set; }
        public List<SMSalvoResponData> data { get; set; }
    }
    public class SMSalvoResponData
    {
        public string messageId { get; set; }
        public string number { get; set; }
    }
    public class IncomeExp
    {
        public string head { get; set; }
        public decimal amount { get; set; }
        public int accountType { get; set; }

    }
    public class PaymentBillNoList
    {
        public decimal Amount { get; set; }
        public decimal sumAmt { get; set; }
        public long FeeId { get; set; }
        public Guid FeeGuid { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal Discount { get; set; }
        public long BillNo { get; set; }
        public bool IsPaid { get; set; }
        public Guid PaymentGuid { get; set; }
        public long StudentId { get; set; }
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public bool IsActive { get; set; }

    }
    public class StudentMarkListData
    {
        public string SubjectName { get; set; }
        public string Internal { get; set; }
        public string External { get; set; }
        public string Total { get; set; }
        public long SubjectId { get; set; }
    }
    public class BalanceSheetData
    {
        public string Liabilities { get; set; }
        public string LiabilitiesAmount { get; set; }
        public string Asset { get; set; }
        public string AssetAmount { get; set; }
    }
    public class IncomeAndExpenditureData
    {
        public string Income { get; set; }
        public string IncomeAmount { get; set; }
        public string Expenditure { get; set; }
        public string ExpenditureAmount { get; set; }
    }
    public class DelivaryCheck
    {
        public int success { get; set; }
        public string mobile { get; set; }
        public string dlr { get; set; }
    }
}
