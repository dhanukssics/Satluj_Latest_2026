using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStockUpdate
{
    public long StockId { get; set; }

    public long CategoryId { get; set; }

    public string Item { get; set; } = null!;

    public decimal Price { get; set; }

    public string PurchaseId { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public int StockStatus { get; set; }

    public long SchoolId { get; set; }

    public long UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbLaboratoryCategory Category { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
