using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Registro
{
    public long RegistroId { get; set; }

    public DateTime FechaHora { get; set; }

    public decimal TotalSteam { get; set; }

    public decimal TotalGamerPay { get; set; }

    public int UserId { get; set; }

    public int RegistroTypeId { get; set; }

    public virtual ICollection<Itemprecio> Itemprecios { get; set; } = new List<Itemprecio>();

    public virtual Registrotype RegistroType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
