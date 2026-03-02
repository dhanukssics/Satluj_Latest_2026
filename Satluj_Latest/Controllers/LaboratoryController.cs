using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Satluj_Latest;
using Satluj_Latest.Data;
using Satluj_Latest.DataLibrary.Repository;
using Satluj_Latest.Models;
using Satluj_Latest.Controllers;


namespace Satluj_Latest.Controllers
{
    public class LaboratoryController : Controller
    {

        private readonly SchoolDbContext _Entities;
        private readonly TbLogin _user;
        private DateTime CurrentTime => DateTime.UtcNow;
        private readonly DropdownData _dropdownData;

        public LaboratoryController(SchoolDbContext Entities,TbLogin user, DropdownData dropdownData)
        {
            _Entities = Entities;
            _user= user;
            _dropdownData = dropdownData;

        }


        // GET: Laboratory/StockUpdate
        public IActionResult StockUpdate()
        {
            var model = new StockUpdateModel
            {
                SchoolId = _user.SchoolId
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult GetAllSupplierName()
        {
            long schoolId = _user.SchoolId;
            var result = new WebsiteService().GetAllSupplierList(schoolId);

            return Json(new { Status = true, Message = "", result });
        }

        [HttpPost]
        public IActionResult SubmitStockUpdate(StockUpdateModel model)
        {
            var stock = new TbStockUpdate
            {
                CategoryId = model.CategoryId,
                Item = model.Item,
                Price = model.Price,
                PurchaseId = model.PurchaseId,
                SupplierName = model.SupplirName,
                StockStatus = (int)model.Status,
                SchoolId = _user.SchoolId,
                UserId = _user.UserId,
                IsActive = true,
                TimeStamp = CurrentTime
            };

            _Entities.TbStockUpdates.Add(stock);
            bool status = _Entities.SaveChanges() > 0;

            return Json(new { Status = status, Message = status ? "success" : "failed" });
        }

        [HttpPost]
        public IActionResult DeleteStock(long id)
        {
            var stock = _Entities.TbStockUpdates.FirstOrDefault(z => z.StockId == id);

            if (stock == null)
                return Json(new { status = false, msg = "Record not found" });

            stock.IsActive = false;

            bool status = _Entities.SaveChanges() > 0;

            return Json(new { status, msg = status ? "Stock deleted successfully" : "Failed" });
        }

        public IActionResult EditStock(long id)
        {
            var data = _Entities.TbStockUpdates
                .FirstOrDefault(x => x.SchoolId == _user.SchoolId && x.StockId == id && x.IsActive);

            if (data == null)
                return PartialView("_pv_Edit_StockUpdate", null);

            var model = new StockUpdateModel
            {
                CategoryId = data.CategoryId,
                Item = data.Item,
                Price = data.Price,
                PurchaseId = data.PurchaseId,
                SupplirName = data.SupplierName,
                Status = (StockStatus)data.StockStatus,
                SchoolId = _user.SchoolId,
                StockId = data.StockId
            };

            return PartialView("~/Views/Laboratory/_pv_Edit_StockUpdate.cshtml", model);
        }

        [HttpPost]
        public IActionResult SubmitEditStockUpdate(StockUpdateModel model)
        {
            var data = _Entities.TbStockUpdates
                .FirstOrDefault(x => x.StockId == model.StockId && x.SchoolId == _user.SchoolId && x.IsActive);

            if (data == null)
                return Json(new { status = false, msg = "No such data!" });

            data.CategoryId = model.CategoryId;
            data.Item = model.Item;
            data.Price = model.Price;
            data.PurchaseId = model.PurchaseId;
            data.SupplierName = model.SupplirName;
            data.StockStatus = (int)model.Status;

            bool status = _Entities.SaveChanges() > 0;

            return Json(new { status, msg = status ? "Successful" : "No changes made" });
        }

        public IActionResult LabInventoryReport()
        {
            var model = new StockUpdateModel
            {
                SchoolId = _user.SchoolId,
                CategoryId = 0,
                LabCategories = _dropdownData.GetLabCategories(_user.SchoolId)
            };

            return View(model);
        }

        public IActionResult DatatableLabInventoryReport(long id)
        {
            var model = new StockUpdateModel
            {
                SchoolId = _user.SchoolId,
                CategoryId = id
            };

            return PartialView("~/Views/Laboratory/_pv_LabInventoryReportList.cshtml", model);
        }

    }
}