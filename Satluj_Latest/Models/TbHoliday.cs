using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbHoliday
{
    public int HolidayId { get; set; }

    public DateOnly HolidayDate { get; set; }

    public string HolidayName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
