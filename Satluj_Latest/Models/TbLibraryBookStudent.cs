using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbLibraryBookStudent
{
    public long StudentBookId { get; set; }

    public long StudentId { get; set; }

    public long BookId { get; set; }

    public bool Status { get; set; }

    public bool IsActive { get; set; }

    public DateTime IssueDateTime { get; set; }

    public DateTime? AcceptDateTime { get; set; }

    public virtual TbLibraryBook Book { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
