using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbTrip
{
    public TbTrip()
    {

    }
    public TbTrip(TbTrip z)
    {
        Z = z;
    }

    public long TripId { get; set; }

    public long DriverId { get; set; }

    public long SchoolId { get; set; }

    public string? TripNo { get; set; }

    public DateTime TripDate { get; set; }

    public string FromLocation { get; set; } = null!;

    public string ToLocation { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime ReachTime { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid TripGuid { get; set; }

    public bool IsActive { get; set; }

    public int TravellingStatus { get; set; }

    public long BusId { get; set; }

    public int? ShiftStatus { get; set; }

    public virtual TbBu Bus { get; set; } = null!;

    public virtual TbDriver Driver { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbTravel> TbTravels { get; set; } = new List<TbTravel>();
    public TbTrip Z { get; }
}
