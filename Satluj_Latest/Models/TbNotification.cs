using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbNotification
{
    public string? UserName { get; set; }

    public string? NotificationMessage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? IsRead { get; set; }

    public string? Source { get; set; }

    public long? SchoolId { get; set; }

    public long? ClassId { get; set; }

    public long? DivisionId { get; set; }

    public int Id { get; set; }

    public long? SourceId { get; set; }
}
