using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSpeakingSkill
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long StudentId { get; set; }

    public long DivisionId { get; set; }

    public long SubjectId { get; set; }

    public string? Conversation { get; set; }

    public string? Recitation { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long UserId { get; set; }

    public long AssesmentId { get; set; }

    public string? SpeakingSkill { get; set; }

    public virtual TbVAssesment Assesment { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbSubject Subject { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
