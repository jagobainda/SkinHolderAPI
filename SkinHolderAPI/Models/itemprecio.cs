using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Itemprecio
{
    public long Itemprecioid { get; set; }

    public decimal Preciosteam { get; set; }

    public decimal Preciogamerpay { get; set; }

    public long Useritemid { get; set; }

    public long Registroid { get; set; }

    public virtual Registro Registro { get; set; } = null!;

    public virtual Useritem Useritem { get; set; } = null!;
}
