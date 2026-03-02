using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbScolasticAreaResultDetail
{
    public long Id { get; set; }

    public long MainId { get; set; }

    public long SubjectId { get; set; }

    public decimal Score { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbScholasticResultMain Main { get; set; } = null!;

    public virtual TbSubject Subject { get; set; } = null!;
}
