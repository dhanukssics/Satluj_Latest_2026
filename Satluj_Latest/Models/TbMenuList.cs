using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbMenuList
{
    public long MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public int OrderValue { get; set; }

    public virtual ICollection<TbUserAllotedMenu> TbUserAllotedMenus { get; set; } = new List<TbUserAllotedMenu>();
}
