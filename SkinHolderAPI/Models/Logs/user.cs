using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models.Logs;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Logger> Loggers { get; set; } = new List<Logger>();
}
