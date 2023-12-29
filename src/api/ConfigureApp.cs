namespace RFIDify;

public static class ConfigureApp
{
	/// <summary>
	/// Configures the HTTP request pipeline.
	/// </summary>
	public static async Task Configure(this WebApplication app)
	{
		await app.EnsureDatabaseIsCreated();
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseHttpsRedirection();
	}

	/// <summary>
	/// Creates the database if it doesn't exist and applies any pending migrations.
	/// </summary>
	private static async Task EnsureDatabaseIsCreated(this WebApplication app)
	{
		app.Logger.LogInformation("Ensuring database is created...");
		using var scope = app.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await context.Database.MigrateAsync();
		app.Logger.LogInformation("Database was successfully created/updated.");
	}
}
