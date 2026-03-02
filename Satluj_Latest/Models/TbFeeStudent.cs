using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFeeStudent
{
    public long FeeStudentId { get; set; }

    public decimal Amount { get; set; }

    public long StudentId { get; set; }

    public long FeeId { get; set; }

    public Guid FeeStudentGuid { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public DateTime DueDate { get; set; }

    public int Instalment { get; set; }

    public virtual TbFee Fee { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
