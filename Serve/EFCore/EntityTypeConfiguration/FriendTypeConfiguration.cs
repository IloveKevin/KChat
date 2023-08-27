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
	public class FriendTypeConfiguration: IEntityTypeConfiguration<Friend>
	{
		public void Configure(EntityTypeBuilder<Friend> builder)
		{
			builder.ToTable("T_Friends");
			builder.HasKey(x => x.Id);
			builder.Property(x => x.UserId).IsRequired();
			builder.Property(x => x.FriendId).IsRequired();
		}
	}
}
