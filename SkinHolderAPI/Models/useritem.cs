using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Useritem
{
    public long Useritemid { get; set; }

    public int Cantidad { get; set; }

    public decimal Preciomediocompra { get; set; }

    public int Itemid { get; set; }

    public int Userid { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual ICollection<Itemprecio> Itemprecios { get; set; } = new List<Itemprecio>();

    public virtual User User { get; set; } = null!;
}
