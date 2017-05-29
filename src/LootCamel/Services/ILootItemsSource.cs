using LootCamel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LootCamel.Interfaces
{
    public interface ILootItemsSource
    {
        Task<ICollection<JsonLootItem>> GetAllItems();
    }
}
