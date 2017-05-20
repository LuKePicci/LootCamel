using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Interfaces;
using LootCamel.Models;

namespace LootCamel.Services
{
    public class LootCamelServices : IPriceNotifier
    {
        private readonly IBotConnector bot;

        public LootCamelServices(IBotConnector bot)
        {
            this.bot = bot;
        }

        public async Task SendNotifications(ICollection<Subscription> subs, IDictionary<int, (int, long)> updates)
        {
            var actualSubs = from sub in subs
                       group sub by new { sub.LootItemID, sub.ChatID } into subsGroup
                       select new {
                           subsGroup.Key.ChatID,
                           minprice = subsGroup.Min(s => s.PriceEvent),
                           itemname = subsGroup.First().Item.ItemName,
                           update = updates[subsGroup.First().LootItemID]
                       };

            foreach (var s in actualSubs)
            {
                double result = (double)s.update.Item1 / s.minprice;
                this.bot.SendTextMessage(s.ChatID, String.Format("🚩 Hurry up! {0} is on sale now for {1}§ ({2:00.00}% less than your target {3}§) here:\r\n\r\n@lootplusbot {4}",
                    s.itemname, s.update.Item1, (1 - result) * 100, s.minprice, s.update.Item2));
            }
        }
    }
}
