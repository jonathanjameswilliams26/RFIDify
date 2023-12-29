using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RFIDify.Spotify.DataTypes;

namespace RFIDify.Database.TableConfigurations;

public class SpotifyAccessTokenTableConfiguration : IEntityTypeConfiguration<SpotifyAccessToken>
{
	public void Configure(EntityTypeBuilder<SpotifyAccessToken> builder)
	{
		builder.HasKey(x => x.RefreshToken);
	}
}
