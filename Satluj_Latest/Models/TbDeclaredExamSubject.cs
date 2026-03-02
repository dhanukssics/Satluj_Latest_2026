using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDeclaredExamSubject
{
    public long Id { get; set; }

    public long DeclaredExamId { get; set; }

    public long SubjectId { get; set; }

    public DateTime ExamDate { get; set; }

    public decimal TotalScore { get; set; }

    public string Remark { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbDeclaredExam DeclaredExam { get; set; } = null!;

    public virtual TbSubject Subject { get; set; } = null!;

    public virtual ICollection<TbExamResult> TbExamResults { get; set; } = new List<TbExamResult>();
}
