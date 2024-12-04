using GithubRepositoryAnalyzer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GithubRepositoryAnalyzer.Domain.Context.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(255)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<IdValueGenerator>()
            .HasColumnOrder(1);
        
        builder.HasOne(e => e.User)
            .WithMany(e => e.UserTokens)
            .HasForeignKey(k => k.UserId)
            .HasPrincipalKey(p => p.Id)
            .IsRequired();
    }
}