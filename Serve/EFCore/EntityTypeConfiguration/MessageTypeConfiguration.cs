using EFCore.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.EntityTypeConfiguration
{
	public class MessageTypeConfiguration : IEntityTypeConfiguration<Message>
	{
		public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Message> builder)
		{
			builder.ToTable("T_Message");
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.Property(x => x.SenderId).IsRequired();
			builder.Property(x => x.ReceiverId).IsRequired();
			builder.Property(x => x.Content).IsRequired();
			builder.Property(x => x.SendTime).IsRequired();
			builder.Property(x => x.Status).IsRequired();
		}
	}
}
