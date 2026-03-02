using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbUserModuleMain
{
    public long Id { get; set; }

    public bool IsTeacher { get; set; }

    public long SchoolId { get; set; }

    public string UserTypeName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool? IsAdmin { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbStaff> TbStaffs { get; set; } = new List<TbStaff>();

    public virtual ICollection<TbTeacher> TbTeachers { get; set; } = new List<TbTeacher>();

    public virtual ICollection<TbUserModuleDetail> TbUserModuleDetails { get; set; } = new List<TbUserModuleDetail>();
}
