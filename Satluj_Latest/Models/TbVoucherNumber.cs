using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbVoucherNumber
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long ReceiptVoucher { get; set; }

    public long PaymentVoucher { get; set; }

    public long ContraVoucher { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
