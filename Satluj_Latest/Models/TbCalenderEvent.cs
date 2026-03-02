using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCalenderEvent
{
    public TbCalenderEvent()
    {

    }
    public TbCalenderEvent(TbCalenderEvent z)
    {
        Z = z;
    }

    public long EventId { get; set; }

    public string EventHead { get; set; } = null!;

    public string EventDetails { get; set; } = null!;

    public long SchoolId { get; set; }

    public DateTime EventDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;
    public TbCalenderEvent Z { get; }
}
