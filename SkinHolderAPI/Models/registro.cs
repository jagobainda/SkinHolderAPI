﻿using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Registro
{
    public long Registroid { get; set; }

    public DateTime Fechahora { get; set; }

    public decimal Totalsteam { get; set; }

    public decimal Totalgamerpay { get; set; }

    public decimal Totalcsfloat { get; set; }

    public int Userid { get; set; }

    public virtual ICollection<Itemprecio> Itemprecios { get; set; } = new List<Itemprecio>();

    public virtual User User { get; set; } = null!;
}
