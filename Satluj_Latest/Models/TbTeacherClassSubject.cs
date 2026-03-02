using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbTeacherClassSubject
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long TeacherId { get; set; }

    public long ClassId { get; set; }

    public long DivisionId { get; set; }

    public long SubjectId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbSubject Subject { get; set; } = null!;

    public virtual TbTeacher Teacher { get; set; } = null!;
}
