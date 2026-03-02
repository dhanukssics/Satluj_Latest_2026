using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStaff
{
    public long StaffId { get; set; }

    public long UserId { get; set; }

    public string StaffName { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime Dob { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long? DesignationId { get; set; }

    public long? DepartmentId { get; set; }

    public long? UserType { get; set; }

    public virtual TbDepartment? Department { get; set; }

    public virtual TbDesignation? Designation { get; set; }

    public virtual TbLogin User { get; set; } = null!;

    public virtual TbUserModuleMain? UserTypeNavigation { get; set; }
}
