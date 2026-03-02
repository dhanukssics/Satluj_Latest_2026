using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPReceiptPayment : BaseReference
    {
        public SPReceiptPayment()
        {
            
        }
        private sp_ReceiptPayment_Result rp;
        public SPReceiptPayment(sp_ReceiptPayment_Result obj) { rp = obj; }
        public string AccHeadName { get { return rp.AccHeadName; } }
        public int BillNo { get { return rp.BillNo??0; } }
        public int FromData { get { return rp.FromData; } }
        public long? Id { get { return rp.Id; } }
        public decimal? Receipt_ { get { return rp.Receipt; } }
        public decimal? Payment_ { get { return rp.Payment; } }
    }

}
