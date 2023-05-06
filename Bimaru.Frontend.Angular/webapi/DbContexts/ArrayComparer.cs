using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace webapi.DbContexts;

public class ArrayComparer<T> : ValueComparer<T[]> where T : struct
{
    public ArrayComparer() : base(
        (c1, c2) => c1!.SequenceEqual(c2!),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToArray())
    {
    }
}