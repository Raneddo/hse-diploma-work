using Microsoft.Data.SqlClient;

namespace ADSD.Backend.Common;

public static class SqlExtensions
{
    public static string? GetStringSafe(this SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    public static DateTimeOffset? GetDateTimeOffsetSafe(this SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTimeOffset(ordinal);
    }
    
    public static int? GetInt32Safe(this SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }
}