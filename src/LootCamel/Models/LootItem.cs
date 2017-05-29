using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LootCamel.Models
{
    public class LootItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int BasePrice { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<Price> Prices { get; set; }
    }
}
