using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSmtpdetail
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string EmailId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;
}
