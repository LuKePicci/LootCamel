using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Models;

namespace LootCamel.Interfaces
{
    public interface ILootShopExplorer
    {
        Task<ICollection<JsonLootShopItem>> GetShopItems(long shopId);
        Task<ICollection<JsonLootShopItem>> GetUpdatedShopItems(int minutes);
        string HookToken { get; }
    }
}
