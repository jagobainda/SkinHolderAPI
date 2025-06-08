using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class logtype
{
    public int LogTypeID { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<logger> loggers { get; set; } = new List<logger>();
}
