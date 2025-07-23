using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public bool? Isactive { get; set; }

    public bool Isbanned { get; set; }

    public DateTime Createdat { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();

    public virtual ICollection<Useritem> Useritems { get; set; } = new List<Useritem>();
}
