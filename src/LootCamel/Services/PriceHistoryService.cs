using LootCamel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LootCamel.Models;
using System.Text;
using Microsoft.Extensions.Options;

namespace LootCamel.Services
{
    public class PriceHistoryService : IPriceProvider
    {
        private readonly ILootCamelRepository repo;
        private readonly IOptions<FellowAccessOptions> priceHistoryOptions;

        public string AccessToken { get { return priceHistoryOptions.Value.AccessToken; } }

        public PriceHistoryService(IOptions<FellowAccessOptions> priceHistoryOptions, ILootCamelRepository repo)
        {
            this.priceHistoryOptions = priceHistoryOptions;
            this.repo = repo;
        }

        public async Task<string> GetStreetPricesAsText()
        {
            var itemsWithPrice = await this.repo.GetItemsWithAveragePrice(5);
            var sb = new StringBuilder();
            foreach (var item in itemsWithPrice)
                sb.AppendFormat("{0}:{1}\r\n", item.Key, item.Value);

            return sb.ToString();
        }
    }
}
