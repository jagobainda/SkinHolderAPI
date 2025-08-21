namespace SkinHolderAPI.DTOs.UserItemsDto;

public class UserItemDto
{
    public long Useritemid { get; set; }

    public int Cantidad { get; set; }

    public int Itemid { get; set; }

    public int Userid { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string SteamHashName { get; set; } = string.Empty;

    public string GamerPayName { get; set; } = string.Empty;

    public string CSFloatMarketHashName { get; set; } = string.Empty;
}
