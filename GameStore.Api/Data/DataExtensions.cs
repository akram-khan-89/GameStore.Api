using System;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    // migrating database
    // it keeps the update of the record, means if some change
    // is happened to structure (class), it will sync those changes in database
    public static void MigrateDb(this WebApplication app)
    {
        //creating scope
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

}
