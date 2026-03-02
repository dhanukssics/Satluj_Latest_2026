using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class StockUpdate : BaseReference
    {
        private TbStockUpdate stock;
        public StockUpdate(TbStockUpdate obj) { stock = obj; }
        public StockUpdate(long Id) { stock = _Entities.TbStockUpdates.FirstOrDefault(z => z.CategoryId == Id); }

        

        public long StockId { get { return stock.StockId; } }
        public long CategoryId { get { return stock.CategoryId; } }
        public string Item { get { return stock.Item; } }
        public decimal Price { get { return stock.Price; } }
        public string PurchaseId { get { return stock.PurchaseId; } }
        public string SupplierName { get { return stock.SupplierName; } }
        public int StockStatus { get { return stock.StockStatus; } }
        public long SchoolId { get { return stock.SchoolId; } }
        public long UserId { get { return stock.UserId; } }
        public bool IsActive { get { return stock.IsActive; } }
        public System.DateTime TimeStamp { get { return stock.TimeStamp; } }
        public string LabName { get { return stock.Category.LaboratoryName; } }
        public string Status { get { return Getsataus(); } }

        public TbStockUpdate X { get; }

        public string Getsataus()
        {
            string status = "";
            if (stock.StockStatus == 0)
                status = "Active";
            else if(stock.StockStatus==1)
                status = "InActive";
            else if (stock.StockStatus == 2)
                status = "Broken";
            return status;
        }
    }
}
