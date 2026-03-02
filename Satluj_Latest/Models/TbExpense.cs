using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExpense
{
    public long Id { get; set; }

    public string? AccountHead { get; set; }

    public string? Particular { get; set; }

    public double? Amount { get; set; }

    public bool IsActive { get; set; }

    public long SchoolId { get; set; }

    public DateTime Date { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
