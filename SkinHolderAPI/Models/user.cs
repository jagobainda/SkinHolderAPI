using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsBanned { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

    public virtual ICollection<Useritem> Useritems { get; set; } = new List<Useritem>();
}
