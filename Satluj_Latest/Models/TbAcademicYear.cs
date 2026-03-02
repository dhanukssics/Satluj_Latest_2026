using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAcademicYear
{
    public long YearId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? CurrentYear { get; set; }

    public virtual ICollection<TbClass> TbClasses { get; set; } = new List<TbClass>();
}
