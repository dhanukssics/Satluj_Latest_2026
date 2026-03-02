using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFee
{
    private TbFee fee;

    public TbFee()
    {

    }
    public TbFee(TbFee fee)
    {
        this.fee = fee;
    }

    public long FeeId { get; set; }

    public string FeesName { get; set; } = null!;

    public long SchoolId { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool IsActive { get; set; }

    public int? FeeType { get; set; }

    public int Interval { get; set; }

    public DateTime FeeStartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal? FineAmount { get; set; }

    public int? NoOfDays { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbBillCancelAccount> TbBillCancelAccounts { get; set; } = new List<TbBillCancelAccount>();

    public virtual ICollection<TbFeeClass> TbFeeClasses { get; set; } = new List<TbFeeClass>();

    public virtual ICollection<TbFeeDiscount> TbFeeDiscounts { get; set; } = new List<TbFeeDiscount>();

    public virtual ICollection<TbFeeDue> TbFeeDues { get; set; } = new List<TbFeeDue>();

    public virtual ICollection<TbFeeStudent> TbFeeStudents { get; set; } = new List<TbFeeStudent>();

    public virtual ICollection<TbLibraryFine> TbLibraryFines { get; set; } = new List<TbLibraryFine>();

    public virtual ICollection<TbPayment> TbPayments { get; set; } = new List<TbPayment>();
}
