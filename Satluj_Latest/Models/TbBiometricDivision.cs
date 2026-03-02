using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBiometricDivision
{
    public long BioDivId { get; set; }

    public long SchoolId { get; set; }

    public long DivisionId { get; set; }

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;
}
