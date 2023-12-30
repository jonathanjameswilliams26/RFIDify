using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RFIDify.Spotify.DataTypes;

namespace RFIDify.Database.TableConfigurations;

public class SpotifyCredentialsTableConfiguration : IEntityTypeConfiguration<SpotifyCredentials>
{
	public void Configure(EntityTypeBuilder<SpotifyCredentials> builder)
	{
		builder.HasKey(x => x.ClientId);
	}
}
