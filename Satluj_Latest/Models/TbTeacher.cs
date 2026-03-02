using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbTeacher
{
    public long TeacherId { get; set; }

    public string TeacherSpecialId { get; set; } = null!;

    public string TeacherName { get; set; } = null!;

    public long SchoolId { get; set; }

    public string ContactNumber { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid TeacherGuid { get; set; }

    public bool IsActive { get; set; }

    public string? FilePath { get; set; }

    public long UserId { get; set; }

    public long? DesignationId { get; set; }

    public long? DepartmentId { get; set; }

    public long? UserType { get; set; }

    public virtual TbDepartment? Department { get; set; }

    public virtual TbDesignation? Designation { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbAllMessage> TbAllMessages { get; set; } = new List<TbAllMessage>();

    public virtual ICollection<TbMessage> TbMessages { get; set; } = new List<TbMessage>();

    public virtual ICollection<TbTeacherClassSubject> TbTeacherClassSubjects { get; set; } = new List<TbTeacherClassSubject>();

    public virtual ICollection<TbTeacherClass> TbTeacherClasses { get; set; } = new List<TbTeacherClass>();

    public virtual ICollection<TbTimeTable> TbTimeTables { get; set; } = new List<TbTimeTable>();

    public virtual TbLogin User { get; set; } = null!;

    public virtual TbUserModuleMain? UserTypeNavigation { get; set; }
}
