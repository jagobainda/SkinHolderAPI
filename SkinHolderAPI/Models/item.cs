using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public string Nombre { get; set; } = null!;

    public string Hashnamesteam { get; set; } = null!;

    public string Gamerpaynombre { get; set; } = null!;

    public virtual ICollection<Useritem> Useritems { get; set; } = new List<Useritem>();
}
