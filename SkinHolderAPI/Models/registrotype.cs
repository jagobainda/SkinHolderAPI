using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class registrotype
{
    public int RegistroTypeID { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<registro> registros { get; set; } = new List<registro>();
}
