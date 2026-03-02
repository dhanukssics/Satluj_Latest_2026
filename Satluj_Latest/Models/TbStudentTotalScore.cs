using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentTotalScore
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long ClassId { get; set; }

    public long DivisionId { get; set; }

    public long SchoolId { get; set; }

    public long ExamId { get; set; }

    public decimal TotalScore { get; set; }

    public decimal Percentage { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbExamBook Exam { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
