using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentPaidAmount
{
    public long PaidId { get; set; }

    public long StudentId { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    public long BillNo { get; set; }

    public bool IsActive { get; set; }

    public decimal? PreviousBalance { get; set; }

    public bool? AddAccountStatus { get; set; }

    public virtual TbStudent Student { get; set; } = null!;
}
