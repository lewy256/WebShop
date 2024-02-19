using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityApi.Extensions;

public static class MigrationExtensions {
    public static WebApplication MigrateDatabase(this WebApplication webApp) {
        using(var scope = webApp.Services.CreateScope()) {
            using(var appContext = scope.ServiceProvider.GetRequiredService<IdentitytContext>()) {
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
