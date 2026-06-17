IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [Hospitals] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [City] nvarchar(100) NULL,
        [Address] nvarchar(300) NULL,
        [Latitude] float NOT NULL,
        [Longitude] float NOT NULL,
        [IsKnown] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Hospitals] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Age] int NULL,
        [BloodType] int NULL,
        [Role] int NOT NULL,
        [Latitude] float NOT NULL,
        [Longitude] float NOT NULL,
        [IsAvailable] bit NOT NULL,
        [LastDonationDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [BloodRequests] (
        [Id] uniqueidentifier NOT NULL,
        [RequiredBloodType] int NOT NULL,
        [Urgency] int NOT NULL,
        [Status] int NOT NULL,
        [HospitalId] uniqueidentifier NULL,
        [CustomHospitalName] nvarchar(200) NULL,
        [Latitude] float NOT NULL,
        [Longitude] float NOT NULL,
        [MedicalDocumentUrl] nvarchar(500) NULL,
        [Notes] nvarchar(500) NULL,
        [ContactPhone] nvarchar(30) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [PatientId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BloodRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BloodRequests_Hospitals_HospitalId] FOREIGN KEY ([HospitalId]) REFERENCES [Hospitals] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BloodRequests_Users_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [Token] nvarchar(500) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsExpired] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [UserReports] (
        [Id] uniqueidentifier NOT NULL,
        [ReporterId] uniqueidentifier NOT NULL,
        [ReportedUserId] uniqueidentifier NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_UserReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserReports_Users_ReportedUserId] FOREIGN KEY ([ReportedUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserReports_Users_ReporterId] FOREIGN KEY ([ReporterId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [AiMatchingLogs] (
        [Id] uniqueidentifier NOT NULL,
        [BloodRequestId] uniqueidentifier NOT NULL,
        [PriorityResult] int NOT NULL,
        [MatchedDonorsCount] int NOT NULL,
        [NotificationsSent] int NOT NULL,
        [SearchRadiusKm] float NOT NULL,
        [UsedCompatibleBloodTypes] nvarchar(max) NOT NULL,
        [PipelineSummary] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AiMatchingLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AiMatchingLogs_BloodRequests_BloodRequestId] FOREIGN KEY ([BloodRequestId]) REFERENCES [BloodRequests] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [BloodRequestAcceptances] (
        [Id] uniqueidentifier NOT NULL,
        [BloodRequestId] uniqueidentifier NOT NULL,
        [DonorId] uniqueidentifier NOT NULL,
        [AcceptedAt] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        CONSTRAINT [PK_BloodRequestAcceptances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BloodRequestAcceptances_BloodRequests_BloodRequestId] FOREIGN KEY ([BloodRequestId]) REFERENCES [BloodRequests] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BloodRequestAcceptances_Users_DonorId] FOREIGN KEY ([DonorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [DonationHistories] (
        [Id] uniqueidentifier NOT NULL,
        [DonorId] uniqueidentifier NOT NULL,
        [PatientId] uniqueidentifier NOT NULL,
        [BloodRequestId] uniqueidentifier NULL,
        [HospitalName] nvarchar(200) NOT NULL,
        [DonationDate] datetime2 NOT NULL,
        [Notes] nvarchar(500) NULL,
        CONSTRAINT [PK_DonationHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DonationHistories_BloodRequests_BloodRequestId] FOREIGN KEY ([BloodRequestId]) REFERENCES [BloodRequests] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DonationHistories_Users_DonorId] FOREIGN KEY ([DonorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DonationHistories_Users_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(150) NOT NULL,
        [Message] nvarchar(500) NOT NULL,
        [Type] nvarchar(50) NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [BloodRequestId] uniqueidentifier NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_BloodRequests_BloodRequestId] FOREIGN KEY ([BloodRequestId]) REFERENCES [BloodRequests] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_AiMatchingLogs_BloodRequestId] ON [AiMatchingLogs] ([BloodRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BloodRequestAcceptances_BloodRequestId_DonorId] ON [BloodRequestAcceptances] ([BloodRequestId], [DonorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_BloodRequestAcceptances_DonorId] ON [BloodRequestAcceptances] ([DonorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_BloodRequests_HospitalId] ON [BloodRequests] ([HospitalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_BloodRequests_PatientId] ON [BloodRequests] ([PatientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_DonationHistories_BloodRequestId] ON [DonationHistories] ([BloodRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_DonationHistories_DonorId] ON [DonationHistories] ([DonorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_DonationHistories_PatientId] ON [DonationHistories] ([PatientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_Notifications_BloodRequestId] ON [Notifications] ([BloodRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_UserReports_ReportedUserId] ON [UserReports] ([ReportedUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE INDEX [IX_UserReports_ReporterId] ON [UserReports] ([ReporterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602235048_AddAuthAndIdentityChanges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602235048_AddAuthAndIdentityChanges', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [BloodRequests] DROP CONSTRAINT [FK_BloodRequests_Users_PatientId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    EXEC sp_rename N'[BloodRequests].[PatientId]', N'UserId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    EXEC sp_rename N'[BloodRequests].[IX_BloodRequests_PatientId]', N'IX_BloodRequests_UserId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'IsKnown');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Hospitals] ADD DEFAULT CAST(1 AS bit) FOR [IsKnown];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'CreatedAt');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Hospitals] ADD DEFAULT (GETUTCDATE()) FOR [CreatedAt];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [Email] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [Phone] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [UserId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    DROP INDEX [IX_BloodRequests_HospitalId] ON [BloodRequests];
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BloodRequests]') AND [c].[name] = N'HospitalId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [BloodRequests] DROP CONSTRAINT [' + @var2 + '];');
    EXEC(N'UPDATE [BloodRequests] SET [HospitalId] = ''00000000-0000-0000-0000-000000000000'' WHERE [HospitalId] IS NULL');
    ALTER TABLE [BloodRequests] ALTER COLUMN [HospitalId] uniqueidentifier NOT NULL;
    ALTER TABLE [BloodRequests] ADD DEFAULT '00000000-0000-0000-0000-000000000000' FOR [HospitalId];
    CREATE INDEX [IX_BloodRequests_HospitalId] ON [BloodRequests] ([HospitalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [UnitsNeeded] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Hospitals_UserId] ON [Hospitals] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD CONSTRAINT [FK_BloodRequests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    ALTER TABLE [Hospitals] ADD CONSTRAINT [FK_Hospitals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607004906_AddUnitsNeededToBloodRequests'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607004906_AddUnitsNeededToBloodRequests', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'Address');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Hospitals] DROP COLUMN [Address];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'IsKnown');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Hospitals] DROP COLUMN [IsKnown];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'Phone');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Hospitals] DROP COLUMN [Phone];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    EXEC sp_rename N'[Hospitals].[Email]', N'LicenseDocumentPath', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Longitude');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Longitude] float NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Latitude');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Latitude] float NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'City');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var8 + '];');
    EXEC(N'UPDATE [Hospitals] SET [City] = N'''' WHERE [City] IS NULL');
    ALTER TABLE [Hospitals] ALTER COLUMN [City] nvarchar(100) NOT NULL;
    ALTER TABLE [Hospitals] ADD DEFAULT N'' FOR [City];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [AddressDetail] nvarchar(300) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [Government] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [Hotline] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607015634_AddHospitalTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607015634_AddHospitalTable', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607133345_addAdmin'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607133345_addAdmin', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] DROP CONSTRAINT [FK_BloodRequests_Users_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    EXEC sp_rename N'[BloodRequests].[UserId]', N'CreatedByUserId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    EXEC sp_rename N'[BloodRequests].[IX_BloodRequests_UserId]', N'IX_BloodRequests_CreatedByUserId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Phone');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Users] ALTER COLUMN [Phone] nvarchar(11) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'LicenseDocumentPath');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Hospitals] ALTER COLUMN [LicenseDocumentPath] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Hospitals]') AND [c].[name] = N'Hotline');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Hospitals] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Hospitals] ALTER COLUMN [Hotline] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BloodRequests]') AND [c].[name] = N'HospitalId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [BloodRequests] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [BloodRequests] ALTER COLUMN [HospitalId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BloodRequests]') AND [c].[name] = N'CreatedAt');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [BloodRequests] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [BloodRequests] ADD DEFAULT (GETUTCDATE()) FOR [CreatedAt];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [ApprovedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [ApprovedByHospitalId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [PatientAge] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [PatientName] nvarchar(150) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD [RejectionReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    CREATE TABLE [OcrVerifications] (
        [Id] uniqueidentifier NOT NULL,
        [BloodRequestId] uniqueidentifier NOT NULL,
        [IsVerified] bit NOT NULL,
        [ConfidenceScore] float NOT NULL,
        [RawExtractedText] nvarchar(max) NULL,
        [FailureReason] nvarchar(500) NULL,
        [VerifiedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_OcrVerifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OcrVerifications_BloodRequests_BloodRequestId] FOREIGN KEY ([BloodRequestId]) REFERENCES [BloodRequests] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Hospitals_Name_City] ON [Hospitals] ([Name], [City]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OcrVerifications_BloodRequestId] ON [OcrVerifications] ([BloodRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    ALTER TABLE [BloodRequests] ADD CONSTRAINT [FK_BloodRequests_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609025541_EditBllodRequest'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609025541_EditBllodRequest', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615225549_editeOcrVerfication'
)
BEGIN
    ALTER TABLE [OcrVerifications] ADD [ExtractedBloodType] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615225549_editeOcrVerfication'
)
BEGIN
    ALTER TABLE [OcrVerifications] ADD [ExtractedUnits] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615225549_editeOcrVerfication'
)
BEGIN
    ALTER TABLE [OcrVerifications] ADD [ExtractedUrgency] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260615225549_editeOcrVerfication'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260615225549_editeOcrVerfication', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616102925_Last'
)
BEGIN

                                IF COL_LENGTH('Hospitals', 'RejectionReason') IS NOT NULL
                                BEGIN
                                    ALTER TABLE [Hospitals] DROP COLUMN [RejectionReason];
                                END
                                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616102925_Last'
)
BEGIN

                                IF COL_LENGTH('Hospitals', 'ReviewedAt') IS NOT NULL
                                BEGIN
                                    ALTER TABLE [Hospitals] DROP COLUMN [ReviewedAt];
                                END
                                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616102925_Last'
)
BEGIN

                                IF COL_LENGTH('Hospitals', 'Status') IS NOT NULL
                                BEGIN
                                    ALTER TABLE [Hospitals] DROP COLUMN [Status];
                                END
                                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616102925_Last'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616102925_Last', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616235751_AddHospitalApprovalFields'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [RejectionReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616235751_AddHospitalApprovalFields'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [ReviewedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616235751_AddHospitalApprovalFields'
)
BEGIN
    ALTER TABLE [Hospitals] ADD [Status] int NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616235751_AddHospitalApprovalFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616235751_AddHospitalApprovalFields', N'8.0.11');
END;
GO

COMMIT;
GO

