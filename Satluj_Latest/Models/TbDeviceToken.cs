using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDeviceToken
{
    public long TokenId { get; set; }

    public int RoleId { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public bool IsActive { get; set; }

    public int LoginStatus { get; set; }
}
