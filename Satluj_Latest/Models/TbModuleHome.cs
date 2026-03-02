using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbModuleHome
{
    public long Id { get; set; }

    public string MainModule { get; set; } = null!;

    public int OrderName { get; set; }

    public bool IsActive { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual ICollection<TbSubModule> TbSubModules { get; set; } = new List<TbSubModule>();

    public virtual ICollection<TbUserModuleDetail> TbUserModuleDetails { get; set; } = new List<TbUserModuleDetail>();
}
