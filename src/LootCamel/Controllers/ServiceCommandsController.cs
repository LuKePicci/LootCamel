using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Data;
using LootCamel.Interfaces;
using LootCamel.Models;

namespace LootCamel.Controllers
{
    [Route("api/[controller]")]
    public class ServiceCommandsController : Controller
    {
        private readonly ILootShopExplorer shopExplorer;
        private readonly ILootItemsSource itemsSource;
        private readonly IPriceNotifier dispatcher;
        private readonly ILootCamelRepository repo;

        public ServiceCommandsController(ILootShopExplorer shopExplorer, ILootItemsSource itemsSource, IPriceNotifier dispatcher, ILootCamelRepository repo)
        {
            this.shopExplorer = shopExplorer;
            this.itemsSource = itemsSource;
            this.dispatcher = dispatcher;
            this.repo = repo;
        }

        // GET: api/servicecommands/triggershopupdates
        [HttpGet("TriggerShopUpdates/{token}")]
        public async Task<string> TriggerShopsUpdates(string token)
        {
            if (token != this.shopExplorer.HookToken)
            {
                HttpContext.Response.StatusCode = 403;
                return "Not Authorized";
            }

            ICollection<JsonLootShopItem> candidates = await this.shopExplorer.GetUpdatedShopItems(15);

            if (candidates.Count > 0)
            {
                // select the best price for each item in updated shops
                IDictionary<int, (int, long)> updates = new Dictionary<int, (int, long)>();

                foreach (JsonLootShopItem item in candidates)
                    if (item.quantity > 0)
                        if (!updates.ContainsKey(item.item_id))
                            updates.Add(item.item_id, (item.price, item.code));
                        else if (item.price <= updates[item.item_id].Item1)
                            updates[item.item_id] = (item.price, item.code);


                // add missing item entities
                ISet<LootItem> newItems = await this.repo.CreateUnkownLootItems(updates.Keys);
                if (newItems.Count > 0)
                {
                    await this.updateItemNames(newItems);
                    await this.repo.AddItems(newItems);
                }

                // save price history
                ISet<Price> newPrices = new HashSet<Price>();
                foreach (var upd in updates)
                {
                    newPrices.Add(this.repo.CreatePrice(upd.Key, upd.Value.Item1));
                }
                await this.repo.AddPrices(newPrices);

                // send notifications
                ICollection<Subscription> subs = await this.repo.GetFilteredSubscriptions(updates);
                await this.dispatcher.SendNotifications(subs, updates);

                // remove triggered subscriptions
                this.repo.RemoveSubscriptions(subs);
                    
                await this.repo.CommitWithIdentities("LootItem");
            }
            return "OK";
        }

        // GET: api/servicecommands/refreshitemnames
        [HttpGet("RefreshItemNames/{token}")]
        public async Task<string> RefreshItemNames(string token)
        {
            if(token != this.shopExplorer.HookToken)
            {
                HttpContext.Response.StatusCode = 403;
                return "Not Authorized";
            }

            await this.updateItemNames(await this.repo.GetKnownLootItems());
            await this.repo.Commit();
            return "update in progress";
        }

        private async Task updateItemNames(ICollection<LootItem> items)
        {
            // get unknown names from lootbotservices and populate newItems ItemNames
            var allItems = await this.itemsSource.GetAllItems();
            foreach (LootItem li in items)
            {
                var itemInfo = allItems.Where(item => item.id == li.ID).Single();
                li.ItemName = itemInfo.name;
                li.BasePrice = itemInfo.value;
            }
        }
    }
}
