using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbVPersonalityDevelopment
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long DivisionId { get; set; }

    public long AssesmentId { get; set; }

    public long UserId { get; set; }

    public string Courteousness { get; set; } = null!;

    public string Confident { get; set; } = null!;

    public string CareOfBelongings { get; set; } = null!;

    public string Neatness { get; set; } = null!;

    public string Punctuality { get; set; } = null!;

    public string Initiative { get; set; } = null!;

    public string SharingCaring { get; set; } = null!;

    public string Property { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long StudentId { get; set; }

    public virtual TbVAssesment Assesment { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
