using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class item
{
    public int ItemID { get; set; }

    public string Nombre { get; set; } = null!;

    public string HashNameSteam { get; set; } = null!;

    public string GamerPayNombre { get; set; } = null!;

    public virtual ICollection<useritem> useritems { get; set; } = new List<useritem>();
}
