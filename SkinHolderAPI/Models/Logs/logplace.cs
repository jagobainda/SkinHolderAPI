using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class logplace
{
    public int LogPlaceID { get; set; }

    public string PlaceName { get; set; } = null!;

    public virtual ICollection<logger> loggers { get; set; } = new List<logger>();
}
