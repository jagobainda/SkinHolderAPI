using System;
using System.Collections.Generic;

namespace SkinHolderAPI.Models;

public partial class Registrotype
{
    public int RegistroTypeId { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
}
