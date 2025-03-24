using Application.Common.Extensions;
using Domain.Entities.Medias;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Common.Converters;

public class MediaToDbJsonConverter : ValueConverter<Media, string>
{
    private static readonly JsonSerializerOptions Settings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly Expression<Func<Media, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, Media>> ConvertFromExpr = x => ConvertFrom(x);

    public MediaToDbJsonConverter()
        : base(ConvertToExpr, ConvertFromExpr)
    {
    }

    private static string ConvertTo(Media media) => media.ToJson(Settings);

    private static Media ConvertFrom(string json) => json.Deserialize<Media>()!;
}
