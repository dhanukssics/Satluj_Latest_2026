using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSchoolSenderId
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string SenderId { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
