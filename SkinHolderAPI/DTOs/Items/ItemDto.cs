namespace SkinHolderAPI.DTOs.Items;

public class ItemDto
{
    public int ItemId { get; set; }

    public string Nombre { get; set; } = null!;

    public string HashNameSteam { get; set; } = null!;

    public string GamerPayNombre { get; set; } = null!;
}
