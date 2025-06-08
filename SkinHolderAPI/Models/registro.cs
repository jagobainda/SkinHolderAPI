using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class registro
{
    public long RegistroID { get; set; }

    public DateTime FechaHora { get; set; }

    public decimal TotalSteam { get; set; }

    public decimal TotalGamerPay { get; set; }

    public int UserID { get; set; }

    public int RegistroTypeID { get; set; }

    public virtual registrotype RegistroType { get; set; } = null!;

    public virtual user User { get; set; } = null!;

    public virtual ICollection<itemprecio> itemprecios { get; set; } = new List<itemprecio>();
}
