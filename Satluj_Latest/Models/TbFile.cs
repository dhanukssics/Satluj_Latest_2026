using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFile
{
    public long FileId { get; set; }

    public string FilePath { get; set; } = null!;

    public int FileModule { get; set; }

    public int FileType { get; set; }

    public long? SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool? School { get; set; }
}
