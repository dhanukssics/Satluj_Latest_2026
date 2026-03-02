using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExam
{
    public TbExam()
    {
    }

    public TbExam(TbExam x)
    {
        X = x;
    }

    public long ExamId { get; set; }

    public long SchoolId { get; set; }

    public long UserId { get; set; }

    public long ClassId { get; set; }

    public string ExamName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbExamSubject> TbExamSubjects { get; set; } = new List<TbExamSubject>();

    public virtual ICollection<TbStudentMark> TbStudentMarks { get; set; } = new List<TbStudentMark>();

    public virtual TbLogin User { get; set; } = null!;
    public TbExam X { get; }
}
