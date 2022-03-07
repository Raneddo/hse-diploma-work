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

    public IEnumerable<AgendaHeader> GetAgendasList(
        int count = int.MaxValue, int offset = 0, SqlConnection connection = null, SqlTransaction transaction = null)
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
        var isActiveOrdinal = reader.GetOrdinal("active");

        while (reader.Read())
        {
            var id = reader.GetInt32(idOrdinal);
            var title = reader.GetString(titleOrdinal);
            var description = reader.GetStringSafe(descriptionOrdinal);
            var isActive = reader.GetBoolean(isActiveOrdinal);

            yield return new AgendaHeader(id, title, description, isActive);
        }
    }

    public AgendaHeader GetAgendaInfo(int agendaId, SqlConnection connection = null, SqlTransaction transaction = null)
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
        var isActiveOrdinal = reader.GetOrdinal("active");

        if (reader.Read())
        {
            var id = reader.GetInt32(idOrdinal);
            var title = reader.GetString(titleOrdinal);
            var description = reader.GetStringSafe(descriptionOrdinal);
            var isActive = reader.GetBoolean(isActiveOrdinal);

            return new AgendaHeader(id, title, description, isActive);
        }

        return null;
    }

    public IEnumerable<Speaker> GetSpeakersByAgenda(int agendaId, SqlConnection connection = null, SqlTransaction transaction = null)
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

    public int AddAgenda(AgendaHeader header, SqlConnection connection = null, SqlTransaction transaction = null)
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

        command.Parameters.AddWithValue("title", header.Title);
        command.Parameters.AddWithValue("description", header.Description);

        using var reader = command.ExecuteReader();
        
        var idOrdinal = reader.GetOrdinal("id");

        if (reader.Read())
        {
            var agendaId = reader.GetInt32(idOrdinal);

            return agendaId;
        }

        return 0;
    }

    public void UpdateAgenda(int id, string title, string description, bool active,
        SqlConnection connection = null, SqlTransaction transaction = null)
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
        command.Parameters.AddWithValue("active", active);

        command.ExecuteNonQuery();
    }

    public void UpdateAgendaSessions(int agendaId, IEnumerable<Session> sessions,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        connection ??= new SqlConnection(_connectionString);
        
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        var values = string.Join(", ", sessions.Select(x => $"({agendaId}, '{x.StartDate}', '{x.EndDate}')"));

        var text = @$"
CREATE TABLE #MyTempTable  
    (
		agenda_id INT,
        start_date datetimeoffset(7),
        end_date datetimeoffset(7)
    );

INSERT INTO #MyTempTable (agenda_id, start_date, end_date)
    VALUES {values};

MERGE [Session] as s
    USING (SELECT agenda_id, start_date, end_date FROM #MyTempTable) as mtt (agenda_id, start_date, end_date)  
    ON (s.agenda_id = mtt.agenda_id AND s.start_date = mtt.start_date)  
    WHEN MATCHED THEN
        UPDATE SET
            start_date = mtt.start_date,
            end_date = mtt.end_date
    WHEN NOT MATCHED by target THEN  
        INSERT (agenda_id, start_date, end_date)  
        VALUES (agenda_id, start_date, end_date)
	WHEN NOT MATCHED by source AND s.agenda_id = (SELECT agenda_id FROM #MyTempTable mtt WHERE agenda_id = mtt.agenda_id) THEN  
        DELETE;

DROP TABLE #MyTempTable;
"; // TODO: Support multiply sessions

        using var command = new SqlCommand(text, connection);
        command.CommandType = CommandType.Text;

        if (transaction != null)
        {
            command.Transaction = transaction;
        }

        command.ExecuteNonQuery();
    }

    public void UpdateSpeakers(int agendaId, IEnumerable<int> speakers,
        SqlConnection connection = null, SqlTransaction transaction = null)
    {
        connection ??= new SqlConnection(_connectionString);
        
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        var values = string.Join(", ", speakers.Select(x => $"({agendaId}, {x})"));

        var text = @$"
CREATE TABLE #MyTempTable  
    (
		agenda_id INT,
        speaker_id INT
    );

INSERT INTO #MyTempTable (agenda_id, speaker_id)
    VALUES {values};

MERGE [AgendaSpeaker] as s
    USING (SELECT agenda_id, speaker_id FROM #MyTempTable) as mtt (agenda_id, speaker_id)  
    ON (s.agenda_id = mtt.agenda_id AND s.speaker_id = mtt.speaker_id)
    WHEN MATCHED THEN
        UPDATE SET
            speaker_id = mtt.speaker_id
    WHEN NOT MATCHED by target THEN  
        INSERT (agenda_id, speaker_id)  
        VALUES (agenda_id, speaker_id)
	WHEN NOT MATCHED by source AND s.agenda_id = (SELECT agenda_id FROM #MyTempTable mtt WHERE agenda_id = mtt.agenda_id) THEN  
        DELETE;

DROP TABLE #MyTempTable;
"; // TODO: Support multiply speakers 

        using var command = new SqlCommand(text, connection);
        command.CommandType = CommandType.Text;

        if (transaction != null)
        {
            command.Transaction = transaction;
        }

        command.ExecuteNonQuery();
    }
}