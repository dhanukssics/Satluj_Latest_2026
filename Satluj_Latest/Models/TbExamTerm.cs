using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbExamTerm
{
    public long Id { get; set; }

    public string DefaultExam { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<TbDeclaredExam> TbDeclaredExams { get; set; } = new List<TbDeclaredExam>();

    public virtual ICollection<TbExamBook> TbExamBooks { get; set; } = new List<TbExamBook>();
}
