using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbHomeworkSm
{
    public long HomeworkSmsId { get; set; }

    public long HeadId { get; set; }

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSmsHead Head { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;
}
