using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbTeacherClass
{
    public long TeacherClassId { get; set; }

    public long TeacherId { get; set; }

    public long ClassId { get; set; }

    public long DivisionId { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbTeacher Teacher { get; set; } = null!;
}
