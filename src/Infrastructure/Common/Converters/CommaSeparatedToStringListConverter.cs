using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace Infrastructure.Common.Converters;

public class CommaSeparatedToStringListConverter : ValueConverter<List<string>, string>
{
    private static readonly Expression<Func<List<string>, string>> ConvertToExpr = x => ConvertTo(x);
    private static readonly Expression<Func<string, List<string>>> ConvertFromExpr = x => ConvertFrom(x);

    public CommaSeparatedToStringListConverter()
   : base(ConvertToExpr, ConvertFromExpr)
    {
    }
    private static string ConvertTo(List<string> list) 
        => string.Join(',', list);

    private static List<string> ConvertFrom(string commaSeparatedValues)
        => commaSeparatedValues.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
}
