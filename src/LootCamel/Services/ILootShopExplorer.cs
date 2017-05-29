using LootCamel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LootCamel.Interfaces
{
    public interface ILootShopExplorer
    {
        Task<ICollection<JsonLootShopItem>> GetShopItems(long shopId);
        Task<ICollection<JsonLootShopItem>> GetUpdatedShopItems(int minutes);
        string HookToken { get; }
    }
}
