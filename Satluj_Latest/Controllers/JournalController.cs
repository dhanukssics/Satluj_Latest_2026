using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.DataLibrary.Repository;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Satluj_Latest.Models.SchoolValue;
using TrialBalanceModel = Satluj_Latest.Models.TrialBalanceModel;



namespace Satluj_Latest.Controllers
{
    public class JournalController : BaseController
    {
        public readonly DropdownData _dropdown;
        public JournalController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
        }

        // GET: Journal
        public ActionResult AccountSettings()
        {
            var model = new AddAccountHeadModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object SubmitAddAccountHead(AddAccountHeadModel model)
        {
            string msg = "Failed";
            bool status = false;
            var old = _Entities.TbAccountHeads.Where(x => x.AccHeadName.ToUpper() == model.AccountHeadName.ToUpper().Trim() && x.SchoolId == _user.SchoolId && x.IsActive).FirstOrDefault();
            if (old == null)
            {
                var head = new TbAccountHead();
                try
                {
                    //basheer on 29/01/2018 touppercase
                    head.AccHeadName = model.AccountHeadName.ToUpper();
                    head.SchoolId = _user.SchoolId;
                    head.IsActive = true;
                    head.TimeStamp = CurrentTime;
                    _Entities.TbAccountHeads.Add(head);
                    status = _Entities.SaveChanges() > 0;
                    msg = "Success";
                    if (status)
                    {
                        if (_Entities.TbSubLedgerData.Any(x => x.AccHeadId == head.AccountId && x.IsActive))
                        {

                        }
                        else
                        {
                            var sub = new TbSubLedgerDatum();
                            sub.SubLedgerName = "--";
                            sub.AccHeadId = head.AccountId;
                            sub.IsActive = true;
                            sub.TimeStamp = CurrentTime;
                            _Entities.TbSubLedgerData.Add(sub);
                            status = _Entities.SaveChanges() > 0;
                            msg = "Success";
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
                return Json(new { status = status, message = status ? "Account Head added successfully !" : "Failed to add Account Head !" } );
            }
            else
                return Json(new { status = false, message = "Already Exists!" });
        }
        public PartialViewResult GetSubLedgerListList()
        {
            var model = new AddAccountHeadModel();
            model.SchoolId = _user.SchoolId;
            var input = _Entities.TbAccountHeads.Where(x => x.IsActive && x.SchoolId == _user.SchoolId).OrderBy(x => x.AccHeadName).ToList();
            ViewBag.store = input.Select(x => new SelectListItem { Text = x.AccHeadName, Value = x.AccountId.ToString() }).ToList();
            return PartialView("~/Views/Journal/_pv_Add_SubLedger.cshtml", model);
        }
        public object SubmitAddSubLedger(AddAccountHeadModel model)
        {
            string msg = "Failed";
            bool status = false;
            var sub = new TbSubLedgerDatum();
            try
            {
                sub.SubLedgerName = model.SubLedger;
                sub.AccHeadId = model.AccountHeadId;
                sub.IsActive = true;
                sub.TimeStamp = CurrentTime;
                _Entities.TbSubLedgerData.Add(sub);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = status ? "Sub Ledger added successfully !" : "Failed to add Sub Ledger !" });
        }
        public PartialViewResult GetAccountHeadDataList()
        {
            var model = new AddAccountHeadModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_AcchountHeadList.cshtml", model);
        }
        public object DeleteSubLedger(string id)
        {
            bool status = false;
            string msg = "False";
            long subId = Convert.ToInt64(id);
            var sub = _Entities.TbSubLedgerData.FirstOrDefault(x => x.LedgerId == subId && x.IsActive);
            if (sub != null)
            {
                //int count = _Entities.TbSubLedgerData.Where(x => x.AccHeadId == sub.AccHeadId && x.IsActive).Count();
                //if(count>0)
                //{
                //    var head = _Entities.TbAccountHeads.Where(x => x.AccountId == sub.AccHeadId && x.IsActive).FirstOrDefault();
                //    if(head!=null)
                //    {
                //        head.IsActive = false;
                //        status = _Entities.SaveChanges() > 0;
                //    }
                //}
                sub.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg });
        }
        //---------------------------------------------------------------Day Book Module Starts----------------------
        public ActionResult DayBook()
        {
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object GetVoucherNo(string id)
        {
            string voucherNumber = "";
            bool status = true;
            string msg = "Failed";
            int typeId = Convert.ToInt32(id);
            var data = _Entities.TbDayBookIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (data != null)
            {
                if (typeId == 0)
                    voucherNumber = Convert.ToString(data.ExpenseId);
                else
                    voucherNumber = Convert.ToString(data.IncomeId);
                msg = "Success";
            }
            else
            {
                var dayBookId = new TbDayBookId ();
                dayBookId.SchoolId = _user.SchoolId;
                dayBookId.IncomeId = 1;
                dayBookId.ExpenseId = 1;
                _Entities.TbDayBookIds.Add(dayBookId);
                status = _Entities.SaveChanges() > 0;
                if (typeId == 0)
                    voucherNumber = Convert.ToString(dayBookId.ExpenseId);
                else
                    voucherNumber = Convert.ToString(dayBookId.IncomeId);
                msg = "Success";
            }
            return Json(new { status = status, result = voucherNumber });
        }
        public object AddDayBook(AddDayBookModel model)
        {
            string msg = "Failed";
            bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            if (_Entities.TbDayBookData.Any(x => x.TypeId == typeId && x.VoucherNo == model.VoucherNo && x.IsActive && x.SchoolId == _user.SchoolId))
            {
                msg = "Please refresh !";
                var check = _user.TbDayBookData.Where(x => x.TypeId == typeId && x.SchoolId == _user.SchoolId && x.IsActive).OrderByDescending(x => x.DayBookId).First();
                var idCheck = _Entities.TbDayBookData.Where(x => x.SchoolId == _user.SchoolId).First();
                if (typeId == 0)
                {
                    if (idCheck.ExpenseId == Convert.ToInt32(check.VoucherNo))
                    {
                        var vou = _Entities.TbDayBookData.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        vou.ExpenseId = vou.ExpenseId + 1;
                        _Entities.SaveChanges();
                        model.VoucherNo = vou.ExpenseId.ToString();
                    }
                }
                else
                {
                    if (idCheck.IncomeId == Convert.ToInt32(check.VoucherNo))
                    {
                        var vou = _Entities.TbDayBookData.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        vou.IncomeId = vou.IncomeId + 1;
                        _Entities.SaveChanges();
                        model.VoucherNo = vou.ExpenseId.ToString();
                    }
                }

            }
            //else
            //{
            try
            {
                var dayBook = new TbDayBookDatum();
                dayBook.TypeId = typeId;
                try
                {
                    if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                    {
                        string[] splitData = model.EntryDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        dayBook.EntryDate = Convert.ToDateTime(date);
                    }
                }
                catch
                {

                }
                //dayBook.EntryDate = model.EntryDate;
                dayBook.VoucherNo = model.VoucherNo;
                dayBook.HeadId = model.HeadId;
                dayBook.SubLedgerId = model.SubLedgerId;
                dayBook.Amount = model.Amount;
                if (model.Narration == null)
                    dayBook.Narration = " ";
                else
                    dayBook.Narration = model.Narration;
                dayBook.SchoolId = _user.SchoolId;
                dayBook.UserId = _user.UserId;
                dayBook.IsActive = true;
                dayBook.TimeStamp = CurrentTime;
                _Entities.TbDayBookData.Add(dayBook);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
                //----- Increase Voucher nummber
                var vou = _Entities.TbDayBookData.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (vou != null)
                {
                    if (dayBook.TypeId == 0)
                        vou.ExpenseId = vou.ExpenseId + 1;
                    else
                        vou.IncomeId = vou.IncomeId + 1;
                    _Entities.SaveChanges();
                }
                //----- Increase Voucher nummber
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            //}
            return Json(new { status = status, msg = status ? "DayBook added successfully !" : "Failed to add DayBook !" });
        }

        public PartialViewResult SearchVoucherNo()
        {
            var model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_SearchVoucherNo.cshtml", model);
        }
        public PartialViewResult EditDayBookView()
        {
            var model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.AccountHeads = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/Journal/_pv_Edit_DayBook.cshtml", model);
        }
        public PartialViewResult SearchVoucher(AddDayBookModel model)
        {
            string msg = "Failed";
            //bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            var data = _Entities.TbDayBookData.Where(x => x.TypeId == typeId && x.SchoolId == _user.SchoolId && x.IsActive && x.VoucherNo == model.SearchVoucherNo).FirstOrDefault();
            model.SchoolId = _user.SchoolId;
            if (data != null)
            {
                model.EntryDateString = data.EntryDate.ToString("dd-MM-yyyy");
                model.EntryDate = data.EntryDate;
                model.HeadId = data.HeadId;
                model.SubLedgerId = data.SubLedgerId;
                model.Amount = data.Amount;
                model.Narration = data.Narration;
                model.VoucherNo = model.SearchVoucherNo;
                ViewBag.Accountheadlist = _dropdown.GetAccountHeads(model.SchoolId);
                if (typeId == 0)
                    model.TypeId = AccountType.Expense;
                else
                    model.TypeId = AccountType.Income;

                model.DayBookId = data.DayBookId;
                return PartialView("~/Views/Journal/_pv_Edit_DayBook.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Journal/_pv_Add_DayBook.cshtml", model);
            }
        }

        public PartialViewResult AddDayBookReaload()
        {
            var model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_Add_DayBook.cshtml", model);
        }

        public object EditDayBook(AddDayBookModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var data = _Entities.TbDayBookData.Where(x => x.DayBookId == model.DayBookId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (data != null)
                {
                    //data.TypeId = Convert.ToInt32(data.TypeId);
                    //data.EntryDate = model.EntryDate;
                    try
                    {
                        if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                        {
                            string[] splitData = model.EntryDateString.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            data.EntryDate = Convert.ToDateTime(date);
                        }
                    }
                    catch
                    {

                    }
                    data.VoucherNo = model.VoucherNo;
                    data.HeadId = model.HeadId;
                    data.SubLedgerId = model.SubLedgerId;
                    data.Amount = model.Amount;
                    if (model.Narration != null)
                        data.Narration = model.Narration;
                    else
                        data.Narration = " ";
                    status = _Entities.SaveChanges() > 0;
                    if (status == true)
                        msg = "Successful";
                    else
                        msg = "No Changes";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = msg });
        }

        public object CheckVoucherNumber(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (text != string.Empty && text != null)
            {
                try
                {
                    string[] data = text.Split('~');
                    var voucherNo = data[0];
                    var typeId = Convert.ToInt32(data[1]);
                    if (_Entities.TbDayBookData.Any(x => x.VoucherNo == voucherNo && x.TypeId == typeId && x.IsActive && x.SchoolId == _user.SchoolId))
                    {

                    }
                    else
                    {
                        Status = true;
                    }
                }
                catch
                {

                }
            }
            return Json(new { Status = Status, Message = Message });
        }

        public PartialViewResult StatusRange()
        {
            var model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_ProfitOrLoss.cshtml", model);
        }
        public PartialViewResult DayBookPrint(AddDayBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            try
            {
                if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                {
                    string[] splitData = model.EntryDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.EntryDate = Convert.ToDateTime(date);
                }
            }
            catch
            {

            }
            var head = new Data.SubLedgerData(model.SubLedgerId);
            model.HeadName = head.AccountHeadName;
            model.SubLedger = head.SubLedgerName;
            int type = Convert.ToInt32(model.TypeId);
            if (type == 0)
                model.TypeData = "Expense";
            else
                model.TypeData = "Income";
            //return PartialView("~/Views/Journal/_pv_DayBookPrint.cshtml", model);
            return PartialView("~/Views/Journal/_pv_Print_DayBook.cshtml", model);
        }

        //---------------------------------------------------------------Cash Book Module Starts----------------------
        public ActionResult CashBook(AddDayBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            return View(model);
        }
        //public object SearchCashBookData(AddDayBookModel model)
        public object SearchCashBookData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            model.startDate = start;
            model.endDate = end;
            return PartialView("~/Views/Journal/_pv_CashBookList.cshtml", model);
        }
        public object SearchCashBookDailyData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            model.startDate = start;
            model.endDate = end;
            return PartialView("~/Views/Journal/_pv_CashBookDailyReport.cshtml", model);
        }

        public object DayBookStatusData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1] + " 11:59:59 PM");
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            model.startDate = start;
            model.endDate = end;
            return PartialView("~/Views/Journal/_pv_CashBookSearchDate.cshtml", model);
        }

        public ActionResult CashBookSummary(AddDayBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            return View(model);
        }
        //public object SearchCashBookData(AddDayBookModel model)
        public PartialViewResult SearchCashBookCashBookSummary(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            model.startDate = start;
            model.endDate = end;
            return PartialView("~/Views/Journal/_pv_CashBookListSummary.cshtml", model);
        }
        public ActionResult CashBookDailyReport(AddDayBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            return View(model);
        }
        public object SearchCashBookDailyReportData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            AddDayBookModel model = new AddDayBookModel();
            model.SchoolId = _user.SchoolId;
            model.startDate = start;
            model.endDate = end;
            return PartialView("~/Views/Journal/_pv_CashBookDailyReport.cshtml", model);
        }
        //---------------------------------------------------------------Ledger Module Starts----------------------

        public ActionResult Ledger(AddDayBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            model.startDate = CurrentTime;
            model.endDate = CurrentTime;
            ViewBag.AccountHeads =_dropdown.GetAccountHeadLists(model.SchoolId);

            return View(model);
        }
        public async Task<object> SearchLedgerData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);

            AddDayBookModel model = new AddDayBookModel
            {
                SchoolId = _user.SchoolId,
                startDate = start,
                endDate = end
            };

            if (splitDate[2] != "0")
            {
                string[] filter = splitDate[2].Split('!');
                model.HeadId = Convert.ToInt64(filter[0]);
                model.FilterTypeId = Convert.ToInt32(filter[1]);
            }

            
            model.LedgerList = await new Satluj_Latest.Data.School(model.SchoolId)
                                        .GetLedgerData(model.startDate, model.endDate, model.HeadId, model.FilterTypeId);

            return PartialView("~/Views/Journal/_pv_LedgerList.cshtml", model);
        }


        //---------------------------------------------------------------Bank Details Module Starts----------------------
        public ActionResult BankDetails(BankDetailsModel model)
        {
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object SubmitAddBankName(BankDetailsModel model)
        {
            string msg = "Failed";
            bool status = false;
            var bank = new TbBank();
            try
            {
                bank.BankName = model.BankName;
                bank.SchoolId = _user.SchoolId;
                bank.IsActive = true;
                bank.TimeStamp = CurrentTime;
                _Entities.TbBanks.Add(bank);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = status ? "Bank Details added successfully !" : "Failed to add Bank Details !" });

        }

        public PartialViewResult GetBankDetailsList()
        {
            var model = new BankDetailsModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_BankDetailsList.cshtml", model);
        }
        public object DeleteBank(string id)
        {
            bool status = false;
            string msg = "False";
            long bankId = Convert.ToInt64(id);
            var bank = _Entities.TbBanks.FirstOrDefault(x => x.BankId == bankId && x.IsActive);
            if (bank != null)
            {
                bank.IsActive = false;
                status = _Entities.SaveChanges() > 0;
            }
            msg = status ? "Deleted" : "Failed";
            return Json(new { status = status, msg = msg } );
        }
        //---------------------------------------------------------------Bank Book Module Starts----------------------
        public ActionResult BankBook(BankBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object GetBankVoucherNo(string id)
        {
            string voucherNumber = "";
            bool status = true;
            string msg = "Failed";
            int typeId = Convert.ToInt32(id);
            var data = _Entities.TbBankBookIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (data != null)
            {
                if (typeId == 0)
                    voucherNumber = Convert.ToString(data.DepositId);
                else
                    voucherNumber = Convert.ToString(data.WithdrawId);
                msg = "Success";
            }
            else
            {
                var dayBookId = new TbBankBookId ();
                dayBookId.SchoolId = _user.SchoolId;
                dayBookId.DepositId = 1;
                dayBookId.WithdrawId = 1;
                _Entities.TbBankBookIds.Add(dayBookId);
                status = _Entities.SaveChanges() > 0;
                if (typeId == 0)
                    voucherNumber = Convert.ToString(dayBookId.DepositId);
                else
                    voucherNumber = Convert.ToString(dayBookId.WithdrawId);
                msg = "Success";
            }
            return Json(new { status = status, result = voucherNumber });
        }
        public PartialViewResult AddBankBookReaload()
        {
            var model = new BankBookModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Accounthead = _dropdown.GetAccountHeads(model.SchoolId);
            ViewBag.Banklist=_dropdown.GetBankLists(model.SchoolId);
            return PartialView("~/Views/Journal/_pv_Add_BankBook.cshtml", model);
        }
        public PartialViewResult SearchVoucherNoForBank()
        {
            var model = new BankBookModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_SearchVoucherNo_Bank.cshtml", model);
        }
        public PartialViewResult SearchVoucherForBank(BankBookModel model)
        {
            string msg = "Failed";
            bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            var data = _Entities.TbBankBookData.Where(x => x.TypeId == typeId && x.SchoolId == _user.SchoolId && x.IsActive && x.VoucherNumber == model.SearchVoucherNo).FirstOrDefault();
            model.SchoolId = _user.SchoolId;
            ViewBag.BankList = _dropdown.GetBankLists(model.SchoolId);
            ViewBag.AccountHeads = _dropdown.GetAccountHeads(model.SchoolId);

            if (data != null)
            {
                model.EntryDateString = data.EntryDate.ToString("dd-MM-yyyy");
                model.EntryDate = data.EntryDate;
                model.HeadId = data.HeadId;
                model.SubLedgerId = data.SubledgerId;
                model.Amount = data.Amount;
                model.Narration = data.Narration;
                model.VoucherNo = model.SearchVoucherNo;
                if (typeId == 0)
                    model.TypeId = BankType.Deposit;
                else
                    model.TypeId = BankType.Withdraw;
                model.BankBookId = data.Id;
                model.BankId = data.BankId;
                model.ChequeNo = data.ChequeNo;
                if (data.ChequeDate != null)
                {
                    model.ChequeDate = data.ChequeDate ?? CurrentTime;
                    model.ChequeDateString = model.ChequeDate.ToString("dd-MM-yyyy");
                }
                return PartialView("~/Views/Journal/_pv_Edit_BankBook.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Journal/_pv_Add_BankDetails.cshtml", model);
            }
        }
        public object CheckVoucherNumberForBank(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (text != string.Empty && text != null)
            {
                try
                {
                    string[] data = text.Split('~');
                    var voucherNo = data[0];
                    var typeId = Convert.ToInt32(data[1]);
                    if (_Entities.TbBankBookData.Any(x => x.VoucherNumber == voucherNo && x.TypeId == typeId && x.IsActive && x.SchoolId == _user.SchoolId))
                    {

                    }
                    else
                    {
                        Status = true;
                    }
                }
                catch
                {

                }
            }
            return Json(new { Status = Status, Message = Message });
        }
        public object AddBankBook(BankBookModel model)
        {
            string msg = "Failed";
            bool status = false;
            var bankBook = new TbBankBookDatum ();
            try
            {
                bankBook.TypeId = Convert.ToInt32(model.TypeId);
                try
                {
                    if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                    {
                        string[] splitData = model.EntryDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        bankBook.EntryDate = Convert.ToDateTime(date);
                    }
                }
                catch
                {

                }
                try
                {
                    if (model.ChequeDateString != string.Empty && model.ChequeDateString != null)
                    {
                        string[] splitData = model.ChequeDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        bankBook.ChequeDate = Convert.ToDateTime(date);
                    }
                }
                catch
                {

                }
                bankBook.VoucherNumber = model.VoucherNo;
                bankBook.HeadId = model.HeadId;
                bankBook.SubledgerId = model.SubLedgerId;
                bankBook.Amount = model.Amount;
                if (model.Narration == null)
                    bankBook.Narration = " ";
                else
                    bankBook.Narration = model.Narration;
                bankBook.SchoolId = _user.SchoolId;
                bankBook.UserId = _user.UserId;
                bankBook.IsActive = true;
                bankBook.TimeStamp = CurrentTime;
                bankBook.ChequeNo = model.ChequeNo;
                bankBook.BankId = model.BankId;
                if (bankBook.TypeId != 0)
                {
                    if (model.iswithdraw != true)
                        bankBook.IsWithdraw = false;
                    else
                        bankBook.IsWithdraw = true;

                }
                _Entities.TbBankBookData.Add(bankBook);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
                var vou = _Entities.TbBankBookIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (vou != null)
                {
                    if (bankBook.TypeId == 0)
                        vou.DepositId = vou.DepositId + 1;
                    else
                        vou.WithdrawId = vou.WithdrawId + 1;
                    _Entities.SaveChanges();
                }

                ///////////////////////////////////
                if (bankBook.TypeId != 0)
                {
                    var xx = _Entities.TbDayBookData.Where(x => x.SchoolId == _user.SchoolId && x.HeadId == model.HeadId && x.SubLedgerId == model.SubLedgerId && x.Amount == model.Amount && x.Narration == model.Narration).FirstOrDefault();
                    if (xx == null)
                    {

                        if (model.iswithdraw != true)
                        {

                            long Vouchr = 1;
                            var vouch1 = _Entities.TbDayBookIdBanks.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                            if (vouch1 != null)
                            {

                                Vouchr = vouch1.ExpenseId;
                            }


                            var dayBook = new TbDayBookDatum();
                            dayBook.TypeId = 0;  //Expense
                            try
                            {
                                //if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                                //{
                                //    string[] splitData = model.EntryDateString.Split('-');
                                //    var dd = splitData[0];
                                //    var mm = splitData[1];
                                //    var yyyy = splitData[2];
                                //    var date = mm + '-' + dd + '-' + yyyy;
                                //    dayBook.EntryDate = Convert.ToDateTime(date);
                                //}
                            }
                            catch
                            {

                            }

                            dayBook.EntryDate = bankBook.EntryDate;
                            dayBook.VoucherNo = "BK" + Vouchr;
                            dayBook.HeadId = model.HeadId;
                            dayBook.SubLedgerId = model.SubLedgerId;
                            dayBook.Amount = model.Amount;
                            if (model.Narration == null)
                                dayBook.Narration = " ";
                            else
                                dayBook.Narration = model.Narration;
                            dayBook.SchoolId = _user.SchoolId;
                            dayBook.UserId = _user.UserId;
                            dayBook.IsActive = true;
                            dayBook.TimeStamp = CurrentTime;
                            dayBook.IsWithdraw = false;
                            _Entities.TbDayBookData.Add(dayBook);
                            status = _Entities.SaveChanges() > 0;
                            msg = "Success";
                            //----- Increase Voucher nummber
                            var vouch = _Entities.TbDayBookIdBanks.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                            if (vouch != null)
                            {

                                vouch.ExpenseId = vouch.ExpenseId + 1;
                                _Entities.SaveChanges();
                            }
                            else
                            {
                                var voucherTable = new TbDayBookIdBank();
                                voucherTable.SchoolId = _user.SchoolId;
                                voucherTable.ExpenseId = 2;
                                voucherTable.IncomeId = 1;
                                _Entities.TbDayBookIdBanks.Add(voucherTable);
                                _Entities.SaveChanges();

                            }
                            //----- Increase Voucher nummber
                        }
                    }
                    ///////////////////////////////////
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = status ? "BankBook added successfully !" : "Failed to add BankBook !" } );
        }
        public object EditBankBook(BankBookModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var data = _Entities.TbBankBookData.Where(x => x.Id == model.BankBookId && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (data != null)
                {
                    try
                    {
                        if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                        {
                            string[] splitData = model.EntryDateString.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            data.EntryDate = Convert.ToDateTime(date);
                        }
                    }
                    catch
                    {

                    }
                    data.VoucherNumber = model.VoucherNo;
                    data.HeadId = model.HeadId;
                    data.SubledgerId = model.SubLedgerId;
                    data.Amount = model.Amount;
                    if (model.Narration != null)
                        data.Narration = model.Narration;
                    else
                        data.Narration = " ";
                    if (model.ChequeNo != null)
                        data.ChequeNo = model.ChequeNo;
                    else
                        data.ChequeNo = " ";
                    data.BankId = model.BankId;
                    try
                    {
                        if (model.ChequeDateString != string.Empty && model.ChequeDateString != null)
                        {
                            string[] splitData = model.ChequeDateString.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            data.ChequeDate = Convert.ToDateTime(date);
                        }
                    }
                    catch
                    {

                    }
                    status = _Entities.SaveChanges() > 0;
                    if (status == true)
                        msg = "Successful";
                    else
                        msg = "No Changes";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = msg });
        }
        public PartialViewResult BankBookPrint(BankBookModel model)
        {
            model.SchoolId = _user.SchoolId;
            try
            {
                if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                {
                    string[] splitData = model.EntryDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.EntryDate = Convert.ToDateTime(date);
                }
            }
            catch
            {

            }
            var head = new Data.SubLedgerData(model.SubLedgerId);
            model.HeadName = head.AccountHeadName;
            model.SubLedger = head.SubLedgerName;
            model.BankName = _Entities.TbBanks.Where(x => x.BankId == model.BankId && x.IsActive).Select(x => x.BankName).FirstOrDefault();
            try
            {
                if (model.ChequeDateString != string.Empty && model.ChequeDateString != null)
                {
                    string[] splitData = model.ChequeDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.ChequeDate = Convert.ToDateTime(date);
                }
            }
            catch
            {

            }
            int type = Convert.ToInt32(model.TypeId);
            if (type == 0)
                model.TypeData = "Deposit";
            else
                model.TypeData = "Withdraw";
            return PartialView("~/Views/Journal/_pv_BankBookPrint.cshtml", model);
        }
        public PartialViewResult CurrentBankBalance(string id)
        {
            var model = new BankBookModel();
            model.SchoolId = _user.SchoolId;
            model.BankId = Convert.ToInt64(id);
            return PartialView("~/Views/Journal/_pv_BankBalance.cshtml", model);
        }
        public object CheckWithdrawAmount(string text)// From Add
        {
            bool Status = false;
            string Message = "Failed";
            try
            {
                string[] data = text.Split('~');
                var bankId = Convert.ToInt64(data[1]);
                decimal amountWithdraw = Convert.ToDecimal(data[0]);
                var amount = new Data.School(_user.SchoolId).GetBankCurrentBalance(bankId);
                if (amountWithdraw <= amount)
                {
                }
                else
                {
                    Status = true;
                    Message = "Dont have this much amount !";
                }
            }
            catch
            {

            }
            return Json(new { Status = Status, Message = Message });
        }
        public object CheckWithdrawAmountEdit(string text)// From Edit
        {
            string[] data = text.Split('~');
            bool Status = false;
            string Message = "Failed";
            try
            {
                decimal amountWithdraw = Convert.ToDecimal(data[0]);
                long bankId = Convert.ToInt64(data[1]);
                var accountbankId = Convert.ToInt64(data[2]);
                var bankdata = _Entities.TbBankBookData.Where(x => x.Id == bankId).FirstOrDefault();
                var amount = new Data.School(_user.SchoolId).GetBankCurrentBalance(accountbankId);
                amount = amount + bankdata.Amount;
                if (amountWithdraw <= amount)
                {
                }
                else
                {
                    Status = true;
                    Message = "Dont have this much amount !";
                }
            }
            catch
            {

            }
            return Json(new { Status = Status, Message = Message });
        }
        //---------------------------------------------------------------Assets / Liabilities Module Starts-----------

        public ActionResult Assets()
        {
            var model = new AssetsLiabilityModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object GetAssestsInvoiceNumber(string id)
        {
            string invoiceNumber = "";
            bool status = true;
            string msg = "Failed";
            int typeId = Convert.ToInt32(id);
            var data = _Entities.TbAssetsLiabilityIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
            if (data != null)
            {
                if (typeId == 0)
                    invoiceNumber = "A" + Convert.ToString(data.AssetsId);
                else
                    invoiceNumber = "L" + Convert.ToString(data.LiabilityId);
                msg = "Success";
            }
            else
            {
                var assetsId = new TbAssetsLiabilityId();
                assetsId.SchoolId = _user.SchoolId;
                assetsId.AssetsId = 1;
                assetsId.LiabilityId = 1;
                _Entities.TbAssetsLiabilityIds.Add(assetsId);
                status = _Entities.SaveChanges() > 0;
                if (typeId == 0)
                    invoiceNumber = "A" + Convert.ToString(assetsId.AssetsId);
                else
                    invoiceNumber = "L" + Convert.ToString(assetsId.LiabilityId);
                msg = "Success";
            }
            return Json(new { status = status, result = invoiceNumber });
        }
        public PartialViewResult SearchVoucherNoForAssets()
        {
            var model = new AssetsLiabilityModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_SearchVoucherNo_Assets.cshtml", model);
        }
        public PartialViewResult AddAssetsReaload()
        {
            var model = new AssetsLiabilityModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Accounthead = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/Journal/_pv_Add_Assests.cshtml", model);
        }

        public PartialViewResult AssestsPrint(AssetsLiabilityModel model)
        {
            model.SchoolId = _user.SchoolId;
            try
            {
                if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                {
                    string[] splitData = model.EntryDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    model.EntryDate = Convert.ToDateTime(date);
                }
            }
            catch
            {

            }
            var head = new Data.AccountHead(model.HeadId);
            model.HeadName = head.AccHeadName;
            int type = Convert.ToInt32(model.TypeId);
            if (type == 0)
                model.TypeData = "Assets";
            else
                model.TypeData = "Liability";
            if (model.AddStatus == true)
                model.AddStatusString = "Yes";
            else
                model.AddStatusString = "No";
            return PartialView("~/Views/Journal/_pv_AssetsPrint.cshtml", model);
        }

        public object CheckInvoiceNumberForAssets(string text)
        {
            bool Status = false;
            string Message = "Failed";
            if (text != string.Empty && text != null)
            {
                try
                {
                    string[] data = text.Split('~');
                    var invoiceNo = data[0];
                    var typeId = Convert.ToInt32(data[1]);
                    if (_Entities.TbAssetsLiabilityData.Any(x => x.InviceNumber == invoiceNo && x.TypeId == typeId && x.IsActive && x.SchoolId == _user.SchoolId))
                    {

                    }
                    else
                    {
                        Status = true;
                    }
                }
                catch
                {

                }
            }
            return Json(new { Status = Status, Message = Message } );
        }

        public PartialViewResult SearchInvoiceForAssets(AssetsLiabilityModel model)
        {
            string msg = "Failed";
            bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            var data = _Entities.TbAssetsLiabilityData.Where(x => x.TypeId == typeId && x.SchoolId == _user.SchoolId && x.IsActive && x.InviceNumber == model.SearchInviceNumber).FirstOrDefault();
            model.SchoolId = _user.SchoolId;
            if (data != null)
            {
                model.EntryDateString = data.EntryDate.ToString("dd-MM-yyyy");
                model.HeadId = data.HeadId;
                model.Amount = data.Amount;
                model.AddStatus = data.AddStatus;
                model.Narration = data.Narration;
                model.InviceNumber = data.InviceNumber;
                if (typeId == 0)
                    model.TypeId = AssetsLiabilityType.Assets;
                else
                    model.TypeId = AssetsLiabilityType.Liability;
                model.Id = data.Id;
                ViewBag.Accounthead = _dropdown.GetAccountHeads(model.SchoolId);
                return PartialView("~/Views/Journal/_pv_Edit_Assets.cshtml", model);
            }
            else
            {
                return PartialView("~/Views/Journal/_pv_Add_Assests.cshtml", model);
            }
        }

        public object AddAssests(AssetsLiabilityModel model)
        {
            string msg = "Failed";
            bool status = false;
            var assets = new TbAssetsLiabilityDatum();
            try
            {
                assets.TypeId = Convert.ToInt32(model.TypeId);
                try
                {
                    if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                    {
                        string[] splitData = model.EntryDateString.Split('-');
                        var dd = splitData[0];
                        var mm = splitData[1];
                        var yyyy = splitData[2];
                        var date = mm + '-' + dd + '-' + yyyy;
                        assets.EntryDate = Convert.ToDateTime(date);
                    }
                }
                catch
                {

                }
                assets.InviceNumber = model.InviceNumber;
                assets.HeadId = model.HeadId;
                assets.Amount = model.Amount;
                if (model.Narration == null)
                    assets.Narration = " ";
                else
                    assets.Narration = model.Narration;
                assets.SchoolId = _user.SchoolId;
                assets.UserId = _user.UserId;
                assets.IsActive = true;
                assets.TimeStamp = CurrentTime;
                assets.AddStatus = model.AddStatus;
                _Entities.TbAssetsLiabilityData.Add(assets);
                status = _Entities.SaveChanges() > 0;
                msg = "Success";
                var invoice = _Entities.TbAssetsLiabilityIds.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (invoice != null)
                {
                    if (assets.TypeId == 0)
                        invoice.AssetsId = invoice.AssetsId + 1;
                    else
                        invoice.LiabilityId = invoice.LiabilityId + 1;
                    _Entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, msg = status ? "Successfully !" : "Failed !" } );
        }

        public object EditAssets(AssetsLiabilityModel model)
        {
            bool status = false;
            string msg = "Failed";
            try
            {
                var data = _Entities.TbAssetsLiabilityData.Where(x => x.Id == model.Id && x.IsActive && x.SchoolId == _user.SchoolId).FirstOrDefault();
                if (data != null)
                {

                    try
                    {
                        if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                        {
                            string[] splitData = model.EntryDateString.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            data.EntryDate = Convert.ToDateTime(date);
                        }
                    }
                    catch
                    {

                    }
                    data.InviceNumber = model.InviceNumber;
                    data.AddStatus = model.AddStatus;
                    data.HeadId = model.HeadId;
                    data.Amount = model.Amount;
                    if (model.Narration != null)
                        data.Narration = model.Narration;
                    else
                        data.Narration = " ";
                    status = _Entities.SaveChanges() > 0;
                    if (status == true)
                        msg = "Successful";
                    else
                        msg = "No Changes";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { status = status, message = msg } );
        }

        //---------------------------------------------------------------Trial Balance--------------------------------

        public ActionResult TrialBalance()
        {
            Models.TrialBalanceModel model = new Models.TrialBalanceModel();
            model.Today = CurrentTime;
            model.StartDate = CurrentTime;
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public async Task<object> SearchTrialBalanceData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);

            TrialBalanceModel model = new TrialBalanceModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = start;
            model.Today = end;

            
            model.TrialBalanceList = await new Satluj_Latest.Data.School(model.SchoolId)
                                        .GetTrialBalanceList(start, end);

            return PartialView("~/Views/Journal/_pv_TrialBalanceList.cshtml", model);
        }

        //---------------------------------------------------------------Balance Sheet--------------------------------
        public ActionResult BalanceSheet()
        {
            Models.TrialBalanceModel model = new Models.TrialBalanceModel();
            model.Today = CurrentTime;
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        //---------------------------------------------------------------Bank Statement--------------------------------
        public ActionResult BankStatement()
        {
            BankStatementModel model = new BankStatementModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = new DateTime(CurrentTime.Year, CurrentTime.Month, 1);
            model.EndDate = model.StartDate.AddMonths(1).AddDays(-1);
            model.BankId = 0;
            //DropdownData dropdown = new DropdownData();
            ViewBag.BankList = _dropdown.GetBankLists(model.SchoolId);

            return View(model);
        }
        public object SearchBankBalanceData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            var checkId = splitDate[2];
            long bankId = 0;
            try
            {
                bankId = Convert.ToInt64(checkId);
            }
            catch
            {
                bankId = 0;
            }

            BankStatementModel model = new BankStatementModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = start;
            model.EndDate = end;
            model.BankId = bankId;
            return PartialView("~/Views/Journal/_pv_BankStatementList.cshtml", model);
        }
        //---------------------------------------------------------------Receipt Payment--------------------------------
        public ActionResult ReceiptPayment()
        {
            ReceiptPaymentModel model = new ReceiptPaymentModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = CurrentTime;
            model.EndDate = CurrentTime;
            return View(model);
        }
        public object SearchReceiptPaymentData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            ReceiptPaymentModel model = new ReceiptPaymentModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = start;
            model.EndDate = end;
            return PartialView("~/Views/Journal/_pv_ReceiptPaymentList.cshtml", model);
        }
        public object SearchReceiptPaymentBankData(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            ReceiptPaymentModel model = new ReceiptPaymentModel();
            model.StartDate = start;
            model.EndDate = end;
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/Journal/_pv_ReceiptPaymentBankList.cshtml", model);
        }
        public object ReceiptPaymentAmount(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            ReceiptPaymentModel model = new ReceiptPaymentModel();
            model.SchoolId = _user.SchoolId;
            model.StartDate = start;
            model.EndDate = end;
            return PartialView("~/Views/Journal/_pv_ReceiptPaymentAmount.cshtml", model);
        }
        public object Checkaccounthead(string id)
        {

            bool status = false;
            string msg = "";
            int accheadcount = _Entities.TbAccountHeads.Where(x => x.AccHeadName == id && x.IsActive == true).Count();
            if (accheadcount > 0)
            {
                status = true;
                msg = "AccountHead already Exists";
            }
            return Json(new { status = status, msg = msg } );
        }
       

    }
}






