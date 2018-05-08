namespace LootCamel
{
    public class TelegramBotOptions : RestrictedServiceOptions
    {
        public string ApiToken { get; set; }
        public string WebhookEndpoint { get; set; }
    }

}
