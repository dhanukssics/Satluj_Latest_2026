using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbUserModuleDetail
{
    public long Id { get; set; }

    public long UserModuleId { get; set; }

    public long MainId { get; set; }

    public long SubModuleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbModuleHome Main { get; set; } = null!;

    public virtual TbSubModule SubModule { get; set; } = null!;

    public virtual TbUserModuleMain UserModule { get; set; } = null!;
}
