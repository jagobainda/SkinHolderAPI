using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class useritem
{
    public long UserItemID { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioMedioCompra { get; set; }

    public int ItemID { get; set; }

    public int UserID { get; set; }

    public virtual item Item { get; set; } = null!;

    public virtual user User { get; set; } = null!;

    public virtual ICollection<itemprecio> itemprecios { get; set; } = new List<itemprecio>();
}
