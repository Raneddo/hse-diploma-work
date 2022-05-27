create table Agenda
(
    id          int identity
        constraint PK_Agenda
        primary key,
    title       nvarchar(64)                  not null,
    description nvarchar(256),
    active      bit
        constraint DF_Agenda_active default 1 not null,
    start_date  datetimeoffset,
    end_date    datetimeoffset
)
    go

create table AgendaSpeaker
(
    id         int identity
        constraint PK_AgendaSpeaker
        primary key,
    agenda_id  int not null,
    speaker_id int not null
)
    go

create table BasicToken
(
    token   nvarchar(128) not null,
    user_id int           not null
)
    go

create table Chat
(
    id   bigint identity
        constraint Chat_pk
        primary key,
    type smallint,
    name nvarchar(64) not null
)
    go

create table ChatParticipants
(
    chat_id bigint not null,
    user_id int    not null,
    constraint ChatParticipants_pk
        primary key (user_id, chat_id)
)
    go

create table Info
(
    [key] nvarchar(128) not null
    constraint Info_pk
    primary key,
    text  nvarchar(max)
    )
    go

create table PersonalChat
(
    first_user_id  int not null,
    second_user_id int not null,
    chat_id        bigint
        constraint PersonalChat_Chat_id_fk
            references Chat
            on delete cascade,
    constraint PersonalChat_pk
        primary key (first_user_id, second_user_id)
)
    go

create table Poll
(
    id           int identity
        constraint poll_pk
        primary key,
    name         nvarchar(64) not null,
    text         nvarchar(max),
    multi_choice bit default 0,
    agenda_id    int
)
    go

create table PollOption
(
    id      int identity
        constraint PollOption_pk
        primary key,
    poll_id int not null
        constraint PollOption_Poll_id_fk
            references Poll
            on delete cascade,
    text    nvarchar(256)
)
    go

create table Session
(
    id         int identity
        constraint PK_Session
        primary key,
    agenda_id  int            not null,
    start_date datetimeoffset not null,
    end_date   datetimeoffset
)
    go

create table [User]
(
    id                 int                                  not null
    constraint PK_User
    primary key,
    first_name         nvarchar(64)                         not null,
    last_name          nvarchar(64)                         not null,
    prefix             nvarchar(10),
    full_name          nvarchar(256),
    organization_name  nvarchar(128),
    email              nvarchar(128),
    is_active          bit          default 0               not null,
    application_status nvarchar(64) default 'not activated' not null
    )
    go

create table ChatMessage
(
    chat_id    bigint         not null
        constraint ChatMessage_Chat_id_fk
            references Chat
            on delete cascade,
    user_id    int
        constraint ChatMessage_User_id_fk
            references [User],
    message_id bigint identity,
    message    nvarchar(max),
    created_at datetimeoffset not null,
    constraint ChatMessage_pk
        unique (chat_id, user_id, message_id)
)
    go

create table OptionCheck
(
    option_id int not null,
    user_id   int not null
        constraint OptionCheck_User_id_fk
            references [User]
        on delete cascade
)
    go

create unique index OptionCheck_option_id_user_id_uindex
    on OptionCheck (option_id asc, user_id desc)
    go

create table UserAdditional
(
    id                   int not null
        constraint UserAdditional_pk
            primary key,
    job_title            nvarchar(128),
    bio                  nvarchar(max),
    nationality          nvarchar(128),
    document             nvarchar(128),
    is_local             bit default 1,
    vip                  bit default 0,
    need_transportation  bit default 0,
    transport_comments   nvarchar(max),
    airline_name         nvarchar(128),
    plane_ticket_number  nvarchar(128),
    hotel_booking_number nvarchar(128),
    hotel_checkin        datetimeoffset,
    hotel_checkout       datetimeoffset
)
    go

create table UserAuthData
(
    user_id      int identity,
    username     nvarchar(50)                             not null,
    pass_hash    nvarchar(64),
    is_activated bit
        constraint DF_UserAuthData_is_activated default 0 not null
)
    go

create table UserGroup
(
    id      int identity,
    [group] int not null,
    user_id int not null
)
    go

CREATE PROCEDURE [dbo].[agenda_add]
    @title NVARCHAR(64),
    @description NVARCHAR(256),
    @start_date DATETIMEOFFSET,
    @end_date DATETIMEOFFSET
AS
BEGIN
    INSERT INTO Agenda
    (title, description, active, start_date, end_date)
    OUTPUT
        inserted.id
    VALUES
        (@title, @description, 0, @start_date, @end_date)
END
go

CREATE PROCEDURE [dbo].[agenda_delete]
@id INT
AS
BEGIN
    DELETE [Agenda]
    WHERE id = @id
end
go

CREATE PROCEDURE [dbo].[agenda_edit]
    @id INT,
    @title NVARCHAR(64),
    @description NVARCHAR(256),
    @start_date DATETIMEOFFSET,
    @end_date DATETIMEOFFSET,
    @active BIT
AS
BEGIN
    UPDATE Agenda
    SET
        title = @title,
        description = @description,
        start_date = @start_date,
        end_date = @end_date,
        active = @active
    WHERE
            id = @id;
END
go

CREATE PROCEDURE [dbo].[agenda_get]
@id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT TOP(1) id
                , title
                , description
                , active
                , start_date
                , end_date
    FROM Agenda WHERE id = @id;
END
go

CREATE PROCEDURE [dbo].[agenda_list]

AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT a.id
         , a.title
         , a.description
         , a.active
         , a.start_date
         , a.end_date
    FROM Agenda a;
END
go

CREATE PROCEDURE [dbo].[auth_activate]
@user_id INT
AS
BEGIN
    UPDATE [UserAuthData]
    SET is_activated = 1
    WHERE user_id = @user_id;

    UPDATE [User]
    SET is_active = 1,
        application_status = 'activated'
    WHERE id = @user_id;
end
go

CREATE PROCEDURE [dbo].[auth_edit]
    @id INT,
    @username NVARCHAR(64),
    @pass_hash NVARCHAR(64)
AS
BEGIN
    UPDATE [UserAuthData]
    SET
        username = @username,
        pass_hash = @pass_hash
    WHERE user_id = @id
end
go

CREATE PROCEDURE [dbo].[auth_get]
@id INT
AS
BEGIN
    SELECT TOP(1) user_id, username, is_activated FROM UserAuthData
    WHERE user_id = @id
end
go

CREATE PROCEDURE [dbo].[auth_register]
    @username NVARCHAR(64),
    @pass_hash NVARCHAR(64)
AS
BEGIN
    INSERT INTO [UserAuthData]
    (username, pass_hash, is_activated)
    OUTPUT
        inserted.user_id as id
    VALUES (@username, @pass_hash, 0)
end
go

CREATE PROCEDURE chat_by_id
@chat_id BIGINT
AS
BEGIN
    SELECT TOP(1) id chat_id, type chat_type, name chat_name
    FROM Chat
    WHERE id = @chat_id
end
go

CREATE PROCEDURE chat_message_send
    @chat_id BIGINT,
    @user_id INT,
    @message NVARCHAR(MAX),
    @created_at DATETIMEOFFSET
AS
BEGIN
    INSERT INTO ChatMessage
    (chat_id, user_id, message, created_at)
    VALUES (@chat_id, @user_id, @message, @created_at)

    SELECT CONVERT(BIGINT, SCOPE_IDENTITY()) message_id
end
go

CREATE PROCEDURE chat_messages_get
    @chat_id BIGINT,
    @after_message_id BIGINT,
    @count BIGINT
AS
BEGIN
    SELECT TOP (@count) cm.chat_id
                      , cm.user_id
                      , u.full_name
                      , cm.created_at
                      , cm.message_id
                      , cm.message
    FROM ChatMessage cm
             left join [User] u on u.id = cm.user_id
    WHERE @chat_id = chat_id
      AND message_id > @after_message_id
    ORDER BY cm.message_id
end
go

CREATE PROCEDURE chat_new
    @chat_name NVARCHAR(128),
    @chat_type smallint
AS
BEGIN
    INSERT INTO Chat
    (type, name)
    VALUES (@chat_type, @chat_name);

    SELECT CONVERT(bigint, SCOPE_IDENTITY()) chat_id
end
go

CREATE PROCEDURE chat_participants
@chat_id BIGINT
AS
BEGIN
    SELECT user_id FROM ChatParticipants
    WHERE @chat_id = chat_id
end
go

CREATE PROCEDURE chat_personal_exists
    @first_user_id INT,
    @second_user_id INT
AS
BEGIN
    SELECT TOP(1) chat_id
    FROM PersonalChat
    WHERE @first_user_id = first_user_id AND @second_user_id = second_user_id
end
go

CREATE PROCEDURE chat_personal_link
    @first_user_id INT,
    @second_user_id INT,
    @chat_id BIGINT
AS
BEGIN
    INSERT INTO PersonalChat
    (first_user_id, second_user_id, chat_id)
    VALUES
        (@first_user_id, @second_user_id, @chat_id)
end
go

CREATE PROCEDURE chat_user_add
    @user_id INT,
    @chat_id BIGINT
AS
BEGIN
    MERGE ChatParticipants as tgt
    USING (SELECT @chat_id, @user_id) AS src (chat_id, user_id)
    ON (tgt.chat_id = src.chat_id AND tgt.user_id = src.user_id)
    WHEN NOT MATCHED THEN
        INSERT (chat_id, user_id)
        VALUES (chat_id, user_id);

end
go

CREATE PROCEDURE chats_by_user
@user_id int
AS
BEGIN
    SELECT c.id chat_id, c.name chat_name, c.type chat_type
    FROM Chat c
             inner join ChatParticipants CP on c.id = CP.chat_id
    WHERE cp.user_id = @user_id AND c.type <> 3
    UNION
    SELECT c.id chat_id, c.name chat_name, c.type chat_type
    FROM Chat c
    WHERE c.type = 3
end
go

CREATE PROCEDURE [dbo].[info_get]
@key NVARCHAR(64)
AS
BEGIN
    SELECT TOP(1) [key], [text] FROM [Info]
    WHERE [key] = @key
END
go

CREATE PROCEDURE [dbo].[info_merge]
    @key NVARCHAR(64),
    @text NVARCHAR(MAX)
AS
BEGIN
    MERGE [Info] AS tgt
    USING (SELECT @key, @text) as src ([key], [text])
    ON (tgt.[key] = src.[key])
    WHEN MATCHED THEN
        UPDATE SET text = src.text
    WHEN NOT MATCHED THEN
        INSERT ([key], [text])
        VALUES (src.[key], src.text);
END
go


CREATE PROCEDURE [dbo].[login]
    @login NVARCHAR(MAX),
    @pass_hash NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT user_id FROM dbo.UserAuthData
    WHERE @login = [username] AND @pass_hash = pass_hash
END
go

CREATE PROCEDURE personal_chat_name_by_id
@user_id INT
AS
BEGIN
    SELECT chat_id, u.full_name
    FROM PersonalChat pc
             inner join [User] u ON pc.second_user_id = u.id
    WHERE first_user_id = @user_id
end
go

CREATE PROCEDURE [dbo].[poll_add]
    @name NVARCHAR(64),
    @text NVARCHAR(MAX),
    @multi_choice BIT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    INSERT INTO Poll (name, text, multi_choice, agenda_id)
    OUTPUT
        inserted.id
    VALUES
        (@name, @text, @multi_choice, NULL)
END
go

CREATE PROCEDURE [dbo].[poll_agenda_set]
    @poll_id INT,
    @agenda_id INT
AS
BEGIN
    UPDATE Poll
    SET
        agenda_id = @agenda_id
    WHERE
            id = @poll_id
END
go

CREATE PROCEDURE [dbo].[poll_by_agenda]
@agenda_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT id
    FROM Poll
    WHERE @agenda_id = agenda_id AND id <> 0
    ORDER BY id
END
go

CREATE PROCEDURE [dbo].[poll_delete]
@poll_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DELETE Poll
    WHERE @poll_id = id
END
go

CREATE PROCEDURE [dbo].[poll_get]
@poll_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT TOP(1) id, name, text, multi_choice, agenda_id
    FROM Poll
    WHERE id = @poll_id
END
go

CREATE PROCEDURE [dbo].[poll_list]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT id, name, text, multi_choice, agenda_id
    FROM Poll
    ORDER BY id
END
go

CREATE PROCEDURE [dbo].[poll_option_add]
    @text NVARCHAR(MAX),
    @poll_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    INSERT INTO PollOption (text, poll_id)
    OUTPUT
        inserted.id
    VALUES
        (@text, @poll_id)
END
go

CREATE PROCEDURE [dbo].[poll_option_list]
    @poll_id INT,
    @user_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here


    WITH Counter (id, cnt) AS (
        SELECT oc.option_id, COUNT(*)
        FROM OptionCheck oc
        GROUP BY oc.option_id
    ),
         Checked AS (
             SELECT oc.option_id FROM OptionCheck oc
             WHERE oc.user_id = @user_id
         )
    SELECT
        po.id
         , text
         , IIF(cr.cnt is NULL, 0, cr.cnt) as cnt
         , IIF(ch.option_id is NULL, 0, 1) as is_checked
    FROM PollOption po
             inner join dbo.OptionCheck oc ON po.id = oc.option_id
             left join Counter cr ON oc.option_id = cr.id
             left join Checked ch ON ch.option_id = po.id
    WHERE (@poll_id = 0 OR po.poll_id = @poll_id)
END
go

CREATE PROCEDURE [dbo].[poll_option_unvote]
    @option_id INT,
    @user_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    DELETE OptionCheck
    WHERE @user_id = user_id AND @option_id = option_id
END
go

CREATE PROCEDURE [dbo].[poll_option_vote]
    @option_id INT,
    @user_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    INSERT INTO OptionCheck (option_id, user_id)
    VALUES (@option_id, @user_id)
END
go

CREATE PROCEDURE [dbo].[poll_options]
    @poll_id INT,
    @user_id INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    WITH Checked (option_id, cnt) AS (
        SELECT oc.option_id, COUNT(*) FROM OptionCheck oc
                                               inner join PollOption po on oc.option_id = po.id
        WHERE poll_id = @poll_id
        GROUP BY oc.option_id
    ), CheckedByUser AS (
        SELECT oc.option_id FROM OptionCheck oc
                                     inner join PollOption po on oc.option_id = po.id
        WHERE poll_id = @poll_id AND oc.user_id = @user_id
    )
    SELECT po.id
         , po.text
         , IIF(c.cnt is NULL, 0, c.cnt) as cnt
         , CAST(IIF(cby.option_id is NULL, 0, 1) as BIT) as checked_by_user
    FROM PollOption po
             left join Checked c ON c.option_id = po.id
             left join CheckedByUser cby ON cby.option_id = po.id
    WHERE poll_id = @poll_id
    ORDER BY id
END
go

CREATE PROCEDURE [dbo].[role_by_user]
@id INT
AS
BEGIN
    SELECT ug.[group] FROM [UserGroup] ug
    WHERE user_id = @id
END
go

CREATE PROCEDURE [dbo].[speakers_get]
    -- Add the parameters for the stored procedure here
@agenda_id INT
AS
BEGIN
    SELECT u.id, first_name, last_name
    FROM [User] u
             inner join AgendaSpeaker ags ON ags.speaker_id = u.id
    WHERE ags.agenda_id = @agenda_id
END
go

CREATE PROCEDURE [dbo].[user_add]
    @id INT,
    @first_name nvarchar(64),
    @last_name nvarchar(64),
    @full_name nvarchar(140),
    @prefix NVARCHAR(10),
    @email NVARCHAR(64),
    @organization_name NVARCHAR(64),
    @application_status NVARCHAR(64),
    @is_active bit
AS
BEGIN
    INSERT INTO [User]
    (id, first_name, last_name, prefix, full_name, organization_name, email, is_active, application_status)
    OUTPUT
        inserted.id
    VALUES
        (@id
        , @first_name
        , @last_name
        , @prefix
        , @full_name
        , @organization_name
        , @email
        , @is_active
        , @application_status)
END
go

CREATE PROCEDURE [dbo].[user_baseinfo_list]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT id, first_name, last_name, prefix, full_name, organization_name, email, is_active, application_status FROM [User]
END
go

CREATE PROCEDURE [dbo].[user_by_token_get]
@token NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT username, '' as groups FROM dbo.BasicToken b
                                           inner join UserAuthData u ON b.user_id = u.user_id
    WHERE @token = token
END
go

CREATE PROCEDURE [dbo].[user_email_find]
@email NVARCHAR(64)
AS
BEGIN
    SELECT TOP(1) id FROM [User]
    WHERE email = @email
end
go

CREATE PROCEDURE [dbo].[user_get]
AS
BEGIN
    SELECT  id
         , first_name
         , last_name
         , prefix
         , full_name
         , organization_name
         , email
         , is_active
         , application_status
    FROM [User]
END
go

CREATE PROCEDURE [dbo].[user_get_by_id]
@id INT
AS
BEGIN
    SELECT TOP(1) id
                , first_name
                , last_name
                , prefix
                , full_name
                , organization_name
                , email
                , is_active
                , application_status
    FROM [User]
    WHERE @id = id
END
go

CREATE PROCEDURE user_in_chat
    @chat_id BIGINT,
    @user_id INT
AS
BEGIN
    SELECT IIF(COUNT(*) > 0, 1, 0) ok
    FROM ChatParticipants
    WHERE chat_id = @chat_id AND user_id = @user_id
end
go

CREATE PROCEDURE [dbo].[user_token_add]
    @user_id int,
    @token NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BasicToken
    (user_id, token) VALUEs (@user_id, @token)
END
go

CREATE PROCEDURE [dbo].[user_update]
    @id INT,
    @first_name nvarchar(64),
    @last_name nvarchar(64),
    @full_name nvarchar(140),
    @prefix NVARCHAR(10),
    @email NVARCHAR(64),
    @organization_name NVARCHAR(64),
    @application_status NVARCHAR(64),
    @is_active bit
AS
BEGIN
    UPDATE [User]
    SET
        first_name = @first_name,
        last_name = @last_name,
        prefix = @prefix,
        full_name = @full_name,
        organization_name = @organization_name,
        email = @email,
        is_active = @is_active,
        application_status = @application_status
    WHERE id = @id
END
go

create type OneInt as table
(
    intVal int
)
go
