using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDayBookIdBank
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long ExpenseId { get; set; }

    public long IncomeId { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
