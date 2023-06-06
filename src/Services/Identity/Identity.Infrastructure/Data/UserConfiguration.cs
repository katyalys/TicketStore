using Identity.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data
{
	public class UserConfiguration : IEntityTypeConfiguration<IdentityUser>
	{
		public void Configure(EntityTypeBuilder<IdentityUser> builder)
		{
			builder.HasIndex(x => x.UserName).IsUnique();
			builder.HasIndex(x => x.Email).IsUnique();
			//builder.HasIndex(x => x.PasswordHash).IsUnique();
			builder.Property(x => x.UserName).HasMaxLength(15).IsRequired();
		}
	}
}
