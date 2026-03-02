using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExamResult
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long StudentId { get; set; }

    public long ExamId { get; set; }

    public long SubjectId { get; set; }

    public decimal StudentScore { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal? PracticalScore { get; set; }

    public virtual TbDeclaredExam Exam { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbDeclaredExamSubject Subject { get; set; } = null!;

}
