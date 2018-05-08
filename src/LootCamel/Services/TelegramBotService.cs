using LootCamel.Interfaces;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LootCamel.Services
{
    public class TelegramBotService : IBotConnector
    {
        private readonly TelegramBotClient tgapi;
        private readonly IOptions<TelegramBotOptions> tbotOptions;

        public string HookToken { get { return tbotOptions.Value.AccessToken; } } 

        public TelegramBotService(IOptions<TelegramBotOptions> tbotOptions)
        {
            this.tbotOptions = tbotOptions;
            this.tgapi = new TelegramBotClient(tbotOptions.Value.ApiToken);
        }

        public async Task SetWebhookAsync()
        {
            await this.tgapi.SetWebhookAsync(tbotOptions.Value.WebhookEndpoint + "/" + tbotOptions.Value.AccessToken);
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

        public InlineKeyboardMarkup Get1rKeyboardMarkup((string, string)[] buttons)
        {
            var markup = new InlineKeyboardMarkup();
            markup.InlineKeyboard = new InlineKeyboardButton[1][];
            markup.InlineKeyboard[0] = new InlineKeyboardButton[buttons.Length];

            for (int i = 0; i < buttons.Length; i++)
                markup.InlineKeyboard[0][i] = new InlineKeyboardButton(buttons[i].Item1, buttons[i].Item2);

            return markup;
        }

        public async Task SendInlineKeyboardMessage(long chatId, string text, InlineKeyboardMarkup markup)
        {            
            await this.tgapi.SendTextMessageAsync(chatId, text, true, false, 0, markup);
        }

        public async Task AckCallbackQuery(string queryId)
        {
            await this.tgapi.AnswerCallbackQueryAsync(queryId, null, false, null, 120);
        }
    }
}
