namespace WatermarkApi.DbContext;


    public class DataDbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataDbContext>();
            context.Database.EnsureCreated();
        }
    }

