using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LootCamel.Models
{
    public class LootPlayer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Nickname { get; set; }
    }
}

