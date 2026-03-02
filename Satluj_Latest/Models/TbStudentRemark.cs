using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentRemark
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long ExamId { get; set; }

    public string Remark { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? AdditionalRemarks { get; set; }

    public virtual TbExamBook Exam { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
