using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Models;

namespace LootCamel.Interfaces
{
    public interface ILootItemsSource
    {
        Task<ICollection<JsonLootItem>> GetAllItems();
    }
}
