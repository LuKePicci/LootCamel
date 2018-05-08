using LootCamel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LootCamel.Interfaces
{
    public interface IUserRegistrar
    {
        Task<LootPlayer> GetExistingUser(int userId);
        Task<bool> IsExistingUser(int userId);
        Task RegisterNewUser(int userId, string username);
        Task UpdateRegisteredUser(LootPlayer player, string newUsername);
    }
}
