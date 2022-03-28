using System.Data;
using ADSD.Backend.App.Json;
using ADSD.Backend.App.Models;
using ADSD.Backend.Common;
using Microsoft.Data.SqlClient;

namespace ADSD.Backend.App.Clients;

public class AppDbClient
{
    public AppDbClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("appDbConnectionString")
            .Replace("{password}", Environment.GetEnvironmentVariable("SQL_EPCCONF_PASSWORD"));
    }
    
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;

    public (SqlConnection Connection, SqlTransaction Transaction) BeginTransaction()
    {
        var connection = new SqlConnection(_connectionString);
        
        var transaction = connection.BeginTransaction(IsolationLevel.Snapshot);
        
        return (connection, transaction);
    }

    public IEnumerable<AgendaResponse> GetAgendasList(
        int count = int.MaxValue, int offset = 0, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[agenda_list]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            // command.Parameters.AddWithValue("count", count);
            // command.Parameters.AddWithValue("offset", offset);

            var reader = command.ExecuteReader();

            var idOrdinal = reader.GetOrdinal("id");
            var titleOrdinal = reader.GetOrdinal("title");
            var descriptionOrdinal = reader.GetOrdinal("description");
            var startDateOrdinal = reader.GetOrdinal("start_date");
            var endDateOrdinal = reader.GetOrdinal("end_date");

            while (reader.Read())
            {
                var id = reader.GetInt32(idOrdinal);
                var title = reader.GetString(titleOrdinal);
                var description = reader.GetStringSafe(descriptionOrdinal);
                var startDate = reader.GetDateTimeOffsetSafe(startDateOrdinal);
                var endDate = reader.GetDateTimeOffsetSafe(endDateOrdinal);

                yield return new AgendaResponse()
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                
                };
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public AgendaResponse GetAgendaInfo(int agendaId, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[agenda_get]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("id", agendaId);

            var reader = command.ExecuteReader();

            var idOrdinal = reader.GetOrdinal("id");
            var titleOrdinal = reader.GetOrdinal("title");
            var descriptionOrdinal = reader.GetOrdinal("description");
            var startDateOrdinal = reader.GetOrdinal("start_date");
            var endDateOrdinal = reader.GetOrdinal("end_date");

            if (reader.Read())
            {
                var id = reader.GetInt32(idOrdinal);
                var title = reader.GetString(titleOrdinal);
                var description = reader.GetStringSafe(descriptionOrdinal);
                var startDate = reader.GetDateTimeOffsetSafe(startDateOrdinal);
                var endDate = reader.GetDateTimeOffsetSafe(endDateOrdinal);

                return new AgendaResponse()
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                
                };
            }

            return null;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public IEnumerable<Speaker> GetSpeakersByAgenda(int agendaId, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[speakers_get]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("agenda_id", agendaId);

            using var reader = command.ExecuteReader();

            var userIdOrdinal = reader.GetOrdinal("id");
            var firstNameOrdinal = reader.GetOrdinal("first_name");
            var lastNameOrdinal = reader.GetOrdinal("last_name");
        
            while (reader.Read())
            {
                var userId = reader.GetInt32(userIdOrdinal);
                var firstName = reader.GetString(firstNameOrdinal);
                var lastName = reader.GetString(lastNameOrdinal);

                yield return new Speaker(userId, firstName, lastName);
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public IEnumerable<int> GetPollsByAgenda(int agendaId, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_by_agenda]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("agenda_id", agendaId);

            using var reader = command.ExecuteReader();

            var userIdOrdinal = reader.GetOrdinal("id");

            while (reader.Read())
            {
                var pollId = reader.GetInt32(userIdOrdinal);

                yield return pollId;
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public int CreateAgenda(UpdateAgendaRequest req, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[agenda_add]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("title", req.Title);
            command.Parameters.AddWithValue("description", req.Description);
            command.Parameters.AddWithValue("start_date", req.StartDate);
            command.Parameters.AddWithValue("end_date", req.EndDate);

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                var agendaId = reader.GetInt32(idOrdinal);

                return agendaId;
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public void UpdateAgenda(int id, string title, string description,
        DateTimeOffset startDate, DateTimeOffset endDate, bool active,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[agenda_edit]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("title", title);
            command.Parameters.AddWithValue("description", description);
            command.Parameters.AddWithValue("start_date", startDate);
            command.Parameters.AddWithValue("end_date", endDate);
            command.Parameters.AddWithValue("active", active);

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public void SetPollAgenda(int agendaId, int pollId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_agenda_set]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("agenda_id", agendaId);
            command.Parameters.AddWithValue("poll_id", pollId);

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public void UpdateSpeakers(int agendaId, IEnumerable<int> speakers,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var values = string.Join(", ", speakers?.Select(x => $"({agendaId}, {x})") ?? Array.Empty<string>());
            if (string.IsNullOrWhiteSpace(values))
            {
                return;
            }

            var text = @$"
DELETE FROM AgendaSpeaker
    WHERE @agenda_id = agenda_id

INSERT INTO AgendaSpeaker (agenda_id, speaker_id)
    VALUES {values};

";

            using var command = new SqlCommand(text, connection);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@agenda_id", agendaId);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public IEnumerable<PollResponse> GetPolls(SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_list]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }
        
            using var reader = command.ExecuteReader();

            var pollIdOrdinal = reader.GetOrdinal("id");
            var nameOrdinal = reader.GetOrdinal("name");
            var textOrdinal = reader.GetOrdinal("text");
            var agendaIdOrdinal = reader.GetOrdinal("agenda_id");
            var multiChoiceOrdinal = reader.GetOrdinal("multi_choice");

            while (reader.Read())
            {
                var pollId = reader.GetInt32(pollIdOrdinal);
                var name = reader.GetString(nameOrdinal);
                var text = reader.GetStringSafe(textOrdinal);
                var agendaId = reader.GetInt32Safe(agendaIdOrdinal);
                var multiChoice = reader.GetBoolean(multiChoiceOrdinal);

                yield return new PollResponse
                {
                    Id = pollId,
                    Name = name,
                    Text = text,
                    AgendaId = agendaId,
                    MultiChoice = multiChoice
                };
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public IEnumerable<PollOptionResponse> GetOptionsByPoll(int pollId, int userId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_options]", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("user_id", userId);
            command.Parameters.AddWithValue("poll_id", pollId);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }
        
            using var reader = command.ExecuteReader();

            var idOrdinal = reader.GetOrdinal("id");
            var textOrdinal = reader.GetOrdinal("text");
            var countOrdinal = reader.GetOrdinal("cnt");
            var checkedByUserOrdinal = reader.GetOrdinal("checked_by_user");

            while (reader.Read())
            {
                var id = reader.GetInt32(idOrdinal);
                var text = reader.GetStringSafe(textOrdinal);
                var count = reader.GetInt32(countOrdinal);
                var checkedByUser = reader.GetBoolean(checkedByUserOrdinal);

                yield return new PollOptionResponse()
                {
                    Id = id,
                    Text = text,
                    Count = count,
                    IsChosen = checkedByUser
                };
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public PollResponse GetPollById(int pollId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_get]", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("poll_id", pollId);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }
        
            using var reader = command.ExecuteReader();

            var pollIdOrdinal = reader.GetOrdinal("id");
            var nameOrdinal = reader.GetOrdinal("name");
            var textOrdinal = reader.GetOrdinal("text");
            var agendaIdOrdinal = reader.GetOrdinal("agenda_id");
            var multiChoiceOrdinal = reader.GetOrdinal("multi_choice");

            if (reader.Read())
            {
                var id = reader.GetInt32(pollIdOrdinal);
                var name = reader.GetString(nameOrdinal);
                var text = reader.GetStringSafe(textOrdinal);
                var agendaId = reader.GetInt32Safe(agendaIdOrdinal);
                var multiChoice = reader.GetBoolean(multiChoiceOrdinal);

                return new PollResponse
                {
                    Id = id,
                    Name = name,
                    Text = text,
                    AgendaId = agendaId,
                    MultiChoice = multiChoice
                };
            }

            return null;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public int CreatePoll(string name, string text, bool multiChoice,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_add]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("text", text);
            command.Parameters.AddWithValue("multi_choice", multiChoice);

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                var agendaId = reader.GetInt32(idOrdinal);

                return agendaId;
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public int AddOption(int pollId, string optionText,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_option_add]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("poll_id", pollId);
            command.Parameters.AddWithValue("text", optionText);

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                var agendaId = reader.GetInt32(idOrdinal);

                return agendaId;
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public void VotePollOptions(int userId, int optionId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_option_vote]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("option_id", optionId);
            command.Parameters.AddWithValue("user_id", userId);
            
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void UnvotePollOptions(int userId, int optionId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_option_unvote]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("option_id", optionId);
            command.Parameters.AddWithValue("user_id", userId);
            
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public void DeletePoll(int pollId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[poll_delete]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.Parameters.AddWithValue("poll_id", pollId);
            
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public IEnumerable<UserBaseInfo> GetUsers(SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[user_baseinfo_list]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");
            var fnOrdinal = reader.GetOrdinal("first_name");
            var lnOrdinal = reader.GetOrdinal("last_name");
            var fullNameOrdinal = reader.GetOrdinal("full_name");
            var prefixOrdinal = reader.GetOrdinal("prefix");
            var emailOrdinal = reader.GetOrdinal("email");
            var organizationOrdinal = reader.GetOrdinal("organization_name");
            var appStatusOrdinal = reader.GetOrdinal("application_status");
            var isActiveOrdinal = reader.GetOrdinal("is_active");

            while (reader.Read())
            {
                var userId = reader.GetInt32(idOrdinal);
                var prefix = reader.GetStringSafe(prefixOrdinal);
                var firstName = reader.GetStringSafe(fnOrdinal);
                var lastName = reader.GetStringSafe(lnOrdinal);
                var fullName = reader.GetStringSafe(fullNameOrdinal)
                    ?? NamesToFullName(prefix, firstName, lastName);
                var organizationName = reader.GetStringSafe(organizationOrdinal);
                var email = reader.GetStringSafe(emailOrdinal);
                var applicationStatus = reader.GetStringSafe(appStatusOrdinal);
                var isActive = reader.GetBoolean(isActiveOrdinal);

                yield return new UserBaseInfo()
                {
                    Id = userId,
                    Prefix = prefix,
                    FirstName = firstName,
                    LastName = lastName,
                    FullName = fullName,
                    Organization = organizationName,
                    Email = email,
                    IsActive = isActive,
                    ApplicationStatus = applicationStatus
                };
            }
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public UserFullInfo GetUserById(int id,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[user_get_by_id]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@id", id);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");
            var fnOrdinal = reader.GetOrdinal("first_name");
            var lnOrdinal = reader.GetOrdinal("last_name");
            var fullNameOrdinal = reader.GetOrdinal("full_name");
            var prefixOrdinal = reader.GetOrdinal("prefix");
            var emailOrdinal = reader.GetOrdinal("email");
            var organizationOrdinal = reader.GetOrdinal("organization_name");
            var appStatusOrdinal = reader.GetOrdinal("application_status");
            var isActiveOrdinal = reader.GetOrdinal("is_active");

            if (reader.Read())
            {
                var userId = reader.GetInt32(idOrdinal);
                var prefix = reader.GetStringSafe(prefixOrdinal);
                var firstName = reader.GetStringSafe(fnOrdinal);
                var lastName = reader.GetStringSafe(lnOrdinal);
                var fullName = reader.GetStringSafe(fullNameOrdinal)
                    ?? NamesToFullName(prefix, firstName, lastName);
                var organizationName = reader.GetStringSafe(organizationOrdinal);
                var email = reader.GetStringSafe(emailOrdinal);
                var applicationStatus = reader.GetStringSafe(appStatusOrdinal);
                var isActive = reader.GetBoolean(isActiveOrdinal);

                return new UserFullInfo()
                {
                    Id = userId,
                    Prefix = prefix,
                    FirstName = firstName,
                    LastName = lastName,
                    FullName = fullName,
                    Organization = organizationName,
                    Email = email,
                    IsActive = isActive,
                    ApplicationStatus = applicationStatus
                };
            }

            return null;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection.Close();
            }
        }
    }
    
    public int RegisterUser(string userName, string passHash,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[auth_register]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@username", userName);
            command.Parameters.AddWithValue("@pass_hash", passHash);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                var userId = reader.GetInt32(idOrdinal);

                return userId;
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public InfoJson GetInfoByKey(string key,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[info_get]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@key", key);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();

            var textOrdinal = reader.GetOrdinal("text");

            if (reader.Read())
            {
                var text = reader.GetStringSafe(textOrdinal);
                return new InfoJson()
                {
                    Key = key,
                    Text = text,
                };
            }

            return new InfoJson()
            {
                Key = key,
                Text = string.Empty,
            };
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public int CreateUser(UserFullInfo userFullInfo,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[user_add]", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (string.IsNullOrWhiteSpace(userFullInfo.FullName))
            {
                userFullInfo.FullName =
                    NamesToFullName(userFullInfo.Prefix, userFullInfo.FirstName, userFullInfo.LastName);
            }
            
            command.Parameters.AddWithValue("@id", userFullInfo.Id);
            command.Parameters.AddWithValue("@first_name", userFullInfo.FirstName);
            command.Parameters.AddWithValue("@last_name", userFullInfo.LastName);
            command.Parameters.AddWithValue("@full_name", userFullInfo.FullName);
            command.Parameters.AddWithValue("@prefix", userFullInfo.Prefix);
            command.Parameters.AddWithValue("@email", userFullInfo.Email);
            command.Parameters.AddWithValue("@organization_name", userFullInfo.Organization);
            command.Parameters.AddWithValue("@application_status", userFullInfo.ApplicationStatus);
            command.Parameters.AddWithValue("@is_active", userFullInfo.IsActive);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();
        
            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                var userId = reader.GetInt32(idOrdinal);

                return userId;
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public int FindUserByEmail(string email,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[user_email_find]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@email", email);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();

            var idOrdinal = reader.GetOrdinal("id");

            if (reader.Read())
            {
                return reader.GetInt32(idOrdinal);
            }

            return 0;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void DeleteAgenda(int id,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[agenda_delete]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@id", id);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public AuthUser GetAuthUser(int userId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[auth_get]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@id", userId);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            using var reader = command.ExecuteReader();

            var idOrdinal = reader.GetOrdinal("user_id");
            var userNameOrdinal = reader.GetOrdinal("username");
            var isActiveOrdinal = reader.GetOrdinal("is_activated");

            if (reader.Read())
            {
                var id = reader.GetInt32(idOrdinal);
                var userName = reader.GetStringSafe(userNameOrdinal);
                var isActivate = reader.GetBoolean(isActiveOrdinal);

                return new AuthUser()
                {
                    Id = id,
                    UserName = userName,
                    IsActive = isActivate,
                };
            }

            return null;
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void MergeInfo(string key, string text,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[info_merge]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@text", text);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void AuthActivate(int userId,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[auth_activate]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@user_id", userId);
            
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void AuthUpdate(int userId, string userName, string passHash,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[auth_edit]", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@id", userId);
            command.Parameters.AddWithValue("@username", userName);
            command.Parameters.AddWithValue("@pass_hash", passHash);
            
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public void UpdateUser(int id, UserFullInfo userFullInfo,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = new SqlCommand("[dbo].[user_update]", connection);
            command.CommandType = CommandType.StoredProcedure;
            
            if (string.IsNullOrWhiteSpace(userFullInfo.FullName))
            {
                userFullInfo.FullName =
                    NamesToFullName(userFullInfo.Prefix, userFullInfo.FirstName, userFullInfo.LastName);
            }

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@first_name", userFullInfo.FirstName);
            command.Parameters.AddWithValue("@last_name", userFullInfo.LastName);
            command.Parameters.AddWithValue("@full_name", userFullInfo.FullName);
            command.Parameters.AddWithValue("@prefix", userFullInfo.Prefix);
            command.Parameters.AddWithValue("@email", userFullInfo.Email);
            command.Parameters.AddWithValue("@organization_name", userFullInfo.Organization ?? string.Empty);
            command.Parameters.AddWithValue("@application_status", userFullInfo.ApplicationStatus);
            command.Parameters.AddWithValue("@is_active", userFullInfo.IsActive);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }
    
    public int GetUserIdByCredentials(string login, string passHash)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var cmd = new SqlCommand("[dbo].[login]", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        
        cmd.Parameters.AddWithValue("login", login);
        cmd.Parameters.AddWithValue("pass_hash", passHash);

        using var reader = cmd.ExecuteReader();

        var userIdOrdinal = reader.GetOrdinal("user_id");

        if (!reader.Read())
            return 0;
        
        var userId = reader.GetInt32(userIdOrdinal);
        return userId;
    }
    
    public void UpdateUserRoles(int id, List<string> roles,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        var connectionNeedClose = connection == null;
        try
        {
            connection ??= new SqlConnection(_connectionString);
        
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var rolesString = roles
                .Select(x =>
                {
                    _ = Enum.TryParse<UserRole>(x, out var roleId);
                    return (int)roleId;
                })
                .Select(x => $"({x}, {id})");

            var text = @$"
DELETE FROM [UserGroup]
    WHERE user_id = {id};

INSERT INTO [UserGroup]
    ([group], user_id) 
    VALUES {string.Join(",", rolesString)};
";

            using var command = new SqlCommand(text, connection);
            command.CommandType = CommandType.Text;
            
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.ExecuteNonQuery();
        }
        finally
        {
            if (connectionNeedClose)
            {
                connection?.Close();
            }
        }
    }

    public IEnumerable<UserRole> GetRolesByUserId(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var cmd = new SqlCommand("[dbo].[role_by_user]", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", userId);

        using var reader = cmd.ExecuteReader();

        var groupOrdinal = reader.GetOrdinal("group");

        while (reader.Read())
        {
            yield return (UserRole)reader.GetInt32(groupOrdinal);
        }
    }

    private static string NamesToFullName(string prefix, string firstName, string lastName)
    {
        var fullNameList = new List<string>();
        if (!string.IsNullOrWhiteSpace(prefix))
        {
            fullNameList.Add(prefix);
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            fullNameList.Add(firstName);
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            fullNameList.Add(lastName);
        }

        return string.Join(" ", fullNameList);
    }
}