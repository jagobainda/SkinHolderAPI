using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class itemprecio
{
    public long ItemPrecioID { get; set; }

    public decimal PrecioSteam { get; set; }

    public decimal PrecioGamerPay { get; set; }

    public long UserItemID { get; set; }

    public long RegistroID { get; set; }

    public virtual registro Registro { get; set; } = null!;

    public virtual useritem UserItem { get; set; } = null!;
}
