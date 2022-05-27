using System.Data;
using ADSD.Backend.App.Models;
using Microsoft.Data.SqlClient;

namespace ADSD.Backend.App.Clients;

public class SessionTokenDbClient
{
    private readonly IConfiguration _configuration;
    private readonly string _authDbConnectionString;

    public SessionTokenDbClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _authDbConnectionString = Environment.GetEnvironmentVariable("SQL_AUTH_CONNECTION_STRING");
    }

    public async Task<AuthData> GetUserByToken(string token)
    {
        if (token == null)
        {
            return null;
        }
        
        await using var connection = new SqlConnection(_authDbConnectionString);
        connection.Open();

        await using var cmd = new SqlCommand("[dbo].[user_by_token_get]", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Parameters.AddWithValue("token", token);

        await using var reader = await cmd.ExecuteReaderAsync();

        var userNameOrdinal = reader.GetOrdinal("username");
        var groupsOrdinal = reader.GetOrdinal("groups");

        if (!reader.Read())
            return null;
        
        var userName = reader.GetString(userNameOrdinal);
        var groups = reader.GetString(groupsOrdinal).Split(',');
        return new AuthData(userName, groups);
    }
}