using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Useritem
{
    public long UserItemId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioMedioCompra { get; set; }

    public int ItemId { get; set; }

    public int UserId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual ICollection<Itemprecio> Itemprecios { get; set; } = new List<Itemprecio>();

    public virtual User User { get; set; } = null!;
}
