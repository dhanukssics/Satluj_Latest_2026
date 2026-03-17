using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Satluj_Latest;
using Satluj_Latest.Controllers;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Satluj_Latest.Models.SchoolValue;

namespace Satluj_Latest.Controllers
{
    public class SchoolAccountController : BaseController
    {
        private readonly SchoolDbContext _Entites;
        private readonly DropdownData _dropdown;

        public SchoolAccountController(SchoolRepository schoolRepository, ParentRepository parentRepository, TeacherRepository teacherRepository, SchoolDbContext Entities,DropdownData dropdown) : base(schoolRepository, parentRepository, teacherRepository, Entities)
        {
            _dropdown = dropdown;
            _Entites=Entities;
        }
       

        // GET: SchoolAccount
        public ActionResult CashHome()
        {
            CashEntryModel model = new CashEntryModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object GetVoucherNo(string id)
        {
            string voucherNumber = "";
            bool status = true;
            string msg = "Failed";
            int typeId = Convert.ToInt32(id);
            var data = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
            if (data != null)
            {
                if (typeId == 0)
                    voucherNumber = Convert.ToString(data.PaymentVoucher);
                else
                    voucherNumber = Convert.ToString(data.ReceiptVoucher);
                msg = "Success";
            }
            else
            {
                var cashData = new TbVoucherNumber();
                cashData.SchoolId = _user.SchoolId;
                cashData.ReceiptVoucher = 1;
                cashData.PaymentVoucher = 1;
                cashData.ContraVoucher = 1;
                cashData.IsActive = true;
                cashData.TimeStamp = CurrentTime;
                _Entities.TbVoucherNumbers.Add(cashData);
                status = _Entities.SaveChanges() > 0;
                if (typeId == 0)
                    voucherNumber = Convert.ToString(cashData.PaymentVoucher);
                else
                    voucherNumber = Convert.ToString(cashData.ReceiptVoucher);
                msg = "Success";
            }
            return Json(new { status = status, result = voucherNumber });
        }
        public object AddCashEntry(CashEntryModel model)
        {
            string msg = "Failed";
            bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            bool possible = true;
            if (typeId == 0)
            {
                model.VoucherType = "PV";
                model.TransactionType = "P";
                model.ReverseStatus = false;
            }
            else if (typeId == 2) //Reverse
            {
                model.VoucherType = "RV";
                model.TransactionType = "R";
                model.ReverseStatus = true;
            }
            else
            {
                model.VoucherType = "RV";
                model.TransactionType = "R";
                model.ReverseStatus = false;
            }
            #region Balance Checking
            if (typeId == 0)
            {
                DateTime currentDate = CurrentTime;
                if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                {
                    string[] splitData = model.EntryDateString.Split('-');
                    var dd = splitData[0];
                    var mm = splitData[1];
                    var yyyy = splitData[2];
                    var date = mm + '-' + dd + '-' + yyyy;
                    currentDate = Convert.ToDateTime(date);
                }
                var balanceAmount = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == 0 && x.CurrentDate.Date <= currentDate.Date).OrderByDescending(x => x.CurrentDate).FirstOrDefault();
                if (balanceAmount != null)
                {
                    if (balanceAmount.Closing >= model.Amount)
                    {
                        possible = true;
                    }
                    else
                    {
                        possible = false;
                    }
                }
                else
                {
                    possible = false;
                }

            }
            #endregion Balance Checking

            if (possible == true)
            {
                #region Adding
                if (_Entities.TbCashEntries.Any(x => x.VoucherNumber == model.VoucherNo && x.CancelStatus == false && x.IsActive == true && x.TransactionType == model.TransactionType && x.VoucherType == model.VoucherType && x.SchoolId == _user.SchoolId))
                {
                    #region Checking the voucher number
                    msg = "Please refresh the Data";
                    if (model.TransactionType == "P")
                    {
                        var old = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        old.PaymentVoucher = old.PaymentVoucher + 1;
                        _Entities.SaveChanges();
                    }
                    else
                    {
                        var old = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        old.ReceiptVoucher = old.ReceiptVoucher + 1;
                        _Entities.SaveChanges();
                    }
                    #endregion Checking the voucher number
                }
                else
                {
                    try
                    {
                        #region Add Data
                        var cash = new TbCashEntry();
                        cash.VoucherNumber = model.VoucherNo;
                        cash.VoucherType = model.VoucherType;
                        cash.BillNo = "";
                        cash.TransactionType = model.TransactionType;
                        cash.Amount = model.Amount;
                        cash.HeadId = model.HeadId;
                        cash.SubId = model.SubLedgerId;
                        cash.Narration = model.Narration == null ? " " : model.Narration;
                        try
                        {
                            if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                            {
                                string[] splitData = model.EntryDateString.Split('-');
                                var dd = splitData[0];
                                var mm = splitData[1];
                                var yyyy = splitData[2];
                                var date = mm + '-' + dd + '-' + yyyy;
                                cash.EnterDate = Convert.ToDateTime(date);
                            }
                        }
                        catch
                        {
                        }
                        if (cash.EnterDate.Date == CurrentTime.Date)
                            cash.EditStatus = "N";
                        else if (cash.EnterDate.Date < CurrentTime.Date)
                            cash.EditStatus = "P";
                        else
                            cash.EditStatus = "F";
                        cash.UserId = _user.UserId;
                        cash.DataFromStatus = Convert.ToBoolean(DataFromStatus.Cash);
                        cash.CancelStatus = false;
                        cash.SchoolId = _user.SchoolId;
                        cash.Migration = false;
                        cash.IsActive = true;
                        cash.TimeStamp = CurrentTime;
                        cash.ReverseStatus = model.ReverseStatus;
                        if (typeId == 0)
                        {
                            cash.AdvanceStatus = model.AdvanceStatus;
                        }
                        else
                        {
                            cash.AdvanceStatus = false;
                        }
                        _Entities.TbCashEntries.Add(cash);
                        status = _Entities.SaveChanges() > 0;
                        msg = "Success";
                        #endregion Add Data
                        if (status == true)// Increase the voucher number 
                        {
                            var vou = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                            if (typeId == 0)
                                vou.PaymentVoucher = vou.PaymentVoucher + 1;
                            else
                                vou.ReceiptVoucher = vou.ReceiptVoucher + 1;
                            _Entities.SaveChanges();
                            #region Balance
                            int sourceId = Convert.ToInt32(DataFromStatus.Cash);
                            if (typeId == 0) // Payment , then the amount reduced from the opening balance 
                            {
                                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == cash.EnterDate.Date && x.SourceId == sourceId).FirstOrDefault();
                                if (balance != null)
                                {
                                    balance.Closing = balance.Closing - cash.Amount ?? 0;
                                    balance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = cash.EnterDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cash.EnterDate.Date && x.SchoolId == _user.SchoolId && x.SourceId == sourceId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening - cash.Amount ?? 0;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = 0;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                AllUpdatesInBalanceCash(typeId, cash.EnterDate, model.Amount);
                            }
                            else
                            {
                                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == cash.EnterDate.Date && x.SourceId == sourceId).FirstOrDefault();
                                if (balance != null)
                                {
                                    balance.Closing = balance.Closing + cash.Amount ?? 0;
                                    balance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var balanceEntry = new TbBalance();
                                    balanceEntry.SchoolId = _user.SchoolId;
                                    balanceEntry.CurrentDate = cash.EnterDate;
                                    balanceEntry.SourceId = sourceId;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cash.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        balanceEntry.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        balanceEntry.Opening = 0;
                                    balanceEntry.Closing = balanceEntry.Opening + cash.Amount ?? 0;
                                    balanceEntry.IsActive = true;
                                    balanceEntry.BankId = 0;
                                    balanceEntry.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(balanceEntry);
                                    _Entities.SaveChanges();
                                }
                                typeId = 1;  // Becouse the reverse data is actually is like an income , so we wants to update the Income amounts . We we put the type reverse to Income 
                                AllUpdatesInBalanceCash(typeId, cash.EnterDate, model.Amount);
                            }
                            #endregion Balance
                        }

                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                    if (status == true)
                        msg = "Succeddfully Added !";
                    else
                        msg = "Failed to add Cash ENtry !";
                }
                #endregion Adding
            }
            else
            {
                msg = "Insufficient amount in Hand";
            }

            return Json(new { status = status, msg = msg });
        }
        private void AllUpdatesInBalanceCash(int typeId, DateTime enterDate, decimal amount)
        {
            int sourceId = Convert.ToInt32(DataFromStatus.Cash);
            if (typeId == 1)// Receipt ,here also we considering the Reverse income 
            {
                var cashData = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == 0 && x.SourceId == sourceId && x.CurrentDate.Date > enterDate.Date).OrderByDescending(x => x.CurrentDate).ToList();
                if (cashData != null && cashData.Count > 0)
                {
                    foreach (var item in cashData)
                    {
                        item.Opening = item.Opening + amount;
                        item.Closing = item.Closing + amount;
                        _Entities.SaveChanges();
                    }
                }
            }
            else //Payment
            {
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == 0 && x.SourceId == sourceId && x.CurrentDate.Date > enterDate.Date).OrderByDescending(x => x.CurrentDate).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        item.Opening = item.Opening - amount;
                        item.Closing = item.Closing - amount;
                        _Entities.SaveChanges();
                    }
                }
            }
        }
        public PartialViewResult AddDayEntryReaload()
        {
            var model = new CashEntryModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Accounthead = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/SchoolAccount/_pv_AddCashEntryData.cshtml", model);
        }
        public ActionResult BankHome()
        {
            BankEntryModel model = new BankEntryModel();
            model.SchoolId = _user.SchoolId;
            return View(model);
        }
        public object GetVoucherNoFromBank(string id)
        {
            string[] splitData = id.Split('~');
            int typeId = Convert.ToInt32(splitData[0]); // 0: Deposit , 1: Withdraw
            int cashStatus = Convert.ToInt32(splitData[1]);//0: Non Cash ,1: Cash
            string voucherNumber = "";
            bool status = true;
            string msg = "Failed";
            var data = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
            if (data != null)
            {
                if (typeId == 0 && cashStatus == 0)
                    voucherNumber = Convert.ToString(data.ReceiptVoucher);
                else if (typeId == 1 && cashStatus == 0)
                    voucherNumber = Convert.ToString(data.PaymentVoucher);
                else if (typeId == 1 && cashStatus == 1)
                    voucherNumber = Convert.ToString(data.ContraVoucher);
                else if (typeId == 0 && cashStatus == 1)
                    voucherNumber = Convert.ToString(data.ContraVoucher);

                msg = "Success";
            }
            else
            {
                var cashData = new TbVoucherNumber();
                cashData.SchoolId = _user.SchoolId;
                cashData.ReceiptVoucher = 1;
                cashData.PaymentVoucher = 1;
                cashData.ContraVoucher = 1;
                cashData.IsActive = true;
                cashData.TimeStamp = CurrentTime;
                _Entities.TbVoucherNumbers.Add(cashData);
                status = _Entities.SaveChanges() > 0;

                if (typeId == 0 && cashStatus == 0)
                    voucherNumber = Convert.ToString(cashData.ReceiptVoucher);
                else if (typeId == 1 && cashStatus == 0)
                    voucherNumber = Convert.ToString(cashData.PaymentVoucher);
                else if (typeId == 1 && cashStatus == 1)
                    voucherNumber = Convert.ToString(cashData.ContraVoucher);
                else if (typeId == 0 && cashStatus == 1)
                    voucherNumber = Convert.ToString(cashData.ContraVoucher);
                msg = "Success";
            }
            return Json(new { status = status, result = voucherNumber });
        }
        public object AddBankEntry(BankEntryModel model)
        {
            // PaymentType 1: Cash , 2: Cheque  , 3: Bank
            string msg = "Failed";
            bool status = false;
            int typeId = Convert.ToInt32(model.TypeId);
            bool cash = Convert.ToBoolean(model.CashTransaction);
            DateTime currentDate = CurrentTime;
            bool haveBalance = true;
            #region  Balance Checking
            if (model.EntryDateString != string.Empty && model.EntryDateString != null)
            {
                string[] splitData = model.EntryDateString.Split('-');
                var dd = splitData[0];
                var mm = splitData[1];
                var yyyy = splitData[2];
                var date = mm + '-' + dd + '-' + yyyy;
                currentDate = Convert.ToDateTime(date);
            }
            if (typeId == 1)
            {
                var balancee = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date <= currentDate.Date && x.SourceId == 2 && x.BankId == model.BankId).OrderByDescending(x => x.CurrentDate).FirstOrDefault();
                if (balancee != null)
                {
                    if (model.Amount <= balancee.Closing)
                    {
                        haveBalance = true;
                    }
                    else
                    {
                        haveBalance = false;
                    }
                }
                else
                {
                    haveBalance = false;
                }
            }
            #endregion  Balance Checking
            if (haveBalance == true)
            {
                #region voucher checking
                if (typeId == 0)
                {
                    if (cash == true)
                        model.VoucherType = "CV";
                    else
                        model.VoucherType = "RV";
                    model.TransactionType = "R";
                }
                else
                {
                    if (cash == true)
                        model.VoucherType = "CV";
                    else
                        model.VoucherType = "PV";
                    model.TransactionType = "P";
                }
                #endregion voucher checking
                if (_Entities.TbBankEntries.Any(x => x.VoucherNumber == model.VoucherNo && x.CancelStatus == false && x.IsActive == true && x.TransactionType == model.TransactionType && x.VoucherType == model.VoucherType && x.SchoolId == _user.SchoolId))
                {
                    #region Checking the voucher number
                    msg = "Please refresh the Data";
                    if (model.TransactionType == "P")
                    {
                        var old = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        if (model.VoucherType == "CV")
                            old.ContraVoucher = old.ContraVoucher + 1;
                        else
                            old.PaymentVoucher = old.PaymentVoucher + 1;
                        _Entities.SaveChanges();
                    }
                    else
                    {
                        var old = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId).FirstOrDefault();
                        if (model.VoucherType == "CV")
                            old.ContraVoucher = old.ContraVoucher + 1;
                        else
                            old.PaymentVoucher = old.ReceiptVoucher + 1;
                        _Entities.SaveChanges();
                    }
                    #endregion Checking the voucher number
                }
                else
                {
                    #region Add Data
                    var bank = new TbBankEntry();
                    bank.VoucherNumber = model.VoucherNo;
                    bank.VoucherType = model.VoucherType;
                    bank.BillNo = "";
                    bank.TransactionType = model.TransactionType;
                    bank.Amount = model.Amount;
                    #region Mode
                    if (cash == true)
                        bank.ModeType = 1;
                    else
                    {
                        //if (model.ChequeNo != string.Empty && model.ChequeNo != null)
                        //    bank.ModeType = 2;
                        //else
                        //    bank.ModeType = 3;
                        if (model.PaymentModeId == 0)
                        {
                            bank.ModeType = 2;
                        }
                        else
                        {
                            bank.ModeType = 3;
                        }
                    }
                    #endregion Mode
                    #region Check date
                    try
                    {
                        if (bank.ModeType == 2)
                        {
                            if (model.ChequeDateString != string.Empty && model.ChequeDateString != null)
                            {
                                string[] splitData = model.ChequeDateString.Split('-');
                                var dd = splitData[0];
                                var mm = splitData[1];
                                var yyyy = splitData[2];
                                var date = mm + '-' + dd + '-' + yyyy;
                                bank.ChequeDate = Convert.ToDateTime(date);
                            }
                        }
                    }
                    catch
                    {
                    }
                    #endregion Check date
                    if (model.ChequeNo != null)
                        bank.ChequeNumber = model.ChequeNo;
                    bank.HeadId = model.HeadId;
                    bank.SubId = model.SubLedgerId;
                    bank.BankId = model.BankId;
                    if (model.Narration == null || model.Narration == string.Empty)
                        bank.Narration = " ";
                    else
                        bank.Narration = model.Narration;
                    #region Entry Date
                    try
                    {
                        if (model.EntryDateString != string.Empty && model.EntryDateString != null)
                        {
                            string[] splitData = model.EntryDateString.Split('-');
                            var dd = splitData[0];
                            var mm = splitData[1];
                            var yyyy = splitData[2];
                            var date = mm + '-' + dd + '-' + yyyy;
                            bank.EnterDate = Convert.ToDateTime(date);
                        }
                    }
                    catch
                    {
                    }
                    #endregion Entry Date
                    if (bank.EnterDate.Date == CurrentTime.Date)
                        bank.EditStatus = "N";
                    else if (bank.EnterDate.Date < CurrentTime.Date)
                        bank.EditStatus = "P";
                    else
                        bank.EditStatus = "F";
                    bank.UserId = _user.UserId;
                    bank.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bank);
                    bank.CancelStatus = false;
                    bank.SchoolId = _user.SchoolId;
                    bank.Migration = false;
                    bank.IsActive = true;
                    bank.TimeStamp = CurrentTime;
                    _Entities.TbBankEntries.Add(bank);
                    try
                    {
                        status = _Entities.SaveChanges() > 0;
                    }
                    catch (Exception ex)
                    {
                        msg = "Failed";
                    }
                    msg = "Success";

                    if (cash == true)
                    {
                        var cashEntry = new TbCashEntry();
                        cashEntry.VoucherNumber = model.VoucherNo;
                        cashEntry.VoucherType = model.VoucherType;
                        cashEntry.BillNo = "";
                        if (model.TransactionType == "R")
                            cashEntry.TransactionType = "P";
                        else
                            cashEntry.TransactionType = "R";
                        cashEntry.Amount = model.Amount;
                        cashEntry.HeadId = model.HeadId;
                        cashEntry.SubId = model.SubLedgerId;
                        if (model.Narration == null)
                            cashEntry.Narration = "";
                        else
                            cashEntry.Narration = model.Narration;
                        cashEntry.EnterDate = bank.EnterDate;
                        cashEntry.UserId = _user.UserId;
                        cashEntry.DataFromStatus = Convert.ToBoolean(DataFromStatus.Bank);
                        cashEntry.CancelStatus = false;
                        cashEntry.SchoolId = _user.SchoolId;
                        cashEntry.Migration = false;
                        cashEntry.IsActive = true;
                        cashEntry.TimeStamp = CurrentTime;
                        if (cashEntry.EnterDate.Date == CurrentTime.Date)
                            cashEntry.EditStatus = "N";
                        else if (cashEntry.EnterDate.Date < CurrentTime.Date)
                            cashEntry.EditStatus = "P";
                        else
                            cashEntry.EditStatus = "F";
                        cashEntry.ReverseStatus = false;
                        cashEntry.AdvanceStatus = false;
                        _Entities.TbCashEntries.Add(cashEntry);
                        status = _Entities.SaveChanges() > 0;
                        msg = "Success";
                    }
                    #endregion Add Data
                    #region Increase the voucher number 
                    if (status == true)
                    {
                        var vou = _Entities.TbVoucherNumbers.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true).FirstOrDefault();
                        if (cash == true)
                        {
                            vou.ContraVoucher = vou.ContraVoucher + 1;
                        }
                        else
                        {
                            if (typeId == 0)
                                vou.ReceiptVoucher = vou.ReceiptVoucher + 1;
                            else
                                vou.PaymentVoucher = vou.PaymentVoucher + 1;
                        }
                        _Entities.SaveChanges();

                        #region Balance
                        int sourceBankId = Convert.ToInt32(DataFromStatus.Bank);
                        int sourceCashId = Convert.ToInt32(DataFromStatus.Cash);
                        var bankBalance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == bank.EnterDate.Date && x.SourceId == sourceBankId && x.BankId == bank.BankId).FirstOrDefault();
                        var cashBalance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == bank.EnterDate.Date && x.SourceId == sourceCashId && x.BankId == 0).FirstOrDefault();
                        #region Cash Handiling
                        if (typeId == 1)//Withdraw
                        {
                            if (bankBalance != null)
                            {
                                bankBalance.Closing = bankBalance.Closing - model.Amount;
                                bankBalance.TimeStamp = CurrentTime;
                                _Entities.SaveChanges();
                            }
                            else
                            {
                                var bankInsert = new TbBalance();
                                bankInsert.SchoolId = _user.SchoolId;
                                bankInsert.CurrentDate = bank.EnterDate;
                                bankInsert.SourceId = sourceBankId;
                                bankInsert.BankId = bank.BankId;
                                DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < bank.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceBankId && x.BankId == bank.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                if (yesterday.Year != 0001)
                                    bankInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceBankId && x.BankId == bank.BankId).ToList().Sum(x => x.Closing);
                                else
                                    bankInsert.Opening = 0;
                                bankInsert.Closing = bankInsert.Opening - model.Amount;
                                bankInsert.IsActive = true;
                                bankInsert.TimeStamp = CurrentTime;
                                _Entities.TbBalances.Add(bankInsert);
                                _Entities.SaveChanges();
                            }
                            if (cash == true)
                            {
                                if (cashBalance != null)
                                {
                                    cashBalance.Closing = cashBalance.Closing + model.Amount;
                                    cashBalance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var cashInsert = new TbBalance();
                                    cashInsert.SchoolId = _user.SchoolId;
                                    cashInsert.CurrentDate = bank.EnterDate;
                                    cashInsert.SourceId = sourceCashId;
                                    cashInsert.BankId = 0;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate < bank.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        cashInsert.Opening = 0;
                                    cashInsert.Closing = cashInsert.Opening + model.Amount;
                                    cashInsert.IsActive = true;
                                    cashInsert.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(cashInsert);
                                    _Entities.SaveChanges();
                                }
                            }
                            AllUpdatesInBalance(typeId, bank.EnterDate, cash, model.Amount, model.BankId);
                        }
                        else // Deposit
                        {
                            if (bankBalance != null)
                            {
                                bankBalance.Closing = bankBalance.Closing + model.Amount;
                                bankBalance.TimeStamp = CurrentTime;
                                _Entities.SaveChanges();
                            }
                            else
                            {
                                var bankInsert = new TbBalance();
                                bankInsert.SchoolId = _user.SchoolId;
                                bankInsert.CurrentDate = bank.EnterDate;
                                bankInsert.SourceId = sourceBankId;
                                bankInsert.BankId = bank.BankId;
                                DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < bank.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceBankId && x.BankId == bank.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                if (yesterday.Year != 0001)
                                    bankInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date== yesterday.Date && x.SourceId == sourceBankId && x.BankId == bank.BankId).ToList().Sum(x => x.Closing);
                                else
                                    bankInsert.Opening = 0;
                                bankInsert.Closing = bankInsert.Opening + model.Amount;
                                bankInsert.IsActive = true;
                                bankInsert.TimeStamp = CurrentTime;
                                _Entities.TbBalances.Add(bankInsert);
                                _Entities.SaveChanges();
                            }
                            if (cash == true)
                            {
                                if (cashBalance != null)
                                {
                                    cashBalance.Closing = cashBalance.Closing - model.Amount;
                                    cashBalance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var cashInsert = new TbBalance();
                                    cashInsert.SchoolId = _user.SchoolId;
                                    cashInsert.CurrentDate = bank.EnterDate;
                                    cashInsert.SourceId = sourceCashId;
                                    cashInsert.BankId = 0;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < bank.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        cashInsert.Opening = 0;
                                    cashInsert.Closing = cashInsert.Opening - model.Amount;
                                    cashInsert.IsActive = true;
                                    cashInsert.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(cashInsert);
                                    _Entities.SaveChanges();
                                }
                            }
                            AllUpdatesInBalance(typeId, bank.EnterDate, cash, model.Amount, model.BankId);
                        }
                        #endregion Cash Handiling

                        #endregion Balance
                    }
                    #endregion Increase the voucher number 
                }
                if (status == true)
                    msg = "Succeddfully Added !";
                else
                    msg = "Failed to add Bank ENtry !";
            }
            else
            {
                msg = "Insufficient amount in this Account !";
            }
            return Json(new { status = status, msg = msg });
        }
        private void AllUpdatesInBalance(int typeId, DateTime enterDate, bool cash, decimal amount, long bankId)
        {

            if (typeId == 1)//Withdraw
            {
                int sourceId = Convert.ToInt32(DataFromStatus.Bank);
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == bankId && x.SourceId == sourceId && x.CurrentDate.Date > enterDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        var xx = item.Opening;
                        var vv = amount;
                        var b = xx - vv;

                        item.Opening = item.Opening - amount;
                        item.Closing = item.Closing - amount;
                        _Entities.SaveChanges();
                    }
                }
                if (cash == true)
                {
                    sourceId = Convert.ToInt32(DataFromStatus.Cash);
                    var cashData = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == 0 && x.SourceId == 0 && x.CurrentDate.Date > enterDate.Date).ToList();
                    if (cashData != null && cashData.Count > 0)
                    {
                        foreach (var item in cashData)
                        {
                            item.Opening = item.Opening + amount;
                            item.Closing = item.Closing + amount;
                            _Entities.SaveChanges();
                        }
                    }
                }
            }
            else // Deposit
            {
                int sourceId = Convert.ToInt32(DataFromStatus.Bank);
                var balance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == bankId && x.SourceId == sourceId && x.CurrentDate.Date > enterDate.Date).ToList();
                if (balance != null && balance.Count > 0)
                {
                    foreach (var item in balance)
                    {
                        item.Opening = item.Opening + amount;
                        item.Closing = item.Closing + amount;
                        _Entities.SaveChanges();
                    }
                }
                if (cash == true)
                {
                    sourceId = Convert.ToInt32(DataFromStatus.Cash);
                    var cashData = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.BankId == 0 && x.SourceId == 0 && x.CurrentDate.Date > enterDate.Date).ToList();
                    if (cashData != null && cashData.Count > 0)
                    {
                        foreach (var item in cashData)
                        {
                            item.Opening = item.Opening - amount;
                            item.Closing = item.Closing - amount;
                            _Entities.SaveChanges();
                        }
                    }
                }
            }
        }
        public PartialViewResult AddBankBookReaload()
        {
            var model = new BankEntryModel();
            model.SchoolId = _user.SchoolId;
            ViewBag.Bnaklist = _dropdown.GetBankLists(model.SchoolId);
            ViewBag.Accounthead = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/SchoolAccount/_pv_AddBankEntryData.cshtml", model);
        }
        public ActionResult DayBookReportHome()
        {
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = CurrentTime;
            model.ToDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult DayBookReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolName = _user.School.SchoolName;
            model.Heading = start + " to " + end;
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            model.ToDate = end;
            return PartialView("~/Views/SchoolAccount/_pv_DayBookReport.cshtml", model);
        }
        public ActionResult CashEntryReportHome()
        {
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = CurrentTime;
            model.ToDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult CashBookReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            model.ToDate = end;
            model.SchoolName = _user.School.SchoolName;
            model.Heading = start + " to " + end;
            return PartialView("~/Views/SchoolAccount/_pv_CashBookReport.cshtml", model);
        }
        public PartialViewResult GetOpenBallanceByCash(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            return PartialView("~/Views/SchoolAccount/_pv_CashEntryOpeningBalance.cshtml", model);
        }
        public PartialViewResult GetClosingBallanceByCash(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.ToDate = end;
            return PartialView("~/Views/SchoolAccount/_pv_CashEntryClosingBalance.cshtml", model);
        }

        public ActionResult BankEntryReportHome()
        {
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = CurrentTime;
            model.ToDate = CurrentTime;
            //DropdownData dropdown = new DropdownData();
            ViewBag.BankList = _dropdown.GetBankLists(model.SchoolId);
            return View(model);
        }
        public PartialViewResult BankBookReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            model.ToDate = end;
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
            model.BankId = bankId;
            model.SchoolName = _user.School.SchoolName;
            model.Heading = start + " to " + end;
            return PartialView("~/Views/SchoolAccount/_pv_BankBookReport.cshtml", model);
        }
        public PartialViewResult GetOpenBallanceByBank(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
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
            model.BankId = bankId;
            return PartialView("~/Views/SchoolAccount/_pv_BankEntryOpeningBalance.cshtml", model);
        }
        public PartialViewResult GetClosingBallanceByBank(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.ToDate = end;
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
            model.BankId = bankId;
            return PartialView("~/Views/SchoolAccount/_pv_BankEntryClosingBalance.cshtml", model);
        }
        public ActionResult ReceiptAndPaymentHome()
        {
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = CurrentTime;
            model.ToDate = CurrentTime;
            return View(model);
        }
        public PartialViewResult RPReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            Models.DayBookReportModel model = new Models.DayBookReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            model.ToDate = end;
            model.SchoolName = _user.School.SchoolName;
            model.Heading = start + " to " + end;
            return PartialView("~/Views/SchoolAccount/_pv_RPReport.cshtml", model);
        }
        public PartialViewResult LoadBankEntrySearchView()
        {
            BankEntryModel model = new BankEntryModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/SchoolAccount/_pv_BankEntrySearch.cshtml", model);
        }

        [HttpPost]
        public object SearchVoucherExistance(BankEntryModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (model.bankTypeId == 0)
            {
                model.TransactionType = "R";
                model.VoucherType = "RV";
            }
            else
            {
                model.TransactionType = "P";
                model.VoucherType = "PV";
            }
            if (model.contra == 1)
                model.VoucherType = "CV";
            var VoucherExist = _Entities.TbBankEntries.Where(x => x.SchoolId == _user.SchoolId && x.CancelStatus == false && x.TransactionType == model.TransactionType && x.VoucherType == model.VoucherType && x.VoucherNumber == model.voucher && x.IsActive).FirstOrDefault();
            if (VoucherExist == null)
            {
                msg = "No such voucher number in this criteria!";
            }
            else
            {
                if (VoucherExist.BillNo != null && VoucherExist.BillNo != string.Empty)
                {
                    try
                    {
                        int billNo = Convert.ToInt32(VoucherExist.BillNo);
                        if (billNo != 0)
                        {
                            msg = "This is a bill voucher, please cancel it from the billing section ";
                        }
                        else
                        {
                            status = true;
                        }
                    }
                    catch
                    {
                        msg = "";
                        status = true;
                    }
                }
                else
                {
                    var feeCheck = _Entities.TbAccountHeads.Where(x => x.AccountId == VoucherExist.HeadId && x.ForBill == true).FirstOrDefault();
                    if (feeCheck != null)
                    {
                        msg = "This Voucher is created from the Bill , you can't be change it from here !";
                    }
                    else
                    {
                        msg = "";
                        status = true;
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult LoadBankEntryEditView(string id)
        {
            string[] splitDate = id.Split('~');
            int type = Convert.ToInt32(splitDate[0]);
            int contra = Convert.ToInt32(splitDate[1]);
            string voucherNumber = Convert.ToString(splitDate[2]);
            string voucherType = "";
            string transactionType = "";
            if (type == 0)
            {
                voucherType = "RV";
                transactionType = "R";
            }
            else
            {
                voucherType = "PV";
                transactionType = "P";
            }
            if (contra == 1)
            {
                voucherType = "CV";
            }
            var bankData = _Entities.TbBankEntries.Where(x => x.SchoolId == _user.SchoolId && x.CancelStatus == false && x.VoucherType == voucherType && x.VoucherNumber == voucherNumber && x.IsActive == true && x.TransactionType == transactionType).FirstOrDefault();
            BankEntryModel model = new BankEntryModel();
            model.SchoolId = _user.SchoolId;
            model.VoucherNo = bankData.VoucherNumber;
            model.BankId = bankData.BankId;
            if (type == 0)
                model.TypeId = BankType.Deposit;
            else
                model.TypeId = BankType.Withdraw;
            model.HeadId = bankData.HeadId;
            model.SubLedgerId = bankData.SubId;
            model.Narration = bankData.Narration;
            if (bankData.ChequeNumber != string.Empty && bankData.ChequeNumber != null)
                model.ChequeNo = bankData.ChequeNumber;
            if (bankData.ChequeDate != null)
                model.ChequeDateString = bankData.ChequeDate.ToString();
            model.Amount = bankData.Amount ?? 0;
            model.EntryDateString = bankData.EnterDate.ToString();
            if (bankData.VoucherType == "CV")
                model.CashTransaction = true;
            else
                model.CashTransaction = false;
            model.SubLedgerData = _Entities.TbSubLedgerData.Where(x => x.LedgerId == bankData.SubId).Select(x => x.SubLedgerName).FirstOrDefault();
            model.BankDataId = bankData.Id;
            ViewBag.Banklist = _dropdown.GetBankLists(model.SchoolId);
            ViewBag.Accountheadlist = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/SchoolAccount/_pv_EditBankEntryData.cshtml", model);

        }
        public object EditBankEntry(BankEntryModel model)
        {
            long Id = model.BankDataId;
            string msg = "Failed";
            bool status = false;
            if (Id != 0)
            {
                var bankdata = _Entities.TbBankEntries.Where(x => x.Id == Id && x.CancelStatus == false && x.IsActive == true).FirstOrDefault();
                if (bankdata != null)
                {
                    bankdata.HeadId = model.HeadId;
                    if (bankdata.SubId != model.SubLedgerId && model.SubLedgerId != 0)
                    {
                        bankdata.SubId = model.SubLedgerId;
                    }
                    bankdata.Narration = model.Narration;
                    bankdata.ChequeNumber = model.ChequeNo;
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Successfull";
                    else
                        msg = "Failed";
                    #region Contra
                    if (bankdata.VoucherType == "CV")
                    {
                        var cashData = _Entities.TbCashEntries.Where(x => x.IsActive == true && x.CancelStatus == false && x.VoucherType == "CV" && x.VoucherNumber == bankdata.VoucherNumber).FirstOrDefault();
                        if (cashData != null)
                        {
                            cashData.HeadId = model.HeadId;
                            if (cashData.SubId != model.SubLedgerId && model.SubLedgerId != 0)
                            {
                                cashData.SubId = model.SubLedgerId;
                            }
                            cashData.Narration = model.Narration;
                            status = _Entities.SaveChanges() > 0;
                        }
                    }
                    #endregion Contra

                }
                else
                {
                    msg = "No Such data!";
                }
            }
            else
            {
                msg = "No Such data!";
            }
            return Json(new { status = status, msg = msg });
        }
        public object CancelBankEntry(string id)
        {
            long BankEntryId = Convert.ToInt64(id);
            bool status = false;
            string msg = "Failed";
            var bankData = _Entities.TbBankEntries.Where(x => x.Id == BankEntryId && x.CancelStatus == false && x.IsActive).FirstOrDefault();
            if (bankData != null)
            {
                bankData.CancelStatus = true;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    msg = "Successfully canceled";
                    int typeId = 0;
                    bool cash = false;
                    #region CurrentDateUpdate 
                    if (bankData.TransactionType == "P")
                    {
                        typeId = 0;//Actually this is a withdraw ,but here we cancel the data,so we doing the opposit function
                    }
                    else if (bankData.TransactionType == "R")
                    {
                        typeId = 1;//Actually this is a deposit ,but here we cancel the data,so we doing the opposit function
                    }
                    if (bankData.VoucherType == "CV")
                    {
                        cash = true;
                    }
                    int sourceBankId = Convert.ToInt32(DataFromStatus.Bank);
                    int sourceCashId = Convert.ToInt32(DataFromStatus.Cash);
                    #region BankUpdate
                    //*************************************************************************************
                    var bankBalance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == bankData.EnterDate.Date && x.SourceId == sourceBankId && x.BankId == bankData.BankId).FirstOrDefault();
                    if (typeId == 1)//Like withdraw
                    {
                        if (bankBalance != null)
                        {
                            bankBalance.Closing = bankBalance.Closing - bankData.Amount ?? 0;
                            bankBalance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            var bankInsert = new TbBalance();
                            bankInsert.SchoolId = _user.SchoolId;
                            bankInsert.CurrentDate = bankData.EnterDate;
                            bankInsert.SourceId = sourceBankId;
                            bankInsert.BankId = bankData.BankId;
                            DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < bankData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceBankId && x.BankId == bankData.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                bankInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceBankId && x.BankId == bankData.BankId).ToList().Sum(x => x.Closing);
                            else
                                bankInsert.Opening = 0;
                            bankInsert.Closing = bankInsert.Opening - bankData.Amount ?? 0;
                            bankInsert.IsActive = true;
                            bankInsert.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(bankInsert);
                            _Entities.SaveChanges();
                        }
                    }
                    else
                    {
                        if (bankBalance != null)
                        {
                            bankBalance.Closing = bankBalance.Closing + bankData.Amount ?? 0;
                            bankBalance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            var bankInsert = new TbBalance();
                            bankInsert.SchoolId = _user.SchoolId;
                            bankInsert.CurrentDate = bankData.EnterDate;
                            bankInsert.SourceId = sourceBankId;
                            bankInsert.BankId = bankData.BankId;
                            DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < bankData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceBankId && x.BankId == bankData.BankId).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                bankInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceBankId && x.BankId == bankData.BankId).ToList().Sum(x => x.Closing);
                            else
                                bankInsert.Opening = 0;
                            bankInsert.Closing = bankInsert.Opening + bankData.Amount ?? 0;
                            bankInsert.IsActive = true;
                            bankInsert.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(bankInsert);
                            _Entities.SaveChanges();
                        }
                    }
                    AllUpdatesInBalance(typeId, bankData.EnterDate, cash, bankData.Amount ?? 0, bankData.BankId);
                    //*************************************************************************************
                    #endregion BankUpdate
                    #region CashUpdate
                    if (cash == true)
                    {
                        //*************************************************************************************
                        var cashBalance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == bankData.EnterDate.Date && x.SourceId == sourceCashId && x.BankId == 0).FirstOrDefault();
                        var cashData = _Entities.TbCashEntries.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.VoucherType == "CV" && x.VoucherNumber == bankData.VoucherNumber && x.EnterDate.Date == bankData.EnterDate.Date && x.CancelStatus == false && x.TransactionType != bankData.TransactionType).FirstOrDefault();
                        if (cashData != null)
                        {
                            cashData.CancelStatus = true;
                            _Entities.SaveChanges();
                            if (typeId == 1)
                            {
                                if (cashBalance != null)
                                {
                                    cashBalance.Closing = cashBalance.Closing + cashData.Amount ?? 0;
                                    cashBalance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var cashInsert = new TbBalance();
                                    cashInsert.SchoolId = _user.SchoolId;
                                    cashInsert.CurrentDate = cashData.EnterDate;
                                    cashInsert.SourceId = sourceCashId;
                                    cashInsert.BankId = 0;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cashData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        cashInsert.Opening = 0;
                                    cashInsert.Closing = cashInsert.Opening + cashData.Amount ?? 0;
                                    cashInsert.IsActive = true;
                                    cashInsert.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(cashInsert);
                                    _Entities.SaveChanges();
                                }
                            }
                            else
                            {
                                if (cashBalance != null)
                                {
                                    cashBalance.Closing = cashBalance.Closing - cashData.Amount ?? 0;
                                    cashBalance.TimeStamp = CurrentTime;
                                    _Entities.SaveChanges();
                                }
                                else
                                {
                                    var cashInsert = new TbBalance();
                                    cashInsert.SchoolId = _user.SchoolId;
                                    cashInsert.CurrentDate = cashData.EnterDate;
                                    cashInsert.SourceId = sourceCashId;
                                    cashInsert.BankId = 0;
                                    DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cashData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                                    if (yesterday.Year != 0001)
                                        cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                                    else
                                        cashInsert.Opening = 0;
                                    cashInsert.Closing = cashInsert.Opening - cashData.Amount ?? 0;
                                    cashInsert.IsActive = true;
                                    cashInsert.TimeStamp = CurrentTime;
                                    _Entities.TbBalances.Add(cashInsert);
                                    _Entities.SaveChanges();
                                }
                            }
                            //int cashType = 0;
                            //if (typeId == 0)
                            //    cashType = 1;
                            //AllUpdatesInBalanceCash(cashType, cashData.EnterDate, cashData.Amount ?? 0);
                        }
                        //*************************************************************************************
                    }
                    #endregion CashUpdate
                    #endregion CurrentDateUpdate 
                }
                else
                    msg = "Failed";
            }
            else
            {
                msg = "Failed";
            }
            return Json(new { status = status, msg = msg });
        }
        public PartialViewResult LoadCashEntrySearchView()
        {
            CashEntryModel model = new CashEntryModel();
            model.SchoolId = _user.SchoolId;
            return PartialView("~/Views/SchoolAccount/_pv_CashEntrySearch.cshtml", model);
        }
        [HttpPost]
        public object SearchCashVoucherExistance(CashEntryModel model)
        {
            bool status = false;
            string msg = "Failed";
            if (model.cashTypeId == 0)
            {
                model.TransactionType = "P";
                model.VoucherType = "PV";
            }
            else
            {
                model.TransactionType = "R";
                model.VoucherType = "RV";
            }
            var VoucherExist = _Entities.TbCashEntries.Where(x => x.SchoolId == _user.SchoolId && x.CancelStatus == false && x.TransactionType == model.TransactionType && x.VoucherType == model.VoucherType && x.VoucherNumber == model.voucher && x.IsActive && x.VoucherType != "CV").FirstOrDefault();
            if (VoucherExist == null)
            {
                msg = "No such voucher number in this criteria!";
            }
            else
            {
                if (VoucherExist.BillNo != null && VoucherExist.BillNo != string.Empty)
                {
                    try
                    {
                        int billNo = Convert.ToInt32(VoucherExist.BillNo);
                        if (billNo != 0)
                        {
                            msg = "This is a bill voucher, please cancel it from the billing section ";
                        }
                        else
                        {
                            status = true;
                        }
                    }
                    catch
                    {
                        msg = "";
                        status = true;
                    }
                }
                else
                {
                    var feeCheck = _Entities.TbAccountHeads.Where(x => x.AccountId == VoucherExist.HeadId && x.ForBill == true).FirstOrDefault();
                    if (feeCheck != null)
                    {
                        msg = "This Voucher is created from the Bill , you can't be change it from here !";
                    }
                    else
                    {
                        msg = "";
                        status = true;
                    }
                }
            }
            return Json(new { status = status, msg = msg });
        }

        public PartialViewResult LoadCashEntryEditView(string id)
        {
            string[] splitDate = id.Split('~');
            int type = Convert.ToInt32(splitDate[0]);
            string voucherNumber = Convert.ToString(splitDate[1]);
            string voucherType = "";
            string transactionType = "";
            if (type == 1)
            {
                voucherType = "RV";
                transactionType = "R";
            }
            else
            {
                voucherType = "PV";
                transactionType = "P";
            }
            var cashData = _Entities.TbCashEntries.Where(x => x.SchoolId == _user.SchoolId && x.CancelStatus == false && x.VoucherType == voucherType && x.VoucherNumber == voucherNumber && x.IsActive == true && x.TransactionType == transactionType).FirstOrDefault();
            CashEntryModel model = new CashEntryModel();
            model.SchoolId = _user.SchoolId;
            model.VoucherNo = cashData.VoucherNumber;
            if (type == 0)
                model.TypeId = AccountType.Expense;
            else
                model.TypeId = AccountType.Income;
            model.HeadId = cashData.HeadId;
            model.SubLedgerId = cashData.SubId;
            model.Narration = cashData.Narration;
            model.Amount = cashData.Amount ?? 0;
            model.EntryDateString = cashData.EnterDate.ToString();
            model.SubLedgerData = _Entities.TbSubLedgerData.Where(x => x.LedgerId == cashData.SubId).Select(x => x.SubLedgerName).FirstOrDefault();
            model.CashId = cashData.Id;
            ViewBag.Accountlist = _dropdown.GetAccountHeads(model.SchoolId);
            return PartialView("~/Views/SchoolAccount/_pv_EditCashEntryData.cshtml", model);

        }

        public object CancelCashEntry(string id)
        {
            long CashEntryId = Convert.ToInt64(id);
            bool status = false;
            string msg = "Failed";
            var cashEntryData = _Entities.TbCashEntries.Where(x => x.Id == CashEntryId && x.CancelStatus == false && x.IsActive).FirstOrDefault();
            if (cashEntryData != null)
            {
                cashEntryData.CancelStatus = true;
                status = _Entities.SaveChanges() > 0;
                if (status)
                {
                    msg = "Successfully canceled";
                    int typeId = 0;
                    #region CurrentDateUpdate 
                    if (cashEntryData.TransactionType == "P")
                    {
                        typeId = 0;//Actually this is a expense ,but here we cancel the data,so we doing the opposit function
                    }
                    else if (cashEntryData.TransactionType == "R")
                    {
                        typeId = 1;//Actually this is a income ,but here we cancel the data,so we doing the opposit function
                    }
                    int sourceCashId = Convert.ToInt32(DataFromStatus.Cash);
                    #region CashUpdate
                    //*************************************************************************************
                    var cashBalance = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == cashEntryData.EnterDate.Date && x.SourceId == sourceCashId && x.BankId == 0).FirstOrDefault();
                    if (typeId == 0)
                    {
                        if (cashBalance != null)
                        {
                            cashBalance.Closing = cashBalance.Closing + cashEntryData.Amount ?? 0;
                            cashBalance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            var cashInsert = new TbBalance();
                            cashInsert.SchoolId = _user.SchoolId;
                            cashInsert.CurrentDate = cashEntryData.EnterDate;
                            cashInsert.SourceId = sourceCashId;
                            cashInsert.BankId = 0;
                            DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cashEntryData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                            else
                                cashInsert.Opening = 0;
                            cashInsert.Closing = cashInsert.Opening + cashEntryData.Amount ?? 0;
                            cashInsert.IsActive = true;
                            cashInsert.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(cashInsert);
                            _Entities.SaveChanges();
                        }
                    }
                    else
                    {
                        if (cashBalance != null)
                        {
                            cashBalance.Closing = cashBalance.Closing - cashEntryData.Amount ?? 0;
                            cashBalance.TimeStamp = CurrentTime;
                            _Entities.SaveChanges();
                        }
                        else
                        {
                            var cashInsert = new TbBalance();
                            cashInsert.SchoolId = _user.SchoolId;
                            cashInsert.CurrentDate = cashEntryData.EnterDate;
                            cashInsert.SourceId = sourceCashId;
                            cashInsert.BankId = 0;
                            DateTime yesterday = _Entities.TbBalances.Where(x => x.CurrentDate.Date < cashEntryData.EnterDate.Date && x.SchoolId == _user.SchoolId && x.IsActive == true && x.SourceId == sourceCashId && x.BankId == 0).OrderByDescending(x => x.CurrentDate).Select(x => x.CurrentDate).FirstOrDefault();
                            if (yesterday.Year != 0001)
                                cashInsert.Opening = _Entities.TbBalances.Where(x => x.SchoolId == _user.SchoolId && x.IsActive == true && x.CurrentDate.Date == yesterday.Date && x.SourceId == sourceCashId && x.BankId == 0).ToList().Sum(x => x.Closing);
                            else
                                cashInsert.Opening = 0;
                            cashInsert.Closing = cashInsert.Opening - cashEntryData.Amount ?? 0;
                            cashInsert.IsActive = true;
                            cashInsert.TimeStamp = CurrentTime;
                            _Entities.TbBalances.Add(cashInsert);
                            _Entities.SaveChanges();
                        }
                    }
                    int cash = 0;
                    if (typeId == 0)
                        cash = 1;
                    AllUpdatesInBalanceCash(cash, cashEntryData.EnterDate, cashEntryData.Amount ?? 0);
                    //*************************************************************************************
                    #endregion CashUpdate
                    #endregion CurrentDateUpdate 
                }
                else
                    msg = "Failed";
            }
            else
            {
                msg = "Failed";
            }
            return Json(new { status = status, msg = msg });
        }

        public object EditCashEntry(CashEntryModel model)
        {
            long Id = model.CashId;
            string msg = "Failed";
            bool status = false;
            if (Id != 0)
            {
                var cashData = _Entities.TbCashEntries.Where(x => x.Id == Id && x.CancelStatus == false && x.IsActive == true).FirstOrDefault();
                if (cashData != null)
                {
                    cashData.HeadId = model.HeadId;
                    if (cashData.SubId != model.SubLedgerId && model.SubLedgerId != 0)
                    {
                        cashData.SubId = model.SubLedgerId;
                    }
                    cashData.Narration = model.Narration;
                    status = _Entities.SaveChanges() > 0;
                    if (status)
                        msg = "Successfull";
                    else
                        msg = "Failed";
                }
                else
                {
                    msg = "No Such data!";
                }
            }
            else
            {
                msg = "No Such data!";
            }
            return Json(new { status = status, msg = msg });
        }
        public ActionResult LedgerHome()
        {
            LedgerReportModel model = new LedgerReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = CurrentTime;
            model.ToDate = CurrentTime;
            ViewBag.AccountHeadsWithFeeHead =_dropdown.GetAccountHeadsWithFeeHead(model.SchoolId);
            return View(model);
        }

        public PartialViewResult LedgerReport(string id)
        {
            string[] splitDate = id.Split('~');
            DateTime start = Convert.ToDateTime(splitDate[0]);
            DateTime end = Convert.ToDateTime(splitDate[1]);
            long headId = Convert.ToInt64(splitDate[2]);
            long subId = Convert.ToInt64(splitDate[3]);
            LedgerReportModel model = new LedgerReportModel();
            model.SchoolId = _user.SchoolId;
            model.FromDate = start;
            model.ToDate = end;
            model.SchoolName = _user.School.SchoolName;
            model.Heading= Convert.ToString(splitDate[4]);
            try
            {
                model.HeadId = headId;
                model.SubId = subId;
            }
            catch
            {
                model.HeadId = 0;
                model.SubId = 0;
            }
            return PartialView("~/Views/SchoolAccount/_pv_LedgerReport.cshtml", model);
        }

        public object LoadSubLedgerListWithFee(long id)
        {
            var head = _Entities.TbAccountHeads.Where(x => x.IsActive == true && x.AccountId == id).FirstOrDefault();
            if (head.ForBill == true)
            {
                var result = _Entities.TbFees.Where(x => x.IsActive == true && x.SchoolId == _user.SchoolId).ToList().Select(x =>
                new { x.FeeId, x.FeesName }
                ).ToList().OrderBy(x => x.FeesName);

                var data = result.Select(x => new SelectListItem { Text = x.FeesName, Value = x.FeeId.ToString() }).ToList();
                data.Add(new SelectListItem { Text = "Select", Value = "0" });
                data.Add(new SelectListItem { Text = "All", Value = "1" });
                return Json(new { status = data.Count > 0, list = data.OrderBy(x => x.Value).ToList() });
            }
            else
            {
                var dropdown = new Satluj_Latest.Data.DropdownData(_Entities);
                var result = dropdown.GetSubLedgerList(id);
                result.Add(new SelectListItem { Text = "All", Value = "0" });
                return Json(new { status = result.Count > 0, list = result.OrderBy(x => x.Value).ToList() });
            }
        }
    }
}


