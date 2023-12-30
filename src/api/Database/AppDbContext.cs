using RFIDify.RFID.DataTypes;
using RFIDify.Spotify.DataTypes;

namespace RFIDify.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SpotifyCredentials> SpotifyCredentials { get; set; }
    public DbSet<SpotifyAccessToken> SpotifyAccessToken { get; set; }
    public DbSet<RFIDTag> RFIDs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}