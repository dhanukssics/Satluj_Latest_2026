using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbRegions
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string RegionName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbAcademicPeriod> TbAcademicPeriods { get; set; } = new List<TbAcademicPeriod>();

    public virtual ICollection<TbCoScholasticArea> TbCoScholasticAreas { get; set; } = new List<TbCoScholasticArea>();

    public virtual ICollection<TbRegionsClass> TbRegionsClasses { get; set; } = new List<TbRegionsClass>();

    public virtual ICollection<TbScholasticArea> TbScholasticAreas { get; set; } = new List<TbScholasticArea>();
}
