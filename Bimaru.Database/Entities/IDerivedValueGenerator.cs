namespace Bimaru.Database.Entities;

public interface IDerivedValueGenerator<in TEntity, out TValue>
{
    TValue GenerateValue(TEntity entity);
}