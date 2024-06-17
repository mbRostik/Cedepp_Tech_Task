using Cedepp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cedepp.Infrastructure.Data.EntityTypeConfiguration
{
    public class UserModelEntityConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CodiceFiscale)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(c => c.CAP)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(c => c.Age)
                .IsRequired();

            builder.Property(c => c.Workplace)
                .IsRequired(false)
                .HasMaxLength(100);
        }
    }
}