using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bimaru.Database.DbContexts;

public class ArrayConverter<T> : ValueConverter<T[], string> where T : struct
{
    public ArrayConverter() : base(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
        v => JsonSerializer.Deserialize<T[]>(v, (JsonSerializerOptions)null!)!)
    {
    }
}