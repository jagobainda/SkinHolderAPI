using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class user
{
    public int UserID { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<registro> registros { get; set; } = new List<registro>();

    public virtual ICollection<useritem> useritems { get; set; } = new List<useritem>();
}
