using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExamBook
{
    public long Id { get; set; }

    public long TermId { get; set; }

    public string ExamName { get; set; } = null!;

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbDeclaredExam> TbDeclaredExams { get; set; } = new List<TbDeclaredExam>();

    public virtual ICollection<TbStudentRemark> TbStudentRemarks { get; set; } = new List<TbStudentRemark>();

    public virtual ICollection<TbStudentTotalScore> TbStudentTotalScores { get; set; } = new List<TbStudentTotalScore>();

    public virtual TbExamTerm Term { get; set; } = null!;
}
