using System;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    // migrating database
    // it keeps the update of the record, means if some change is 
    // happened to structure (model classes), it will sync those changes in database schema
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        //creating scope to get service like DbContext from the container
        using var scope = app.Services.CreateScope();

        //Get the GameStoreContext (DbContext) from the service provider
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        
        //Applying any pending migrations to the database equivalent to-
        //running 'dotnet ef database update' using command line
        await dbContext.Database.MigrateAsync();
    }

}
