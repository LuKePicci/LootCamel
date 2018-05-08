using LinqKit;
using LootCamel.Interfaces;
using LootCamel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LootCamel.Data
{
    public class LootCamelRepository : ILootCamelRepository
    {
        private readonly LootCamelContext context;

        public LootCamelRepository(LootCamelContext context)
        {
            this.context = context;
        }

        private async Task<ICollection<LootItem>> getItemsById(ICollection<int> itemsIds)
        {
            var result = this.context.LootItems.Where(item => itemsIds.Contains(item.ID));
            return await result.ToListAsync();
        }

        public async Task<ICollection<LootItem>> GetKnownLootItems()
        {
            return await this.context.LootItems.ToListAsync();
        }

        public class testdto
        {
            public int ID { get; set; }
            public string ItemName { get; set; }
            public string AvgPrice { get; set; }
        }

        public async Task<IDictionary<string, Int64>> GetItemsWithAveragePrice(int pricesPerItem)
        {
            //var query1 = from i in this.context.LootItems

            //         let avgPrice =
            //         (
            //           from price in i.Prices
            //           orderby price.Date descending
            //           select price
            //         ).Take(pricesPerItem).Average(x => x.Value)

            //         select new { Item = i, AveragePrice = avgPrice };


            var rdr = await context.Database.ExecuteSqlQueryAsync(
                "SELECT dbo.LootItems.ID, dbo.LootItems.ItemName, AVG(CAST(dbo.Prices.Value AS BIGINT)) AS AvgPrice " +
                "FROM dbo.LootItems JOIN dbo.Prices ON Prices.LootItemID = LootItems.ID " +
                "WHERE dbo.Prices.ID IN(SELECT TOP 5 dbo.Prices.ID FROM dbo.Prices " +
                "WHERE LootItemID = dbo.LootItems.ID " +
                "ORDER BY dbo.Prices.ID desc) " +
                "GROUP BY dbo.LootItems.ID, dbo.LootItems.ItemName");

            var results = new Dictionary<string, Int64>();

            while (rdr.DbDataReader.Read())
                results.Add(
                    (string)(rdr.DbDataReader[1]),
                    (Int64)(rdr.DbDataReader[2])
                    );

            return results;
        }

        public async Task<LootItem> GetLootItemByName(string name)
        {
            return await this.context.LootItems.Where(i => i.ItemName == name).Include(i => i.Subscriptions).SingleOrDefaultAsync();
        }

        public async Task<ISet<LootItem>> CreateUnkownLootItems(ICollection<int> itemIdCollection)
        {
            var knownIds = (await this.getItemsById(itemIdCollection)).Select(item => item.ID);
            IList<int> unknownIds = itemIdCollection.Where(id => !knownIds.Contains(id)).ToList();
            ISet<LootItem> newItems = new HashSet<LootItem>();
            foreach (int id in unknownIds)
            {
                LootItem newItem = new LootItem() { ID = id };
                newItems.Add(newItem);
            }

            return newItems;
        }

        public async Task AddItems(ISet<LootItem> itemsToAdd)
        {
            await this.context.AddRangeAsync(itemsToAdd);
        }

        public Price CreatePrice(int lootItemId, int value)
        {
            return new Price()
            {
                LootItemID = lootItemId,
                Value = value,
                Date = DateTime.Now
            };
        }

        public async Task AddPrices(ICollection<Price> pricesToAdd)
        {
            await this.context.AddRangeAsync(pricesToAdd);
        }



        public async Task<LootPlayer> GetLootPlayerById(int id)
        {
            return await this.context.LootPlayers.Where(lp => lp.ID == id).SingleOrDefaultAsync();
        }

        public LootPlayer CreateLootPlayer(int id, string nickname)
        {
            return new LootPlayer()
            {
                ID = id,
                Nickname = nickname
            };
        }

        public async Task AddPlayer(LootPlayer player)
        {
            await this.context.LootPlayers.AddAsync(player);
        }

        public async Task<ICollection<Subscription>> GetFilteredSubscriptions(IDictionary<int, (int, long)> itemEvent)
        {
            ExpressionStarter<Subscription> predicate = PredicateBuilder.New<Subscription>(false);

            foreach (var e in itemEvent)
                predicate = predicate.Or(sub => e.Key == sub.LootItemID && e.Value.Item1 <= sub.PriceEvent);

            return await this.context.Subscriptions.AsExpandable().Where(predicate).Include(s => s.Item).ToListAsync();
        }

        public async Task<ICollection<Subscription>> GetSubscriptionsByLootPlayerId(long pid, bool withItem = false)
        {
            var data = this.context.Subscriptions.Where(s => s.LootPlayerID == pid);

            if (withItem)
                data = data.Include(s => s.Item);

            return await data.ToListAsync();
        }

        public Subscription CreateSubscription(int itemId, int playerId, long chatId, int priceEvent)
        {
            return new Subscription()
            {
                LootItemID = itemId,
                LootPlayerID = playerId,
                ChatID = chatId,
                PriceEvent = priceEvent
            };
        }

        public async Task AddSubscription(Subscription newSub)
        {
            await this.context.Subscriptions.AddAsync(newSub);
        }

        public void RemoveSubscriptions(ICollection<Subscription> toBeRemoved)
        {
            this.context.Subscriptions.RemoveRange(toBeRemoved);
        }

        public async Task Commit()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task CommitWithIdentities(string dbEntity)
        {
            this.context.Database.OpenConnection();
            try
            {
                this.context.Database.ExecuteSqlCommand(String.Format("SET IDENTITY_INSERT dbo.{0}s ON", dbEntity));
                await this.context.SaveChangesAsync();
                this.context.Database.ExecuteSqlCommand(String.Format("SET IDENTITY_INSERT dbo.{0}s OFF", dbEntity));
            }
            finally
            {
                this.context.Database.CloseConnection();
            }
        }
    }
}
