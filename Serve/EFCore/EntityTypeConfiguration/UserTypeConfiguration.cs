using EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.EntityTypeConfiguration
{
	public class UserTypeConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("T_Users");
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Account).HasMaxLength(12).IsRequired();
			builder.Property(x => x.Password).HasMaxLength(12).IsRequired();
			builder.Property(x => x.NickName).HasMaxLength(12).IsRequired();
		}
	}
}
