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
	public class FriendRequestTypeConfiguration: IEntityTypeConfiguration<FriendRequest>
	{
		public void Configure(EntityTypeBuilder<FriendRequest> builder)
		{
			builder.ToTable("T_FriendRequest");
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.Property(x => x.UserId).IsRequired();
			builder.Property(x => x.FriendId).IsRequired();
		}
	}
}
