using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LootCamel.Models
{
    public class JsonLootItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int value { get; set; }
        public int estimate { get; set; }
    }
}