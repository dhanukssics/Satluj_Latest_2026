using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentBalance
{
    public long BalanceId { get; set; }

    public long StudentId { get; set; }

    public decimal Amount { get; set; }

    public bool IsActive { get; set; }

    public virtual TbStudent Student { get; set; } = null!;
}
