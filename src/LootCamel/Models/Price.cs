using System;

namespace LootCamel.Models
{
    public class Price
    {
        public int ID { get; set; }
        public int LootItemID { get; set; }
        public int Value { get; set; }

        public LootItem Item { get; set; }
        public DateTime Date { get; set; }
    }
}
