namespace LootCamel.Models
{
    public class Subscription
    {
        public int ID { get; set; }
        public int LootItemID { get; set; }
        public int PriceEvent { get; set; }
        public int LootPlayerID { get; set; }
        public long ChatID { get; set; }

        public LootItem Item { get; set; }
        public LootPlayer Subscriber { get; set; }
    }
}
