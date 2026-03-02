using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBankBookId
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long DepositId { get; set; }

    public long WithdrawId { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
