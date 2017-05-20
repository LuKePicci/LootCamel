using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LootCamel.Data;

namespace LootCamel.Data
{
    public static class DbInitializer
    {
        public static void Initialize(LootCamelContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}