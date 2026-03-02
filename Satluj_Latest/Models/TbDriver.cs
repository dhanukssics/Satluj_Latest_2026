using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDriver
{
    public long DriverId { get; set; }

    public long SchoolId { get; set; }

    public string DriverSpecialId { get; set; } = null!;

    public string DriverName { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid DriverGuid { get; set; }

    public bool IsActive { get; set; }

    public string? FilePath { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbTrip> TbTrips { get; set; } = new List<TbTrip>();
}
