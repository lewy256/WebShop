using Microsoft.EntityFrameworkCore;
using OrderApi.Infrastructure;
using Serilog;

namespace OrderApi.Extensions;

public static class MigrationExtensions {
    public static WebApplication MigrateDatabase(this WebApplication webApp) {
        using(var scope = webApp.Services.CreateScope()) {
            using(var appContext = scope.ServiceProvider.GetRequiredService<OrderContext>()) {
                try {
                    appContext.Database.Migrate();
                }
                catch(Exception ex) {
                    Log.Error($"Migration failed: {ex}");
                }
            }
        }
        return webApp;
    }
}
