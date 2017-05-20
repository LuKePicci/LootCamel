using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LootCamel.Interfaces;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using System.Text;
using System.Security.Cryptography;

namespace LootCamel.Services
{
    public class TelegramBotServices : IBotConnector
    {
        private readonly TelegramBotClient tgapi;
        private readonly IOptions<TelegramBotOptions> tbotOptions;

        public string HookToken { get { return tbotOptions.Value.HookToken; } } 

        public TelegramBotServices(IOptions<TelegramBotOptions> tbotOptions)
        {
            this.tbotOptions = tbotOptions;
            this.tgapi = new TelegramBotClient(tbotOptions.Value.ApiToken);
        }

        public async Task SetWebhookAsync()
        {
            await this.tgapi.SetWebhookAsync(tbotOptions.Value.WebhookEndpoint + "/" + tbotOptions.Value.HookToken);
        }

        public async Task Echo(Message msg)
        {
            await this.tgapi.SendTextMessageAsync(msg.Chat.Id, msg.Text);
        }

        public async Task SendTextMessage(long chatId, string text,
            bool disableWebPagePreview = false, bool disableNotification = false)
        {
            await this.tgapi.SendTextMessageAsync(chatId, text, disableWebPagePreview, disableNotification);
        }

        public async Task SendMuteTextMessage(long chatId, string text)
        {
            await this.tgapi.SendTextMessageAsync(chatId, text, false, true);
        }
    }
}
