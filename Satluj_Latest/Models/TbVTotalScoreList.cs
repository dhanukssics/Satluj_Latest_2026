using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbVTotalScoreList
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public bool IsSubject { get; set; }

    public int EnumTypeId { get; set; }

    public string SubTitle { get; set; } = null!;

    public long Mark { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
