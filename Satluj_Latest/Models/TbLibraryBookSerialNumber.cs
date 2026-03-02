using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbLibraryBookSerialNumber
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long SerialNo { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
