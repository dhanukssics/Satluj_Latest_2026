using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBillCancelAccount
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public bool CashBankType { get; set; }

    public long CashBankId { get; set; }

    public long ItemId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CancelDate { get; set; }

    public bool IsActive { get; set; }

    public virtual TbFee Item { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;
}
