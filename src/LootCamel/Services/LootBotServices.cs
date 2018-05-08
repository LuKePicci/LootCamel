using LootCamel.Interfaces;
using LootCamel.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LootCamel.Services
{
    public class LootBotServices : ILootItemsSource, ILootShopExplorer
    {
        private readonly IOptions<LootBotOptions> lootbotOptions;
        private ICollection<JsonLootItem> allItemsCache;

        public string HookToken { get { return lootbotOptions.Value.AccessToken; } }

        public LootBotServices(IOptions<LootBotOptions> lootbotOptions)
        {
            this.lootbotOptions = lootbotOptions;
        }

        // /newapi/<time>
        public async Task<ICollection<JsonLootShopItem>> GetUpdatedShopItems(int minutes)
        {
            JToken shops = (await this.getJsonResponse("/updatedshops/" + minutes.ToString()));
            List<JsonLootShopItem> responseObject = new List<JsonLootShopItem>();

            responseObject.AddRange(JsonConvert.DeserializeObject<ICollection<JsonLootShopItem>>(shops.ToString()));
            
            return responseObject;
        }

        // /shop/<code>
        public async Task<ICollection<JsonLootShopItem>> GetShopItems(long shopId)
        {
            JToken token = (await this.getJsonResponse("/shop/" + shopId.ToString()));
            List<JsonLootShopItem> responseObject = new List<JsonLootShopItem>();

            responseObject.AddRange(JsonConvert.DeserializeObject<ICollection<JsonLootShopItem>>(token.ToString()));

            return responseObject;
        }

        // /items
        public async Task<ICollection<JsonLootItem>> GetAllItems()
        {
            if (this.allItemsCache == null)
            {
                JToken token = (await this.getJsonResponse("/items"));
                List<JsonLootItem> responseObject = new List<JsonLootItem>();
                
                responseObject.AddRange(JsonConvert.DeserializeObject<ICollection<JsonLootItem>>(token.ToString()));

                return this.allItemsCache = responseObject;
            }
            else
            {
                return this.allItemsCache;
            }
        }

        private async Task<JToken> getJsonResponse(string apiQuery)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(this.lootbotOptions.Value.ApiEndpoint + apiQuery) as HttpWebRequest;
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }

                    JObject responseJson = JObject.Parse(await new StreamReader(response.GetResponseStream()).ReadToEndAsync());
                    if (int.Parse(responseJson["code"].ToString()) != 200)
                    {
                        throw new Exception(String.Format("API error {0}", responseJson["code"].ToString()));
                    }

                    return responseJson["res"];
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
