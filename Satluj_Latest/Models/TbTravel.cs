using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbTravel
{
    public long TravelId { get; set; }

    public long TripId { get; set; }

    public string Longitude { get; set; } = null!;

    public string Latitude { get; set; } = null!;

    public string? Place { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid TravelGuid { get; set; }

    public virtual TbTrip Trip { get; set; } = null!;
}
