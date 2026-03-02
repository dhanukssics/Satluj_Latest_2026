using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbLaboratoryCategory
{
    public TbLaboratoryCategory()
    {
    }

    public TbLaboratoryCategory(TbLaboratoryCategory x)
    {
        X = x;
    }

    public long CategoryId { get; set; }

    public string LaboratoryName { get; set; } = null!;

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbStockUpdate> TbStockUpdates { get; set; } = new List<TbStockUpdate>();
    public TbLaboratoryCategory X { get; }
}
