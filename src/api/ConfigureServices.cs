﻿using RFIDify.Api.ExceptionHandlers;
using RFIDify.Shared.Services;
using RFIDify.Spotify.Services;
using Serilog;

namespace RFIDify;

public static class ConfigureServices
{
	/// <summary>
	/// Configures the application services.
	/// </summary>
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.AddSerilog();
		builder.AddSwagger();
		builder.AddDatabase();
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
		builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
		builder.AddSpotify();
		builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
	}

	private static void AddSerilog(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, loggerConfiguration) =>
		{
			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.Enrich.FromLogContext();
		});
	}

	private static void AddSwagger(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
	}

	private static void AddDatabase(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<AppDbContext>(options =>
		{
			var connectionString = builder.Configuration.GetConnectionString("Default");
			options.UseSqlite(connectionString);
		});
	}

	private static void AddSpotify(this WebApplicationBuilder builder)
	{
		builder.Services.AddTransient<ISpotifyAccessTokenProvider, SpotifyAccessTokenProvider>();
		builder.Services.AddTransient<SpotifyWebApiAccessTokenHandler>();

		builder.Services.AddHttpClient<ISpotifyAccountsApi, SpotifyAccountsApi>(client =>
		{
			client.BaseAddress = new Uri(builder.Configuration["Spotify:AccountsApiBaseUrl"]!);
		});

		builder.Services.AddHttpClient<ISpotifyWebApi, SpotifyWebApi>(client =>
		{
			client.BaseAddress = new Uri(builder.Configuration["Spotify:WebApiBaseUrl"]!);
		})
		.AddHttpMessageHandler<SpotifyWebApiAccessTokenHandler>();
	}
}
