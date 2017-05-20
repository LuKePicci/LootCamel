using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace LootCamel.Interfaces
{
    public interface IBotConnector
    {
        string HookToken { get; }
        Task SetWebhookAsync();
        Task Echo(Message msg);
        Task SendTextMessage(long chatId, string text,
            bool disableWebPagePreview = false, bool disableNotification = false);
        Task SendMuteTextMessage(long chatId, string text);
    }
}
