using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbOtpmessage
{
    public long OtpId { get; set; }

    public string Otp { get; set; } = null!;

    public int Otptype { get; set; }

    public long StudentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public DateTime ExpTimeStamp { get; set; }

    public virtual TbStudent Student { get; set; } = null!;
}
