using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LootCamel
{
    public class TelegramBotOptions
    {
        public string ApiToken { get; set; }
        public string WebhookEndpoint { get; set; }
        public string HookToken { get; set; }
    }

}
