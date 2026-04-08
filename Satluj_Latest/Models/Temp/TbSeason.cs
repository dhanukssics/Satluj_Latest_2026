using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

[Table("tb_Season")]
public partial class TbSeason
{
    [Key]
    public long SeasonId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string SeasonName { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public TimeOnly? Break1StartTime { get; set; }

    public TimeOnly? Break1EndTime { get; set; }

    public TimeOnly? Break2StartTime { get; set; }

    public TimeOnly? Break2EndTime { get; set; }
}
