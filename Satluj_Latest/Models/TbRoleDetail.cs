using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbRoleDetail
{
    private TbRoleDetail x;

    public TbRoleDetail()
    {
    }

    public TbRoleDetail(TbRoleDetail x)
    {
        this.x = x;
    }

    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string RoleName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbUserRole> TbUserRoles { get; set; } = new List<TbUserRole>();
}
