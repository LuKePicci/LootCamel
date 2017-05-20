using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Models;

namespace LootCamel.Interfaces
{
    public interface IPriceNotifier
    {
        Task SendNotifications(ICollection<Subscription> subs, IDictionary<int, (int, long)> updates);
    }
}
