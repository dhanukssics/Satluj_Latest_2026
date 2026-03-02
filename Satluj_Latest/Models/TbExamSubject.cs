using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExamSubject
{
    public long SubId { get; set; }

    public long ExamId { get; set; }

    public string Subject { get; set; } = null!;

    public decimal Mark { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal InternalMarks { get; set; }

    public decimal ExternalMark { get; set; }

    public DateTime ExamDate { get; set; }

    public long SubjectId { get; set; }

    public virtual TbExam Exam { get; set; } = null!;

    public virtual TbSubject SubjectNavigation { get; set; } = null!;

    public virtual ICollection<TbStudentMark> TbStudentMarks { get; set; } = new List<TbStudentMark>();
}
