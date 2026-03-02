using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbOptionalSubjectStudent
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long StudentId { get; set; }

    public long SubjectId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbSubject Subject { get; set; } = null!;
}
