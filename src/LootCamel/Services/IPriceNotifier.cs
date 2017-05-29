using LootCamel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LootCamel.Interfaces
{
    public interface IPriceNotifier
    {
        Task SendNotifications(ICollection<Subscription> subs, IDictionary<int, (int, long)> updates);
    }
}
