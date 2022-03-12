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

    public IEnumerable<OptionInfo> GetOptionsInfo(int userId, int optionId = 0, SqlConnection connection = null, SqlTransaction transaction = null)
    {
        throw new NotImplementedException();
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
            command.Parameters.AddWithValue("option_id", optionId);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }
        
            using var reader = command.ExecuteReader();

            var optionIdOrdinal = reader.GetOrdinal("id");
            var pollIdOrdinal = reader.GetOrdinal("poll_id");
            var textOrdinal = reader.GetOrdinal("text");
            var countOrdinal = reader.GetOrdinal("count");
            var isChosenOrdinal = reader.GetOrdinal("is_chosen");

            while (reader.Read())
            {
                // var pollId = reader.GetInt32(optionIdOrdinal);
                // var name = reader.GetString(nameOrdinal);
                // var text = reader.GetStringSafe(textOrdinal);
                // var multiChoice = reader.GetBoolean(multiChoiceOrdinal);
                //
                // yield return new PollResponse
                // {
                //     Id = pollId,
                //     Name = name,
                //     Text = text,
                //     MultiChoice = multiChoice
                // };
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
}