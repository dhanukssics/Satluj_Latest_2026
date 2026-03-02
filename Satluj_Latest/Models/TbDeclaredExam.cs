using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDeclaredExam
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long ExamId { get; set; }

    public long ClassId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long? TermId { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbExamBook Exam { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbCoScholasticResultmain> TbCoScholasticResultmains { get; set; } = new List<TbCoScholasticResultmain>();

    public virtual ICollection<TbDeclaredExamSubject> TbDeclaredExamSubjects { get; set; } = new List<TbDeclaredExamSubject>();

    public virtual ICollection<TbExamResult> TbExamResults { get; set; } = new List<TbExamResult>();

    public virtual ICollection<TbScholasticResultMain> TbScholasticResultMains { get; set; } = new List<TbScholasticResultMain>();

    public virtual TbExamTerm? TbExamTerm { get; set; }
    public virtual TbExamBook? TbExamBook { get;  set; }
}
