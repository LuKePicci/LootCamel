using LootCamel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                switch (upd.Type)
                {
                    case UpdateType.MessageUpdate:
                        Message msg = upd.Message;

                        if (msg.Type == MessageType.TextMessage)
                        {
                            // disallow forwarded commands
                            if (msg.ForwardDate != null)
                            {
                                this.bot.SendMuteTextMessage(msg.Chat.Id, "Sorry, you can't forward commands");
                                return "Forwarded";
                            }

                            return await this.routeMessage(msg);
                        }
                        else if (msg.Type == MessageType.PhotoMessage)
                        {
                            this.bot.SendMuteTextMessage(msg.Chat.Id, "Thx for the Pic");
                            return "Pic";
                        }
                        return "Unmanaged message";

                    case UpdateType.CallbackQueryUpdate:
                        this.bot.AckCallbackQuery(upd.CallbackQuery.Id);
                        return await this.routeCallback(upd.CallbackQuery);

                    default:
                        return "Unmanaged update";
                }

            }
            catch (Exception ex)
            {
                return "fail";
            }
        }

        private async Task<string> routeMessage(Message msg)
        {
            string[] tokens = msg.Text.Split(' ');

            switch (tokens[0].ToLower())
            {
                case "/start":
                    await this.startCommand(msg);
                    break;

                case "/subscribe":
                    await this.subscribeCommand(msg.From, msg.Text);
                    break;

                case "/subscriptions":
                    await this.subscriptionsCommand(msg);
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

            return "Ok";
        }

        private async Task<string> routeCallback(CallbackQuery query)
        {
            string[] tokens = query.Data.Split(' ');

            switch (tokens[0].ToLower())
            {
                case "/subscribe":
                    await this.subscribeCommand(query.From, query.Data);
                    break;

                default:
                    return "Bho";
            }

            return "Ok";
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
                this.bot.SendTextMessage(msg.From.Id, welcomeText);
            }
            else
            {
                // update nickname if changed
                if (player.Nickname != msg.From.Username)
                {
                    player.Nickname = msg.From.Username;
                    await this.repo.Commit();
                }

                this.bot.SendTextMessage(msg.From.Id, String.Format("Welcome back {0}!", msg.From.Username));
            }
        }

        private async Task subscribeCommand(User from, string content)
        {
            // parse the command
            string[] tokens = content.Split(' ');
            string itemName = content.Replace(tokens.First(), "").Replace(tokens.Last(), "").Trim();
            int priceEvent;

            if(!int.TryParse(tokens.Last(), out priceEvent) || priceEvent < 1)
            {
                this.bot.SendMuteTextMessage(from.Id, "Sorry, last token must be a valid price");
                return;
            }

            // item lookup my user's query
            var item = await this.repo.GetLootItemByName(itemName);

            if (item == null)
            {
                this.bot.SendMuteTextMessage(from.Id, "Sorry, I've never seen this item before, maybe it does not exist or has just been introduced or renamed");
                return;
            }

            // check price is above or equal to minimum base
            if(priceEvent < item.BasePrice)
            {
                this.bot.SendMuteTextMessage(from.Id, String.Format("Sorry, this item has a base price of {0}§, noboy will never sell ti for less", item.BasePrice));
                return;
            }

            // insert a new subscription
            await this.repo.AddSubscription(this.repo.CreateSubscription(item.ID, from.Id, from.Id, priceEvent));
            await this.repo.Commit();
            this.bot.SendTextMessage(from.Id, String.Format("Ok, I'll notify you as soon as I find a {0} on sale for {1}§ or less", itemName, priceEvent));
        }
 
        private async Task subscriptionsCommand(Message msg)
        {
            var subCollection = await this.repo.GetSubscriptionsByLootPlayerId(msg.Chat.Id, true);

            if (subCollection.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var sub in subCollection)
                    sb.AppendFormat("> {0} {1}§\r\n", sub.Item.ItemName, sub.PriceEvent);

                this.bot.SendMuteTextMessage(msg.From.Id, sb.ToString());
            }
            else
            {
                this.bot.SendMuteTextMessage(msg.From.Id, "I have no active subscriptions from you");
            }
        }

        private async Task unsubscribeCommand(Message msg)
        {
            this.repo.RemoveSubscriptions(await this.repo.GetSubscriptionsByLootPlayerId(msg.From.Id));
            await this.repo.Commit();
            this.bot.SendMuteTextMessage(msg.From.Id, "Done, you're now subscribed to nothing! ");
        }

        private async Task helpCommand(Message msg)
        {
            string helpText = "Click the / button to see a list of currently supported commands. Here is a small quickref:\r\n\r\n" +
                "/subscribe <item> <price>\r\n" +
                "Subscribe to price decrease alert for an item when it is available for your target price or less\r\n" +
                "Example: /subscribe Pozione Grande 3600\r\n\r\n" +
                "/subscriptions\r\n" +
                "Show all your active subscriptions\r\n\r\n" +
                "/unsubscribe\r\n" +
                "Delete all active subscriptions previously created\r\n\r\n" +
                "/help\r\n" +
                "Show this help";

            this.bot.SendMuteTextMessage(msg.From.Id, helpText);
        }
    }
}
