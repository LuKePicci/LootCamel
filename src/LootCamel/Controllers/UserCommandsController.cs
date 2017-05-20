using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Data;
using LootCamel.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LootCamel.Controllers
{
    [Route("api/[controller]")]
    public class UserCommandsController : Controller
    {
        private readonly IBotConnector bot;
        private readonly ILootCamelRepository repo;

        public UserCommandsController(IBotConnector bot, ILootCamelRepository repo)
        {
            this.bot = bot;
            this.repo = repo;
        }

        // GET: api/usercommands/registerwebhook
        [HttpGet("RegisterWebhook")]
        public async Task<string> RegisterWebhook()
        {
            await this.bot.SetWebhookAsync();
            return "webhook registration sent";
        }

        // POST: api/usercommands/{token}
        [HttpPost("{token}")]
        public async Task<string> Post([FromBody]Update upd, string token)
        {
            if (token != this.bot.HookToken)
            {
                HttpContext.Response.StatusCode = 404;
                return "Not Found";
            }

            try
            {
                if (upd.Type != UpdateType.MessageUpdate)
                    return "Bho";

                Message msg = upd.Message;

                if (msg.Type == MessageType.TextMessage)
                {
                    // disallow forwarded commands
                    if (msg.ForwardDate != null)
                    {
                        this.bot.SendMuteTextMessage(msg.Chat.Id, "Sorry, you can't forward commands");
                        return "Forwarded";
                    }

                    string[] tokens = msg.Text.Split(' ');

                    switch (tokens[0].ToLower())
                    {
                        case "/start":
                            await this.startCommand(msg);
                            break;

                        case "/subscribe":
                            await this.subscribeCommand(msg);
                            break;

                        case "/subscriptions":
                            // register new LootPlayer
                            break;

                        case "/unsubscribe":
                            await this.unsubscribeCommand(msg);
                            break;

                        case "/help":
                            await this.helpCommand(msg);
                            break;

                        case "/echo":
                            // Echo each Message
                            this.bot.Echo(msg);
                            break;

                        default:
                            return "Bho";
                    }
                }
                else if (msg.Type == MessageType.PhotoMessage)
                {
                    this.bot.SendMuteTextMessage(msg.Chat.Id, "Thx for the Pic");
                }

                return "Ok";
            }
            catch (Exception ex)
            {
                return "fail";
            }
        }

        private async Task startCommand(Message msg)
        {
            var player = await this.repo.GetLootPlayerById(msg.From.Id);

            if (player == null)
            {
                // register new LootPlayer
                await this.repo.AddPlayer(this.repo.CreateLootPlayer(msg.From.Id, msg.From.Username));
                await this.repo.CommitWithIdentities("LootPlayer");
                string welcomeText = String.Format("Hi {0}, welcome to Loot Camel!\r\n" +
                    "I'll let you subscribe to price decrease events in any public LootBot market.\r\n" +
                    "See /help for commands description.", msg.From.Username);
                this.bot.SendTextMessage(msg.Chat.Id, welcomeText);
            }
            else
            {
                // update nickname if changed
                if (player.Nickname != msg.From.Username)
                {
                    player.Nickname = msg.From.Username;
                    await this.repo.Commit();
                }

                this.bot.SendTextMessage(msg.Chat.Id, String.Format("Welcome back {0}!", msg.From.Username));
            }
        }

        private async Task subscribeCommand(Message msg)
        {
            // parse the command
            string[] tokens = msg.Text.Split(' ');
            string itemName = msg.Text.Replace(tokens.First(), "").Replace(tokens.Last(), "").Trim();
            int priceEvent;

            if(!int.TryParse(tokens.Last(), out priceEvent) || priceEvent < 1)
            {
                this.bot.SendMuteTextMessage(msg.Chat.Id, "Sorry, last token must be a valid price");
                return;
            }

            // item lookup my user's query
            var item = await this.repo.GetLootItemByName(itemName);

            if (item == null)
            {
                this.bot.SendMuteTextMessage(msg.Chat.Id, "Sorry, I never seen this item, maybe it does not exist or has just been introduced or renamed");
                return;
            }

            // check price is above or equal to minimum base
            if(priceEvent < item.BasePrice)
            {
                this.bot.SendMuteTextMessage(msg.Chat.Id, String.Format("Sorry, this item has a base price of {0}§, noboy will never sell ti for less", item.BasePrice));
                return;
            }

            // insert a new subscription
            await this.repo.AddSubscription(this.repo.CreateSubscription(item.ID, msg.From.Id, msg.Chat.Id, priceEvent));
            await this.repo.Commit();
            this.bot.SendTextMessage(msg.Chat.Id, String.Format("Ok, I'll notify you if somebody puts a {0} on sale for {1}§ or less", itemName, priceEvent));
        }
 
        private async Task unsubscribeCommand(Message msg)
        {
            this.repo.RemoveSubscriptions(await this.repo.GetSubscriptionsByLootPlayerId(msg.From.Id));
            await this.repo.Commit();
            this.bot.SendMuteTextMessage(msg.Chat.Id, "Done, you're now subscribed to nothing! ");
        }

        private async Task helpCommand(Message msg)
        {
            string helpText = "Click the / button to see a list of currently supported commands. Here is a small quickref:\r\n\r\n" +
                "/subscribe <item> <price>\r\n" +
                "Subscribe to price decrease alert for an item when it is available for your target price or less\r\n" +
                "Example: /subscribe Pozione Grande 3600\r\n\r\n" +
                "/unsubscribe\r\n" +
                "Delete all active subscriptions previously created\r\n\r\n" +
                "/help\r\n" +
                "Show this help";

            this.bot.SendMuteTextMessage(msg.Chat.Id, helpText);
        }
    }
}
