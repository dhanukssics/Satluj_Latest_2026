using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentMark
{
    public long MarkId { get; set; }

    public long StudentId { get; set; }

    public long ExamId { get; set; }

    public long SubjectId { get; set; }

    public long Mark { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal? InternalMark { get; set; }

    public decimal? ExternalMark { get; set; }

    public virtual TbExam Exam { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbExamSubject Subject { get; set; } = null!;
}
