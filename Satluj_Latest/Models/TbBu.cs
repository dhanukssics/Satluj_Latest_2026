using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBu
{
    public long BusId { get; set; }

    public string BusSpecialId { get; set; } = null!;

    public int TripNumber { get; set; }

    public string LocationStart { get; set; } = null!;

    public string? LocationEnd { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid BusGuid { get; set; }

    public bool IsActive { get; set; }

    public string? BusType { get; set; }

    public string? BusName { get; set; }

    public long SchoolId { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbStudent> TbStudents { get; set; } = new List<TbStudent>();

    public virtual ICollection<TbTrip> TbTrips { get; set; } = new List<TbTrip>();
}
