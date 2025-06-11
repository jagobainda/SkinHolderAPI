using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class Logger
{
    public long LoggerId { get; set; }

    public string LogDescription { get; set; } = null!;

    public DateTime LogDateTime { get; set; }

    public int LogTypeId { get; set; }

    public int LogPlaceId { get; set; }

    public int UserId { get; set; }

    public virtual Logplace LogPlace { get; set; } = null!;

    public virtual Logtype LogType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
