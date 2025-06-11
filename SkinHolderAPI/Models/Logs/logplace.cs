using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class Logplace
{
    public int LogPlaceId { get; set; }

    public string PlaceName { get; set; } = null!;

    public virtual ICollection<Logger> Loggers { get; set; } = new List<Logger>();
}
