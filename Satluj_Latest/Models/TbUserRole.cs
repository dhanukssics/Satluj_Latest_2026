using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbUserRole
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbRoleDetail Role { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
