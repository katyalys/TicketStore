using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data
{
	public class UserConfiguration : IEntityTypeConfiguration<IdentityUser>
	{
		public void Configure(EntityTypeBuilder<IdentityUser> builder)
		{
			builder.HasIndex(x => x.UserName).IsUnique();
			builder.HasIndex(x => x.Email).IsUnique();
			builder.Property(x => x.UserName).HasMaxLength(15).IsRequired();
		}
	}
}
