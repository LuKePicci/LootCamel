using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
        InlineKeyboardMarkup Get1rKeyboardMarkup((string, string)[] buttons);
        Task SendInlineKeyboardMessage(long chatId, string text, InlineKeyboardMarkup markup);
        Task AckCallbackQuery(string queryId);
    }
}
