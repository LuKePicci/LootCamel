using LootCamel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LootCamel.Interfaces
{
    public interface IPriceProvider
    {
        Task<string> GetStreetPricesAsText();
        string AccessToken { get; }
    }
}
