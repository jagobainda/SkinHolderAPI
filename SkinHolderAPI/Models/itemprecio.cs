using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Itemprecio
{
    public long ItemPrecioId { get; set; }

    public decimal PrecioSteam { get; set; }

    public decimal PrecioGamerPay { get; set; }

    public long UserItemId { get; set; }

    public long RegistroId { get; set; }

    public virtual Registro Registro { get; set; } = null!;

    public virtual Useritem UserItem { get; set; } = null!;
}
