﻿using RFIDify.Api;
using Serilog;

namespace RFIDify;

public static class ConfigureApp
{
	/// <summary>
	/// Configures the HTTP request pipeline.
	/// </summary>
	public static async Task Configure(this WebApplication app)
	{
		await app.EnsureDatabaseIsCreated();
		app.UseSerilogRequestLogging();

		// The reason for a empty lambda is due to this issue:
		// https://github.com/dotnet/aspnetcore/issues/51888
		app.UseExceptionHandler(_ => { });

		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseHttpsRedirection();
		app.UseEndpoints();
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
