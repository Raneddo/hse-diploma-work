using Microsoft.Data.SqlClient;

namespace ADSD.Backend.Common;

public static class SqlExtensions
{
    public static string? GetStringSafe(this SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}