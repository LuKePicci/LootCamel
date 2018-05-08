using LootCamel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LootCamel.Controllers
{
    [Route("api/[controller]")]
    public class FellowCommandsController : Controller
    {
        private readonly IPriceProvider priceProvider;

        public FellowCommandsController(IPriceProvider priceProvider)
        {
            this.priceProvider = priceProvider;
        }

        // GET: api/fellowcommands/getavgtextprices
        [HttpGet("GetAvgPrices/Text/{token}")]
        public async Task<string> GetAvgPricesText(string token)
        {
            if (token != this.priceProvider.AccessToken)
            {
                HttpContext.Response.StatusCode = 403;
                return "Not Authorized";
            }

            return await this.priceProvider.GetStreetPricesAsText();
        }
    }
}
