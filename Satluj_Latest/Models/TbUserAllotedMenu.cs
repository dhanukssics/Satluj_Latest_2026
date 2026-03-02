using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbUserAllotedMenu
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long? MenuId { get; set; }

    public virtual TbMenuList? Menu { get; set; }

    public virtual TbLogin? User { get; set; }
}
