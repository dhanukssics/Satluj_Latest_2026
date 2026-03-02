using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbVGame
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long DivisionId { get; set; }

    public long AssesmentId { get; set; }

    public long UserId { get; set; }

    public string? Enthusiasm { get; set; }

    public string? Discipline { get; set; }

    public string? TeamSpirit { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long StudentId { get; set; }

    public virtual TbVAssesment Assesment { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
