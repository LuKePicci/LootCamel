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