using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbResetPassword
{
    public long ResetPasswordId { get; set; }

    public bool LinkExpireStatus { get; set; }

    public long UserId { get; set; }

    public Guid UserGuid { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }
}
