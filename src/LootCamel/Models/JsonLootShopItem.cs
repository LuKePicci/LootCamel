using System;

namespace LootCamel.Models
{
    public class JsonLootShopItem
    {
        public long code { get; set; }
        public int player_id { get; set; }
        public int item_id { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public DateTime time_end { get; set; }
    }
}