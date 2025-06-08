using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class logger
{
    public long LoggerId { get; set; }

    public string LogDescription { get; set; } = null!;

    public DateTime LogDateTime { get; set; }

    public int LogTypeID { get; set; }

    public int LogPlaceID { get; set; }

    public int UserID { get; set; }

    public virtual logplace LogPlace { get; set; } = null!;

    public virtual logtype LogType { get; set; } = null!;

    public virtual user User { get; set; } = null!;
}
