namespace MainBot.Database;

internal static class DatabaseExtensions
{
    internal static async Task<int> ApplyChangesAsync(this DatabaseContext database, object? entity = null)
    {
        if (entity is not null)
            database.Update(entity);
        return await database.SaveChangesAsync();
    }

    internal static async Task LogErrorAsync(this Exception e, string? extraInformation = null)
    {
        try
        {
            await using var database = new DatabaseContext();
            var entry = new Models.Logs.ErrorLog
            {
                errorTime = DateTime.UtcNow,
                source = e.Source,
                message = e.Message,
                stackTrace = e.StackTrace,
                extraInformation = extraInformation
            };
            await database.Errors.AddAsync(entry);
            await database.ApplyChangesAsync();
        }
        catch
        {
            Console.WriteLine($"{e.Source} | {e.Message}\n{e.StackTrace}");
        }
    }
}
