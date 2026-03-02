using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCoScholasticResultmain
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long StudentId { get; set; }

    public long ExamId { get; set; }

    public long CoScholasticId { get; set; }

    public string Score { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbCoScholasticArea CoScholastic { get; set; } = null!;

    public virtual TbDeclaredExam Exam { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
