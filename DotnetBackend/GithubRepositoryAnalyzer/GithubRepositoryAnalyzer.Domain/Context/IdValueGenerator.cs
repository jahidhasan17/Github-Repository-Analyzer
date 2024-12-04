using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace GithubRepositoryAnalyzer.Domain.Context;

public class IdValueGenerator : ValueGenerator
{
    /// <inheritdoc />
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc />
    protected override object NextValue(EntityEntry entry) => Guid.NewGuid().ToString();
}
