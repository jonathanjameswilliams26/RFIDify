namespace RFIDify;

public static class ConfigureApp
{
	/// <summary>
	/// Configures the HTTP request pipeline.
	/// </summary>
	public static void Configure(this WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI();
		app.UseHttpsRedirection();
	}
}
