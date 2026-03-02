using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSubModule
{
    public long Id { get; set; }

    public long MainId { get; set; }

    public string SubModule { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbModuleHome Main { get; set; } = null!;

    public virtual ICollection<TbUserModuleDetail> TbUserModuleDetails { get; set; } = new List<TbUserModuleDetail>();
}
