using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LootCamel.Data;
using LootCamel.Interfaces;
using LootCamel.Models;

namespace LootCamel.Services
{
    public class UserManagementService : IUserRegistrar
    {
        private readonly ILootCamelRepository repo;

        public UserManagementService(ILootCamelRepository repo)
        {
            this.repo = repo;
        }

        public async Task<LootPlayer> GetExistingUser(int userId)
        {
            return await this.repo.GetLootPlayerById(userId);
        }

        public async Task<bool> IsExistingUser(int userId)
        {
            return (await this.GetExistingUser(userId)) == null;
        }

        public async Task RegisterNewUser(int userId, string username)
        {
            await this.repo.AddPlayer(this.repo.CreateLootPlayer(userId, username));
            await this.repo.CommitWithIdentities("LootPlayer");
        }

        public async Task UpdateRegisteredUser(LootPlayer player, string newUsername)
        {
            if (player.Nickname != newUsername)
            {
                player.Nickname = newUsername;
                await this.repo.Commit();
            }
        }
    }
}
