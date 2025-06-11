using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class Logtype
{
    public int LogTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Logger> Loggers { get; set; } = new List<Logger>();
}
