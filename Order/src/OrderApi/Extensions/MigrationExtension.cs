using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using Serilog;

namespace OrderApi.Extensions;

public static class MigrationExtension {
    public static WebApplication MigrateDatabase(this WebApplication webApp) {
        using(var scope = webApp.Services.CreateScope()) {
            using(var appContext = scope.ServiceProvider.GetRequiredService<OrderContext>()) {
                try {
                    appContext.Database.Migrate();
                }
                catch(Exception ex) {
                    Log.Error("Migration error");
                    throw;
                }
            }
        }

        return webApp;
    }
}