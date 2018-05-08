using LootCamel.Interfaces;
using LootCamel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LootCamel.Services
{
    public class PriceNotificationService : IPriceNotifier
    {
        private readonly IBotConnector bot;

        public PriceNotificationService(IBotConnector bot)
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

                string text = String.Format("🚩 Hurry up! {0} is on sale now for {1}§ ({2:00.00}% less than your target {3}§) here:\r\n\r\n@lootplusbot {4}\r\n\r\n" +
                    "Would you like more alerts for this item? Pick a price:",
                    s.itemname, s.update.Item1, (1 - result) * 100, s.minprice, s.update.Item2);

                double lowerPrice = Math.Floor(s.minprice * 0.85);
                double higherPrice = Math.Floor(s.minprice * 1.15);

                (string, string)[] buttons = {
                    ( lowerPrice + "§", String.Format("/subscribe {0} {1}", s.itemname, lowerPrice) ),
                    ( s.minprice + "§", String.Format("/subscribe {0} {1}", s.itemname, s.minprice) ),
                    ( higherPrice + "§", String.Format("/subscribe {0} {1}", s.itemname, higherPrice) )
                };

                this.bot.SendInlineKeyboardMessage(s.ChatID, text, this.bot.Get1rKeyboardMarkup(buttons));
            }
        }
    }
}
