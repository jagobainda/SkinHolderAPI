using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class user
{
    public int UserID { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<logger> loggers { get; set; } = new List<logger>();
}
