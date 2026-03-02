using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class FeeAlertData:BaseReference
    {
        private TbFeeAlertDatum feeAlertData;
        public FeeAlertData(TbFeeAlertDatum obj) { feeAlertData = obj; }
        public FeeAlertData(long id) { feeAlertData = _Entities.TbFeeAlertData.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return feeAlertData.Id; } }
        public long SchoolId { get { return feeAlertData.SchoolId; } }
        public DateTime AlertDate { get { return feeAlertData.AlertDate; } }
        public bool IsActive { get { return feeAlertData.IsActive; } }
        public DateTime? TimeStamp { get { return feeAlertData.TimeStamp; } }
        

       
    }
}
