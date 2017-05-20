using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Models;

namespace LootCamel.Interfaces
{
    public interface ILootCamelRepository
    {
        Task<ICollection<LootItem>> GetKnownLootItems();
        Task<LootItem> GetLootItemByName(string name);
        Task<ISet<LootItem>> CreateUnkownLootItems(ICollection<int> itemIdCollection);
        Task AddItems(ISet<LootItem> toBeSaved);

        Price CreatePrice(int lootItemId, int value);
        Task AddPrices(ICollection<Price> toBeSaved);

        Task<LootPlayer> GetLootPlayerById(int id);
        LootPlayer CreateLootPlayer(int id, string nickname);
        Task AddPlayer(LootPlayer player);
        
        Task<ICollection<Subscription>> GetFilteredSubscriptions(IDictionary<int, (int, long)> itemEvent);
        Task<ICollection<Subscription>> GetSubscriptionsByLootPlayerId(long pid);
        Subscription CreateSubscription(int itemId, int playerId, long chatId, int priceEvent);
        Task AddSubscription(Subscription newSub);
        void RemoveSubscriptions(ICollection<Subscription> toBeRemoved);

        Task Commit();
        Task CommitWithIdentities(string dbEntityName);
    }
}
