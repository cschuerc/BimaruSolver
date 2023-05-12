using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Bimaru.Database.Entities;

public class DerivedValueGenerator<TEntity, TValue> : ValueGenerator<TValue>
{
    private readonly IDerivedValueGenerator<TEntity, TValue> derivedValueGenerator;

    public DerivedValueGenerator(IDerivedValueGenerator<TEntity, TValue> derivedValueGenerator)
    {
        this.derivedValueGenerator = derivedValueGenerator;
    }

    public override TValue Next(EntityEntry entry)
    {
        if (entry.Entity is not TEntity entity)
        {
            throw new ArgumentException($"This generator is used with type ${entry.Entity.GetType()} instead of ${typeof(TEntity)}", nameof(entry));
        }

        return derivedValueGenerator.GenerateValue(entity);
    }

    public override bool GeneratesTemporaryValues => false;
}