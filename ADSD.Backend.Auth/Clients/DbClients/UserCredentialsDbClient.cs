using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ADSD.Backend.Auth.Clients.DbClients;

public class UserCredentialsDbClient
{
    private readonly string _authDbConnectionString;

    public UserCredentialsDbClient(IConfiguration configuration)
    {
        _authDbConnectionString = configuration.GetConnectionString("authDbConnectionString")
            .Replace("{password}", Environment.GetEnvironmentVariable("SQL_EPCCONF_PASSWORD"));;
    }
    
    public async Task<int?> GetUserIdByCredentials(string login, string passHash)
    {
        await using var connection = new SqlConnection(_authDbConnectionString);
        connection.Open();

        await using var cmd = new SqlCommand("[dbo].[login]", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Parameters.AddWithValue("login", login);
        cmd.Parameters.AddWithValue("pass_hash", passHash);

        await using var reader = await cmd.ExecuteReaderAsync();

        var userIdOrdinal = reader.GetOrdinal("user_id");

        if (!reader.Read())
            return null;
        
        var userId = reader.GetInt32(userIdOrdinal);
        return userId;
    }

    public async Task AddUserToken(int userId, string token)
    {
        await using var connection = new SqlConnection(_authDbConnectionString);
        connection.Open();

        await using var cmd = new SqlCommand("[dbo].[user_token_add]", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("token", token);

        await cmd.ExecuteNonQueryAsync();
    }
}