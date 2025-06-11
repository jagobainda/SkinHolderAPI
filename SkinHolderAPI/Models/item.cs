using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Nombre { get; set; } = null!;

    public string HashNameSteam { get; set; } = null!;

    public string GamerPayNombre { get; set; } = null!;

    public virtual ICollection<Useritem> Useritems { get; set; } = new List<Useritem>();
}
